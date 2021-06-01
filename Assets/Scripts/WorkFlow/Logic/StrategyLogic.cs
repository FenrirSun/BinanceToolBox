using System.Collections.Generic;
using GameEvents;
using M3C.Finance.BinanceSdk.Enumerations;

public class StrategyLogic : LogicBase
{
    private SymbolType runningSymbol;
    private Dictionary<SymbolType, List<StrategyBase>> strategyDic;

    protected override void Awake() {
        AddListener();
        strategyDic = new Dictionary<SymbolType, List<StrategyBase>>();
        foreach (var type in SymbolType.Types) {
            strategyDic[type] = new List<StrategyBase>();
        }
    }

    private void AddListener() {
        GetEventComp().Listen<OnAggTradeUpdate>(evt =>
        {
            if (runningSymbol == null)
                return;
            
            var strategyList = strategyDic[runningSymbol];
            foreach (var strategy in strategyList) {
                if (strategy != null && evt != null)
                    strategy.OnAggTradeUpdate(evt.msg);
            }
        });
        GetEventComp().Listen<OnOrderInfoUpdate>(evt =>
        {
            if (runningSymbol == null)
                return;
            
            var strategyList = strategyDic[runningSymbol];
            foreach (var strategy in strategyList) {
                if (strategy != null && evt != null)
                    strategy.OnOrderInfoUpdate(evt.msg);
            }
        });

        GetEventComp().Listen<StartStrategyEvent>(evt => { StartStrategy(evt.Symbol, evt.Strategy); });
    }

    private void StartStrategy<T>(SymbolType symbol, T strategy) where T : StrategyBase {
        if (IsRunningStrategy()) {
            return;
        }

        var accountDataList = GameRuntime.Instance.GetLogic<AccountLogic>().GetAccounts();
        strategyDic[symbol].Clear();
        foreach (var ad in accountDataList) {
            var cloneStrategy = strategy.Clone<T>();
            cloneStrategy.accountData = ad;
            strategyDic[symbol].Add(cloneStrategy);
            cloneStrategy.StartStrategy();
        }
        runningSymbol = symbol;
        GetEventComp().Send(AfterStartStrategyEvent.Create(strategy));
    }

    public void StopStrategy() {
        var strategyList = strategyDic[runningSymbol];
        if (strategyList != null && strategyList.Count > 0) {
            foreach (var strategy in strategyList) {
                if (strategy.state == StrategyState.Executing) {
                    strategy.StopStrategy();
                }
            }
        }
    }

    public StrategyBase GetStrategy(SymbolType symbol, AccountData ad) {
        if (strategyDic[symbol] != null) {
            foreach (var strategy in strategyDic[symbol]) {
                if (strategy.accountData == ad)
                    return strategy;
            }
        }
        return null;
    }

    public bool IsRunningStrategy() {
        if (runningSymbol == null)
            return false;
        
        if (strategyDic[runningSymbol] != null) {
            foreach (var strategy in strategyDic[runningSymbol]) {
                return strategy.state == StrategyState.Executing;
            }
        }

        return false;
    }
}