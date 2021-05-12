using binance.dex.sdk.noderpc;
using binance.dex.sdk.noderpc.endpoint;
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace binance.dex.sdk.test
{
    public class NodeRpcTest
    {
        private const string endpoint = "https://data-seed-pre-0-s1.binance.org";

        [Test]
        public void JsonTest()
        {
            string test =
                "{\n  \"jsonrpc\": \"2.0\",\n  \"id\": \"abci_info-1\",\n  \"result\": {\n    \"response\": {\n      \"data\": \"BNBChain\",\n      \"last_block_height\": \"9701248\",\n      \"last_block_app_hash\": \"LH9gA62y7QiJFa2HieBsMttoLuyTNI+wWQT4BOf80XY=\"\n    }\n  }\n}";
            RpcResponse<ResponseData> rpcResponse =
                Newtonsoft.Json.JsonConvert.DeserializeObject<RpcResponse<ResponseData>>(test);
            Assert.IsNotNull(rpcResponse.Result);
        }

        [Test]
        public void AbciInfoTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResponseData responseData = nodeRpcClient.AbciInfo();
            Assert.IsNotNull(responseData.Response.Data);
        }

        [Test]
        public void ConsensusStateTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ConsensusRoundStateData result = nodeRpcClient.ConsensusState();
            Assert.IsNotNull(result.RoundState.HeightVoteSets[0]);
        }

        [Test]
        public void DumpConsensusStateTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            DumpRoundStateData result = nodeRpcClient.DumpConsensusState();
            Assert.IsNotNull(result.RoundState.Height);
        }

        [Test]
        public void NetInfoTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultNetInfo result = nodeRpcClient.NetInfo();
            Assert.IsNotNull(result.Peers[0].NodeInfo.Id);
        }

        [Test]
        public void GenesisTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultGenesis result = nodeRpcClient.Genesis();
            Assert.IsNotNull(result.Genesis);
        }

        [Test]
        public void HealthTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultHealth result = nodeRpcClient.Health();
            Assert.IsNotNull(result);
        }

        /*[Test]
        public void NodeRpcExceptionTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultUnconfirmedTxs result;
            Assert.Throws<NodeRpcException>(() => result = nodeRpcClient.NumUnconfirmedTxs());
        }*/

        [Test]
        public void NumUnconfirmedTxsTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultNumUnconfirmedTxs result = nodeRpcClient.NumUnconfirmedTxs();
            Assert.IsNotNull(result.NTxs);
        }

        [Test]
        public void StatusTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultStatus result = nodeRpcClient.Status();
            Assert.IsNotNull(result.NodeInfo.ListenAddr);
        }

        [Test]
        public void AbciQueryTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultAbciQuery response = nodeRpcClient.AbciQuery(EAbciQueryPath.StoreAccKey,
                "0x6163636F756E743A89F856CB39D25C1BDDAAEC74A381577CA8E2F886");
            Assert.IsNotNull(response.Response.Value);
        }

        [Test]
        public void AbciQueryTokenListTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultAbciQuery response = nodeRpcClient.AbciQueryTokenList();
            Assert.IsNotNull(response.Response.Value);
        }

        [Test]
        public void BlockTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultBlock result = nodeRpcClient.Block(10779111);
            Assert.IsNotNull(result.Block.Header.ChainId);
        }

        [Test]
        public void BlockByHashTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultBlock result =
                nodeRpcClient.BlockByHash("0x989A3A075B6168A5B0F9B75F69BC71608E65D7A8A632F0D462AD311A646923ED");
            Assert.IsNotNull(result.Block.Header.ChainId);
        }

        [Test]
        public void BlockResultsTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultBlockResults result = nodeRpcClient.BlockResults(10779111);
            Assert.IsNotNull(result.Results.BeginBlock);
        }

        [Test]
        public void BlockchainTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultBlockchainInfo result = nodeRpcClient.Blockchain(10, 11);
            Assert.IsNotNull(result.BlockMetas[0].Header.ChainId);
        }

        [Test]
        public void CommitTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultCommit result = nodeRpcClient.Commit(10779111);
            Assert.IsNotNull(result.SignedHeader.Header.ChainId);
        }

        [Test]
        public void ConsensusParamsTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultConsensusParams result = nodeRpcClient.ConsensusParams(10779111);
            Assert.IsNotNull(result.ConsensusParams.Evidence.MaxAge);
        }

        [Test]
        public void TxTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultTx result =
                nodeRpcClient.Tx("0xAB1B84C7C0B0B195941DCE9CFE1A54214B72D5DB54AD388D8B146A6B62911E8E", true);
            Assert.IsNotNull(result.TxResult.Data);
        }

        [Test]
        public void TxSearchTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultTxSearch result = nodeRpcClient.TxSearch("tx.height=10779179", true);
            Assert.IsNotNull(result.TotalCount);
        }

        [Test]
        public void UnconfirmedTxsTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultUnconfirmedTxs result = nodeRpcClient.UnconfirmedTxs();
            Assert.IsNotNull(result.NTxs);
        }

        [Test]
        public void ValidatorsTest()
        {
            NodeRpcClient nodeRpcClient = new NodeRpcClient(endpoint);
            ResultValidators result = nodeRpcClient.Validators(10779111);
            Assert.IsNotNull(result.Validators[0].Address);
        }
    }
}