using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BirdlienRaw : EnemyBaseBoss
{

    public Transform quakePivot;
    public GameObject windPrefab;
    public Transform knockDownRayPivot;
    public GameObject additiveWallBreaker;
    public Transform[] windThrowPivots;

    int attackSave = -1;
    int attackSubSave = -1;
    float windInterval;
    float rushInterval;
    bool quakeFlag;
    float continuousQuakeInterval;
    bool t_WhiteOwlAttacked;
    bool t_EagleOwlAttacked;
    bool t_GotTrophy;
    float attackSpeed = 1;

    float bombIntervalRemain;
    Vector3 bombPositionSave;

    const int attackIndexHead = 0;
    const int attackIndexFireParticle = 3;
    const int throwIndexBomb = 1;
    const int effectIndexDeadSave = 12;

    protected override void Awake()
    {
        base.Awake();
        deadTimer = 3;
        attackWaitingLockonRotSpeed = 1.5f;

        superAttackRate = 1.1f;
        superDefenseRate = 1.1f;
        superKnockedRate = 1f;
        superSpeedRate = 1.25f;
        superAngularRate = 1.25f;
        superAccelerationRate = 1.25f;
        checkGroundPivotHeight = 1f;
        checkGroundTolerance = 2f;
        checkGroundTolerance_Jumping = 2f;
        sandstarRawKnockEndurance = 5000;
        sandstarRawKnockEnduranceLight = 5000f / 3f;
        fireDamageRate = 0;
        attackedTimeRemainOnDamage = 0.1f;
        if (additiveWallBreaker)
        {
            additiveWallBreaker.SetActive(true);
        }
    }

    protected override void BattleStart()
    {
        base.BattleStart();
        actDistNum = 1;
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (state != State.Attack)
        {
            quakeFlag = false;
        }
        if (state == State.Attack && quakeFlag)
        {
            continuousQuakeInterval -= deltaTimeCache;
            if (continuousQuakeInterval <= 0f)
            {
                continuousQuakeInterval = 0.03f;
                CameraManager.Instance.SetQuake(GetCenterPosition(), 3, 8, 0, 0.05f, 0f, 3f, dissipationDistance_Large);
            }
        }

        if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled)
        {
            weakProgress = (nowHP <= GetMaxHP() / 3 ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 0);
        }
        else
        {
            weakProgress = (nowHP <= GetMaxHP() / 2 ? 1 : 0);
        }
        // Bombing
        if (state == State.Attack && attackDetection[attackIndexHead].attackEnabled)
        {
            bombIntervalRemain -= deltaTimeMove * attackSpeed;
            if (bombIntervalRemain <= 0 && 
                target &&
                enemyBalloonElements.PaperPlane.activeSelf == false && 
                enemyBalloonElements.MargayVoice.activeSelf == false && 
                (trans.position - bombPositionSave).sqrMagnitude >= 5f * 5f && 
                GetTargetDistance(true, true, false) >= 3f * 3f)
            {
                throwing.ThrowStart(throwIndexBomb);
                bombIntervalRemain = 10f / 60f;
                bombPositionSave = trans.position;
            }
        }
    }

    void MoveAttack0_0()
    {
        fbStepTime = 20f / 60f / attackSpeed;
        fbStepMaxDist = 4f;
        BackStep(6f);
    }

    void MoveAttack0_1()
    {
        LockonEnd();
        PerfectLockon();
        SpecialStep(-15f, 90f / 60f / attackSpeed, 40f, 24f, 24f, true, false, EasingType.SineInOut);
        quakeFlag = true;
        continuousQuakeInterval = 0f;
    }

    void MoveAttack1()
    {
        fbStepTime = 20f / 60f / attackSpeed;
        fbStepMaxDist = 4f;
        ApproachOrSeparate(1.9f);
    }

    void MoveAttack2()
    {
        fbStepTime = 20f / 60f / attackSpeed;
        fbStepMaxDist = 4f;
        ApproachOrSeparateBetween(3f, 2f);
    }

    void MoveAttack3()
    {
        fbStepTime = 50f / 60f / attackSpeed;
        fbStepMaxDist = 1.5f;
        SeparateFromTarget(8f);
    }

    void MoveAttack6()
    {
        fbStepTime = 30f / 60f / attackSpeed;
        fbStepMaxDist = 4f;
        StepToTarget(-3f);
    }

    void MoveAttack7()
    {
        fbStepTime = 30f / 60f / attackSpeed;
        fbStepMaxDist = 0.9f;
        SeparateFromTarget(8f);
    }

    void MoveAttack8_0()
    {
        LockonEnd();
        PerfectLockon();
        SpecialStep(-10f, 60f / 60f / attackSpeed, 20f, 16f, 16f, true, false, EasingType.SineInOut);
        quakeFlag = true;
        continuousQuakeInterval = 0f;
    }

    void MoveAttack8_1()
    {
        LockonEnd();
        PerfectLockon();
        SpecialStep(-15f, 60f / 60f / attackSpeed, 30f, 20f, 20f, true, false, EasingType.SineInOut);
        quakeFlag = true;
        continuousQuakeInterval = 0f;
    }

    void QuakeEnd()
    {
        if (state == State.Attack)
        {
            CameraManager.Instance.SetQuake(GetCenterPosition(), 3, 8, 0, 0f, 0.6f, 3f, dissipationDistance_Large);
        }
        quakeFlag = false;
    }

    void StopAndLockonStart()
    {
        lockonRotSpeed = 12f;
        specialMoveDuration = 0f;
        LockonStart();
    }

    void ChangeForRushTurn()
    {
        attackPowerMultiplier = 1.15f;
    }

    void ThrowStartToReady(int index)
    {
        throwing.ThrowStart(index);
        throwing.ThrowReady(index);
    }

    void ThrowWind()
    {
        LockonEnd();
        int targetNum = Mathf.Clamp(CharacterManager.Instance.GetFriendsCount(), 1, 100);
        int windCount = weakProgress >= 2 ? windThrowPivots.Length : 1;
        for (int i = 0; i < windCount; i++)
        {
            GameObject windObj = Instantiate(windPrefab, windThrowPivots[i].position, Quaternion.identity);
            windObj.GetComponent<Rigidbody>().AddForce(windThrowPivots[i].TransformDirection(vecForward) * 15f, ForceMode.VelocityChange);
            windObj.GetComponentInChildren<Trap_Warp_SuperWind>().maxNum = targetNum;
        }
        windInterval = 20f;
    }

    protected override void KnockHeavyProcess()
    {
        if (!fixKnockAmount && lastDamagedColorType == damageColor_Critical && attackerCB == CharacterManager.Instance.pCon && throwing.GetIsReady(0))
        {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_BirdlienCounter, true);
        }
        base.KnockHeavyProcess();
        agent.radius = 0.1f;
        agent.height = 0.1f;
    }

    public void QuakeHeavyKnock()
    {
        if (state == State.Damage)
        {
            CameraManager.Instance.SetQuake(quakePivot.position, 5, 4, 0, 0, 1.5f, 3f, dissipationDistance_Large);
        }
    }

    protected override void Update_TimeCount()
    {
        base.Update_TimeCount();
        if (windInterval > 0f)
        {
            windInterval -= deltaTimeCache * CharacterManager.Instance.riskyIncrease * attackSpeed;
        }
        if (rushInterval > 0f)
        {
            rushInterval -= deltaTimeCache * CharacterManager.Instance.riskyIncrease * attackSpeed;
        }
    }

    protected override void DamageCommonProcess()
    {
        base.DamageCommonProcess();
        if (!fixKnockAmount && !t_GotTrophy && attackerCB && CharacterManager.Instance.GetFriendsExist(13) && CharacterManager.Instance.GetFriendsExist(14))
        {
            if (!t_WhiteOwlAttacked && attackerCB == CharacterManager.Instance.friends[13].fBase)
            {
                t_WhiteOwlAttacked = true;
            }
            if (!t_EagleOwlAttacked && attackerCB == CharacterManager.Instance.friends[14].fBase)
            {
                t_EagleOwlAttacked = true;
            }
        }
        if (actDistNum != 1)
        {
            BattleStart();
        }
    }

    protected override void DeadProcess()
    {
        base.DeadProcess();
        if (t_WhiteOwlAttacked && t_EagleOwlAttacked && CharacterManager.Instance.GetFriendsExist(13, true) && CharacterManager.Instance.GetFriendsExist(14, true))
        {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_Owls, true);
        }
    }

    int GetAttackRandom()
    {
        float sqrDist = (targetTrans.position - trans.position).sqrMagnitude;
        if (sqrDist > 8f * 8f)
        {
            if (windInterval <= 0 && sqrDist < 20f * 20f)
            {
                int rand = Random.Range(0, 3);
                return (rand == 0 && rushInterval <= 3 ? 0 : rand == 2 ? 4 : 3);
            }
            else
            {
                return ((Random.Range(0, 2) == 0 && rushInterval <= 3f) ? 0 : 3);
            }
        }
        else
        {
            if (windInterval <= 0)
            {
                return Random.Range(rushInterval <= 0f ? 0 : 1, 5);
            }
            return Random.Range(rushInterval <= 0 ? 0 : 1, 4);
        }
    }

    protected override void Attack()
    {
        base.Attack();
        attackSpeed = (weakProgress == 2 ? 1.15f : 1);
        bombIntervalRemain = 0;
        bombPositionSave = trans.position;
        resetAgentRadiusOnChangeState = true;
        resetAgentHeightOnChangeState = true;
        if (targetTrans)
        {
            int attackTemp = GetAttackRandom();
            if (attackTemp == attackSave)
            {
                attackTemp = GetAttackRandom();
            }
            attackSave = attackTemp;
            int attackSubTemp = 0;
            if (weakProgress >= 1)
            {
                attackSubTemp = Random.Range(0, 2);
                if (attackSubTemp == attackSubSave)
                {
                    attackSubTemp = Random.Range(0, 2);
                }
            }
            attackSubSave = attackSubTemp;

            if (!battleStarted)
            {
                BattleStart();
                attackSave = attackTemp = 0;
                attackSubSave = attackSubSave = 0;
            }
            float intervalPlus = 0f;
            intervalPlus = (weakProgress == 2 ? Random.Range(0.3f, 0.5f) : weakProgress == 1 ? Random.Range(0.4f, 0.6f) : Random.Range(0.5f, 0.7f));
            switch (attackTemp)
            {
                case 0:
                    if (attackSubTemp == 1)
                    {
                        AttackBase(8, 1f, 2.1f, 0, 210f / 60f / attackSpeed, 210f / 60f / attackSpeed + intervalPlus, 0.5f, attackSpeed, true, 20f);
                    }
                    else
                    {
                        AttackBase(0, 1f, 2.1f, 0, 140f / 60f / attackSpeed, 140f / 60f / attackSpeed + intervalPlus, 0.5f, attackSpeed, true, 20f);
                    }
                    rushInterval = 7f;
                    SuperarmorStart();
                    break;
                case 1:
                    if (attackSubTemp == 1)
                    {
                        AttackBase(5, 1.1f, 1.2f, 0, 140f / 60f / attackSpeed, 140f / 60f / attackSpeed + intervalPlus, 0.5f, attackSpeed);
                    }
                    else
                    {
                        AttackBase(1, 1.1f, 1.2f, 0, 70f / 60f / attackSpeed, 70f / 60f / attackSpeed + intervalPlus, 0.5f, attackSpeed);
                    }
                    break;
                case 2:
                    if (attackSubTemp == 1)
                    {
                        AttackBase(6, 1.2f, 1.6f, 0, 125f / 60f / attackSpeed, 125f / 60f / attackSpeed + intervalPlus, attackSpeed);
                    }
                    else
                    {
                        AttackBase(2, 1.2f, 1.6f, 0, 85f / 60f / attackSpeed, 85f / 60f / attackSpeed + intervalPlus, attackSpeed);
                    }
                    SuperarmorStart();
                    break;
                case 3:
                    if (attackSubTemp == 1)
                    {
                        AttackBase(7, 1f, 1.1f, 0, 200f / 60f / attackSpeed, 200f / 60f / attackSpeed, 1, attackSpeed);
                    }
                    else
                    {
                        AttackBase(3, 1f, 1.1f, 0, 120f / 60f / attackSpeed, 120f / 60f / attackSpeed, 1, attackSpeed);
                    }
                    break;
                case 4:
                    AttackBase(4, 0f, 0f, 0, 135f / 60f / attackSpeed, 135f / 60f / attackSpeed + intervalPlus * 0.5f, 0f, attackSpeed);
                    fbStepTime = 30f / 60f;
                    windInterval = 10f;
                    SeparateFromTarget(3f);
                    break;
            }
        }
    }

    void ShiftOnKnockDown()
    {
        if (knockDownRayPivot)
        {
            ray.origin = knockDownRayPivot.position;
            ray.direction = knockDownRayPivot.TransformDirection(vecForward);
            if (Physics.Raycast(ray, out raycastHit, 2.5f, fieldLayerMask, QueryTriggerInteraction.Ignore))
            {
                cCon.Move(knockDownRayPivot.TransformDirection(vecBack) * (2.5f - raycastHit.distance));
            }
        }
    }

    public override void EmitEffectString(string type)
    {
        switch (type)
        {
            case "Dead":
                EmitEffect(effectIndexDeadSave);
                break;
            case "FireParticle":
                AttackStart(attackIndexFireParticle);
                break;
        }
    }

}
