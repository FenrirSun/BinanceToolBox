using System;
using GameEvents;
using NLog.LayoutRenderers;
using UnityEngine;
using UnityEngine.UI;

public class AccountInfoItem: MonoBehaviour
{
    public Text nameTxt;
    public Text numTxt;
    public Text futuresTxt;
    public GameObject activeMark;
    public Button deleteBtn;
    private AccountData _accountData;
    
    public void Init() {
        
    }

    public void SetItemData(AccountData data) {
        _accountData = data;
        nameTxt.text = data.name;
        deleteBtn.onClick.RemoveAllListeners();
        deleteBtn.onClick.AddListener(() =>
        {
            GameRuntime.Instance.UserData.RemoveAccount(data.id);
            EventManager.Instance.Send(RefreshAccountList.Create());
        });
    }

    private void Update() {
        if (gameObject.activeInHierarchy && _accountData != null) {
            var msg = GetLastTradeMessage.Create(MainAccountDialog.curSymbol);
            EventManager.Instance.Send(msg);
            if (msg.message != null) {
                //curPrice.text = msg.message.Price.ToString();
            }
        }
    }
}