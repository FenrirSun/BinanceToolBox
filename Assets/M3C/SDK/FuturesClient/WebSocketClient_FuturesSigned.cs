using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using WebSocketSharp;

namespace M3C.Finance.BinanceSdk
{
    /// <summary>
    /// 每个账户保留一个账户信息流
    /// </summary>
    public class WebSocketClient_FuturesSigned
    {
        private const int KeepAliveMilliseconds = 30 * 60 * 1000;
        private const string BaseUrl = "wss://fstream.binance.com/ws/";
        private const string TestBaseUrl = "wss://stream.binancefuture.com";
        private string WebSocketBaseUrl => GameConfig.isRealEnvironment ? BaseUrl : TestBaseUrl;
        protected SslProtocols SupportedProtocols { get; } = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
        private WebSocket ws;

        public delegate void WebSocketMessageHandler<T>(T messageContent) where T : WebSocketMessageBase;

        public string ConnectUserDataEndpointSync(BinanceFuturesClient client,
            WebSocketMessageHandler<WsFuturesUserDataAccountUpdateMessage> accountUpdateHandler,
            WebSocketMessageHandler<WsFuturesUserDataOrderTradeUpdateMessage> orderTradeUpdateHandler) {
            return ConnectUserDataEndpoint(client, accountUpdateHandler, orderTradeUpdateHandler).Result;
        }

        public async Task<string> ConnectUserDataEndpoint(BinanceFuturesClient client,
            WebSocketMessageHandler<WsFuturesUserDataAccountUpdateMessage> accountUpdateHandler,
            WebSocketMessageHandler<WsFuturesUserDataOrderTradeUpdateMessage> orderTradeUpdateHandler) {
            Dispose();
            var listenKey = await client.StartUserDataStream();
            var endpoint = GetWsEndpoint(string.Empty, listenKey);
            ws = CreateNewWebSocket(endpoint, listenKey);

            ws.OnMessage += (sender, e) =>
            {
                Debug.Log("Msg: " + e.Data);
                var responseObject = JObject.Parse(e.Data);
                var eventType = (string) responseObject["e"];

                switch (eventType) {
                    case "ACCOUNT_UPDATE":
                        accountUpdateHandler(JsonConvert.DeserializeObject<WsFuturesUserDataAccountUpdateMessage>(e.Data));
                        return;
                    case "ORDER_TRADE_UPDATE":
                        orderTradeUpdateHandler(JsonConvert.DeserializeObject<WsFuturesUserDataOrderTradeUpdateMessage>(e.Data));
                        return;
                    // default:
                    //     throw new ApplicationException("Unexpected Event Type In Message");
                }
            };

            ws.Connect();

            var keepAliveTimer = new Timer(KeepAliveHandler, new KeepAliveContext
            {
                Client = client,
                ListenKey = listenKey
            }, KeepAliveMilliseconds, KeepAliveMilliseconds);

            return listenKey;
        }

        private Uri GetWsEndpoint(string method, string symbol) {
            var postfix = string.IsNullOrEmpty(method) ? string.Empty : $"@{method}";
            return new Uri($"{WebSocketBaseUrl}/{(method.IsNullOrEmpty() ? symbol : symbol.ToLowerInvariant())}{postfix}");
        }

        private static async void KeepAliveHandler(object context) {
            var ctx = (KeepAliveContext) context;
            Debug.Log("Making Keepalive Request for :" + ctx.ListenKey);
            await ctx.Client.KeepAliveUserDataStream(ctx.ListenKey);
        }

        private BinanceWebSocket CreateNewWebSocket(Uri endpoint, string listenKey = null) {
            var ws = new BinanceWebSocket(endpoint.AbsoluteUri, listenKey);

            ws.OnOpen += delegate { Debug.Log($"{endpoint} | Socket Connection Established ({ws.Id})"); };

            ws.OnClose += (sender, e) => { Debug.Log($"Socket Connection Closed! ({ws.Id})"); };

            ws.OnError += (sender, e) => { Debug.Log("Msg: " + e.Message + " | " + e.Exception.Message); };
            ws.SslConfiguration.EnabledSslProtocols = SupportedProtocols;
            return ws;
        }

        private class KeepAliveContext
        {
            public string ListenKey { get; set; }
            public BinanceFuturesClient Client { get; set; }
        }

        public void Dispose() {
            Debug.Log("Disposing WebSocket Client...");
            ws?.Close();
            ws = null;
        }
    }
}