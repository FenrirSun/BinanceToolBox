using System.Collections.Generic;
using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;

public class StrategyLogic : LogicBase
{
    private Dictionary<SymbolType, StrategyBase> strategyDic;

    public void Init() {
        AddListener();
        strategyDic = new Dictionary<SymbolType, StrategyBase>();
        foreach (var type in SymbolType.Types) {
            strategyDic[type] = null;
        }
    }

    private void AddListener() {
        GetEventComp().Listen<OnAggTradeUpdate>(evt =>
        {
            foreach (var strategy in strategyDic.Values) {
                strategy.OnAggTradeUpdate(evt.msg);
            }
        });
        GetEventComp().Listen<OnOrderInfoUpdate>(evt =>
        {
            foreach (var strategy in strategyDic.Values) {
                strategy.OnOrderInfoUpdate(evt.msg);
            }
        });

        GetEventComp().Listen<StartStrategy>(evt => { StartStrategy(evt.Symbol, evt.Strategy); });
    }

    private void StartStrategy<T>(SymbolType symbol, T strategy) where T : StrategyBase {
        if (strategyDic[symbol] != null && strategyDic[symbol].state == StrategyState.Executing) {
            return;
        }

        strategyDic[symbol] = strategy;
        strategy.StartStrategy();
    }

    public void StopStrategy<T>(SymbolType symbol) where T : StrategyBase {
        if (strategyDic[symbol] != null && strategyDic[symbol].state == StrategyState.Executing) {
            strategyDic[symbol].StopStrategy();
        }
    }
}