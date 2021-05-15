using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MainMarketDialog :MainPageBase
{
    public const string Prefab = "Main_Market_Dialog";
    private MainMarketView _view;
    
    protected override void SetView(DialogViewBase v) {
        _view = v as MainMarketView;
    }

    public void Init()
    {
        // _pageBarView.levelTxt.text = GameRuntime.Instance.UserData.level.ToString();
    }

    private void AddListener()
    {
        _view.marketBtn.onClick.AddListener(SwitchToMarket);
    }

    private void SwitchToMarket()
    {
        
    }
}