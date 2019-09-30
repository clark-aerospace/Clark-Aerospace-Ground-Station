using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GraphLib;

public class Graph2020Test : MonoBehaviour
{
    public Button button;
    public LineGraph graph;
    public float xVal = -1f;
    public float yVal = 0;

    public void AddPoint() {
        xVal++;
        yVal += Random.Range(0f, 1f);

        Vector2 vec = new Vector2(xVal, yVal);
        Debug.Log(string.Format("Plotting point {0}", vec));

        graph.AddPoint(vec, 2, true);
    }
}
