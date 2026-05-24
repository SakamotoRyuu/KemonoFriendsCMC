using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_CampoFlicker : PlayerController
{

    protected override void Awake()
    {
        base.Awake();
        attackedTimeRemainOnDamage = 2f;
        moveCost.attack = 100;
        belligerent = false;
        mudToWalk = true;
        separateFearRate = 8f;
        justDodgeAmountMultiplier = 4f;
        sandstarHealMultiplierForAssist = 1f;
    }

    protected override void AttackBody()
    {
        float spRate = isSuperman ? 4f / 3f : 1f;
        if (AttackBase(0, 1, 1, GetCost(CostType.Attack), 67f / 30f / spRate, 67f / 30f / spRate, 0f, spRate, false))
        {
            attackingMoveReservedTimer = 59f / 30f / spRate;
            attackingDodgeReservedTimer = 12f / 30f / spRate;
            ReserveSendDamageForContainer(8f / 30f / spRate);
        }
    }
}
