using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;

public class UIManager2020 : MonoBehaviour
{
    public static UIManager2020 instance;

    [Header("General Rocket Stuff")]
    public GameObject InformationItemPrefab;

    public InformationItem2020 altitudeItem, headingItem, speedItem, distanceItem, bearingItem;

    public Vector2 ourPosition = new Vector2(0,0);
    public Vector2 targetPosition = new Vector2(0,0);

    [Header("Rocket Part Specific")]
    public GameObject RocketPartInfoPrefab;

    [Header("Map Stuff")]
    public AbstractMap overheadMap;
    

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RocketData latestData = ArduinoReceiver2020.instance.latestData;
        if (latestData == null) return;
        
        Vector3 gps = latestData.position;

        // Calculates the distance and heading to the rocket
        targetPosition = new Vector2(gps.x, gps.y);
        Vector2 rocketPosition = targetPosition;
        float distanceToRocket = GlobeMath.Haversine(ourPosition, rocketPosition);
        float bearingToRocket = GlobeMath.BearingToPoint(ourPosition, rocketPosition);

        distanceItem.Value = distanceToRocket.ToString("F1");
        bearingItem.Value = bearingToRocket.ToString("F1");

        overheadMap.UpdateMap(new Mapbox.Utils.Vector2d(gps.x, gps.y));
    }
}
