using System;
using System.Collections.Generic;
using M3C.Finance.BinanceSdk;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;
using UnityEngine;

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

    public AccountData AddAccount(string name, string api_key, string secret_key, float order_ratio) {
        var newAccount = new AccountData();
        newAccount.id = GameUtils.GetNewGuid();
        newAccount.name = name;
        newAccount.apiKey = api_key;
        newAccount.secretKey = secret_key;
        newAccount.orderRatio = order_ratio;
        
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
        var adTradeInfos = ad.GetPositionInfos();
        if (positions != null && positions.Count > 0) {
            foreach (var position in positions) {
                var oldInfo = adTradeInfos.Find((t) => t.symbol == position.symbol && t.positionSide == position.positionSide);
                if (oldInfo != null) {
                    adTradeInfos.Remove(oldInfo);
                }
                    
                adTradeInfos.Add(position);
            }
        }
    }
}
