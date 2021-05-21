// 策略状态
public enum StrategyState
{
    Idle,
    Executing,
    Pause,
    Finish
}

//合约类型
public enum ContractType
{
    PERPETUAL, // 永续合约
    CURRENT_MONTH, // 当月交割合约
    NEXT_MONTH, // 次月交割合约
    CURRENT_QUARTER, // 当季交割合约
    NEXT_QUARTER, // 次季交割合约
}

// 持仓方向:
public enum HoldDir
{
    BOTH,// 单一持仓方向
    LONG,// 多头(双向持仓下)
    SHORT,// 空头(双向持仓下)
}


//条件价格触发类型 (workingType)
public enum WorkingType
{
    MARK_PRICE,
    CONTRACT_PRICE,
}

//响应类型 (newOrderRespType)
public enum NewOrderRespType
{
    ACK,
    RESULT
}
