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
    public Queue<GameEvent> eventList;
    private WebSocketClient_FuturesPublic _client;
    private SymbolType curKlineSymbol;
    private Dictionary<SymbolType, WebSocketTradesMessage> lastTradesMessages;

    protected override void Awake() {
        eventList = new Queue<GameEvent>();
        _client = new WebSocketClient_FuturesPublic();
        lastTradesMessages = new Dictionary<SymbolType, WebSocketTradesMessage>();
        _client.MessageHandler = OnGetMessage;
        _client.ConnectStream();
        base.Awake();
    }

    protected override void RegisterEvents() {
        var ec = GetEventComp();
        ec.Listen<GetLastTradeMessage>((evt) =>
        {
            if (lastTradesMessages.ContainsKey(evt.symbol))
                evt.message = lastTradesMessages[evt.symbol];
        });
        ec.Listen<SubscribeKLine>((evt) => { SubscribeKline(evt.symbol); });
    }

    private void OnGetMessage(MessageEventArgs e) {
        var responseObject = JObject.Parse(e.Data);
        var eventType = (string) responseObject["e"];

        string klineType = $"continuousKline_{curKlineInterval}";
        if (eventType == "aggTrade") {
            var tradeData = JsonConvert.DeserializeObject<WebSocketTradesMessage>(e.Data);
            eventList.Enqueue(OnAggTradeUpdate.Create(tradeData));
        } else if (eventType == klineType) {
            var klineData = JsonConvert.DeserializeObject<WebSocketKlineMessage>(e.Data);
            eventList.Enqueue(OnKlineUpdate.Create(klineData));
        }
    }

    private void SubscribeKline(SymbolType symbol) {
        if (curKlineSymbol != null)
            _client.UnSubscribe($"continuousKline_{curKlineInterval}", curKlineSymbol);

        _client.Subscribe($"continuousKline_{curKlineInterval}", symbol);
        curKlineSymbol = symbol;
    }

    private void Update() {
        if (eventList != null) {
            while (eventList.Count > 0) {
                var e = eventList.Dequeue();
                GetEventComp().Send(e);
            }
        }
    }

    private void OnDestroy() {
        _client?.Dispose();
    }
}