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
    private SymbolType[] curSymbols;
    
    protected override void SetView(DialogViewBase v) {
        _view = v as MainMarketView;
    }

    public void Init() {
        curSymbols = new SymbolType[_view.symbolDropdowns.Length];
        List<string> symbolOptions = new List<string>();
        foreach (var type in SymbolType.Types) {
            symbolOptions.Add(type);
        }
        for (int i = 0; i < _view.symbolDropdowns.Length; ++i) {
            var dropDown = _view.symbolDropdowns[i];
            dropDown.ClearOptions();
            dropDown.AddOptions(symbolOptions);
            int index = i;
            dropDown.value = index;
            dropDown.onValueChanged.AddListener((value) =>
            {
                OnSelectSymbol(index, value);
            });
            OnSelectSymbol(index, dropDown.value);
        }

        // List<string> intervalOptions = new List<string>();
        // foreach (var value in KlineInterval.Values) {
        //     intervalOptions.Add(value);
        // }
        // _view.intervalDropdown.AddOptions(intervalOptions);
        // _view.intervalDropdown.value = 0;
        // OnSelectInterval(0);
        // _view.intervalDropdown.onValueChanged.AddListener(OnSelectInterval);
        
        AddListener();
    }

    private void OnSelectSymbol(int index, int value) {
        var symbol = SymbolType.Types[value];
        curSymbols[index] = symbol;
        var msg = GetLastTradeMessage.Create(symbol);
        GetEventComp().Send(msg);
        if (msg.message != null) {
            _view.curPrices[index].text = msg.message.Price.ToString();
        } else {
            _view.curPrices[index].text = "-";
        }
    }
    
    private void OnSelectInterval(int index) {
        if (StreamDataLogic.curKlineInterval == KlineInterval.Values[index])
            return;
        
        StreamDataLogic.curKlineInterval = KlineInterval.Values[index];
        var msg = SubscribeKLine.Create(SymbolType.Types[index]);
        GetEventComp().Send(msg);
    }
    
    private void AddListener() {
        GetEventComp().Listen<OnAggTradeUpdate>(evt =>
        {
            for (int i = 0; i < curSymbols.Length; ++i) {
                var curSymbol = curSymbols[i];
                if (evt.msg.Symbol.Value == curSymbol) {
                    _view.curPrices[i].text = evt.msg.Price.ToString();
                }
            }
        });
    }
}