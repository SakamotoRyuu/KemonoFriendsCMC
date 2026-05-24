using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_SilverFox : PlayerController
{

    public GameObject obstaclePrefab;

    int throwIndex = 0;
    float eventTimeRemain;
    int eventFaceIndex = -1;
    float obstacleResetTimeRemain = 0f;
    GameObject obstacleInstance;

    protected override void Awake()
    {
        base.Awake();
        attackedTimeRemainOnDamage = 3f;
        moveCost.attack = 80;
        belligerent = false;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 2f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
    }

    void ThrowReadySpecial()
    {
        if (throwing)
        {
            int throwRand = Random.Range(0, 100);
            if (throwRand < 60)
            {
                throwIndex = 0;
            }
            else if (throwRand < 85)
            {
                throwIndex = 1;
            }
            else if (throwRand < 98)
            {
                throwIndex = 2;
            }
            else
            {
                throwIndex = 3;
            }
            throwing.ThrowReady(throwIndex);
        }
    }

    void ThrowStartSpecial()
    {
        if (throwing)
        {
            throwing.ThrowStart(throwIndex);
        }
    }

    public void SetDance()
    {
        eventFaceIndex = fCon.GetFaceIndex("Event");
        eventTimeRemain = 240f / 30f;
        AttackBase(1, 0f, 0f, 0f, 180f / 30f, 180f / 30f, 0f, 1f, false);
        obstacleResetTimeRemain = 5f;
        gravityZeroTimeRemain = 5.5f;
        if (agent)
        {
            agent.enabled = false;
        }
        obstacleInstance = Instantiate(obstaclePrefab, transform);
    }

    protected override void Update_TimeCount()
    {
        base.Update_TimeCount();
        if (obstacleResetTimeRemain > 0f)
        {
            obstacleResetTimeRemain -= deltaTimeCache;
            if (obstacleResetTimeRemain <= 0f)
            {
                if (obstacleInstance)
                {
                    Destroy(obstacleInstance);
                }
                if (agent && !agent.enabled)
                {
                    agent.enabled = true;
                }
            }
            else
            {
                if (agent && agent.enabled)
                {
                    agent.enabled = false;
                }
            }
        }
    }

    void EventFace2()
    {
        eventFaceIndex = fCon.GetFaceIndex("Event2");
    }

    void EventEffect()
    {
        EmitEffect(0);
    }

    protected override void Update_FaceControl()
    {
        if (eventTimeRemain > 0f)
        {
            eventTimeRemain -= deltaTimeCache;
            if (fCon && eventFaceIndex != -1 && fCon.CurrentFaceIndex != eventFaceIndex)
            {
                SetFace(eventFaceIndex);
            }
        }
        else
        {
            base.Update_FaceControl();
        }
    }

    protected override void AttackBody()
    {
        float spRate = (isSuperman ? 4f / 3f : 1f) * 2f;
        throwing.target = targetTrans;
        if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 82f / 30f / spRate, 82f / 30f / spRate, 0f, spRate))
        {
            if (groundedFlag && targetTrans && GetTargetDistance(true, true, true) < 4.5f * 4.5f && !playerInput.GetButton(RewiredConsts.Action.Dodge))
            {
                fbStepMaxDist = 2.5f;
                fbStepTime = 30f / 30f / spRate;
                SeparateFromTarget(4.5f);
            }
            AttackStartAir();
            attackingMoveReservedTimer = (82f - 8f * 2f) / 30f / spRate;
            attackingDodgeReservedTimer = 58f / 30f / spRate;
        }
    }
}
