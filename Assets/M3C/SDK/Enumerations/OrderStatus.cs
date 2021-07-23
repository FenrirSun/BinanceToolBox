namespace M3C.Finance.BinanceSdk.Enumerations
{
    // 订单状态
    public class OrderStatus
    {
        public OrderStatus(string value) { Value = value; }

        public string Value { get; }

        // 新建订单
        public static OrderStatus New => new OrderStatus("NEW");
        // 部分成交
        public static OrderStatus PartiallyFilled => new OrderStatus("PARTIALLY_FILLED");
        // 全部成交
        public static OrderStatus Filled => new OrderStatus("FILLED");
        // 已撤销
        public static OrderStatus Canceled => new OrderStatus("CANCELED");
        // 订单过期(根据timeInForce参数规则)
        public static OrderStatus Expired => new OrderStatus("EXPIRED");
        //  风险保障基金(强平)
        public static OrderStatus NEW_INSURANCE => new OrderStatus("NEW_INSURANCE");
        // 自动减仓序列(强平)
        public static OrderStatus NEW_ADL => new OrderStatus("NEW_ADL");
        
        public static implicit operator string(OrderStatus orderStatus) => orderStatus.Value;
        public static implicit operator OrderStatus(string text) => new OrderStatus(text);
        public override string ToString() => Value;
        
        public static bool operator ==(OrderStatus one, OrderStatus two) {
            return one?.Value == two?.Value;
        }
        
        public static bool operator !=(OrderStatus one, OrderStatus two) {
            return one?.Value != two?.Value;
        }
    }
}
