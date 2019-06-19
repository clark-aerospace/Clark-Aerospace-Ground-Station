using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

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


    public StreamWriter logWriter;
    public StreamReader logReader;

    public FileStream logStream;


    [Header("Data playback")]
    public bool dataPlaybackMode = false;
    public int dataPlaybackTime = 0;
    public float fracsOfSecond = 0f;
    public PlaybackMode currentPlaybackMode = PlaybackMode.Forward;

    string logPath = "";

    public Sprite forwardPlayIcon, backwardPlayIcon, pauseIcon;

    void Awake() {
        if (reciever != null) {Destroy(gameObject);}
        else {reciever = this;}
    }

    // 224 seconds from apogee to ground!!

    void Start()
    {
        #if UNITY_EDITOR
        EditorApplication.playModeStateChanged += PlayModeChanged;
        #endif
        lastRecievedEpoch = (uint)(DateTime.Now - new DateTime(1970,1,1)).TotalSeconds;
        // InputToDict("PAYLOAD_TEMP:122");

        // Debug.Log("Value of PAYLOAD_TEMP is " + GetValue("PAYLOAD_TEMP").ToString());

        serialPort = new SerialPort(PlayerPrefs.GetString("port_name", "/dev/cu.usbserial-AK06RGGT"), PlayerPrefs.GetInt("port_baud_rate", 57600));
        serialPort.ReadTimeout = 0;
        serialPort.WriteTimeout = 0;
        serialPort.Parity = Parity.None;
        serialPort.DataBits = 8;
        serialPort.StopBits = StopBits.One;
        serialPort.DtrEnable = true;
        serialPort.RtsEnable = true;
        //serialPort.Handshake = Handshake.None;
        //serialPort.Encoding = System.Text.Encoding.BigEndianUnicode;
        serialPort.Open();
        dateTimeAtRecieve = DateTime.Now;

        // logging
        logPath = Application.dataPath + "/recorded_data.csv";
        Debug.LogError(logPath);
        logStream = File.Open(logPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        logWriter = new StreamWriter(logStream);

    }

    #if UNITY_EDITOR
    void PlayModeChanged(PlayModeStateChange state) {
        if (state == PlayModeStateChange.ExitingPlayMode) {
            OnApplicationQuit();
        }
    }
    #endif

    public void SetPlaybackMode(int mode) {
        currentPlaybackMode = (PlaybackMode)mode;

        switch (mode)
        {
            case 0:
                GeneralManager.manager.replayDataButtonImage.sprite = forwardPlayIcon;
                break;
            case 1:
                GeneralManager.manager.replayDataButtonImage.sprite = backwardPlayIcon;
                break;
            case 2:
                GeneralManager.manager.replayDataButtonImage.sprite = pauseIcon;
                break;
        }
    }

    void UpdateDataPlayback() {
        if (currentPlaybackMode != PlaybackMode.Pause) {
            fracsOfSecond += Time.deltaTime;
            if (fracsOfSecond >= 1f) {
                dataPlaybackTime += Mathf.FloorToInt(fracsOfSecond) * (currentPlaybackMode == PlaybackMode.Forward ? 1 : -1);
                fracsOfSecond = 0;
            }
            //dataPlaybackTime = dataPlaybackTime + Time.deltaTime;
            Debug.Log(dataPlaybackTime);
            LoadValuesForTime(dataPlaybackTime);
        }
    }

    void LoadValuesForTime(int time) {
        if (!logReader.BaseStream.CanRead) {return;}
        logReader.BaseStream.Seek(0, SeekOrigin.Begin);


        // loop through each line in the record until we find a time that's larger than the current one
        int targetEpochTime = time;
        int targetRelativeTime = time;


        if (new FileInfo(logPath).Length == 0) {
            Debug.LogError("Empty log file");
            return;
        }

        string[] lines = logReader.ReadToEnd().Split('\n');
        string closestVal = "";

        foreach (string line in lines) {
            Debug.LogError(line);
            Debug.LogError("This line has " + line.Split(',').Length.ToString() + " entries");
            if (line.Split(',').Length < 16) {
                continue;
            }
            //targetEpochTime = int.Parse(line.Split(',')[15]);
            targetRelativeTime = int.Parse(line.Split(',')[16]);

            if (targetRelativeTime > time) {
                break;
            } else {
                closestVal = line;
            }
        }
    
        string[] allData = closestVal.Split(',');
        Debug.Log(allData.Length.ToString() + " should be 16");
        if (allData.Length < 16) return;
        Debug.LogError("Current time is " + time + ", closest match found was " + allData[15] + " - alt was " + allData[2].ToString());
        float pos_lat = float.Parse(allData[0]);
        float pos_long = float.Parse(allData[1]);
        float pos_alt = float.Parse(allData[2]);

        float acc_x = float.Parse(allData[3]);
        float acc_y = float.Parse(allData[4]);
        float acc_z = float.Parse(allData[5]);

        float rot_x = float.Parse(allData[6]);
        float rot_y = float.Parse(allData[7]);
        float rot_z = float.Parse(allData[8]);
        float rot_w = float.Parse(allData[9]);

        float airbrakes_angle = float.Parse(allData[10]);
        float temp_payload = float.Parse(allData[11]);
        float temp_avionics = float.Parse(allData[12]);
        float temp_ambient = float.Parse(allData[13]);
        bool para = float.Parse(allData[14]) == 1f;

        InputToDict("pos_lat", pos_lat);
        InputToDict("pos_long", pos_long);
        InputToDict("pos_alt", pos_alt);

        InputToDict("acc_x", acc_x);
        InputToDict("acc_y", acc_y);
        InputToDict("acc_z", acc_z);

        InputToDict("rot_x", rot_x);
        InputToDict("rot_y", rot_y);
        InputToDict("rot_z", rot_z);
        InputToDict("rot_w", rot_w);

        InputToDict("airbrakes_angle", airbrakes_angle);
        InputToDict("temp_payload", temp_payload);
        InputToDict("temp_avionics", temp_avionics);
        InputToDict("temp_ambient", temp_ambient);
        InputToDict("para", (para ? 1f : 0f));
        
        
    }

    void Update() {
        if (dataPlaybackMode) {
            UpdateDataPlayback();
            return;
        }
        if (!serialPort.IsOpen) {
            debuglabel.text = "Serial port is not open";
            //InputExistingValues();
            return;
            
        }
        //Debug.Log(serialPort.ReadLine());

        byte[] buf = new byte[119];

        // UNCOMMENT
        try {serialPort.Read(buf, 0, 119);}
        catch (TimeoutException e) {
            //InputExistingValues();
            //Debug.LogError("No data recieved");
            return;

        }

        debuglabel.text = buf.ToString();
        debuglabel.text = BitConverter.ToString(buf).Replace("-","");

        Stream stream = new MemoryStream(buf);
        reader = new BinaryReader(stream, Encoding.GetEncoding("iso-8859-1"));
        char[] startMarker = reader.ReadChars(3);

        //Debug.Log(new string(startMarker));// + " " + new string(endMarker));
        debuglabel.text = BitConverter.ToString(buf).Replace("-","") + System.Environment.NewLine + new string(startMarker);

        if (new string(startMarker) != "STR") { // || endMarker != new char[] {(char)69, (char)78, (char)68}) {
            Debug.Log("Start marker is " + new string(startMarker) + " not STR");
            //InputExistingValues();
            return;
        }

        float pos_lat = (float)reader.ReadDouble();
        InputToDict("pos_lat", pos_lat);

        float pos_long = (float)reader.ReadDouble();
        InputToDict("pos_long", pos_long);

        int alt = (int)reader.ReadUInt32();
        InputToDict("pos_alt", alt);

        float acc_x = (float)reader.ReadDouble();
        InputToDict("acc_x", acc_x);

        float acc_y = (float)reader.ReadDouble();
        InputToDict("acc_y", acc_y);

        float acc_z = (float)reader.ReadDouble();
        InputToDict("acc_z", acc_z);

        float rot_x = (float)reader.ReadDouble();
        InputToDict("rot_x", rot_x);

        float rot_y = (float)reader.ReadDouble();
        InputToDict("rot_y", rot_y);

        float rot_z = (float)reader.ReadDouble();
        InputToDict("rot_z", rot_z);

        float rot_w = (float)reader.ReadDouble();
        InputToDict("rot_w", rot_w);

        float airbrakes_angle = (float)reader.ReadDouble();
        InputToDict("airbrakes_angle", airbrakes_angle);

        float temp_payload = (float)reader.ReadDouble();
        InputToDict("temp_payload", temp_payload);

        float temp_avionics = (float)reader.ReadDouble();
        InputToDict("temp_avionics", temp_avionics);

        float temp_ambient = (float)reader.ReadDouble();
        InputToDict("temp_ambient", temp_ambient);

        bool para_out = reader.ReadBoolean();
        InputToDict("para", (para_out ? 1f : 0f));

        debuglabel.text += System.Environment.NewLine + "Alt: " + alt.ToString() + System.Environment.NewLine + "Para: " + para_out.ToString();

        lastRecievedEpoch = reader.ReadUInt32();
        dateTimeAtRecieve = DateTime.Now;

        logWriter.BaseStream.Seek(0, SeekOrigin.End);
        logWriter.WriteLine(
            pos_lat.ToString() + "," +
            pos_long.ToString() + "," +
            alt.ToString() + "," +
            acc_x.ToString() + "," +
            acc_y.ToString() + "," +
            acc_z.ToString() + "," +
            rot_x.ToString() + "," +
            rot_y.ToString() + "," +
            rot_z.ToString() + "," +
            rot_w.ToString() + "," +
            airbrakes_angle.ToString() + "," +
            temp_payload.ToString() + "," + 
            temp_avionics.ToString() + "," +
            temp_ambient.ToString() + "," +
            (para_out ? "1" : "0") + "," +
            GetCurrentEpochTime().ToString() + 
            GetSecondsSinceLaunch()
        );

        logWriter.Flush();
        Debug.Log("Data written to log file");
    }

    public string GetSecondsSinceLaunch() {
        if (GeneralManager.manager.launchQueued) {
            return (DateTime.Now - GeneralManager.manager.departureTime).TotalSeconds.ToString();
        }
        else {
            return "NA";
        }
    }

    public void SendGoMessage() {
        serialPort.Write(new char[] {'S', 'E', 'N', 'D'}, 0, 4);
    }

    public static int GetCurrentEpochTime() {
        return (int)((DateTime.Now - new DateTime(1970,1,1)).TotalSeconds);
    }

    public int ToUnixTime(DateTime time) {
        return (int)((time - new DateTime(1970,1,1)).TotalSeconds);
    }

    public void EnableDataPlayback() {
        logReader = new StreamReader(logStream);
        dataPlaybackMode = true;
        dataPlaybackTime = 0;
        //ToUnixTime(GeneralManager.manager.departureTime);
    }

    public void DisableDataPlayback() {
        logReader?.Close();
        dataPlaybackMode = false;
    }

    void OnApplicationQuit() {
        logWriter?.Close();
        Debug.Log("Application closing - log writer was closed");
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
            //Debug.LogError("The key " + key + " was not found!");
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

    public enum PlaybackMode {
        Forward = 0,
        Backwards = 1,
        Pause = 2
    }
}
