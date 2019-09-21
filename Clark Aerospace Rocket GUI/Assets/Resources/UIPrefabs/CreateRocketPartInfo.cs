﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRocketPartInfo : MonoBehaviour
{

    public string rocketPartName = "Part";


    public bool useBatteryLevel = false;

    public bool useTemperature = false;

    public bool useDetail = false;

    [Header("Sources")]
    public string batteryLevelSource = "null";
    public string temperatureSource = "null";

    
    private RocketPartInfo2020 rocketPart;


    // Start is called before the first frame update
    void Start()
    {
        rocketPart = Instantiate(UIManager2020.instance.RocketPartInfoPrefab).GetComponent<RocketPartInfo2020>();
        RectTransform r = rocketPart.GetComponent<RectTransform>();

        r.SetParent(transform, false);
        r.SetAsFirstSibling();

        r.anchorMax = new Vector2(0.5f, 1.0f);
        r.anchorMin = new Vector2(0.5f, 0.0f);

        rocketPart.partName = rocketPartName;
        rocketPart.useBatteryLevel = useBatteryLevel;
        rocketPart.useTemperature = useTemperature;
        rocketPart.useDetailIndicator = useDetail;
    }

    // Update is called once per frame
    void Update()
    {
        rocketPart.batteryLevel = ArduinoReciever.GetValue(batteryLevelSource);
    }
}
