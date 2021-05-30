using System;
using GameEvents;
using M3C.Finance.BinanceSdk.ResponseObjects;
using NLog.LayoutRenderers;
using UnityEngine;
using UnityEngine.UI;

public class OrderInfoItem: MonoBehaviour
{
    public Text timeTxt;
    public Text symbolTxt;
    public Text orderDirTxt;
    public Text positionDirTxt;
    public Text typeTxt;
    public Text aPriceTxt;
    public Text quantityTxt;
    public Text triggerTxt;
    public Text stateTxt;
    public Button operateBtn;
    
    private FuturesUserDataOpenOrderInfoMessage _orderData;
    
    public void Init() {
        
    }

    public void SetItemData(FuturesUserDataOpenOrderInfoMessage orderInfo) {
        _orderData = orderInfo;
        UpdateData();
        operateBtn.onClick.RemoveAllListeners();
        operateBtn.onClick.AddListener(() =>
        {
            EventManager.Instance.Send(RefreshAccountList.Create());
        });
    }

    private float lastUpdateTime;
    private void Update() {
        if (Time.time - lastUpdateTime > 0.5f) {
            lastUpdateTime = Time.time;
            UpdateData();
        }
    }

    private void UpdateData() {
        timeTxt.text = TimeUtils.ToDateTime((long)(_orderData.time * 0.001f)).ToString("yyyy/MM/dd HH:mm:ss");
        symbolTxt.text = _orderData.symbol;
        orderDirTxt.text = _orderData.side;
        positionDirTxt.text = _orderData.positionSide;
        typeTxt.text = _orderData.type;
        aPriceTxt.text = _orderData.price.ToString();
        quantityTxt.text = _orderData.origQty.ToString();
        triggerTxt.text = _orderData.workingType;
        stateTxt.text = _orderData.status;
    }
}