using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dissodinium : EnemyBase {

    int attackSave = -1;

    protected override void Awake() {
        base.Awake();
        throwing = GetComponent<Throwing>();
    }

    protected override void SetLevelModifier() {
        if (level >= 2) {
            actDistNum = 1;
        } else {
            actDistNum = 0;
        }
    }

    void MoveAttack1() {
        SpecialStep(1f, 25f / 60f, 2f, 0f, 0f, true, false);
    }

    protected override void DeadProcess() {
        base.DeadProcess();
        if (t_AngryFlag) {
            CharacterManager.Instance.CheckTrophy_Ibis();
        }
    }

    protected override void Attack() {
        int attackTemp = 0;
        if (level >= 3) {
            attackTemp = Random.Range(0, level >= 4 ? 3 : 2);
            if (attackTemp == attackSave) {
                attackTemp = Random.Range(0, level >= 4 ? 3 : 2);
            }
        }
        attackSave = attackTemp;
        switch (attackTemp) {
            case 0:
                AttackBase(0, 1, 1.2f, 0, 90f / 60f, 90f / 60f + GetAttackInterval(1.3f), 0);
                SpecialStep(0f, 50f / 60f, 1f, 0f, 0.5f);
                break;
            case 1:
                AttackBase(1, 1f, 1.2f, 0, 130f / 60f, 130f / 60f + GetAttackInterval(1.3f, -2), 0);
                SpecialStep(0f, 50f / 60f, 1f, 0f, 0.5f);
                break;
            case 2:
                AttackBase(2, 1.1f, 1.2f, 0, 150f / 60f, 150f / 60f + GetAttackInterval(1.3f, -3));
                SpecialStep(1f, 45f / 60f, 1.5f, 0f, 0f);
                break;
        }
    }
   
    private void ThrowingAssortment() {
        if (level >= 2) {
            throwing.ThrowStart(level - 2);
        }
    }

}
