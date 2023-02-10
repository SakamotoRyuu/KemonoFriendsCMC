using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_PlugAdapter : EnemyBase {

    public Transform quakePivot;
    public GameObject wallBreaker;
    public DamageDetection criticalDD;

    int attackSave = -1;
    int throwReservedIndex = 0;

    static readonly float[] damageRateArray = new float[5] { 4f, 4f, 3.833333f, 3.666666f, 3.5f };

    void MoveAttack0() {
        SpecialStep(0.2f, 40f / 60f, 4f, 0f, 0f, true, false, EasingType.SineOut);
    }

    void MoveAttack1_1() {
        if (state == State.Attack) {
            fbStepTime = 0.3f;
            ForwardOrBackStep(3);
        }
    }

    void MoveAttack1_2() {
        if (state == State.Attack) {
            fbStepTime = 35f / 60f;
            ForwardStep(-0.5f);
        }
    }

    void MoveEnd() { }

    void QuakeAttack() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePivot.position, 8, 4, 0, 0, 1.5f, 2f, dissipationDistance_Normal);
        }
    }

    void MoveAttack2() {
        if (state == State.Attack) {
            fbStepTime = 25f / 60f;
            BackStep(6f);
        }
    }

    void ThrowSpecialStart() {
        throwing.ThrowStart(throwReservedIndex);
    }

    protected override void SetLevelModifier() {
        if (level >= 3) {
            actDistNum = 2;
            wallBreaker.SetActive(true);
        } else if (level >= 2) {
            actDistNum = 1;
            wallBreaker.SetActive(true);
        } else {
            actDistNum = 0;
            wallBreaker.SetActive(false);
        }
        if (criticalDD) {
            criticalDD.damageRate = damageRateArray[Mathf.Clamp(level, 0, damageRateArray.Length - 1)];
        }
        attackSave = -1;
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (!fixKnockAmount && nowHP <= 0 && CharacterManager.Instance.GetFriendsExist(8) && attackerCB == CharacterManager.Instance.friends[8].fBase) {
            CharacterManager.Instance.CheckTrophy_Tsuchinoko();
        }
    }

    protected override void Attack() {
        int attackTemp = 0;
        resetAgentRadiusOnChangeState = true;
        if (level >= 2) {
            attackTemp = Random.Range(0, 2);
            if (attackTemp == 1 && level >= 3) {
                attackTemp = Random.Range(1, 3);
            }
            if (level >= 3 && targetTrans && (targetTrans.position - trans.position).sqrMagnitude > 7f * 7f) {
                attackTemp = 2;
            }
            if (attackTemp == attackSave) {
                attackTemp = Random.Range(0, 2);
            }
            if (attackTemp == 2) {
                if (level >= 4 && Random.Range(0, 2) == 1) {
                    throwReservedIndex = 1;
                } else {
                    throwReservedIndex = 0;
                }
            }
        }
        attackSave = attackTemp;
        switch (attackTemp) {
            case 0:
                AttackBase(0, 1, 1.4f, 0, 150f / 60f, 150f / 60f + GetAttackInterval(1.5f), 0f);
                agent.radius = 0.05f;
                break;
            case 1:
                AttackBase(1, 1, 0.8f, 0, 120f / 60f + (IsSuperLevel ? 0f : level >= 4 ? 0.5f : level == 3 ? 1.0f : 1.5f), 120f / 60f + GetAttackInterval(1.5f, -1));
                break;
            case 2:
                AttackBase(2, 1, 0.8f, 0, 90f / 60f + (IsSuperLevel ? 0f : throwReservedIndex == 1 || level == 3 ? 1.5f : 1.0f), 90f / 60f + GetAttackInterval(1.5f, throwReservedIndex == 0 ? -2 : -3));
                break;
        }
    }
}
