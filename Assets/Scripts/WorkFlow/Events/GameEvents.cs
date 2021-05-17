using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

namespace GameEvents
{
    public class GameStart : GameEventBase<GameStart>
    {
    }

    public class ListenTradesMessage : GameEventBaseNoDefaultCreate<ListenTradesMessage>
    {
        public SymbolType symbol;

        public static ListenTradesMessage Create(SymbolType symbol) {
            var result = new ListenTradesMessage();
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
}