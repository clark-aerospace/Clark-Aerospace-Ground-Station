using UnityEngine;
using System.Collections;

using System.Threading;
using System.Text;
using System.IO;
using System.IO.Ports;


//#if UNITY_EDITOR
//using UnityEditor;
//#endif

public class ArduinoReceiver2020 : MonoBehaviour
{
    public static ArduinoReceiver2020 instance;
    public RocketData latestData;
    private Thread SerialThread;

    private Queue outputQueue;

    private SerialPort serialPort;
    private BinaryReader reader;
    public bool threadIsLooping = true;

    const int lengthOfData = 147;

    private static readonly System.DateTime unixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

    void Awake() {
        if (instance != null) Destroy(this);
        else instance = this;
    }

    // Use this for initialization
    void Start()
    {
        outputQueue = Queue.Synchronized(new Queue());

        //SerialThread = new Thread(SerialLoop);
        Debug.Log("Thread created");
        //SerialThread.Start();
        Debug.Log("Thread started");
    }

    // Update is called once per frame
    void Update()
    {
        //latestData = GetLatestRocketData();
    }

    public RocketData GetLatestRocketData() {
        // Debug.LogFormat("{0} items in queue", outputQueue.Count);
        if (outputQueue.Count == 0) return null;
        return (RocketData)outputQueue.Dequeue();
    }

    public bool SerialPortIsOpen() {
        if (serialPort == null) return false;
        return serialPort.IsOpen;
    }

    public string SerialPortName()
    {
        if (serialPort == null) return "NOT CONNECTED";
        return serialPort.PortName;
    }

    //#if UNITY_EDITOR
    //void PlayModeChanged(PlayModeStateChange state)
    //{
    //    if (state == PlayModeStateChange.ExitingPlayMode)
    //    {
    //        OnApplicationQuit();
    //    }
    //}
    //#endif

    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        StopThread();
        SerialThread.Abort();
    }

    public void StopThread()
    {
        lock (this)
        {
            threadIsLooping = false;
        }
    }

    public bool ThreadIsLooping()
    {
        lock (this)
        {
            return threadIsLooping;
        }
    }
    

    void SerialLoop()
    {
        Debug.Log("Hello from the other thread!");
        // Open the serial port
        // TODO: re add support for setting baud rate and serial port
        serialPort = new SerialPort("/dev/cu.usbserial-AK06RGGT", 57600);
        serialPort.ReadTimeout = 50;
        serialPort.WriteTimeout = 0;
        serialPort.Parity = Parity.None;
        serialPort.DataBits = 8;
        serialPort.StopBits = StopBits.One;
        serialPort.DtrEnable = true;
        serialPort.RtsEnable = true;
        //serialPort.Handshake = Handshake.None;
        serialPort.Encoding = System.Text.Encoding.BigEndianUnicode;
        try
        {
            Debug.Log("Gonna try to open the serial port...");
            serialPort.Open();
            Debug.Log("Success! I think?");
        }
        catch (IOException e)
        {
            Debug.LogError("Not opened");
            return;
        }


        // reading loop
        while (ThreadIsLooping())
        {
            Debug.Log("get new rocket data...");
            RocketData newRocketData = ReadRocketData();
            outputQueue.Enqueue(newRocketData);
        }

        Debug.Log("Broken out of thread loop");
        serialPort.Close();
        Debug.Log("Serial port has been closed :)");
    }

    RocketData ReadRocketData()
    {
        RocketData newData = new RocketData();
        try
        {
            // READ ROCKET DATA

            byte[] buf = new byte[lengthOfData];

            serialPort.Read(buf, 0, lengthOfData);

            Stream stream = new MemoryStream(buf);
            reader = new BinaryReader(stream, Encoding.GetEncoding("iso-8859-1"));


            // Start marker "STR"
            char[] startMarker = reader.ReadChars(3);

            // Confirm that start marker is correct
            if (new string(startMarker) != "STR")
            {
                Debug.LogErrorFormat("Error - Expected start marker was \"STR\", got \"{0}\"", startMarker);
            }

            // We made it here, so let's get rocket data
            // First is the GPS coordinates
            Vector3 gps = new Vector3(ReadDoubleAsFloat(), ReadDoubleAsFloat(), ReadDoubleAsFloat());

            // Next is acceleration
            Vector3 accel = new Vector3(ReadDoubleAsFloat(), ReadDoubleAsFloat(), ReadDoubleAsFloat());

            // Rotation
            Quaternion rot = new Quaternion(ReadDoubleAsFloat(), ReadDoubleAsFloat(), ReadDoubleAsFloat(), ReadDoubleAsFloat());

            // and all the other stuff
            double aaAngle = ReadDoubleAsFloat();

            double tPayload = ReadDoubleAsFloat();
            double tAvionics = ReadDoubleAsFloat();
            double tAmbient = ReadDoubleAsFloat();

            bool parachuteOut = reader.ReadBoolean();

            double bPayload = ReadDoubleAsFloat();
            double bAvionics = ReadDoubleAsFloat();
            double bAirbrakes = ReadDoubleAsFloat();

            uint sentEpochTime = reader.ReadUInt32();
            System.DateTime sentTime = UnixTime(sentEpochTime);

            // Now let's package this all into the RocketData object
            newData.position = gps;
            newData.acceleration = accel;
            newData.rotation = rot;
            newData.airbrakesAngle = aaAngle;
            newData.payloadTemp = tPayload;
            newData.avionicsTemp = tAvionics;
            newData.ambientTemp = tAmbient;
            newData.parachuteDeployed = parachuteOut;
            newData.payloadBattery = bPayload;
            newData.avionicsBattery = bAvionics;
            newData.airbrakesBattery = bAirbrakes;
            newData.sentTime = sentTime;
            newData.receivedTime = System.DateTime.Now;
            newData.status = RocketDataStatus.Success;
        }

        catch (System.TimeoutException e)
        {
            Debug.Log("timed out");
            newData.status = RocketDataStatus.Error;

        }
        return newData;
    }

    public System.DateTime UnixTime(uint ticks) {
        return unixEpoch.AddTicks((long)ticks);
    }

    public float ReadDoubleAsFloat()
    {
        return (float)reader.ReadDouble();
    }
}

