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
        List<string> options = new List<string>();
        foreach (var type in SymbolType.Types) {
            options.Add(type);
        }
        _view.symbolDropdown.AddOptions(options);
        _view.symbolDropdown.value = 0;
        OnSelectSymbol(0);
        _view.symbolDropdown.onValueChanged.AddListener(OnSelectSymbol);
        AddListener();
    }

    public void OnSelectSymbol(int index) {
        curSymbol = SymbolType.Types[index];
        return;
        GetEventComp().Send(ListenTradesMessage.Create(curSymbol));
    }
    
    private void AddListener() {
        GetEventComp().Listen<OnAggTradeUpdate>(evt =>
        {
            if (evt.msg.Symbol == curSymbol) {
                _view.curPrice.text = evt.msg.Price.ToString();
            }
        });
    }

    private void SwitchToMarket() {
    }
}