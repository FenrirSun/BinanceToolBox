using System;
using GameEvents;
using M3C.Finance.BinanceSdk.ResponseObjects;
using NLog.LayoutRenderers;
using UnityEngine;
using UnityEngine.UI;

public class AccountInfoItem : MonoBehaviour
{
    public Text nameTxt;
    public Text numTxt;
    public Text orderRatioTxt;
    // public GameObject activeMark;
    public Button updateBtn;
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

        updateBtn.onClick.RemoveAllListeners();
        updateBtn.onClick.AddListener(() => 
        {
            EventManager.Instance.Send(UpdateAccountInfo.Create(data));
        });
        
        configBtn.onClick.RemoveAllListeners();
        configBtn.onClick.AddListener(() =>
        {
            var addAccountDialog = UIManager.Instance.PushDialog<AddAccountDialog>(AddAccountDialog.Prefab);
            addAccountDialog.Init(data);
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
                    orderRatioTxt.text = _accountData.orderRatio.ToString();
                }

                var tradeInfos = _accountData.GetPositionInfos();
                positionCache.Display(tradeInfos, posTextTemp.transform.parent, (text, tradeInfo, index) =>
                {
                    if (tradeInfo.positionAmt == 0) {
                        return false;
                    }

                    text.text =
                        $"{tradeInfo.symbol}  {tradeInfo.positionSide}  {tradeInfo.positionAmt} 持仓成本:{tradeInfo.entryPrice} 杠杆:{tradeInfo.leverage} 是否逐仓:{tradeInfo.isolated} ";
                    return true;
                });
            }
        }
    }
}