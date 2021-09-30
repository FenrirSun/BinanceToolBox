using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

public class SimpleOrderView : DialogViewBase
{
    public Button closeBtn;
    public Button startBtn;

    public InputField priceInput;
    public InputField quantityInput;
    public InputField stopInput;
    public InputField takeProfitInput;
    public Text symbolTxt;
    
    public Dropdown sellOrBuyDropdown;
    public Dropdown directionDropdown;
    public Dropdown orderTypeDropdown;
}
