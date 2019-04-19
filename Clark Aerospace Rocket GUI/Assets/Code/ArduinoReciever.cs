using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;
using System.Threading;

public class ArduinoReciever : MonoBehaviour
{

    [SerializeField]
    public Dictionary<string, float> values = new Dictionary<string, float>();


    public static ArduinoReciever reciever;


    public SerialPort serialPort;

    void Awake() {
        if (reciever != null) {Destroy(gameObject);}
        else {reciever = this;}
    }

    void Start()
    {
        // InputToDict("PAYLOAD_TEMP:122");

        // Debug.Log("Value of PAYLOAD_TEMP is " + GetValue("PAYLOAD_TEMP").ToString());

        serialPort = new SerialPort("/dev/cu.usbmodem14101", 9600);
        serialPort.ReadTimeout = 100;
        serialPort.Open();
        //serialPort.DataReceived += ReadFromArduino;
    }

    void Update() {
        ReadFromArduino();
    }

    void ReadFromArduino() {
        if (serialPort.IsOpen) {
            string value = serialPort.ReadLine();
            InputToDict(value);
        }
        else {
            Debug.Log("Serial port not open");
            
        }
    }

    public static float GetValue(string key) {
        float value;
        bool hasValue = reciever.values.TryGetValue(key, out value);
        if (hasValue) {
            return value;
        } else {
            return 0f;
        }
    }

    void InputToDict(string input) {
        Debug.Log("RAW INPUT - " + input);
        int keyEndIndex = input.IndexOf(":");
        
        string key = input.Substring(0, keyEndIndex);

        string valueStr = input.Substring(keyEndIndex + 1);

        float valueOut = 0f;

        Debug.Log(key + " --- " + valueStr);

        bool canConv = float.TryParse(valueStr, out valueOut);

        if (!values.ContainsKey(key)) {
            values.Add(key, valueOut);
        } else {
            values[key] = valueOut;
        }
        Debug.Log("Key is " + key + ", value is " + valueOut.ToString());

    }
}
