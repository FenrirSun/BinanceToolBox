using System;
using System.Collections.Generic;
using M3C.Finance.BinanceSdk;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

public class UserData : UserDataBase
{
    public List<AccountData> accountDataList;
    public int newestId;

    public UserData() : base() {
        accountDataList = new List<AccountData>();
    }

    public void Clear() {
    }

    public int GetNewGuid() {
        if (newestId >= int.MaxValue)
            newestId = 1;
        newestId += 1;
        MarkDirty();
        return newestId;
    }

    public AccountData AddAccount(string name, string api_key, string secret_key) {
        var newAccount = new AccountData();
        newAccount.id = GameUtils.GetNewGuid();
        newAccount.name = name;
        newAccount.apiKey = api_key;
        newAccount.secretKey = secret_key;

        accountDataList.Add(newAccount);
        MarkDirty();
        return newAccount;
    }

    public AccountData GetAccount(int id) {
        foreach (var accountData in accountDataList) {
            if (accountData.id == id) {
                return accountData;
            }
        }

        return null;
    }

    public void RemoveAccount(int id) {
        foreach (var accountData in accountDataList) {
            if (accountData.id == id) {
                accountDataList.Remove(accountData);
                MarkDirty();
                return;
            }
        }
    }

    public void OnGetBalance(AccountData ad, FuturesUserDataAccountBalanceMessage info) {
        ad.SetBalanceInfo(info);
    }
    
    public void OnGetAccountInfo(AccountData ad, FuturesUserDataAccountInfoMessage info) {
        var positions = info.positions;
        var adTradeInfos = ad.GetTradeInfos();
        if (positions != null && positions.Count > 0) {
            foreach (var position in positions) {
                adTradeInfos.Add(position);
            }
        }
    }
}

[Serializable]
public class AccountData
{
    public int id;
    public string name;
    public string apiKey;
    public string secretKey;

    // 资产信息
    [NonSerialized]
    private FuturesUserDataAccountBalanceMessage BalanceInfo;
    public void SetBalanceInfo(FuturesUserDataAccountBalanceMessage info) {
        BalanceInfo = info;
    }
    
    public FuturesUserDataAccountBalanceMessage GetBalanceInfo() {
        return BalanceInfo;
    }
    
    // 仓位信息
    [NonSerialized]
    private List<FuturesUserDataPositionInfo> TradeInfos;
    public List<FuturesUserDataPositionInfo> GetTradeInfos() {
        if(TradeInfos == null)
            TradeInfos = new List<FuturesUserDataPositionInfo>();
        return TradeInfos;
    }
    
    // 订单信息
    [NonSerialized]
    private List<FuturesUserDataOpenOrderInfoMessage> OrderInfos;
    public List<FuturesUserDataOpenOrderInfoMessage> GetOrderInfos() {
        if(OrderInfos == null)
            OrderInfos = new List<FuturesUserDataOpenOrderInfoMessage>();

        return OrderInfos;
    }
    
    public List<FuturesUserDataOpenOrderInfoMessage> GetOrderInfos(SymbolType symbol) {
        if(OrderInfos == null)
            OrderInfos = new List<FuturesUserDataOpenOrderInfoMessage>();

        List<FuturesUserDataOpenOrderInfoMessage> result = new List<FuturesUserDataOpenOrderInfoMessage>();
        foreach (var order in OrderInfos) {
            if (order.symbol.Value == symbol) {
                result.Add(order);
            }
        }
        return result;
    }
}
