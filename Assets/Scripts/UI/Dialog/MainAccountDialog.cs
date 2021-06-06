using System;
using System.Collections.Generic;
using System.Text;
using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

public class MainAccountDialog : MainPageBase
{
    public const string Prefab = "Main_Account_Dialog";
    private MainAccountView _view;
    private UserData ud;
    public static CurrencyType curCurrency;
    
    protected override void SetView(DialogViewBase v) {
        _view = v as MainAccountView;
    }

    public void Init() {
        ud = GameRuntime.Instance.UserData;
        _view.mLoopListView.InitListView(ud.accountDataList.Count, OnGetItemByIndex);
        
        _view.symbolDropdown.ClearOptions();
        List<string> symbolOptions = new List<string>();
        foreach (var type in CurrencyType.Types) {
            symbolOptions.Add(type);
        }
        _view.symbolDropdown.AddOptions(symbolOptions);
        _view.symbolDropdown.value = 0;
        OnSelectSymbol(0);
        _view.symbolDropdown.onValueChanged.AddListener(OnSelectSymbol);
        AddListener();
    }

    private void AddListener() {
        GetEventComp().Listen<RefreshAccountList>((evt) =>
        {
            _view.mLoopListView.SetListItemCount(ud.accountDataList.Count);
            _view.mLoopListView.RefreshAllShownItem();
        });
        _view.addAccountBtn.onClick.AddListener(() =>
        {
            var strategyLogic = GameRuntime.Instance.GetLogic<StrategyLogic>();
            if (strategyLogic.IsRunningStrategy()) {
                CommonMessageDialog.OpenWithOneButton("当前有执行中的策略，不可添加账户", null);
                return;
            }
            var addAccountDialog = UIManager.Instance.PushDialog<AddAccountDialog>(AddAccountDialog.Prefab);
            addAccountDialog.Init(null);
        });
    }

    private void OnSelectSymbol(int index) {
        curCurrency = CurrencyType.Types[index];
    }
    
    LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index) {
        if (index < 0 || index >= ud.accountDataList.Count) {
            return null;
        }

        var itemData = ud.accountDataList[index];
        if (itemData == null) {
            return null;
        }

        //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
        //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
        LoopListViewItem2 item = listView.NewListViewItem("AccountPrefab");
        AccountInfoItem itemScript = item.GetComponent<AccountInfoItem>();
        if (item.IsInitHandlerCalled == false) {
            item.IsInitHandlerCalled = true;
            itemScript.Init();
        }

        itemScript.SetItemData(itemData);
        return item;
    }
}