using System.Collections.Generic;
using LitJson;
using M3C.Finance.BinanceSdk;
using UnityEngine;
using WebSocketSharp;

public class WebSocketTest
{
    private static WebSocket socket;

    public static void TestWS() {
        
        socket = new WebSocket("ws://echo.websocket.org");

        socket.OnOpen += (sender, e) =>
        {
            socket.Send("Hi, there!");
            Debug.Log("Connect success");
        };
        socket.OnMessage += (sender, e) =>
        {
            Debug.Log("Server says: " + e.Data);
        };
        socket.OnClose += (sender, e) =>
        {
            Debug.Log("Server closed: " + e.Code);
        };
        socket.Log.Level = LogLevel.Trace;
        Debug.Log("Connect socket");
        socket.Connect();
        //socket.Send("BALUS");
        //Debug.Log (true);
    }

    public static void TestBinanceWS() {
        
        socket = new WebSocket("wss://fstream.binance.com/ws/");

        socket.EmitOnPing = true;
        socket.OnOpen += (sender, e) =>
        {
            WSRequest r = new WSRequest();
            r.method = "SUBSCRIBE";
            r.@params = new List<string>();
            r.@params.Add("btcusdt@aggTrade");
            r.id = 1;
            var json = JsonMapper.ToJson(r);
            socket.Send(json);
            // socket.Send("{\"method\": \"SUBSCRIBE\",\"params\":[\"btcusdt@aggTrade\",\"btcusdt@depth\"],\"id\": 1}");
            Debug.Log("Connect success");
        };
        socket.OnMessage += (sender, e) =>
        {
            if (e.IsPing) {
                Debug.LogError("server is pinging!");
                socket.Ping();
            }
                
            Debug.Log("Server says: " + e.Data);
        };
        socket.OnError += (sender, e) => {
            Debug.Log("socket error: " + e.Message);
        };
        socket.OnClose += (sender, e) =>
        {
            Debug.Log("Server closed: " + e.Code);
        };
        socket.Log.Level = LogLevel.Trace;
        Debug.Log("Connect socket");
        socket.Connect();
        //socket.Send("BALUS");
        //Debug.Log (true);
    }
    
    public static void Dispose() {
        socket?.Close();
    }
}