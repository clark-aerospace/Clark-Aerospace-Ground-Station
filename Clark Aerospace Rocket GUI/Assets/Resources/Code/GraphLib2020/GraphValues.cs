using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI.Extensions;

namespace GraphLib
{
    [System.Serializable]
    public class DataSet {
        public Color mainColor = Color.white;
        public Color secondaryColor = Color.white;
        public List<Vector2> values = new List<Vector2>();

        public bool useBezier = false;

        public UILineRenderer lineRenderer;
        //public List<Image> dots = new List<Image>();


        // For use with animating n stuff
        [HideInInspector]
        public Vector2 valueToAdd;
        public float interpolateAmt;
        public bool currentlyInterpolating = false;
    }
}