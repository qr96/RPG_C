using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuageBar : MonoBehaviour
{
    Image guage;
    TMP_Text number;

    private void Awake()
    {
        var guageGO = transform.Find("Guage");
        var numberGO = transform.Find("Number");

        if (guageGO != null)
            guage = guageGO.GetComponent<Image>();
        if (numberGO != null)
            number = numberGO.GetComponent<TMP_Text>();
    }

    public void SetAmount(long nowValue, long maxValue)
    {
        if (maxValue == 0)
            return;

        if (nowValue < 0)
            nowValue = 0;

        if (guage != null)
            guage.fillAmount = (float)nowValue / maxValue;
        if (number != null)
            number.text = $"{nowValue} / {maxValue}";
    }
}
