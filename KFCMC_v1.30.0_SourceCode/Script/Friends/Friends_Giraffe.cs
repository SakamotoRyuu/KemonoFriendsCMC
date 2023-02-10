using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Giraffe : FriendsBase {

    public Transform[] checkScaleTarget;

    bool refresh = false;
    float spinAttackedTimeRemain = 0f;
    Quaternion[] checkScaleTargetDefaultRotation;
    Vector3[] checkScaleTargetDefaultScale;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            moveCost.attack = 25f * staminaCostRate;
            moveCost.skill = 32f;
            checkScaleTargetDefaultRotation = new Quaternion[checkScaleTarget.Length];
            checkScaleTargetDefaultScale = new Vector3[checkScaleTarget.Length];
            for (int i = 0; i < checkScaleTarget.Length; i++) {
                if (checkScaleTarget[i]) {
                    checkScaleTargetDefaultRotation[i] = checkScaleTarget[i].localRotation;
                    checkScaleTargetDefaultScale[i] = checkScaleTarget[i].localScale;
                }
            }
        }
    }

    protected override void Update_Transition_Moves() {
        if (refresh && nowST >= GetMaxST() * staminaBorder) {
            refresh = false;
            actDistNum = 0;
        } else if (!refresh && !JudgeStamina(GetCost(CostType.Attack))) {
            refresh = true;
            actDistNum = 1;
        }
        base.Update_Transition_Moves();
    }

    private void OnDisable() {
        if (animatorForBattle) {
            for (int i = 0; i < checkScaleTarget.Length; i++) {
                if (checkScaleTarget[i]) {
                    checkScaleTarget[i].localRotation = checkScaleTargetDefaultRotation[i];
                    checkScaleTarget[i].localScale = checkScaleTargetDefaultScale[i];
                }
            }
        }
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (spinAttackedTimeRemain > 0f) {
            spinAttackedTimeRemain -= deltaTimeMove * (isSuperman ? 4f / 3f : 1);
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (spinAttackedTimeRemain > 0f) {
            spinAttackedTimeRemain = 0f;
        }
    }

    void MoveAttack1() {
        if (!refresh) {
            float spRate = isSuperman ? 4f / 3f : 1;
            SpecialStep(0.4f, 0.25f / spRate, 4f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
        }
    }

    void MoveAttack2() {
        if (!refresh) {
            float spRate = isSuperman ? 4f / 3f : 1;
            SpecialStep(1.2f, 0.25f / spRate, 4f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
        }
    }

    protected override void Attack() {
        base.Attack();
        float spRate = isSuperman ? 4f / 3f : 1;
        float heightDist = 0f;
        if (targetTrans) {
            heightDist = targetTrans.position.y - targetRadius - trans.position.y;
        }
        if (attackProcess == 0 && spinAttackedTimeRemain <= 0f && JudgeStamina(GetCost(CostType.Skill)) && (Random.Range(0, 100) < 75 || searchArea[0].IsBesieged(CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Long) ? 10f : 3.5f))) {
            if (AttackBase(heightDist >= 1.4f ? 5 : 4, 1.1f, 1.25f, GetCost(CostType.Skill), 30f / 30f / 0.7f / spRate, 30f / 30f / 0.7f / spRate, 1f, 0.7f * spRate)) {
                S_ParticlePlay(0);
                S_ParticlePlay(1);
                SuperarmorStart();
                MoveAttack2();
                spinAttackedTimeRemain = 3.5f;
            }
        } else {
            if (attackProcess >= 1 && !JudgeStamina(GetCost(CostType.Attack) * (34f / 25f))) {
                attackProcess = 0;
            }
            if (attackProcess == 2 && JudgeStamina(GetCost(CostType.Attack) * (48f / 25f))) {
                if (AttackBase(3, 1.15f, 1.15f, GetCost(CostType.Attack) * (48f / 25f) , 48f / 30f / spRate, 48f / 30f / spRate, 1f, spRate)) {
                    S_ParticlePlay(2);
                    S_ParticlePlay(3);
                    SuperarmorStart();
                    MoveAttack1();
                    attackProcess = 0;
                }
            } else if (heightDist >= 1.2f && JudgeStamina(GetCost(CostType.Attack) * (34f / 25f))) {
                if (AttackBase(2, 1.3f, 1.3f, GetCost(CostType.Attack) * (34f / 25f), 34f / 30f / spRate, 34f / 30f / spRate, 1f, spRate)) {
                    S_ParticlePlay(2);
                    MoveAttack1();
                    attackProcess = 2;
                }
            } else {
                if (attackProcess == 0) {
                    if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 25f / 30f / spRate, 25f / 30f / spRate, 1f, spRate)) {
                        S_ParticlePlay(2);
                        MoveAttack1();
                        attackProcess = 1;
                    }
                } else {
                    if (AttackBase(1, 1.25f, 1.25f, GetCost(CostType.Attack) * (31f / 25f), 31f / 30f / spRate, 31f / 30f / spRate, 1f, spRate)) {
                        S_ParticlePlay(2);
                        MoveAttack1();
                        attackProcess = 2;
                    }
                }
            }
        }
    }

}
