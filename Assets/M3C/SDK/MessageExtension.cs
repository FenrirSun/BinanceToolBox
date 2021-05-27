namespace M3C.Finance.BinanceSdk.ResponseObjects
{
    public static class MessageExtension
    {
        public static FuturesUserDataOpenOrderInfoMessage ConvertToOrderInfoMessage(this WsFuturesUserDataOrderTradeUpdateMessage msg) {
            FuturesUserDataOpenOrderInfoMessage result = new FuturesUserDataOpenOrderInfoMessage();
            SyncOrderInfoData(msg, result);
            return result;
        }

        public static void SyncOrderInfoData(this WsFuturesUserDataOrderTradeUpdateMessage wsMsg, FuturesUserDataOpenOrderInfoMessage msg) {
            var wsInfo = wsMsg.OrderInfo;
            msg.avgPrice = wsInfo.AveryPrice;
            msg.price = wsInfo.OriginPrice;
            msg.stopPrice = wsInfo.TriggerPrice;
            msg.activatePrice = wsInfo.TraceStopLossActivationPrice;
            msg.symbol = wsInfo.Symbol;
            msg.time = wsInfo.Time;
            msg.UpdateTime = wsMsg.OrderTime;
            msg.clientOrderId = wsInfo.ClientId;
            msg.orderId = wsInfo.Id;
            msg.side = wsInfo.side;
            msg.positionSide = wsInfo.positionSide;
            msg.origType = wsInfo.OriginOrderType;
            msg.type = wsInfo.OrderType;
            msg.status = wsInfo.CurrentState;
            msg.workingType = wsInfo.WorkingType;
            msg.priceRate = wsInfo.priceRate;
            msg.timeInForce = wsInfo.timeInForce;
            msg.reduceOnly = wsInfo.IsReduceOnly;
            msg.executedQty = wsInfo.TotalTradingVolume;
        }
    }
}