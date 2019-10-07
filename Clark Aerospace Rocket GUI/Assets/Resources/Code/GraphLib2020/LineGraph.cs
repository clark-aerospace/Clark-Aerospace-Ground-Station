using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI.Extensions;


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

        public AxisGraphScaleType xScaleType = AxisGraphScaleType.ScaleAutomatically;
        public AxisGraphScaleType yScaleType = AxisGraphScaleType.ScaleAutomatically;

        public Vector2 xBounds = new Vector2(0, 1);
        public Vector2 yBounds = new Vector2(0, 1);


        [Header("Grid")]
        public Color gridColor = Color.black;


        private RectTransform rectTransform;

        private UIGridRenderer gridRenderer;

        void Start() {
            rectTransform = GetComponent<RectTransform>();
            // Create the line renderer

            foreach (DataSet d in dataSets) {
                d.lineRenderer = new GameObject("DataSet Line Renderer").AddComponent<UILineRenderer>();
                d.lineRenderer.transform.SetParent(transform, false);

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
            gridRenderer.GridColumns = Mathf.FloorToInt(intervals.x);
            gridRenderer.GridRows = Mathf.FloorToInt(intervals.y);
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

                    point.x -= xBounds.x;
                    point.y -= yBounds.x;
                    
                    point.x /= xBounds.y;
                    point.y /= yBounds.y;

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