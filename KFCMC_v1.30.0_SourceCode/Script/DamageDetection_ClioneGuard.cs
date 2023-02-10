using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetection_ClioneGuard : DamageDetection
{

    public AttackDetection.ElementalAttribute weakAttribute;
    public int normalHitIndex = 0;
    public int weakHitIndex = 1;
    Enemy_Clione parentClione;

    protected override void Awake() {
        base.Awake();
        parentClione = GetComponentInParent<Enemy_Clione>();
    }

    public override bool ReceiveDamage(ref Vector3 effectPosition, int damage, float knockAmount, ref Vector3 knockVector, AttackDetection attackDetection = null, bool penetrate = false, int overrideColorType = -1) {
        if (parentCBase && parentCBase.enabled && parentCBase.GetCanTakeDamage(penetrate)) {
            float knockTemp = knockAmount;
            if (attackDetection && attackDetection.elementalAttribute == weakAttribute) {
                hitEffectNum = weakHitIndex;
                overrideColorType = 3;
                knockTemp *= 4;
            } else {
                hitEffectNum = normalHitIndex;
            }
            parentClione.ReceiveGuardKnock(knockTemp);
        } else {
            hitEffectNum = normalHitIndex;
        }
        return base.ReceiveDamage(ref effectPosition, damage, knockAmount, ref knockVector, attackDetection, penetrate, overrideColorType);
    }

}
