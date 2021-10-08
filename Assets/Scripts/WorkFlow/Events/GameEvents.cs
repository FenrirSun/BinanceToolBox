using System;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

namespace GameEvents
{

    #region public Message

    public class GetLastTradeMessage : GameEventBaseNoDefaultCreate<GetLastTradeMessage>
    {
        public SymbolType symbol;
        public WebSocketTradesMessage message;
        
        public static GetLastTradeMessage Create(SymbolType symbol) {
            var result = new GetLastTradeMessage();
            result.symbol = symbol;
            return result;
        }
    }
    
    public class SubscribeKLine : GameEventBaseNoDefaultCreate<SubscribeKLine>
    {
        public SymbolType symbol;
        
        public static SubscribeKLine Create(SymbolType symbol) {
            var result = new SubscribeKLine();
            result.symbol = symbol;
            return result;
        }
    }

    public class OnAggTradeUpdate : GameEventBaseNoDefaultCreate<OnAggTradeUpdate>
    {
        public WebSocketTradesMessage msg;

        public static OnAggTradeUpdate Create(WebSocketTradesMessage _msg) {
            var result = new OnAggTradeUpdate();
            result.msg = _msg;
            return result;
        }
    }
    
    public class OnKlineUpdate : GameEventBaseNoDefaultCreate<OnKlineUpdate>
    {
        public WebSocketKlineMessage msg;

        public static OnKlineUpdate Create(WebSocketKlineMessage _msg) {
            var result = new OnKlineUpdate();
            result.msg = _msg;
            return result;
        }
    }

    // 断开连接
    public class OnDisconnect : GameEventBase<OnDisconnect> { }
    // 重新连接
    public class OnReconnect : GameEventBase<OnReconnect> { }
    
    public class RefreshAccountList : GameEventBase<RefreshAccountList> { }
    
    #endregion

    #region Signed Message

    public class OnWsOrderInfoUpdate : GameEventBaseNoDefaultCreate<OnWsOrderInfoUpdate>
    {
        public WsFuturesUserDataOrderTradeUpdateMessage msg;
        public AccountData ad;
        
        public static OnWsOrderInfoUpdate Create(WsFuturesUserDataOrderTradeUpdateMessage _msg, AccountData _ad) {
            var result = new OnWsOrderInfoUpdate();
            result.msg = _msg;
            result.ad = _ad;
            return result;
        }
    }

    public class OnOrderInfoUpdate : GameEventBaseNoDefaultCreate<OnOrderInfoUpdate>
    {
        public GetOrderResponse msg;
        public AccountData ad;
        
        public static OnOrderInfoUpdate Create(GetOrderResponse _msg, AccountData _ad) {
            var result = new OnOrderInfoUpdate();
            result.msg = _msg;
            result.ad = _ad;
            return result;
        }
    }
    
    public class StartStrategyEvent : GameEventBaseNoDefaultCreate<StartStrategyEvent>
    {
        public SymbolType Symbol;
        public StrategyBase Strategy;

        public static StartStrategyEvent Create(SymbolType _symbol, StrategyBase _strategy) {
            var result = new StartStrategyEvent();
            result.Symbol = _symbol;
            result.Strategy = _strategy;
            return result;
        }
    }
    
    public class AfterStartStrategyEvent : GameEventBaseNoDefaultCreate<AfterStartStrategyEvent>
    {
        public StrategyBase Strategy;
        public static AfterStartStrategyEvent Create(StrategyBase _strategy) {
            var result = new AfterStartStrategyEvent();
            result.Strategy = _strategy;
            return result;
        }
    }
    
    public class StopStrategyEvent : GameEventBaseNoDefaultCreate<StopStrategyEvent>
    {
        public StrategyBase Strategy;

        public static StopStrategyEvent Create(StrategyBase _strategy) {
            var result = new StopStrategyEvent();
            result.Strategy = _strategy;
            return result;
        }
    }
    
    public class NewOrder : GameEventBaseNoDefaultCreate<NewOrder>
    {
        public AccountData data;
        public OrderInfo orderInfo;
        public Action<Exception> onOrderSendSuccess;

        public static NewOrder Create(AccountData account, OrderInfo order, Action<Exception> callback) {
            var result = new NewOrder();
            result.data = account;
            result.orderInfo = order;
            result.onOrderSendSuccess = callback;
            return result;
        }
    }
    
    public class CancelOrder : GameEventBaseNoDefaultCreate<CancelOrder>
    {
        public AccountData data;
        public SymbolType symbol;
        public long orderId;

        public static CancelOrder Create(AccountData data, SymbolType symbol, long _id) {
            var result = new CancelOrder();
            result.data = data;
            result.symbol = symbol;
            result.orderId = _id;
            return result;
        }
    }
    
    public class GetOrder : GameEventBaseNoDefaultCreate<GetOrder>
    {
        public AccountData data;
        public SymbolType symbol;
        public string clientOrderId;
        
        public static GetOrder Create(AccountData data, SymbolType symbol, string clientIOrderId) {
            var result = new GetOrder();
            result.data = data;
            result.symbol = symbol;
            result.clientOrderId = clientIOrderId;
            return result;
        }
    }
    
    public class GetCurrentOrders : GameEventBaseNoDefaultCreate<GetCurrentOrders>
    {
        public AccountData data;
        public SymbolType symbol;
        
        public static GetCurrentOrders Create(AccountData data, SymbolType symbol) {
            var result = new GetCurrentOrders();
            result.data = data;
            result.symbol = symbol;
            return result;
        }
    }
    
    public class GetAllOrders : GameEventBaseNoDefaultCreate<GetAllOrders>
    {
        public AccountData data;
        public SymbolType symbol;
        
        public static GetAllOrders Create(AccountData data, SymbolType symbol) {
            var result = new GetAllOrders();
            result.data = data;
            result.symbol = symbol;
            return result;
        }
    }
    
    public class CancelOrderByClientId : GameEventBaseNoDefaultCreate<CancelOrderByClientId>
    {
        public AccountData data;
        public SymbolType symbol;
        public string clientOrderId;

        public static CancelOrderByClientId Create(AccountData data, SymbolType symbol, string clientId) {
            var result = new CancelOrderByClientId();
            result.data = data;
            result.symbol = symbol;
            result.clientOrderId = clientId;
            return result;
        }
    }
    
    public class UpdateAccountInfo : GameEventBaseNoDefaultCreate<UpdateAccountInfo>
    {
        public AccountData data;
        
        public static UpdateAccountInfo Create(AccountData data) {
            var result = new UpdateAccountInfo();
            result.data = data;
            return result;
        }
    }
    
    #endregion
    
}