[System.Serializable]
public class RocketData
{
    public RocketDataStatus status;

    // x = latitude
    // y = longitude
    // z = altitude
    public Vector3 position;

    public Vector3 acceleration;
    public Quaternion rotation;
    public double airbrakesAngle;
    public double payloadTemp;
    public double avionicsTemp;
    public double ambientTemp;
    public bool parachuteDeployed;

    public double payloadBattery;
    public double avionicsBattery;
    public double airbrakesBattery;

    public System.DateTime sentTime, receivedTime;

    public float GetFloatValue(string id) {
        switch (id) {
            case "pos_lat": return (float)position.x;
            case "pos_long": return (float)position.y;
            case "pos_alt": return (float)position.z;
            case "acc_x": return (float)acceleration.x;
            case "acc_y": return (float)acceleration.y;
            case "acc_z": return (float)acceleration.z;
            case "rot_x": return (float)rotation.x;
            case "rot_y": return (float)rotation.y;
            case "rot_z": return (float)rotation.z;
            case "rot_w": return (float)rotation.w;
            case "temp_payload": return (float)payloadTemp;
            case "temp_avionics": return (float)avionicsTemp;
            case "temp_ambient": return (float)ambientTemp;
            case "temp_airbrakes": return 0f;
            case "para": return (parachuteDeployed ? 1f : 0f);
            case "batt_payload": return (float)payloadBattery;
            case "batt_avionics": return (float)avionicsBattery;
            case "batt_airbrakes": return (float)airbrakesBattery;
            case "airbrakes_angle": return (float)airbrakesAngle;
            default:
                //Debug.LogErrorFormat("Error - GetFloatValue() does not recognize value ID {0}", id);
                return 0f;
        }
    }

    public string ToString() {
        if (status == RocketDataStatus.Error) return "Error";
        else {
            string str = "";
            str += "Position: " + position.ToString() + "\n";
            str += "Acceleration: " + acceleration.ToString() + "\n";
            str += "other stuff probably too idk";
            return str;
        }
    }
}

public enum RocketDataStatus
{
    Success = 0,
    Error = 1
}