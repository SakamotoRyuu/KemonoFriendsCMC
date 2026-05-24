using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class PlayerController_RedFox : PlayerController
{

    public XWeaponTrail jumpTrail;

    bool jumpTrailEnabled = false;
    bool accurateAimFlag;

    protected override void Start()
    {
        base.Start();
        jumpTrail.Init();
        jumpTrail.Deactivate();
        jumpTrailEnabled = false;
        moveCost.attack = 62f * staminaCostRate;
        specialMoveDirectionAdjustEnabled = false;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 1f; // some damages in 1 attack;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
    }

    public override void SetForItem()
    {
        jumpTrail.gameObject.SetActive(false);
        base.SetForItem();
    }

    void FoxJumpStart()
    {
        if (enabled)
        {
            LockonEnd();
            if (accurateAimFlag)
            {
                PerfectLockon();
            }
            jumpTrail.Activate();
            jumpTrailEnabled = true;
            EmitEffect(accurateAimFlag ? 1 : 0);
            SetSpecialMove(trans.TransformDirection(vecForward), attackType >= 4 ? 6f : attackType == 3 ? 7f : 8f, 28f / 30f, EasingType.Linear);
        }
    }

    void FoxJumpEnd()
    {
        if (enabled)
        {
            if (jumpTrailEnabled)
            {
                jumpTrail.StopSmoothly(0.1f);
                jumpTrailEnabled = false;
            }
            SuperarmorEnd();
        }
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (state == State.Attack && accurateAimFlag && specialMoveDuration > 0f && targetTrans)
        {
            float distTemp = GetTargetDistance(false, true, false);
            if (distTemp < 0.05f)
            {
                specialMoveDirection = vecZero;
            }
            else
            {
                specialMoveDirection = GetTargetVector(true, true);
                if (distTemp < 0.125f)
                {
                    specialMoveDirection *= distTemp * 8f;
                }
            }
        }
        if (state != State.Attack)
        {
            if (jumpTrailEnabled)
            {
                FoxJumpEnd();
            }
        }
    }

    protected override void AttackBody()
    {
        int attackTemp = 0;
        accurateAimFlag = (Random.Range(0, 100) < (isSuperman ? 50 : 25));
        float attackStiffTemp = isSuperman ? 54f / 30f : 62f / 30f;
        if (targetTrans)
        {
            if (targetTrans.position.y > trans.position.y + 2.15f)
            {
                attackTemp = 4;
            }
            else if (targetTrans.position.y > trans.position.y + 1.7f)
            {
                attackTemp = 3;
            }
            else if (targetTrans.position.y > trans.position.y + 1.35f)
            {
                attackTemp = 2;
            }
            else if (targetTrans.position.y > trans.position.y + 0.9f)
            {
                attackTemp = 1;
            }
        }
        if (AttackBase(attackTemp, 1, 1, GetCost(CostType.Attack), attackStiffTemp, attackStiffTemp, 0f, 1f, true, accurateAimFlag ? 20 : 15))
        {
            S_ParticlePlay(0);
            if (accurateAimFlag)
            {
                SuperarmorStart();
            }
            AttackStartAir();
            attackingMoveReservedTimer = 54f / 30f;
            attackingDodgeReservedTimer = 36f / 30f;
        }
    }
}
