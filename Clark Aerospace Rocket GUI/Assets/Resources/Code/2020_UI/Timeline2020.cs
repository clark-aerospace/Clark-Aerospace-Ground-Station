using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timeline2020 : MonoBehaviour
{
    public List<TimelineStop> stops = new List<TimelineStop>();
    public float iconSize = 50f;


    void Start()
    {
        float width = GetComponent<RectTransform>().sizeDelta.x;

        float soFar = 0f;

        float incrementPerItem = width / stops.Count;

        foreach (TimelineStop stop in stops) {
            GameObject newTimelineStop = new GameObject("Timeline Stop - " + stop.name);
            RectTransform iconRectTransform = newTimelineStop.AddComponent<RectTransform>();
            iconRectTransform.SetParent(transform, false);
            iconRectTransform.sizeDelta = new Vector2(iconSize, iconSize);
            iconRectTransform.anchorMax = new Vector2(0.0f, 0.5f);
            iconRectTransform.anchorMin = new Vector2(0.0f, 0.5f);
            iconRectTransform.anchoredPosition = new Vector2(soFar, 0f);
            soFar += incrementPerItem;



        }
    }
}

[System.Serializable]
public class TimelineStop {

    [Header("Editable Properties")]
    public string name = "Stop";
    public int secondsToActivate = 0;
    public Sprite icon;

    [Header("Internal Properties")]
    public GameObject displayIconCircle;
    public GameObject displayIcon;
    public TextMeshProUGUI displayNameLabel;
    public TextMeshProUGUI displayTimeLabel;
}
