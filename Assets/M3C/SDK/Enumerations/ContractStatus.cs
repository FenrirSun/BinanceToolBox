namespace M3C.Finance.BinanceSdk.Enumerations
{
    public class ContractStatus
    {
        public ContractStatus(string value) { Value = value; }

        private string Value { get; }

        // 待上市
        public static ContractStatus PENDING_TRADING => new ContractStatus("PENDING_TRADING");
        // 交易中
        public static ContractStatus TRADING => new ContractStatus("TRADING");
        // 预交割
        public static ContractStatus PRE_DELIVERING => new ContractStatus("PRE_DELIVERING");
        // 交割中
        public static ContractStatus DELIVERING => new ContractStatus("DELIVERING");
        // 已交割
        public static ContractStatus DELIVERED => new ContractStatus("DELIVERED");
        // 预结算
        public static ContractStatus PRE_SETTLE => new ContractStatus("PRE_SETTLE");
        // 结算中
        public static ContractStatus SETTLING => new ContractStatus("SETTLING");
        // 已下架
        public static ContractStatus CLOSE => new ContractStatus("CLOSE");
        
        public static implicit operator string(ContractStatus orderType) => orderType.Value;
        public static implicit operator ContractStatus(string text) => new ContractStatus(text);
        public override string ToString() => Value;
        
        public static bool operator ==(ContractStatus one, ContractStatus two) {
            return one?.Value == two?.Value;
        }
        
        public static bool operator !=(ContractStatus one, ContractStatus two) {
            return one?.Value != two?.Value;
        }
    }
}
