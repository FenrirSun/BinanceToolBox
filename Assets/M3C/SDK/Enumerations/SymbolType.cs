using System;
using System.Collections.Generic;

namespace M3C.Finance.BinanceSdk.Enumerations
{
    [Serializable]
    public class SymbolType
    {
        public SymbolType(string value) {
            Value = value;
        }

        public string Value { get; }

        public static List<SymbolType> Types = new List<SymbolType>
        {
            BTC, ETH, LTC, XRP, EOS, BCH, ETC, DOGE, SHIB, BNB
        };

        public static SymbolType BTC => new SymbolType("BTCUSDT");
        public static SymbolType ETH => new SymbolType("ETHUSDT");
        public static SymbolType LTC => new SymbolType("LTCUSDT");
        public static SymbolType XRP => new SymbolType("XRPUSDT");
        public static SymbolType EOS => new SymbolType("EOSUSDT");
        public static SymbolType BCH => new SymbolType("BCHUSDT");
        public static SymbolType ETC => new SymbolType("ETCUSDT");
        public static SymbolType DOGE => new SymbolType("DOGEUSDT");
        public static SymbolType SHIB => new SymbolType("1000SHIBUSDT");
        public static SymbolType BNB => new SymbolType("BNBUSDT");
        
        public static implicit operator string(SymbolType type) { return type?.Value; }
        public static implicit operator SymbolType(string text) => new SymbolType(text);
        public override string ToString() => Value;
        
        public static bool operator ==(SymbolType one, SymbolType two) {
            return one?.Value == two?.Value;
        }
        
        public static bool operator !=(SymbolType one, SymbolType two) {
            return one?.Value != two?.Value;
        }
    }
}