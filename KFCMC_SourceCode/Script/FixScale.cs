using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixScale : MonoBehaviour {

    Vector3 defaultScale;
    Transform trans;
    Vector3 lossyScale;
    Vector3 localScale;
    Vector3 scaleTemp;
    bool activated;

    void Awake() {
        if (!activated) {
            trans = transform;
            defaultScale = trans.lossyScale;
            activated = true;
        }
    }

    public void ForceAwake() {
        if (!activated) {
            trans = transform;
            defaultScale = trans.lossyScale;
            activated = true;
        }
    }

    void Update() {
        lossyScale = trans.lossyScale;
        localScale = trans.localScale;
        scaleTemp.x = localScale.x / lossyScale.x * defaultScale.x;
        scaleTemp.y = localScale.y / lossyScale.y * defaultScale.y;
        scaleTemp.z = localScale.z / lossyScale.z * defaultScale.z;
        if (trans.localScale != scaleTemp) {
            trans.localScale = scaleTemp;
        }
    }
}
