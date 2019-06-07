using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using TMPro;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ArduinoReciever : MonoBehaviour
{

    [SerializeField]
    public Dictionary<string, float> values = new Dictionary<string, float>();


    public static ArduinoReciever reciever;


    public SerialPort serialPort;

    public uint lastRecievedEpoch;
    public DateTime dateTimeAtRecieve;

    Stream stream;
    BinaryReader reader;

    public TextMeshProUGUI debuglabel;

    [TextArea]
    public string input = "535452001c00000001454e44";

    void Awake() {
        if (reciever != null) {Destroy(gameObject);}
        else {reciever = this;}
    }

    // 224 seconds from apogee to ground!!

    void Start()
    {
        lastRecievedEpoch = (uint)(DateTime.Now - new DateTime(1970,1,1)).TotalSeconds;
        // InputToDict("PAYLOAD_TEMP:122");

        // Debug.Log("Value of PAYLOAD_TEMP is " + GetValue("PAYLOAD_TEMP").ToString());

        serialPort = new SerialPort(PlayerPrefs.GetString("port_name", "/dev/cu.usbserial-AK06RGGT"), PlayerPrefs.GetInt("port_baud_rate", 57600));
        serialPort.ReadTimeout = 0;
        serialPort.Parity = Parity.None;
        serialPort.DataBits = 8;
        serialPort.StopBits = StopBits.One;
        serialPort.DtrEnable = true;
        serialPort.RtsEnable = true;
        //serialPort.Handshake = Handshake.None;
        //serialPort.Encoding = System.Text.Encoding.BigEndianUnicode;
        serialPort.Open();
        dateTimeAtRecieve = DateTime.Now;
    }

    // void Update() {
    //     if (!serialPort.IsOpen) {return;}

    //     byte[] buf = new byte[128];
    //     serialPort.Read(buf, 0, 128);

        
    //     string str = "";
    //     for (int i = 0; i < buf.Length; i++) {
    //         str += (char)buf[i];
    //     }
    //     debuglabel.text = BitConverter.ToString(buf);

    //     dateTimeAtRecieve = DateTime.Now;
    // }

    void Update() {
        if (!serialPort.IsOpen) {
            debuglabel.text = "Serial port is not open";
            InputExistingValues();
            return;
            
        }
        //Debug.Log(serialPort.ReadLine());

        byte[] buf = new byte[115];

        // UNCOMMENT
        try {serialPort.Read(buf, 0, 115);}
        catch (TimeoutException e) {
            InputExistingValues();
            return;

        }

        //buf = StringToByteArray(input);
        

        debuglabel.text = buf.ToString();
        debuglabel.text = BitConverter.ToString(buf).Replace("-","");

        Stream stream = new MemoryStream(buf);
        reader = new BinaryReader(stream, Encoding.GetEncoding("iso-8859-1"));
        char[] startMarker = reader.ReadChars(3);

        Debug.Log(new string(startMarker));// + " " + new string(endMarker));
        debuglabel.text = BitConverter.ToString(buf).Replace("-","") + System.Environment.NewLine + new string(startMarker);

        if (new string(startMarker) != "STR") { // || endMarker != new char[] {(char)69, (char)78, (char)68}) {
            Debug.Log("Start marker is " + new string(startMarker) + " not STR");
            InputExistingValues();
            return;
        }
        
        InputToDict("pos_lat", (float)reader.ReadDouble());
        InputToDict("pos_long", (float)reader.ReadDouble());

        int alt = (int)reader.ReadUInt32();
        InputToDict("pos_alt", alt);
        InputToDict("acc_x", (float)reader.ReadDouble());
        InputToDict("acc_y", (float)reader.ReadDouble());
        InputToDict("acc_z", (float)reader.ReadDouble());
        InputToDict("rot_x", (float)reader.ReadDouble());
        InputToDict("rot_y", (float)reader.ReadDouble());
        InputToDict("rot_z", (float)reader.ReadDouble());
        InputToDict("rot_w", (float)reader.ReadDouble());
        InputToDict("temp_payload", (float)reader.ReadDouble());
        InputToDict("temp_avionics", (float)reader.ReadDouble());
        InputToDict("temp_airbrakes", (float)reader.ReadDouble());
        InputToDict("temp_ambient", (float)reader.ReadDouble());

        bool para_out = reader.ReadBoolean();
        InputToDict("para", (para_out ? 1f : 0f));


        debuglabel.text += System.Environment.NewLine + "Alt: " + alt.ToString() + System.Environment.NewLine + "Para: " + para_out.ToString();

        //lastRecievedEpoch = reader.ReadUInt32();
        dateTimeAtRecieve = DateTime.Now;
    }

    public void InputExistingValues() {
        InputToDict("pos_lat", GetValue("pos_lat"));
        InputToDict("pos_long", GetValue("pos_long"));
        InputToDict("pos_alt", GetValue("pos_alt"));
        InputToDict("acc_x", GetValue("acc_x"));
        InputToDict("acc_y", GetValue("acc_y"));
        InputToDict("acc_z", GetValue("acc_z"));
        InputToDict("rot_x", GetValue("rot_x"));
        InputToDict("rot_y", GetValue("rot_y"));
        InputToDict("rot_z", GetValue("rot_z"));
        InputToDict("rot_w", GetValue("rot_w"));
        InputToDict("temp_payload", GetValue("temp_payload"));
        InputToDict("temp_avionics", GetValue("temp_avionics"));
        InputToDict("temp_airbrakes", GetValue("temp_airbrakes"));
        InputToDict("temp_ambient", GetValue("temp_ambient"));
        InputToDict("para", GetValue("para"));
        return;
    }

    public static byte[] StringToByteArray(String hex)
    {
        int NumberChars = hex.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    public static float GetValue(string key) {
        float value;
        if (key == null) {
            Debug.LogError("The key requested was null!");
            return 0f;
        }
        bool hasValue = reciever.values.TryGetValue(key, out value);
        if (hasValue) {
            return value;
        } else {
            Debug.LogError("The key " + key + " was not found!");
            return 0f;
        }
    }

    void InputToDict(string key, float value) {
        if (!values.ContainsKey(key)) {
            values.Add(key, value);
        } else {
            values[key] = value;
        }
        //Debug.Log("Key is " + key + ", value is " + value.ToString());

    }
}
