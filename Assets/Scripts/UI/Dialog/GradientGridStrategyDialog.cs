using System;
using System.Collections.Generic;
using System.Text;
using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;
using UnityEngine;
using UnityEngine.UI;

public class GradientGridStrategyDialog : GameDialogBase
{
    public const string Prefab = "Gradient_Grid_Strategy_Dialog";
    private GradientGridStrategyView _view;

    protected override void SetView(DialogViewBase v) {
        _view = v as GradientGridStrategyView;
    }

    public void Init() {
        AddListener();

        var msg = GetLastTradeMessage.Create(MainOrderDialog.curSymbol);
        GetEventComp().Send(msg);
        if (msg.message != null) {
            _view.priceInput.text = msg.message.Price.ToString();
        }

        _view.SymbolTxt.text = MainOrderDialog.curSymbol;
        _view.directionDropdown.ClearOptions();
        List<string> sideOptions = new List<string>();
        sideOptions.Add(PositionSide.LONG);
        sideOptions.Add(PositionSide.SHORT);

        _view.directionDropdown.AddOptions(sideOptions);
        _view.directionDropdown.value = 0;
    }

    private void AddListener() {
        _view.closeBtn.onClick.AddListener(OnClose);
        _view.startBtn.onClick.AddListener(ClickStartStrategy);
        _view.priceInput.onValueChanged.AddListener((str) =>
        {
            UpdateNextTriggerPriceTips();
            UpdateNextOrderPriceTips();
        });
        _view.quantityInput.onValueChanged.AddListener((str) => { UpdateNextQuantityTips(); });
        _view.triggerPriceGapInput.onValueChanged.AddListener((str) => { UpdateNextTriggerPriceTips(); });
        _view.orderPriceGapInput.onValueChanged.AddListener((str) => { UpdateNextOrderPriceTips(); });
        _view.quantityRatioGapInput.onValueChanged.AddListener((str) => { UpdateNextQuantityTips(); });
    }

    private void UpdateNextTriggerPriceTips() {
        if (decimal.TryParse(_view.priceInput.text, out var price)) {
            if (decimal.TryParse(_view.triggerPriceGapInput.text, out var triggerPriceGap)) {
                _view.nextTriggerPriceTxt.text = $"下一单触发价格：{price + triggerPriceGap}";
            }
        }
    }

    private void UpdateNextOrderPriceTips() {
        if (decimal.TryParse(_view.priceInput.text, out var price)) {
            if (decimal.TryParse(_view.orderPriceGapInput.text, out var orderPriceGap)) {
                _view.nextOrderPriceTxt.text = $"下一单挂单价格：{price + orderPriceGap}";
            }
        }
    }

    private void UpdateNextQuantityTips() {
        if (decimal.TryParse(_view.quantityInput.text, out var quantity)) {
            if (decimal.TryParse(_view.quantityRatioGapInput.text, out var quantityRatio)) {
                _view.nextQuantityTxt.text = $"下一单挂单数量：{quantity * (1 + quantityRatio)}";
            }
        }
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

            StrategyGradientGridsTrade strategy = new StrategyGradientGridsTrade();
            if (decimal.TryParse(_view.triggerPriceGapInput.text, out var triggerPriceGap)) {
                strategy.triggerPriceGap = triggerPriceGap;
            } else {
                CommonMessageDialog.OpenWithOneButton("下一单触发价格格式错误", null);
                return;
            }

            if (decimal.TryParse(_view.orderPriceGapInput.text, out var orderPriceGap)) {
                strategy.orderPriceGap = orderPriceGap;
            } else {
                CommonMessageDialog.OpenWithOneButton("下一单挂单价格格式错误", null);
                return;
            }

            if (decimal.TryParse(_view.quantityRatioGapInput.text, out var quantityRatioGap)) {
                strategy.quantityRatioGap = quantityRatioGap;
            } else {
                CommonMessageDialog.OpenWithOneButton("下一单数量间隔格式错误", null);
                return;
            }

            if (decimal.TryParse(_view.maxOrderPriceInput.text, out var maxOrderPrice)) {
                strategy.maxOrderPrice = maxOrderPrice;
            } else {
                CommonMessageDialog.OpenWithOneButton("最高挂单价格格式错误", null);
                return;
            }

            strategy.stopStrategyWhenTriggerStopPrice = _view.stopToggle.isOn;
            StrategyOrderInfo orderInfo = new StrategyOrderInfo();
            orderInfo.symbol = MainOrderDialog.curSymbol;
            orderInfo.pendingPrice = price;
            orderInfo.quantity = quantity;
            orderInfo.stopPrice = stopPrice;
            orderInfo.takeProfitPrice = takeProfitPrice;
            orderInfo.posSide = _view.directionDropdown.value == 0 ? PositionSide.LONG : PositionSide.SHORT;
            orderInfo.side = OrderSide.Buy;
            orderInfo.orderType = OrderType.Limit;
            orderInfo.orderClientId = GameUtils.GetNewGuid();
            strategy.Init(MainOrderDialog.curAccountData, orderInfo);
            GetEventComp().Send(StartStrategyEvent.Create(orderInfo.symbol, strategy));
            OnClose();
        }
    }

    public override void OnClose() {
        base.OnClose();
    }
}