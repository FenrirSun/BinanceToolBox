using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;

public class StrategyBase : ComponentBase
{
    public SymbolType symbol;
    public StrategyState state;
    public decimal lastPrice;
    
    public virtual void Init() {
        state = StrategyState.Idle;
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
    
    protected virtual void AddListener() {
        GetEventComp().Listen<OnAggTradeUpdate>((evt) =>
        {
            if (evt.msg.Symbol == symbol) {
                OnPriceChange(evt.msg.Price);
            }
        });
    }

    protected virtual void OnPriceChange(decimal price) {
        this.lastPrice = price;
    }

}

public class StrategyOrderInfo
{
    public decimal pendingPrice;
    public decimal quantity;
    
}