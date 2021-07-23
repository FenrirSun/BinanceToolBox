using M3C.Finance.BinanceSdk.Enumerations;
using Newtonsoft.Json;

namespace M3C.Finance.BinanceSdk.ResponseObjects
{
    public class GetOrderResponse
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }
        
        [JsonProperty("avgPrice")]
        public decimal AvgPrice { get; set; }
        
        [JsonProperty("price")]
        public decimal Price { get; set; }
        
        [JsonProperty("stopPrice")]
        public decimal StopPrice { get; set; }
        
        [JsonProperty("side")]
        public OrderSide Side { get; set; }
        
        [JsonProperty("positionSide")]
        public PositionSide PositionSide { get; set; }
        
        [JsonProperty("status")]
        public OrderStatus Status { get; set; }
        
        [JsonProperty("type")]
        public OrderType Type{ get; set; }
        
        [JsonProperty("origType")]
        public OrderType OrigType{ get; set; }
        
        [JsonProperty("updateTime")]
        public long UpdateTime { get; set; }
    }
}
