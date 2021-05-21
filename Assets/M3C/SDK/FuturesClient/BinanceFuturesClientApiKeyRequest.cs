using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using M3C.Finance.BinanceSdk.Enumerations;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace M3C.Finance.BinanceSdk
{
    public partial class BinanceFuturesClient
    {
        
        public string StartUserDataStreamSync() {
            return StartUserDataStream().Result;
        }
        
        public async Task<string> StartUserDataStream() {
            var response = await SendRequest<JObject>("listenKey", ApiVersion.Version1, ApiMethodType.ApiKey, HttpMethod.Post);
            return (string) response["listenKey"];
        }

        public bool KeepAliveUserDataStreamSync(string listenKey) {
            return KeepAliveUserDataStream(listenKey).Result;
        }
        
        public async Task<bool> KeepAliveUserDataStream(string listenKey) {
            var response = await SendRequest<JObject>("listenKey", ApiVersion.Version1, ApiMethodType.ApiKey, HttpMethod.Put,
                new Dictionary<string, string> {{"listenKey", listenKey}});
            var success = (response.Type == JTokenType.Object && response.HasValues == false);
            if (success) {
                Debug.Log($"Send keep alive request for listenkey: {listenKey} Success...");
            }

            return success;
        }

        public bool CloseUserDataStreamSync(string listenKey) {
            return CloseUserDataStream(listenKey).Result;
        }

        public async Task<bool> CloseUserDataStream(string listenKey) {
            var response = await SendRequest<JObject>("listenKey", ApiVersion.Version1, ApiMethodType.ApiKey, HttpMethod.Delete,
                new Dictionary<string, string> {{"listenKey", listenKey}});
            return response.Type == JTokenType.Object && response.HasValues == false;
        }
    }
}