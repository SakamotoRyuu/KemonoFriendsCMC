using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetUI : MonoBehaviour {

    public Color[] imageColor;
    public Image image;
    public RectTransform rectTransform;
    private int cursorSize = -1;
    private static readonly Vector2 baseSize = new Vector2(50f, 50f);
    private static readonly float[] sizeMultiplier = new float[12] { 1f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f };

    public void SetCursor(Vector2 position, int size = 6) {
        rectTransform.position = position;
        if (size != cursorSize) {
            cursorSize = size;
            rectTransform.sizeDelta = baseSize * sizeMultiplier[Mathf.Clamp(size, 0, sizeMultiplier.Length - 1)];
        }
    }

    public void SetColor(int num) {
        image.color = imageColor[num];
    }
}
