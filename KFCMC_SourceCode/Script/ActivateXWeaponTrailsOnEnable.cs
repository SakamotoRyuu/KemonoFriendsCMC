using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class ActivateXWeaponTrailsOnEnable : MonoBehaviour {

    public float stopSmoothTime;
    XWeaponTrail xwt;

    private void Awake() {
        xwt = GetComponent<XWeaponTrail>();
        if (xwt) {
            xwt.Init();
        }
    }

    private void OnEnable() {
        if (xwt) {
            xwt.Activate();
        }
    }

    private void OnDisable() {
        if (xwt) {
            if (stopSmoothTime <= 0f) {
                xwt.Deactivate();
            } else {
                xwt.StopSmoothly(stopSmoothTime);
            }
        }
    }
}
