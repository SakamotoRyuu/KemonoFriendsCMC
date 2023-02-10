using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetectionProjectileHitStop : AttackDetectionProjectile {
    
    public Rigidbody controlRigidbody;
    public bool hitStopParenting;
    public float hitStopDestroyTimer;

    protected float remainTime;
    protected bool stopped;

    protected override void Update() {
        base.Update();
        if (stopped) {
            remainTime -= Time.deltaTime;
            if (remainTime < 0) {
                Destroy(controlRigidbody.gameObject);
            }
        }
    }    

    protected override bool SendDamage(ref DamageDetection damageDetection, ref Vector3 closestPoint, ref Vector3 direction) {
        bool hitFlag = base.SendDamage(ref damageDetection, ref closestPoint, ref direction);
        if (hitFlag && !stopped) {
            stopped = true;
            DetectionEnd();
            if (controlRigidbody) {
                controlRigidbody.velocity = Vector3.zero;
                controlRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                controlRigidbody.isKinematic = true;
            }
            if (hitStopParenting && damageDetection.transform != null) {
                controlRigidbody.transform.SetParent(damageDetection.transform);
            }
            remainTime = hitStopDestroyTimer;
        }
        return hitFlag;
    }

}
