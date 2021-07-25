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
    private string description;
    private List<string> showDescs = new List<string>();

    public static void OpenWithOneButton(string desc, Action cb) {
        var dlg = UIManager.Instance.FindDialogByName(Prefab, true);
        if (dlg == null)
            dlg = UIManager.Instance.PushFloatDialog<CommonMessageDialog>(Prefab, 900);
        if (dlg is CommonMessageDialog msgDlg)
            msgDlg.InitWithOneBtn(desc, cb);
    }

    public static void OpenWithTwoButton(string desc, Action<bool> cb) {
        var dlg = UIManager.Instance.FindDialogByName(Prefab, true);
        if (dlg == null)
            dlg = UIManager.Instance.PushFloatDialog<CommonMessageDialog>(Prefab, 900);
        if (dlg is CommonMessageDialog msgDlg)
            msgDlg.InitWithTwoBtn(desc, cb);
    }

    protected override void SetView(DialogViewBase v) {
        _view = v as CommonMessageView;
    }

    private void InitWithOneBtn(string desc, Action cb) {
        description = desc;
        if (!showDescs.Contains(description)) {
            showDescs.Add(description);
        }
        if (showDescs.Count > 1) {
            return;
        }

        _view.oneButtonRoot.SetActive(true);
        _view.twoButtonRoot.SetActive(false);
        _view.desc.text = desc;
        oneCallback = cb;
        _view.closeBtn.onClick.RemoveAllListeners();
        _view.closeBtn.onClick.AddListener(() =>
        {
            oneCallback?.Invoke();
            OnClose();
        });
    }

    private void InitWithTwoBtn(string desc, Action<bool> cb) {
        description = desc;
        if (!showDescs.Contains(description))
            showDescs.Add(description);
        _view.oneButtonRoot.SetActive(false);
        _view.twoButtonRoot.SetActive(true);
        _view.desc.text = desc;
        twoCallback = cb;
        _view.closeBtn.onClick.RemoveAllListeners();
        _view.closeTwoBtn.onClick.AddListener(() =>
        {
            twoCallback?.Invoke(false);
            OnClose();
        });
        _view.confirmBtn.onClick.RemoveAllListeners();
        _view.confirmBtn.onClick.AddListener(() =>
        {
            twoCallback?.Invoke(true);
            OnClose();
        });
    }

    public override void OnClose() {
        if (showDescs.Contains(_view.desc.text))
            showDescs.Remove(_view.desc.text);
        if (showDescs.Count > 0) {
            OpenWithOneButton(showDescs[0], null);
            return;
        }

        GameObject.Destroy(gameObject);
    }
}