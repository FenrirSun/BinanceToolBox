using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

[Serializable]
public class StrategyBase
{
    [NonSerialized]
    public AccountData accountData;
    public SymbolType symbol;
    public StrategyState state;
    public decimal lastPrice;
    public int roundNum;
    public StrategyOrderInfo firstOrderInfo;
    public StrategyOrderInfo lastOrderInfo;
    public List<StrategyOrderInfo> historyOrderList;
    
    public virtual void Init(AccountData ad, StrategyOrderInfo firstOrder) {
        state = StrategyState.Idle;
        symbol = firstOrder.symbol;
        firstOrderInfo = firstOrder;
        accountData = ad;
    }

    public virtual void StartStrategy() {
        state = StrategyState.Executing;
        roundNum = 0;
    }
    
    public virtual void NextRound() {
        roundNum++;
    }

    public virtual void PauseStrategy() {
        state = StrategyState.Pause;
    }

    public virtual void StopStrategy() {
        state = StrategyState.Finish;
        EventManager.Instance.Send(StopStrategyEvent.Create(this));
    }
    
    public virtual void OnAggTradeUpdate(WebSocketTradesMessage msg) {
        if (msg.Symbol.Value == symbol) {
            this.lastPrice = msg.Price;
            OnPriceChange(msg.Price);
        }
    }

    public virtual void OnWsOrderInfoUpdate(WsFuturesUserDataOrderTradeUpdateMessage msg) {
        if (msg.OrderInfo.Symbol.Value == symbol) {
            OnWsDataOrderTradeUpdate(msg);
        }
    }
    
    public virtual void OnOrderInfoUpdate(GetOrderResponse msg) {
        if (msg.Symbol == symbol.Value) {
            OnDataOrderTradeUpdate(msg);
        }
    }
    
    public virtual void OnDisconnected() { }
    
    public virtual void OnReconnect() { }
    
    protected virtual void OnPriceChange(decimal price) { }

    protected virtual void OnWsDataOrderTradeUpdate(WsFuturesUserDataOrderTradeUpdateMessage msg) { }

    protected virtual void OnDataOrderTradeUpdate(GetOrderResponse msg) { }

    public T Clone<T>() where T : StrategyBase {
        object obj = null;
        BinaryFormatter inputFormatter = new BinaryFormatter();
        MemoryStream inputStream;
        using (inputStream = new MemoryStream()) {
            inputFormatter.Serialize(inputStream, this);
        }
        using (MemoryStream outputStream = new MemoryStream(inputStream.ToArray())) {
            BinaryFormatter outputFormatter = new BinaryFormatter();
            obj = outputFormatter.Deserialize(outputStream);
        }

        return (T) obj;
    }
}

[Serializable]
public class StrategyOrderInfo
{
    public enum OrderState
    {
        idle,// 未开始
        waitForConfirmOrder,//等待挂单成功
        waitForDeal,//等待挂单交易成功
        waitComfirmStopOrTakeprofit,//等待止盈止损挂单成功
        waitDealOfStopOrTakeprofit,//等待止盈止损交易成功
        finish,//结束
        cancel
    }

    public SymbolType symbol;
    public decimal pendingPrice;
    public decimal quantity;
    public decimal stopPrice;
    public decimal takeProfitPrice;
    public PositionSide posSide;
    public OrderSide side;
    public OrderType orderType;
    
    public int orderClientId;
    public OrderState state;

    public StrategyOrderInfo Clone() {
        StrategyOrderInfo result = new StrategyOrderInfo();
        result.symbol = symbol;
        result.pendingPrice = pendingPrice;
        result.quantity = quantity;
        result.stopPrice = stopPrice;
        result.takeProfitPrice = takeProfitPrice;
        result.posSide = posSide;
        result.side = side;
        result.orderType = orderType;
        result.orderClientId = orderClientId;
        result.state = state;
        return result;
    }
}