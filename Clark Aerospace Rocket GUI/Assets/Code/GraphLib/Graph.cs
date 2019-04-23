using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Graph : MonoBehaviour
{

    public enum Axis {
        X = 0,
        Y = 1
    }
    private List<Vector2> points = new List<Vector2>();
    
    public LineRenderer lineRenderer;
    public LineRenderer gridLineRenderer;

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
    float time = 4;

    int thing = 0;


    public void Start() {
        lineRenderer = new GameObject().AddComponent<LineRenderer>();
        lineRenderer.transform.SetParent(transform, false);

        lineRenderer.numCornerVertices = 8;
        lineRenderer.numCapVertices = 8;
        lineRenderer.SetWidth(3f, 3f);
        lineRenderer.alignment = LineAlignment.TransformZ;

        lineRenderer.useWorldSpace = false;

        gridLineRenderer = new GameObject().AddComponent<LineRenderer>();
        gridLineRenderer.transform.SetParent(transform, false);
        gridLineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        gridLineRenderer.material.color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        gridLineRenderer.transform.localPosition = new Vector3(0f, 0f, 1f);
        //vertLinesRenderer.numCornerVertices = 8;
        //vertLinesRenderer.numCapVertices = 8;
        //vertLinesRenderer.SetWidth(3f, 3f);
        gridLineRenderer.alignment = LineAlignment.TransformZ;

        gridLineRenderer.useWorldSpace = false;

        rectTransform = GetComponent<RectTransform>();


        dotImage = new GameObject().AddComponent<Image>();
        dotImage.transform.SetParent(transform, false);
        dotImage.sprite = dotSprite;
        dotImage.color = color;
        dotImage.useSpriteMesh = true;
        dotImage.rectTransform.sizeDelta = new Vector2(10,10);

        mFilter = new GameObject().AddComponent<MeshFilter>();
        mFilter.transform.SetParent(transform, false);
        mRenderer = mFilter.gameObject.AddComponent<MeshRenderer>();

        mesh = new Mesh();
        mFilter.mesh = mesh;


        AddPoint(0, 0);
        AddPoint(1, 2);
        AddPoint(2, 3);
        AddPoint(3, 5);
        // AddPoint(4, 6);
        // AddPoint(5, 7);
        // AddPoint(6, 8);
        // AddPoint(7, 10);
        // AddPoint(8, 12);
        // AddPoint(9, 13);
        // AddPoint(10, 15);
        // AddPoint(11, 17);
        // AddPoint(12, 19);
        // AddPoint(13, 20);
        // AddPoint(14, 21);
        // AddPoint(15, 24);
        // AddPoint(16, 27);
        // AddPoint(17, 30);
        // AddPoint(18, 34);

        //SetLineColor();
        
        GenerateMesh();
        


        
        
    }

    public void Update() {
        if (thing >= 40) {AddTestPoint();}
        thing++;

        ResetLineRenderer();
        SetLineColor();
        OnMouseOver();
        GenerateMesh();
        DrawGridLines();
    }

    public void AddTestPoint() {
        currentYPt = currentYPt + Random.Range(-1f, 2f);
        AddPoint(time, currentYPt);
        time++;
        thing = 0;
    }



    public void GenerateMesh() {

        Vector3[] _pts = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(_pts);

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
        mRenderer.transform.localPosition = new Vector3(0f, 0f, 0.5f);






    }

    public void SetLineColor() {
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = color;
        dotImage.color = color;

    }

    public void AddPoint(float x, float y) {

        // Ensure only one point exists for each x
        if (points.Find(item => item.x == x) != null) {
            points.Add(new Vector2(x, y));
        }

        //ResetLineRenderer();
        
    }

    public void OnMouseOver() {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.main, out mousePos);
        if (rectTransform.rect.Contains(mousePos)) {

            float xPercent = (mousePos.x + (rectTransform.pivot.x * rectTransform.rect.width)) / rectTransform.rect.width;

            float xHover = (xPercent * GetMaxValue(Axis.X).x);
            dotImage.rectTransform.anchoredPosition = ScalePoint(GetInterpolatedValue(xHover)); //ScalePoint(ValueAtX(xHover));
            Debug.Log(xHover.ToString());
        }
    }

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

    public bool HasValueAtX(float val) {
        foreach (Vector2 pt in points) {
            if (pt.x == val) {
                return true;
            }
        }
        return false;
    }

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
            //biggestPoint = ScalePoint(biggestPoint);
            return rectTransform.rect.max;
        }

        foreach (Vector2 pt in points) {
            if (axis == Axis.X) {if (pt.x > biggestPoint.x) {biggestPoint = pt;}}
            else if (axis == Axis.Y) {if (pt.y > biggestPoint.y) {biggestPoint = pt;}}
        }


        if (biggestPoint == Vector2.negativeInfinity) {return Vector2.one;}
        return biggestPoint;
    }

    public Vector2 GetMinValue(Axis axis, bool scaled = false) {
        Vector2 smallestPoint = Vector2.positiveInfinity;
        if (scaled) {
            //biggestPoint = ScalePoint(biggestPoint);
            return rectTransform.rect.min;
        }

        foreach (Vector2 pt in points) {
            if (axis == Axis.X) {if (pt.x < smallestPoint.x) {smallestPoint = pt;}}
            else if (axis == Axis.Y) {if (pt.y < smallestPoint.y) {smallestPoint = pt;}}
        }



        if (smallestPoint == Vector2.positiveInfinity) {return Vector2.one;}
        return smallestPoint;
    }

    private void ResetLineRenderer() {

        for (int i = 0; i < lineRenderer.positionCount; i++) {
            lineRenderer.SetPosition(i, Vector3.zero);
        }
        lineRenderer.positionCount = points.Count;
        
        Vector3[] array = Vec2ArrayToVec3Array(ScaledPoints(points.ToArray()));

        lineRenderer.SetPositions(array);

        lineRenderer.Simplify((GetMaxValue(Axis.Y).y - GetMinValue(Axis.Y).y) * 0.01f);
        

    }

    public void DrawGridLines() {
        // The line needs to start at the TOP LEFT corner (min x, max y) and go to
        // the TOP RIGHT corner (max x, max y). THEN, move one unit down (max x, max y - 1)
        // and then back to (min x, max y - 1)

        Vector3 startPos = new Vector3(GetMinValue(Axis.X, true).x, GetMaxValue(Axis.Y, true).y, 0f);



        Vector3 endPos = new Vector3(GetMaxValue(Axis.X, true).x, GetMaxValue(Axis.Y, true).y, 0f);



        int numberOfVertsY = ((int)(GetMaxValue(Axis.Y).y) * 2) + 2;

        int numberOfVertsX = ((int)(GetMaxValue(Axis.X).x) * 2) + 1;

        int numberOfVerts = numberOfVertsY + numberOfVertsX;


        gridLineRenderer.positionCount = numberOfVerts;
        //print(numberOfVerts);
        Vector3[] pos = new Vector3[numberOfVerts];
        pos[0] = startPos;

        Debug.Log(ScalePoint(Vector2.one));

        float yUnitAmt = GetYStep();
        float xUnitAmt = GetXStep();
        Debug.Log("Y unit amt: " + yUnitAmt.ToString());

        int status = 0;
        // 0 = moving right
        // 1 = moving down after having moved right
        // 2 = moving left
        // 3 = moving down after having moved left

        Vector3 currentPos = new Vector3(GetMinValue(Axis.X, true).x, GetMaxValue(Axis.Y, true).y, 0f);

        for (int i = 1; i < numberOfVertsY; i++) {
            //Debug.Log("loop iteration " + i.ToString());
            if (status == 0) {
                //Debug.Log("moving right");
                currentPos.x = GetMaxValue(Axis.X, true).x;
                status = 1;
            }

            else if (status == 1) {
                //Debug.Log("moving down");
                currentPos.y -= yUnitAmt;
                status = 2;
            }

            else if (status == 2) {
                //Debug.Log("moving left");
                currentPos.x = GetMinValue(Axis.X, true).x;
                status = 3;
            } else if (status == 3) {
                //Debug.Log("moving down");
                currentPos.y -= yUnitAmt;
                status = 0;
            }

            pos[i] = currentPos;
        }

        // the Ys are taken care of, now for the Xs

        status = 0;
        // 0 = moving up
        // 1 = moving left after having moved up
        // 2 = moving down
        // 3 = moving left after having moved down

        for (int i = numberOfVertsY; i < numberOfVerts; i++) {
            if (status == 0) {
                currentPos.y = GetMaxValue(Axis.Y, true).y;
                status = 1;
            }

            else if (status == 1) {
                currentPos.x -= xUnitAmt;
                status = 2;
            }

            else if (status == 2) {
                currentPos.y = GetMinValue(Axis.Y, true).y;
                status = 3;
            }

            else if (status == 3) {
                currentPos.x -= xUnitAmt;
                status = 0;
            }

            pos[i] = currentPos;
        }

        gridLineRenderer.SetPositions(pos);
        gridLineRenderer.startWidth = 1f;
        gridLineRenderer.endWidth = 1f;



        // gridLineRenderer.startColor = new Color(0.3f, 0.3f, 0.3f, 0.7f);
        // gridLineRenderer.endColor = new Color(0.3f, 0.3f, 0.3f, 0.7f);

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
        Vector3[] outval = new Vector3[val.Length];

        for (int i = 0; i < val.Length; i++) {
            outval[i] = Vec2ToVec3(val[i]);
        }

        return outval;
    }

    public Vector2[] Vec3ArrayToVec2Array(Vector3[] val) {
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

        Vector2[] scaledPts = new Vector2[ptsIn.Length];
        
        for (int i = 0; i < ptsIn.Length; i++) {

            scaledPts[i] = ScalePoint(ptsIn[i]);
        }

        return scaledPts;


    }

    public Vector2 ScalePoint(Vector2 val) {
        return new Vector2(((val.x / GetMaxValue(Axis.X).x) * rectTransform.rect.width) - (rectTransform.pivot.x * rectTransform.rect.width), ((val.y / GetMaxValue(Axis.Y).y) * rectTransform.rect.height) - (rectTransform.pivot.y * rectTransform.rect.height));
    }

}
