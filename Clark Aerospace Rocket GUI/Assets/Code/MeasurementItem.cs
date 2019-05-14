using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "Measurment Item", menuName = "Aerospace/Measurement Item", order = 1)]
public class MeasurementItem : ScriptableObject
{
    public Gradient gradient;
    public Vector2 minMaxValues;
    public string suffix;
    public string id;

    public bool buttonEnabled;
    public string buttonText;

    public GameObject guiInstance;
    public TextMeshProUGUI infoLabel;
    public Image colorIndicator;
    public Button button;
    public Material mat;
    public Graph graphUI;


}
