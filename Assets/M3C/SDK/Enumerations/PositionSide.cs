namespace M3C.Finance.BinanceSdk.Enumerations
{
    public class PositionSide
    {
        private PositionSide(string value) { Value = value; }

        public string Value { get; }

        public static PositionSide BOTH => new PositionSide("BOTH");
        public static PositionSide LONG => new PositionSide("LONG");
        public static PositionSide SHORT => new PositionSide("SHORT");

        public static implicit operator string(PositionSide side) => side.Value;
        public static implicit operator PositionSide(string text) => new PositionSide(text);
        public override string ToString() => Value;
    }
}
