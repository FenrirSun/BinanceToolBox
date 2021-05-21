using System.Collections.Generic;
using M3C.Finance.BinanceSdk.Enumerations;

public class AccountLogic : LogicBase
{
    private Dictionary<SymbolType, StrategyBase>  strategyDic;

    public void Init() {
        strategyDic = new Dictionary<SymbolType, StrategyBase>();
        foreach (var type in SymbolType.Types) {
            strategyDic[type] = null;
        }
    }
    
    public void StartStrategy<T>(SymbolType symbol, T strategy) where T: StrategyBase {
        if (strategyDic[symbol] != null && strategyDic[symbol].state == StrategyState.Executing) {
            return;
        }

        strategyDic[symbol] = strategy;
        strategy.StartStrategy();
    }
    
    public void StopStrategy<T>(SymbolType symbol) where T: StrategyBase {
        if (strategyDic[symbol] != null && strategyDic[symbol].state == StrategyState.Executing) {
            strategyDic[symbol].StopStrategy();
        }
    }
}