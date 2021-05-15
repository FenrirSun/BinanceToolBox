using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MainPageBarDialog :GameDialogBase
{
    public const string Prefab = "Main_Page_Bar";
    private MainPageBarView _pageBarView;
    
    protected override void SetView(DialogViewBase v) {
        _pageBarView = v as MainPageBarView;
    }

    public void Init()
    {
        // _pageBarView.levelTxt.text = GameRuntime.Instance.UserData.level.ToString();
    }

    private void AddListener()
    {
        _pageBarView.marketBtn.onClick.AddListener(SwitchToMarket);
    }

    private void SwitchToMarket()
    {
        
    }
}