using System;
using System.Collections.Generic;
using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

[Serializable]
public class StrategyGradientGridsTrade : StrategyBase
{
    // 单间差价
    public decimal triggerPriceGap;
    public decimal orderPriceGap;
    public decimal quantityRatioGap;
    public bool stopStrategyWhenTriggerStopPrice; // 止损后是否停止策略
    public decimal maxOrderPrice;
    
    // 单内差价
    private decimal stopPriceSpread;
    private decimal takeProfitPriceSpread;

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
        stopPriceSpread = firstOrder.stopPrice < (decimal)float.Epsilon ? 0 : firstOrder.stopPrice - firstOrder.pendingPrice;
        takeProfitPriceSpread = firstOrder.takeProfitPrice < (decimal)float.Epsilon ? 0 : firstOrder.takeProfitPrice - firstOrder.pendingPrice;
    }

    public override void StartStrategy() {
        base.StartStrategy();
        lastOrderInfo = firstOrderInfo.Clone();
        lastOrderInfo.state = StrategyOrderInfo.OrderState.idle;
        historyOrderList.Add(lastOrderInfo);
        SendNextOrder();
    }

    public override void StopStrategy() {
        CommonMessageDialog.OpenWithOneButton("策略终止", null);
        base.StopStrategy();
    }

    public override void NextRound() {
        base.NextRound();
        // StopStrategy();
        // return;
        SendNextOrder();
    }

    private void SendNextOrder() {
        tradeState = GradientGridsState.holdOrder;
        lastOrderInfo.pendingPrice = firstOrderInfo.pendingPrice + roundNum * orderPriceGap;
        lastOrderInfo.orderClientId = GameUtils.GetNewGuid();
        if (Math.Abs(stopPriceSpread) > (decimal) float.Epsilon)
            lastOrderInfo.stopPrice = lastOrderInfo.pendingPrice + stopPriceSpread;
        else
            lastOrderInfo.stopPrice = 0;
        if(Math.Abs(takeProfitPriceSpread) > (decimal)float.Epsilon)
            lastOrderInfo.takeProfitPrice = lastOrderInfo.pendingPrice + takeProfitPriceSpread;
        else
            lastOrderInfo.takeProfitPrice = 0;
        lastOrderInfo.quantity = firstOrderInfo.quantity * (1 + quantityRatioGap * roundNum) * (decimal)accountData.orderRatio;
        lastOrderInfo.quantity = decimal.Parse(lastOrderInfo.quantity.ToString("G0"));
        lastOrderInfo.state = StrategyOrderInfo.OrderState.waitForConfirmOrder;
        var newOrder = Utility.GenerateOrderInfo(lastOrderInfo, 1);
        EventManager.Instance.Send(NewOrder.Create(accountData, newOrder));
    }

    protected override void OnPriceChange(decimal price) {
        if (state != StrategyState.Executing || tradeState != GradientGridsState.waitForStopPrice)
            return;
        
        if (firstOrderInfo.posSide == PositionSide.LONG) {
            if (price <= firstOrderInfo.pendingPrice + (roundNum + 1) * triggerPriceGap) {
                NextRound();
            }
        } else if (firstOrderInfo.posSide == PositionSide.SHORT) {
            if (price >= firstOrderInfo.pendingPrice + (roundNum + 1) * triggerPriceGap) {
                NextRound();
            }
        }
    }

    protected override void OnDataOrderTradeUpdate(WsFuturesUserDataOrderTradeUpdateMessage msg) {
        if (tradeState != GradientGridsState.holdOrder)
            return;
        if (lastOrderInfo == null || state != StrategyState.Executing)
            return;

        if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitForConfirmOrder) {
            if (msg.OrderInfo.ClientId != lastOrderInfo.orderClientId.ToString())
                return;

            // 收到socket消息确认挂单成功
            if (msg.OrderInfo.ExecuteType == EventStatus.New) {
                lastOrderInfo.state = StrategyOrderInfo.OrderState.waitForDeal;
            } else if (msg.OrderInfo.ExecuteType == EventStatus.Trade) {
                OnTradeFinish();
            } else if (msg.OrderInfo.ExecuteType == EventStatus.Canceled) {
                StopStrategy();
            }
        } else if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitForDeal) {
            if (msg.OrderInfo.ClientId != lastOrderInfo.orderClientId.ToString())
                return;

            if (msg.OrderInfo.ExecuteType == EventStatus.Trade) {
                // 收到socket消息确认订单成交,判断是否需要挂止盈止损单
                OnTradeFinish();
            } else if (msg.OrderInfo.ExecuteType == EventStatus.Canceled) {
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
                    lastOrderInfo.state = StrategyOrderInfo.OrderState.finish;
                } else if (msg.OrderInfo.ExecuteType == EventStatus.Canceled) {
                    StopStrategy();
                }
            }
        } else if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitDealOfStopOrTakeprofit) {
            // 等待止盈止损单交易成功
            if (msg.OrderInfo.ClientId == GameConfig.TakeProfitPrefix + lastOrderInfo.orderClientId.ToString()) {
                // 止盈成功
                if (msg.OrderInfo.ExecuteType == EventStatus.Trade) {
                    tradeState = GradientGridsState.waitForStopPrice;
                    lastOrderInfo.state = StrategyOrderInfo.OrderState.finish;
                    if (lastOrderInfo.stopPrice > 0) {
                        // 如果有止损，关掉止损单
                        EventManager.Instance.Send(CancelOrderByClientId.Create(MainOrderDialog.curAccountData, MainOrderDialog.curSymbol,
                            GameConfig.StopPrefix + lastOrderInfo.orderClientId.ToString()));
                    }
                } else if (msg.OrderInfo.ExecuteType == EventStatus.Canceled) {
                    StopStrategy();
                }
            }

            if (msg.OrderInfo.ClientId == GameConfig.StopPrefix + lastOrderInfo.orderClientId.ToString()) {
                // 止损成功
                if (msg.OrderInfo.ExecuteType == EventStatus.Trade) {
                    tradeState = GradientGridsState.waitForStopPrice;
                    lastOrderInfo.state = StrategyOrderInfo.OrderState.finish;
                    if (lastOrderInfo.takeProfitPrice > 0) {
                        // 如果有止盈，关掉止盈单
                        EventManager.Instance.Send(CancelOrderByClientId.Create(MainOrderDialog.curAccountData, MainOrderDialog.curSymbol,
                            GameConfig.TakeProfitPrefix + lastOrderInfo.orderClientId.ToString()));
                    }

                    if (stopStrategyWhenTriggerStopPrice) {
                        StopStrategy();;
                        return;
                    }
                } else if (msg.OrderInfo.ExecuteType == EventStatus.Canceled) {
                    StopStrategy();
                }
            }
        }

        void OnTradeFinish() {
            if (lastOrderInfo.takeProfitPrice > (decimal)float.Epsilon) {
                lastOrderInfo.state = StrategyOrderInfo.OrderState.waitComfirmStopOrTakeprofit;
                EventManager.Instance.Send(NewOrder.Create(accountData, Utility.GenerateOrderInfo(lastOrderInfo, 2)));
            }

            if (lastOrderInfo.stopPrice > (decimal)float.Epsilon) {
                lastOrderInfo.state = StrategyOrderInfo.OrderState.waitComfirmStopOrTakeprofit;
                EventManager.Instance.Send(NewOrder.Create(accountData, Utility.GenerateOrderInfo(lastOrderInfo, 3)));
            }

            if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitForDeal) {
                // 如果没有止盈止损单，该单结束，策略结束
                lastOrderInfo.state = StrategyOrderInfo.OrderState.finish;
                StopStrategy();
            }
        }
    }
}