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
    /// <summary>
    /// 合约 WebSocket 测试端
    /// stream data logic 保留一个公共数据信息流
    /// </summary>
    public class WebSocketClient_FuturesPublic : IDisposable
    {
// #if ENV_PRODUCTION 
        // private const string WebSocketBaseUrl = "wss://fstream.binance.com/ws/";
        // private const string WebSocketBaseUrl = "https://testnet.binancefuture.com/ws";
        private const string WebSocketBaseUrl = "wss://fstream.binancefuture.com/ws";
// #else
        //private const string WebSocketBaseUrl = "wss://stream.binancefuture.com/ws/";
// #endif
        private WebSocket ws;

        public delegate void WebSocketMessageHandler(MessageEventArgs messageContent);

        public WebSocketMessageHandler MessageHandler;

        public void ConnectStream()
        {
            Dispose();
            ws = new WebSocket(WebSocketBaseUrl);
            ws.EmitOnPing = true;
            ws.Log.Level = LogLevel.Trace;
            ws.OnOpen += (sender, e) =>
            {
                SDEBUG.InfoAsync("NetWork", "Public Socket OnOpen");
                Subscribe("aggTrade");
            };
            ws.OnMessage += (sender, e) =>
            {
                // SDEBUG.InfoAsync("NetWork", "Public Socket OnMessage, isPing:" + e.IsPing);
                if (e.IsPing)
                {
                    ws.Ping();
                }
                else
                {
                    MessageHandler?.Invoke(e);
                }
            };
            ws.OnClose += (sender, e) =>
            {
                SDEBUG.InfoAsync("NetWork", "Public Socket Connection Closed! " + e.Code);
            };
            ws.OnError += (sender, e) =>
            {
                SDEBUG.InfoAsync("NetWork", "Public socket error: " + e.Message + " | " + e.Exception.Message);
            };

            Debug.Log("Connect socket");
            ws.ConnectAsync();
        }

        public void Subscribe(string method, SymbolType _type = null)
        {
            WSRequest r = new WSRequest();
            r.method = "SUBSCRIBE";
            r.@params = new List<string>();
            if (_type != null)
            {
                r.@params.Add(GetSubscribeParam(method, _type));
            }
            else
            {
                foreach (var type in SymbolType.Types)
                {
                    r.@params.Add(GetSubscribeParam(method, type));
                }
            }

            r.id = GameUtils.GetNewGuid();
            var json = JsonMapper.ToJson(r);
            ws.SendAsync(json, null);
        }

        public void UnSubscribe(string method, SymbolType _type = null)
        {
            WSRequest r = new WSRequest();
            r.method = "UNSUBSCRIBE";
            r.@params = new List<string>();
            if (_type != null)
            {
                r.@params.Add(GetSubscribeParam(method, _type));
            }
            else
            {
                foreach (var type in SymbolType.Types)
                {
                    r.@params.Add(GetSubscribeParam(method, type));
                }
            }

            r.id = GameUtils.GetNewGuid();
            var json = JsonMapper.ToJson(r);
            ws.SendAsync(json, null);
        }

        private string GetSubscribeParam(string method, string symbol)
        {
            var postfix = string.IsNullOrEmpty(method) ? string.Empty : $"@{method}";
            return $"{(method.IsNullOrEmpty() ? symbol : symbol.ToLowerInvariant())}{postfix}";
        }

        private bool lastConnected;
        private float lastUpdateTime;

        public void Update()
        {
            // 断线后尝试重连
            if (Time.time - lastUpdateTime > 5f)
            {
                lastUpdateTime = Time.time;
                if (ws != null)
                {
                    var isCurrentConnected = ws.IsAlive;
                    if (isCurrentConnected)
                    {
                        if (!lastConnected)
                        {
                            EventManager.Instance.Send(OnReconnect.Create());
                        }
                    }

                    if (!isCurrentConnected)
                    {
                        ConnectStream();
                        if (lastConnected)
                        {
                            EventManager.Instance.Send(OnDisconnect.Create());
                        }
                    }

                    lastConnected = isCurrentConnected;
                }
            }
        }

        public void Dispose()
        {
            if (ws != null && ws.IsAlive)
                ws.CloseAsync();
            ws = null;
        }
    }

    public class WSRequest
    {
        public string method;
        public List<string> @params;
        public int id;
    }
}