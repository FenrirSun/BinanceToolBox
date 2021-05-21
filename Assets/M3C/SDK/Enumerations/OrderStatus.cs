namespace M3C.Finance.BinanceSdk.Enumerations
{
    public class OrderStatus
    {
        private OrderStatus(string value) { Value = value; }

        public string Value { get; }

        // 新建订单
        public static OrderStatus New => new OrderStatus("NEW");
        // 部分成交
        public static OrderStatus PartiallyFilled => new OrderStatus("PARTIALLY_FILLED");
        // 全部成交
        public static OrderStatus Filled => new OrderStatus("FILLED");
        // 已撤销
        public static OrderStatus Canceled => new OrderStatus("CANCELED");
        public static OrderStatus PendingCancel => new OrderStatus("PENDING_CANCEL");
        // 订单被拒绝
        public static OrderStatus Rejected => new OrderStatus("REJECTED");
        // 订单过期(根据timeInForce参数规则)
        public static OrderStatus Expired => new OrderStatus("EXPIRED");
        
        public static implicit operator string(OrderStatus orderStatus) => orderStatus.Value;
        public static implicit operator OrderStatus(string text) => new OrderStatus(text);
        public override string ToString() => Value;
    }
}
