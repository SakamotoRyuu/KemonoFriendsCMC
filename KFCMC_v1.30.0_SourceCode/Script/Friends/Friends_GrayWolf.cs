using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class Friends_GrayWolf : FriendsBase {

    public XWeaponTrail jumpTrail;
    public Transform movePivot;

    float wolfHowlingRemainTime = 0f;
    float jumpAttackedRemainTime = 0f;
    float heightDistConditionTime = 0f;
    bool jumpTrailEnabled = false;
    bool refresh = false;
    bool movingFlag;
    const int effectJump = 0;
    const string specialAttackFaceName = "Attack2";
    const float spBias = 1.2f;

    protected override void Start() {
        base.Start();
        if (animatorForBattle) {
            jumpTrail.Init();
            jumpTrail.Deactivate();
            jumpTrailEnabled = false;
            moveCost.attack = 14f * staminaCostRate;
            moveCost.skill = 30f;
            specialMoveDirectionAdjustEnabled = true;
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

    void Howling() {
        wolfHowlingRemainTime = Random.Range(10f, 12f);
        if (throwing) {
            throwing.ThrowReady(0);
        }
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (wolfHowlingRemainTime > 0f) {
            wolfHowlingRemainTime -= deltaTimeMove;
        }
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
            if (wolfHowlingRemainTime <= 0f && JudgeStamina(GetCost(CostType.Skill) * 2f)) {
                actDistNum = 3;
            } else if (jumpAttackedRemainTime <= 0f && sqrDist > 4.5f * 4.5f && JudgeStamina(GetCost(CostType.Skill))) {
                actDistNum = 1;
            } else {
                actDistNum = 0;
            }
        }
        base.Update_Transition_Moves();
    }

    protected override void SetFace(int faceIndex) {
        if (faceIndex == this.faceIndex[(int)FaceName.Attack] && attackType == 5) {
            fCon.SetFace(specialAttackFaceName);
        } else {
            base.SetFace(faceIndex);
        }
    }
    
    public override void ChangeActionDistance(bool isBossBattle) {
        if (!isBossBattle) {
            agentActionDistance[3].attack.y = 20f;
        } else {
            agentActionDistance[3].attack.y = 30f;
        }
    }

    void MoveAttack() {
        if (!refresh) {
            float spRate = isSuperman ? 4f / 3f : 1;
            SpecialStep(0.3f, 0.25f / spRate, 4f, 0.05f, 0.4f, true, true, EasingType.SineInOut, true);
        }
    }

    void MoveAttack2() {
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
            if (actDistNum == 3 && attackProcess == 0 && sqrDist >= 5f * 5f) {
                if (AttackBase(5, 1f, 1f, GetCost(CostType.Skill) * 2f, 76f / 30f / spRate, (76f / 30f + 0.5f) / spRate, 0, spRate, false)) {
                    wolfHowlingRemainTime = 3f;
                }
            } else if (jumpAttackedRemainTime <= 0f && (sqrDist >= 2.5f * 2.5f || heightDistConditionTime >= 0.4f) && JudgeStamina(GetCost(CostType.Jump) + GetCost(CostType.Skill)) && !GetSick(SickType.Mud)) {
                anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], 1);
                jumpTrailEnabled = true;
                jumpTrail.Activate();
                isJumpingAttack = true;
                S_ParticlePlay(0);
                Jump(Mathf.Clamp(4.2f + GetTargetHeight(), 5.6f, 8f));
                gravityMultiplier = 0.8f;
                jumpAttackedRemainTime = 2.5f;
                attackedTimeRemain = 0.3f;
            } else {
                isJumpingAttack = false;
                if (attackProcess >= 2 && !JudgeStamina(GetCost(CostType.Attack) * (20f / 14f))) {
                    attackProcess = 0;
                }
                if (attackProcess == 0) {
                    if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 14f / 30f / spRate, 14f / 30f / spRate, 0.5f, spRate)) {
                        S_ParticlePlay(0);
                        MoveAttack2();
                        attackProcess = 1;
                    }
                } else if (attackProcess == 1) {
                    if (AttackBase(1, 1f, 1f, GetCost(CostType.Attack), 14f / 30f / spRate, 14f / 30f / spRate, 0.5f, spRate)) {
                        S_ParticlePlay(1);
                        MoveAttack2();
                        attackProcess = 2;
                    }
                } else if (attackProcess == 2) {
                    if (AttackBase(2, 0.9f, 0.7f, GetCost(CostType.Attack) * (20f / 14f), 20f / 30f / spRate, 20f / 30f / spRate, 0.5f, spRate)) {
                        S_ParticlePlay(0);
                        S_ParticlePlay(1);
                        MoveAttack();
                        attackProcess = 3;
                    }
                } else {
                    if (AttackBase(3, 2f, 2.4f, GetCost(CostType.Attack) * ((41f / spBias) / 14f), 41f / 30f / spBias / spRate, 41f / 30f / spBias / spRate, 0.5f, spBias * spRate)) {
                        S_ParticlePlay(2);
                        MoveAttack2();
                        attackProcess = 0;
                    }
                }
            }
        } else {
            if (nowST < GetCost(CostType.Skill)) {
                nowST = GetCost(CostType.Skill);
            }
            if (AttackBase(4, 2.2f, 2.5f, GetCost(CostType.Skill), 20f / 30f / 1.2f, 38f / 30f / 1.2f, 0f, 1.2f)) {
                S_ParticlePlay(0);
                gravityMultiplier = 2f;
                isJumpingAttack = true;
                movingFlag = true;
                // SpecialStep(0f, 0.25f / spRate, 4f, 0f, 0f, true, true);
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

    protected override void Update_Transition_Jump() {
        base.Update_Transition_Jump();
        if (targetTrans && GetCanControl() && (command == Command.Default || command == Command.Free)) {
            float sqrDist = GetTargetDistance(true, true, true);
            if (state != State.Attack && sqrDist < 2f * 2f && attackedTimeRemain < 0) {
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
}
