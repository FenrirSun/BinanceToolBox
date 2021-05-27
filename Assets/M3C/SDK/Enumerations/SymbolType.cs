using System.Collections.Generic;

namespace M3C.Finance.BinanceSdk.Enumerations
{
    public class SymbolType
    {
        private SymbolType(string value) {
            Value = value;
        }

        private string Value { get; }

        public static List<SymbolType> Types = new List<SymbolType>
        {
            BTC, ETH, ETC, XRP, DOGE, SHIB, BNB, EOS
        };

        public static SymbolType BTC => new SymbolType("BTCUSDT");
        public static SymbolType ETH => new SymbolType("ETHUSDT");
        public static SymbolType ETC => new SymbolType("ETCUSDT");
        public static SymbolType XRP => new SymbolType("XRPUSDT");
        public static SymbolType DOGE => new SymbolType("DOGEUSDT");
        public static SymbolType SHIB => new SymbolType("1000SHIBUSDT");
        public static SymbolType BNB => new SymbolType("BNBUSDT");
        public static SymbolType EOS => new SymbolType("EOSUSDT");
        
        public static implicit operator string(SymbolType type) => type.Value;
        public static implicit operator SymbolType(string text) => new SymbolType(text);
        public override string ToString() => Value;
    }
}