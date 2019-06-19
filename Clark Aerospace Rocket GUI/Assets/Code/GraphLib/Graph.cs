using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;

public class Graph : MonoBehaviour
{

    public static List<Graph> graphs = new List<Graph>();

    public string XAxisLabel {
        set {
            _xAxisLabelText = value;
            if (xAxisLabel != null) {xAxisLabel.text = value;}
        }
        get {
            return _xAxisLabelText;
        }
    }

    public string YAxisLabel {
        set {
            _yAxisLabelText = value;
            if (yAxisLabel != null) {yAxisLabel.text = value;}
        }
        get {
            return _yAxisLabelText;
        }
    }

    private string _xAxisLabelText, _yAxisLabelText;


    public enum Axis {
        X = 0,
        Y = 1
    }
    private List<Vector2> points = new List<Vector2>();
    public int maxValuesOnGraph = 0;

    public bool useSimplification = true;
    public int simplificationAmt = 0;
    public bool addTestPoints = false;
    public bool scalePoints = true;

    // if true, the x axis won't be scaled to set amounts
    public bool xAxisFluidMax = false;

    public UILineRenderer uiLineRenderer;
    public UIGridRenderer gridRenderer;

    [SerializeField]
    private RectTransform rectTransform;

    public Color color = Color.white;

    public Image dotImage;
    public Sprite dotSprite;

    // MESH stuff
    Vector3[] verts;
    int[] tris;
    Mesh mesh;
    MeshRenderer mRenderer;
    MeshFilter mFilter;

    public Material backMat;

    public float xPadding = 1f;
    public float yPadding = 1f;

    float currentYPt = 5;
    float time = 3;

    int thing = 0;

    public float xSpacePerLine = 1;
    public float ySpacePerLine = 1;

    [Header("Labels")]
    public TextMeshProUGUI xAxisMaxLabel;
    public TextMeshProUGUI xAxisMinLabel;

    public TextMeshProUGUI yAxisMinLabel;
    public TextMeshProUGUI yAxisMaxLabel;

    public TextMeshProUGUI xAxisLabel;
    public TextMeshProUGUI yAxisLabel;
    public TMP_FontAsset defaultTextFont;

    public RectTransform scrolleyThing;
    public TextMeshProUGUI scrolleyThing_Label;


    [Header("TESTING")]
    public TextMeshProUGUI latestPointBox;


    public static void ClearAllGraphs() {
        foreach (Graph g in graphs) {
            g.ClearData();
        }
    }

