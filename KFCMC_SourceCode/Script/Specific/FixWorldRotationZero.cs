using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixWorldRotationZero : MonoBehaviour {

    public bool fixX;
    public bool fixY;
    public bool fixZ;

    void LateUpdate() {
        Vector3 vecTemp = transform.eulerAngles;
        if (fixX) {
            vecTemp.x = 0f;
        }
        if (fixY) {
            vecTemp.y = 0f;
        }
        if (fixZ) {
            vecTemp.z = 0f;
        }
        transform.eulerAngles = vecTemp;
    }

}
