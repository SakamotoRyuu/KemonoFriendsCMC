using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class PlayerController_Tsuchinoko : PlayerController
{

    public LookatTarget lookatTarget;
    public Transform nullTarget;
    LaserOption laserOption;

    protected override void Awake()
    {
        base.Awake();
        if (animatorForBattle)
        {
            moveCost.attack = 28f * staminaCostRate;
            moveCost.skill = 120;
            laserOption = GetComponent<LaserOption>();
            lookatTarget.enabled = true;
            sandstarHealMultiplier = justDodgeAmountMultiplier = 1f;
            if (attackPower > 0)
            {
                sandstarHealMultiplier *= 10f / attackPower;
            }
        }
    }

    public override void ResetStatus()
    {
        base.ResetStatus();
        if (laserOption)
        {
            laserOption.CancelLaser();
        }
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (state != State.Attack && laserOption)
        {
            laserOption.CancelLaser();
        }
        if (state != State.Attack && !isSuperman && supermanSettings.isSpecial)
        {
            LightEyeActivate(0);
        }
    }

    protected override void Update_Targeting()
    {
        base.Update_Targeting();
        if (lookatTarget)
        {
            if (targetTrans)
            {
                lookatTarget.SetTarget(targetTrans);
            }
            else
            {
                lookatTarget.SetTarget(nullTarget);
            }
        }
    }

    protected void LightEyeActivate(int param)
    {
        if (!isSuperman)
        {
            SupermanSetMaterial(param != 0);
        }
    }

    void MoveAttack()
    {
        if (targetTrans && groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float sqrDist = GetTargetDistance(true, true, true);
            if (sqrDist < 4f * 4f)
            {
                fbStepMaxDist = 4f;
                fbStepTime = 11f / 30f;
                SeparateFromTarget(4f);
            }
            else
            {
                SpecialStep(4.2f, 11f / 30f, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
            }
        }
    }

    protected override void AttackBody()
    {
        float spRate = isSuperman ? 4f / 3f : 1;
        float sqrDist = GetTargetDistance(true, false, true);
        float skillCost = GetCost(CostType.Skill);
        if (playerInput.GetButton(RewiredConsts.Action.Special) && JudgeStamina(skillCost))
        {
            if (AttackBase(1, 1f, 1f, skillCost, 98f / 30f, 98f / 30f, 0f, 1f))
            {
                S_ParticlePlay(1);
                fbStepMaxDist = 2f;
                fbStepTime = 0.25f;
                if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    SeparateFromTarget(4f);
                }
                isSkillAttacking = true;
                attackingMoveReservedTimer = 90f / 30f;
                attackingDodgeReservedTimer = 80f / 30f;
                AttackStartAir();
            }
        }
        else
        {
            if (AttackBase(0, 1.2f, 2f, GetCost(CostType.Attack), 28f / 30f / spRate, 28f / 30f / spRate, 1f, spRate))
            {
                S_ParticlePlay(0);
                if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    SpecialStep(0.4f, 0.25f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                }
                isSkillAttacking = false;
                attackingMoveReservedTimer = 20f / 30f / spRate;
                attackingDodgeReservedTimer = 14f / 30f / spRate;
                AttackStartAir();
                nowSpeed = Mathf.Min(nowSpeed, 3f);
            }
        }
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        if (attackType != 1)
        {
            AttackContinuousAir();
        }
    }

    private void LaserReady()
    {
        if (!isItem)
        {
            AttackStart(2);
            laserOption.LightFlickeringChargeStart();
            LightEyeActivate(1);
        }
    }

    private void LaserStart()
    {
        if (!isItem)
        {
            AttackEnd(2);
            laserOption.LightFlickeringChargeEnd();
            AttackStart(1);
        }
    }

    private void LaserEnd()
    {
        if (!isItem)
        {
            AttackEnd(1);
            laserOption.LightFlickeringBlastEnd();
            LightEyeActivate(0);
            LockonEnd();
        }
    }
}