    public void Start() {
        graphs.Add(this);
        uiLineRenderer = new GameObject("Graph Line Renderer").AddComponent<UILineRenderer>();
        uiLineRenderer.RelativeSize = true;
        uiLineRenderer.transform.SetParent(transform, false);
        uiLineRenderer.LineThickness = 3;

        RectTransform rT = uiLineRenderer.rectTransform;
        rT.anchorMax = new Vector2(1,1);
        rT.anchorMin = new Vector2(0,0);
        rT.anchoredPosition = new Vector2(0,0);
        rT.pivot = new Vector2(0.5f,0.5f);
        rT.offsetMax = new Vector2(1,1);
        rT.offsetMin = new Vector2(0,0);


        gridRenderer = gameObject.AddComponent<UIGridRenderer>();
        gridRenderer.color = new Color(0.5f, 0.5f, 0.5f);

        rectTransform = GetComponent<RectTransform>();


        // dotImage = new GameObject("Dot Image").AddComponent<Image>();
        // dotImage.transform.SetParent(transform, false);
        // dotImage.sprite = dotSprite;
        // dotImage.color = color;
        // dotImage.useSpriteMesh = true;
        // dotImage.rectTransform.sizeDelta = new Vector2(10,10);

        // mFilter = new GameObject("Area Mesh").AddComponent<MeshFilter>();
        // mFilter.transform.SetParent(transform, false);
        // mRenderer = mFilter.gameObject.AddComponent<MeshRenderer>();

        // mesh = new Mesh();
        // mFilter.mesh = mesh;

        // Create the X axis Max label
        xAxisMaxLabel = new GameObject("X Axis Max Label").AddComponent<TextMeshProUGUI>();
        xAxisMaxLabel.rectTransform.SetParent(transform, false);
        xAxisMaxLabel.rectTransform.pivot = new Vector2(1f,1f);
        xAxisMaxLabel.rectTransform.anchorMax = new Vector2(1f,0f);
        xAxisMaxLabel.rectTransform.anchorMin = new Vector2(1f,0f);
        xAxisMaxLabel.rectTransform.anchoredPosition = new Vector2(0f, 0f);
        xAxisMaxLabel.rectTransform.sizeDelta = new Vector2(50,25);
        xAxisMaxLabel.font = defaultTextFont;
        xAxisMaxLabel.fontSize = 15;
        xAxisMaxLabel.text = "Hello!";
        xAxisMaxLabel.alignment = TextAlignmentOptions.MidlineRight;


        // Create the X axis Min label
        xAxisMinLabel = new GameObject("X Axis Min Label").AddComponent<TextMeshProUGUI>();
        xAxisMinLabel.rectTransform.SetParent(transform, false);
        xAxisMinLabel.rectTransform.pivot = new Vector2(0f,1f);
        xAxisMinLabel.rectTransform.anchorMax = new Vector2(0f,0f);
        xAxisMinLabel.rectTransform.anchorMin = new Vector2(0f,0f);
        xAxisMinLabel.rectTransform.anchoredPosition = new Vector2(0f, 0f);
        xAxisMinLabel.rectTransform.sizeDelta = new Vector2(50,25);
        xAxisMinLabel.font = defaultTextFont;
        xAxisMinLabel.fontSize = 15;
        xAxisMinLabel.alignment = TextAlignmentOptions.MidlineLeft;
        xAxisMinLabel.text = "0";

        // Create the Y axis Min label
        yAxisMinLabel = new GameObject("Y Axis Min Label").AddComponent<TextMeshProUGUI>();
        yAxisMinLabel.rectTransform.SetParent(transform, false);
        yAxisMinLabel.rectTransform.pivot = new Vector2(1f,0f);
        yAxisMinLabel.rectTransform.anchorMax = new Vector2(0f,0f);
        yAxisMinLabel.rectTransform.anchorMin = new Vector2(0f,0f);
        yAxisMinLabel.rectTransform.anchoredPosition = new Vector2(-10f, 0f);
        yAxisMinLabel.rectTransform.sizeDelta = new Vector2(50,25);
        yAxisMinLabel.font = defaultTextFont;
        yAxisMinLabel.fontSize = 15;
        yAxisMinLabel.alignment = TextAlignmentOptions.BottomRight;
        yAxisMinLabel.text = "0";

        // Create the Y axis Max label
        yAxisMaxLabel = new GameObject("Y Axis Max Label").AddComponent<TextMeshProUGUI>();
        yAxisMaxLabel.rectTransform.SetParent(transform, false);
        yAxisMaxLabel.rectTransform.pivot = new Vector2(1f,1f);
        yAxisMaxLabel.rectTransform.anchorMax = new Vector2(0f,1f);
        yAxisMaxLabel.rectTransform.anchorMin = new Vector2(0f,1f);
        yAxisMaxLabel.rectTransform.anchoredPosition = new Vector2(-10f, 0f);
        yAxisMaxLabel.rectTransform.sizeDelta = new Vector2(50,25);
        yAxisMaxLabel.font = defaultTextFont;
        yAxisMaxLabel.fontSize = 15;
        yAxisMaxLabel.alignment = TextAlignmentOptions.TopRight;
        yAxisMaxLabel.text = "?";


        // Create the Y axis label
        yAxisLabel = new GameObject("Y Axis Label").AddComponent<TextMeshProUGUI>();
        yAxisLabel.rectTransform.SetParent(transform, false);
        yAxisLabel.rectTransform.pivot = new Vector2(0.5f,0f);
        yAxisLabel.rectTransform.anchorMax = new Vector2(0f,0.5f);
        yAxisLabel.rectTransform.anchorMin = new Vector2(0f,0.5f);
        yAxisLabel.rectTransform.anchoredPosition = new Vector2(-10f, 0f);
        yAxisLabel.rectTransform.sizeDelta = new Vector2(100,25);
        yAxisLabel.rectTransform.localEulerAngles = new Vector3(0f,0f,90f);
        yAxisLabel.font = defaultTextFont;
        yAxisLabel.fontSize = 15;
        yAxisLabel.alignment = TextAlignmentOptions.Center;
        yAxisLabel.text = "Y Axis";

        // Create the X axis label
        xAxisLabel = new GameObject("X Axis Label").AddComponent<TextMeshProUGUI>();
        xAxisLabel.rectTransform.SetParent(transform, false);
        xAxisLabel.rectTransform.pivot = new Vector2(0.5f,1f);
        xAxisLabel.rectTransform.anchorMax = new Vector2(0.5f,0f);
        xAxisLabel.rectTransform.anchorMin = new Vector2(0.5f,0f);
        xAxisLabel.rectTransform.anchoredPosition = new Vector2(0f, 0f);
        xAxisLabel.rectTransform.sizeDelta = new Vector2(100,25);
        xAxisLabel.font = defaultTextFont;
        xAxisLabel.fontSize = 15;
        xAxisLabel.alignment = TextAlignmentOptions.Center;
        xAxisLabel.text = "X Axis";


        // AddPoint(1, 1);
        // AddPoint(2, 3);
        // AddPoint(3, 5);
        // AddPoint(4, 6);
        // AddPoint(5, 7);
        // AddPoint(6, 8);
        // AddPoint(7, 10);
        // AddPoint(8, 12);
        // AddPoint(9, 13);
        // AddPoint(10, 15);
        // AddPoint(11, 17);
        // AddPoint(12, 19);
        // AddPoint(13, 16);
        // AddPoint(14, 20);
        // AddPoint(15, 24);
        // AddPoint(16, 27);
        // AddPoint(17, 30);
        // AddPoint(18, 34);
        // ResetLineRenderer();

        //SetLineColor();
        
        //GenerateMesh();
        


        
        
    }

