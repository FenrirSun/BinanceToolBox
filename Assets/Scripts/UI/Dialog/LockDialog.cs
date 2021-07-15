public class LockDialog : GameDialogBase
{
    public const string Prefab = "Lock_Dialog";
    private LockView _view;
    private int lockState;
    
    protected override void SetView(DialogViewBase v) {
        _view = v as LockView;
    }

    public void Init() {
        _view.backBtn.onClick.AddListener(() =>
        {
            lockState = 0;
        });
        
        _view.oneBtn.onClick.AddListener(() =>
        {
            if (lockState == 0) {
                lockState = 1;
            }
            else {
                lockState = 0;
            }
        });
        
        _view.twoBtn.onClick.AddListener(() =>
        {
            if (lockState == 1) {
                lockState = 2;
            }
            else {
                lockState = 0;
            }
        });
        
        _view.threeBtn.onClick.AddListener(() =>
        {
            if (lockState == 2) {
                Destroy(gameObject);
            }
        });
    }
}