using System;
using System.Collections;
using System.Collections.Generic;
using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

[Serializable]
public class StrategySingleTrade : StrategyBase
{
    
    public override void Init(AccountData ad, StrategyOrderInfo firstOrder) {
        historyOrderList = new List<StrategyOrderInfo>();
        base.Init(ad, firstOrder);
    }

    public override void StartStrategy() {
        base.StartStrategy();
        lastOrderInfo = firstOrderInfo.Clone();
        lastOrderInfo.state = StrategyOrderInfo.OrderState.idle;
        historyOrderList.Add(lastOrderInfo);
        SendNextOrder();
    }
    
    private void SendNextOrder() {
        lastOrderInfo.orderClientId = GameUtils.GetNewGuid();
        var quantity = firstOrderInfo.quantity * (decimal) accountData.orderRatio;
        lastOrderInfo.quantity = decimal.Parse(quantity.ToString("G0"));
        lastOrderInfo.state = StrategyOrderInfo.OrderState.waitForConfirmOrder;

        if (lastOrderInfo.quantity > (decimal)float.Epsilon) {
            GameRuntime.Instance.StartCoroutine(SendOrder());
        }
        StopStrategy();
    }

    IEnumerator SendOrder() {
        yield return null;
        
        var newOrder = Utility.GenerateSimpleOrderInfo(lastOrderInfo);
        EventManager.Instance.Send(NewOrder.Create(accountData, newOrder, e =>
        {
            if (e == null) {
                // tradeState = GradientGridsState.holdOrder;
            } else {
                // GameRuntime.Instance.StartCoroutine(SendOrder());
                CommonMessageDialog.OpenWithOneButton($"下单失败，{e.Message}", null);
            }
        }));
    }

    // // 价格变更
    // protected override void OnPriceChange(decimal price) { }
    //
    // // 推送交易变更
    // protected override void OnWsDataOrderTradeUpdate(WsFuturesUserDataOrderTradeUpdateMessage msg) {
    //     if(state == StrategyState.Executing)
    //         StopStrategy();
    // }
    //
    // // 交易变更
    // protected override void OnDataOrderTradeUpdate(GetOrderResponse msg) {
    //     if(state == StrategyState.Executing)
    //         StopStrategy();
    // }
}