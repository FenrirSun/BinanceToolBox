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
    
    protected override void SetView(DialogViewBase v) {
        _view = v as AddAccountView;
    }

    public void Init() {
        AddListener();
    }

    private void AddListener() {
        _view.closeBtn.onClick.AddListener(OnClose);
        _view.addAccountBtn.onClick.AddListener(CreateAccount);
    }

    private void CreateAccount() {
        var name = _view.nameInput.text;
        var apiKey = _view.apiInput.text;
        var secretKey = _view.secretInput.text;

        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(secretKey)) {
            GameRuntime.Instance.UserData.AddAccount(name, apiKey, secretKey);
            GetEventComp().Send(RefreshAccountList.Create());
            OnClose();
        }
    }

    public override void OnClose() {
        base.OnClose();
    }
}