    public void Update() {
        //return;

        xAxisLabel.text = _xAxisLabelText;
        yAxisLabel.text = _yAxisLabelText;

        SetLineColor();
        //OnMouseOver();
        //GenerateMesh();

        if (thing >= 5 && addTestPoints) {AddTestPoint();}
        if (addTestPoints) {thing++;}

        ResetLineRenderer();
        Vector2 intervals = GetTickInterval();
        gridRenderer.GridColumns = Mathf.CeilToInt(intervals.x);
        Debug.LogWarning((intervals.y));
        gridRenderer.GridRows = Mathf.CeilToInt(intervals.y);

        
        xAxisMaxLabel.text = xAxisFluidMax ? "Now" : (gridRenderer.GridColumns * xSpacePerLine).ToString();
        yAxisMaxLabel.text = (gridRenderer.GridRows * ySpacePerLine).ToString();

        //xAxisMinLabel.text = GetMinValue(Axis.X, false).x.ToString("0");
        //yAxisMinLabel.text = GetMinValue(Axis.Y, false).y.ToString("0");




        Debug.Log("------");
    }

    public void ClearData() {
        Vector2 lastPoint = points[points.Count-1];
        points.Clear();
        //AddPoint(0, 0);
        Update();
    }
    public void AddTestPoint() {
        if (points.Count == 0) {

        }
        currentYPt = currentYPt + Random.Range(-1f, 2f);
        AddPoint(time, currentYPt);
        //Debug.Log(time.ToString() + " , " + currentYPt.ToString());
        time++;
        thing = 0;

        //latestPointBox.text = time.ToString() + " , " + currentYPt.ToString();
        //ResetLineRenderer();
    }



