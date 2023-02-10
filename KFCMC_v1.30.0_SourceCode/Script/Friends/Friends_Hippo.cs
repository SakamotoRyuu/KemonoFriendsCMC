using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Hippo : FriendsBase {

    public CheckTriggerStay checkTriggerStay;

    private float counterTimeRemain;
    private float coolTimeRemain;
    private int fatiguePoint;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            //moveCost.attack = 12f;
            //moveCost.skill = 12f;
            moveCost.attack = 30f * staminaCostRate;
            moveCost.skill = 30f * staminaCostRate;
            stoppingDistanceBattle = 2;
        }
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (counterTimeRemain > 0f) {
            counterTimeRemain -= deltaTimeMove;
        }
        if (coolTimeRemain > 0f) {
            coolTimeRemain -= deltaTimeMove;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (actDistNum == 0 && !JudgeStamina(GetCost(CostType.Skill)) && (state != State.Attack || attackedTimeRemain < 0.05f)) {
            actDistNum = 1;
        } else if (actDistNum == 1 && nowST >= GetMaxST() * staminaBorder) {
            actDistNum = 0;
        }
    }
    
    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        if (!isSuperarmor && counterTimeRemain > 0f) {
            counterTimeRemain = 0f;
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        if (counterTimeRemain > 0f) {
            counterTimeRemain = 0f;
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        fatiguePoint -= 5;
        if (fatiguePoint < 0) {
            fatiguePoint = 0;
        }
    }

    public override void SuperarmorEnd() {
        if (counterTimeRemain <= 0f) {
            isSuperarmor = false;
        }
    }

    protected override void SideStep_Head(int direction, float mutekiTime = 0, bool changeState = true) {
        if (counterTimeRemain > 0f) {
            changeState = false;
        }
        base.SideStep_Head(direction, mutekiTime, changeState);
    }

    protected override void SideStep_Move(Vector3 dir, float distance = 5) {
        if (counterTimeRemain <= 0f) {
            base.SideStep_Move(dir, distance);
        }
    }

    void MoveAttack() {
        if (actDistNum == 0) {
            float spRate = isSuperman ? 4f / 3f : 1f;
            SpecialStep(0.3f, 8f / 30f / spRate, 5f, 0.05f, 0.5f, true, true, EasingType.SineInOut, true);
        }
    }

    public override void ResetGuts() {
        base.ResetGuts();
        fatiguePoint = 0;
    }

    protected override void Attack() {
        base.Attack();
        float spRate = isSuperman ? 4f / 3f : 1f;
        if (counterTimeRemain > 0) {
            attackProcess = 0;
            if (AttackBase(3, 1.2f, 1.5f, GetCost(CostType.Attack), 27f / 30f / spRate, 27f / 30f / spRate, 0.5f, spRate, true, 40f)) {
                PerfectLockon();
                SpecialStep(0.3f, 7f / 30f / spRate, 5f, 0.05f, 0.5f, true, true, EasingType.SineInOut, true);
                S_ParticlePlay(0);
                SuperarmorStart();
                counterTimeRemain = 0f;
            }
        } else {
            if (AttackBase(attackProcess, 1f, 1f, GetCost(CostType.Attack), 30f / 30f / spRate, 30f / 30f / spRate, 0.5f, spRate)) {
                S_ParticlePlay(0);
            }
            attackProcess = (attackProcess + 1) % 2;
        }
    }

    public override void CounterAttack(GameObject attackerObject = null, bool isProjectile = false) {
        if (!isProjectile && counterTimeRemain <= 0f && coolTimeRemain <= 0f && checkTriggerStay.stayFlag && GetCanMove() && JudgeStamina(GetCost(CostType.Skill))) {
            base.CounterAttack(attackerObject);
            SetState(State.Wait);
            state = State.Attack;
            if (AttackBase(2, 1f, 1f, GetCost(CostType.Skill), 12f / 30f, 12f / 30f, 0f, 1f, true, 15f)) {
                PerfectLockon();
                S_ParticlePlay(0);
                S_ParticlePlay(1);
                AttackStart(2);
                SuperarmorStart();
                attackProcess = 0;
                EmitEffect(0);
                counterTimeRemain = 1.5f;
                coolTimeRemain = 1.5f + fatiguePoint * 0.2f;
                specialMoveDuration = 0f;
                if (fatiguePoint < 10) {
                    fatiguePoint++;
                }
            }
        }
    }

}
