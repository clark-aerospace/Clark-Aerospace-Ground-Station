using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LeTai.Asset.TranslucentImage;
using TMPro;
using UnityEngine.UI.Extensions;
using System.IO.Ports;
using Mapbox.Unity.Map;

public class GeneralManager : MonoBehaviour
{
    public static GeneralManager manager;

    public DropDownList arduinoPortsList;
    public TMP_InputField arduinoBaudRateInput;
    public TMP_InputField launchTimeInput;

    public TMP_InputField timeoutInput;

    public Button openSettingsButton;
    public Button saveSettingsButton;
    public Button cancelSettingsButton;
    public GameObject settingsPanel;

    [Header("Timeline Stuff")]
    public TextMeshProUGUI launchTimerLabel;
    public TextMeshProUGUI bigLaunchTimerLabel;
    public Button openTimelineButton;
    public Button closeTimelineButton;
    public GameObject timelinePanel;
    public Animator timelineAnimator;
    public Image timelineBarTakeoffToApogee;
    public Image timelineBarApogeeToLanding;

    public Image timelineBarTakeoffToApogeeBig;
    public Image timelineBarApogeeToLandingBig;

    public int launchEpochTime;

    public DateTime departureTime = new DateTime(2019, 5, 14, 15, 00, 0);
    public bool launchQueued = false;

    public int quitLength = 0;

    public Graph altGraph;
    public int programStartedEpochTime;
    public float timeOffsetOfGraphClear = 0f;

    [Header("Replay data button")]
    public TextMeshProUGUI replayDataButtonText;
    public Image replayDataButtonImage;
    public Sprite replaySprite, liveSprite;
    public GameObject seekButtons;

    [Header("Map stuff")]
    public AbstractMap worldScaleMap;
    public AbstractMap overheadMap;
    public GameObject overheadCamera;
    public Camera mainCamera;
    public MapMode currentMapMode = MapMode.WorldScale;

    

    public enum MapMode {
        WorldScale = 0,
        Overhead = 1
    }




    public void Awake() {
        if (manager) {Destroy(gameObject);}
        else {manager = this;}

        programStartedEpochTime = ArduinoReciever.GetCurrentEpochTime();
    }
    public void Start() {
        openSettingsButton.onClick.AddListener(OpenSettings);
        saveSettingsButton.onClick.AddListener(SaveSettings);

        openTimelineButton.onClick.AddListener(OpenTimeline);
        closeTimelineButton.onClick.AddListener(CloseTimeline);

        departureTime = DateTime.Now;
        //departureTime = DateTime.Parse(PlayerPrefs.GetString("launch_time", "00:00"));

        DisableReplayData();

    }

    public void LoadSerialPorts() {
        string[] portNames = SerialPort.GetPortNames();
        
        string allnames = "";
        List<DropDownListItem> items = new List<DropDownListItem>();

        foreach (string p in portNames) {
            //Debug.Log(p);
            DropDownListItem item = new DropDownListItem(p, p);
            items.Add(item);
            allnames += p + ", ";
        }

        Debug.LogError("ALL PORT NAMES: " + allnames);

        arduinoPortsList.Items = items;

    }

    public void OpenSettings() {
        settingsPanel.SetActive(true);
        Debug.LogError("Before loading serial ports");
        LoadSerialPorts();
        Debug.LogError("After loading serial ports");

        arduinoBaudRateInput.text = PlayerPrefs.GetInt("port_baud_rate", 57600).ToString();
        timeoutInput.text = PlayerPrefs.GetInt("port_timeout", 0).ToString();
        launchTimeInput.text = PlayerPrefs.GetString("launch_time", "00:00");
        arduinoPortsList.SetSelectedIndex(arduinoPortsList.Items.FindIndex(item => item.ID == PlayerPrefs.GetString("port_name", "/dev/cu.usbserial-AK06RGGT")));
    }

    public void SaveSettings() {
        settingsPanel.SetActive(false);
        PlayerPrefs.SetString("port_name", arduinoPortsList.SelectedItem.ID);
        PlayerPrefs.SetInt("port_baud_rate", int.Parse(arduinoBaudRateInput.text));
        PlayerPrefs.SetInt("port_timeout", int.Parse(timeoutInput.text));
        PlayerPrefs.SetString("launch_time", launchTimeInput.text);

        ArduinoReciever.reciever.serialPort.Close();

        ArduinoReciever.reciever.serialPort.PortName = arduinoPortsList.SelectedItem.ID;
        ArduinoReciever.reciever.serialPort.BaudRate = int.Parse(arduinoBaudRateInput.text);
        ArduinoReciever.reciever.serialPort.ReadTimeout = int.Parse(timeoutInput.text);
        ArduinoReciever.reciever.serialPort.Open();

        departureTime = DateTime.Parse(launchTimeInput.text);
        
    }

    public void CancelSettings() {
        settingsPanel.SetActive(false);
    }

    public void OpenTimeline() {
        timelineAnimator.SetBool("timeline_active", true);
        //timelinePanel.SetActive(true);
    }

    public void CloseTimeline() {
        timelineAnimator.SetBool("timeline_active", false);
        //timelinePanel.SetActive(false);
    }