    public void GenerateMesh() {

        Vector3[] _pts = Vec2ArrayToVec3Array(uiLineRenderer.Points);

        Vector2[] pts = ScaledPoints(Vec3ArrayToVec2Array(_pts));
        pts = points.ToArray();

        Color colTop = color * 0.7f;
        colTop.a = 0.4f;

        Color colBottom = color * 0.7f;
        colBottom.a = 0f;


        verts = new Vector3[pts.Length * 2];
        Color[] colors = new Color[pts.Length * 2];
        tris = new int[(pts.Length - 1) * 6];

        for (int i = 0; i < pts.Length; i++) {
            verts[i] = ScalePoint(Vec2ArrayToVec3Array(pts)[i]);
            verts[i + pts.Length] = new Vector3(verts[i].x, -(rectTransform.rect.height * rectTransform.pivot.y), 0f);

            if (verts[i].y != 0) {
                colors[i] = colTop;
            } else {
                colors[i] = colBottom;
            }
        }
        mesh.vertices = verts;
        for (int i = 0; i < pts.Length - 1; i++) {
            tris[i * 6] = i;
            tris[i * 6 + 1] = i + 1;
            tris[i * 6 + 2] = i + pts.Length;
            tris[i * 6 + 3] = i + 1;
            tris[i * 6 + 4] = i + pts.Length + 1;
            tris[i * 6 + 5] = i + pts.Length;
        }
        mesh.triangles = tris;
        mesh.colors = colors;

        mRenderer.material = new Material(Shader.Find("Unlit/GraphBG"));



        mRenderer.material.SetColor("_ColTop", colTop);



        mRenderer.material.SetColor("_ColBottom", colBottom);
        mRenderer.material.SetFloat("_HeightInc", rectTransform.rect.height * 0.75f);
        mRenderer.transform.localPosition = new Vector3(0f, 0f, -5f);






    }

    /// <summary>Set the color of the graph line</summary>
    public void SetLineColor() {
        uiLineRenderer.color = color;
        if (dotImage != null) {dotImage.color = color;}

    }

    /// <summary> Add a point at coordinates (x,y) </summary>
    public void AddPoint(float x, float y) {
        if (points.Count == 0 && (x != 0f && y != 0f)) {
            AddPoint(0f, 0f);
        }
        // Ensure only one point exists for each x
        if (points.Find(item => item.x == x) != null) {
            points.Add(new Vector2(x, y));
        }

        Debug.Log("ADDING POINT " + x.ToString() + "," + y.ToString());
        //ResetLineRenderer();
        
    }

    public void OnMouseOver() {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.main, out mousePos);

        if (rectTransform.rect.Contains(mousePos)) {

            float xPercent = (mousePos.x + (rectTransform.pivot.x * rectTransform.rect.width)) / rectTransform.rect.width;

            float xHover = (xPercent * GetMaxValue(Axis.X).x);
            dotImage.rectTransform.localScale = Vector2.one;
            if (scrolleyThing != null) {scrolleyThing.localScale = Vector3.one;}
            dotImage.rectTransform.anchoredPosition = ScalePoint(GetInterpolatedValue(xHover)); //ScalePoint(ValueAtX(xHover));
            //Debug.Log(xHover.ToString());
            if (scrolleyThing != null) {scrolleyThing.anchoredPosition = ScalePoint(GetInterpolatedValue(xHover)) + new Vector2(0f, 3f);}
        }

