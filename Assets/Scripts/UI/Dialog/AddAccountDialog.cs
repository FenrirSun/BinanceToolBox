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
            _view.addAccountBtn.transform.parent.gameObject.SetActive(true);
            _view.configAccountBtn.transform.parent.gameObject.SetActive(false);
        } else {
            account = ad;
            _view.addAccountBtn.transform.parent.gameObject.SetActive(false);
            _view.configAccountBtn.transform.parent.gameObject.SetActive(true);
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

        if (account != null) {
            _view.deleteAccountBtn.onClick.RemoveAllListeners();
            _view.deleteAccountBtn.onClick.AddListener(() =>
            {
                var strategyLogic = GameRuntime.Instance.GetLogic<StrategyLogic>();
                if (strategyLogic.IsRunningStrategy()) {
                    CommonMessageDialog.OpenWithOneButton("当前有执行中的策略，不可删除账户", null);
                    return;
                }

                GameRuntime.Instance.GetLogic<AccountLogic>().DeleteAccount(account);
                EventManager.Instance.Send(RefreshAccountList.Create());
                OnClose();
            });
        }
    }

    private void CreateAccount() {
        var inputName = _view.nameInput.text;
        var apiKey = _view.apiInput.text;
        var secretKey = _view.secretInput.text;
        var orderRatioStr = _view.orderRatioInput.text;
        float orderRatio = 1f;
        float.TryParse(orderRatioStr, out orderRatio);
        if (!string.IsNullOrEmpty(inputName) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(secretKey)) {
            account = GameRuntime.Instance.UserData.AddAccount(inputName, apiKey, secretKey, orderRatio);
            GameRuntime.Instance.GetLogic<AccountLogic>().AddAccount(account);
            GetEventComp().Send(RefreshAccountList.Create());
            OnClose();
        }
    }

    private void ConfigAccount() {
        var accountName = _view.nameInput.text;
        var apiKey = _view.apiInput.text;
        var secretKey = _view.secretInput.text;
        var orderRatioStr = _view.orderRatioInput.text;
        float.TryParse(orderRatioStr, out var orderRatio);
        if (!string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(secretKey)) {
            account = GameRuntime.Instance.UserData.GetAccount(this.account.id);
            account.name = accountName;
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