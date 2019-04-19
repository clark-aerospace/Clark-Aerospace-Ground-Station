using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MeasurementItemGraphical : MonoBehaviour
{
    public MeasurementItem measurementItem;
    public void SetUpUI(MeasurementItem mI, Transform list) {
        measurementItem = mI;
        measurementItem.guiInstance = Instantiate(RocketPartPopulator.populator.measurementPrefab);
        measurementItem.guiInstance.transform.SetParent(list, false);
        measurementItem.infoLabel = measurementItem.guiInstance.transform.Find("NumValue").GetComponent<TextMeshProUGUI>();
        measurementItem.colorIndicator = measurementItem.guiInstance.transform.Find("ColorIndicator").GetComponent<Image>();
        measurementItem.button = measurementItem.guiInstance.transform.Find("InteractButton").GetComponent<Button>();

        if (!measurementItem.buttonEnabled) {measurementItem.button.gameObject.SetActive(false);}
        measurementItem.button.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = measurementItem.buttonText;
        //mat = 
    }
    void Update()
    {
        float val = ArduinoReciever.GetValue(measurementItem.id);
        measurementItem.infoLabel.text = val.ToString() + measurementItem.suffix;


        Color colOut = measurementItem.gradient.Evaluate(Mathf.InverseLerp(measurementItem.minMaxValues.x, measurementItem.minMaxValues.y, val));
        measurementItem.colorIndicator.color = colOut;

        // if (mat != null) {
        //     mat.color = colOut;
        // }
        
    }
}
