using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DebugView : DialogViewBase
{
    public Button showBtn;
    public Text fpsLabel;
    public GameObject panel;
    
    public Button commonBtn;
    public Button testBtn;
    public Button statueBtn;
    
    public Text logLabel;
    public Button buttonTemplate;
    public InputField iptTemplate;
    public Transform parentRoot;
}