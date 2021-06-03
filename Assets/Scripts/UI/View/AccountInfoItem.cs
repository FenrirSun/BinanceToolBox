using System;
using GameEvents;
using M3C.Finance.BinanceSdk.ResponseObjects;
using NLog.LayoutRenderers;
using UnityEngine;
using UnityEngine.UI;

public class AccountInfoItem: MonoBehaviour
{
    public Text nameTxt;
    public Text numTxt;
    // public Text futuresTxt;
    public GameObject activeMark;
    public Button deleteBtn;
    public Button configBtn;
    public Text posTextTemp;
    
    private AccountData _accountData;
    private ComponentEasyCache<Text, FuturesUserDataPositionInfo> positionCache;
    
    public void Init() {
        positionCache = new ComponentEasyCache<Text, FuturesUserDataPositionInfo>();
        positionCache.template = posTextTemp;
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
        configBtn.onClick.RemoveAllListeners();
        configBtn.onClick.AddListener(() =>
        {
            var addAccountDialog = UIManager.Instance.PushDialog<AddAccountDialog>(AddAccountDialog.Prefab);
            addAccountDialog.accountId = _accountData.id;
            addAccountDialog.Init(false);
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

                var tradeInfos = _accountData.GetTradeInfos();
                positionCache.Display(tradeInfos, posTextTemp.transform.parent, (text, tradeInfo, index) =>
                {
                    if (tradeInfo.positionAmt == 0) {
                        return false;
                    }

                    text.text = $"{tradeInfo.symbol}  {tradeInfo.positionSide}  {tradeInfo.positionAmt} 持仓成本:{tradeInfo.entryPrice} 杠杆:{tradeInfo.leverage} 是否逐仓:{tradeInfo.isolated} ";
                    return true;
                });
            }
        }
    }
}