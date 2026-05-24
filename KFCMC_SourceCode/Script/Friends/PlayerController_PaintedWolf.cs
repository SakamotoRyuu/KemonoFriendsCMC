using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_PaintedWolf : PlayerController
{

    public ParticleSystem rushParticle;
    public ParticleSystem upperParticle;
    public Transform movePivot;

    bool continuousMove = false;
    float skillAttackCount;
    int attackSave = -1;
    float targetLostTime = 0f;
    string[] chatKey_Sp = new string[3];
    bool isUpperReserved;
    const float skillAttackMax = 16f;
    const int skillAttackIndex = 8;
    const int upperAttackIndex = 9;
    const int upperEffectIndex = 2;

    protected override void Awake()
    {
        base.Awake();
        moveCost.attack = 30f * staminaCostRate;
        moveCost.skill = 95f;
        skillAttackCount = skillAttackMax;
        chatAttackCount = 6;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 1.10f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
    }

    protected override void ChatKeyInit()
    {
        base.ChatKeyInit();
        chatKey_Sp[0] = StringUtils.Format("{0}{1}_SP_00", talkHeader, talkName);
        chatKey_Sp[1] = StringUtils.Format("{0}{1}_SP_01", talkHeader, talkName);
        chatKey_Sp[2] = StringUtils.Format("{0}{1}_SP_02", talkHeader, talkName);
    }

    public override void ResetStatus()
    {
        base.ResetStatus();
        skillAttackCount = skillAttackMax;
        UpdateSkillPercentage();
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (state != State.Attack)
        {
            ActivateRushParticle(false);
            ActivateUpperParticle(false);
        }
    }

    void MoveAttack(int index)
    {
        float spRate = isSuperman ? 4f / 3f : 1f;
        switch (index)
        {
            case 0:
                if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    SpecialStep(0.35f, 0.2f / spRate, 3.5f, 0f, 0f, true, true, EasingType.SineInOut, true);
                    continuousMove = true;
                }
                break;
            case 1:
                if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    SpecialStep(0.45f, 0.2f / spRate, 3.5f, 0f, 0f, true, true, EasingType.SineInOut, true);
                    continuousMove = true;
                }
                break;
            case 2:
                if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    SeparateFromTarget(2.5f);
                }
                break;
            case 3:
                continuousMove = true;
                targetLostTime = 0f;
                ActivateRushParticle(true);
                break;
            case 4:
                continuousMove = false;
                ActivateRushParticle(false);
                break;
            case 5:
                continuousMove = false;
                break;
            case 6:
                ActivateUpperParticle(true);
                break;
            case 7:
                ActivateUpperParticle(false);
                break;
        }
    }

    void ActivateRushParticle(bool flag)
    {
        if (enabled && rushParticle)
        {
            if (flag)
            {
                rushParticle.Play();
            }
            else if (!flag && rushParticle.isPlaying)
            {
                rushParticle.Stop();
            }
        }
    }

    void ActivateUpperParticle(bool flag)
    {
        if (enabled && upperParticle)
        {
            if (flag)
            {
                upperParticle.Play();
            }
            else if (!flag && upperParticle.isPlaying)
            {
                upperParticle.Stop();
            }
        }
    }

    void ChangeAttack(int index)
    {
        switch (index)
        {
            case 0:
                attackPowerMultiplier = 1.3f;
                knockPowerMultiplier = 1.4f;
                break;
            case 1:
                attackPowerMultiplier = 1.1f;
                knockPowerMultiplier = 1.14f;
                break;
            case 3:
                attackPowerMultiplier = 1.1f;
                knockPowerMultiplier = 1.14f;
                break;
            case 4:
                attackPowerMultiplier = 1.25f;
                knockPowerMultiplier = 1.32f;
                break;
            case 5:
                attackPowerMultiplier = 1.1f;
                knockPowerMultiplier = 1.14f;
                break;
            case 6:
                attackPowerMultiplier = 1.2f;
                knockPowerMultiplier = 1.25f;
                break;
            case 7:
                attackPowerMultiplier = 1.1f;
                knockPowerMultiplier = 1.14f;
                break;
            case 8:
                attackPowerMultiplier = 1.1f;
                knockPowerMultiplier = 1.14f;
                break;
            case 9:
                attackPowerMultiplier = 3.5f;
                knockPowerMultiplier = 4.5f;
                break;
        }
    }

    protected override void PlayerJump()
    {
        if (playerInput.GetButton(RewiredConsts.Action.Special))
        {
            lastJumpButtonTime = 100f;
            lastAttackButtonTime = 100f;
            isUpperReserved = true;
            SetState(State.Attack);
        }
        else
        {
            base.PlayerJump();
        }
    }

    protected override void AttackBody()
    {
        float spRate = (isSuperman ? 4f / 3f : 1f);
        int attackTemp;
        resetAgentRadiusOnChangeState = true;
        continuousMove = false;
        isSkillAttacking = false;
        if (isUpperReserved)
        {
            attackTemp = 9;
            isUpperReserved = false;
        }
        else if (skillAttackCount <= 0f && JudgeStamina(GetCost(CostType.Skill)) && playerInput.GetButton(RewiredConsts.Action.Special))
        {
            attackTemp = skillAttackIndex;
        }
        else
        {
            attackTemp = Random.Range(0, 8);
            if (attackTemp == attackSave)
            {
                attackTemp = (attackTemp + 1) % 8;
            }
        }
        attackSave = attackTemp;
        switch (attackTemp)
        {
            case 0:
                if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack) * (68f / 60f), 68f / 60f / spRate, 68f / 60f / spRate, 1f, spRate))
                {
                    skillAttackCount -= (1f + 68f / 60f);
                    attackingMoveReservedTimer = (68f - 16f) / 60f / spRate;
                    attackingDodgeReservedTimer = 15f / 60f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
                break;
            case 1:
                if (AttackBase(1, 1f, 1f, GetCost(CostType.Attack) * (66f / 60f), 66f / 60f / spRate, 66f / 60f / spRate, 1f, spRate))
                {
                    skillAttackCount -= (1f + 66f / 60f);
                    attackingMoveReservedTimer = (66f - 16f) / 60f / spRate;
                    attackingDodgeReservedTimer = 15f / 60f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
                break;
            case 2:
                if (AttackBase(2, 1f, 1f, GetCost(CostType.Attack) * (94f / 60f), 94f / 60f / spRate, 94f / 60f / spRate, 1f, spRate))
                {
                    skillAttackCount -= (1f + 94f / 60f);
                    attackingMoveReservedTimer = (94f - 16f) / 60f / spRate;
                    attackingDodgeReservedTimer = 15f / 60f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
                break;
            case 3:
                if (AttackBase(3, 1f, 1f, GetCost(CostType.Attack) * (68f / 60f), 68f / 60f / spRate, 68f / 60f / spRate, 1f, spRate))
                {
                    skillAttackCount -= (1f + 68f / 60f);
                    attackingMoveReservedTimer = (68f - 16f) / 60f / spRate;
                    attackingDodgeReservedTimer = 15f / 60f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
                break;
            case 4:
                if (AttackBase(4, 1f, 1f, GetCost(CostType.Attack), 60f / 60f / spRate, 60f / 60f / spRate, 1f, spRate))
                {
                    skillAttackCount -= (1f + 60f / 60f);
                    attackingMoveReservedTimer = (60f - 16f) / 60f / spRate;
                    attackingDodgeReservedTimer = 15f / 60f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
                break;
            case 5:
                if (AttackBase(5, 0.92f, 0.9f, GetCost(CostType.Attack), 60f / 60f / spRate, 60f / 60f / spRate, 1f, spRate))
                {
                    skillAttackCount -= (1f + 60f / 60f);
                    attackingMoveReservedTimer = (60f - 16f) / 60f / spRate;
                    attackingDodgeReservedTimer = 15f / 60f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
                break;
            case 6:
                if (AttackBase(6, 1.1f, 1.14f, GetCost(CostType.Attack) * (62f / 60f), 62f / 60f / spRate, 62f / 60f / spRate, 1f, spRate))
                {
                    skillAttackCount -= (1f + 62f / 60f);
                    attackingMoveReservedTimer = (62f - 16f) / 60f / spRate;
                    attackingDodgeReservedTimer = 15f / 60f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
                break;
            case 7:
                if (AttackBase(7, 1f, 1f, GetCost(CostType.Attack) * (66f / 60f), 66f / 60f / spRate, 66f / 60f / spRate, 1f, spRate))
                {
                    skillAttackCount -= (1f + 66f / 60f);
                    attackingMoveReservedTimer = (66f - 16f) / 60f / spRate;
                    attackingDodgeReservedTimer = 15f / 60f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
                break;
            case 8:
                if (AttackBase(8, 1.6f, 1.8f, GetCost(CostType.Attack), 265f / 60f, 280f / 60f, 1f, 1f))
                {
                    continuousMove = false;
                    SuperarmorStart();
                    knockRemain = GetHeavyKnockEndurance();
                    knockRemainLight = GetLightKnockEndurance();
                    skillAttackCount = skillAttackMax * 0.25f;
                    isSkillAttacking = true;
                    attackingMoveReservedTimer = 265f / 60f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
                break;
            case 9:
                if (AttackBase(9, 2.4f, 3f, GetCost(CostType.Attack) * (4f / 3f), 31f / 30f / 0.8f / spRate, 31f / 30f / 0.8f / spRate, 0f, 0.8f * spRate, true, attackLockonDefaultSpeed * 20f))
                {
                    skillAttackCount -= (1f + 31f / 30f / 0.8f);
                    continuousMove = true;
                    EmitEffect(upperEffectIndex);
                    SetMutekiTime(0.4f);
                    if (agent)
                    {
                        agent.radius = 0.1f;
                    }
                    isSkillAttacking = true;
                    attackingMoveReservedTimer = 26f / 30f / 0.8f / spRate;
                    attackingDodgeReservedTimer = 23f / 30f / 0.8f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
                break;
        }
        UpdateSkillPercentage();
    }

    void NoCostDodge()
    {
        attackStiffTime = 0f;
        attackedTimeRemain = 0f;
        SetState(State.Wait);
        float costSave = dodgeCost;
        dodgeCost = 0f;
        SideStep(0, dodgeDistance, dodgeMutekiTime, true);
        dodgeCost = costSave;
    }

    void SkillTrigger()
    {
        if (enabled)
        {
            if (!target && playerInput.GetButton(RewiredConsts.Action.Dodge))
            {
                NoCostDodge();
            }
            else
            {
                EmitEffect(0);
                skillAttackCount = skillAttackMax;
                nowST -= (GetCost(CostType.Skill) - GetCost(CostType.Attack));
                targetLostTime = 0f;
                if (nowST < 0f)
                {
                    nowST = 0f;
                }
                UpdateSkillPercentage();
            }
        }
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        if (attackType == skillAttackIndex)
        {
            if (isSuperarmor)
            {
                SetMutekiTime(0.4f);
            }
            if (continuousMove)
            {
                if (target && !playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    targetLostTime = 0f;
                    Continuous_Approach(Mathf.Max(GetMaxSpeed(false, false, false, true), 16.875f), 0.35f, 0.25f, true, true);
                }
                else
                {
                    targetLostTime += deltaTimeCache;
                    if (targetLostTime >= 0.25f && playerInput.GetButton(RewiredConsts.Action.Dodge))
                    {
                        NoCostDodge();
                    }
                }
            }
            AttackContinuousAir();
        }
        else if (attackType == upperAttackIndex)
        {
            if (continuousMove && target && !playerInput.GetButton(RewiredConsts.Action.Dodge))
            {
                ApproachTransformPivot(movePivot, 16.875f * (isSuperman ? 4f / 3f : 1f), 0.3f, 0.03f, true, 0.4f);
            }
        }
        else
        {
            if (continuousMove && target && !playerInput.GetButton(RewiredConsts.Action.Dodge))
            {
                Continuous_Approach(6f, 0.5f, 0.3f, true, true);
            }
            AttackContinuousAir();
        }
    }

    void UpdateSkillPercentage()
    {
        if (CharacterManager.Instance)
        {
            CharacterManager.Instance.SetSkillPercentage(Mathf.FloorToInt((skillAttackMax - skillAttackCount) / skillAttackMax * 100f));
        }
    }
}
