using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOperationCanvas_Scaling : UIOperationCanvas
{
    public Transform pivot;
    public RectTransform rect;
    public float baseScale = 0.25f;
    public float scaleAdjustMax = 1f;
    public float scaleAdjustMin = 0.5f;

    float scaleSave = -1f;
    const float distanceMin = 1f;
    const float distanceMax = 50f;

    protected override void Update() {
        base.Update();
        if (camT) {
            float distTemp = Mathf.Clamp(Vector3.Distance(camT.position, pivot.position), distanceMin, distanceMax);
            if (distTemp <= distanceMin && canvas.enabled) {
                scaleSave = -1f;
                canvas.enabled = false;
            } else {
                float scaleTemp = distTemp * Mathf.Lerp(scaleAdjustMax, scaleAdjustMin, (distTemp - distanceMin) / (distanceMax - distanceMin)) * baseScale;
                if (scaleTemp != scaleSave) {
                    rect.localScale = Vector3.one * scaleTemp;
                    scaleSave = scaleTemp;
                }
                if (!canvas.enabled) {
                    canvas.enabled = true;
                }
            }
        }
    }

}
