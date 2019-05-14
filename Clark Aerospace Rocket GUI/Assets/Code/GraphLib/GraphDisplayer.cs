using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GraphDisplayer : MonoBehaviour
{

    public RectTransform parentRectTransform;
    public void ToggleGraphVisible() {
        gameObject.active = !gameObject.active;
        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRectTransform);
    }
}
