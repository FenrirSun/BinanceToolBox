using System;
using System.Collections.Generic;
using System.Text;
using M3C.Finance.BinanceSdk;
using UnityEngine;
using UnityEngine.UI;

public static class SDEBUG {
    
    public static void INFO(string key, string value)
    {
        if(DebugDialog.dialog)
            DebugDialog.dialog.DebugInfo(key, value);
    }

    public static void ERROR(string value) {
        if(DebugDialog.dialog)
            DebugDialog.dialog.DebugError(value);
    }

    public static void STATUS(string key, GameObject target, Func<string> fetch)
    {
        if(DebugDialog.dialog)
            DebugDialog.dialog.DebugStatus(key, target, fetch);
    }
}

public class DebugDialog : GameDialogBase
{
    
#region base

    public static DebugDialog dialog;
    public const string Prefab = "Common_Debug_Dialog";
    private DebugView view;

    private bool isShowing = false;
    private bool isStatusMode = false;
    private SortedDictionary<string, string> logDic = new SortedDictionary<string, string>();
    private SortedDictionary<string, Tuple<Func<string>, GameObject>> statusDic = new SortedDictionary<string, Tuple<Func<string>, GameObject>>();
    private StringBuilder strBuf = new StringBuilder();
    
    protected override void SetView(DialogViewBase v) {
        view = v as DebugView;
    }

    public void Init() {
        logDic.Add("__project", ProjectConfig.BUILD_ENV + " " + ProjectConfig.APP_VERSION);
        logDic.Add("__safeArea", Screen.safeArea.ToString() + "/" + Screen.width + "," + Screen.height);

        AddListener();
        DebugStatus("网络连接", gameObject, () => Application.internetReachability.ToString());
        ShowPanel(false);
    }

    private void AddListener()
    {
        Application.logMessageReceived += HandleException;
        view.showBtn.onClick.AddListener(() => { ShowPanel(!isShowing); ShowCommonButtons(); });
        view.commonBtn.onClick.AddListener(() => { SetStatusMode(false); ShowCommonButtons(); });
        view.testBtn.onClick.AddListener(() => { SetStatusMode(false); ShowTestButtons(); });
        view.statueBtn.onClick.AddListener(() => { SetStatusMode(true); view.parentRoot.RemoveAllChildren(); });
    }

    public void DebugInfo(string key, string value, bool systemLog = true)
    {
        if(systemLog)
            Debug.Log("DEBUG INFO: " + key + " " + value);
        logDic[key] = value;
        UpdateLogs();
    }

    public void DebugError(string value)
    {
        PrintException("错误", value);
    }

    public void DebugStatus(string key, GameObject target, Func<string> fetch)
    {
        if (statusDic.ContainsKey(key)) {
            return;
        }
        var d = new Tuple<Func<string>, GameObject>(fetch, target);
        statusDic.Add(key, d);
    }

    void ShowPanel(bool show)
    {
        isShowing = show;
        view.panel.SetActive(isShowing);
        UpdateLogs();
        UpdateStatus();
    }
    
    void HandleException(string condition, string stackTrace, LogType type) {
        if (type == LogType.Exception) {
            StringBuilder desc = new StringBuilder(condition);
            desc.Append('\n');
            desc.Append(stackTrace);
            PrintException("异常", desc.ToString());
        }
    }
    
    void PrintException(string key, string msg) {
        logDic[key] = msg;
        ShowPanel(true);
    }
    
    void UpdateLogs() {
        if (isStatusMode || !isShowing) {
            return;
        }
        strBuf.Length = 0;
        var itr = logDic.GetEnumerator();
        while (itr.MoveNext()) {
            strBuf.Append("<color=red><b>");
            strBuf.Append(itr.Current.Key);
            strBuf.Append("</b></color>: ");
            strBuf.Append(itr.Current.Value);
            strBuf.Append("\n");
        }
        view.logLabel.text = strBuf.ToString();
    }
    
    void SetStatusMode(bool value) {
        isStatusMode = value;
        UpdateStatus();
        UpdateLogs();
    }
    
