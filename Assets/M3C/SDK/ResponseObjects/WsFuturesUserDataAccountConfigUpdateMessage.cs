using System.Collections.Generic;
using M3C.Finance.BinanceSdk.Enumerations;
using Newtonsoft.Json;

namespace M3C.Finance.BinanceSdk.ResponseObjects
{
    public class WsFuturesUserDataAccountConfigUpdateMessage : WebSocketMessageBase
    {
        [JsonProperty(PropertyName = "T")]
        public long OrderTime { get; set; }
        
        [JsonProperty(PropertyName = "ac")]
        public WsFuturesAC ac { get; set; }
        
        [JsonProperty(PropertyName = "ai")]
        public WsFuturesAI ai { get; set; }
    }
    
    public class WsFuturesAC
    {
        [JsonProperty(PropertyName = "s")]
        public SymbolType Symbol { get; set; }

        // 杠杆倍数
        [JsonProperty(PropertyName = "l")]
        public int Leverage { get; set; }
    }
    
    public class WsFuturesAI
    {
        // 联合保证金状态
        [JsonProperty(PropertyName = "j")]
        public bool JointDepositState { get; set; }
    }
}