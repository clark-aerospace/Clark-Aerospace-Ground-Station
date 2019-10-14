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

    public bool boop = false;

    public void Start() {
        DOTween.Init();

        float a = 0;
        float b = 1;
        float val = 2;

        float origLerpInverse = Mathf.InverseLerp(a, b, val);
        float myLerpInverse = NiceExtensions.InverseLerpUnclamped(a, b, val);
        Debug.Log(string.Format("Normal lerp inverse gives {0}, mine gives {1}", origLerpInverse, myLerpInverse));
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
            if (boop) {
                graph.SetBoundsAnimated(new Vector2(0, 10), new Vector2(0, 10), 0.5f);
            } else {
                graph.SetBoundsAnimated(new Vector2(-3, 5), new Vector2(-2, 4), 0.5f);
            }
            boop = !boop;
        }
    }
}
