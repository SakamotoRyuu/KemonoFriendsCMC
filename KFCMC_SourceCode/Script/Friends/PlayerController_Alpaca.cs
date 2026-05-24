using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Alpaca : PlayerController
{

    public Transform spitTrans;

    static readonly Vector3 defaultEuler = new Vector3(353f, 0f, 0f);

    protected override void Awake()
    {
        base.Awake();
        moveCost.attack = 80;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 3f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
        sandstarHealMultiplierForAssist = 1f;
    }

    protected override void AttackBody()
    {
        float spRate = isSuperman ? 4f / 3f : 1f;
        if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 1f / spRate, 1f / spRate, 1f, spRate, true, 15f))
        { 
            attackingMoveReservedTimer = 20f / 30f / spRate;
            attackingDodgeReservedTimer = 11f / 30f / spRate;
            ReserveSendDamageForContainer(10f / 30f / spRate);
            AttackStartAir();
            nowSpeed = Mathf.Min(nowSpeed, 9f);
        }
    }

    public override void AttackStart(int index)
    {
        Vector3 spitEuler = defaultEuler;
        if (targetTrans)
        {
            spitEuler.x = Mathf.Clamp(353f - ((GetTargetDistance(false, false, true) - 3f) * 2f), 346f, 353f);
        }
        spitTrans.localEulerAngles = spitEuler;
        LockonEnd();
        base.AttackStart(index);
    }
}
