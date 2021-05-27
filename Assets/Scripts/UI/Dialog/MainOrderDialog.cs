using System;
using System.Collections.Generic;
using System.Text;
using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

public class MainOrderDialog : MainPageBase
{
    public const string Prefab = "Main_Order_Dialog";
    private MainOrderView _view;
    public static SymbolType curSymbol;
    private AccountData curAccountData;
    private UserData ud;

    protected override void SetView(DialogViewBase v) {
        _view = v as MainOrderView;
    }

    public void Init() {
        ud = GameRuntime.Instance.UserData;
        _view.mLoopListView.InitListView(0, OnGetItemByIndex);

        _view.symbolDropdown.ClearOptions();
        List<string> symbolOptions = new List<string>();
        foreach (var type in SymbolType.Types) {
            symbolOptions.Add(type);
        }

        _view.symbolDropdown.AddOptions(symbolOptions);
        _view.symbolDropdown.value = 0;
        OnSelectSymbol(0);
        _view.symbolDropdown.onValueChanged.AddListener(OnSelectSymbol);

        AddListener();
    }

    public override void SwitchToPage() {
        _view.accountDropdown.ClearOptions();
        List<string> symbolOptions = new List<string>();
        foreach (var account in ud.accountDataList) {
            symbolOptions.Add(account.name);
        }

        _view.accountDropdown.AddOptions(symbolOptions);
        _view.accountDropdown.value = 0;
        OnSelectAccount(0);
        _view.accountDropdown.onValueChanged.AddListener(OnSelectAccount);
    }

    private void AddListener() {
    }

    private void OnSelectAccount(int index) {
        if (ud.accountDataList.Count > index) {
            curAccountData = ud.accountDataList[index];
            UpdateOrderPanel();
        }
    }

    private void OnSelectSymbol(int index) {
        curSymbol = SymbolType.Types[index];
        UpdateOrderPanel();
    }

    private void UpdateOrderPanel() {
        if (curAccountData != null) {
            _view.mLoopListView.SetListItemCount(curAccountData.GetOrderInfos(curSymbol).Count);
        } else {
            _view.mLoopListView.SetListItemCount(0);
        }

        _view.mLoopListView.RefreshAllShownItem();
    }

    LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index) {
        var orders = curAccountData.GetOrderInfos(curSymbol);
        if (index < 0 || index >= orders.Count) {
            return null;
        }

        var itemData = orders[index];
        if (itemData == null) {
            return null;
        }

        //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
        //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
        LoopListViewItem2 item = listView.NewListViewItem("OrderPrefab");
        OrderInfoItem itemScript = item.GetComponent<OrderInfoItem>();
        if (item.IsInitHandlerCalled == false) {
            item.IsInitHandlerCalled = true;
            itemScript.Init();
        }

        itemScript.SetItemData(itemData);
        return item;
    }
}