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
            newOrder.OrderSide = strategyOrder.side;
            newOrder.OrderType = strategyOrder.orderType;
        } else if (orderType == 2) {
            // 止盈单
            newOrder.reduceOnly = true;
            newOrder.OrderType = OrderType.TAKE_PROFIT;
            newOrder.OrderSide = strategyOrder.posSide == PositionSide.LONG ? OrderSide.Sell : OrderSide.Buy;
            newOrder.StopPrice = strategyOrder.stopPrice;
            newOrder.Price = strategyOrder.stopPrice;
            newOrder.ClientOrderId = GameConfig.TakeProfitPrefix + strategyOrder.orderClientId.ToString();
        } else if (orderType == 3) {
            // 止损单
            newOrder.reduceOnly = true;
            newOrder.OrderType = OrderType.STOP;
            newOrder.OrderSide = strategyOrder.posSide == PositionSide.LONG ? OrderSide.Sell : OrderSide.Buy;
            newOrder.StopPrice = strategyOrder.takeProfitPrice;
            newOrder.Price = strategyOrder.takeProfitPrice;
            newOrder.ClientOrderId = GameConfig.StopPrefix + strategyOrder.orderClientId.ToString();
        }

        return newOrder;
    }
    
    public static T DeepCloneObject<T>(this T t) where T : class
    {
        T model = Activator.CreateInstance<T>();                     //实例化一个T类型对象
        PropertyInfo[] propertyInfos = model.GetType().GetProperties();     //获取T对象的所有公共属性
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
            //判断值是否为空，如果空赋值为null见else
            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                //如果convertsionType为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                NullableConverter nullableConverter = new NullableConverter(propertyInfo.PropertyType);
                //将convertsionType转换为nullable对的基础基元类型
                propertyInfo.SetValue(model, Convert.ChangeType(propertyInfo.GetValue(t), nullableConverter.UnderlyingType), null);
            }
            else
            {
                propertyInfo.SetValue(model, Convert.ChangeType(propertyInfo.GetValue(t), propertyInfo.PropertyType), null);
            }
        }
        return model;
    }
}