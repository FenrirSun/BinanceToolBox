public class ConnectingDialog : GameDialogBase
{
    public const string Prefab = "Connecting_Dialog";
    private ConnectingView _view;
    
    protected override void SetView(DialogViewBase v) {
        _view = v as ConnectingView;
    }

    public void Init() {

    }
}