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

        public static OnOrderInfoUpdate Create(WsFuturesUserDataOrderTradeUpdateMessage _msg) {
            var result = new OnOrderInfoUpdate();
            result.msg = _msg;
            return result;
        }
    }

    public class StartStrategy : GameEventBaseNoDefaultCreate<StartStrategy>
    {
        public SymbolType Symbol;
        public StrategyBase Strategy;

        public static StartStrategy Create(SymbolType _symbol, StrategyBase _strategy) {
            var result = new StartStrategy();
            result.Symbol = _symbol;
            result.Strategy = _strategy;
            return result;
        }
    }
    
    public class NewOrder : GameEventBaseNoDefaultCreate<NewOrder>
    {
        public AccountData data;
        public StrategyOrderInfo orderInfo;

        public static NewOrder Create(AccountData account, StrategyOrderInfo order) {
            var result = new NewOrder();
            result.data = account;
            result.orderInfo = order;
            return result;
        }
    }
    
    #endregion
    
    
    public class RefreshAccountList : GameEventBase<RefreshAccountList> { }
}