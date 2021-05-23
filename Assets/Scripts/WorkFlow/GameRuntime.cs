using System;
using System.Collections.Generic;

public class GameRuntime : GameRuntimeBase<GameRuntime, UserData>
{
    private UserData _userData;
    private ProjectInfo _projectInfo;
    public override UserData UserData {
        get { return _userData; }
    }
    
    public override ProjectInfo ProjectInfo {
        get { return ProjectConfig.GetProjectInfo(); }
    } 

    protected override void Awake() {
        _userData = UserDataBase.Load<UserData>();
        if (_userData == null)
            _userData = new UserData();
        base.Awake();

#if !ENV_PRODUCTION
        DebugDialog.dialog = UIManager.Instance.PushFloatDialog<DebugDialog>(DebugDialog.Prefab, 1000);
        DebugDialog.dialog.Init();
#endif
        TableManager.Instance.LoadAllTables();
        AddAllLogic();

        InitDialog();
    }

    void AddAllLogic()
    {
        AddLogic<StreamDataLogic>();
        AddLogic<AccountLogic>();
    }

    void InitDialog() {
        UIManager.Instance.PushDialog<MainPageBarDialog>(MainPageBarDialog.Prefab).Init();
    }
}
