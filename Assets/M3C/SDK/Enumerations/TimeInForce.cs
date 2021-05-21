namespace M3C.Finance.BinanceSdk.Enumerations
{
    public class TimeInForce
    {
        private TimeInForce(string value) { Value = value; }

        private string Value { get; }

        // - Good Till Cancel 成交为止
        public static TimeInForce GoodUntilCanceled => new TimeInForce("GTC");
        // - Immediate or Cancel 无法立即成交(吃单)的部分就撤销
        public static TimeInForce ImmediateOrCancel => new TimeInForce("IOC");
        // - Fill or Kill 无法全部立即成交就撤销
        public static TimeInForce FillOrKill => new TimeInForce("FOK");
        // - Good Till Crossing 无法成为挂单方就撤销
        public static TimeInForce GoodTillCrossing => new TimeInForce("GTX");
        
        public static implicit operator string(TimeInForce timeInForce) => timeInForce.Value;
        public static implicit operator TimeInForce(string text) => new TimeInForce(text);
        public override string ToString() => Value;
    }
}
