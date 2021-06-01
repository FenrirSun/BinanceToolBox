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
            var strategyLogic = GameRuntime.Instance.GetLogic<StrategyLogic>();
            if (strategyLogic.IsRunningStrategy()) {
                CommonMessageDialog.OpenWithOneButton("当前有执行中的策略，不可删除账户", null);
                return;
            }
            
            GameRuntime.Instance.GetLogic<AccountLogic>().DeleteAccount(data);
            EventManager.Instance.Send(RefreshAccountList.Create());
        });
    }

    private float lastUpdateTime;
    private void Update() {
        if (Time.time - lastUpdateTime > 1) {
            lastUpdateTime = Time.time;
            if (gameObject.activeInHierarchy && _accountData != null) {
                var balanceInfo = _accountData.GetBalanceInfo();
                if (balanceInfo != null) {
                    var balanceStr = balanceInfo.Balance.ToString();
                    numTxt.text = balanceStr;
                }
            }
        }
    }
}