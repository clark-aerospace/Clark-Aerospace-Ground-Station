using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TemperatureIndicator : MonoBehaviour
{

    public float currentTemperature = 0f;
    public float minTemperature = 0f;
    public float maxTemperature = 1f;


    [Header("Game Objects")]
    public Image temperatureVisual;
    public TextMeshProUGUI temperatureLabel;

    // Update is called once per frame
    void Update()
    {
        temperatureVisual.fillAmount = Mathf.InverseLerp(minTemperature, maxTemperature, currentTemperature);
        temperatureLabel.text = string.Format("{0}°", (int)currentTemperature);
    }
}
