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
    public static AccountData curAccountData;
    private UserData ud;
    private StrategyLogic logic;

    protected override void SetView(DialogViewBase v) {
        _view = v as MainOrderView;
    }

    public void Init() {
        ud = GameRuntime.Instance.UserData;
        logic = GameRuntime.Instance.GetLogic<StrategyLogic>();
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

    private void SetStrategyButtons() {
        var strategy = logic.GetStrategy(curSymbol);
        if (strategy != null && strategy.state == StrategyState.Executing) {
            _view.newStrategyBtn.gameObject.SetActive(false);
            _view.checkStrategyBtn.gameObject.SetActive(false);
            _view.stopStrategyBtn.gameObject.SetActive(true);
            _view.symbolDropdown.gameObject.SetActive(false);
        } else {
            _view.newStrategyBtn.gameObject.SetActive(true);
            _view.checkStrategyBtn.gameObject.SetActive(false);
            _view.stopStrategyBtn.gameObject.SetActive(false);
            _view.symbolDropdown.gameObject.SetActive(true);
        }
    }

    private void AddListener() {
        GetEventComp().Listen<StopStrategyEvent>(evt => { SetStrategyButtons(); });
        GetEventComp().Listen<AfterStartStrategyEvent>(evt => { SetStrategyButtons(); });
        _view.newStrategyBtn.onClick.AddListener(() =>
        {
            var strategyDialog = UIManager.Instance.PushDialog<GradientGridStrategyDialog>(GradientGridStrategyDialog.Prefab);
            strategyDialog.Init();
        });
        _view.stopStrategyBtn.onClick.AddListener(() =>
        {
            var logic = GameRuntime.Instance.GetLogic<StrategyLogic>();
            logic.StopStrategy(curSymbol);
             SetStrategyButtons();
        });
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

        LoopListViewItem2 item = listView.NewListViewItem("OrderPrefab");
        OrderInfoItem itemScript = item.GetComponent<OrderInfoItem>();
        if (item.IsInitHandlerCalled == false) {
            item.IsInitHandlerCalled = true;
            itemScript.Init();
        }

        itemScript.SetItemData(itemData);
        return item;
    }

    private float lastUpdateTime;

    private void Update() {
        if (Time.time - lastUpdateTime > 1f) {
            lastUpdateTime = Time.time;
            UpdateOrderPanel();
        }
    }
}