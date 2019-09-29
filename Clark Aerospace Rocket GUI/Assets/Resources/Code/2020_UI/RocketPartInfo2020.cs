using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class RocketPartInfo2020 : MonoBehaviour
{
    public string partName = "Rocket Part";
    public bool useDetailIndicator = false; 

    [Header("Battery")]
    public bool useBatteryLevel = false;

    [Range(0,1)]
    public float batteryLevel = 1f;

    [Header("Temperature")]
    public bool useTemperature = false;
    public float temperature = 1f;
    public Vector2 minMaxTemperatures = new Vector2(0f, 1f);

    [Header("GameObjects")]
    public TextMeshProUGUI partLabel;
    public BatteryIndicator batteryIndicator;
    public TemperatureIndicator temperatureIndicator;
    public Image detailIndicator;

    void Start() {
        Debug.Log(GetComponent<RectTransform>().anchorMax);
        Debug.Log(GetComponent<RectTransform>().anchorMin);
    }

    // Update is called once per frame
    void Update()
    {
        // Set part name label
        partLabel.text = partName;

        // Set up detail indicator if need be
        if (useDetailIndicator) {
            detailIndicator.SetAlpha(1f); 
        } else {
            detailIndicator.SetAlpha(0f);
        }

        // Set up battery level if need be
        if (useBatteryLevel) {
            batteryIndicator.gameObject.SetActive(true);
            batteryIndicator.batteryLevel = batteryLevel;
        } else {
            batteryIndicator.gameObject.SetActive(false);
        }

        // Set up temperature if need be
        if (useTemperature) {
            temperatureIndicator.gameObject.SetActive(true);
            temperatureIndicator.minTemperature = minMaxTemperatures.x;
            temperatureIndicator.maxTemperature = minMaxTemperatures.y;
            temperatureIndicator.currentTemperature = temperature;
        } else {
            temperatureIndicator.gameObject.SetActive(false);
        }
    }
}
