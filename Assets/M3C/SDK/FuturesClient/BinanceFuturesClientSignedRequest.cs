using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;

namespace M3C.Finance.BinanceSdk
{
    public partial class BinanceFuturesClient
    {
        public async Task<AccountResponse> GetAccountInfo(bool filterZeroBalance = false) {
            var response = await SendRequest<AccountResponse>("account", ApiVersion.Version3, ApiMethodType.Signed, HttpMethod.Get);
            if (filterZeroBalance) {
                response.Balances = response.Balances.Where(a => a.Free + a.Locked != 0).ToList();
            }

            return response;
        }

        public async Task<FuturesUserDataAccountBalanceMessage> GetBalanceInfo() {
            var responses = await SendRequest<List<FuturesUserDataAccountBalanceMessage>>("balance", ApiVersion.Version2,
                ApiMethodType.Signed, HttpMethod.Get);
            FuturesUserDataAccountBalanceMessage result = null;
            foreach (var r in responses) {
                if (r.Asset.Value == CurrencyType.USDT.Value) {
                    result = r;
                    break;
                }
            }

            GameRuntime.Instance.UserData.GetBalance(Ad, result);
            return result;
        }

        public async Task<NewOrderResponse> NewOrder(SymbolType symbol, OrderSide side, PositionSide posSide, OrderType orderType,
            TimeInForce timeInForce, decimal quantity, decimal price, string newClientOrderId, bool isTestOrder = false, decimal? stopPrice = null,
            decimal? icebergQuantity = null, long? recvWindow = null) {
            var parameters = new Dictionary<string, string>
            {
                {"symbol", symbol},
                {"side", side},
                {"positionSide", posSide},
                {"type", orderType},
                {"timeInForce", timeInForce},
                {"quantity", quantity.ToString(CultureInfo.InvariantCulture)},
                {"price", price.ToString(CultureInfo.InvariantCulture)}
            };

            CheckAndAddReceiveWindow(recvWindow, parameters);

            if (!string.IsNullOrEmpty(newClientOrderId)) {
                parameters.Add("newClientOrderId", newClientOrderId);
            }

            if (stopPrice.HasValue) {
                parameters.Add("stopPrice", stopPrice.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (icebergQuantity.HasValue) {
                parameters.Add("icebergQty", icebergQuantity.Value.ToString(CultureInfo.InvariantCulture));
            }

            return await SendRequest<NewOrderResponse>(isTestOrder ? "order/test" : "order", ApiVersion.Version1, ApiMethodType.Signed,
                HttpMethod.Post, parameters);
        }

        public async Task<CancelOrderResponse> CancelOrder(string symbol, long? orderId = null, string originalClientOrderId = null,
            string newClientOrderId = null, long? recvWindow = null) {
            var parameters = new Dictionary<string, string>
            {
                {"symbol", symbol},
            };

            CheckAndAddReceiveWindow(recvWindow, parameters);

            if (orderId.HasValue) {
                parameters.Add("orderId", orderId.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrEmpty(originalClientOrderId)) {
                parameters.Add("origClientOrderId", originalClientOrderId);
            }

            if (!string.IsNullOrEmpty(newClientOrderId)) {
                parameters.Add("newClientOrderId", newClientOrderId);
            }

            return await SendRequest<CancelOrderResponse>("order", ApiVersion.Version3, ApiMethodType.Signed,
                HttpMethod.Delete, parameters);
        }

        public async Task<IEnumerable<OrderInfo>> CurrentOpenOrders(string symbol, long? recvWindow = null) {
            var parameters = new Dictionary<string, string>
            {
                {"symbol", symbol},
            };

            CheckAndAddReceiveWindow(recvWindow, parameters);

            return await SendRequest<List<OrderInfo>>("openOrders", ApiVersion.Version3, ApiMethodType.Signed, HttpMethod.Get, parameters);
        }

        private void CheckAndAddReceiveWindow(long? recvWindow, IDictionary<string, string> parameters) {
            if (recvWindow.HasValue) {
                parameters.Add("recvWindow", recvWindow.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}