using System;
using System.ComponentModel;
using System.Reflection;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

public static class Utility
{
    public static OrderInfo GenerateOrderInfo(StrategyOrderInfo strategyOrder, int orderType) {
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