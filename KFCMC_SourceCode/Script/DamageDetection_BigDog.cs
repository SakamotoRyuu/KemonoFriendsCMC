using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetection_BigDog : DamageDetection
{

    public int footNumber;
    Enemy_BigDog parentBigDog;

    protected override void Awake() {
        base.Awake();
        parentBigDog = GetComponentInParent<Enemy_BigDog>();
    }

    public override bool ReceiveDamage(ref Vector3 effectPosition, int damage, float knockAmount, ref Vector3 knockVector, AttackDetection attackDetection = null, bool penetrate = false, int overrideColorType = -1) {
        if (parentCBase && parentCBase.enabled && parentCBase.GetCanTakeDamage(penetrate)) {
            parentBigDog.ReceiveScarDamage(footNumber, damage);
        }
        return base.ReceiveDamage(ref effectPosition, damage, knockAmount, ref knockVector, attackDetection, penetrate, overrideColorType);
    }

}
