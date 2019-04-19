using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RocketPartItem : MonoBehaviour
{

    public RocketComponent component;

    public TextMeshProUGUI partLabel;

    public List<MeasurementItem> measurementItems = new List<MeasurementItem>();

    public Transform graphicalListOfMeasurements;

    public Material rocketMat;

    public void SetComponent(RocketComponent comp) {
        component = comp;
        partLabel.text = component.name;
        measurementItems = new List<MeasurementItem>(component.measurementItems);

        rocketMat = comp.mat;

        graphicalListOfMeasurements = transform.Find("InfoStuffList");

        foreach (MeasurementItem item in measurementItems) {
            Debug.Log("Setting item with ID " + item.id);
            MeasurementItemGraphical bl = new GameObject().AddComponent<MeasurementItemGraphical>();
            bl.SetUpUI(item, graphicalListOfMeasurements);
        }
    }


}
