using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventoryTooltipBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textTop1 = null;
    [SerializeField] private TextMeshProUGUI textTop2 = null;
    [SerializeField] private TextMeshProUGUI textTop3 = null;
    [SerializeField] private TextMeshProUGUI textBottom1 = null;
    [SerializeField] private TextMeshProUGUI textBottom2 = null;
    [SerializeField] private TextMeshProUGUI textBottom3 = null;

    public void SetToolTipsText(string textTop1, string textTop2, string textTop3, 
        string textBottom1, string textBottom2, string textBottom3)
    {
        this.textTop1.text = textTop1;
        this.textTop2.text = textTop2;
        this.textTop3.text = textTop3;

        this.textBottom1.text = textBottom1;
        this.textBottom2.text = textBottom2;
        this.textBottom3.text = textBottom3;
    }

}
