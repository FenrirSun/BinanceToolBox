using System.Collections.Generic;
using GameEvents;
using M3C.Finance.BinanceSdk;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class AccountLogic : LogicBase
{
    private Dictionary<AccountData, BinanceFuturesClient>  accountClientList;
    private Dictionary<AccountData, WebSocketClient_FuturesSigned>  accountWsClientList;
    public Queue<GameEvent> eventList;
    
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
        GetEventComp().Listen<NewOrder>((evt) =>
        {
            if (accountClientList.ContainsKey(evt.data)) {
                var client = accountClientList[evt.data];
                client.NewOrder(evt.orderInfo.Symbol, evt.orderInfo.OrderSide, evt.orderInfo.PositionSide, evt.orderInfo.OrderType,
                    TimeInForce.GoodUntilCanceled, evt.orderInfo.OriginalQuantity, evt.orderInfo.Price, evt.orderInfo.ClientOrderId.ToString());
            }
        });
    }
    
    public async void AddAccount(AccountData account) {
        if (!accountClientList.ContainsKey(account)) {
            var client = new BinanceFuturesClient(account);
            accountClientList[account] = client;
            var time = await client.Time();
            Utilities.OnGetServerTime(time.ServerTime);
            var result = await client.GetBalanceInfo();
            
            var wsClient = new WebSocketClient_FuturesSigned();
            wsClient.ConnectUserDataEndpoint(client, OnGetAccountUpdateMessage, OnGetOrderTradeUpdateMessage, OnGetConfigUpdateMessage);
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
        }
    }
    
    private void OnGetOrderTradeUpdateMessage(WsFuturesUserDataOrderTradeUpdateMessage e, AccountData ad) {
        var orderInfos = ad.GetOrderInfos();
        if (orderInfos.Count > 30) {
            orderInfos.RemoveAt(0);
        }

        foreach (var order in orderInfos) {
            if (order.clientOrderId == e.OrderInfo.ClientId) {
                e.SyncOrderInfoData(order);
            }
        }
        GetEventComp().Send(OnOrderInfoUpdate.Create(e));
        orderInfos.Add(e.ConvertToOrderInfoMessage());
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
    }

    private void OnDestroy() {
        foreach (var client in accountWsClientList.Values) {
            client?.Dispose();
        }
    }
}