using System.Collections.Generic;
using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;

public class StrategyLogic : LogicBase
{
    // 目前只考虑单个账户
    private Dictionary<SymbolType, StrategyBase> strategyDic;

    protected override void Awake() {
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
                if (strategy != null && evt != null)
                    strategy.OnAggTradeUpdate(evt.msg);
            }
        });
        GetEventComp().Listen<OnOrderInfoUpdate>(evt =>
        {
            foreach (var strategy in strategyDic.Values) {
                if (strategy != null && evt != null)
                    strategy.OnOrderInfoUpdate(evt.msg);
            }
        });

        GetEventComp().Listen<StartStrategyEvent>(evt => { StartStrategy(evt.Symbol, evt.Strategy); });
    }

    private void StartStrategy<T>(SymbolType symbol, T strategy) where T : StrategyBase {
        if (strategyDic[symbol] != null && strategyDic[symbol].state == StrategyState.Executing) {
            return;
        }

        strategyDic[symbol] = strategy;
        strategy.StartStrategy();
        GetEventComp().Send(AfterStartStrategyEvent.Create(strategy));
    }

    public void StopStrategy(SymbolType symbol) {
        var strategy = GetStrategy(symbol);
        if (strategyDic[symbol] != null && strategyDic[symbol].state == StrategyState.Executing) {
            strategy.StopStrategy();
        }
    }

    public StrategyBase GetStrategy(SymbolType symbol) {
        if (strategyDic[symbol] != null) {
            return strategyDic[symbol];
        }

        return null;
    }
}