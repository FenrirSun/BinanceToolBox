namespace M3C.Finance.BinanceSdk.Enumerations
{
    public class EventStatus
    {
        private EventStatus(string value) { Value = value; }

        public string Value { get; }

        // 新建订单
        public static EventStatus New => new EventStatus("NEW");
        // 已撤销
        public static EventStatus Canceled => new EventStatus("CANCELED");

        public static EventStatus Calculated => new EventStatus("CALCULATED");
        //  订单失效
        public static EventStatus Expired => new EventStatus("EXPIRED");
        //  交易
        public static EventStatus Trade => new EventStatus("TRADE");
        
        public static implicit operator string(EventStatus EventStatus) => EventStatus.Value;
        public static implicit operator EventStatus(string text) => new EventStatus(text);
        public override string ToString() => Value;
    }
}