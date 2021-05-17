using System;
using System.Collections.Generic;
using GameEvents;
using M3C.Finance.BinanceSdk;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;
using NLog;
using UnityEngine;

public class StreamDataLogic : LogicBase
{
    private BinanceFuturesWebSocketPublicClient _client;
    private Dictionary<SymbolType, BinanceFuturesWebSocketPublicClient.WebSocketMessageHandler<WebSocketTradesMessage>> _messageHandler;

    protected override void Awake() {
        _client = new BinanceFuturesWebSocketPublicClient();
        _messageHandler = new Dictionary<SymbolType, BinanceFuturesWebSocketPublicClient.WebSocketMessageHandler<WebSocketTradesMessage>>();
        base.Awake();
    }

    protected override void RegisterEvents() {
        var ec = GetEventComp();
        ec.Listen<ListenTradesMessage>(evt =>
        {
            if (!_messageHandler.ContainsKey(evt.symbol)) {
                ListenAggTrade(evt.symbol);
            }
        });
    }

    private void ListenAggTrade(SymbolType type) {
        _client.ConnectTradesEndpoint(type, HandleAggTrade);
    }

    private void HandleAggTrade(WebSocketTradesMessage msg) {
        Debug.Log($"{msg.Symbol} : {msg.Price}");
        GetEventComp().Send(OnAggTradeUpdate.Create(msg));
    }
    
    private void Update() {
    }

    private void OnDestroy() {
        _client?.Dispose();
    }
}