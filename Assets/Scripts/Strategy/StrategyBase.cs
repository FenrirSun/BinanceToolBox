using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

public class StrategyBase
{
    public AccountData accountData;
    public SymbolType symbol;
    public StrategyState state;
    public decimal lastPrice;
    public int roundNum;
    public StrategyOrderInfo firstOrderInfo;
    public StrategyOrderInfo lastOrderInfo;
    
    public virtual void Init(StrategyOrderInfo firstOrder) {
        state = StrategyState.Idle;
        firstOrderInfo = firstOrder;
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
    }
    
    public virtual void OnAggTradeUpdate(WebSocketTradesMessage msg) {
        if (msg.Symbol == symbol) {
            this.lastPrice = msg.Price;
            OnPriceChange(msg.Price);
        }
    }

    public virtual void OnOrderInfoUpdate(WsFuturesUserDataOrderTradeUpdateMessage msg) {
        if (msg.OrderInfo.Symbol == symbol) {
            OnDataOrderTradeUpdate(msg);
        }
    }
    
    protected virtual void OnPriceChange(decimal price) {
        
    }

    protected virtual void OnDataOrderTradeUpdate(WsFuturesUserDataOrderTradeUpdateMessage msg) {
        
    }
}

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
}