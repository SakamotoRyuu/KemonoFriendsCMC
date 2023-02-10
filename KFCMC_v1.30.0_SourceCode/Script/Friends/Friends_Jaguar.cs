using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class Friends_Jaguar : FriendsBase {
    
    public XWeaponTrail jumpTrail;
    public Transform movePivot;

    float jumpAttackedRemainTime;
    float heightDistConditionTime;
    bool jumpTrailEnabled;
    bool refresh;
    bool movingFlag;

    const int effectJump = 0;
    const float spBias = 1.1f;
    
    protected override void Start() {
        base.Start();
        if (animatorForBattle) {
            jumpTrail.Init();
            jumpTrail.Deactivate();
            jumpTrailEnabled = false;
            moveCost.attack = 14f * staminaCostRate;
            moveCost.skill = 40f;
        }
    }

    public override void SetForItem() {
        jumpTrail.gameObject.SetActive(false);
        base.SetForItem();
    }

    public override void EmitEffectString(string type) {
        switch (type) {
            case "Jump":
                EmitEffect(effectJump);
                break;
        }
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (jumpAttackedRemainTime > 0f) {
            jumpAttackedRemainTime -= deltaTimeMove;
        }
    }
    
    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (state != State.Jump && state != State.Attack) {
            isJumpingAttack = false;
            if (jumpTrailEnabled) {
                jumpTrail.StopSmoothly(0.1f);
                jumpTrailEnabled = false;
            }
        }
        if (targetTrans) {
            float heightDist = targetTrans.position.y - targetRadius - trans.position.y;
            if (heightDist >= 1.2f && heightDist <= 4f) {
                heightDistConditionTime += deltaTimeCache;
            } else {
                heightDistConditionTime = 0f;
            }
        } else {
            heightDistConditionTime = 0f;
        }
    }
    
    void MoveEnd() {
        movingFlag = false;
        LockonEnd();
    }

    protected override void Update_Transition_Moves() {
        if (refresh && nowST >= GetMaxST() * staminaBorder) {
            refresh = false;
            actDistNum = 0;
        } else if (!refresh && !JudgeStamina(GetCost(CostType.Attack))) {
            refresh = true;
            actDistNum = 2;
        }
        if (!refresh && target && targetTrans) {
            float sqrDist = GetTargetDistance(true, false, true);
            if (jumpAttackedRemainTime <= 0f && sqrDist > 4.5f * 4.5f && JudgeStamina(GetCost(CostType.Skill))) {
                actDistNum = 1;
            } else {
                actDistNum = 0;
            }
        }
        base.Update_Transition_Moves();
    }
    
    void SetCombo2() {
        SetAttackPowerMultiplier(1.9f);
        SetKnockPowerMultiplier(2.3f);
    }

    void MoveAttack() {
        if (!refresh) {
            float spRate = isSuperman ? 4f / 3f : 1;
            SpecialStep(0.4f, 0.25f / spRate, 4f, 0.05f, 0.4f, true, true, EasingType.SineInOut, true);
        }
    }

    float GetTargetHeight() {
        if (targetTrans) {
            return targetTrans.position.y - trans.position.y;
        }
        return 0f;
    }

    protected override void Attack() {
        base.Attack();
        float spRate = isSuperman ? 4f / 3f : 1;
        Vector3 targetTemp = trans.position;
        float sqrDist = GetTargetDistance(true, true, true);
        movingFlag = false;
        if (!isJumpingAttack) {
            if (jumpAttackedRemainTime <= 0f && (sqrDist >= 2.5f * 2.5f || heightDistConditionTime >= 0.4f) && JudgeStamina(GetCost(CostType.Jump) + GetCost(CostType.Skill)) && !GetSick(SickType.Mud)) {
                anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], 1);
                jumpTrailEnabled = true;
                jumpTrail.Activate();
                isJumpingAttack = true;
                S_ParticlePlay(0);
                Jump(Mathf.Clamp(4.2f + GetTargetHeight(), 5.6f, 8f));
                gravityMultiplier = 0.8f;
                jumpAttackedRemainTime = 2.5f;
                attackedTimeRemain = 0.3f;
                SuperarmorStart();
            } else {
                isJumpingAttack = false;
                if (attackProcess >= 2 && !JudgeStamina(GetCost(CostType.Attack) * ((60f / spBias) / 14f))) {
                    attackProcess = 0;
                }
                if (attackProcess < 2) {
                    if (AttackBase(attackProcess, 1f, 1f, GetCost(CostType.Attack), 14f / 30f / spRate, 14f / 30f / spRate, 0.5f, spRate)) {
                        S_ParticlePlay(attackProcess);
                        MoveAttack();
                    }
                } else {
                    if (AttackBase(2, 1.35f, 1.6f, GetCost(CostType.Attack) * ((60f / spBias) / 14f), 60f / 30f / spBias / spRate, 60f / 30f / spBias / spRate, 0.5f, spBias * spRate)) {
                        S_ParticlePlay(2);
                        MoveAttack();
                    }
                }
                attackProcess = (attackProcess + 1) % 3;
            }
        } else {
            if (nowST < GetCost(CostType.Skill)) {
                nowST = GetCost(CostType.Skill);
            }
            if (AttackBase(4, 2.2f, 2.5f, GetCost(CostType.Skill), 20f / 30f / 1.2f, 38f / 30f / 1.2f, 0f, 1.2f)) {
                gravityMultiplier = 2f;
                isJumpingAttack = true;
                movingFlag = true;
                attackProcess = 0;
            }
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (attackType == 4 && movingFlag && targetTrans) {
            ApproachTransformPivot(movePivot, 15f, targetRadius + 0.3f, targetRadius, false);
        }
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (isJumpingAttack && damage > 0) {
            damage = Mathf.Max(damage / 2, 1);
        }
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    protected override void Update_Transition_Jump() {
        base.Update_Transition_Jump();
        if (targetTrans && GetCanControl() && (command == Command.Default || command == Command.Free)) {
            float sqrDist = GetTargetDistance(true, true, true);
            if (state != State.Attack && sqrDist < 2f * 2f && attackedTimeRemain <= 0) {
                SetState(State.Attack);
            }
        }
    }

    protected override void Update_Process_Jump() {
        base.Update_Process_Jump();
        if (targetTrans && GetCanControl() && (command == Command.Default || command == Command.Free)) {
            lockonRotSpeed = 15f;
            CommonLockon();
            /*
            float sqrDist = GetTargetDistance(true, true, false);
            if (sqrDist > MyMath.Square(targetRadius)) {
                cCon.Move(GetTargetVector(true, true) * 11f * deltaTimeMove);
            }
            */
            Continuous_Approach(11f, 0.4f, 0.2f, true, true);
        }
    }

    public bool CheckTrophy_IsJumpingAttack() {
        return state == State.Attack && attackType == 4;
    }

}
