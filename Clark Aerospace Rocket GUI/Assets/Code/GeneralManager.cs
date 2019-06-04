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

    public Button openSettingsButton;
    public Button saveSettingsButton;
    public GameObject settingsPanel;

    [Header("Timeline Stuff")]
    public TextMeshProUGUI launchTimerLabel;
    public TextMeshProUGUI bigLaunchTimerLabel;
    public Button openTimelineButton;
    public Button closeTimelineButton;
    public GameObject timelinePanel;
    public Image timelineBar;

    public int launchEpochTime;

    public DateTime departureTime = new DateTime(2019, 5, 14, 15, 00, 0);

    public int quitLength = 0;


    public void Start() {
        openSettingsButton.onClick.AddListener(OpenSettings);
        saveSettingsButton.onClick.AddListener(SaveSettings);

        openTimelineButton.onClick.AddListener(OpenTimeline);
        closeTimelineButton.onClick.AddListener(CloseTimeline);

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
        arduinoBaudRateInput.text = PlayerPrefs.GetInt("port_baud_rate").ToString();
    }

    public void SaveSettings() {
        PlayerPrefs.SetString("port_name", arduinoPortsList.SelectedItem.ID);
        PlayerPrefs.SetInt("port_baud_rate", int.Parse(arduinoBaudRateInput.text));

        ArduinoReciever.reciever.serialPort.Close();

        ArduinoReciever.reciever.serialPort.PortName = arduinoPortsList.SelectedItem.ID;
        ArduinoReciever.reciever.serialPort.BaudRate = int.Parse(arduinoBaudRateInput.text);
        ArduinoReciever.reciever.serialPort.Open();

        departureTime = DateTime.Parse(launchTimeInput.text);
        settingsPanel.SetActive(false);
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

        timelineBar.fillAmount = Mathf.InverseLerp(0f, 20f * 60f, (float)diff.TotalSeconds);
    }
}
