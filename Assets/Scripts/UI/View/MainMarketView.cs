using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMarketView : DialogViewBase
{
    public GameObject mainRoot;
    public Text curPrice;
    public Dropdown symbolDropdown;
    public Dropdown intervalDropdown;
}
