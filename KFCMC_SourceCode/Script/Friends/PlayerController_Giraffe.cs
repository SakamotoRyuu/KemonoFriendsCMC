using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Giraffe : PlayerController
{
    public Transform[] checkScaleTarget;

    Quaternion[] checkScaleTargetDefaultRotation;
    Vector3[] checkScaleTargetDefaultScale;

    protected override void Awake()
    {
        base.Awake();
        moveCost.attack = 24f * staminaCostRate;
        moveCost.skill = 32f;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 2.65f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
        checkScaleTargetDefaultRotation = new Quaternion[checkScaleTarget.Length];
        checkScaleTargetDefaultScale = new Vector3[checkScaleTarget.Length];
        for (int i = 0; i < checkScaleTarget.Length; i++)
        {
            if (checkScaleTarget[i])
            {
                checkScaleTargetDefaultRotation[i] = checkScaleTarget[i].localRotation;
                checkScaleTargetDefaultScale[i] = checkScaleTarget[i].localScale;
            }
        }
        justDodgeCounterAttackProcess = 2;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        for (int i = 0; i < checkScaleTarget.Length; i++)
        {
            if (checkScaleTarget[i])
            {
                checkScaleTarget[i].localRotation = checkScaleTargetDefaultRotation[i];
                checkScaleTarget[i].localScale = checkScaleTargetDefaultScale[i];
            }
        }
    }

    void MoveAttack1()
    {
        if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1;
            SpecialStep(0.4f, 0.25f / spRate, 4f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
        }
    }

    void MoveAttack2()
    {
        if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1;
            SpecialStep(1.2f, 0.25f / spRate, 4f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
        }
    }

    protected override void Attack()
    {
        base.Attack();
        float spRate = isSuperman ? 4f / 3f : 1;
        float heightDist = 0f;
        if (targetTrans)
        {
            heightDist = targetTrans.position.y - targetRadius - trans.position.y;
        }
        if (playerInput.GetButton(RewiredConsts.Action.Special) && JudgeStamina(GetCost(CostType.Skill)))
        {
            if (AttackBase(heightDist >= 1.4f ? 5 : 4, 1.1f, 1.25f, GetCost(CostType.Skill), 27f / 30f / 0.7f / spRate, 27f / 30f / 0.7f / spRate, 1f, 0.7f))
            {
                S_ParticlePlay(0);
                S_ParticlePlay(1);
                SuperarmorStart();
                MoveAttack2();
                attackingMoveReservedTimer = 26f / 30f / 0.7f / spRate;
                attackingDodgeReservedTimer = 22f / 30f / 0.7f / spRate;
                AttackStartAir();
                nowSpeed = Mathf.Min(nowSpeed, 9f);
            }
        }
        else
        {
            if (attackProcess >= 1 && !JudgeStamina(GetCost(CostType.Attack) * (33f / 24f)))
            {
                attackProcess = 0;
            }
            if (attackProcess == 2 && JudgeStamina(GetCost(CostType.Attack) * (45f / 24f)))
            {
                if (AttackBase(3, 1.15f, 1.15f, GetCost(CostType.Attack) * (45f / 24f), 45f / 30f / spRate, 45f / 30f / spRate, 1f, spRate)) // Summersault
                {
                    S_ParticlePlay(2);
                    S_ParticlePlay(3);
                    SuperarmorStart();
                    MoveAttack1();
                    attackProcess = 0;
                    attackingMoveReservedTimer = 40f / 30f / spRate;
                    attackingDodgeReservedTimer = 23f / 30f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
            }
            else if (heightDist >= 1.2f && JudgeStamina(GetCost(CostType.Attack) * (33f / 24f)))
            {
                if (AttackBase(2, 1.3f, 1.3f, GetCost(CostType.Attack) * (33f / 24f), 33f / 30f / spRate, 33f / 30f / spRate, 1f, spRate)) // High Kick
                {
                    S_ParticlePlay(2);
                    MoveAttack1();
                    attackProcess = 2;
                    attackingMoveReservedTimer = 26f / 30f / spRate;
                    attackingDodgeReservedTimer = 19f / 30f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
            }
            else
            {
                if (attackProcess == 0)
                {
                    if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 24f / 30f / spRate, 24f / 30f / spRate, 1f, spRate)) // Forward Kick
                    {
                        S_ParticlePlay(2);
                        MoveAttack1();
                        attackProcess = 1;
                        attackingMoveReservedTimer = 17f / 30f / spRate;
                        attackingDodgeReservedTimer = 11f / 30f / spRate;
                        AttackStartAir();
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                }
                else
                {
                    if (AttackBase(1, 1.25f, 1.25f, GetCost(CostType.Attack) * (30f / 24f), 30f / 30f / spRate, 30f / 30f / spRate, 1f, spRate)) // Middle Kick
                    {
                        S_ParticlePlay(2);
                        MoveAttack1();
                        attackProcess = 2;
                        attackingMoveReservedTimer = 23f / 30f / spRate;
                        attackingDodgeReservedTimer = 16f / 30f / spRate;
                        AttackStartAir();
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                }
            }
        }
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        if (attackType != 4 && attackType != 5)
        {
            AttackContinuousAir();
        }
    }
}
