using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

public class MainOrderView : DialogViewBase
{
    public GameObject mainRoot;
    public Button newOrderBtn;
    public Button updateOrderBtn;
    public Button updateAllOrderBtn;
    
    public Button newStrategyBtn;
    public Button checkStrategyBtn;
    public Button stopStrategyBtn;
    public Dropdown symbolDropdown;
    public Dropdown accountDropdown;
    public LoopListView2 mLoopListView;
}
