using System.Collections.Generic;
using M3C.Finance.BinanceSdk.Enumerations;
using Newtonsoft.Json;

namespace M3C.Finance.BinanceSdk.ResponseObjects
{
    public class FuturesUserDataOrderResponse
    {
        [JsonProperty(PropertyName = "symbol")] 
        public SymbolType symbol { get; set; }
        
        [JsonProperty(PropertyName = "updateTime")] 
        public long UpdateTime { get; set; }
        
        // 用户自定义的订单号
        [JsonProperty(PropertyName = "clientOrderId")] 
        public string clientOrderId { get; set; }
        
        [JsonProperty(PropertyName = "cumQty")] 
        public decimal cumQty { get; set; }
        
        // 成交金额
        [JsonProperty(PropertyName = "cumQuote")] 
        public decimal cumQuote { get; set; }
        
        // 成交量
        [JsonProperty(PropertyName = "executedQty")] 
        public decimal executedQty { get; set; }
        
        // 系统订单号
        [JsonProperty(PropertyName = "orderId")] 
        public long orderId { get; set; }
        
        // 平均成交价
        [JsonProperty(PropertyName = "avgPrice")] 
        public decimal avgPrice { get; set; }
        
        // 原始委托数量
        [JsonProperty(PropertyName = "origQty")] 
        public decimal origQty { get; set; }

        // 委托价格
        [JsonProperty(PropertyName = "price")] 
        public decimal price { get; set; }

        // 仅减仓
        [JsonProperty(PropertyName = "reduceOnly")] 
        public bool reduceOnly { get; set; }

        // 买卖方向
        [JsonProperty(PropertyName = "side")] 
        public OrderSide side { get; set; }

        // 持仓方向
        [JsonProperty(PropertyName = "positionSide")] 
        public PositionSide positionSide { get; set; }

        // 订单状态
        [JsonProperty(PropertyName = "status")] 
        public string status { get; set; }
        
        // 触发价，对`TRAILING_STOP_MARKET`无效
        [JsonProperty(PropertyName = "stopPrice")] 
        public decimal stopPrice { get; set; }

        // 是否条件全平仓
        [JsonProperty(PropertyName = "closePosition")] 
        public bool closePosition { get; set; }

        // 有效方法
        [JsonProperty(PropertyName = "timeInForce")] 
        public string timeInForce { get; set; }

        // 订单类型
        [JsonProperty(PropertyName = "type")] 
        public string type { get; set; }
        
        // 触发前订单类型
        [JsonProperty(PropertyName = "origType")] 
        public string origType { get; set; }
        
        // 跟踪止损激活价格, 仅`TRAILING_STOP_MARKET` 订单返回此字段
        [JsonProperty(PropertyName = "activatePrice")] 
        public decimal activatePrice { get; set; }
        
        // 跟踪止损回调比例, 仅`TRAILING_STOP_MARKET` 订单返回此字段
        [JsonProperty(PropertyName = "priceRate")] 
        public decimal priceRate { get; set; }

        // 条件价格触发类型
        [JsonProperty(PropertyName = "workingType")] 
        public string workingType { get; set; }
        
        // 是否开启条件单触发保护
        [JsonProperty(PropertyName = "priceProtect")] 
        public bool priceProtect { get; set; }
    }
}