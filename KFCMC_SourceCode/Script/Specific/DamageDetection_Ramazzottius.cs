using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetection_Ramazzottius : DamageDetection {

    GameObject parentObj;
    Enemy_Ramazzottius parentRamazzottius;

    protected override void Awake() {
        base.Awake();
        parentRamazzottius = GetComponentInParent<Enemy_Ramazzottius>();
        parentObj = transform.parent.gameObject;
    }

    public override bool ReceiveDamage(ref Vector3 effectPosition, int damage, float knockAmount, ref Vector3 knockVector, AttackDetection attackDetection = null, bool penetrate = false, int overrideColorType = -1) {
        if (parentRamazzottius) {
            parentRamazzottius.GetCritical(parentObj);
        }
        return base.ReceiveDamage(ref effectPosition, damage, knockAmount, ref knockVector, attackDetection, penetrate, overrideColorType);
    }

}
