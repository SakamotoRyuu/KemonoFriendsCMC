using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Ibis : PlayerController
{

    protected override void Awake()
    {
        base.Awake();
        moveCost.attack = 100;
        mudToWalk = true;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 4f;
        sandstarHealMultiplierForAssist = 2f;
    }

    protected override void AttackBody()
    {
        float spRate = 0.8f;
        if (AttackBase(0, 1, 1, GetCost(CostType.Attack), 50f / 30f / spRate, 50f / 30f / spRate, 0f, spRate, false))
        {
            attackingMoveReservedTimer = 44f / 30f / spRate;
            attackingDodgeReservedTimer = 24f / 30f / spRate;
            ReserveSendDamageForContainer(6f / 30f / spRate);
        }
    }

}
