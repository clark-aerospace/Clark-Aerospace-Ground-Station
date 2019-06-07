﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TestRocket : MonoBehaviour
{

    public TextMeshProUGUI altitudeLabel;
    public TextMeshProUGUI lastUpdatedLabel;
    public TextMeshProUGUI timeSinceLastDataLabel;

    public TextMeshProUGUI latLabel;
    public TextMeshProUGUI longLabel;
    public TextMeshProUGUI ambientTempLabel;

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

            string lastRecieved = "Last recieved at " + timeOfLastData.ToLongTimeString() + " ";
            string sinceLastRecieved = timeSinceLastData.TotalSeconds.ToString("0.##") + " s ago";
            //string lagAmt = "Lag " + lagTime.TotalSeconds.ToString("0.##") + " s";

            string allTogether = lastRecieved + System.Environment.NewLine + sinceLastRecieved + System.Environment.NewLine;// + lagAmt;

            timeSinceLastDataLabel.text = allTogether;
        }

        UpdatePosition();
        UpdateLabels();
     
    }

    void UpdatePosition() {
        transform.position = new Vector3(0f, ArduinoReciever.GetValue("pos_alt"), 0f);
        rocketObj.transform.rotation = new Quaternion(ArduinoReciever.GetValue("rot_x"), ArduinoReciever.GetValue("rot_y"), ArduinoReciever.GetValue("rot_z"), ArduinoReciever.GetValue("rot_w"));
        altitudeLabel.text = ArduinoReciever.GetValue("pos_alt").ToString() + " ft";

        parachuteObj.SetActive(ArduinoReciever.GetValue("para") != 0f);



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
        ambientTempLabel.text = ArduinoReciever.GetValue("temp_ambient").ToString("0.#") + "°C";

        // lastUpdatedLabel.text = "Last updated " + dateOfNewData.ToLongTimeString() + " " + dateOfNewData.ToShortDateString();
        //altitudeLabel.text = "Alt: " + transform.position.y.ToString(); 
    }
}
