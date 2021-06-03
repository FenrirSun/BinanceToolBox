using System;
using System.Collections.Generic;
using System.Text;
using GameEvents;
using UnityEngine;
using UnityEngine.UI;

public class AddAccountDialog : GameDialogBase
{
    public const string Prefab = "Add_Account_Dialog";
    private AddAccountView _view;
    private bool isCreate;
    public int accountId;
    
    protected override void SetView(DialogViewBase v) {
        _view = v as AddAccountView;
    }

    public void Init(bool isCreate) {
        this.isCreate = isCreate;
        _view.addAccountBtn.gameObject.SetActive(isCreate);
        _view.configAccountBtn.gameObject.SetActive(!isCreate);
        AddListener();
    }

    private void AddListener() {
        _view.closeBtn.onClick.AddListener(OnClose);
        _view.addAccountBtn.onClick.AddListener(CreateAccount);
        _view.configAccountBtn.onClick.AddListener(ConfigAccount);
    }

    private void CreateAccount() {
        var name = _view.nameInput.text;
        var apiKey = _view.apiInput.text;
        var secretKey = _view.secretInput.text;
        var orderRatioStr = _view.orderRatioInput.text;
        float orderRatio = 1f;
        float.TryParse(orderRatioStr, out orderRatio);
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(secretKey)) {
            var account = GameRuntime.Instance.UserData.AddAccount(name, apiKey, secretKey, orderRatio);
            GameRuntime.Instance.GetLogic<AccountLogic>().AddAccount(account);
            GetEventComp().Send(RefreshAccountList.Create());
            OnClose();
        }
    }

    private void ConfigAccount() {
        var name = _view.nameInput.text;
        var apiKey = _view.apiInput.text;
        var secretKey = _view.secretInput.text;
        var orderRatioStr = _view.orderRatioInput.text;
        float orderRatio = 1f;
        float.TryParse(orderRatioStr, out orderRatio);
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(secretKey)) {
            var account = GameRuntime.Instance.UserData.GetAccount(accountId);
            account.name = name;
            account.apiKey = apiKey;
            account.secretKey = secretKey;
            account.orderRatio = orderRatio;
            GameRuntime.Instance.GetLogic<AccountLogic>().AddAccount(account);
            GetEventComp().Send(RefreshAccountList.Create());
            OnClose();
        }
    }
    
    public override void OnClose() {
        base.OnClose();
    }
}