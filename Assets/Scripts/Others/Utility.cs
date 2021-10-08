using System;
using System.ComponentModel;
using System.Reflection;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

public static class Utility
{
    public static OrderInfo GenerateSimpleOrderInfo(StrategyOrderInfo strategyOrder) {
        OrderInfo newOrder = new OrderInfo();
        newOrder.Symbol = strategyOrder.symbol;
        newOrder.Price = strategyOrder.pendingPrice;
        newOrder.OriginalQuantity = strategyOrder.quantity;
        newOrder.PositionSide = strategyOrder.posSide;
        newOrder.ClientOrderId = strategyOrder.orderClientId.ToString();
        // 迷惑项，把可读参数转化为实际传递的参数
        if (strategyOrder.side == OrderSide.Buy) {
            if (strategyOrder.posSide == PositionSide.LONG) {
                newOrder.OrderSide = OrderSide.Buy;
            } else if(strategyOrder.posSide == PositionSide.SHORT) {
                newOrder.OrderSide = OrderSide.Sell;
            }
        } else {
            if (strategyOrder.posSide == PositionSide.LONG) {
                newOrder.OrderSide = OrderSide.Sell;
            } else if(strategyOrder.posSide == PositionSide.SHORT) {
                newOrder.OrderSide = OrderSide.Buy;
            }
        }

        newOrder.reduceOnly = strategyOrder.side == OrderSide.Sell ? "true" : "false";
        newOrder.OrderType = strategyOrder.orderType;
        return newOrder;
        
        // if (orderType == 1) {
        //     // 首单
        //     if (strategyOrder.posSide == PositionSide.LONG) {
        //         newOrder.OrderSide = OrderSide.Buy;
        //     } else if(strategyOrder.posSide == PositionSide.SHORT) {
        //         newOrder.OrderSide = OrderSide.Sell;
        //     }
        //     newOrder.OrderType = strategyOrder.orderType;
        // } else if (orderType == 2) {
        //     // 止盈单
        //     // newOrder.reduceOnly = "true";
        //     newOrder.OrderType = OrderType.TAKE_PROFIT;
        //     if (strategyOrder.posSide == PositionSide.LONG) {
        //         newOrder.OrderSide = OrderSide.Sell;
        //     } else if(strategyOrder.posSide == PositionSide.SHORT) {
        //         newOrder.OrderSide = OrderSide.Buy;
        //     }
        //     newOrder.PositionSide = strategyOrder.posSide;
        //     newOrder.StopPrice = strategyOrder.takeProfitPrice;
        //     newOrder.Price = strategyOrder.takeProfitPrice;
        //     newOrder.ClientOrderId = GameConfig.TakeProfitPrefix + strategyOrder.orderClientId.ToString();
        // } else if (orderType == 3) {
        //     // 止损单
        //     // newOrder.reduceOnly = "true";
        //     newOrder.OrderType = OrderType.STOP;
        //     if (strategyOrder.posSide == PositionSide.LONG) {
        //         newOrder.OrderSide = OrderSide.Sell;
        //     } else if(strategyOrder.posSide == PositionSide.SHORT) {
        //         newOrder.OrderSide = OrderSide.Buy;
        //     }
        //     newOrder.PositionSide = strategyOrder.posSide;
        //     newOrder.StopPrice = strategyOrder.stopPrice;
        //     newOrder.Price = strategyOrder.stopPrice;
        //     newOrder.ClientOrderId = GameConfig.StopPrefix + strategyOrder.orderClientId.ToString();
        // }

        // return newOrder;
    }
    
    // 梯度策略中的生成订单信息
    public static OrderInfo GenerateGradientOrderInfo(StrategyOrderInfo strategyOrder, int orderType) {
        OrderInfo newOrder = new OrderInfo();
        newOrder.Symbol = strategyOrder.symbol;
        newOrder.Price = strategyOrder.pendingPrice;
        newOrder.OriginalQuantity = strategyOrder.quantity;
        newOrder.PositionSide = strategyOrder.posSide;
        newOrder.ClientOrderId = strategyOrder.orderClientId.ToString();
        if (orderType == 1) {
            // 首单
            if (strategyOrder.posSide == PositionSide.LONG) {
                newOrder.OrderSide = OrderSide.Buy;
            } else if(strategyOrder.posSide == PositionSide.SHORT) {
                newOrder.OrderSide = OrderSide.Sell;
            }
            newOrder.OrderType = strategyOrder.orderType;
        } else if (orderType == 2) {
            // 止盈单
            // newOrder.reduceOnly = "true";
            newOrder.OrderType = OrderType.TAKE_PROFIT;
            if (strategyOrder.posSide == PositionSide.LONG) {
                newOrder.OrderSide = OrderSide.Sell;
            } else if(strategyOrder.posSide == PositionSide.SHORT) {
                newOrder.OrderSide = OrderSide.Buy;
            }
            newOrder.PositionSide = strategyOrder.posSide;
            newOrder.StopPrice = strategyOrder.takeProfitPrice;
            newOrder.Price = strategyOrder.takeProfitPrice;
            newOrder.ClientOrderId = GameConfig.TakeProfitPrefix + strategyOrder.orderClientId.ToString();
        } else if (orderType == 3) {
            // 止损单
            // newOrder.reduceOnly = "true";
            newOrder.OrderType = OrderType.STOP;
            if (strategyOrder.posSide == PositionSide.LONG) {
                newOrder.OrderSide = OrderSide.Sell;
            } else if(strategyOrder.posSide == PositionSide.SHORT) {
                newOrder.OrderSide = OrderSide.Buy;
            }
            newOrder.PositionSide = strategyOrder.posSide;
            newOrder.StopPrice = strategyOrder.stopPrice;
            newOrder.Price = strategyOrder.stopPrice;
            newOrder.ClientOrderId = GameConfig.StopPrefix + strategyOrder.orderClientId.ToString();
        }

        return newOrder;
    }
}