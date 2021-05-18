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
    private BinanceFuturesWebSocketPublicClient _client;
    public Queue<GameEvent> eventList;

    protected override void Awake() {
        eventList = new Queue<GameEvent>();
        _client = new BinanceFuturesWebSocketPublicClient();
        _client.MessageHandler = OnGetMessage;
        _client.ConnectStream();
        base.Awake();
    }

    protected override void RegisterEvents() {
        var ec = GetEventComp();
    }

    private void OnGetMessage(MessageEventArgs e) {
        var responseObject = JObject.Parse(e.Data);
        var eventType = (string) responseObject["e"];

        switch (eventType) {
            case "aggTrade":
                var tradeData = JsonConvert.DeserializeObject<WebSocketTradesMessage>(e.Data);
                eventList.Enqueue(OnAggTradeUpdate.Create(tradeData));
                return;
            case "kline":
                var klineData = JsonConvert.DeserializeObject<WebSocketKlineMessage>(e.Data);
                eventList.Enqueue(OnKlineUpdate.Create(klineData));
                return;
        }
    }

    private void Update() {
        if (eventList != null) {
            while (eventList.Count > 0) {
                var e = eventList.Dequeue();
                EventManager.Instance.Send(e);
            }
        }
    }

    private void OnDestroy() {
        _client?.Dispose();
    }
}