using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class Friends_Tsuchinoko : FriendsBase {

    public LookatTarget lookatTarget;
    public Transform nullTarget;
    LaserOption laserOption;
    bool refresh = false;
    float lastKickTimeRemain = 0f;
    float skilledTimeRemain;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            moveCost.attack = 28f * staminaCostRate;
            moveCost.skill = 120;
            laserOption = GetComponent<LaserOption>();
            lookatTarget.enabled = true;
        }
    }

    public override void ResetGuts() {
        base.ResetGuts();
        if (laserOption) {
            laserOption.CancelLaser();
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (state != State.Attack && laserOption) {
            laserOption.CancelLaser();
        }
        if (state != State.Attack && !isSuperman && supermanSettings.isSpecial) {
            LightEyeActivate(0);
        }
        if (state != State.Attack && skilledTimeRemain > 0f) {
            skilledTimeRemain -= deltaTimeCache;
        }
    }

    protected override void Update_Targeting() {
        base.Update_Targeting();
        if (lookatTarget) {
            if (targetTrans) {
                lookatTarget.SetTarget(targetTrans);
            } else {
                lookatTarget.SetTarget(nullTarget);
            }
        }
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        lastKickTimeRemain -= deltaTimeMove;
    }

    protected override void Update_Transition_Moves() {
        if (refresh && nowST >= GetMaxST() * staminaBorder && JudgeStamina(GetCost(CostType.Skill))) {
            refresh = false;
            actDistNum = 0;
        } else if (!refresh && !JudgeStamina(GetCost(CostType.Skill))) {
            refresh = true;
            actDistNum = 2;
        }
        if (!refresh && target && targetTrans) {
            Vector3 dirTemp = (trans.position - targetTrans.position).normalized;
            float dist = ((targetTrans.position + dirTemp * targetRadius) - trans.position).sqrMagnitude;
            if (JudgeStamina(GetCost(CostType.Skill))) {
                actDistNum = 1;
            } else {
                actDistNum = 0;
            }
        }
        base.Update_Transition_Moves();
    }

    protected void LightEyeActivate(int param) {
        if (!isSuperman) {
            SupermanSetMaterial(param != 0);
        }
    }

    void MoveAttack() {
        if (targetTrans) {
            float sqrDist = GetTargetDistance(true, true, true);
            if (sqrDist < 4f * 4f) {
                fbStepMaxDist = 4f;
                fbStepTime = 11f / 30f;
                SeparateFromTarget(4f);
            } else {
                SpecialStep(4.2f, 11f / 30f, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
            }
        }
    }

    protected override void Attack() {
        base.Attack();
        float spRate = isSuperman ? 4f / 3f : 1;
        float sqrDist = GetTargetDistance(true, false, true);
        float skillCost = GetCost(CostType.Skill);
        skilledTimeRemain = 0f;
        if (nowST < skillCost) {
            skilledTimeRemain = Mathf.Abs(nowST - skillCost) / Mathf.Max(1f, GetMaxST() * GetSTHealRateChild_Normal());
        }
        if ((sqrDist >= 4f || lastKickTimeRemain > 0) && JudgeStamina(skillCost)) {
            if (AttackBase(1, 1f, 1f, skillCost, 98f / 30f, 98f / 30f + 0.6f, 0f, 1f)) {
                S_ParticlePlay(1);
                fbStepMaxDist = 2f;
                fbStepTime = 0.25f;
                SeparateFromTarget(4f);
            }
        } else {
            if (AttackBase(0, 1.2f, 2f, GetCost(CostType.Attack), 28f / 30f / spRate, 28f / 30f / spRate + 0.5f, 0f, spRate)) {
                S_ParticlePlay(0);
                if (!refresh) {
                    SpecialStep(0.4f, 0.25f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                }
                lastKickTimeRemain = 1.5f;
            }
        }
    }

    private void LaserReady() {
        if (!isItem) {
            AttackStart(2);
            laserOption.LightFlickeringChargeStart();
            LightEyeActivate(1);
        }
    }

    private void LaserStart() {
        if (!isItem) {
            AttackEnd(2);
            laserOption.LightFlickeringChargeEnd();
            AttackStart(1);
        }
    }

    private void LaserEnd() {
        if (!isItem) {
            AttackEnd(1);
            laserOption.LightFlickeringBlastEnd();
            LightEyeActivate(0);
            LockonEnd();
        }
    }

    protected override float GetSTHealRate() {
        return base.GetSTHealRate() * (skilledTimeRemain <= 0f ? 1f : 0f);
    }

}
