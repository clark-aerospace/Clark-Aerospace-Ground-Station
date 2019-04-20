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
    public LineRenderer vertLinesRenderer;

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


    public void Start() {
        lineRenderer = new GameObject().AddComponent<LineRenderer>();
        lineRenderer.transform.SetParent(transform, false);

        lineRenderer.numCornerVertices = 8;
        lineRenderer.numCapVertices = 8;
        lineRenderer.SetWidth(3f, 3f);
        lineRenderer.alignment = LineAlignment.TransformZ;

        lineRenderer.useWorldSpace = false;

        vertLinesRenderer = new GameObject().AddComponent<LineRenderer>();
        vertLinesRenderer.transform.SetParent(transform, false);
        //vertLinesRenderer.numCornerVertices = 8;
        //vertLinesRenderer.numCapVertices = 8;
        //vertLinesRenderer.SetWidth(3f, 3f);
        vertLinesRenderer.alignment = LineAlignment.TransformZ;

        vertLinesRenderer.useWorldSpace = false;

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
        AddPoint(4, 6);
        AddPoint(5, 7);
        AddPoint(6, 8);
        AddPoint(7, 10);
        AddPoint(8, 12);
        AddPoint(9, 13);
        AddPoint(10, 15);
        AddPoint(11, 17);
        AddPoint(12, 19);
        AddPoint(13, 20);
        AddPoint(14, 21);
        AddPoint(15, 24);
        AddPoint(16, 27);
        AddPoint(17, 30);
        AddPoint(18, 34);

        //SetLineColor();
        GenerateMesh();
        //DrawGridLines();


        
        
    }

    public void Update() {
        ResetLineRenderer();
        SetLineColor();
        OnMouseOver();
        GenerateMesh();
    }



    public void GenerateMesh() {
        Color colTop = color * 0.7f;
        colTop.a = 0.4f;

        Color colBottom = color * 0.7f;
        colBottom.a = 0f;


        verts = new Vector3[points.ToArray().Length * 2];
        Color[] colors = new Color[points.ToArray().Length * 2];
        tris = new int[(points.ToArray().Length - 1) * 6];

        for (int i = 0; i < points.ToArray().Length; i++) {
            verts[i] = ScalePoint(Vec2ArrayToVec3Array(points.ToArray())[i]);
            verts[i + points.ToArray().Length] = new Vector3(verts[i].x, -(rectTransform.rect.height * rectTransform.pivot.y), 0f);

            if (verts[i].y != 0) {
                colors[i] = colTop;
            } else {
                colors[i] = colBottom;
            }
        }
        mesh.vertices = verts;
        for (int i = 0; i < points.ToArray().Length - 1; i++) {
            tris[i * 6] = i;
            tris[i * 6 + 1] = i + 1;
            tris[i * 6 + 2] = i + points.ToArray().Length;
            tris[i * 6 + 3] = i + 1;
            tris[i * 6 + 4] = i + points.ToArray().Length + 1;
            tris[i * 6 + 5] = i + points.ToArray().Length;
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

    public Vector2 GetMaxValue(Axis axis) {
        Vector2 biggestPoint = Vector2.zero;

        foreach (Vector2 pt in points) {
            if (axis == Axis.X) {if (pt.x > biggestPoint.x) {biggestPoint = pt;}}
            else if (axis == Axis.Y) {if (pt.y > biggestPoint.y) {biggestPoint = pt;}}
        }
        return biggestPoint;
    }

    private void ResetLineRenderer() {

        for (int i = 0; i < lineRenderer.positionCount; i++) {
            lineRenderer.SetPosition(i, Vector3.zero);
        }
        lineRenderer.positionCount = points.Count;
        
        Vector3[] array = Vec2ArrayToVec3Array(ScaledPoints(points.ToArray()));

        lineRenderer.SetPositions(array);
        

    }

    public void DrawGridLines() {
        vertLinesRenderer.positionCount = ((int)GetMaxValue(Axis.X).x * 2) + 1;

        for (int i = 0; i < vertLinesRenderer.positionCount; i++) {
            vertLinesRenderer.SetPosition(i, Vector3.zero);
        }
        

        Debug.Log((int)GetMaxValue(Axis.X).x * 2);
        Vector3[] vertArray = new Vector3[((int)GetMaxValue(Axis.X).x * 2) + 1];

        int g = 0;
        for (int i = 0; g < GetMaxValue(Axis.X).x; i += 2) {
            vertArray[i] = Vec2ToVec3(new Vector2(g , 0f));
            vertArray[i+1] = Vec2ToVec3(new Vector2(g, GetMaxValue(Axis.Y).y));
            g++;
        }

        Debug.Log(vertArray.Length);

        foreach (Vector3 i in vertArray) {Debug.Log(i);}
        vertLinesRenderer.SetPositions(Vec2ArrayToVec3Array(ScaledPoints(Vec3ArrayToVec2Array(vertArray))));
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
