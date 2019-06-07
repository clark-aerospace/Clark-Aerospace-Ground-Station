using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UI.Extensions;
using System.IO.Ports;

public class GeneralManager : MonoBehaviour
{
    public DropDownList arduinoPortsList;
    public TMP_InputField arduinoBaudRateInput;
    public TMP_InputField launchTimeInput;

    public TMP_InputField timeoutInput;

    public Button openSettingsButton;
    public Button saveSettingsButton;
    public GameObject settingsPanel;

    [Header("Timeline Stuff")]
    public TextMeshProUGUI launchTimerLabel;
    public TextMeshProUGUI bigLaunchTimerLabel;
    public Button openTimelineButton;
    public Button closeTimelineButton;
    public GameObject timelinePanel;
    public Image timelineBarTakeoffToApogee;
    public Image timelineBarApogeeToLanding;

    public int launchEpochTime;

    public DateTime departureTime = new DateTime(2019, 5, 14, 15, 00, 0);

    public int quitLength = 0;


    public void Start() {
        openSettingsButton.onClick.AddListener(OpenSettings);
        saveSettingsButton.onClick.AddListener(SaveSettings);

        openTimelineButton.onClick.AddListener(OpenTimeline);
        closeTimelineButton.onClick.AddListener(CloseTimeline);

        departureTime = DateTime.Parse(PlayerPrefs.GetString("launch_time", "00:00"));

    }

    public void LoadSerialPorts() {
        string[] portNames = SerialPort.GetPortNames();
        List<DropDownListItem> items = new List<DropDownListItem>();

        foreach (string p in portNames) {
            //Debug.Log(p);
            DropDownListItem item = new DropDownListItem(p, p);
            items.Add(item);
        }

        arduinoPortsList.Items = items;

    }

    public void OpenSettings() {
        settingsPanel.SetActive(true);
        LoadSerialPorts();

        //arduinoPortsList.OverrideHighlighted = true;
        //arduinoPortsList.SelectedItem = arduinoPortsList.Items.Find(item => item.ID == PlayerPrefs.GetString("port_name"));
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

    public void OpenTimeline() {
        timelinePanel.SetActive(true);
    }

    public void CloseTimeline() {
        timelinePanel.SetActive(false);
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

        TimeSpan diff = DateTime.Now - departureTime;

        string symbol = (diff > TimeSpan.Zero) ? "+" : "-";
        string tMinus = "T" + symbol + diff.ToString(@"mm\:ss");
        launchTimerLabel.text = tMinus;
        bigLaunchTimerLabel.text = tMinus;

        timelineBarTakeoffToApogee.fillAmount = Mathf.InverseLerp(0f, 30f, (float)diff.TotalSeconds);
        timelineBarApogeeToLanding.fillAmount = Mathf.InverseLerp(30, 224f, (float)diff.TotalSeconds);
    }
}
