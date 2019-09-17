using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager2020 : MonoBehaviour
{
    public static UIManager2020 instance;

    public GameObject InformationItemPrefab;

    public InformationItem2020 altitudeItem, headingItem, speedItem, distanceItem, bearingItem;

    public Vector2 ourPosition = new Vector2(0,0);
    public Vector2 targetPosition = new Vector2(0,0);

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
        float rocketLat = ArduinoReciever.GetValue("pos_lat");
        float rocketLong = ArduinoReciever.GetValue("pos_long");
        float rocketAlt = ArduinoReciever.GetValue("pos_alt");

        // Calculates the distance and heading to the rocket
        //Vector2 rocketPosition = new Vector2(rocketLat, rocketLong);
        Vector2 rocketPosition = targetPosition;
        float distanceToRocket = GlobeMath.Haversine(ourPosition, rocketPosition);
        float bearingToRocket = GlobeMath.BearingToPoint(ourPosition, rocketPosition);

        distanceItem.Value = string.Format("{0} km", distanceToRocket.ToString("F1"));
        bearingItem.Value = string.Format("{0}°", bearingToRocket.ToString("F1"));
    }
}
