﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;



namespace GraphLib {
    public class LineGraph : MonoBehaviour
    {

        public float lineWidth = 3f;
        public float animationSpeed = 3f;
        public List<DataSet> dataSets = new List<DataSet>();

        public enum AxisGraphScaleType {
            ScaleAutomatically,
            ScaleManually
        }

        public enum AxisGraphLabelType {
            Normal,
            Time,
            TimeRelativeToNow,
            Percent
        }

        public enum AxisGraphDotType {
            NoDots,
            DotOnEveryPoint,
            DotOnLastPoint
        }

        public AxisGraphScaleType xScaleType = AxisGraphScaleType.ScaleAutomatically;
        public AxisGraphScaleType yScaleType = AxisGraphScaleType.ScaleAutomatically;

        public AxisGraphLabelType xLabelType = AxisGraphLabelType.Normal;
        public AxisGraphLabelType yLabelType = AxisGraphLabelType.Normal;

        public AxisGraphDotType xDotType = AxisGraphDotType.NoDots;
        public AxisGraphDotType yDotType = AxisGraphDotType.NoDots;

        public Vector2 xBounds = new Vector2(0, 1);
        public Vector2 yBounds = new Vector2(0, 1);

        public float GridAlpha {
            get {
                return gridColor.a;
            } set {
                gridColor.a = value;
            }
        }

        public float xMinimum {
            get {
                return xBounds.x;
            } set {
                xBounds.x = value;

                ResetAxisLabels();
            }
        }

        public float xMaximum {
            get {
                return xBounds.y;
            } set {
                xBounds.y = value;

                ResetAxisLabels();
            }
        }

        public float yMinimum {
            get {
                return yBounds.x;
            } set {
                yBounds.x = value;
                ResetAxisLabels();
            }
        }

        public float yMaximum {
            get {
                return yBounds.y;
            } set {
                yBounds.y = value;
                ResetAxisLabels();
            }
        }


        [Header("Grid")]
        public Color gridColor = Color.black;

        /// <summary>
        /// The number of x axis lines in between the edges of the graph.
        /// The true number of lines is thus this value, plus 2.
        /// </summary>
        public int xAxisLines = 5;

        /// <summary>
        /// The number of y axis lines in between the edges of the graph.
        /// The true number of lines is thus this value, plus 2.
        /// </summary>
        public int yAxisLines = 5;


        public List<TextMeshProUGUI> xAxisLabels = new List<TextMeshProUGUI>();
        public List<TextMeshProUGUI> yAxisLabels = new List<TextMeshProUGUI>();


        private RectTransform rectTransform;

        private UIGridRenderer gridRenderer;

