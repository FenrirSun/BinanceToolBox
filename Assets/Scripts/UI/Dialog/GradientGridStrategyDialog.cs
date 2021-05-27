using System;
using System.Collections.Generic;
using System.Text;
using GameEvents;
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
    }

    private void AddListener() {
        _view.closeBtn.onClick.AddListener(OnClose);
        _view.startBtn.onClick.AddListener(StartStrategy);
    }

    private void StartStrategy() {
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
            
            
        }
    }

    public override void OnClose() {
        base.OnClose();
    }
}