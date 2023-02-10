using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScaleForLineRenderer : MonoBehaviour {

    public float delay = 0;
    public float time = 1;
    public float startWidthMultiplier;
    public float endWidthMultiplier;
    public bool completeAndDestroy;

    LineRenderer lineRenderer;
    float duration;
    bool complete;

    void Start() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update() {
        if (!complete && lineRenderer) {
            duration += Time.deltaTime;
            if (duration >= delay) {
                if (duration >= delay + time) {
                    lineRenderer.widthMultiplier = endWidthMultiplier;
                    complete = true;
                    if (completeAndDestroy) {
                        Destroy(gameObject);
                    }
                } else if (time > 0) {
                    lineRenderer.widthMultiplier = startWidthMultiplier + (endWidthMultiplier - startWidthMultiplier) * (duration - delay) / time;
                }
            }
        }
    }
}
