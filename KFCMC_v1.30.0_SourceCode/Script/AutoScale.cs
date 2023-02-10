using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScale : MonoBehaviour {

    public float delay = 0;
    public float time = 1;
    public Vector3 endScale;
    public bool completeAndDestroy;

    Transform trans;
    Vector3 startScale;
    bool gotStartScale;
    float duration;
    bool complete;

    void Start() {
        trans = transform;
    }

    // Update is called once per frame
    void Update() {
        if (!complete) {
            duration += Time.deltaTime;
            if (duration >= delay) {
                if (!gotStartScale) {
                    startScale = trans.localScale;
                    gotStartScale = true;
                }
                if (duration >= delay + time) {
                    trans.localScale = endScale;
                    complete = true;
                    if (completeAndDestroy) {
                        Destroy(gameObject);
                    }
                } else if (time > 0) {
                    trans.localScale = startScale + (endScale - startScale) * (duration - delay) / time;
                }
            }
        }
    }
}
