using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Moose : FriendsBase {

    public GameObject wallBreakerTackle;
    public GameObject wallBreakerImpact;
    public Transform quakePivot;
    public Transform throwOrigin;
    public int throwingCount;
    public float throwingRadius;
    public Transform[] movePivot;

    float spBias = 5f / 3f;
    int moveIndex = -1;
    float tackledTimeRemain;
    const int attackIndexTackle = 4;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            mesAtkMax = 3;
            chatAttackCount = 4;
            moveCost.attack = 42f / spBias * staminaCostRate;
            moveCost.skill = 28f * staminaCostRate;
        }
    }

    void MoveAttack1() {
        if (actDistNum == 0) {
            float spRate = isSuperman ? 4f / 3f : 1;
            specialMoveDirectionAdjustEnabled = true;
            SpecialStep(0.45f, 14f / 30f / spBias / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
        }
    }

    void MoveAttack2() {
        if (actDistNum == 0) {
            float spRate = isSuperman ? 4f / 3f : 1;
            specialMoveDirectionAdjustEnabled = true;
            SpecialStep(0.45f, 14f / 30f / spBias / spRate, 4f, 0f, 0f, true, true, EasingType.Linear, true);
        }
    }

    void MoveAttack3() {
        float spRate = isSuperman ? 4f / 3f : 1;
        specialMoveDirectionAdjustEnabled = false;
        SpecialStep(-1.5f, 12f / 30f / spRate, 6f, 4f, 4f, true, false);
    }

    void SetWallBreakerTackle(int flag) {
        wallBreakerTackle.SetActive(flag != 0);
    }

    void SetWallBreakerImpact(int flag) {
        wallBreakerImpact.SetActive(flag != 0);
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (state != State.Attack && wallBreakerTackle.activeSelf) {
            wallBreakerTackle.SetActive(false);
        }
        if (state != State.Attack && wallBreakerImpact.activeSelf) {
            wallBreakerImpact.SetActive(false);
        }
        if (actDistNum == 0 && !JudgeStamina(GetCost(CostType.Attack)) && (state != State.Attack || attackedTimeRemain < 0.05f)) {
            actDistNum = 1;
        } else if (actDistNum == 1 && nowST >= GetMaxST() * staminaBorder) {
            actDistNum = 0;
        }
        if (tackledTimeRemain > 0f && !(state == State.Attack && attackType == attackIndexTackle)) {
            tackledTimeRemain -= deltaTimeMove;
            if (tackledTimeRemain < 0f) {
                tackledTimeRemain = 0f;
            }
        }
    }

    protected override void Attack() {
        base.Attack();
        float spRate = isSuperman ? 4f / 3f : 1;
        if (attackProcess == 0 && Random.Range(0f, 6f + tackledTimeRemain) < 3f && JudgeStamina(GetCost(CostType.Skill))) {
            if (AttackBase(attackIndexTackle, 1.1f, 1.3f, GetCost(CostType.Skill), 28f / 30f, 28f / 30f)) {
                S_ParticlePlay(1);
                attackProcess = 0;
                tackledTimeRemain += 1f;
                if (tackledTimeRemain > 3f) {
                    tackledTimeRemain = 3f;
                }
            }
        } else {
            if (attackProcess >= 2 && !JudgeStamina(GetCost(CostType.Attack) * (48f / 42f))) {
                attackProcess = 0;
            }
            switch (attackProcess) {
                case 0:
                    if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 42f / 30f / spBias / spRate, 42f / 30f / spBias / spRate, 1f, spBias * spRate)) {
                        S_ParticlePlay(0);
                        MoveStart(0);
                    }
                    break;
                case 1:
                    if (AttackBase(1, 1f, 1f, GetCost(CostType.Attack), 42f / 30f / spBias / spRate, 42f / 30f / spBias / spRate, 1f, spBias * spRate)) {
                        S_ParticlePlay(0);
                    }
                    break;
                case 2:
                    if (AttackBase(2, 1.15f, 1.2f, GetCost(CostType.Attack) * (48f / 42f), 48f / 30f / spBias / spRate, 48f / 30f / spBias / spRate, 1f, spBias * spRate)) {
                        S_ParticlePlay(0);
                    }
                    break;
                case 3:
                    if (AttackBase(GetTargetHeight(true) >= 1.2f ? 5 : 3, 1.3f, 2f, GetCost(CostType.Attack) * (57f / 42f), 57f / 30f / spBias / spRate, 70f / 30f / spBias / spRate, 1f, spBias * spRate)) {
                        S_ParticlePlay(0);
                        tackledTimeRemain = 0f;
                    }
                    break;
            }
            attackProcess = (attackProcess + 1) % 4;
        }
        SuperarmorStart();        
    }

    void Impact() {
        if (!isItem && state == State.Attack) {
            EmitEffect(0);
            CameraManager.Instance.SetQuake(quakePivot.position, 6, 4, 0, 0, 1.5f, 1.5f, 12.5f);
            SetWallBreakerImpact(1);
            for (int i = 0; i < throwingCount; i++) {
                Vector3 pivotPos = throwOrigin.localPosition;
                Vector2 circlePos = Random.insideUnitCircle * throwingRadius;
                int throwIndex = Random.Range(0, throwing.throwSettings.Length);
                pivotPos.x += circlePos.x;
                pivotPos.z += circlePos.y;
                throwing.throwSettings[throwIndex].from.transform.localPosition = pivotPos;
                throwing.ThrowStart(throwIndex);
            }
        }
    }

    void MoveStart(int index) {
        moveIndex = index;
    }

    void MoveEnd() {
        moveIndex = -1;
        LockonEnd();
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (moveIndex >= 0 && moveIndex < movePivot.Length && movePivot[moveIndex]) {
            float spRate = isSuperman ? 4f / 3f : 1f;
            ApproachTransformPivot(movePivot[moveIndex], 13.5f * spRate, Mathf.Max(targetRadius, 0.4f), Mathf.Max(targetRadius - 0.4f, 0.075f), true, 0.25f);
        }
    }

}
