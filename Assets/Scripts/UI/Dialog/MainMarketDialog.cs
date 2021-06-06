using System;
using System.Collections.Generic;
using System.Text;
using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class MainMarketDialog : MainPageBase
{
    public const string Prefab = "Main_Market_Dialog";
    private MainMarketView _view;
    private SymbolType curSymbol;
    
    protected override void SetView(DialogViewBase v) {
        _view = v as MainMarketView;
    }

    public void Init() {
        _view.symbolDropdown.ClearOptions();
        List<string> symbolOptions = new List<string>();
        foreach (var type in SymbolType.Types) {
            symbolOptions.Add(type);
        }
        _view.symbolDropdown.AddOptions(symbolOptions);
        _view.symbolDropdown.value = 0;
        OnSelectSymbol(0);
        _view.symbolDropdown.onValueChanged.AddListener(OnSelectSymbol);
        
        List<string> intervalOptions = new List<string>();
        foreach (var value in KlineInterval.Values) {
            intervalOptions.Add(value);
        }
        _view.intervalDropdown.AddOptions(intervalOptions);
        _view.intervalDropdown.value = 0;
        OnSelectInterval(0);
        _view.intervalDropdown.onValueChanged.AddListener(OnSelectInterval);
        
        AddListener();
    }

    private void OnSelectSymbol(int index) {
        curSymbol = SymbolType.Types[index];
        var msg = GetLastTradeMessage.Create(curSymbol);
        GetEventComp().Send(msg);
        if (msg.message != null) {
            _view.curPrice.text = msg.message.Price.ToString();
        } else {
            _view.curPrice.text = "-";
        }
    }
    
    private void OnSelectInterval(int index) {
        if (StreamDataLogic.curKlineInterval == KlineInterval.Values[index])
            return;
        
        StreamDataLogic.curKlineInterval = KlineInterval.Values[index];
        var msg = SubscribeKLine.Create(curSymbol);
        GetEventComp().Send(msg);
    }
    
    private void AddListener() {
        GetEventComp().Listen<OnAggTradeUpdate>(evt =>
        {
            if (evt.msg.Symbol.Value == curSymbol) {
                _view.curPrice.text = evt.msg.Price.ToString();
            }
        });
    }

    private void SwitchToMarket() {
    }
}