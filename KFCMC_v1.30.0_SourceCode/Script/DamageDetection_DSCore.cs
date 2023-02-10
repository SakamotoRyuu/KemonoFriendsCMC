using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetection_DSCore : DamageDetection {

    public override bool ReceiveDamage(ref Vector3 effectPosition, int damage, float knockAmount, ref Vector3 knockVector, AttackDetection attackDetection = null, bool penetrate = false, int overrideColorType = -1) {
        if (attackDetection != null && attackDetection.parentCBase != null && attackDetection.parentCBase.isPlayer) {
            if (!attackDetection.isProjectile && attackDetection.parentCBase.isSuperman) {
                knockAmount *= 8f;
            } else {
                knockAmount *= 2f;
            }
        }
        return base.ReceiveDamage(ref effectPosition, damage, knockAmount, ref knockVector, attackDetection, penetrate, overrideColorType);
    }

}
