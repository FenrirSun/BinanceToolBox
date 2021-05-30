using System.Collections.Generic;
using GameEvents;
using LitJson;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

public class StrategyGradientGridsTrade : StrategyBase
{
    public enum GradientGridsState
    {
        idle,
        holdOrder,
        waitForStopPrice,
    }

    public GradientGridsState tradeState;

    public void Init(AccountData ad, StrategyOrderInfo firstOrder) {
        historyOrderList = new List<StrategyOrderInfo>();
        base.Init(ad, firstOrder);
        tradeState = GradientGridsState.idle;
    }

    public override void StartStrategy() {
        base.StartStrategy();
        lastOrderInfo = firstOrderInfo.Clone();
        lastOrderInfo.state = StrategyOrderInfo.OrderState.idle;
        historyOrderList.Add(lastOrderInfo);
        SendNextOrder();
    }

    public override void StopStrategy() {
        base.StopStrategy();
    }

    public override void NextRound() {
        base.NextRound();
        StopStrategy();
        return;
        
        SendNextOrder();
    }

    private void SendNextOrder() {
        tradeState = GradientGridsState.holdOrder;
        lastOrderInfo.state = StrategyOrderInfo.OrderState.waitForConfirmOrder;
        EventManager.Instance.Send(NewOrder.Create(accountData, Utility.GenerateOrderInfo(lastOrderInfo, 1)));
    }

    protected override void OnPriceChange(decimal price) {
        if (tradeState != GradientGridsState.waitForStopPrice)
            return;
        if (firstOrderInfo.posSide == PositionSide.LONG) {
            
        } else if (firstOrderInfo.posSide == PositionSide.SHORT) {
            
        }
    }

    protected override void OnDataOrderTradeUpdate(WsFuturesUserDataOrderTradeUpdateMessage msg) {
        if (tradeState != GradientGridsState.holdOrder)
            return;
        if (lastOrderInfo == null || state != StrategyState.Executing)
            return;

        if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitForConfirmOrder) {
            // 收到socket消息确认挂单成功
            if (msg.OrderInfo.ClientId != lastOrderInfo.orderClientId.ToString())
                return;

            if (msg.OrderInfo.ExecuteType == EventStatus.New) {
                lastOrderInfo.state = StrategyOrderInfo.OrderState.waitForDeal;
            } else if (msg.OrderInfo.ExecuteType == EventStatus.Trade) {
                OnTradeFinish();
            } else if (msg.OrderInfo.ExecuteType == EventStatus.Canceled || msg.OrderInfo.ExecuteType == EventStatus.Expired) {
                StopStrategy();
            }
        } else if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitForDeal) {
            if (msg.OrderInfo.ClientId != lastOrderInfo.orderClientId.ToString())
                return;

            if (msg.OrderInfo.ExecuteType == EventStatus.Trade) {
                // 收到socket消息确认订单成交,判断是否需要挂止盈止损单
                OnTradeFinish();
            } else if (msg.OrderInfo.ExecuteType == EventStatus.Canceled || msg.OrderInfo.ExecuteType == EventStatus.Expired) {
                StopStrategy();
            }
        } else if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitComfirmStopOrTakeprofit) {
            // 等待止盈止损挂单成功
            if (msg.OrderInfo.ClientId == GameConfig.TakeProfitPrefix + lastOrderInfo.orderClientId.ToString()
                || msg.OrderInfo.ClientId == GameConfig.StopPrefix + lastOrderInfo.orderClientId.ToString()) {
                if (msg.OrderInfo.ExecuteType == EventStatus.New) {
                    lastOrderInfo.state = StrategyOrderInfo.OrderState.waitDealOfStopOrTakeprofit;
                } else if (msg.OrderInfo.ExecuteType == EventStatus.Trade) {
                    tradeState = GradientGridsState.waitForStopPrice;
                } else if (msg.OrderInfo.ExecuteType == EventStatus.Canceled || msg.OrderInfo.ExecuteType == EventStatus.Expired) {
                    StopStrategy();
                }
            }
        } else if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitDealOfStopOrTakeprofit) {
            // 等待止盈止损单交易成功
            if (msg.OrderInfo.ClientId == GameConfig.TakeProfitPrefix + lastOrderInfo.orderClientId.ToString()) {
                // 止盈成功
                if (msg.OrderInfo.ExecuteType == EventStatus.Trade) {
                    tradeState = GradientGridsState.waitForStopPrice;
                } else if (msg.OrderInfo.ExecuteType == EventStatus.Canceled || msg.OrderInfo.ExecuteType == EventStatus.Expired) {
                    StopStrategy();
                }
            }

            if (msg.OrderInfo.ClientId == GameConfig.StopPrefix + lastOrderInfo.orderClientId.ToString()) {
                // 止损成功
                if (msg.OrderInfo.ExecuteType == EventStatus.Trade) {
                    tradeState = GradientGridsState.waitForStopPrice;
                } else if (msg.OrderInfo.ExecuteType == EventStatus.Canceled || msg.OrderInfo.ExecuteType == EventStatus.Expired) {
                    StopStrategy();
                }
            }
        }

        void OnTradeFinish() {
            if (lastOrderInfo.takeProfitPrice > 0) {
                lastOrderInfo.state = StrategyOrderInfo.OrderState.waitComfirmStopOrTakeprofit;
                EventManager.Instance.Send(NewOrder.Create(accountData, Utility.GenerateOrderInfo(firstOrderInfo, 2)));
            }

            if (lastOrderInfo.stopPrice > 0) {
                lastOrderInfo.state = StrategyOrderInfo.OrderState.waitComfirmStopOrTakeprofit;
                EventManager.Instance.Send(NewOrder.Create(accountData, Utility.GenerateOrderInfo(firstOrderInfo, 3)));
            }

            if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitForDeal) {
                // 如果没有止盈止损单，该单结束
                lastOrderInfo.state = StrategyOrderInfo.OrderState.finish;
            }
        }
    }
}