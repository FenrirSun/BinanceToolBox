using System;
using System.Configuration;
using M3C.Finance.BinanceSdk.Enumerations;
using NLog.Internal;
using NUnit.Framework;


namespace M3C.Finance.BinanceSdk.Tests
{
    public class SignedRequestTests
    {
        private BinanceClient _client;

        // [TestInitializeAttribute]
        public void Setup() {
            _client = new BinanceClient("BinanceApiKey", "BinanceApiSecret");
        }

        [Test]
        public void TestAccountInfo() {
            var response = _client.GetAccountInfo();
        }

        [Test]
        public void TestNewTestOrder() {
            //var response = _client.NewOrder("ETHBTC",OrderSide.Sell,OrderType.Limit, TimeInForce.ImmediateOrCancel, 0.1m,1.0m,true);
        }

        [Test]
        public void TestNewOrder() {
            var response = _client.NewOrder("NEOBTC", OrderSide.Sell, OrderType.Limit, TimeInForce.GoodUntilCanceled, 1m, 0.05m,false,"order1");
        }

        [Test]
        public void TestQueryOrder() {
            var response = _client.QueryOrder("NEOBTC", 5240943);
        }

        [Test]
        public void TestOpenOrders() {
            var response = _client.CurrentOpenOrders("NEOBTC");
        }

        [Test]
        public void TestListAllOrders() {
            var response = _client.ListAllOrders("NEOBTC");
        }

        [Test]
        public void TestCancelOrder() {
            //var response = _client.CancelOrder("NEOBTC",null,"order1");
        }

        [Test]
        public void TestListMyTrades() {
            var response = _client.ListMyTrades("STRATBTC");
        }
    }
}