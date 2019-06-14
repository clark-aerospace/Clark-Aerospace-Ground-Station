using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MeasurementItemGraphical : MonoBehaviour
{
    public MeasurementItem measurementItem;
    public int timer = 0;
    public void SetUpUI(MeasurementItem mI, Transform list) {
        measurementItem = mI;
        measurementItem.guiInstance = Instantiate(RocketPartPopulator.populator.measurementPrefab);
        measurementItem.guiInstance.transform.SetParent(list, false);
        measurementItem.infoLabel = measurementItem.guiInstance.transform.Find("HorizPart/NumValue").GetComponent<TextMeshProUGUI>();
        measurementItem.colorIndicator = measurementItem.guiInstance.transform.Find("HorizPart/ColorIndicator").GetComponent<Image>();
        measurementItem.button = measurementItem.guiInstance.transform.Find("HorizPart/InteractButton").GetComponent<Button>();

        if (!measurementItem.buttonEnabled) {measurementItem.button.gameObject.SetActive(false);}
        measurementItem.button.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = measurementItem.buttonText;

        measurementItem.graphUI = measurementItem.guiInstance.transform.Find("GraphContainer/Graph").GetComponent<Graph>();
        measurementItem.graphUI.XAxisLabel = measurementItem.graphXAxisLabel;
        measurementItem.graphUI.YAxisLabel = measurementItem.graphYAxisLabel;
        //measurementItem.graphUI.addTestPoints = true;

        GraphDisplayer disp = measurementItem.guiInstance.transform.Find("GraphContainer").GetComponent<GraphDisplayer>();
        disp.parentRectTransform = GetComponent<RectTransform>();
        //mat = 
    }
    void Update()
    {
        float val = ArduinoReciever.GetValue(measurementItem.id);
        if (val == 0f) {return;}
        measurementItem.infoLabel.text = val.ToString("0.0") + measurementItem.suffix;

        if (timer >= 8) {measurementItem.graphUI.AddPoint(Time.time, val); timer = 0;}
        timer++;


        Color colOut = measurementItem.gradient.Evaluate(Mathf.InverseLerp(measurementItem.minMaxValues.x, measurementItem.minMaxValues.y, val));
        measurementItem.colorIndicator.color = colOut;

        if (measurementItem.mat != null) {
            measurementItem.mat.color = colOut;
        }

        //measurementItem.graphUI.AddPoint(Time.time, val);
        
    }
}
