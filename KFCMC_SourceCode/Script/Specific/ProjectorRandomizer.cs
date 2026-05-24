using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorRandomizer : MonoBehaviour {

    public Projector projector;
    public Vector2 orthographicSizeRange;
    public Vector2 aspectRatioRange;
    public Vector2 nearClipRange;
    public bool multiplyRoot;

    private void Awake() {
        if (projector) {
            float sizeTemp = Random.Range(orthographicSizeRange.x, orthographicSizeRange.y);
            float aspectTemp = Random.Range(aspectRatioRange.x, aspectRatioRange.y);
            float nearClipTemp = Random.Range(nearClipRange.x, nearClipRange.y);
            if (multiplyRoot && aspectTemp > 0f) {
                sizeTemp *= Mathf.Sqrt(1f / aspectTemp);
            }
            projector.aspectRatio = aspectTemp;
            projector.orthographicSize = sizeTemp;
            projector.nearClipPlane = nearClipTemp;
        }
    }

}