        else {
            dotImage.rectTransform.localScale = Vector2.zero;
            if (scrolleyThing != null) {scrolleyThing.localScale = Vector3.zero;}
        }
    }

    /// <summary>Get the interpolated Vector2 at a given point</summary>
    public Vector2 GetInterpolatedValue(float xPosition) {
        // find the nearest two defined x points

        int lesserXGuess = (int)Mathf.Floor(xPosition);
        while (!HasValueAtX(lesserXGuess)) {
            lesserXGuess--;
        }
        Vector2 lesserPoint = ValueAtX(lesserXGuess);

        int greaterXGuess = (int)Mathf.Ceil(xPosition);
        while (!HasValueAtX(greaterXGuess)) {
            greaterXGuess++;
        }
        Vector2 greaterPoint = ValueAtX(greaterXGuess);

        // find the ratio we're between them
        float lerpAmt = Mathf.InverseLerp(lesserPoint.x, greaterPoint.x, xPosition);

        // lerp it boi
        Vector2 lerpedVec = Vector2.Lerp(lesserPoint, greaterPoint, lerpAmt);
        return lerpedVec;
    }

    /// <summary>Returns True if there is a point at the specified X value.</summary>
    public bool HasValueAtX(float val) {
        foreach (Vector2 pt in GetPointList()) {
            if (pt.x == val) {
                return true;
            }
        }
        return false;
    }

    /// <summary>Returns the point with a given X value.</summary>
    public Vector2 ValueAtX(float val) {
        foreach (Vector2 pt in points) {
            if (pt.x == val) {
                return pt;
            }
        }
        return Vector2.zero;
    }

    public Vector2 GetMaxValue(Axis axis, bool scaled = false) {
        Vector2 biggestPoint = Vector2.negativeInfinity;

        if (scaled) {
            return rectTransform.rect.max;
        }

        foreach (Vector2 pt in GetPointList()) {
            if (axis == Axis.X) {if (pt.x > biggestPoint.x) {biggestPoint = pt;}}
            else if (axis == Axis.Y) {if (pt.y > biggestPoint.y) {biggestPoint = pt;}}
        }


        if (biggestPoint == Vector2.negativeInfinity) {return Vector2.one;}
        //Debug.Log("Biggest value on " + axis.ToString() + " is " + biggestPoint.ToString());
        return biggestPoint;
    }

    public Vector2 GetMinValue(Axis axis, bool scaled = false) {
        Vector2 smallestPoint = Vector2.positiveInfinity;

        if (scaled) {
            return rectTransform.rect.min;
        }


        foreach (Vector2 pt in GetPointList()) {
            if (axis == Axis.X) {if (pt.x < smallestPoint.x) {smallestPoint = pt;}}
            else if (axis == Axis.Y) {if (pt.y < smallestPoint.y) {smallestPoint = pt;}}
        }



        if (smallestPoint == Vector2.positiveInfinity) {return Vector2.one;}
        return smallestPoint;
    }

    public List<Vector2> GetPointList() {
        List<Vector2> fullPoints = new List<Vector2>(points);
        if (maxValuesOnGraph > 0 && fullPoints.Count > maxValuesOnGraph) {
            int amountToTruncate = fullPoints.Count - maxValuesOnGraph;
            fullPoints.RemoveRange(0, amountToTruncate);
        }
        return fullPoints;
    }

    private void ResetLineRenderer() {
        if (points.Count > 0) {
            //uiLineRenderer.Points = ScaledPoints(useSimplification ? DouglasPeuckerLineSimplification(points.ToArray(), simplificationAmt) : points.ToArray());

            List<Vector2> simplifiedPointList = new List<Vector2>();



            
            LineUtility.Simplify(GetPointList(), simplificationAmt, simplifiedPointList);

            uiLineRenderer.Points = useSimplification ? simplifiedPointList.ToArray() : GetPointList().ToArray();

            if (scalePoints) {
                uiLineRenderer.Points = ScaledPoints(uiLineRenderer.Points);
            }
        }
    }


    public Vector2 GetTickInterval() {
        Debug.Log("GETTING TICK INTERVAL");

        if (points.Count == 0) {
            return new Vector2(5,5);
        }
        float xRange = Mathf.Ceil(GetMaxValue(Axis.X, false).x - GetMinValue(Axis.X, false).x);

        // We now have the range of X values. Let's find the CLOSEST number
        // to it that fits a good number (power of 1, 5, or 10).

        xSpacePerLine = CalcStepSize(xRange == 0 ? 1f : xRange, 16f);
        //Debug.Log("Step size is " + xSpacePerLine.ToString());

        // the number of steps would be 
    
        float xTicks = xRange / xSpacePerLine;

        //Debug.Log("Max Y is " + GetMaxValue(Axis.Y, false).y.ToString());

        float yRange = Mathf.Ceil(GetMaxValue(Axis.Y, false).y - GetMinValue(Axis.Y, false).y);
        ySpacePerLine = CalcStepSize(yRange == 0 ? 1f : yRange, 10f);
        float yTicks = yRange / ySpacePerLine;

        Debug.Log("Absolute max X is " + GetMaxValue(Axis.X, false).x.ToString() + ", upper bound is " + (xSpacePerLine * xTicks).ToString());
        Debug.Log("Absolute max Y is " + GetMaxValue(Axis.Y, false).y.ToString() + ", upper bound is " + (ySpacePerLine * yTicks).ToString());

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
    
    public float GetYStep() {
        // Return the world-space units that correspond to one graph-unit on the Y axis

        float percentAcross = 1 / (GetMaxValue(Axis.Y).y - GetMinValue(Axis.Y).y);
        return percentAcross * rectTransform.rect.height;
        //return Mathf.Abs(ScalePoint(new Vector2(0f, 1f)).y);
    }

    public float GetXStep() {
        // Return the world-space units that correspond to one graph-unit on the X axis
        float percentAcross = 1 / (GetMaxValue(Axis.X).x - GetMinValue(Axis.X).x);
        return percentAcross * rectTransform.rect.width;
    }
    public void RemovePoint(float x) {

    }

    public Vector3[] Vec2ArrayToVec3Array(Vector2[] val) {
        if (val == null) {return null;}
        Vector3[] outval = new Vector3[val.Length];

        for (int i = 0; i < val.Length; i++) {
            outval[i] = Vec2ToVec3(val[i]);
        }

        return outval;
    }

    public Vector2[] Vec3ArrayToVec2Array(Vector3[] val) {
        if (val == null) {return null;}
        Vector2[] outval = new Vector2[val.Length];

        for (int i = 0; i < val.Length; i++) {
            outval[i] = Vec3ToVec2(val[i]);
        }

        return outval;
    }

    public Vector3 Vec2ToVec3(Vector2 val) {
        return new Vector3(val.x, val.y, 0f);
    }

    public Vector2 Vec3ToVec2(Vector3 val) {
        return new Vector2(val.x, val.y);
    }


    public Vector2[] ScaledPoints(Vector2[] ptsIn) {
        if (ptsIn == null) {return null;}
        Vector2[] scaledPts = new Vector2[ptsIn.Length];
        
        for (int i = 0; i < ptsIn.Length; i++) {

            scaledPts[i] = ScalePoint(ptsIn[i]);
        }

        return scaledPts;


    }

    public Vector2 ScalePoint(Vector2 val) {
    
        float maxX = ((gridRenderer.GridColumns) * xSpacePerLine);
        float maxY = ((gridRenderer.GridRows) * ySpacePerLine);

        if (maxValuesOnGraph > 0 && points.Count > maxValuesOnGraph) {
            val -= GetPointList()[0];
        }

        return new Vector2(
            val.x / (maxX == 0 ? 1 : maxX),
            val.y / (maxY == 0 ? 1 : maxY)
        );

    }

    private Vector2[] DouglasPeuckerLineSimplification(Vector2[] inval, int epsilon) {
        int dmax = 0;
        int index = 0;
        int end = inval.Length;

        for (int i = 1; i < end; i++) {
            int d = GetPerpendicularDistance(inval[i], inval[0], inval[end - 1]);
            if (d > dmax) {
                index = i;
                dmax = d;
            }
        }

        List<Vector2> result = new List<Vector2>();

        if (dmax > epsilon) {

            Vector2[] in_zero_to_index = inval.Take(index).ToArray();
            Vector2[] in_index_to_end = inval.Skip(index).ToArray();
            Vector2[] recursiveOne = DouglasPeuckerLineSimplification(in_zero_to_index, epsilon);
            Vector2[] recursiveTwo = DouglasPeuckerLineSimplification(in_index_to_end, epsilon);

            // some line I don't understand should go here
            // ResultList[] = {recursiveOne[1...recursiveLength.Length-1], recursiveTwo[1...length(recursiveTwo)]}

            for (int i = 0; i < recursiveOne.Length; i++) {
                result.Add(recursiveOne[i]);
            }

            for (int i = 0; i < recursiveTwo.Length; i++) {
                result.Add(recursiveTwo[i]);
            }
            
        }
        else {
            //Debug.Log("The length of inval is " + inval.Length.ToString());
            //Debug.Log(inval[inval.Length - 1]);
            result = new List<Vector2> {inval[0], inval[inval.Length - 1]};

        }
        return result.ToArray();
    }

    public int GetPerpendicularDistance(Vector2 inpt, Vector2 line_pt1, Vector2 line_pt2) {
        float numerator = Mathf.Abs(
            (line_pt2.y - line_pt1.y) * inpt.x - (line_pt2.x - line_pt1.x) * inpt.y + line_pt2.x * line_pt1.y - line_pt2.y * line_pt1.x
        );

        float denominator = Mathf.Sqrt(
            Mathf.Pow(line_pt2.y - line_pt1.y, 2f) + Mathf.Pow(line_pt2.x - line_pt1.x, 2f)
        );

        return (int)(numerator / denominator);
    }

}