    public void StartTenSecondCountdown() {
        launchQueued = true;
        departureTime = DateTime.Now.AddSeconds(10);
        StartCoroutine(ArduinoReciever.reciever.SendGoMessage());
        //CloseTimeline();

    }

    public void Update() {

        if (Input.GetKey(KeyCode.Q)) {
            quitLength++;
        } else {
            quitLength = 0;
        }

        if (quitLength >= 100) {
            Application.Quit();
            Debug.LogError("QUIT");
            Debug.Break();
        }

        if (ArduinoReciever.GetValue("pos_alt") != 0f) {altGraph.AddPoint(Time.time - timeOffsetOfGraphClear, ArduinoReciever.GetValue("pos_alt"));}

        bool playback = ArduinoReciever.reciever.dataPlaybackMode;
        if (playback) {
            TimeSpan diff = new TimeSpan(0, 0, ArduinoReciever.reciever.dataPlaybackTime);
            timelineBarTakeoffToApogee.fillAmount = Mathf.InverseLerp(0f, 30f, ArduinoReciever.reciever.dataPlaybackTime);
            timelineBarApogeeToLanding.fillAmount = Mathf.InverseLerp(30, 224f, ArduinoReciever.reciever.dataPlaybackTime);
            UpdateTimelineTimeLabels(diff);
        }
        else if (!launchQueued) {
            launchTimerLabel.text = "Not Set";
            bigLaunchTimerLabel.text = "Not Set";
            timelineBarTakeoffToApogee.fillAmount = 0f;
            timelineBarApogeeToLanding.fillAmount = 0f;

            timelineBarTakeoffToApogeeBig.fillAmount = 0f;
            timelineBarApogeeToLandingBig.fillAmount = 0f;
        }
        else {
            TimeSpan diff;
            diff = (playback ? FromUnixTime((long)ArduinoReciever.reciever.dataPlaybackTime) : DateTime.Now) - departureTime;
            timelineBarTakeoffToApogee.fillAmount = Mathf.InverseLerp(0f, 30f, (float)diff.TotalSeconds);
            timelineBarApogeeToLanding.fillAmount = Mathf.InverseLerp(30, 224f, (float)diff.TotalSeconds);

            timelineBarTakeoffToApogeeBig.fillAmount = Mathf.InverseLerp(0f, 30f, (float)diff.TotalSeconds);
            timelineBarApogeeToLandingBig.fillAmount = Mathf.InverseLerp(30, 224f, (float)diff.TotalSeconds);
            UpdateTimelineTimeLabels(diff);
        }

    }

    public void UpdateTimelineTimeLabels(TimeSpan diff) {
        string symbol = (diff > TimeSpan.Zero) ? "+" : "-";
        string tMinus = "T" + symbol + diff.ToString(@"mm\:ss");
        launchTimerLabel.text = tMinus;
        bigLaunchTimerLabel.text = tMinus;
    }

    public void SetOverheadMapActive() {
        overheadMap.gameObject.SetActive(true);
        worldScaleMap.gameObject.SetActive(false);
        overheadCamera.SetActive(true);
        mainCamera.orthographic = true;
        currentMapMode = MapMode.Overhead;
    }

    public void SetWorldScaleMapActive() {
        overheadMap.gameObject.SetActive(false);
        worldScaleMap.gameObject.SetActive(true);
        overheadCamera.SetActive(false);
        mainCamera.orthographic = false;
        currentMapMode = MapMode.WorldScale;
    }

    public void SetLatLongMaps(float lat, float lon) {
        // worldScaleMap.SetCenterLatitudeLongitude(new Mapbox.Utils.Vector2d(lat, lon));
        // overheadMap.SetCenterLatitudeLongitude(new Mapbox.Utils.Vector2d(lat, lon));

        worldScaleMap.UpdateMap(new Mapbox.Utils.Vector2d(lat, lon));
        overheadMap.UpdateMap(new Mapbox.Utils.Vector2d(lat, lon));
    }

    public AbstractMap GetCurrentMap() {
        switch (currentMapMode) {
            case MapMode.WorldScale:
                return worldScaleMap;
                break;
            case MapMode.Overhead:
                return overheadMap;
                break;
            default:
                return worldScaleMap;
                break;
        }
    }

    public void ToggleReplayData() {
        if (ArduinoReciever.reciever.dataPlaybackMode == false) {
            EnableReplayData();
        } else {
            DisableReplayData();
        }
    }

    public void EnableReplayData() {
        replayDataButtonText.text = "Replay";
        replayDataButtonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.75f);
        replayDataButtonImage.sprite = replaySprite;
        ArduinoReciever.reciever.EnableDataPlayback();
        seekButtons.SetActive(true);
    }

    public void DisableReplayData() {
        replayDataButtonText.text = "Live";
        replayDataButtonImage.color = new Color(1f, 0f, 0.25f, 1f);
        replayDataButtonImage.sprite = liveSprite;
        seekButtons.SetActive(false);
        ArduinoReciever.reciever.DisableDataPlayback();
    }

    public void ClearGraphs() {
        timeOffsetOfGraphClear = Time.time;
        Graph.ClearAllGraphs();
    }

    public static DateTime FromUnixTime(long unixTime)
    {
        return epoch.AddSeconds(unixTime);
    }
    private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}
