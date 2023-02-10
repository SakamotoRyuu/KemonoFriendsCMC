using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetectionElemental : DamageDetection
{

    public AttackDetection.ElementalAttribute toleranceAttribute;
    public int normalHitIndex = 0;
    public int toleranceHitIndex = 1;
    public float toleranceRate = 0.5f;

    public override bool ReceiveDamage(ref Vector3 effectPosition, int damage, float knockAmount, ref Vector3 knockVector, AttackDetection attackDetection = null, bool penetrate = false, int overrideColorType = -1) {
        if (attackDetection && attackDetection.elementalAttribute == toleranceAttribute) {
            damage = Mathf.Max(1, (int)(damage * toleranceRate));
            knockAmount = knockAmount * toleranceRate;
            hitEffectNum = toleranceHitIndex;
        } else {
            hitEffectNum = normalHitIndex;
        }
        return base.ReceiveDamage(ref effectPosition, damage, knockAmount, ref knockVector, attackDetection, penetrate, overrideColorType);
    }


}
