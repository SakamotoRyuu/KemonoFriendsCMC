using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetection_Imperatrix : DamageDetection {

    public int hyperColorType;
    public Collider checkerCollider;
    Enemy_ImperatrixMundi parentImperatrix;
    static readonly Vector3 vecDown = Vector3.down;

    protected override void Start() {
        base.Start();
        if (parentCBase) {
            parentImperatrix = parentCBase.GetComponent<Enemy_ImperatrixMundi>();
        }
    }

    public override bool ReceiveDamage(ref Vector3 effectPosition, int damage, float knockAmount, ref Vector3 knockVector, AttackDetection attackDetection = null, bool penetrate = false, int overrideColorType = -1) {
        hitEffectNum = 0;
        if (overrideColorType < 0 && attackDetection != null && attackDetection.parentCBase != null && attackDetection.parentCBase.isPlayer && attackDetection.parentCBase.isSuperman) {
            PlayerController pConTemp = attackDetection.parentCBase.GetComponent<PlayerController>();
            if (pConTemp && pConTemp.isHyper) {
                hitEffectNum = 1;
                overrideColorType = hyperColorType;
            }
        }
        if (parentImperatrix) {
            int knockType = 0;
            if (checkerCollider && attackDetection != null && attackDetection.parentCBase && attackDetection.parentCBase.isPlayer) {
                Vector3 attackerPos = attackDetection.parentCBase.GetCenterPosition();
                Vector3 closestPos = checkerCollider.ClosestPoint(attackerPos);
                Vector3 direction = closestPos - attackerPos;
                if (direction.sqrMagnitude >= 0.0001f) {
                    float angle = Vector3.Angle(vecDown, direction);
                    if (angle < 45) {
                        knockType = 1;
                    }
                }

            }
            parentImperatrix.SetKnockType(knockType);
        }
        return base.ReceiveDamage(ref effectPosition, damage, knockAmount, ref knockVector, attackDetection, penetrate, overrideColorType);
    }

}
