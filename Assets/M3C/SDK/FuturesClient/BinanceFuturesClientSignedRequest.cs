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
    }
}