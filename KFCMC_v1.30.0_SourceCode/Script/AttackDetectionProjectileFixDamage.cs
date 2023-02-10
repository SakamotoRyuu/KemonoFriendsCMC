using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetectionProjectileFixDamage : AttackDetectionProjectile {

    public int fixDamage;
    public float fixKnock;
    public bool attackerIsNull;

    protected override bool SendDamage(ref DamageDetection damageDetection, ref Vector3 closestPoint, ref Vector3 direction) {
        return damageDetection.ReceiveDamage(ref closestPoint, fixDamage, fixKnock, ref direction, attackerIsNull ? null : mySelf, penetrate, overrideColorType);
    }

}
