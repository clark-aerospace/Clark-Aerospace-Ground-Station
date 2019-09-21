using UnityEngine.UI;
using UnityEngine;

public static class NiceExtensions {
    public static void SetAlpha(this Image image, float amount) {
        Color c = image.color;
        c.a = amount;
        image.color = c;
        return;
    }
}