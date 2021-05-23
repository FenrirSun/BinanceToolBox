using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MainPageBase :GameDialogBase
{
    public void HidePage() {
        transform.localScale = Vector3.zero;
    }

    public void ShowPage() {
        transform.localScale = Vector3.one;
    }
}