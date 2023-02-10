using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetectionKnockAssort : DamageDetection {

    public bool projectileTolerance;
    protected float referenceValue;

    protected override void Start() {
        base.Start();
        if (parentCBase != null) {
            referenceValue = parentCBase.knockEndurance;
        }
    }

    public override bool ReceiveDamage(ref Vector3 effectPosition, int damage, float knockAmount, ref Vector3 knockVector, AttackDetection attackDetection = null, bool penetrate = false, int overrideColorType = -1) {
        if (projectileTolerance && damage >= 1 && attackDetection && attackDetection.isProjectile) {
            damage = Mathf.Max(damage / 2, 1);
        }
        if (knockAmount < referenceValue * 0.5f) {
            hitEffectNum = 0;
        } else if (knockAmount < referenceValue) {
            hitEffectNum = 1;
        } else {
            hitEffectNum = 2;
        }
        return base.ReceiveDamage(ref effectPosition, damage, knockAmount, ref knockVector, attackDetection, penetrate, overrideColorType);
    }
}
