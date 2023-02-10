using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRotation : MonoBehaviour {

    public bool freezeX;
    public bool freezeY;
    public bool freezeZ;
    Transform trans;
    Vector3 startRotation;

    void Awake() {
        trans = transform;
        startRotation = trans.eulerAngles;
    }

    private void Update() {
        Vector3 eulerTemp = trans.eulerAngles;
        if (freezeX) {
            eulerTemp.x = startRotation.x;
        }
        if (freezeY) {
            eulerTemp.y = startRotation.y;
        }
        if (freezeZ) {
            eulerTemp.z = startRotation.z;
        }
        trans.eulerAngles = eulerTemp;
    }
}
