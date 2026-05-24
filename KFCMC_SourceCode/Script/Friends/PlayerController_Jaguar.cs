using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class PlayerController_Jaguar : PlayerController
{

    public XWeaponTrail jumpTrail;
    public Transform movePivot;

    bool jumpTrailEnabled;
    bool movingFlag;
    int jumpingAttackState;

    const int effectJump = 0;
    const float spBias = 1.1f;

    protected override void Start()
    {
        base.Start();
        jumpTrail.Init();
        jumpTrail.Deactivate();
        jumpTrailEnabled = false;
        moveCost.attack = 14f * staminaCostRate;
        moveCost.skill = 40f;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 1.88f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
        justDodgeCounterAttackProcess = 2;
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

    void SetCombo2()
    {
        SetAttackPowerMultiplier(1.9f);
        SetKnockPowerMultiplier(2.3f);
    }

    void MoveAttack()
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

    protected override void AttackBody()
    {
        float spRate = isSuperman ? 4f / 3f : 1;
        Vector3 targetTemp = trans.position;
        float sqrDist = GetTargetDistance(true, true, true);
        movingFlag = false;
        isSkillAttacking = false;
        if (jumpingAttackState != 1)
        {
            if (JudgeStamina(GetCost(CostType.Jump) + GetCost(CostType.Skill)) && !GetSick(SickType.Mud) && playerInput.GetButton(RewiredConsts.Action.Special))
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
                SuperarmorStart();
                jumpingAttackState = 1;
            }
            else
            {
                jumpingAttackState = 0;
                if (attackProcess >= 2 && !JudgeStamina(GetCost(CostType.Attack) * ((60f / spBias) / 14f)))
                {
                    attackProcess = 0;
                }
                if (attackProcess < 2)
                {
                    if (AttackBase(attackProcess, 1f, 1f, GetCost(CostType.Attack), 14f / 30f / spRate, 14f / 30f / spRate, 1f, spRate))
                    {
                        S_ParticlePlay(attackProcess);
                        MoveAttack();
                        attackingMoveReservedTimer = 11f / 30f / spRate;
                        attackingDodgeReservedTimer = 11f / 30f / spRate;
                        AttackStartAir();
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                }
                else
                {
                    if (AttackBase(2, 1.35f, 1.6f, GetCost(CostType.Attack) * ((60f / spBias) / 14f), 60f / 30f / spBias / spRate, 60f / 30f / spBias / spRate, 1f, spBias * spRate))
                    {
                        S_ParticlePlay(2);
                        MoveAttack();
                        attackingMoveReservedTimer = 40f / 30f / spBias / spRate;
                        attackingDodgeReservedTimer = 14f / 30f / spBias / spRate;
                        AttackStartAir();
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                }
                attackProcess = (attackProcess + 1) % 3;
            }
        }
        else
        {
            if (nowST < GetCost(CostType.Skill))
            {
                nowST = GetCost(CostType.Skill);
            }
            if (AttackBase(4, 2.2f, 2.5f, GetCost(CostType.Skill), 20f / 30f / 1.2f, 38f / 30f / 1.2f, 0f, 1.2f))
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

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false)
    {
        if (jumpingAttackState != 0 && damage > 0)
        {
            damage = Mathf.Max(damage / 2, 1);
        }
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
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
