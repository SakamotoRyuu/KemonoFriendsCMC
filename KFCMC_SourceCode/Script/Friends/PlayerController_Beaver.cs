using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Beaver : PlayerController
{

    protected override void Awake()
    {
        base.Awake();
        moveCost.attack = 51f * staminaCostRate;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 4f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
    }

    protected override void AttackBody()
    {
        float spRate = isSuperman ? 4f / 3f : 1;
        if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 51f / 30f / spRate, 51f / 30f / spRate, 0f, spRate))
        {
            if (groundedFlag && targetTrans && GetTargetDistance(true, true, true) < 6f * 6f && !playerInput.GetButton(RewiredConsts.Action.Dodge))
            {
                fbStepMaxDist = 4f;
                fbStepTime = 18f / 30f / spRate;
                SeparateFromTarget(6f);
            }
            attackingMoveReservedTimer = 43f / 30f / spRate;
            attackingDodgeReservedTimer = 25f / 30f / spRate;
            AttackStartAir();
        }
    }
}
