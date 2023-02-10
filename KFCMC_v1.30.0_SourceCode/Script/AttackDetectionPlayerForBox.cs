using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetectionPlayerForBox : AttackDetectionPlayer {

    public Vector3 boxLongCenter;
    public Vector3 boxLongSize;

    Vector3 boxDefaultCenter;
    Vector3 boxDefaultSize;
    BoxCollider selfBoxCol;

    protected override void SetColliderSize(bool toLong) {
        if (selfBoxCol) {
            if (toLong) {
                selfBoxCol.center = boxLongCenter;
                selfBoxCol.size = boxLongSize;
                selectTopDamageRate = true;
            } else {
                selfBoxCol.center = boxDefaultCenter;
                selfBoxCol.size = boxDefaultSize;
                selectTopDamageRate = defSelect;
            }
        }
    }

    protected override void Start() {
        base.Start();
        selfBoxCol = GetComponent<BoxCollider>();
        if (selfBoxCol) {
            boxDefaultCenter = selfBoxCol.center;
            boxDefaultSize = selfBoxCol.size;
        }
    }

}
