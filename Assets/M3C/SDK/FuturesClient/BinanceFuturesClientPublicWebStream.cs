using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using GameEvents;
using LitJson;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using WebSocketSharp;

namespace M3C.Finance.BinanceSdk
{
    public class BinanceFuturesWebSocketPublicClient : IDisposable
    {
        private const string WebSocketBaseUrl = "wss://fstream.binance.com/ws/";
        private WebSocket ws;
        
        public delegate void WebSocketMessageHandler(MessageEventArgs messageContent);

        public WebSocketMessageHandler MessageHandler;

        public BinanceFuturesWebSocketPublicClient() {
        }

        public void ConnectStream() {
            ws = new WebSocket(WebSocketBaseUrl);
            ws.EmitOnPing = true;
            ws.Log.Level = LogLevel.Trace;
            ws.OnOpen += (sender, e) =>
            {
                Debug.Log("Connect success");
                Subscribe("aggTrade");
            };
            ws.OnMessage += (sender, e) =>
            {
                // Debug.Log("Server says: " + e.Data);
                if (e.IsPing) {
                    Debug.LogError("get server ping!");
                    ws.Ping();
                } else {
                    MessageHandler?.Invoke(e);
                }
            };
            ws.OnClose += (sender, e) =>
            {
                Debug.Log("Server closed: " + e.Code);
                ConnectStream();
            };
            ws.OnError += (sender, e) =>
            {
                Debug.Log("socket error: " + e.Message);
                ws.Close();
                ConnectStream();
            };

            Debug.Log("Connect socket");
            ws.ConnectAsync();
        }

        public void Subscribe(string method, SymbolType _type = null) {
            WSRequest r = new WSRequest();
            r.method = "SUBSCRIBE";
            r.@params = new List<string>();
            if (_type != null) {
                r.@params.Add(GetSubscribeParam(method, _type));
            } else {
                foreach (var type in SymbolType.Types) {
                    r.@params.Add(GetSubscribeParam(method, type));
                }
            }

            r.id = GameUtils.GetNewGuid();
            var json = JsonMapper.ToJson(r);
            ws.SendAsync(json, null);
        }

        public void UnSubscribe(string method, SymbolType _type = null) {
            WSRequest r = new WSRequest();
            r.method = "UNSUBSCRIBE";
            r.@params = new List<string>();
            if (_type != null) {
                r.@params.Add(GetSubscribeParam(method, _type));
            } else {
                foreach (var type in SymbolType.Types) {
                    r.@params.Add(GetSubscribeParam(method, type));
                }
            }

            r.id = GameUtils.GetNewGuid();
            var json = JsonMapper.ToJson(r);
            ws.SendAsync(json, null);
        }
        
        private string GetSubscribeParam(string method, string symbol) {
            var postfix = string.IsNullOrEmpty(method) ? string.Empty : $"@{method}";
            return $"{(method.IsNullOrEmpty() ? symbol : symbol.ToLowerInvariant())}{postfix}";
        }

        public void Dispose() {
            ws?.Close();
        }
    }

    public class WSRequest
    {
        public string method;
        public List<string> @params;
        public int id;
    }
}