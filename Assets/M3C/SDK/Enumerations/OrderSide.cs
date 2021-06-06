using System;

namespace M3C.Finance.BinanceSdk.Enumerations
{
    [Serializable]
    public class OrderSide
    {
        private OrderSide(string value) { Value = value; }

        public string Value { get; }

        // 开仓
        public static OrderSide Buy => new OrderSide("BUY");
        // 平仓
        public static OrderSide Sell => new OrderSide("SELL");

        public static implicit operator string(OrderSide side) => side.Value;
        public static implicit operator OrderSide(string text) => new OrderSide(text);
        public override string ToString() => Value;
        
        public static bool operator ==(OrderSide one, OrderSide two) {
            return one?.Value == two?.Value;
        }
        
        public static bool operator !=(OrderSide one, OrderSide two) {
            return one?.Value != two?.Value;
        }
    }
}
