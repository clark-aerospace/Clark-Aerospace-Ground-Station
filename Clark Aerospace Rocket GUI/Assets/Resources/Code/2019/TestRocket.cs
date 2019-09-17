using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Mapbox.Unity.Map;

public class TestRocket : MonoBehaviour
{

    public TextMeshProUGUI altitudeLabel;
    public TextMeshProUGUI lastUpdatedLabel;
    public TextMeshProUGUI timeSinceLastDataLabel;

    public TextMeshProUGUI latLabel;
    public TextMeshProUGUI longLabel;
    public TextMeshProUGUI ambientTempLabel;

    public TextMeshProUGUI accXLabel, accYLabel, accZLabel;
    public TextMeshProUGUI airbrakesLabel;

    public System.DateTime dateOfNewData;

    public Transform mapTransform;

    public GameObject rocketObj;
    public GameObject parachuteObj;

    public float timeThing = 0f;

    private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (!ArduinoReciever.reciever.serialPort.IsOpen) {
            //Debug.LogError("Serial port not open!");
            timeSinceLastDataLabel.text = "Serial port not open";
        } else {
            System.DateTime timeOfLastData = epoch.AddSeconds(ArduinoReciever.reciever.lastRecievedEpoch);
            System.TimeSpan timeSinceLastData = System.DateTime.Now - ArduinoReciever.reciever.dateTimeAtRecieve;

            //TimeSpan lagTime = ArduinoReciever.reciever.dateTimeAtRecieve - timeOfLastData;

            string lastRecieved = "Last recieved at " + ArduinoReciever.reciever.dateTimeAtRecieve.ToLongTimeString() + " ";
            string sinceLastRecieved = timeSinceLastData.TotalSeconds.ToString("0.##") + " s ago";
            //string lagAmt = "Lag " + lagTime.TotalSeconds.ToString("0.##") + " s";

            string allTogether = lastRecieved + System.Environment.NewLine + sinceLastRecieved + System.Environment.NewLine;// + lagAmt;

            timeSinceLastDataLabel.text = allTogether;
        }

        UpdatePosition();
        UpdateLabels();
     
    }

    void UpdatePosition() {

        rocketObj.transform.rotation = new Quaternion(ArduinoReciever.GetValue("rot_x"), ArduinoReciever.GetValue("rot_y"), ArduinoReciever.GetValue("rot_z"), ArduinoReciever.GetValue("rot_w"));
        if (ArduinoReciever.GetValue("pos_alt") != 0f) {
            altitudeLabel.text = ArduinoReciever.GetValue("pos_alt").ToString() + " ft";
        }

        parachuteObj.SetActive(ArduinoReciever.GetValue("para") != 0f);

        float lat = ArduinoReciever.GetValue("pos_lat");
        float lon = ArduinoReciever.GetValue("pos_long");
        if (lat != 0f && lon != 0f) {
            GeneralManager.manager.SetLatLongMaps(lat, lon);
        }
        else {
            GeneralManager.manager.SetLatLongMaps(32.9405884f, -106.9204034f);
        }

        if (ArduinoReciever.GetValue("pos_alt") != 0f) {
            Vector3 rocketAlt = new Vector3(0f, ArduinoReciever.GetValue("pos_alt"), 0f);
            float rocketAltOffset = GeneralManager.manager.worldScaleMap.QueryElevationInUnityUnitsAt(new Mapbox.Utils.Vector2d(lat, lon)); // = GeneralManager.manager.worldScaleMap.GeoToWorldPosition(new Mapbox.Utils.Vector2d(lat, lon), true).y;
            rocketAlt.y += rocketAltOffset;
            transform.position = rocketAlt;
        }



        //parachuteObj.SetActive(true);
        //parachuteObj.transform.rotation = Quaternion.Inverse(transform.rotation);
        //transform.Translate(0f, 0.5f, 0f);
        return;
    }

    void UpdateLabels() {
        timeThing = 0f;

        dateOfNewData = System.DateTime.Now;

        latLabel.text = ArduinoReciever.GetValue("pos_lat").ToString("0.##") + "°";
        longLabel.text = ArduinoReciever.GetValue("pos_long").ToString("0.##") + "°";
        ambientTempLabel.text = ArduinoReciever.GetValue("temp_ambient").ToString("0.#") + "°F";


        accXLabel.text = ArduinoReciever.GetValue("acc_x").ToString("0.##");
        accYLabel.text = ArduinoReciever.GetValue("acc_y").ToString("0.##");
        accZLabel.text = ArduinoReciever.GetValue("acc_z").ToString("0.##");

        airbrakesLabel.text = ArduinoReciever.GetValue("airbrakes_angle").ToString();

        // lastUpdatedLabel.text = "Last updated " + dateOfNewData.ToLongTimeString() + " " + dateOfNewData.ToShortDateString();
        //altitudeLabel.text = "Alt: " + transform.position.y.ToString(); 
    }
}
