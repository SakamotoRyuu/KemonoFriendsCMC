using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class PlayerController_GrayWolf : PlayerController
{
    public XWeaponTrail jumpTrail;
    public Transform movePivot;

    bool jumpTrailEnabled = false;
    bool movingFlag;
    int jumpingAttackState;
    const int effectJump = 0;
    const string specialAttackFaceName = "Attack2";
    const float spBias = 1.2f;

    protected override void Start()
    {
        base.Start();
        jumpTrail.Init();
        jumpTrail.Deactivate();
        jumpTrailEnabled = false;
        moveCost.attack = 14f * staminaCostRate;
        moveCost.skill = 60f;
        specialMoveDirectionAdjustEnabled = true;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 1.74f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
        justDodgeCounterAttackProcess = 2;
    }

    public override void SetForItem()
    {
        jumpTrail.gameObject.SetActive(false);
        base.SetForItem();
    }

    public override void EmitEffectString(string type)
    {
        switch (type)
        {
            case "Jump":
                EmitEffect(effectJump);
                break;
        }
    }

    void Howling()
    {
        if (throwing)
        {
            throwing.ThrowReady(0);
        }
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if ((state != State.Jump && state != State.Attack) || (state == State.Jump && jumpType != 2))
        {
            jumpingAttackState = 0;
            movingFlag = false;
            gravityMultiplier = 1f;
            if (jumpTrailEnabled)
            {
                jumpTrail.StopSmoothly(0.1f);
                jumpTrailEnabled = false;
            }
        }
    }

    void MoveEnd()
    {
        movingFlag = false;
        LockonEnd();
    }

    protected override void SetFace(int faceIndex)
    {
        if (faceIndex == this.faceIndex[(int)FaceName.Attack] && attackType == 5)
        {
            fCon.SetFace(specialAttackFaceName);
        }
        else
        {
            base.SetFace(faceIndex);
        }
    }

    void MoveAttack()
    {
        if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1;
            SpecialStep(0.3f, 0.25f / spRate, 4f, 0.05f, 0.4f, true, true, EasingType.SineInOut, true);
        }
    }

    void MoveAttack2()
    {
        if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1;
            SpecialStep(0.4f, 0.25f / spRate, 4f, 0.05f, 0.4f, true, true, EasingType.SineInOut, true);
        }
    }

    float GetTargetHeight()
    {
        if (targetTrans)
        {
            return targetTrans.position.y - trans.position.y;
        }
        return 0f;
    }

    protected override void SetJumpAnim()
    {
        if (jumpingAttackState != 1 && JudgeStamina(GetCost(CostType.Jump) + GetCost(CostType.Skill) * 0.5f) && !GetSick(SickType.Mud) && playerInput.GetButton(RewiredConsts.Action.Special))
        {
            anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 2);
            jumpTrailEnabled = true;
            jumpTrail.Activate();
            S_ParticlePlay(0);
            if (groundedFlag)
            {
                Jump(Mathf.Clamp(4.2f + GetTargetHeight(), 5.6f, 8f));
            }
            else
            {
                Jump(move.y);
            }
            gravityMultiplier = 0.8f;
            attackedTimeRemain = 0.3f;
            jumpingAttackState = 1;
        }
        else
        {
            base.SetJumpAnim();
        }
    }

    protected override void AttackBody()
    {
        float spRate = isSuperman ? 4f / 3f : 1;
        Vector3 targetTemp = trans.position;
        float sqrDist = GetTargetDistance(true, true, true);
        movingFlag = false;
        isSkillAttacking = false;
        if (jumpingAttackState != 1)
        {
            if (playerInput.GetButton(RewiredConsts.Action.Special))
            {
                if (AttackBase(5, 1f, 1f, GetCost(CostType.Skill), 76f / 30f / spRate, 76f / 30f / spRate, 0, spRate, false))
                {
                    attackingMoveReservedTimer = 68f / 30f / spRate;
                    attackingDodgeReservedTimer = 42f / 30f / spRate;
                    isSkillAttacking = true;
                }
            }
            else
            {
                jumpingAttackState = 0;
                if (attackProcess >= 2 && !JudgeStamina(GetCost(CostType.Attack) * (20f / 14f)))
                {
                    attackProcess = 0;
                }
                if (attackProcess == 0)
                {
                    if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 14f / 30f / spRate, 14f / 30f / spRate, 1f, spRate))
                    {
                        S_ParticlePlay(0);
                        MoveAttack2();
                        attackProcess = 1;
                        attackingMoveReservedTimer = 11f / 30f / spRate;
                        attackingDodgeReservedTimer = 11f / 30f / spRate;
                        AttackStartAir();
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                }
                else if (attackProcess == 1)
                {
                    if (AttackBase(1, 1f, 1f, GetCost(CostType.Attack), 14f / 30f / spRate, 14f / 30f / spRate, 1f, spRate))
                    {
                        S_ParticlePlay(1);
                        MoveAttack2();
                        attackProcess = 2;
                        attackingMoveReservedTimer = 11f / 30f / spRate;
                        attackingDodgeReservedTimer = 11f / 30f / spRate;
                        AttackStartAir();
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                }
                else if (attackProcess == 2)
                {
                    if (AttackBase(2, 0.9f, 0.7f, GetCost(CostType.Attack) * (20f / 14f), 20f / 30f / spRate, 20f / 30f / spRate, 1f, spRate))
                    {
                        S_ParticlePlay(0);
                        S_ParticlePlay(1);
                        MoveAttack();
                        attackProcess = 3;
                        attackingMoveReservedTimer = 18f / 30f / spRate;
                        attackingDodgeReservedTimer = 13f / 30f / spRate;
                        AttackStartAir();
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                }
                else
                {
                    if (AttackBase(3, 2f, 2.4f, GetCost(CostType.Attack) * ((41f / spBias) / 14f), 41f / 30f / spBias / spRate, 41f / 30f / spBias / spRate, 1f, spBias * spRate))
                    {
                        S_ParticlePlay(2);
                        MoveAttack2();
                        attackProcess = 0;
                        attackingMoveReservedTimer = (41f - 8f * spBias) / 30f / spBias / spRate;
                        attackingDodgeReservedTimer = 24f / 30f / spBias / spRate;
                        AttackStartAir();
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                }
            }
        }
        else
        {
            nowST = Mathf.Max(nowST, GetCost(CostType.Skill) * 0.5f);
            if (AttackBase(4, 2.2f, 2.5f, GetCost(CostType.Skill) * 0.5f, 20f / 30f / 1.2f, 38f / 30f / 1.2f, 0f, 1.2f))
            {
                S_ParticlePlay(0);
                gravityMultiplier = 2f;
                movingFlag = true;
                attackProcess = 0;
                isSkillAttacking = true;
                jumpingAttackState = 2;
                attackingMoveReservedTimer = 20f / 30f / 1.2f;
                AttackStartAir();
            }
        }
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        if (attackType == 4 && movingFlag)
        {
            if (targetTrans && !playerInput.GetButton(RewiredConsts.Action.Dodge))
            {
                move.y = 0f;
                ApproachTransformPivot(movePivot, 15f, targetRadius + 0.3f, targetRadius, false);
            }
        }
        else
        {
            AttackContinuousAir();
        }
    }

    protected override void Update_Transition_Jump()
    {
        base.Update_Transition_Jump();
        if (targetTrans && GetCanControl())
        {
            float sqrDist = GetTargetDistance(true, true, true);
            if (state != State.Attack && sqrDist < 2f * 2f && attackedTimeRemain <= 0 && jumpingAttackState == 1)
            {
                SetState(State.Attack);
            }
        }
    }

    protected override void Update_Process_Jump()
    {
        base.Update_Process_Jump();
        if (targetTrans && GetCanControl() && jumpingAttackState == 1)
        {
            lockonRotSpeed = 15f;
            CommonLockon();
            Continuous_Approach(11f, 0.4f, 0.2f, true, true);
        }
    }

}
