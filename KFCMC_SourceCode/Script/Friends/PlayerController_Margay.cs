using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Margay : PlayerController
{
    protected override void Awake()
    {
        base.Awake();
        attackedTimeRemainOnDamage = 70f / 30f;
        moveCost.attack = 70;
        belligerent = false;
        separateFearRate = 8f;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 4f;
        sandstarHealMultiplierForAssist = 4f;
    }

    protected override void AttackBody()
    {
        float spRate = (isSuperman ? 4f / 3f : 1f) * 0.8f;
        if (AttackBase(0, 1, 1, GetCost(CostType.Attack), 70f / 30f / spRate, 70f / 30f / spRate, 0f, spRate, false))
        {
            AttackStartAir();
            attackingMoveReservedTimer = 63.6f / 30f / spRate;
            attackingDodgeReservedTimer = 12f / 30f / spRate;
            ReserveSendDamageForContainer(10f / 30f / spRate);
        }
    }
}
