using System;
using System.Collections.Generic;
using M3C.Finance.BinanceSdk;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

[Serializable]
public class AccountData
{
    public int id;
    public string name;
    public string apiKey;
    public string secretKey;
    public float orderRatio;
    
    // 资产信息
    [NonSerialized]
    private FuturesUserDataAccountBalanceMessage balanceInfo;
    public void SetBalanceInfo(FuturesUserDataAccountBalanceMessage info) {
        balanceInfo = info;
    }
    
    public FuturesUserDataAccountBalanceMessage GetBalanceInfo() {
        return balanceInfo;
    }
    
    // 仓位/头寸 信息
    [NonSerialized]
    private List<FuturesUserDataPositionInfo> positionInfos;
    public List<FuturesUserDataPositionInfo> GetPositionInfos() {
        if(positionInfos == null)
            positionInfos = new List<FuturesUserDataPositionInfo>();
        
        return positionInfos;
    }
    
    // 订单信息
    [NonSerialized]
    private List<FuturesUserDataOpenOrderInfoMessage> orderInfos;
    public List<FuturesUserDataOpenOrderInfoMessage> GetOrderInfos() {
        if(orderInfos == null)
            orderInfos = new List<FuturesUserDataOpenOrderInfoMessage>();

        return orderInfos;
    }
    
    public List<FuturesUserDataOpenOrderInfoMessage> GetOrderInfos(SymbolType symbol) {
        if(orderInfos == null)
            orderInfos = new List<FuturesUserDataOpenOrderInfoMessage>();

        List<FuturesUserDataOpenOrderInfoMessage> result = new List<FuturesUserDataOpenOrderInfoMessage>();
        foreach (var order in orderInfos) {
            if (order.symbol.Value == symbol) {
                result.Add(order);
            }
        }
        return result;
    }
}