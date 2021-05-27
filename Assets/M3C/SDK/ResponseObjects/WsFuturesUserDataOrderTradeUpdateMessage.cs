using System.Collections.Generic;
using M3C.Finance.BinanceSdk.Enumerations;
using Newtonsoft.Json;

namespace M3C.Finance.BinanceSdk.ResponseObjects
{
    public class WsFuturesUserDataOrderTradeUpdateMessage : WebSocketMessageBase
    {
        [JsonProperty(PropertyName = "T")]
        public long OrderTime { get; set; }
        
        [JsonProperty(PropertyName = "o")]
        public WsFuturesOrderInfo OrderInfo { get; set; }
    }
    
    public class WsFuturesOrderInfo
    {
        [JsonProperty(PropertyName = "s")]
        public SymbolType Symbol { get; set; }

        // 客户端自定订单ID
        [JsonProperty(PropertyName = "c")]
        public string ClientId { get; set; }
        
        // 订单方向
        [JsonProperty(PropertyName = "S")]
        public OrderSide side { get; set; }
        
        // 订单类型
        [JsonProperty(PropertyName = "o")]
        public string OrderType { get; set; }
        
        // 有效方式
        [JsonProperty(PropertyName = "f")]
        public string timeInForce { get; set; }
        
        // 订单原始数量
        [JsonProperty(PropertyName = "q")]
        public decimal OriginQuantity { get; set; }
        
        // 订单原始价格
        [JsonProperty(PropertyName = "p")]
        public decimal OriginPrice { get; set; }
        
        // 订单平均价格
        [JsonProperty(PropertyName = "ap")]
        public decimal AveryPrice { get; set; }
        
        // 条件订单触发价格，对追踪止损单无效
        [JsonProperty(PropertyName = "sp")]
        public decimal TriggerPrice { get; set; }
        
        // 本次事件的具体执行类型
        [JsonProperty(PropertyName = "x")]
        public string ExecuteType { get; set; }
        
        // 订单的当前状态
        [JsonProperty(PropertyName = "X")]
        public string CurrentState { get; set; }
        
        // 订单ID
        [JsonProperty(PropertyName = "i")]
        public long Id { get; set; }
        
        // 订单末次成交量
        [JsonProperty(PropertyName = "l")]
        public decimal LastTradingVolume { get; set; }
        
        // 订单累计已成交量
        [JsonProperty(PropertyName = "z")]
        public decimal TotalTradingVolume { get; set; }
        
        // 订单末次成交价格
        [JsonProperty(PropertyName = "L")]
        public decimal LastTradingPrice { get; set; }
        
        // 手续费资产类型
        [JsonProperty(PropertyName = "N")]
        public CurrencyType ChargeType { get; set; }
        
        // 手续费数量
        [JsonProperty(PropertyName = "n")]
        public decimal ChargePrice { get; set; }
        
        // 成交时间
        [JsonProperty(PropertyName = "T")]
        public long Time { get; set; }
        
        // 成交ID
        [JsonProperty(PropertyName = "t")]
        public long TradeId { get; set; }
        
        // 买单净值
        [JsonProperty(PropertyName = "b")]
        public decimal BuyNetValue { get; set; }
        
        // 卖单净值
        [JsonProperty(PropertyName = "a")]
        public decimal CellNetValue { get; set; }
        
        // 该成交是作为挂单成交吗？
        [JsonProperty(PropertyName = "m")]
        public bool IsPendingOrder { get; set; }
        
        // 是否是只减仓单
        [JsonProperty(PropertyName = "R")]
        public bool IsReduceOnly { get; set; }
        
        // 触发价类型
        [JsonProperty(PropertyName = "wt")]
        public string WorkingType { get; set; }
        
        // 原始订单类型
        [JsonProperty(PropertyName = "ot")]
        public OrderType OriginOrderType { get; set; }
        
        // 持仓方向
        [JsonProperty(PropertyName = "ps")]
        public PositionSide positionSide { get; set; }
        
        // 是否为触发平仓单; 仅在条件订单情况下会推送此字段
        [JsonProperty(PropertyName = "cp")]
        public bool IsTriggerCloseout { get; set; }
        
        // 追踪止损激活价格, 仅在追踪止损单时会推送此字段
        [JsonProperty(PropertyName = "AP")]
        public decimal TraceStopLossActivationPrice { get; set; }
        
        // 追踪止损回调比例, 仅在追踪止损单时会推送此字段
        [JsonProperty(PropertyName = "cr")]
        public decimal priceRate { get; set; }
        
        // 该交易实现盈亏
        [JsonProperty(PropertyName = "rp")]
        public decimal ProfitAndLoss  { get; set; }
    }
}