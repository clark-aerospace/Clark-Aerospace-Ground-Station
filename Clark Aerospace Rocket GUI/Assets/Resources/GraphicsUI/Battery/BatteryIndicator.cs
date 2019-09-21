using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BatteryIndicator : MonoBehaviour
{
    public TextMeshProUGUI batteryLevelText;
    public Image batteryLevelVisual;

    public Color normalBatteryColor, lowBatteryColor;

    [Range(0,1)]
    public float batteryLevel;

    void Update()
    {
        batteryLevelVisual.fillAmount = batteryLevel;
        if (batteryLevel <= 0.2f) {
            batteryLevelVisual.color = lowBatteryColor;
        } else {
            batteryLevelVisual.color = normalBatteryColor;
        }

        batteryLevelText.text = string.Format("{0}%", (int)(batteryLevel * 100f));
    }
}
