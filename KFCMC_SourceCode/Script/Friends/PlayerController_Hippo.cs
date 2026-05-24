using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Hippo : PlayerController
{

    public CheckTriggerStay checkTriggerStay;



    private float counterTimeRemain;
    private float coolTimeRemain;
    //private int fatiguePoint;

    protected override void Awake()
    {
        base.Awake();
        moveCost.attack = 30f * staminaCostRate;
        moveCost.skill = 30f * staminaCostRate;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 3.66f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
    }

    protected override void Update_TimeCount()
    {
        base.Update_TimeCount();
        if (counterTimeRemain > 0f)
        {
            counterTimeRemain -= deltaTimeMove;
        }
        if (coolTimeRemain > 0f)
        {
            coolTimeRemain -= deltaTimeMove;
        }
        if (CharacterManager.Instance)
        {
            CharacterManager.Instance.SetSkillPercentage(coolTimeRemain <= 0f ? 100 : 100 - (int)(coolTimeRemain / 3.5f * 100f));
        }
    }

    protected override void KnockLightProcess()
    {
        base.KnockLightProcess();
        if (!isSuperarmor && counterTimeRemain > 0f)
        {
            counterTimeRemain = 0f;
        }
    }

    protected override void KnockHeavyProcess()
    {
        base.KnockHeavyProcess();
        if (counterTimeRemain > 0f)
        {
            counterTimeRemain = 0f;
        }
    }

    //protected override void DamageCommonProcess()
    //{
    //    base.DamageCommonProcess();
    //    fatiguePoint -= 5;
    //    if (fatiguePoint < 0)
    //    {
    //        fatiguePoint = 0;
    //    }
    //}

    public override void SuperarmorEnd()
    {
        if (counterTimeRemain <= 0f)
        {
            isSuperarmor = false;
        }
    }

    protected override void SideStep_Head(int direction, float mutekiTime = 0, bool changeState = true)
    {
        if (counterTimeRemain > 0f)
        {
            changeState = false;
        }
        base.SideStep_Head(direction, mutekiTime, changeState);
    }

    protected override void SideStep_Move(Vector3 dir, float distance = 5)
    {
        if (counterTimeRemain <= 0f)
        {
            base.SideStep_Move(dir, distance);
        }
    }

    void MoveAttack()
    {
        if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1f;
            SpecialStep(0.3f, 8f / 30f / spRate, 5f, 0.05f, 0.5f, true, true, EasingType.SineInOut, true);
        }
    }

    //public override void ResetStatus()
    //{
    //    base.ResetStatus();
    //    fatiguePoint = 0;
    //}

    protected override void AttackBody()
    {
        float spRate = isSuperman ? 4f / 3f : 1f;
        if (counterTimeRemain > 0)
        {
            attackProcess = 0;
            if (AttackBase(3, 1.2f, 1.5f, GetCost(CostType.Attack), 27f / 30f / spRate, 27f / 30f / spRate, 1f, spRate, true, 40f))
            {
                PerfectLockon();
                if (!playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    SpecialStep(0.3f, 7f / 30f / spRate, 5f, 0.05f, 0.5f, true, true, EasingType.SineInOut, true);
                }
                S_ParticlePlay(0);
                SuperarmorStart();
                counterTimeRemain = 0f;
                isSkillAttacking = false;
                attackingMoveReservedTimer = 19f / 30f / spRate;
                attackingDodgeReservedTimer = 16f / 30f / spRate;
                AttackStartAir();
                nowSpeed = Mathf.Min(nowSpeed, 9f);
            }
        }
        else
        {
            if (AttackBase(attackProcess, 1f, 1f, GetCost(CostType.Attack), 30f / 30f / spRate, 30f / 30f / spRate, 1f, spRate))
            {
                S_ParticlePlay(0);
                isSkillAttacking = false;
                attackingMoveReservedTimer = 22f / 30f / spRate;
                attackingDodgeReservedTimer = 19f / 30f / spRate;
                AttackStartAir();
                nowSpeed = Mathf.Min(nowSpeed, 9f);
            }
            attackProcess = (attackProcess + 1) % 2;
        }
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        AttackContinuousAir();
    }

    public override void CounterAttack(GameObject attackerObject = null, bool isProjectile = false)
    {
        if (!isProjectile && counterTimeRemain <= 0f && coolTimeRemain <= 0f && checkTriggerStay.stayFlag && GetCanMove() && JudgeStamina(GetCost(CostType.Skill)))
        {
            if (!playerInput.GetButton(RewiredConsts.Action.Special))
            {
                return;
            }
            base.CounterAttack(attackerObject);
            SetState(State.Wait);
            state = State.Attack;
            if (AttackBase(2, 1f, 1f, GetCost(CostType.Skill), 12f / 30f, 12f / 30f, 0f, 1f, true, 15f))
            {
                PerfectLockon();
                S_ParticlePlay(0);
                S_ParticlePlay(1);
                AttackStart(2);
                SuperarmorStart();
                attackProcess = 0;
                EmitEffect(0);
                counterTimeRemain = 1.5f;
                // coolTimeRemain = 1.5f + fatiguePoint * 0.2f;
                coolTimeRemain = 3.5f;
                specialMoveDuration = 0f;
                //if (fatiguePoint < 10)
                //{
                //    fatiguePoint++;
                //}
                isSkillAttacking = true;
                attackingMoveReservedTimer = 9f / 30f;
                attackingDodgeReservedTimer = 9f / 30f;
            }
        }
    }

}
