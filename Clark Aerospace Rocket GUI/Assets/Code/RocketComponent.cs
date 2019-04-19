using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rocket Component", menuName = "Aerospace/Rocket Component", order = 2)]
public class RocketComponent : ScriptableObject
{
    public string name;
    public string id;

    public List<MeasurementItem> measurementItems = new List<MeasurementItem>();

    public Material mat;
}
