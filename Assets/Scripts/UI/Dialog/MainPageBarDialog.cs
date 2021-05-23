using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MainPageBarDialog :GameDialogBase
{
    public const string Prefab = "Main_Page_Bar";
    private MainPageBarView _pageBarView;
    private MainMarketDialog marketDlg;
    private MainOrderDialog orderDlg;
    private MainAccountDialog accountDlg;
    
    
    protected override void SetView(DialogViewBase v) {
        _pageBarView = v as MainPageBarView;
    }

    public void Init()
    {
        // _pageBarView.levelTxt.text = GameRuntime.Instance.UserData.level.ToString();
        marketDlg = UIManager.Instance.PushDialog<MainMarketDialog>(MainMarketDialog.Prefab);
        orderDlg = UIManager.Instance.PushDialog<MainOrderDialog>(MainOrderDialog.Prefab);
        accountDlg = UIManager.Instance.PushDialog<MainAccountDialog>(MainAccountDialog.Prefab);
        marketDlg.Init();
        orderDlg.Init();
        accountDlg.Init();
        marketDlg.ShowPage();
        orderDlg.HidePage();
        accountDlg.HidePage();

        AddListener();
    }

    private void AddListener()
    {
        _pageBarView.marketBtn.onClick.AddListener(SwitchToMarket);
        _pageBarView.orderBtn.onClick.AddListener(SwitchToOrder);
        _pageBarView.accountBtn.onClick.AddListener(SwitchToAccount);
    }

    private void SwitchToMarket()
    {
        marketDlg.ShowPage();
        orderDlg.HidePage();
        accountDlg.HidePage();
    }
    
    private void SwitchToOrder()
    {
        marketDlg.HidePage();
        orderDlg.ShowPage();
        accountDlg.HidePage();
    }
    
    private void SwitchToAccount()
    {
        marketDlg.HidePage();
        orderDlg.HidePage();
        accountDlg.ShowPage();
    }
}