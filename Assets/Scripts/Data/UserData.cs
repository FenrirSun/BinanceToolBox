using System;
using System.Collections.Generic;

public class UserData : UserDataBase
{
    public List<AccountData> accountDataList;
    public int newestId;
    
    public UserData() : base()
    {
        accountDataList = new List<AccountData>();
    }

    public void Clear()
    {

    }

    public int GetNewGuid() {
        if (newestId >= int.MaxValue)
            newestId = 1;
        newestId += 1;
        MarkDirty();
        return newestId;
    }
    
    public void AddAccount(string name, string api_key, string secret_key) {
        var newAccount = new AccountData();
        newAccount.id = GameUtils.GetNewGuid();
        newAccount.name = name;
        newAccount.apiKey = api_key;
        newAccount.secretKey = secret_key;
        
        accountDataList.Add(newAccount);
        MarkDirty();
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
}

[Serializable]
public class AccountData
{
    public int id;
    public string name;
    public string apiKey;
    public string secretKey;
}