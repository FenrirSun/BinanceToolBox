using System;
using System.Collections;
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
    public decimal maxOrderPrice;
    public bool stopStrategyWhenTriggerStopPrice; // 止损后是否停止策略

    // 单内差价
    private decimal stopPriceSpread;
    private decimal takeProfitPriceSpread;

    public enum GradientGridsState
    {
        idle,
        sendOrder,
        holdOrder,
        waitForTriggerPrice,
    }

    public GradientGridsState tradeState;
    public bool isPending;

    public void Init(AccountData ad, StrategyOrderInfo firstOrder) {
        historyOrderList = new List<StrategyOrderInfo>();
        base.Init(ad, firstOrder);
        tradeState = GradientGridsState.idle;
        stopPriceSpread = firstOrder.stopPrice < (decimal) float.Epsilon ? 0 : firstOrder.stopPrice - firstOrder.pendingPrice;
        takeProfitPriceSpread = firstOrder.takeProfitPrice < (decimal) float.Epsilon ? 0 : firstOrder.takeProfitPrice - firstOrder.pendingPrice;
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
        lastOrderInfo.pendingPrice = firstOrderInfo.pendingPrice + roundNum * orderPriceGap;
        lastOrderInfo.orderClientId = GameUtils.GetNewGuid();
        if (Math.Abs(stopPriceSpread) > (decimal) float.Epsilon)
            lastOrderInfo.stopPrice = lastOrderInfo.pendingPrice + stopPriceSpread;
        else
            lastOrderInfo.stopPrice = 0;
        if (Math.Abs(takeProfitPriceSpread) > (decimal) float.Epsilon)
            lastOrderInfo.takeProfitPrice = lastOrderInfo.pendingPrice + takeProfitPriceSpread;
        else
            lastOrderInfo.takeProfitPrice = 0;
        lastOrderInfo.quantity = firstOrderInfo.quantity * (1 + quantityRatioGap * roundNum) * (decimal) accountData.orderRatio;
        lastOrderInfo.quantity = decimal.Parse(lastOrderInfo.quantity.ToString("G0"));
        lastOrderInfo.state = StrategyOrderInfo.OrderState.waitForConfirmOrder;

        tradeState = GradientGridsState.sendOrder;
        GameRuntime.Instance.StartCoroutine(SendOrder());
    }

    IEnumerator SendOrder() {
        yield return null;
        var newOrder = Utility.GenerateGradientOrderInfo(lastOrderInfo, 1);
        EventManager.Instance.Send(NewOrder.Create(accountData, newOrder, e =>
        {
            if (e == null) {
                tradeState = GradientGridsState.holdOrder;
            } else {
                // GameRuntime.Instance.StartCoroutine(SendOrder());
                StopStrategy();
            }
        }));
    }

    // 价格变更
    protected override void OnPriceChange(decimal price) {
        if (state != StrategyState.Executing || tradeState != GradientGridsState.waitForTriggerPrice)
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

    // 推送交易变更
    protected override void OnWsDataOrderTradeUpdate(WsFuturesUserDataOrderTradeUpdateMessage msg) {
        DataOrderTradeUpdate(msg.OrderInfo.ClientId, msg.OrderInfo.ExecuteType);
    }

    // 交易变更
    protected override void OnDataOrderTradeUpdate(GetOrderResponse msg) {
        EventStatus eventStatus = EventStatus.Trade;
        if (msg.Status == OrderStatus.New) {
            eventStatus = EventStatus.New;
        } else if (msg.Status == OrderStatus.Expired) {
            eventStatus = EventStatus.Expired;
        } else if (msg.Status == OrderStatus.Canceled) {
            eventStatus = EventStatus.Canceled;
        }

        DataOrderTradeUpdate(msg.ClientOrderId, eventStatus);
    }

    private string lastUpdatedTradeId;
    private EventStatus lastUpdatedExecuteType;

    protected void DataOrderTradeUpdate(string clientId, EventStatus executeType) {
        if (isPending)
            return;
        if (tradeState != GradientGridsState.holdOrder)
            return;
        if (lastOrderInfo == null || state != StrategyState.Executing)
            return;
        if (lastUpdatedTradeId == clientId && lastUpdatedExecuteType == executeType)
            return;

        lastUpdatedTradeId = clientId;
        lastUpdatedExecuteType = executeType;
        var lastOrderClientId = lastOrderInfo.orderClientId.ToString();
        if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitForConfirmOrder) {
            if (clientId != lastOrderClientId)
                return;

            // 收到socket消息确认挂单成功
            if (executeType == EventStatus.New) {
                lastOrderInfo.state = StrategyOrderInfo.OrderState.waitForDeal;
            } else if (executeType == EventStatus.Trade) {
                OnTradeFinish();
            } else if (executeType == EventStatus.Canceled) {
                StopStrategy();
            }
        } else if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitForDeal) {
            if (clientId != lastOrderClientId)
                return;

            if (executeType == EventStatus.Trade) {
                // 收到socket消息确认订单成交,判断是否需要挂止盈止损单
                OnTradeFinish();
            } else if (executeType == EventStatus.Canceled) {
                StopStrategy();
            }
        } else if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitComfirmStopOrTakeprofit) {
            // 等待止盈止损挂单成功
            if (clientId == GameConfig.TakeProfitPrefix + lastOrderClientId
                || clientId == GameConfig.StopPrefix + lastOrderClientId) {
                if (executeType == EventStatus.New) {
                    lastOrderInfo.state = StrategyOrderInfo.OrderState.waitDealOfStopOrTakeprofit;
                } else if (executeType == EventStatus.Trade) {
                    tradeState = GradientGridsState.waitForTriggerPrice;
                    lastOrderInfo.state = StrategyOrderInfo.OrderState.finish;
                } else if (executeType == EventStatus.Canceled) {
                    StopStrategy();
                }
            }
        } else if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitDealOfStopOrTakeprofit) {
            // 等待止盈止损单交易成功
            if (clientId == GameConfig.TakeProfitPrefix + lastOrderClientId) {
                // 止盈成功
                if (executeType == EventStatus.Trade) {
                    tradeState = GradientGridsState.waitForTriggerPrice;
                    lastOrderInfo.state = StrategyOrderInfo.OrderState.finish;
                    if (lastOrderInfo.stopPrice > 0) {
                        // 如果有止损，关掉止损单
                        EventManager.Instance.Send(CancelOrderByClientId.Create(MainOrderDialog.curAccountData, MainOrderDialog.curSymbol,
                            GameConfig.StopPrefix + lastOrderClientId));
                    }
                } else if (executeType == EventStatus.Canceled) {
                    StopStrategy();
                }
            } else if (clientId == GameConfig.StopPrefix + lastOrderClientId) {
                // 止损成功
                if (executeType == EventStatus.Trade) {
                    tradeState = GradientGridsState.waitForTriggerPrice;
                    lastOrderInfo.state = StrategyOrderInfo.OrderState.finish;
                    if (lastOrderInfo.takeProfitPrice > 0) {
                        // 如果有止盈，关掉止盈单
                        EventManager.Instance.Send(CancelOrderByClientId.Create(MainOrderDialog.curAccountData, MainOrderDialog.curSymbol,
                            GameConfig.TakeProfitPrefix + lastOrderClientId));
                    }

                    if (stopStrategyWhenTriggerStopPrice) {
                        StopStrategy();
                    }
                } else if (executeType == EventStatus.Canceled) {
                    StopStrategy();
                }
            }
        }
    }

    private void OnTradeFinish() {
        if (lastOrderInfo.takeProfitPrice > (decimal) float.Epsilon) {
            isPending = true;
            EventManager.Instance.Send(NewOrder.Create(accountData, Utility.GenerateGradientOrderInfo(lastOrderInfo, 2),
                (e) =>
                {
                    if (e == null) {
                        lastOrderInfo.state = StrategyOrderInfo.OrderState.waitComfirmStopOrTakeprofit;
                    }

                    isPending = false;
                }));
        }

        if (lastOrderInfo.stopPrice > (decimal) float.Epsilon) {
            isPending = true;
            EventManager.Instance.Send(NewOrder.Create(accountData, Utility.GenerateGradientOrderInfo(lastOrderInfo, 3),
                (e) =>
                {
                    if (e == null) {
                        lastOrderInfo.state = StrategyOrderInfo.OrderState.waitComfirmStopOrTakeprofit;
                    }

                    isPending = false;
                }));
        }

        if (!isPending) {
            // 如果没有止盈止损单，该单结束，策略结束
            lastOrderInfo.state = StrategyOrderInfo.OrderState.finish;
            StopStrategy();
        }
    }

    public override void OnReconnect() {
        CheckOrderState();
    }

    // 手动检测订单状态，用于断线重连时确认websocket错过的信息
    private void CheckOrderState() {
        if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitForConfirmOrder
            || lastOrderInfo.state == StrategyOrderInfo.OrderState.waitForDeal) {
            EventManager.Instance.Send(GetOrder.Create(accountData, lastOrderInfo.symbol, lastOrderInfo.orderClientId.ToString()));
        } else {
            if (lastOrderInfo.state == StrategyOrderInfo.OrderState.waitDealOfStopOrTakeprofit
                || lastOrderInfo.state == StrategyOrderInfo.OrderState.waitComfirmStopOrTakeprofit) {
                if (lastOrderInfo.stopPrice > (decimal) float.Epsilon) {
                    var clientOrderId = GameConfig.StopPrefix + lastOrderInfo.orderClientId;
                    EventManager.Instance.Send(GetOrder.Create(accountData, lastOrderInfo.symbol, clientOrderId));
                }

                if (lastOrderInfo.takeProfitPrice > (decimal) float.Epsilon) {
                    var clientOrderId = GameConfig.TakeProfitPrefix + lastOrderInfo.orderClientId;
                    EventManager.Instance.Send(GetOrder.Create(accountData, lastOrderInfo.symbol, clientOrderId));
                }
            }
        }
    }
}