        void Start() {
            rectTransform = GetComponent<RectTransform>();

            // Create container for line renderers
            RectTransform lineRendererContainer = new GameObject("Line Renderer Container").AddComponent<RectTransform>();
            lineRendererContainer.SetParent(transform, false);
            lineRendererContainer.anchorMin = new Vector2(0,0);
            lineRendererContainer.anchorMax = new Vector2(1,1);
            lineRendererContainer.sizeDelta = new Vector2(1,1);
            lineRendererContainer.anchoredPosition = Vector2.zero;
            lineRendererContainer.gameObject.AddComponent<Mask>();

            Image a = lineRendererContainer.gameObject.AddComponent<Image>();
            a.SetAlpha(0.01f);

            // Create the line renderer

            foreach (DataSet d in dataSets) {
                d.lineRenderer = new GameObject("DataSet Line Renderer").AddComponent<UILineRenderer>();
                d.lineRenderer.transform.SetParent(lineRendererContainer.transform, false);

                d.lineRenderer.GetComponent<RectTransform>().StretchToFill();

                d.lineRenderer.color = d.mainColor;
                d.lineRenderer.RelativeSize = true;



                //d.lineRenderer.BezierMode = d.useBezier ? UILineRenderer.BezierType.Quick : UILineRenderer.BezierType.None;
            }


            // TODO: create the grid background

            gridRenderer = new GameObject("Grid Renderer").AddComponent<UIGridRenderer>();
            gridRenderer.transform.SetParent(transform, false);
            gridRenderer.transform.SetAsFirstSibling();
            gridRenderer.GetComponent<RectTransform>().StretchToFill();

            Vector2 intervals = GetTickInterval();
            // gridRenderer.GridColumns = Mathf.FloorToInt(intervals.x);
            gridRenderer.GridRows = Mathf.FloorToInt(intervals.y);


            // Create ticks on x axis
            float xAxisDistancePerTick = (xBounds.y - xBounds.x) / xAxisLines;
            gridRenderer.GridColumns = xAxisLines;
            for (int i = 0; i <= xAxisLines; i++) {
                TextMeshProUGUI newLabel = new GameObject("X Axis Tick Label").AddComponent<TextMeshProUGUI>();
                RectTransform newLabelRectTransform = newLabel.GetComponent<RectTransform>();

                newLabelRectTransform.SetParent(rectTransform, false);

                Vector2 anchorVec = new Vector2((float)i/(float)xAxisLines, 0);
                newLabelRectTransform.anchorMin = anchorVec;
                newLabelRectTransform.anchorMax = anchorVec;
                newLabelRectTransform.pivot = new Vector2(0.5f, 1f);
                newLabelRectTransform.anchoredPosition = new Vector2(0f, -5f);
                newLabelRectTransform.sizeDelta = new Vector2(50, 20);

                switch (xLabelType) {
                    case AxisGraphLabelType.Percent:
                        newLabel.text = ((xBounds.x + (xAxisDistancePerTick) * i) * 100).ToString() + "%";
                        break;
                    case AxisGraphLabelType.Time:
                        break;
                    case AxisGraphLabelType.TimeRelativeToNow:
                        if (i == xAxisLines) {
                            newLabel.text = "Now";
                        } else {
                            int secondsAgo = (int)(xBounds.y - (xBounds.x + (xAxisDistancePerTick) * i));
                            //int minutesAgo = secondsAgo % 60;
                            newLabel.text = string.Format("{0}s", secondsAgo);
                        }
                        break;
                    default:
                        newLabel.text = (xBounds.x + (xAxisDistancePerTick) * i).ToString();
                        break;
                }
                newLabel.alignment = TextAlignmentOptions.Top;
                newLabel.enableWordWrapping = false;
                xAxisLabels.Add(newLabel);

            }

            // Create ticks on y axis
            float yAxisDistancePerTick = (yBounds.y - yBounds.x) / yAxisLines;
            gridRenderer.GridRows = yAxisLines;
            for (int i = 0; i <= yAxisLines; i++) {
                TextMeshProUGUI newLabel = new GameObject("Y Axis Tick Label").AddComponent<TextMeshProUGUI>();
                RectTransform newLabelRectTransform = newLabel.GetComponent<RectTransform>();

                newLabelRectTransform.SetParent(rectTransform, false);

                Vector2 anchorVec = new Vector2(0, (float)i/(float)yAxisLines);
                newLabelRectTransform.anchorMin = anchorVec;
                newLabelRectTransform.anchorMax = anchorVec;
                newLabelRectTransform.pivot = new Vector2(1f, 0.5f);
                newLabelRectTransform.anchoredPosition = new Vector2(-5f, 0f);
                newLabelRectTransform.sizeDelta = new Vector2(50, 20);

                switch (yLabelType) {
                    case AxisGraphLabelType.Percent:
                        newLabel.text = ((yBounds.x + (yAxisDistancePerTick) * i) * 100).ToString() + "%";
                        break;
                    case AxisGraphLabelType.Time:
                        break;
                    case AxisGraphLabelType.TimeRelativeToNow:
                        if (i == yAxisLines) {
                            newLabel.text = "Now";
                        } else {
                            int secondsAgo = (int)(yBounds.y - (yBounds.x + (yAxisDistancePerTick) * i));
                            //int minutesAgo = secondsAgo % 60;
                            newLabel.text = string.Format("{0}s", secondsAgo);
                        }
                        break;
                    default:
                        newLabel.text = (yBounds.x + (yAxisDistancePerTick) * i).ToString();
                        break;
                }

                newLabel.alignment = TextAlignmentOptions.Right;
                newLabel.enableWordWrapping = false;
                yAxisLabels.Add(newLabel);

            }
        }


