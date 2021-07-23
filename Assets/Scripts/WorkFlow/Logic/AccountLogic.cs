using System;
using System.Collections.Generic;
using System.Linq;
using GameEvents;
using M3C.Finance.BinanceSdk;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;
using UnityEngine;

public class AccountLogic : LogicBase
{
    private Dictionary<AccountData, BinanceFuturesClient> accountClientList;
    private Dictionary<AccountData, WebSocketClient_FuturesSigned> accountWsClientList;
    private Queue<GameEvent> eventList;

    protected override void Awake() {
        accountClientList = new Dictionary<AccountData, BinanceFuturesClient>();
        accountWsClientList = new Dictionary<AccountData, WebSocketClient_FuturesSigned>();
        eventList = new Queue<GameEvent>();
        InitAccount();
        AddListeners();
    }

    private void InitAccount() {
        var accounts = GameRuntime.Instance.UserData.accountDataList;
        foreach (var accountData in accounts) {
            AddAccount(accountData);
        }
    }

    private void AddListeners() {
        GetEventComp().Listen<NewOrder>(SendNewOrder);

        GetEventComp().Listen<CancelOrder>((evt) =>
        {
            if (accountClientList.ContainsKey(evt.data)) {
                var client = accountClientList[evt.data];
                client.CancelOrder(evt.symbol, evt.orderId);
            }
        });

        GetEventComp().Listen<CancelOrderByClientId>((evt) =>
        {
            if (accountClientList.ContainsKey(evt.data)) {
                var client = accountClientList[evt.data];
                client.CancelOrder(evt.symbol, originalClientOrderId: evt.clientOrderId);
            }
        });

        GetEventComp().Listen<GetOrder>(GetOrderInfo);
    }

    public async void AddAccount(AccountData account) {
        if (!accountClientList.ContainsKey(account)) {
            var client = new BinanceFuturesClient(account);
            accountClientList[account] = client;
            var time = await client.Time();
            if (time == null)
                return;
            Utilities.OnGetServerTime(time.ServerTime);
            var resultBalance = await client.GetBalanceInfo();
            var resultAccount = await client.GetAccountInfo();

            var wsClient = new WebSocketClient_FuturesSigned();
            wsClient.ConnectUserDataEndpoint(client, OnGetAccountUpdateMessage, OnGetOrderTradeUpdateMessage, OnGetConfigUpdateMessage);
            accountWsClientList[account] = wsClient;
        }
    }

    public void DeleteAccount(AccountData account) {
        if (accountClientList.ContainsKey(account)) {
            accountClientList.Remove(account);
        }

        if (accountWsClientList.ContainsKey(account)) {
            accountWsClientList[account].Dispose();
            accountWsClientList.Remove(account);
        }

        GameRuntime.Instance.UserData.RemoveAccount(account.id);
    }

    public List<AccountData> GetAccounts() {
        return accountClientList.Keys.ToList();
    }

    private async void SendNewOrder(NewOrder evt) {
        if (accountClientList.ContainsKey(evt.data)) {
            var client = accountClientList[evt.data];
            try {
                var result = await client.NewOrder(evt.orderInfo.Symbol, evt.orderInfo.OrderSide, evt.orderInfo.PositionSide,
                    evt.orderInfo.OrderType, TimeInForce.GoodUntilCanceled, evt.orderInfo.OriginalQuantity, evt.orderInfo.Price,
                    evt.orderInfo.ClientOrderId.ToString(), stopPrice: evt.orderInfo.StopPrice);
                evt.onOrderSendSuccess(true);
            } catch (Exception e) {
                evt.onOrderSendSuccess(false);
            }
        }
    }

    private async void GetOrderInfo(GetOrder evt) {
        if (accountClientList.ContainsKey(evt.data)) {
            var client = accountClientList[evt.data];
            var response = await client.GetOrder(evt.symbol, evt.clientOrderId);
            if (response != null) {
                GetEventComp().Send(OnOrderInfoUpdate.Create(response, evt.data));
            }
        }
    }
    
    private void OnGetAccountUpdateMessage(WsFuturesUserDataAccountUpdateMessage e, AccountData ad) {
        if (e.AccountUpdateInfo != null) {
            var balanceInfo = e.AccountUpdateInfo.BalanceInfo;
            if (balanceInfo != null && balanceInfo.Count > 0) {
                foreach (var info in balanceInfo) {
                    var adBalance = ad.GetBalanceInfo();
                    if (info.CurrencyName == CurrencyType.USDT && adBalance != null) {
                        adBalance.Balance = info.WalletRemain;
                    }
                }
            }

            var tradeInfos = e.AccountUpdateInfo.TradeInfo;
            if (tradeInfos != null && tradeInfos.Count > 0) {
                foreach (var trade in tradeInfos) {
                    var adTrades = ad.GetTradeInfos();
                    bool isUpdate = false;
                    for (int i = 0; i < adTrades.Count; ++i) {
                        if (adTrades[i].symbol.Value == trade.Symbol && adTrades[i].positionSide.Value == trade.PositionSide) {
                            trade.SyncPositionInfoData(adTrades[i]);
                            isUpdate = true;
                            break;
                        }
                    }

                    if (!isUpdate) {
                        adTrades.Add(trade.ConvertToUserDataPositionInfo());
                    }
                }
            }
        }
    }

    private void OnGetOrderTradeUpdateMessage(WsFuturesUserDataOrderTradeUpdateMessage e, AccountData ad) {
        var orderInfos = ad.GetOrderInfos();
        while (orderInfos.Count > 30) {
            orderInfos.RemoveAt(orderInfos.Count - 1);
        }

        bool syncOrder = false;
        foreach (var order in orderInfos) {
            if (order.clientOrderId == e.OrderInfo.ClientId) {
                e.SyncOrderInfoData(order);
                syncOrder = true;
            }
        }

        if (!syncOrder) {
            orderInfos.Add(e.ConvertToOrderInfoMessage());
        }

        orderInfos.Sort((a, b) => (int) (b.time - a.time));
        eventList.Enqueue(OnWsOrderInfoUpdate.Create(e, ad));
    }

    private void OnGetConfigUpdateMessage(WsFuturesUserDataAccountConfigUpdateMessage e, AccountData ad) {
    }

    private void Update() {
        if (eventList != null) {
            while (eventList.Count > 0) {
                var e = eventList.Dequeue();
                GetEventComp().Send(e);
            }
        }

        if (accountWsClientList != null && accountWsClientList.Count > 0) {
            foreach (var wsClient in accountWsClientList.Values) {
                wsClient?.Update();
            }
        }
    }

    private void OnDestroy() {
        foreach (var client in accountWsClientList.Values) {
            client?.Dispose();
        }
    }
}