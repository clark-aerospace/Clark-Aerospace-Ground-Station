using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;
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
        serialPort.ReadTimeout = 75;
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
        if (!serialPort.IsOpen) {return;}
        //Debug.Log(serialPort.ReadLine());

        byte[] buf = new byte[128];
        serialPort.Read(buf, 0, 128);

        //debuglabel.text = BitConverter.ToString(buf).Replace("-","");

        // stream = new MemoryStream(onlyAlt);
        // reader = new BinaryReader(stream);

        // double alt = reader.ReadDouble();
        // bool para = reader.ReadBoolean();
        // //ulong lastRecievedEpoch = reader.ReadUInt64();

        // debuglabel.text += System.Environment.NewLine + "Alt: " + alt.ToString() + System.Environment.NewLine + "Para: " + para.ToString();


        // string startThing = BitConverter.ToString(onlyAlt, 0, 4);
        // Debug.Log(startThing + " should say STRC");


        
        // double _pos_alt = BitConverter.ToDouble(onlyAlt, 4);
        // InputToDict("pos_alt", (float)_pos_alt);
        
        // bool parachuteOut = BitConverter.ToBoolean(onlyAlt, 12);
        // InputToDict("para", parachuteOut ? 1f : 0f);

        // string endThing = BitConverter.ToString(onlyAlt, 13, 4);
        // Debug.Log(endThing + " should say EDRC");

        // byte[] buf = new byte[117]; 
        // serialPort.Read(buf, 0, 117);

        //Debug.Log(BitConverter.ToString(onlyAlt));

        Stream stream = new MemoryStream(buf);
        reader = new BinaryReader(stream);

        char[] startMarker = reader.ReadChars(3);

        stream.Seek(3, SeekOrigin.End);
        char[] endMarker = reader.ReadChars(3);

        if (new string(startMarker) != "STR" || new string(endMarker) != "END") {
            return;
        }

        debuglabel.text = new string(startMarker) + " " + new string(endMarker);
        stream.Seek(4, SeekOrigin.Begin);

        
        InputToDict("pos_lat", (float)reader.ReadDouble());
        InputToDict("pos_long", (float)reader.ReadDouble());
        InputToDict("pos_alt", (float)reader.ReadDouble());
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
        InputToDict("para", (float)(reader.ReadBoolean() ? 1f : 0f));

        lastRecievedEpoch = reader.ReadUInt32();

        

        
        







        //int offsetSoFar = 0;




        // string startString = BitConverter.To(buf)

        // double _pos_lat = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("pos_lat", (float)_pos_lat);
        // offsetSoFar += 8;

        // double _pos_long = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("pos_long", (float)_pos_long);
        // offsetSoFar += 8;

        // double _pos_alt = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("pos_alt", (float)_pos_alt);
        // offsetSoFar += 8;

        // double _acc_x = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("acc_x", (float)_acc_x);
        // offsetSoFar += 8;

        // double _acc_y = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("acc_y", (float)_acc_y);
        // offsetSoFar += 8;

        // double _acc_z = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("acc_z", (float)_acc_z);
        // offsetSoFar += 8;

        // double _rot_x = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("rot_x", (float)_rot_x);
        // offsetSoFar += 8;

        // double _rot_y = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("rot_y", (float)_rot_y);
        // offsetSoFar += 8;

        // double _rot_z = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("rot_z", (float)_rot_z);
        // offsetSoFar += 8;

        // double _rot_w = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("rot_w", (float)_rot_w);
        // offsetSoFar += 8;

        // double _tmp_payload = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("temp_payload", (float)_tmp_payload);
        // offsetSoFar += 8;

        // double _tmp_avionics = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("temp_avionics", (float)_tmp_avionics);
        // offsetSoFar += 8;

        // double _tmp_airbrakesC = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("temp_airbrakes", (float)_tmp_airbrakesC);
        // offsetSoFar += 8;

        // double _tmp_ambient = BitConverter.ToDouble(buf, offsetSoFar);
        // InputToDict("temp_ambient", (float)_tmp_ambient);
        // offsetSoFar += 8;

        // bool parachuteOut = BitConverter.ToBoolean(buf, offsetSoFar);
        // InputToDict("para", parachuteOut ? 1f : 0f);
        // offsetSoFar += 1;

        // lastRecievedEpoch = BitConverter.ToUInt32(buf, offsetSoFar);
        //lastRecievedEpoch = DateTime.Now;
        dateTimeAtRecieve = DateTime.Now;
    }

    public static float GetValue(string key) {
        float value;
        if (key == null) {return 0f;}
        bool hasValue = reciever.values.TryGetValue(key, out value);
        if (hasValue) {
            return value;
        } else {
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
