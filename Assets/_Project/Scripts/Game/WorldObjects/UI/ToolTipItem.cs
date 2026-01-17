using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ToolTipItem : MonoBehaviour
{
    [SerializeField]
    private List<string> toolTips = new();
    [SerializeField]
    private TextMeshProUGUI textToolTip;
    void Start()
    {
        //In Future Can change to Enums
        toolTips.Add("<color=#C5E0D0><b>[V]</b></color> Place item");
        toolTips.Add("<color=#C5E0D0><b>[G]</b></color> Move your cursor to place where drop item");
        toolTips.Add("<color=#C5E0D0><b>[G]</b></color> On Monument to place item");
        toolTips.Add("<color=#C5E0D0><b>[Right Click]</b></color> Eat item");
        toolTips.Add("<color=#C5E0D0><b>[Left Click]</b></color> Attack with item");
        toolTips.Add("<color=#C5E0D0><b>[R]</b></color> Drag item");

        //ShowToolTip(new int[] {0,2});
    }
    public void ShowToolTip(int[] arrayIndexses)
    {
        textToolTip.text = "";
        for (int i = 0; i < toolTips.Count; i++)
            if (arrayIndexses.Contains(i))
                textToolTip.text += toolTips[i] + "\n";
    }
}
