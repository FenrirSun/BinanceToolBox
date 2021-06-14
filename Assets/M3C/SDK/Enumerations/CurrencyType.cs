using System.Collections.Generic;

namespace M3C.Finance.BinanceSdk.Enumerations
{
    public class CurrencyType
    {
        public CurrencyType(string value) {
            Value = value;
        }

        public string Value { get; }

        public static List<CurrencyType> Types = new List<CurrencyType>
        {
            USDT, BTC, ETH, ETC, XRP, DOGE, SHIB, BNB, EOS
        };

        public static CurrencyType USDT => new CurrencyType("USDT");
        public static CurrencyType BTC => new CurrencyType("BTC");
        public static CurrencyType ETH => new CurrencyType("ETH");
        public static CurrencyType ETC => new CurrencyType("ETC");
        public static CurrencyType XRP => new CurrencyType("XRP");
        public static CurrencyType DOGE => new CurrencyType("DOGE");
        public static CurrencyType SHIB => new CurrencyType("1000SHIB");
        public static CurrencyType BNB => new CurrencyType("BNB");
        public static CurrencyType EOS => new CurrencyType("EOS");

        public static implicit operator string(CurrencyType type) => type.Value;
        public static implicit operator CurrencyType(string text) => new CurrencyType(text);
        public override string ToString() => Value;
        
        public static bool operator ==(CurrencyType one, CurrencyType two) {
            return one?.Value == two?.Value;
        }
        
        public static bool operator !=(CurrencyType one, CurrencyType two) {
            return one?.Value != two?.Value;
        }
    }
}