        void Update() {


            foreach (DataSet d in dataSets) {
                int lengthOfArray = d.values.Count;
                if (d.currentlyInterpolating) {
                    if (d.interpolateAmt >= 1f) {
                        // finished interpolating, so add it to the list
                        d.values.Add(d.valueToAdd);
                        d.currentlyInterpolating = false;
                        d.interpolateAmt = 0f;                        
                    }
                    lengthOfArray++;
                }

                Vector2[] newPointArray = new Vector2[lengthOfArray];
                Vector2 lastPoint = new Vector2(0,0);
                for (int i = 0; i < d.values.Count; i++) {
                    // Let's scale the points to fit the whatchamacallit

                    Vector2 point = d.values[i];

                    // point.x /= xBounds.y;
                    // point.y /= yBounds.y;

                    // point.x -= xBounds.x / xBounds.y;
                    // point.y -= yBounds.x / yBounds.y;

                    point.x = Mathf.InverseLerp(xBounds.x, xBounds.y, point.x);
                    point.y = Mathf.InverseLerp(yBounds.x, yBounds.y, point.y);

                    newPointArray[i] = point;
                    lastPoint = point;
                }

                if (d.currentlyInterpolating) {
                    float interpX = d.valueToAdd.x / xBounds.y;
                    float interpY = d.valueToAdd.y / yBounds.y;
                    Vector2 interpPoint = new Vector2(Mathf.SmoothStep(lastPoint.x, interpX, d.interpolateAmt), Mathf.SmoothStep(lastPoint.y, interpY, d.interpolateAmt));
                    newPointArray[lengthOfArray-1] = interpPoint;
                    d.interpolateAmt += Time.deltaTime * animationSpeed;
                }
                d.lineRenderer.Points = newPointArray;
                d.lineRenderer.LineThickness = lineWidth;

                
                d.lineRenderer.color = d.mainColor;

            }

            gridRenderer.color = gridColor;
    }

    public void AddPoint(Vector2 pt, int dataSetIndex, bool animate = true) {
        if (dataSets[dataSetIndex].currentlyInterpolating) {
            // if an interpolating is currently in progress, skip to its end
            dataSets[dataSetIndex].values.Add(dataSets[dataSetIndex].valueToAdd);
            dataSets[dataSetIndex].currentlyInterpolating = false;
            dataSets[dataSetIndex].interpolateAmt = 0f;
        }

        if (!animate) {
            dataSets[dataSetIndex].values.Add(pt);
        } else {
            dataSets[dataSetIndex].valueToAdd = pt;
            dataSets[dataSetIndex].interpolateAmt = 0;
            dataSets[dataSetIndex].currentlyInterpolating = true;
        }
    }


    /// <summary>
    /// Returns the point of the largest X value in the current data sets
    /// </summary>
    Vector2 GetMaximumXValue() {
        Vector2 largestX = Vector2.zero;
        foreach (DataSet d in dataSets) {
            foreach (Vector2 pt in d.values) {
                if (pt.x >= largestX.x) largestX = pt;
            }
        }

        return largestX;
    }

    /// <summary>
    /// Returns the point of the largest Y value in the current data sets
    /// </summary>
    Vector2 GetMaximumYValue() {
        Vector2 largestY = Vector2.zero;
        foreach (DataSet d in dataSets) {
            foreach (Vector2 pt in d.values) {
                if (pt.y >= largestY.y) largestY = pt;
            }
        }

        return largestY;
    }

