using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_PrairieDog : PlayerController
{

    protected override void Awake()
    {
        base.Awake();
        moveCost.attack = 50;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 8f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
        sandstarHealMultiplierForAssist = 1f;
    }

    protected override void AttackBody()
    {
        float spRate = isSuperman ? 4f / 3f : 1;
        if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 66f / 30f / spRate, 66f / 30f / spRate, 0, spRate, false))
        {
            attackingMoveReservedTimer = 60f / 30f / spRate;
            attackingDodgeReservedTimer = 59f / 30f / spRate;
            AttackStartAir();
            ReserveSendDamageForContainer(5f / 30f / spRate);
        }
    }
}