    private List<string> termatedKeys = new List<string>();
    void UpdateStatus() {
        if (!isStatusMode || !isShowing) {
            return;
        }
        strBuf.Length = 0;
        termatedKeys.Clear();
        var itr = statusDic.GetEnumerator();
        while (itr.MoveNext()) {
            Tuple<Func<string>, GameObject> d = itr.Current.Value;
            if (d.Item2 == null) {
                termatedKeys.Add(itr.Current.Key);
            } else {
                strBuf.Append("<color=red><b>");
                strBuf.Append(itr.Current.Key);
                strBuf.Append("</b></color>  :");
                strBuf.Append(d.Item1());
                strBuf.Append("\n");
            }
        }
        foreach (var k in termatedKeys) {
            statusDic.Remove(k);
        }
        view.logLabel.text = strBuf.ToString();
    }
    
    void AddButton(string text, UnityEngine.Events.UnityAction callback) 
    {
        GameObject go = Instantiate<GameObject>(view.buttonTemplate.gameObject);
        go.SetActive(true);
        go.transform.SetParent(view.parentRoot, false);
        var button = go.GetComponent<Button>();
        Text label = button.transform.GetComponentInChildren<Text>();
        label.text = text;
        button.onClick.AddListener(callback);
    }
    
    void AddInputField(UnityEngine.Events.UnityAction<string> callback,
        string placeholderText = "", InputField.ContentType type = InputField.ContentType.Standard,
        string value = "") 
    {
        GameObject go = Instantiate<GameObject>(view.iptTemplate.gameObject);
        go.SetActive(true);
        go.transform.SetParent(view.parentRoot, false);
        var inputField = go.GetComponent<InputField>();
        if (!string.IsNullOrEmpty(placeholderText)) {
            inputField.placeholder.GetComponent<Text>().text = placeholderText;
        }

        inputField.text = value;
        inputField.contentType = type;
        inputField.onEndEdit.AddListener(callback);
    }

    float deltaTime;
    void Update() {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        view.fpsLabel.text = Mathf.Floor(fps).ToString();

        DumpTime();
    }

#endregion

#region time

    void DumpTime() {
        var lt = TimeUtils.LocalTimestamp.ToDateTime();
        DebugInfo("__time", lt.ToString("yyyy-MM-dd HH:mm:ss"), false);
        var memTotal = (UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024 * 1024)).ToString("F3");
        DebugInfo("__memory", memTotal + "MB", false);
    }

        void ShowTestButtons() {
        view.parentRoot.RemoveAllChildren();
        AddButton("WS测试1", () => {
            WebSocketTest.TestWS();
        });
        
        AddButton("WS测试2", () => {
            WebSocketTest.TestBinanceWS();
        });
        
        AddButton("WS回收", () => {
            WebSocketTest.Dispose();
        });
        
        // AddButton("时间加1分钟", () => {
        //     var offsetSec = PlayerPrefs.GetInt(TimeUtils.KEY_TIME_OFFSET_SEC, 0);
        //     offsetSec += 60;
        //     PlayerPrefs.SetInt(TimeUtils.KEY_TIME_OFFSET_SEC, offsetSec);
        //     DumpTime();
        // });
        // AddButton("时间减1分钟", () => {
        //     var offsetSec = PlayerPrefs.GetInt(TimeUtils.KEY_TIME_OFFSET_SEC, 0);
        //     offsetSec -= 60;
        //     PlayerPrefs.SetInt(TimeUtils.KEY_TIME_OFFSET_SEC, offsetSec);
        //     DumpTime();
        // });

    }
        
#endregion

#region common

    void ShowCommonButtons()
    {
        view.parentRoot.RemoveAllChildren();
        AddButton("清除", () =>
        {
            logDic.Clear();
            view.logLabel.text = string.Empty;
        });
        AddButton("显示等级", () =>
        {
            SDEBUG.INFO("Level", GameRuntime.Instance.UserData.level.ToString());
        });
        int newLevel = 0;
        AddInputField((v) => { newLevel = int.Parse(v);}, "level");
        AddButton("设置等级", () =>
        {
            GameRuntime.Instance.UserData.SetLevel(newLevel);
        });
    }

#endregion
    
}