using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GraphLib; 
using DG.Tweening;
using TMPro;

public class Graph2020Test : MonoBehaviour
{
    public Button button;
    public LineGraph graph;
    public float xVal = -1f;
    public float yVal = 0;

    public float v = -5.0f;

    public void Start() {
        DOTween.Init();
    }

    public void AddPoint() {
        xVal++;
        yVal += Random.Range(0f, 1f);

        Vector2 vec = new Vector2(xVal, yVal);
        Debug.Log(string.Format("Plotting point {0}", vec));

        graph.AddPoint(vec, 2, true);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {

            graph.SetXMinMaxAnimated(v, 10f, 0.5f);
            if (v == -5.0f) v = 0f;
            else v = -5.0f;
        }
    }
}
