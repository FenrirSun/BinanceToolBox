using System;
using System.Collections.Generic;
using System.Text;
using GameEvents;
using UnityEngine;
using UnityEngine.UI;

public class CommonMessageDialog : GameDialogBase
{
    public const string Prefab = "Common_Message_Dialog";
    private CommonMessageView _view;
    private Action oneCallback;
    private Action<bool> twoCallback;
    
    public static void OpenWithOneButton(string desc, Action cb) {
        var msgDlg = UIManager.Instance.PushFloatDialog<CommonMessageDialog>(Prefab, 100);
        msgDlg.InitWithOneBtn(desc, cb);
    }
    
    public static void OpenWithTwoButton(string desc, Action<bool> cb) {
        var msgDlg = UIManager.Instance.PushFloatDialog<CommonMessageDialog>(Prefab, 100);
        msgDlg.InitWithTwoBtn(desc, cb);
    }
    
    protected override void SetView(DialogViewBase v) {
        _view = v as CommonMessageView;
    }

    private void InitWithOneBtn(string desc, Action cb) {
        _view.oneButtonRoot.SetActive(true);
        _view.twoButtonRoot.SetActive(false);
        _view.desc.text = desc;
        oneCallback = cb;
        _view.closeBtn.onClick.AddListener(() =>
        {
            oneCallback?.Invoke();
            OnClose();
        });
    }

    private void InitWithTwoBtn(string desc, Action<bool> cb) {
        _view.oneButtonRoot.SetActive(false);
        _view.twoButtonRoot.SetActive(true);
        _view.desc.text = desc;
        twoCallback = cb;
        _view.closeTwoBtn.onClick.AddListener(() =>
        {
            twoCallback?.Invoke(false);
            OnClose();
        });
        _view.confirmBtn.onClick.AddListener(() =>
        {
            twoCallback?.Invoke(true);
            OnClose();
        });
    }

    public override void OnClose() {
        GameObject.Destroy(gameObject);
    }
}