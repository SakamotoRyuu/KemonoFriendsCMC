using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Fennec : PlayerController
{

    const int checkFriendsID = 29;
    //const float defaultAttackCost = 12f;
    // const float angryAttackCost = 12f;
    bool isAngry;
    int defaultAttackFaceIndex;
    int angryAttackFaceIndex;
    int defaultSmileFaceIndex;
    int angrySmileFaceIndex;
    int defaultDeadFaceIndex;
    int angryDeadFaceIndex;
    int attackSave;

    protected override void Awake()
    {
        base.Awake();
        moveCost.attack = 46f * staminaCostRate;
        moveCost.skill = 35f * staminaCostRate;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 1.09f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
    }

    protected override void Start()
    {
        base.Start();
        if (animatorForBattle)
        {
            SetAngry(false);
        }
    }

    protected override void SetFaceIndex()
    {
        base.SetFaceIndex();
        if (fCon)
        {
            defaultAttackFaceIndex = faceIndex[(int)FaceName.Attack];
            angryAttackFaceIndex = fCon.GetFaceIndex("Attack2");
            defaultSmileFaceIndex = faceIndex[(int)FaceName.Smile];
            angrySmileFaceIndex = fCon.GetFaceIndex("Smile2");
            defaultDeadFaceIndex = faceIndex[(int)FaceName.Dead];
            angryDeadFaceIndex = fCon.GetFaceIndex("Dead2");
        }
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (isAngry && CharacterManager.Instance.GetFriendsExist(checkFriendsID, true))
        {
            SetAngry(false);
        }
    }

    public override void ResetStatus()
    {
        base.ResetStatus();
        if (isAngry)
        {
            SetAngry(false);
        }
    }

    public void SetAngry(bool flag = true)
    {
        isAngry = flag;
        if (isAngry)
        {
            attackPower = 10f;
            knockPower = 40f;
            attackSave = 3;
            mesAtkMin = 3;
            mesAtkMax = 3;
            mesDmgLtMin = 4;
            mesDmgLtMax = 4;
            mesDmgHvMin = 5;
            mesDmgHvMax = 5;
            SetChat("TALK_FENNEC_SPECIAL", 40, 3f);
        }
        else
        {
            attackPower = 8f;
            knockPower = 30f;
            mesAtkMin = 0;
            mesAtkMax = 2;
            mesDmgLtMin = 0;
            mesDmgLtMax = 2;
            mesDmgHvMin = 1;
            mesDmgHvMax = 3;
        }
        if (fCon)
        {
            faceIndex[(int)FaceName.Attack] = isAngry ? angryAttackFaceIndex : defaultAttackFaceIndex;
            faceIndex[(int)FaceName.Smile] = isAngry ? angrySmileFaceIndex : defaultSmileFaceIndex;
            faceIndex[(int)FaceName.Dead] = isAngry ? angryDeadFaceIndex : defaultDeadFaceIndex;
        }
    }

    void MoveAttack()
    {
        if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1;
            SpecialStep(0.3f, 0.25f / spRate, 4f, 0.05f, 0.4f);
        }
    }

    void MoveAttack1()
    {
        if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1;
            SpecialStep(0.3f, 0.25f / spRate, 4f, 0f, 0.4f);
        }
    }

    void MoveAttack2()
    {
        if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1;
            SpecialStep(0.25f, 0.25f / spRate, 4f, 0.05f, 0.4f);
        }
    }

    void MoveAttack3()
    {
        if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1;
            SpecialStep(0.3f, 0.25f / spRate, 4f, 0.05f, 0.4f);
        }
    }

    void MoveSeparate()
    {
        if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1;
            fbStepMaxDist = 3f;
            fbStepTime = 12f / 30f / spRate;
            SeparateFromTarget(7f);
        }
    }

    void MoveEscape()
    {
        if (targetTrans && groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float dist = GetTargetDistance(false, true, true);
            if (dist < 7f)
            {
                float spRate = isSuperman ? 4f / 3f : 1;
                Vector3 escapeDestination = GetEscapeDestination(searchArea[0].GetTargetsAveragePosition(), 12f);
                escapeDestination.y = trans.position.y;
                SetSpecialMove((escapeDestination - trans.position).normalized, Mathf.Clamp(7f - dist, 0f, 4f), 16f / 30f / spRate, EasingType.SineOut);
            }
        }
    }

    void SetCombo2()
    {
        SetAttackPowerMultiplier(1.6f);
        SetAttackPowerMultiplier(2f);
    }

    protected override void AttackBody()
    {
        float spRate = isSuperman ? 4f / 3f : 1;
        if (playerInput.GetButton(RewiredConsts.Action.Special))
        {
            int attackRand = Random.Range(1, 4);
            if ((attackSave == 1 && attackRand == 1) || (attackSave == 3 && attackRand == 3))
            {
                attackRand = 2;
            }
            spRate *= 1.05f;
            if (attackRand == 1)
            {
                if (AttackBase(1, 10f / 8f * 1.2f, 40f / 30f * 1.25f, GetCost(CostType.Skill) * (26f / 35f), 34f / 30f / 1.25f / spRate, 34f / 30f / 1.25f / spRate, 1f, 1.25f * spRate))
                {
                    S_ParticlePlay(2);
                    S_ParticlePlay(3);
                    SuperarmorStart();
                    MoveAttack1();
                    attackingMoveReservedTimer = 30f / 30f / 1.25f / spRate;
                    attackingDodgeReservedTimer = 23f / 30f / 1.25f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
            }
            else if (attackRand == 2)
            {
                if (AttackBase(2, 10f / 8f * 0.9f, 40f / 30f * 0.7f, GetCost(CostType.Skill) * (19f / 35f), 20f / 30f / spRate, 20f / 30f / spRate, 1f, spRate))
                {
                    S_ParticlePlay(0);
                    S_ParticlePlay(1);
                    MoveAttack2();
                    attackingMoveReservedTimer = 18f / 30f / spRate;
                    attackingDodgeReservedTimer = 13f / 30f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
            }
            else
            {
                if (AttackBase(3, 10f / 8f * 1.2f, 40f / 30f * 1.25f, GetCost(CostType.Skill), 46f / 30f / 1.25f / spRate, 46f / 30f / 1.25f / spRate, 1f, 1.25f * spRate))
                {
                    S_ParticlePlay(2);
                    MoveAttack3(); 
                    attackingMoveReservedTimer = 38f / 30f / 1.25f / spRate;
                    attackingDodgeReservedTimer = 14f / 30f / 1.25f / spRate;
                    AttackStartAir();
                    nowSpeed = Mathf.Min(nowSpeed, 9f);
                }
            }
            attackSave = attackRand;
        }
        else
        {
            if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 46f / 30f / spRate, 46f / 30f / spRate, 0f, spRate))
            {
                MoveSeparate(); 
                attackingMoveReservedTimer = 38f / 30f / spRate;
                attackingDodgeReservedTimer = 28f / 30f / spRate;
                AttackStartAir();
                nowSpeed = Mathf.Min(nowSpeed, 9f);
            }
        }
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        if (attackType != 0)
        {
            AttackContinuousAir();
        }
    }
}
