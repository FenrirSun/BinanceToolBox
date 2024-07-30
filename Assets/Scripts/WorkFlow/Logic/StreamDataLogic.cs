using System;
using System.Collections.Generic;
using GameEvents;
using M3C.Finance.BinanceSdk;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using WebSocketSharp;

public class StreamDataLogic : LogicBase
{
    public static KlineInterval curKlineInterval = KlineInterval.Minute5;
    private Queue<GameEvent> _eventList;
    private WebSocketClient_FuturesPublic _client;
    private SymbolType _curKlineSymbol;
    private Dictionary<string, WebSocketTradesMessage> _lastTradesMessages;

    protected override void Awake() {
        _eventList = new Queue<GameEvent>();
        _client = new WebSocketClient_FuturesPublic();
        _lastTradesMessages = new Dictionary<string, WebSocketTradesMessage>();
        _client.MessageHandler = OnGetMessage;
        _client.ConnectStream();
        base.Awake();
    }

    protected override void RegisterEvents() {
        var ec = GetEventComp();
        ec.Listen<GetLastTradeMessage>((evt) =>
        {
            if (_lastTradesMessages.TryGetValue(evt.symbol, out var message)) {
                evt.message = message;
            } else {
                evt.message = null;
            }
        });
        ec.Listen<SubscribeKLine>((evt) => { SubscribeKline(evt.symbol); });

        GetEventComp().Listen<OnDisconnect>(evt => { UIManager.Instance.PushFloatDialog<ConnectingDialog>(ConnectingDialog.Prefab, 960); });
        GetEventComp().Listen<OnReconnect>(evt =>
        {
            var dlg = UIManager.Instance.FindDialogByName(ConnectingDialog.Prefab, true);
            if (dlg != null) {
                UIManager.Instance.PopDialog(dlg, true);
            }
        });
    }

    // 注意回调并不在主线程
    private void OnGetMessage(MessageEventArgs e) {
        if (e == null || string.IsNullOrEmpty(e.Data))
            return;

        // SDEBUG.InfoAsync("收到消息1", e.Data);
        var responseObject = JObject.Parse(e.Data);
        var eventType = (string) responseObject["e"];
        string klineType = $"continuousKline_{curKlineInterval}";
        if (eventType == "aggTrade") {
            var tradeData = JsonConvert.DeserializeObject<WebSocketTradesMessage>(e.Data);
            _lastTradesMessages[tradeData.Symbol] = tradeData;
            _eventList.Enqueue(OnAggTradeUpdate.Create(tradeData));
        } else if (eventType == klineType) {
            var klineData = JsonConvert.DeserializeObject<WebSocketKlineMessage>(e.Data);
            _eventList.Enqueue(OnKlineUpdate.Create(klineData));
        }
    }

    private void SubscribeKline(SymbolType symbol) {
        if (_curKlineSymbol != null)
            _client.UnSubscribe($"continuousKline_{curKlineInterval}", _curKlineSymbol);

        _client.Subscribe($"continuousKline_{curKlineInterval}", symbol);
        _curKlineSymbol = symbol;
    }

    private void Update() {
        if (_eventList != null) {
            while (_eventList.Count > 0) {
                var e = _eventList.Dequeue();
                GetEventComp().Send(e);
            }
        }

        _client?.Update();
    }

    private void OnDestroy() {
        _client?.Dispose();
    }
}