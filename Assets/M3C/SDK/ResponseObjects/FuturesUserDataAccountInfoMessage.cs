using System.Collections.Generic;
using M3C.Finance.BinanceSdk.Enumerations;
using Newtonsoft.Json;

namespace M3C.Finance.BinanceSdk.ResponseObjects
{
    public class FuturesUserDataAccountInfoMessage
    {
        [JsonProperty(PropertyName = "updateTime")] 
        public long UpdateTime { get; set; }

        // 当前所需起始保证金总额(存在逐仓请忽略), 仅计算usdt资产
        [JsonProperty(PropertyName = "totalInitialMargin")] 
        public decimal TotalInitialMargin { get; set; }
        
        // 维持保证金总额, 仅计算usdt资产
        [JsonProperty(PropertyName = "totalMaintMargin")] 
        public decimal totalMaintMargin { get; set; }
        
        // 账户总余额, 仅计算usdt资产
        [JsonProperty(PropertyName = "totalWalletBalance")] 
        public decimal totalWalletBalance { get; set; }
        
        // 持仓未实现盈亏总额, 仅计算usdt资产
        [JsonProperty(PropertyName = "totalUnrealizedProfit")] 
        public decimal totalUnrealizedProfit { get; set; }
        
        // 保证金总余额, 仅计算usdt资产
        [JsonProperty(PropertyName = "totalMarginBalance")] 
        public decimal totalMarginBalance { get; set; }
        
        // 持仓所需起始保证金(基于最新标记价格), 仅计算usdt资
        [JsonProperty(PropertyName = "totalPositionInitialMargin")] 
        public decimal totalPositionInitialMargin { get; set; }
        
        // 当前挂单所需起始保证金(基于最新标记价格), 仅计算usdt资
        [JsonProperty(PropertyName = "totalOpenOrderInitialMargin")] 
        public decimal totalOpenOrderInitialMargin { get; set; }
        
        // 全仓账户余额, 仅计算usdt资产
        [JsonProperty(PropertyName = "totalCrossWalletBalance")] 
        public decimal totalCrossWalletBalance { get; set; }
        
        // 全仓持仓未实现盈亏总额, 仅计算usdt资产
        [JsonProperty(PropertyName = "totalCrossUnPnl")] 
        public decimal totalCrossUnPnl { get; set; }
        
        // 可用余额, 仅计算usdt资产
        [JsonProperty(PropertyName = "availableBalance")] 
        public decimal availableBalance { get; set; }
        
        // 最大可转出余额, 仅计算usdt资产
        [JsonProperty(PropertyName = "maxWithdrawAmount")] 
        public decimal maxWithdrawAmount { get; set; }

        [JsonProperty(PropertyName = "assets")] 
        public List<FuturesUserDataAssetInfo> assets { get; set; }

        [JsonProperty(PropertyName = "positions")] 
        public List<FuturesUserDataPositionInfo> positions { get; set; }
    }
    
    public class FuturesUserDataAssetInfo
    {
        [JsonProperty(PropertyName = "asset")] 
        public CurrencyType asset { get; set; }
        //余额
        [JsonProperty(PropertyName = "walletBalance")] 
        public decimal walletBalance { get; set; }
        // 未实现盈亏
        [JsonProperty(PropertyName = "unrealizedProfit")] 
        public decimal unrealizedProfit { get; set; }
        // 保证金余额
        [JsonProperty(PropertyName = "marginBalance")] 
        public decimal marginBalance { get; set; }
        // 维持保证金
        [JsonProperty(PropertyName = "maintMargin")] 
        public decimal maintMargin { get; set; }
        // 当前所需起始保证金
        [JsonProperty(PropertyName = "initialMargin")] 
        public decimal initialMargin { get; set; }
        // 持仓所需起始保证金(基于最新标记价格)
        [JsonProperty(PropertyName = "positionInitialMargin")] 
        public decimal positionInitialMargin { get; set; }
        // 当前挂单所需起始保证金(基于最新标记价格)
        [JsonProperty(PropertyName = "openOrderInitialMargin")] 
        public decimal openOrderInitialMargin { get; set; }
        //全仓账户余额
        [JsonProperty(PropertyName = "crossWalletBalance")] 
        public decimal crossWalletBalance { get; set; }
        // 全仓持仓未实现盈亏
        [JsonProperty(PropertyName = "crossUnPnl")] 
        public decimal crossUnPnl { get; set; }
        // 可用余额
        [JsonProperty(PropertyName = "availableBalance")] 
        public decimal availableBalance { get; set; }
        // 最大可转出余额
        [JsonProperty(PropertyName = "maxWithdrawAmount")] 
        public decimal maxWithdrawAmount { get; set; }
        // 是否可用作联合保证金
        [JsonProperty(PropertyName = "marginAvailable")] 
        public bool marginAvailable { get; set; }
    }
    
    public class FuturesUserDataPositionInfo
    {
        [JsonProperty(PropertyName = "symbol")] 
        public SymbolType symbol { get; set; }
        // 当前所需起始保证金
        [JsonProperty(PropertyName = "initialMargin")] 
        public decimal initialMargin { get; set; }
        // 维持保证金
        [JsonProperty(PropertyName = "maintMargin")] 
        public decimal maintMargin { get; set; }
        // 未实现盈亏
        [JsonProperty(PropertyName = "unrealizedProfit")] 
        public decimal unrealizedProfit { get; set; }
        // 持仓所需起始保证金(基于最新标记价格)
        [JsonProperty(PropertyName = "positionInitialMargin")] 
        public decimal positionInitialMargin { get; set; }
        // 当前挂单所需起始保证金(基于最新标记价格)
        [JsonProperty(PropertyName = "openOrderInitialMargin")] 
        public decimal openOrderInitialMargin { get; set; }
        // 杠杆倍率
        [JsonProperty(PropertyName = "leverage")] 
        public decimal leverage { get; set; }
        // 是否是逐仓模式
        [JsonProperty(PropertyName = "isolated")] 
        public bool isolated { get; set; }
        // 持仓成本价
        [JsonProperty(PropertyName = "entryPrice")] 
        public decimal entryPrice { get; set; }
        // 当前杠杆下用户可用的最大名义价值
        // [JsonProperty(PropertyName = "maxNotional")] 
        // public decimal maxNotional { get; set; }
        // 持仓方向
        [JsonProperty(PropertyName = "positionSide")] 
        public PositionSide positionSide { get; set; }
        // 持仓数量
        [JsonProperty(PropertyName = "positionAmt")] 
        public decimal positionAmt { get; set; }
    }
}