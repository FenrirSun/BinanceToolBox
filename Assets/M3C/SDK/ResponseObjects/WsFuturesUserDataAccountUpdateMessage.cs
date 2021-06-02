using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M3C.Finance.BinanceSdk.Enumerations;
using Newtonsoft.Json;

namespace M3C.Finance.BinanceSdk.ResponseObjects
{
    public class WsFuturesUserDataAccountUpdateMessage : WebSocketMessageBase
    {
        [JsonProperty(PropertyName = "T")]
        public long OrderTime { get; set; }
        
        [JsonProperty(PropertyName = "a")]
        public WsFuturesAccountUpdateInfo AccountUpdateInfo { get; set; }
    }
    
    public class WsFuturesAccountUpdateInfo
    {
        [JsonProperty(PropertyName = "m")]
        public string Reason { get; set; }

        [JsonProperty(PropertyName = "B")]
        public List<WsFuturesAccountBalanceInfo> BalanceInfo { get; set; }

        [JsonProperty(PropertyName = "P")]
        public List<WsFuturesAccountTradeInfo> TradeInfo { get; set; }
    }

    public class WsFuturesAccountBalanceInfo
    {
        // 资产名称
        [JsonProperty(PropertyName = "a")]
        public CurrencyType CurrencyName { get; set; }
        
        // 钱包余额
        [JsonProperty(PropertyName = "wb")]
        public decimal WalletRemain { get; set; }
        
        // 除去逐仓仓位保证金的钱包余额
        [JsonProperty(PropertyName = "cw")]
        public decimal WalletRemainFree { get; set; }
        
        // 除去盈亏与交易手续费以外的钱包余额改变量
        [JsonProperty(PropertyName = "bc")]
        public decimal Reason { get; set; }
    }
    
    public class WsFuturesAccountTradeInfo
    {
        // 资产对
        [JsonProperty(PropertyName = "s")]
        public SymbolType Symbol { get; set; }
        
        // 仓位
        [JsonProperty(PropertyName = "pa")]
        public decimal Positions { get; set; }
        
        // 入仓价格
        [JsonProperty(PropertyName = "ep")]
        public decimal EnterPrice { get; set; }
        
        // (费前)累计实现损益
        [JsonProperty(PropertyName = "cr")]
        public decimal AccumulativeProfit  { get; set; }
        
        // 持仓未实现盈亏
        [JsonProperty(PropertyName = "up")]
        public decimal UnrealizedProfit { get; set; }
        
        // 保证金模式
        [JsonProperty(PropertyName = "mt")]
        public string MarginTarget { get; set; }
        
        // 若为逐仓，仓位保证金
        [JsonProperty(PropertyName = "iw")]
        public decimal Margin { get; set; }
        
        // 持仓方向
        [JsonProperty(PropertyName = "ps")]
        public PositionSide PositionSide { get; set; }
    }
}
