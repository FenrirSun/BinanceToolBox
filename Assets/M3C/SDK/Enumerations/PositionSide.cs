using System;

namespace M3C.Finance.BinanceSdk.Enumerations
{
    [Serializable]
    public class PositionSide
    {
        public PositionSide(string value) { Value = value; }

        public string Value { get; }

        public static PositionSide BOTH => new PositionSide("BOTH");
        // 买多
        public static PositionSide LONG => new PositionSide("LONG");
        // 买空
        public static PositionSide SHORT => new PositionSide("SHORT");

        public static implicit operator string(PositionSide side) => side.Value;
        public static implicit operator PositionSide(string text) => new PositionSide(text);
        public override string ToString() => Value;
        
        public static bool operator ==(PositionSide one, PositionSide two) {
            return one?.Value == two?.Value;
        }
        
        public static bool operator !=(PositionSide one, PositionSide two) {
            return one?.Value != two?.Value;
        }
    }
}
