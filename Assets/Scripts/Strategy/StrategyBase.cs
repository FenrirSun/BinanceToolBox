using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

public class StrategyBase
{
    public AccountData accountData;
    public SymbolType symbol;
    public StrategyState state;
    public decimal lastPrice;
    public StrategyOrderInfo firstOrderInfo;
    public StrategyOrderInfo lastOrderInfo;
    
    public virtual void Init(StrategyOrderInfo firstOrder) {
        state = StrategyState.Idle;
        firstOrderInfo = firstOrder;
    }

    public virtual void StartStrategy() {
        state = StrategyState.Executing;
    }

    public virtual void PauseStrategy() {
        state = StrategyState.Pause;
    }

    public virtual void StopStrategy() {
        state = StrategyState.Finish;
    }
    
    public virtual void OnAggTradeUpdate(WebSocketTradesMessage msg) {
        if (msg.Symbol == symbol) {
            OnPriceChange(msg.Price);
        }
    }

    public virtual void OnOrderInfoUpdate(WsFuturesUserDataOrderTradeUpdateMessage msg) {
        if (msg.OrderInfo.Symbol == symbol) {
            OnDataOrderTradeUpdate(msg);
        }
    }
    
    protected virtual void OnPriceChange(decimal price) {
        this.lastPrice = price;
    }

    protected virtual void OnDataOrderTradeUpdate(WsFuturesUserDataOrderTradeUpdateMessage msg) {
        
    }
}

public class StrategyOrderInfo
{
    public enum OrderState
    {
        idle,
        pending,
        closeFirst,
        waitStopOrTakeprofit,
        finish,
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
    public int stopClientId;
    public int takeProfitClientId;
    
    public OrderState state;
}