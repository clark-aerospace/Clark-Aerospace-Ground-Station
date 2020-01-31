using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadioConnectionStatus2020 : MonoBehaviour
{
    public Image radioIcon;
    public TextMeshProUGUI textMesh;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool radioIsConnected = ArduinoReceiver2020.instance.SerialPortIsOpen();

        if (radioIsConnected)
        {
            radioIcon.color = Color.green;
            textMesh.text = string.Format("Connected to {0}", ArduinoReceiver2020.instance.SerialPortName());
        } else
        {
            radioIcon.color = Color.red;
            textMesh.text = "Not connected to serial";
        }
    }
}
