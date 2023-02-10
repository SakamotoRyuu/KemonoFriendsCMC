using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanicRock_CheckVelocityAttackEnd : MonoBehaviour {

    public Rigidbody checkTargetRigidbody;
    public AttackDetection targetAttackDetection;
    public float checkStartDelay = 0.5f;

    int state;
    float elapsedTime;

    void FixedUpdate() {
        elapsedTime += Time.fixedDeltaTime;
        if (checkTargetRigidbody && state < 100) {
            if (state == 0 && checkTargetRigidbody.velocity.y > 0f) {
                state = 1;
            } else if (state == 1 && checkTargetRigidbody.velocity.y < 0f) {
                state = 2;
            } else if (state == 2 && elapsedTime >= checkStartDelay && checkTargetRigidbody.velocity.y >= 0f) {
                state = 3;
            } else if (state == 3) {
                if (targetAttackDetection && targetAttackDetection.attackEnabled) {
                    targetAttackDetection.DetectionEnd();
                }
                state = 100;
            }
        }
    }
    
}
