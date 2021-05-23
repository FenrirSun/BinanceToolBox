using System.Collections.Generic;
using M3C.Finance.BinanceSdk.Enumerations;
using Newtonsoft.Json;

namespace M3C.Finance.BinanceSdk.ResponseObjects
{
    public class FuturesUserDataAccountBalanceMessage
    {
        // 账户唯一识别码
        [JsonProperty(PropertyName = "accountAlias")] 
        public string AccountAlias { get; set; }

        // 资产
        [JsonProperty(PropertyName = "asset")] 
        public CurrencyType Asset { get; set; }
        
        // 总余额
        [JsonProperty(PropertyName = "balance")] 
        public decimal Balance { get; set; }
        
        // 全仓余额
        [JsonProperty(PropertyName = "crossWalletBalance")] 
        public decimal CrossWalletBalance { get; set; }
        
        // 全仓持仓未实现盈亏
        [JsonProperty(PropertyName = "crossUnPnl")] 
        public decimal CrossUnPnl { get; set; }
        
        // 下单可用余额
        [JsonProperty(PropertyName = "availableBalance")] 
        public decimal AvailableBalance { get; set; }
        
        // 最大可转出余额
        [JsonProperty(PropertyName = "maxWithdrawAmount")] 
        public decimal MaxWithdrawAmount { get; set; }
        
        // 是否可用作联合保证金
        [JsonProperty(PropertyName = "marginAvailable")] 
        public bool MarginAvailable { get; set; }
        
        [JsonProperty(PropertyName = "updateTime")] 
        public long UpdateTime { get; set; }
    }
}