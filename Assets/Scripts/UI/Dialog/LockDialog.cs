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
            SetLockStateShow();
        });

        _view.oneBtn.onClick.AddListener(() =>
        {
            if (lockState == 0) {
                lockState = 1;
            } else {
                lockState = 0;
            }

            SetLockStateShow();
        });

        _view.twoBtn.onClick.AddListener(() =>
        {
            if (lockState == 1) {
                lockState = 2;
            } else {
                lockState = 0;
            }

            SetLockStateShow();
        });

        _view.threeBtn.onClick.AddListener(() =>
        {
            if (lockState == 2) {
                Destroy(gameObject);
            } else {
                lockState = 0;
            }

            SetLockStateShow();
        });
    }

    private void SetLockStateShow() {
        switch (lockState) {
            case 0:
                _view.oneBtn.gameObject.SetActive(true);
                _view.twoBtn.gameObject.SetActive(true);
                _view.threeBtn.gameObject.SetActive(true);
                break;
            case 1:
                _view.oneBtn.gameObject.SetActive(false);
                _view.twoBtn.gameObject.SetActive(true);
                _view.threeBtn.gameObject.SetActive(true);
                break;
            case 2:
                _view.oneBtn.gameObject.SetActive(false);
                _view.twoBtn.gameObject.SetActive(false);
                _view.threeBtn.gameObject.SetActive(true);
                break;
        }
    }
}