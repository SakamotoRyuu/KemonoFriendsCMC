using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixEffectRotation : MonoBehaviour {

    Vector3 startRot;
    Transform trans;

    private void Awake() {
        trans = transform;
        startRot = trans.eulerAngles;
        startRot.x = 0f;
        startRot.z = 0f;
    }

    void LateUpdate() {
        trans.eulerAngles = startRot;
    }
}
