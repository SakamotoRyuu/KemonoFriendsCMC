using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPeriodicMovement : MonoBehaviour
{

    public Vector3 minLocalPos;
    public Vector3 maxLocalPos;
    public float cycleTime;
    public float cycleOffset;

    float duration;
    Vector3 vecTemp = Vector3.zero;
    Transform trans;

    private void Start() {
        duration = cycleOffset;
        trans = transform;
    }

    private void Update() {
        duration += Time.deltaTime;
        if (duration >= cycleTime) {
            duration -= cycleTime;
        }
        float cyclePoint = Mathf.Clamp01((Mathf.Sin(Mathf.PI * 2f * (duration / cycleTime)) + 1f) * 0.5f);
        if (minLocalPos.x == maxLocalPos.x) {
            vecTemp.x = minLocalPos.x;
        } else {
            vecTemp.x = Mathf.Lerp(minLocalPos.x, maxLocalPos.x, cyclePoint);
        }
        if (minLocalPos.y == maxLocalPos.y) {
            vecTemp.y = minLocalPos.y;
        } else {
            vecTemp.y = Mathf.Lerp(minLocalPos.y, maxLocalPos.y, cyclePoint);
        }
        if (minLocalPos.z == maxLocalPos.z) {
            vecTemp.z = minLocalPos.z;
        } else {
            vecTemp.z = Mathf.Lerp(minLocalPos.z, maxLocalPos.z, cyclePoint);
        }
        trans.localPosition = vecTemp;
    }

}
