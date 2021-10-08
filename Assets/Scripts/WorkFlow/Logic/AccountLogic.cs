using System;
using System.Collections.Generic;
using System.Linq;
using DigitalRubyShared;
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
                client?.CancelOrder(evt.symbol, evt.orderId);
            }
        });

        GetEventComp().Listen<CancelOrderByClientId>((evt) =>
        {
            if (accountClientList.ContainsKey(evt.data)) {
                var client = accountClientList[evt.data];
                client?.CancelOrder(evt.symbol, originalClientOrderId: evt.clientOrderId);
            }
        });
        
        GetEventComp().Listen<GetOrder>(GetOrderInfo);
        
        GetEventComp().Listen<GetCurrentOrders>(GetCurrentOpenOrderInfo);
        
        GetEventComp().Listen<GetAllOrders>(GetAllOrderInfo);
        
        GetEventComp().Listen<UpdateAccountInfo>((evt) =>
        {
            UpdateAccount(evt.data);
        });
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
            wsClient?.ConnectUserDataEndpoint(client, OnGetAccountUpdateMessage, OnGetOrderTradeUpdateMessage, OnGetConfigUpdateMessage);
            accountWsClientList[account] = wsClient;
        }
    }

    private async void UpdateAccount(AccountData account) {
        if (accountClientList.ContainsKey(account)) {
            var client = accountClientList[account];
            if (client != null) {
                var resultBalance = await client.GetBalanceInfo();
                var resultAccount = await client.GetAccountInfo();
            }
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

    public void OnUpdateAccountInfo(UpdateAccountInfo msg) {
        if (accountClientList.ContainsKey(msg.data)) {
           
        }
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
                var callBack = evt.onOrderSendSuccess;
                evt.onOrderSendSuccess = null;
                callBack?.Invoke(null);
            } catch (Exception e) {
                evt.onOrderSendSuccess?.Invoke(e);
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
    
    private async void GetCurrentOpenOrderInfo(GetCurrentOrders evt) {
        if (accountClientList.ContainsKey(evt.data)) {
            var client = accountClientList[evt.data];
            IEnumerable<OrderInfo> response = await client.CurrentOpenOrders(evt.symbol);
            if (response != null) {
                var ad = evt.data;
                List<FuturesUserDataOpenOrderInfoMessage> orderInfos = ad.GetOrderInfos();
                SaveOrderInfo(orderInfos, response, evt.symbol);
            }
        }
    }

    private async void GetAllOrderInfo(GetAllOrders evt) {
        if (accountClientList.ContainsKey(evt.data)) {
            var client = accountClientList[evt.data];
            IEnumerable<OrderInfo> response = await client.AllOrders(evt.symbol);
            if (response != null) {
                var ad = evt.data;
                List<FuturesUserDataOpenOrderInfoMessage> orderInfos = ad.GetOrderInfos();
                SaveOrderInfo(orderInfos, response, evt.symbol);
            }
        }
    }
    
    private void SaveOrderInfo(List<FuturesUserDataOpenOrderInfoMessage> orderInfos, IEnumerable<OrderInfo> response, SymbolType symbolType) {
        for (int i = orderInfos.Count - 1; i >= 0; --i) {
            var info = orderInfos[i];
            if (info.symbol == symbolType) {
                orderInfos.RemoveAt(i);
            }
        }

        foreach (var orderInfo in response) {
            var newInfoMsg = new FuturesUserDataOpenOrderInfoMessage();
            newInfoMsg.orderId = orderInfo.OrderId;
            newInfoMsg.price = orderInfo.Price;
            newInfoMsg.side = orderInfo.OrderSide;
            newInfoMsg.positionSide = orderInfo.PositionSide;
            newInfoMsg.stopPrice = orderInfo.StopPrice;
            newInfoMsg.symbol = orderInfo.Symbol;
            newInfoMsg.status = orderInfo.Status;
            newInfoMsg.time = orderInfo.Time;
            newInfoMsg.type = orderInfo.OrderType;
            newInfoMsg.reduceOnly = orderInfo.reduceOnly == "true";
            newInfoMsg.origQty = orderInfo.OriginalQuantity;
            newInfoMsg.executedQty = orderInfo.ExecutedQuantity;
            newInfoMsg.clientOrderId = orderInfo.ClientOrderId;
            orderInfos.Add(newInfoMsg);
        }
                
        orderInfos.Sort((a, b) => (int) (b.time - a.time));    
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
                    var adTrades = ad.GetPositionInfos();
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