    /// <summary>
    /// Returns the point of the smallest Y value in the current data sets
    /// </summary>
    Vector2 GetMinimumYValue() {
        Vector2 smallestY = new Vector2(0, float.PositiveInfinity);
        foreach (DataSet d in dataSets) {
            foreach (Vector2 pt in d.values) {
                if (pt.y < smallestY.y) smallestY = pt;
            }
        }

        return smallestY;
    }

    /// <summary>
    /// Returns the point of the smallest X value in the current data sets
    /// </summary>
    Vector2 GetMinimumXValue() {
        Vector2 smallestX = new Vector2(float.PositiveInfinity, 0);
        foreach (DataSet d in dataSets) {
            foreach (Vector2 pt in d.values) {
                if (pt.x < smallestX.x) smallestX = pt;
            }
        }

        return smallestX;
    }



    public Vector2 GetTickInterval() {

        Vector2 xMinMax = new Vector2(0,1);
        Vector2 yMinMax = new Vector2(0,1);

        if (xScaleType == AxisGraphScaleType.ScaleAutomatically) {
            xMinMax = new Vector2(GetMinimumXValue().x, GetMaximumXValue().x);
        } else {
            xMinMax = xBounds;
        }

        if (yScaleType == AxisGraphScaleType.ScaleAutomatically) {
            xMinMax = new Vector2(GetMinimumYValue().y, GetMaximumYValue().y);
        } else {
            yMinMax = yBounds;
        }

        float xRange = Mathf.Ceil(xMinMax.y - xMinMax.x);

        // We now have the range of X values. Let's find the CLOSEST number
        // to it that fits a good number (power of 1, 5, or 10).

        float xSpacePerLine = CalcStepSize(xRange == 0 ? 1f : xRange, 16f);
        //Debug.Log("Step size is " + xSpacePerLine.ToString());

        // the number of steps would be 
    
        float xTicks = xRange / xSpacePerLine;

        //Debug.Log("Max Y is " + GetMaxValue(Axis.Y, false).y.ToString());

        float yRange = Mathf.Ceil(yMinMax.y - yMinMax.x);
        float ySpacePerLine = CalcStepSize(yRange == 0 ? 1f : yRange, 10f);
        float yTicks = yRange / ySpacePerLine;

        return new Vector2(xTicks, yTicks);
    }

    public static float CalcStepSize(float range, float targetSteps)
    {
        // calculate an initial guess at step size
        var tempStep = range/targetSteps;

        // get the magnitude of the step size
        var mag = (float)Mathf.Floor(Mathf.Log10(tempStep));
        var magPow = (float)Mathf.Pow(10, mag);

        // calculate most significant digit of the new step size
        var magMsd = (int)(tempStep/magPow + 0.5);

        // promote the MSD to either 1, 2, or 5
        if (magMsd > 5)
            magMsd = 10;
        else if (magMsd > 2)
            magMsd = 5;
        else if (magMsd > 1)
            magMsd = 2;

        return magMsd*magPow;
    }


