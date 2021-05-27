using GameEvents;
using M3C.Finance.BinanceSdk.ResponseObjects;

public class StrategyGradientGridsTrade : StrategyBase
{
    public void Init(StrategyOrderInfo firstOrder) {
        base.Init(firstOrder);
    }

    public virtual void StartStrategy() {
        base.StartStrategy();
        lastOrderInfo = firstOrderInfo;
        EventManager.Instance.Send(NewOrder.Create(accountData, firstOrderInfo));
    }

    public virtual void StopStrategy() {
        base.StopStrategy();
    }
    
    protected virtual void OnPriceChange(decimal price) {
        base.OnPriceChange(price);
    }

    protected virtual void OnDataOrderTradeUpdate(WsFuturesUserDataOrderTradeUpdateMessage msg) {
        if (lastOrderInfo == null || state != StrategyState.Executing)
            return;
        
        if (msg.OrderInfo.ClientId == lastOrderInfo.orderClientId.ToString()) {
            if (lastOrderInfo.state == StrategyOrderInfo.OrderState.pending) {
                // 收到消息确认挂单成功  closeFirst
            } else if (lastOrderInfo.state == StrategyOrderInfo.OrderState.closeFirst) {
                // 挂单成交，判断是否需要挂止盈止损单
            } else if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitStopOrTakeprofit) {
                // 等待止盈止损单中
            }
        }
    }
    
}
