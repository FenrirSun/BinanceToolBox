using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace M3C.Finance.BinanceSdk
{
    public partial class BinanceFuturesClient
    {
        private const string BaseUrl = "https://fapi.binance.com/fapi";
        private const string BaseUrlTest = "https://testnet.binancefuture.com/fapi";
        private string CurBaseUrl => GameConfig.isRealEnvironment ? BaseUrl : BaseUrlTest;

        public AccountData Ad;
        private readonly string _apiKey;
        private readonly string _apiSecret;

        /// <summary>
        /// Constructor for Public API Access Only, No Keys Needed
        /// </summary>
        public BinanceFuturesClient() { }

        /// <summary>
        /// Binance Rest Api Client
        /// </summary>
        public BinanceFuturesClient(AccountData ad) {
            Ad = ad;
            _apiKey = ad.apiKey;
            _apiSecret = ad.secretKey;
        }

        private delegate T ResponseParseHandler<T>(string input);

        private async Task<T> SendRequest<T>(string methodName, string version, ApiMethodType apiMethod, HttpMethod httpMethod,
            Dictionary<string, string> parameters = null, ResponseParseHandler<T> customHandler = null) {
            if ((apiMethod == ApiMethodType.ApiKey && string.IsNullOrEmpty(_apiKey)) ||
                (apiMethod == ApiMethodType.Signed &&
                 (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiSecret)))) {
                CommonMessageDialog.OpenWithOneButton("消息发送失败：ApiKey 未初始化", null);
                throw new BinanceRestApiException(0,
                    "You have to instantiate client with proper keys in order to make ApiKey or Signed API requests!");
            }

            if (parameters == null) {
                parameters = new Dictionary<string, string>();
            }

            if (apiMethod == ApiMethodType.Signed) {
                var timestamp = Utilities.GetCurrentMilliseconds();
                parameters.Add("timestamp", timestamp.ToString(CultureInfo.InvariantCulture));

                var parameterTextForSignature = GetParameterText(parameters);
                var signedBytes = Utilities.Sign(_apiSecret, parameterTextForSignature);
                parameters.Add("signature", Utilities.GetHexString(signedBytes));
            }

            var parameterTextPrefix = parameters.Count > 0 ? "?" : string.Empty;
            var parameterText = GetParameterText(parameters);

            string response;
            using (var client = new WebClient()) {
                if (apiMethod == ApiMethodType.Signed || apiMethod == ApiMethodType.ApiKey) {
                    client.Headers.Add("X-MBX-APIKEY", _apiKey);
                }

                try {
                    var getRequestUrl = $"{CurBaseUrl}/{version}/{methodName}{parameterTextPrefix}{parameterText}";
                    var postRequestUrl = $"{CurBaseUrl}/{version}/{methodName}";

                    response = httpMethod == HttpMethod.Get
                        ? await client.DownloadStringTaskAsync(getRequestUrl)
                        : await client.UploadStringTaskAsync(postRequestUrl, httpMethod.Method, parameterText);
                } catch (WebException webException) {
                    using (var reader = new StreamReader(webException.Response.GetResponseStream(), Encoding.UTF8)) {
                        var errorObject = JObject.Parse(reader.ReadToEnd());
                        var errorCode = (int) errorObject["code"];
                        var errorMessage = (string) errorObject["msg"];
                        CommonMessageDialog.OpenWithOneButton($"消息发送失败：错误码：{errorCode.ToString()}; 错误信息：{errorMessage}", null);
                        throw new BinanceRestApiException(errorCode, errorMessage);
                    }
                }
            }

            return customHandler != null ? customHandler(response) : JsonConvert.DeserializeObject<T>(response);
        }

        private static string GetParameterText(Dictionary<string, string> parameters) {
            if (parameters.Count == 0) {
                return string.Empty;
            }

            var builder = new StringBuilder();
            foreach (var item in parameters) {
                builder.Append($"&{item.Key}={item.Value}");
            }

            return builder.Remove(0, 1).ToString();
        }

        private enum ApiMethodType
        {
            None = 0,
            ApiKey = 1,
            Signed = 2
        }
    }
}