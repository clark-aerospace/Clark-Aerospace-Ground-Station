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

    public Button openSettingsButton;
    public Button saveSettingsButton;
    public GameObject settingsPanel;

    [Header("Timer thing")]
    public TextMeshProUGUI launchTimerLabel;
    public Button openTimelineButton;
    public Button closeTimelineButton;
    public GameObject timelinePanel;

    public int launchEpochTime;

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
    }

    public void SaveSettings() {
        PlayerPrefs.SetString("port_name", arduinoPortsList.SelectedItem.ID);
        PlayerPrefs.SetInt("port_baud_rate", int.Parse(arduinoBaudRateInput.text));
        settingsPanel.SetActive(false);
    }

    public void OpenTimeline() {
        timelinePanel.SetActive(true);
    }

    public void CloseTimeline() {
        timelinePanel.SetActive(false);
    }

    public void Update() {
        DateTime departure = new DateTime(2019, 5, 13, 12, 17, 0);

        TimeSpan diff = DateTime.Now - departure;

        string symbol = (diff > TimeSpan.Zero) ? "+" : "-";
        launchTimerLabel.text = "T" + symbol + diff.ToString(@"mm\:ss");
    }
}
