using System;
using System.Configuration;
using System.Linq;
using M3C.Finance.BinanceSdk.Enumerations;
using NUnit.Framework;

namespace M3C.Finance.BinanceSdk.Tests
{
    public class PublicRequestTests
    {
        private BinanceClient _client;

        // [TestInitializeAttribute]
        public void Setup() {
            _client = new BinanceClient();
        }

        [Test]
        public void TestPing() {
            var response = _client.Ping();
            Assert.IsTrue(response.Result);
        }

        [Test]
        public void TestTime() {
            var response = _client.Time();
            Assert.IsTrue(response.Result.ServerTime > 0);
        }

        [Test]
        //[ExpectedException(typeof(BinanceRestApiException))]
        public void TestDepthWithInvalidSymbol() {
            var response = _client.Depth("NEO");
        }

        [Test]
        public void TestDepth() {
            var response = _client.Depth("NEOBTC");
            Assert.IsTrue(response.Result.Asks.Count > 0);
            Assert.IsTrue(response.Result.Bids.Count > 0);
        }

        [Test]
        public void TestDepthWithLimit() {
            var response = _client.Depth("NEOBTC", 10);
            Assert.IsTrue(response.Result.Asks.Count <= 10);
            Assert.IsTrue(response.Result.Bids.Count <= 10);
        }

        [Test]
        public void TestAggTrades() {
            var response = _client.AggregateTrades("NEOBTC");
            Assert.IsTrue(response.Result.Any());
            Assert.IsTrue(response.Result[0].AggregateTradeId > 0);
        }

        [Test]
        public void TestKLines() {
            var response = _client.KLines("NEOBTC", KlineInterval.Month1).Result.ToList();
            Assert.IsTrue(response.Any());
            Assert.IsTrue(response[0].OpenTime > 0);
            Assert.IsTrue(response[0].CloseTime > 0);
        }

        [Test]
        public void TestDailyTicker() {
            var response = _client.TickerDaily("NEOBTC");
            Assert.IsTrue(response.Result.AskPrice > 0);
            Assert.IsTrue(response.Result.BidPrice > 0);
        }

        [Test]
        public void TestAllPricesTicker() {
            var response = _client.TickerAllPrices();
            Assert.IsTrue(response.Result.Any());
        }

        [Test]
        public void TestAllBookTickers() {
            var response = _client.AllBookTickers();
            Assert.IsTrue(response.Result.Any());
        }
    }
}