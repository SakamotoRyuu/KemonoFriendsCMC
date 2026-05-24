using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Owl : PlayerController
{

    public float attackSpeed = 1f;

    float defaultCConHeight;
    float defaultCConStepOffset;

    protected override void Awake()
    {
        base.Awake();
        dodgeDistance = 6f;
        moveCost.attack = 16f / attackSpeed * staminaCostRate;
        mudToWalk = true;
        defaultCConHeight = cCon.height;
        defaultCConStepOffset = cCon.stepOffset;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 1.97f / attackSpeed;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
    }

    protected override void AttackBody()
    {
        if (attackSpeed <= 0f)
        {
            attackSpeed = 1f;
        }
        float spRate = (isSuperman ? 4f / 3f : 1f) * attackSpeed;
        float stiffTimeTemp = 16f / 30f / spRate;
        if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), stiffTimeTemp, stiffTimeTemp, 0f, spRate, true, 100f))
        {
            CommonLockon();
            lockonRotSpeed = 10f;
            S_ParticlePlay(0);
            cCon.height = 0.5f;
            cCon.stepOffset = 0.4f;
            gravityZeroTimeRemain = stiffTimeTemp;
            Vector3 targetPos = trans.position;
            float dist = 9f / attackSpeed;
            if (targetTrans)
            {
                targetPos = targetTrans.position;
                targetPos.y -= Random.Range(0.6f, 0.8f);
                if (GetTargetDistance(true, true, true) <= 1f)
                {
                    Vector2 randCircle = Random.insideUnitCircle * targetRadius;
                    targetPos.x += randCircle.x;
                    targetPos.z += randCircle.y;
                }
                dist = Mathf.Clamp(Vector3.Distance(trans.position, targetPos) + 2f, 5f / attackSpeed, 9f / attackSpeed);
            }
            else
            {
                targetPos = trans.position + trans.TransformDirection(vecForward);
            }
            Vector3 direction = (targetPos - trans.position).normalized;
            SetSpecialMove(direction != vecZero ? direction : trans.TransformDirection(vecForward), dist, stiffTimeTemp, EasingType.SineInOut);
            EmitEffect(0);
            AttackStartAir();
            attackingMoveReservedTimer = 8f / 30f / spRate;
        }
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (state != State.Attack)
        {
            if (cCon.height != defaultCConHeight)
            {
                cCon.height = defaultCConHeight;
                cCon.stepOffset = defaultCConStepOffset;
            }
        }
    }

    public override bool GetCanDodge()
    {
        bool groundedFlagSave = groundedFlag;
        State stateSave = state;
        groundedFlag = true;
        if (state == State.Jump)
        {
            state = State.Wait;
        }
        bool result = base.GetCanDodge();
        groundedFlag = groundedFlagSave;
        state = stateSave;
        return result;
    }

    protected override void Start_Process_Dodge()
    {
        if (!IsCoyote())
        {
            EmitEffect(1);
            setGravityOnDodge = false;
        }
        else
        {
            setGravityOnDodge = true;
        }
        base.Start_Process_Dodge();
    }

}
