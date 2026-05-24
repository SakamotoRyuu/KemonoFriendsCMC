using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using static EffectDatabase;

public class PlayerController_Kaban : PlayerController
{

    const int servalID = 32;
    const int hyperServalID = 33;
    const float japarimanVelocityMin = 10f;
    const float japarimanVelocityMax = 20f;

    protected override void Awake()
    {
        base.Awake();
        moveCost.attack = 90;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 4f;
        sandstarHealMultiplierForAssist = 4f;
    }

    void ThrowReadySpecial()
    {
        if (throwing)
        {
            throwing.ThrowReady(attackType);
        }
    }

    void ThrowStartSpecial()
    {
        if (throwing)
        {
            if (attackType == 1)
            {
                if (GameManager.Instance.GetJaparimanCount() <= 0)
                {
                    throwing.ThrowCancel(attackType, true);
                    return;
                }
                CharacterManager.Instance.UseJapariman(1);
                Transform targetTemp = GetServalBreathPivot();
                if (targetTemp)
                {
                    throwing.throwSettings[1].velocity = Mathf.Clamp(Vector3.Distance(targetTemp.position, transform.position) + 5f, japarimanVelocityMin, japarimanVelocityMax);
                }
                else
                {
                    throwing.throwSettings[1].velocity = japarimanVelocityMin;
                }
            }
            throwing.ThrowStart(attackType);
        }
    }

    Transform GetServalBreathPivot()
    {
        int servalID = isHyper ? CharacterManager.friendsID_HyperServal : CharacterManager.friendsID_Serval;
        if (CharacterManager.Instance.GetFriendsExist(servalID, false))
        {
            return CharacterManager.Instance.GetFriendsBreathPivot(servalID);
        }
        return null;
    }

    protected override void Update_Targeting()
    {
        base.Update_Targeting();
        if (state == State.Attack && attackType == 1 && isLockon)
        {
            Transform targetTemp = GetServalBreathPivot();
            if (targetTemp)
            {
                targetTrans = targetTemp;
                target = targetTemp.gameObject;
                targetRadius = 0f;
            }
        }
    }

    protected override void AttackBody()
    {
        float spRate = (isSuperman ? 4f / 3f : 1f) * 2f;
        if (playerInput.GetButton(RewiredConsts.Action.Special) && GameManager.Instance.GetJaparimanCount() > 0)
        {
            if (AttackBase(1, 1f, 1f, GetCost(CostType.Attack), 82f / 30f / spRate, (82f / 30f + 0f) / spRate, 0f, spRate))
            {
                attackingMoveReservedTimer = 66f / 30f /spRate;
                attackingDodgeReservedTimer = 58f / 30f / spRate;
                AttackStartAir();
            }
        }
        else
        {
            throwing.target = targetTrans;
            if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 82f / 30f / spRate, (82f / 30f + 0f) / spRate, 0f, spRate))
            {
                if (targetTrans && GetTargetDistance(true, true, true) < 4.5f * 4.5f && groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    fbStepMaxDist = 2.5f;
                    fbStepTime = 30f / 30f / spRate;
                    SeparateFromTarget(4.5f);
                }
                isSkillAttacking = true;
                attackingMoveReservedTimer = 66f / 30f / spRate;
                attackingDodgeReservedTimer = 58f / 30f / spRate;
                AttackStartAir();
                ReserveSendDamageForContainer(56f / 30f / spRate);
            }
        }
    }

    public override void SetHyper(bool activate = true)
    {
        base.SetHyper(activate);
        CharacterManager.Instance.ExchangeServal(isHyper);
    }
}
