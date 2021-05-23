using System.Collections.Generic;
using M3C.Finance.BinanceSdk;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

public class AccountLogic : LogicBase
{
    private Dictionary<AccountData, WebSocketClient_FuturesSigned>  accountClientList;

    public void Init() {
        accountClientList = new Dictionary<AccountData, WebSocketClient_FuturesSigned>();

    }
    
    public void AddAccount(AccountData account) {
        if (!accountClientList.ContainsKey(account)) {
            var client = new WebSocketClient_FuturesSigned();
            //client.ConnectUserDataEndpointSync();
        }
    }

}