using M3C.Finance.BinanceSdk.Enumerations;
using Newtonsoft.Json;

namespace M3C.Finance.BinanceSdk.ResponseObjects
{
    public class OrderInfo
    {
        [JsonProperty("symbol")]
        public SymbolType Symbol { get; set; }

        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("origQty")]
        public decimal OriginalQuantity { get; set; }

        [JsonProperty("executedQty")]
        public decimal ExecutedQuantity { get; set; }

        [JsonProperty("status")]
        public OrderStatus Status { get; set; }

        [JsonProperty("timeInForce")]
        public TimeInForce TimeInForce { get; set; }

        [JsonProperty("type")]
        public OrderType OrderType { get; set; }

        [JsonProperty("side")]
        public OrderSide OrderSide { get; set; }
        
        [JsonProperty("stopPrice")]
        public decimal StopPrice { get; set; }

        [JsonProperty("icebergQty")]
        public decimal IcebergQuantity { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("positionSide")]
        public PositionSide PositionSide { get; set; }
        
        [JsonProperty("reduceOnly")]
        public string reduceOnly { get; set; }
    }
}
