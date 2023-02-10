using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCollisionLayer_ToDestroy : StopCollisionLayer {

    public float destroyTimer = 3;
    protected float remainTime = 0;

    protected void Update() {
        if (stopped) {
            remainTime -= Time.deltaTime;
            if (remainTime < 0) {
                Destroy(controlRigidbody.gameObject);
            }
        }
    }

    protected override void HitLayer(Transform other) {
        base.HitLayer(other);
        remainTime = destroyTimer;
    }

}
