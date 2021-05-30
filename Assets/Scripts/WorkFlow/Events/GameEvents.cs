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

    #endregion

    #region Signed Message

    public class OnOrderInfoUpdate : GameEventBaseNoDefaultCreate<OnOrderInfoUpdate>
    {
        public WsFuturesUserDataOrderTradeUpdateMessage msg;
        public AccountData ad;
        
        public static OnOrderInfoUpdate Create(WsFuturesUserDataOrderTradeUpdateMessage _msg, AccountData _ad) {
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

        public static NewOrder Create(AccountData account, OrderInfo order) {
            var result = new NewOrder();
            result.data = account;
            result.orderInfo = order;
            return result;
        }
    }
    
    #endregion
    
    
    public class RefreshAccountList : GameEventBase<RefreshAccountList> { }
}