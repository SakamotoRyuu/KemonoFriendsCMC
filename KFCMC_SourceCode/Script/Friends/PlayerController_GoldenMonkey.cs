using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class PlayerController_GoldenMonkey : PlayerController
{

    public Transform[] checkScaleTarget;
    public LookatTarget lookatTarget;
    public Transform nullTarget;

    bool lookatFlag;
    Transform lookatSave;
    Vector3[] checkScaleTargetDefaultPosition;
    Quaternion[] checkScaleTargetDefaultRotation;
    Vector3[] checkScaleTargetDefaultScale;
    float skillAttackCount;
    int attackSave;
    Vector2 lookatRotationRange = new Vector2(60f, 0f);
    const float skillAttackMax = 20f;
    const int skillAttackIndex = 5;
    const int effSkillReady = 0;
    const int effLengthenNormalPar = 2;
    const int effLengthenNormalSE = 3;
    const int effLengthenSuperPar = 4;
    const int effLengthenSuperSE = 5;
    const float ats1 = 34f / 29f;
    const float ats2 = 1f;
    const float ats3 = 0.8f;


    protected override void Awake()
    {
        base.Awake();
        checkScaleTargetDefaultPosition = new Vector3[checkScaleTarget.Length];
        checkScaleTargetDefaultRotation = new Quaternion[checkScaleTarget.Length];
        checkScaleTargetDefaultScale = new Vector3[checkScaleTarget.Length];
        for (int i = 0; i < checkScaleTarget.Length; i++)
        {
            if (checkScaleTarget[i])
            {
                checkScaleTargetDefaultPosition[i] = checkScaleTarget[i].localPosition;
                checkScaleTargetDefaultRotation[i] = checkScaleTarget[i].localRotation;
                checkScaleTargetDefaultScale[i] = checkScaleTarget[i].localScale;
            }
        }
        if (lookatTarget)
        {
            lookatTarget.enabled = true;
        }
        moveCost.attack = 28f * staminaCostRate;
        moveCost.skill = 50f * staminaCostRate;
        notResetAttackProcessOnDamage = true;
        skillAttackCount = skillAttackMax;
        chatAttackCount = 6;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 2.56f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
        justDodgeCounterAttackProcess = 2;
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
        if (lookatFlag && (state != State.Attack || lookatSave == null || lookatSave != targetTrans))
        {
            LookatEnd();
        }
    }

    void LookatStart()
    {
        if (targetTrans)
        {
            LockonStart();
            lookatFlag = true;
            lookatSave = targetTrans;
            lookatTarget.SetTarget(targetTrans);
            lookatTarget.SetFollowSpeed(isSuperman || attackType == skillAttackIndex ? 0.03f : 0.04f);
            lookatRotationRange.x = (attackType == skillAttackIndex ? 180f : 90f);
            lookatTarget.SetRotationRange(lookatRotationRange);
        }
    }

    void LookatEnd()
    {
        LockonEnd();
        lookatFlag = false;
        lookatTarget.SetTarget(nullTarget);
        lookatTarget.ResetFollowVelocity();
        lookatTarget.SetFollowSpeed(0.5f);
        lookatRotationRange.x = 90f;
        lookatTarget.SetRotationRange(lookatRotationRange);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        for (int i = 0; i < checkScaleTarget.Length; i++)
        {
            if (checkScaleTarget[i])
            {
                checkScaleTarget[i].localPosition = checkScaleTargetDefaultPosition[i];
                checkScaleTarget[i].localRotation = checkScaleTargetDefaultRotation[i];
                checkScaleTarget[i].localScale = checkScaleTargetDefaultScale[i];
            }
        }
    }

    void AfterAttackMove()
    {
        if (targetTrans && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1f;
            switch (attackType)
            {
                case 0:
                    fbStepTime = 7f / 30f / spRate;
                    fbStepMaxDist = 13.5f * 7f / 30f;
                    break;
                case 1:
                case 5:
                    fbStepTime = 12f / 30f / spRate;
                    fbStepMaxDist = 13.5f * 12f / 30f;
                    break;
                case 4:
                    fbStepTime = 8f / 30f / spRate;
                    fbStepMaxDist = 13.5f * 8f / 30f;
                    break;
            }
            EscapeFromTarget(Random.Range(3.4f, 4.4f));
        }
    }

    void EscapeFromTarget(float distanceToTarget)
    {
        if (targetTrans && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float dist = GetTargetDistance(false, fbStepIgnoreY, fbStepConsiderRadius);
            if (dist < distanceToTarget)
            {
                Vector3 escapeDestination = GetEscapeDestination(searchArea[0].GetTargetsAveragePosition(), distanceToTarget);
                escapeDestination.y = trans.position.y;
                SetSpecialMove((escapeDestination - trans.position).normalized, Mathf.Clamp(distanceToTarget - dist, 0f, fbStepMaxDist), fbStepTime, fbStepEaseType);
            }
        }
    }

    void EmitEffectLengthen(int isSuper)
    {
        if (state == State.Attack)
        {
            if (isSuper == 0)
            {
                EmitEffect(effLengthenNormalPar);
                EmitEffect(effLengthenNormalSE);
            }
            else
            {
                EmitEffect(effLengthenSuperPar);
                EmitEffect(effLengthenSuperSE);
            }
        }
    }

    protected override void AttackBody()
    {
        float sqrDist = GetTargetDistance(true, true, true);
        float spRate = isSuperman ? 4f / 3f : 1f;
        float heightOffset = targetTrans ? Mathf.Clamp01((GetTargetHeight(false) - 1.4f) * 0.5f) : 0f;
        float isLion = CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Multi) ? 1f : heightOffset;
        bool skillFlag = (skillAttackCount <= 0f && axisInput.y > 0.5f && JudgeStamina(GetCost(CostType.Skill) * 2.4f));
        fbStepTime = 0.25f / spRate;
        fbStepMaxDist = 4f;
        fbStepConsiderRadius = true;
        isSkillAttacking = false;
        if (playerInput.GetButton(RewiredConsts.Action.Special))
        {
            if (skillFlag)
            {
                AttackBase(skillAttackIndex, 3.0f, 40f / 11f, GetCost(CostType.Skill) * 2.4f, 118f / 60f, 118f / 60f, 0, 1f, true, attackLockonDefaultSpeed * 4f);
                SuperarmorStart();
                knockRemain = GetHeavyKnockEndurance();
                knockRemainLight = GetLightKnockEndurance();
                skillAttackCount = skillAttackMax;
                EmitEffect(effSkillReady);
                SetMutekiTime(1f);
                if (!playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    ApproachOrSeparate(5f);
                }
                attackingMoveReservedTimer = 102f / 60f;
                attackingDodgeReservedTimer = 82f / 60f;
                AttackStartAir();
                nowSpeed = Mathf.Min(nowSpeed, 4.5f);
                isSkillAttacking = true;
            }
            else if (JudgeStamina(GetCost(CostType.Attack) * (20f / 14f)))
            {
                if (AttackBase(4, 1.2f, 1.4f, GetCost(CostType.Attack) * (20f / 14f), 40f / 30f / spRate, 40f / 30f / spRate, 1f, spRate))
                {
                    skillAttackCount -= (1f + 40f / 30f);
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge))
                    {
                        SpecialStep(6f - isLion, 8f / 30f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                    }
                    attackingMoveReservedTimer = 32f / 30f / spRate;
                    attackingDodgeReservedTimer = 29f / 30f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 4.5f);
                    isSkillAttacking = true;
                }
            }
        }
        else
        {
            if ((attackProcess == 2 && !JudgeStamina(GetCost(CostType.Attack) * (19f / 14f))) || (attackProcess == 3 && !JudgeStamina(GetCost(CostType.Skill))))
            {
                attackProcess = 0;
            }
            if (attackProcess == 0)
            {
                if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 34f / 30f / ats1 / spRate, 34f / 30f / ats1 / spRate, 1f, ats1 * spRate))
                {
                    skillAttackCount -= (1f + 34f / 30f / ats1);
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge))
                    {
                        if (sqrDist < MyMath.Square(2.7f - isLion))
                        {
                            SeparateFromTarget(2.7f - isLion);
                        }
                        else
                        {
                            SpecialStep(2.5f - isLion, 8f / 30f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        }
                    }
                    attackProcess = 1;
                    attackingMoveReservedTimer = 28f / 30f / ats1 / spRate;
                    attackingDodgeReservedTimer = 19f / 30f / ats1 / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 4.5f);
                }
            }
            else if (attackProcess == 1)
            {
                if (AttackBase(1, 1f, 1f, GetCost(CostType.Attack), 34f / 30f / ats1 / spRate, 34f / 30f / ats1 / spRate, 1f, ats1 * spRate))
                {
                    skillAttackCount -= (1f + 34f / 30f / ats1);
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge))
                    {
                        if (sqrDist < MyMath.Square(1.7f - isLion))
                        {
                            SeparateFromTarget(1.7f - isLion);
                        }
                        else
                        {
                            SpecialStep(1.5f - isLion, 8f / 30f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        }
                    }
                    attackProcess = (JudgeStamina(GetCost(CostType.Attack) * (19f / 14f)) ? 2 : 0);
                    attackingMoveReservedTimer = 26f / 30f / ats1 / spRate;
                    attackingDodgeReservedTimer = 19f / 30f / ats1 / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 4.5f);
                }
            }
            else if (attackProcess == 2)
            {
                if (AttackBase(2, 0.9f, 0.5f, GetCost(CostType.Attack) * (19f / 14f), 38f / 30f / ats2 / spRate, 38f / 30f / ats2 / spRate, 1f, ats2 * spRate))
                {
                    skillAttackCount -= (1f + 38f / 30f / ats2);
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge))
                    {
                        if (sqrDist < MyMath.Square(2.2f - isLion))
                        {
                            SeparateFromTarget(2.2f - isLion);
                        }
                        else
                        {
                            SpecialStep(2f - isLion, 8f / 30f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        }
                    }
                    attackProcess = (JudgeStamina(GetCost(CostType.Skill)) ? 3 : 0);
                    attackingMoveReservedTimer = 34f / 30f / ats2 / spRate;
                    attackingDodgeReservedTimer = 21f / 30f / ats2 / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 4.5f);
                }
            }
            else if (attackProcess == 3)
            {
                if (AttackBase(3, 2.3f, 2.5f, GetCost(CostType.Skill), (40f / 30f / ats3 + 0.05f) / spRate, (40f / 30f / ats3 + 0.05f) / spRate, 1f, ats3 * spRate)) {
                    skillAttackCount -= (1f + 40f / 30f / ats3);
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge))
                    {
                        if (sqrDist < MyMath.Square(4.5f - isLion * 1.6f))
                        {
                            SeparateFromTarget(4.5f - isLion * 1.6f);
                        }
                        else
                        {
                            SpecialStep(5f - isLion * 1.6f, 8f / 30f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        }
                    }
                    attackProcess = 0;
                    attackingMoveReservedTimer = 35f / 30f / ats3 / spRate;
                    attackingDodgeReservedTimer = 26f / 30f / ats3 / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 4.5f);
                }
            }
        }
        attackSave = attackType;
        S_ParticlePlay(0);
        UpdateSkillPercentage();
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        if (attackType == skillAttackIndex && isSuperarmor)
        {
            SetMutekiTime(0.4f);
        }
        AttackContinuousAir();
    }

    public override bool GetCanDodge()
    {
        if (state == State.Attack && attackType == skillAttackIndex && isSuperarmor)
        {
            return false;
        }
        return base.GetCanDodge();
    }

    void UpdateSkillPercentage()
    {
        if (CharacterManager.Instance)
        {
            CharacterManager.Instance.SetSkillPercentage(Mathf.FloorToInt((skillAttackMax - skillAttackCount) / skillAttackMax * 100f));
        }
    }

}
