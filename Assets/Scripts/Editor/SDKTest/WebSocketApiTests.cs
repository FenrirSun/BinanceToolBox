using System.Configuration;
using System.Threading;
using M3C.Finance.BinanceSdk.Enumerations;
using M3C.Finance.BinanceSdk.ResponseObjects;
using NLog;
using NUnit.Framework;

namespace M3C.Finance.BinanceSdk.Tests
{
    public class WebSocketApiTests
    {
        private BinanceClient _client;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //[TestInitializeAttribute]
        public void Setup() {
            _client = new BinanceClient("BinanceApiKey", "BinanceApiSecret");
        }

        private void GenericMessageHandler(WebSocketMessageBase messageContent) {
            logger.Debug(messageContent.EventTime);
        }

        [Test]
        public void TestStartUserDataStream() {
            var response = _client.StartUserDataStream();
            Assert.IsFalse(string.IsNullOrEmpty(response.Result));
        }

        /// <summary>
        /// First way using Anonymous Function as handler (more compact for short handlers)
        /// </summary>
        [Test]
        public void TestDepthWs() {
            using (var client = new BinanceWebSocketClient()) {
                client.ConnectDepthEndpoint("ethbtc", a => logger.Debug(a.EventTime));
                Thread.Sleep(10000);
            }
        }

        /// <summary>
        /// Second way using explicit function as handler (more readible)
        /// </summary>
        [Test]
        public void TestDepthWsExplicitHandler() {
            using (var client = new BinanceWebSocketClient()) {
                client.ConnectDepthEndpoint("ethbtc", GenericMessageHandler);
                Thread.Sleep(10000);
            }
        }

        [Test]
        public void TestKlineWs() {
            using (var client = new BinanceWebSocketClient()) {
                client.ConnectKlineEndpoint("ethbtc", KlineInterval.Minute1, GenericMessageHandler);
                Thread.Sleep(60000);
            }
        }

        [Test]
        public void TestTradesWs() {
            using (var client = new BinanceWebSocketClient()) {
                client.ConnectTradesEndpoint("ethbtc", GenericMessageHandler);
                Thread.Sleep(60000);
            }
        }

        [Test]
        public void TestUserDataWs() {
            using (var client = new BinanceWebSocketClient()) {
                var response = client.ConnectUserDataEndpoint(_client,
                    accountMessage => logger.Debug("UserData Received! " + accountMessage.EventTime),
                    orderMessage => logger.Debug("Order Message Received! " + orderMessage.EventTime),
                    tradeMessage => logger.Debug("Trade Message Received! " + tradeMessage.EventTime)
                ).Result;
                Thread.Sleep(300000);
            }
        }
    }
}