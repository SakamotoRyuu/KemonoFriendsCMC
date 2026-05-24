using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Otter : PlayerController
{


    private const float baseSpeed = 1.9f;

    protected override void Awake()
    {
        base.Awake();
        moveCost.attack = 60f / baseSpeed * staminaCostRate;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 3.6f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
    }

    protected override void AttackBody()
    {
        float spRate = (isSuperman ? 4f / 3f : 1f) * baseSpeed;
        if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 60f / 30f / spRate, 60f / 30f / spRate, 0f, spRate))
        {
            if (targetTrans && GetTargetDistance(true, true, true) < 4.5f * 4.5f && groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
            {
                fbStepMaxDist = 2.5f;
                fbStepTime = 20f / 30f / spRate;
                SeparateFromTarget(4.5f);
            }
            attackingMoveReservedTimer = 44f / 30f / spRate;
            attackingDodgeReservedTimer = 36f / 30f / spRate;
            AttackStartAir();
        }
    }
}
