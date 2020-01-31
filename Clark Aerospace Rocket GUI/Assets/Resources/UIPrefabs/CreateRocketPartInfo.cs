using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CreateRocketPartInfo : MonoBehaviour
{

    public string rocketPartName = "Part";


    public bool useBatteryLevel = false;

    public bool useTemperature = false;

    public bool useDetail = false;

    public UnityEvent detailEvent; 

    [Header("Sources")]
    public string batteryLevelSource = "null";
    public string temperatureSource = "null";
    public Vector2 temperatureBounds = new Vector2(0,1);

    
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
        RocketData d  = ArduinoReceiver2020.instance.latestData;

        if (d != null) {
            rocketPart.batteryLevel = ArduinoReceiver2020.instance.latestData.GetFloatValue(batteryLevelSource);
            rocketPart.temperature = ArduinoReceiver2020.instance.latestData.GetFloatValue(temperatureSource);
        }
        else {
            rocketPart.batteryLevel = 0;
            rocketPart.temperature = 0;
        }
        // rocketPart.batteryLevel = ArduinoReciever.GetValue(batteryLevelSource);
        // rocketPart.temperature = ArduinoReciever.GetValue(temperatureSource);
        rocketPart.minMaxTemperatures = temperatureBounds;
    }
}
