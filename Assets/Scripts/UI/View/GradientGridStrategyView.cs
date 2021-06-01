using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

public class GradientGridStrategyView : DialogViewBase
{
    public Button closeBtn;
    public Button startBtn;

    public InputField priceInput;
    public InputField quantityInput;
    public InputField stopInput;
    public InputField takeProfitInput;
    public Text SymbolTxt;
    
    public InputField triggerPriceGapInput;
    public InputField orderPriceGapInput;
    public InputField quantityRatioGapInput;
    public InputField maxOrderPriceInput;
    public Text nextTriggerPriceTxt;
    public Text nextOrderPriceTxt;
    public Text nextQuantityTxt;
    
    public Dropdown directionDropdown;
}
