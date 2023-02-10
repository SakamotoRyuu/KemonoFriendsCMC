using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScaleForCapsuleCollider : MonoBehaviour {

    public float delay = 0;
    public float time = 1;
    public float startRadius = 0.5f;
    public float endRadius = 2f;
    public float heightMul = 2f;
    public bool completeAndDestroy;
    
    float duration;
    bool complete;
    CapsuleCollider capCol;

    void Awake() {
        capCol = GetComponent<CapsuleCollider>();
        if (capCol) {
            capCol.radius = startRadius;
            capCol.height = startRadius * heightMul;
        }
    }
    
    void Update() {
        if (!complete && capCol) {
            duration += Time.deltaTime;
            if (duration >= delay) {
                if (duration >= delay + time) {
                    capCol.radius = endRadius;
                    capCol.height = endRadius * heightMul;
                    complete = true;
                    if (completeAndDestroy) {
                        Destroy(gameObject);
                    }
                } else if (time > 0) {
                    float radiusTemp = startRadius + (endRadius - startRadius) * (duration - delay) / time;
                    capCol.radius = radiusTemp;
                    capCol.height = radiusTemp * heightMul;
                }
            }
        }
    }
}
