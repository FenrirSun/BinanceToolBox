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
    private AccountData account;
    
    protected override void SetView(DialogViewBase v) {
        _view = v as AddAccountView;
    }

    public void Init(AccountData ad) {
        if (ad == null) {
            _view.addAccountBtn.gameObject.SetActive(true);
            _view.configAccountBtn.gameObject.SetActive(false);
        } else {
            account = ad;
            _view.addAccountBtn.gameObject.SetActive(false);
            _view.configAccountBtn.gameObject.SetActive(true);
            _view.nameInput.text = account.name;
            _view.apiInput.text = account.apiKey;
            _view.secretInput.text = account.secretKey;
            _view.orderRatioInput.text = account.orderRatio.ToString();
        }
        
        AddListener();
    }

    private void AddListener() {
        _view.closeBtn.onClick.AddListener(OnClose);
        _view.addAccountBtn.onClick.AddListener(CreateAccount);
        _view.configAccountBtn.onClick.AddListener(ConfigAccount);
    }

    private void CreateAccount() {
        var inputName = _view.nameInput.text;
        var apiKey = _view.apiInput.text;
        var secretKey = _view.secretInput.text;
        var orderRatioStr = _view.orderRatioInput.text;
        float orderRatio = 1f;
        float.TryParse(orderRatioStr, out orderRatio);
        if (!string.IsNullOrEmpty(inputName) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(secretKey)) {
            var account = GameRuntime.Instance.UserData.AddAccount(inputName, apiKey, secretKey, orderRatio);
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
            var account = GameRuntime.Instance.UserData.GetAccount(this.account.id);
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