    public void ResetAxisLabels() {
        float xAxisDistancePerTick = (xBounds.y - xBounds.x) / xAxisLines;
        float yAxisDistancePerTick = (yBounds.y - yBounds.x) / yAxisLines;

        for (int i = 0; i <= xAxisLines; i++) {
            TextMeshProUGUI newLabel = xAxisLabels[i];

            switch (xLabelType) {
                case AxisGraphLabelType.Percent:
                    newLabel.text = ((xBounds.x + (xAxisDistancePerTick) * i) * 100).ToString() + "%";
                    break;
                case AxisGraphLabelType.Time:
                    break;
                case AxisGraphLabelType.TimeRelativeToNow:
                    if (i == xAxisLines) {
                        newLabel.text = "Now";
                    } else {
                        int secondsAgo = (int)(xBounds.y - (xBounds.x + (xAxisDistancePerTick) * i));
                        //int minutesAgo = secondsAgo % 60;
                        newLabel.text = string.Format("{0}s", secondsAgo);
                    }
                    break;
                default:
                    newLabel.text = (xBounds.x + (xAxisDistancePerTick) * i).ToString();
                    break;
            }
        }

        for (int i = 0; i <= yAxisLines; i++) {
            TextMeshProUGUI newLabel = yAxisLabels[i];
            switch (yLabelType) {
                case AxisGraphLabelType.Percent:
                    newLabel.text = ((yBounds.x + (yAxisDistancePerTick) * i) * 100).ToString() + "%";
                    break;
                case AxisGraphLabelType.Time:
                    break;
                case AxisGraphLabelType.TimeRelativeToNow:
                    if (i == yAxisLines) {
                        newLabel.text = "Now";
                    } else {
                        int secondsAgo = (int)(yBounds.y - (yBounds.x + (yAxisDistancePerTick) * i));
                        //int minutesAgo = secondsAgo % 60;
                        newLabel.text = string.Format("{0}s", secondsAgo);
                    }
                    break;
                default:
                    newLabel.text = (yBounds.x + (yAxisDistancePerTick) * i).ToString();
                    break;
            }
        }
    }

    }

    // [CustomEditor(typeof(LineGraph))]
    // public class LineGraphEditor : Editor 
    // {
    //     SerializedProperty listOfDataSets;
    //     SerializedProperty xAxisScaleTypeProperty;
    //     SerializedProperty yAxisScaleTypeProperty;
    //     SerializedProperty xAxisManualScaleProperty;
    //     SerializedProperty yAxisManualScaleProperty;

    //     LineGraph.AxisGraphScaleType xAxisScaleType;
    //     LineGraph.AxisGraphScaleType yAxisScaleType;


    //     void OnEnable() {
    //         listOfDataSets = serializedObject.FindProperty("dataSets");

    //         xAxisScaleTypeProperty = serializedObject.FindProperty("xScaleType");
    //         yAxisScaleTypeProperty = serializedObject.FindProperty("yScaleType");

    //         xAxisManualScaleProperty = serializedObject.FindProperty("xBounds");
    //         yAxisManualScaleProperty = serializedObject.FindProperty("yBounds");

    //     }

    //     public override void OnInspectorGUI()
    //     {
    //         serializedObject.Update();
    //         EditorGUILayout.LabelField("Data Sets", EditorStyles.boldLabel);
    //         EditorGUILayout.PropertyField(listOfDataSets);

    //         EditorGUILayout.Space();

    //         EditorGUILayout.LabelField("Scaling", EditorStyles.boldLabel);

    //         xAxisScaleType = (LineGraph.AxisGraphScaleType)EditorGUILayout.EnumPopup("X axis", xAxisScaleType);

    //         if (xAxisScaleType == LineGraph.AxisGraphScaleType.ScaleManually) {
    //             float xMin = EditorGUILayout.FloatField("X axis min", xAxisManualScaleProperty.vector2Value.x);
    //             float xMax = EditorGUILayout.FloatField("X axis max", xAxisManualScaleProperty.vector2Value.y);

    //             xAxisManualScaleProperty.vector2Value = new Vector2(xMin, xMax);
    //         }

    //         yAxisScaleType = (LineGraph.AxisGraphScaleType)EditorGUILayout.EnumPopup("Y axis", yAxisScaleType);

    //         if (yAxisScaleType == LineGraph.AxisGraphScaleType.ScaleManually) {
    //             float yMin = EditorGUILayout.FloatField("Y axis min", yAxisManualScaleProperty.vector2Value.x);
    //             float yMax = EditorGUILayout.FloatField("Y axis max", yAxisManualScaleProperty.vector2Value.y);

    //             yAxisManualScaleProperty.vector2Value = new Vector2(yMin, yMax);
    //         }

    //         serializedObject.ApplyModifiedProperties ();
    //     }
    // }
}