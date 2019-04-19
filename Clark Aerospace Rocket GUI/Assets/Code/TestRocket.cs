using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TestRocket : MonoBehaviour
{

    public TextMeshProUGUI altitudeLabel;
    public TextMeshProUGUI lastUpdatedLabel;
    public TextMeshProUGUI timeSinceLastDataLabel;

    public System.DateTime dateOfNewData;

    public Transform mapTransform;

    public float timeThing = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    //if (mapTransform.position.y != -2884f) {mapTransform.position = new Vector3(mapTransform.position.x, -2884f, mapTransform.position.z);}
     timeThing += Time.deltaTime;

     
     System.TimeSpan timeSinceLastData = System.DateTime.Now - dateOfNewData;
     timeSinceLastDataLabel.text = "(" + timeSinceLastData.Milliseconds + " ms ago)";

     if (timeThing >= 0.1f) {
         UpdatePosition();
         UpdateLabels();
     }
     
    }

    void UpdatePosition() {
        //transform.position = new Vector3(0f, ArduinoReciever.GetValue("ALTITUDE"), 0f);
        //transform.Translate(0f, 0.5f, 0f);
        return;
    }

    void UpdateLabels() {
        timeThing = 0f;

        dateOfNewData = System.DateTime.Now;

        lastUpdatedLabel.text = "Last updated " + dateOfNewData.ToLongTimeString() + " " + dateOfNewData.ToShortDateString();
        altitudeLabel.text = "Alt: " + transform.position.y.ToString(); 
    }
}
