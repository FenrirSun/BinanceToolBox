using System;

namespace M3C.Finance.BinanceSdk.Enumerations
{
    [Serializable]
    public class OrderType
    {
        public OrderType(string value) { Value = value; }

        private string Value { get; }

        // 限价单
        public static OrderType Limit => new OrderType("LIMIT");
        // 市价单
        public static OrderType Market => new OrderType("MARKET");
        // 止损限价单
        public static OrderType STOP => new OrderType("STOP");
        // 止损市价单
        public static OrderType STOP_MARKET => new OrderType("STOP_MARKET");
        // 止盈限价单
        public static OrderType TAKE_PROFIT => new OrderType("TAKE_PROFIT");
        // 止盈市价单
        public static OrderType TAKE_PROFIT_MARKET => new OrderType("TAKE_PROFIT_MARKET");
        // 跟踪止损单
        public static OrderType TRAILING_STOP_MARKET => new OrderType("TRAILING_STOP_MARKET");

        public static implicit operator string(OrderType orderType) => orderType.Value;
        public static implicit operator OrderType(string text) => new OrderType(text);
        public override string ToString() => Value;
        
        public static bool operator ==(OrderType one, OrderType two) {
            return one?.Value == two?.Value;
        }
        
        public static bool operator !=(OrderType one, OrderType two) {
            return one?.Value != two?.Value;
        }
    }
}
