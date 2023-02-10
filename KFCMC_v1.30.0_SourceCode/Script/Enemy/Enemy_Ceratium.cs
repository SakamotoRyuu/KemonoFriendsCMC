using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ceratium : EnemyBase {

    int attackSaveNear = -1;
    int attackSaveFar = -1;
    
    protected override void SetLevelModifier() {
        if (level >= 3) {
            actDistNum = 1;
        } else {
            actDistNum = 0;
        }
    }

    protected override void Attack() {
        if (targetTrans) {
            float sqrDist = GetTargetDistance(true, true, false);
            if (sqrDist > 2f) {
                int attackRand = Random.Range(0, 3);
                if (attackRand == attackSaveFar) {
                    attackRand = Random.Range(0, 3);
                }
                attackSaveFar = attackRand;
                if (level >= 4 && attackRand == 2) {
                    fbStepTime = 20f / 60f;
                    fbStepMaxDist = 4f;
                    ApproachOrSeparate(4.2f);
                    AttackBase(3, 1.2f, 1.6f, 0, 110f / 60f, 110f / 60f + GetAttackInterval(1f, -3), 0);
                } else {
                    int subType = (level >= 3 && sqrDist > 4f ? 4 : 0);
                    AttackBase(subType, 1.1f, 1.2f, 0, 100f / 60f, 100f / 60f + GetAttackInterval(1f, subType == 4 ? -2 : 0), 0);
                }
            } else {
                int attackRand = Random.Range(0, 2);
                if (attackRand == attackSaveNear) {
                    attackRand = Random.Range(0, 2);
                }
                attackSaveNear = attackRand;
                if (level >= 2 && attackRand == 1) {
                    AttackBase(2, 1, 0.8f, 0, 170f / 60f, 170f / 60f + GetAttackInterval(1f, -1), 1, 1, false);
                } else {
                    AttackBase(1, 1, 0.8f, 0, 1.5f, 1.5f + GetAttackInterval(1f), 1, 1, false);
                }
            }
        }
    }

    protected override void ResetTriggerOnDamage() {
        base.ResetTriggerOnDamage();
        if (attackedTimeRemain > 0.5f) {
            attackedTimeRemain = 0.5f;
        }
    }
}
