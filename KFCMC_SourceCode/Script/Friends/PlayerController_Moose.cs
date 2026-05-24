using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Moose : PlayerController
{

    public GameObject wallBreakerTackle;
    public GameObject wallBreakerImpact;
    public Transform quakePivot;
    public Transform throwOrigin;
    public int throwingCount;
    public float throwingRadius;
    public Transform[] movePivot;

    const float spBias = 5f / 3f;
    int moveIndex = -1;
    const int attackIndexTackle = 4;

    protected override void Awake()
    {
        base.Awake();
        mesAtkMax = 3;
        chatAttackCount = 4;
        moveCost.attack = 42f / spBias * staminaCostRate;
        moveCost.skill = 28f * staminaCostRate;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 1.97f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
        justDodgeCounterAttackProcess = 2;
    }

    void MoveAttack1()
    {
        if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1;
            specialMoveDirectionAdjustEnabled = true;
            SpecialStep(0.45f, 14f / 30f / spBias / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
        }
    }

    void MoveAttack2()
    {
        if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1;
            specialMoveDirectionAdjustEnabled = true;
            SpecialStep(0.45f, 14f / 30f / spBias / spRate, 4f, 0f, 0f, true, true, EasingType.Linear, true);
        }
    }

    void MoveAttack3()
    {
        if (groundedFlag) //Tackle
        {
            float spRate = isSuperman ? 4f / 3f : 1;
            specialMoveDirectionAdjustEnabled = false;
            SpecialStep(-1.5f, 12f / 30f / spRate, 8f, 4f, 8f, true, false, EasingType.SineOut);
        }
    }

    void SetWallBreakerTackle(int flag)
    {
        wallBreakerTackle.SetActive(flag != 0);
    }

    void SetWallBreakerImpact(int flag)
    {
        wallBreakerImpact.SetActive(flag != 0);
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (state != State.Attack && wallBreakerTackle.activeSelf)
        {
            wallBreakerTackle.SetActive(false);
        }
        if (state != State.Attack && wallBreakerImpact.activeSelf)
        {
            wallBreakerImpact.SetActive(false);
        }
    }

    protected override void AttackBody()
    {
        float spRate = isSuperman ? 4f / 3f : 1;
        if (playerInput.GetButton(RewiredConsts.Action.Special) && JudgeStamina(GetCost(CostType.Skill)))
        {
            if (AttackBase(attackIndexTackle, 1.1f, 1.3f, GetCost(CostType.Skill), 28f / 30f, 28f / 30f))
            {
                S_ParticlePlay(1);
                attackProcess = 0;
                isSkillAttacking = true;
                attackingMoveReservedTimer = 22f / 30f;
                attackingDodgeReservedTimer = 22f / 30f;
                AttackStartAir();
                nowSpeed = Mathf.Min(nowSpeed, 3f);
            }
        }
        else
        {
            brakeOnAttacking = true;
            if (attackProcess >= 2 && !JudgeStamina(GetCost(CostType.Attack) * (48f / 42f)))
            {
                attackProcess = 0;
            }
            switch (attackProcess)
            {
                case 0:
                    if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 42f / 30f / spBias / spRate, 42f / 30f / spBias / spRate, 1f, spBias * spRate))
                    {
                        S_ParticlePlay(0);
                        MoveStart(0);
                        attackingMoveReservedTimer = 32f / 30f / spBias / spRate;
                        attackingDodgeReservedTimer = 23f / 30f / spBias / spRate;
                        isSkillAttacking = false;
                        AttackStartAir();
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                    break;
                case 1:
                    if (AttackBase(1, 1f, 1f, GetCost(CostType.Attack), 42f / 30f / spBias / spRate, 42f / 30f / spBias / spRate, 1f, spBias * spRate))
                    {
                        S_ParticlePlay(0);
                        attackingMoveReservedTimer = 32f / 30f / spBias / spRate;
                        attackingDodgeReservedTimer = 29f / 30f / spBias / spRate;
                        isSkillAttacking = false;
                        AttackStartAir();
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                    break;
                case 2:
                    if (AttackBase(2, 1.15f, 1.2f, GetCost(CostType.Attack) * (48f / 42f), 48f / 30f / spBias / spRate, 48f / 30f / spBias / spRate, 1f, spBias * spRate))
                    {
                        S_ParticlePlay(0);
                        attackingMoveReservedTimer = 38f / 30f / spBias / spRate;
                        attackingDodgeReservedTimer = 35f / 30f / spBias / spRate;
                        isSkillAttacking = false;
                        AttackStartAir();
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                    break;
                case 3:
                    if (AttackBase(GetTargetHeight(true) >= 1.2f ? 5 : 3, 1.3f, 2f, GetCost(CostType.Attack) * (57f / 42f), 57f / 30f / spBias / spRate, 57f / 30f / spBias / spRate, 1f, spBias * spRate))
                    {
                        S_ParticlePlay(0);
                        attackingMoveReservedTimer = 47f / 30f / spBias / spRate;
                        attackingDodgeReservedTimer = 39f / 30f / spBias / spRate;
                        isSkillAttacking = false;
                        AttackStartAir();
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                    break;
            }
            attackProcess = (attackProcess + 1) % 4;
            isSkillAttacking = false;
        }
        SuperarmorStart();
    }

    void Impact()
    {
        if (!isItem && state == State.Attack)
        {
            EmitEffect(0);
            CameraManager.Instance.SetQuake(quakePivot.position, 6, 4, 0, 0, 1.5f, 1.5f, 12.5f);
            SetWallBreakerImpact(1);
            for (int i = 0; i < throwingCount; i++)
            {
                Vector3 pivotPos = throwOrigin.localPosition;
                Vector2 circlePos = Random.insideUnitCircle * throwingRadius;
                int throwIndex = Random.Range(0, throwing.throwSettings.Length);
                pivotPos.x += circlePos.x;
                pivotPos.z += circlePos.y;
                throwing.throwSettings[throwIndex].from.transform.localPosition = pivotPos;
                throwing.ThrowStart(throwIndex);
            }
        }
    }

    void MoveStart(int index)
    {
        moveIndex = index;
    }

    void MoveEnd()
    {
        moveIndex = -1;
        LockonEnd();
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        if (moveIndex >= 0 && moveIndex < movePivot.Length && movePivot[moveIndex] && groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1f;
            ApproachTransformPivot(movePivot[moveIndex], 13.5f * spRate, Mathf.Max(targetRadius, 0.4f), Mathf.Max(targetRadius - 0.4f, 0.075f), true, 0.25f);
        }
        AttackContinuousAir();
    }
}
