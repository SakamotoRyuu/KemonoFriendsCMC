using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetectionParticleImmediateHit : AttackDetectionParticle {

    public bool destroyOnHit;    

    protected override void Start() {
        base.Start();
        ParticlesPlay();
        enabled = false;
    }

    protected override void Update() {
    }

    public override void WorkEnter(GameObject other) {
        if (other.CompareTag(targetTag)) {
            targetDD = other.GetComponent<DamageDetection>();
            if (targetDD) {
                HitAttack(ref targetDD);
                if (destroyOnHit) {
                    Destroy(gameObject);
                }
            }
        }
    }

}
