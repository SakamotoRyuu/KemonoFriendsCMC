using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Penguin : PlayerController
{

    public float attackSpeed = 1.2f;
    public bool isFast = false;

    float pppExistTime = 0;
    const string chatKey_PPP = "TALK_PPP";

    protected override void Awake()
    {
        base.Awake();
        if (attackSpeed > 0)
        {
            if (isFast)
            {
                moveCost.attack = 18f / attackSpeed * staminaCostRate;
                sandstarHealMultiplier = justDodgeAmountMultiplier = 1.21f;
                if (attackPower > 0)
                {
                    sandstarHealMultiplier *= 10f / attackPower;
                }
            }
            else
            {
                moveCost.attack = 33f / attackSpeed * staminaCostRate;
                sandstarHealMultiplier = justDodgeAmountMultiplier = 4f / attackSpeed;
                if (attackPower > 0)
                {
                    sandstarHealMultiplier *= 10f / attackPower;
                }
            }
        }
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.PPP) != 0)
        {
            if (pppExistTime < 3f)
            {
                pppExistTime += deltaTimeCache;
                if (pppExistTime >= 3f)
                {
                    SetChat(chatKey_PPP, 35);
                }
            }
        }
        else
        {
            pppExistTime = 0f;
        }
    }

    protected override void AttackBody()
    {
        float spRate = isSuperman ? 4f / 3f : 1f;
        float spTemp = spRate * attackSpeed;
        int dir = 0;
        float frames = isFast ? 18f : 33f;
        if (isFast)
        {
            dir = attackProcess;
        }
        else
        {
            if (targetTrans)
            {
                if (Vector3.Cross(trans.TransformDirection(vecForward), targetTrans.position - trans.position).y >= -1)
                {
                    //Right
                    dir = 0;
                }
                else
                {
                    //Left
                    dir = 1;
                }
            }
        }
        if (AttackBase((isFast ? 2 : 0) + dir, 1f, 1f, GetCost(CostType.Attack), frames / 30f / spTemp, frames / 30f / spTemp, 1f, spTemp))
        {
            S_ParticlePlay(dir);
            if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
            {
                SpecialStep(0.35f, 0.25f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
            }
            AttackStartAir();
            attackingMoveReservedTimer = (frames - 8f) / 30f / spTemp;
            attackingDodgeReservedTimer = (isFast ? 14f : 19f) / 30f / spTemp;
            nowSpeed = Mathf.Min(nowSpeed, 9f);
        }
        attackProcess = (attackProcess + 1) % 2;
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        AttackContinuousAir();
    }

}
