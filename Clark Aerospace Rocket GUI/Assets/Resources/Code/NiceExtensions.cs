using UnityEngine.UI;
using UnityEngine;

public static class NiceExtensions {
    public static void SetAlpha(this Image image, float amount) {
        Color c = image.color;
        c.a = amount;
        image.color = c;
        return;
    }

    public static void StretchToFill(this RectTransform r) {
        r.anchorMin = new Vector2(0,0);
        r.anchorMax = new Vector2(1,1);

        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
        return;
    }

    public static float InverseLerpUnclamped(float a, float b, float value) {
        return (value - a) / (b - a);
    }
}