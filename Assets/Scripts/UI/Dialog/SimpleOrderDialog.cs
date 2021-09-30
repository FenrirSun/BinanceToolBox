using System;
using System.Collections.Generic;
using System.Text;
using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;
using UnityEngine;
using UnityEngine.UI;

public class SimpleOrderDialog : GameDialogBase
{
    public const string Prefab = "Simple_Order_Dialog";
    private SimpleOrderView _view;

    protected override void SetView(DialogViewBase v) {
        _view = v as SimpleOrderView;
    }

    public void Init() {
        AddListener();

        var msg = GetLastTradeMessage.Create(MainOrderDialog.curSymbol);
        GetEventComp().Send(msg);
        if (msg.message != null) {
            _view.priceInput.text = msg.message.Price.ToString();
        }

        _view.symbolTxt.text = MainOrderDialog.curSymbol;

        _view.sellOrBuyDropdown.ClearOptions();
        List<string> sellOrBuyOptions = new List<string>();
        sellOrBuyOptions.Add("开单");
        sellOrBuyOptions.Add("平单");
        _view.sellOrBuyDropdown.AddOptions(sellOrBuyOptions);
        _view.sellOrBuyDropdown.value = 0;

        _view.directionDropdown.ClearOptions();
        List<string> sideOptions = new List<string>();
        sideOptions.Add("多");
        sideOptions.Add("空");
        _view.directionDropdown.AddOptions(sideOptions);
        _view.directionDropdown.value = 0;

        _view.orderTypeDropdown.ClearOptions();
        List<string> orderTypeOptions = new List<string>();
        orderTypeOptions.Add("限价单");
        orderTypeOptions.Add("止盈单");
        orderTypeOptions.Add("止损单");
        _view.orderTypeDropdown.AddOptions(orderTypeOptions);
        _view.orderTypeDropdown.value = 0;
        _view.orderTypeDropdown.onValueChanged.AddListener((index) =>
        {
            if (index == 0) {
                _view.takeProfitInput.gameObject.SetActive(false);
                _view.stopInput.gameObject.SetActive(false);
            } else if (index == 1) {
                _view.takeProfitInput.gameObject.SetActive(true);
                _view.stopInput.gameObject.SetActive(false);
            } else if (index == 2) {
                _view.takeProfitInput.gameObject.SetActive(false);
                _view.stopInput.gameObject.SetActive(true);
            }
        });
    }

    private void AddListener() {
        _view.closeBtn.onClick.AddListener(OnClose);
        _view.startBtn.onClick.AddListener(ClickStartStrategy);
    }

    private void ClickStartStrategy() {
        if (decimal.TryParse(_view.priceInput.text, out var price)
            && decimal.TryParse(_view.quantityInput.text, out var quantity)) {
            decimal stopPrice = 0;
            if (!string.IsNullOrEmpty(_view.stopInput.text)) {
                decimal.TryParse(_view.stopInput.text, out stopPrice);
            }

            decimal takeProfitPrice = 0;
            if (!string.IsNullOrEmpty(_view.takeProfitInput.text)) {
                decimal.TryParse(_view.takeProfitInput.text, out takeProfitPrice);
            }

            var accounts = GameRuntime.Instance.UserData.accountDataList;
            foreach (var account in accounts) {
                if (account.orderRatio > 0) {
                    var strategy = new StrategySingleTrade();

                    StrategyOrderInfo orderInfo = new StrategyOrderInfo();
                    orderInfo.symbol = MainOrderDialog.curSymbol;
                    orderInfo.pendingPrice = price;
                    orderInfo.quantity = quantity;
                    orderInfo.stopPrice = stopPrice;
                    orderInfo.takeProfitPrice = takeProfitPrice;
                    orderInfo.posSide = _view.directionDropdown.value == 0 ? PositionSide.LONG : PositionSide.SHORT;
                    orderInfo.side = _view.sellOrBuyDropdown.value == 0 ? OrderSide.Buy : OrderSide.Sell;
                    if (_view.orderTypeDropdown.value == 1) {
                        orderInfo.orderType = OrderType.TAKE_PROFIT;
                    } else if (_view.orderTypeDropdown.value == 2) {
                        orderInfo.orderType = OrderType.STOP;
                    } else {
                        orderInfo.orderType = OrderType.Limit;
                    }

                    orderInfo.orderClientId = GameUtils.GetNewGuid();
                    strategy.Init(account, orderInfo);
                    GetEventComp().Send(StartStrategyEvent.Create(orderInfo.symbol, strategy));
                }
            }

            OnClose();
        }
    }

    public override void OnClose() {
        base.OnClose();
    }
}