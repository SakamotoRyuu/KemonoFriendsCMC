using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityToAttackRate : MonoBehaviour {

    public Rigidbody targetRigidbody;
    public AttackDetection targetAttackDetection;
    public float attackRate;
    public float knockRate;
    public float attackMin;
    public float attackMax;
    public float knockMin;
    public float knockMax;
    
    void Update() {
        if (targetRigidbody && targetAttackDetection) {
            float sqrVel = targetRigidbody.velocity.sqrMagnitude;
            targetAttackDetection.attackRate = Mathf.Clamp(sqrVel * attackRate, attackMin, attackMax);
            targetAttackDetection.knockRate = Mathf.Clamp(sqrVel * knockRate, knockMin, attackMax);
        }
    }
}
