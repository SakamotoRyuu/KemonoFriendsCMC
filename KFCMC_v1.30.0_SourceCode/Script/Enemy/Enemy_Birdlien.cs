using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Birdlien : EnemyBase {

    public Transform quakePivot;
    public GameObject windPrefab;
    public Transform knockDownRayPivot;
    
    int attackSave = -1;
    int attackSubSave = -1;
    float windInterval;
    float rushInterval;
    bool quakeFlag;
    float continuousQuakeInterval;
    bool t_WhiteOwlAttacked;
    bool t_EagleOwlAttacked;
    bool t_GotTrophy;

    protected override void Awake() {
        base.Awake();
        mapChipSize = 1;
        normallyRequiredMaxMultiplier = 1.5f;
    }

    protected override void Start() {
        base.Start();
        if (TrophyManager.Instance) {
            t_GotTrophy = TrophyManager.Instance.IsTrophyHad(TrophyManager.t_Owls);
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (state != State.Attack) {
            quakeFlag = false;
        }
        if (state == State.Attack && quakeFlag) {
            continuousQuakeInterval -= deltaTimeCache;
            if (continuousQuakeInterval <= 0f) {
                continuousQuakeInterval = 0.03f;
                CameraManager.Instance.SetQuake(GetCenterPosition(), 3, 8, 0, 0.05f, 0f, 3f, dissipationDistance_Large);
            }
        }
    }

    void MoveAttack0_0() {
        fbStepTime = 20f / 60f;
        fbStepMaxDist = 4f;
        BackStep(6f);
    }

    void MoveAttack0_1() {
        LockonEnd();
        PerfectLockon();
        SpecialStep(-15f, 90f / 60f, 40f, 24f, 24f, true, false, EasingType.SineInOut);
        quakeFlag = true;
        continuousQuakeInterval = 0f;
    }

    void MoveAttack1() {
        fbStepTime = 20f / 60f;
        fbStepMaxDist = 4f;
        ApproachOrSeparate(1.9f);
    }

    void MoveAttack2() {
        fbStepTime = 20f / 60f;
        fbStepMaxDist = 4f;
        ApproachOrSeparate(3f);
    }

    void MoveAttack3() {
        fbStepTime = 50f / 60f;
        fbStepMaxDist = 2.5f;
        SeparateFromTarget(8f);
    }

    void MoveAttack6() {
        fbStepTime = 30f / 60f;
        fbStepMaxDist = 4f;
        StepToTarget(-3f);
    }

    void MoveAttack7() {
        fbStepTime = 30f / 60f;
        fbStepMaxDist = 1f;
        SeparateFromTarget(8f);
    }

    void MoveAttack8_0() {
        LockonEnd();
        PerfectLockon();
        SpecialStep(-10f, 60f / 60f, 20f, 16f, 16f, true, false, EasingType.SineInOut);
        quakeFlag = true;
        continuousQuakeInterval = 0f;
    }

    void MoveAttack8_1() {
        LockonEnd();
        PerfectLockon();
        SpecialStep(-15f, 60f / 60f, 30f, 20f, 20f, true, false, EasingType.SineInOut);
        quakeFlag = true;
        continuousQuakeInterval = 0f;
    }

    void QuakeEnd() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(GetCenterPosition(), 3, 8, 0, 0f, 0.6f, 3f, dissipationDistance_Large);
        }
        quakeFlag = false;
    }

    void StopAndLockonStart() {
        lockonRotSpeed = 12f;
        specialMoveDuration = 0f;
        LockonStart();
    }

    void ChangeForRushTurn() {
        attackPowerMultiplier = 1.15f;
    }

    void ThrowStartToReady(int index) {
        throwing.ThrowStart(index);
        throwing.ThrowReady(index);
    }

    void ThrowWind() {
        LockonEnd();
        int targetNum = Mathf.Clamp(CharacterManager.Instance.GetFriendsCount(), 1, 100);
        GameObject windObj = Instantiate(windPrefab, transform.position, Quaternion.identity);
        windObj.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(vecForward) * 15f, ForceMode.VelocityChange);
        windObj.GetComponentInChildren<Trap_Warp_SuperWind>().maxNum = targetNum;
        windInterval = 20f;
    }

    protected override void KnockHeavyProcess() {
        if (!fixKnockAmount && lastDamagedColorType == damageColor_Critical && attackerCB == CharacterManager.Instance.pCon && throwing.GetIsReady(0)) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_BirdlienCounter, true);
        }
        base.KnockHeavyProcess();
        agent.radius = 0.1f;
        agent.height = 0.1f;
    }

    public void QuakeHeavyKnock() {
        if (state == State.Damage) {
            CameraManager.Instance.SetQuake(quakePivot.position, 5, 4, 0, 0, 1.5f, 3f, dissipationDistance_Large);
        }
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (windInterval > 0f) {
            windInterval -= deltaTimeCache * CharacterManager.Instance.riskyIncrease;
        }
        if (rushInterval > 0f) {
            rushInterval -= deltaTimeCache * CharacterManager.Instance.riskyIncrease;
        }
    }

    protected override void SetLevelModifier() {
        dropRate[0] = 250;
        dropRate[1] = 250;
        knockRecovery = (knockRecovery + defStats.knockRecovery) * 0.5f;
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (!fixKnockAmount && !t_GotTrophy && attackerCB && CharacterManager.Instance.GetFriendsExist(13) && CharacterManager.Instance.GetFriendsExist(14)) {
            if (!t_WhiteOwlAttacked && attackerCB == CharacterManager.Instance.friends[13].fBase) {
                t_WhiteOwlAttacked = true;
            }
            if (!t_EagleOwlAttacked && attackerCB == CharacterManager.Instance.friends[14].fBase) {
                t_EagleOwlAttacked = true;
            }
        }
    }

    protected override void DeadProcess() {
        base.DeadProcess();
        if (t_WhiteOwlAttacked && t_EagleOwlAttacked && CharacterManager.Instance.GetFriendsExist(13, true) && CharacterManager.Instance.GetFriendsExist(14, true)) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_Owls, true);
        }
    }

    int GetAttackRandom() {
        float sqrDist = (targetTrans.position - trans.position).sqrMagnitude;
        if (sqrDist > 8f * 8f) {
            if (level >= 2 && windInterval <= 0 && searchArea[0].GetTargetNum > 1 && sqrDist < 20f * 20f) {
                int rand = Random.Range(0, 3);
                return (rand == 0 && rushInterval <= 3 ? 0 : rand == 2 ? 4 : 3);
            } else {
                return ((Random.Range(0, 2) == 0 && rushInterval <= 3f) ? 0 : 3);
            }
        } else {
            if (level >= 2 && windInterval <= 0 && searchArea[0].GetTargetNum > 1) {
                return Random.Range(rushInterval <= 0f ? 0 : 1, 5);
            }
            return Random.Range(rushInterval <= 0 ? 0 : 1, 4);
        }
    }

    protected override void Attack() {
        base.Attack();
        resetAgentRadiusOnChangeState = true;
        resetAgentHeightOnChangeState = true;
        if (targetTrans) {
            int attackTemp = GetAttackRandom();
            if (attackTemp == attackSave) {
                attackTemp = GetAttackRandom();
            }
            attackSave = attackTemp;
            int attackSubTemp = 0;
            if (level >= 3) {
                attackSubTemp = Random.Range(0, 2);
                if (attackSubTemp == attackSubSave) {
                    attackSubTemp = Random.Range(0, 2);
                }
            }
            attackSubSave = attackSubTemp;
            float intervalPlus = GetAttackInterval(1.5f);
            if (nowHP <= GetMaxHP() / 2) {
                intervalPlus *= 0.5f;
            }
            switch (attackTemp) {
                case 0:
                    if (attackSubTemp == 1 && level >= 4) {
                        AttackBase(8, 1f, 2.1f, 0, 210f / 60f, 210f / 60f + intervalPlus, 0.5f, 1f, true, 20f);
                    } else {
                        AttackBase(0, 1f, 2.1f, 0, 140f / 60f, 140f / 60f + intervalPlus, 0.5f, 1f, true, 20f);
                    }
                    rushInterval = 7f;
                    SuperarmorStart();
                    break;
                case 1:
                    if (attackSubTemp == 1 && level >= 3) {
                        AttackBase(5, 1.1f, 1.2f, 0, 140f / 60f, 140f / 60f + intervalPlus, 0.5f);
                    } else {
                        AttackBase(1, 1.1f, 1.2f, 0, 70f / 60f, 70f / 60f + intervalPlus, 0.5f);
                    }
                    break;
                case 2:
                    if (attackSubTemp == 1 && level >= 3) {
                        AttackBase(6, 1f, 1.3f, 0, 125f / 60f, 125f / 60f + intervalPlus);
                    } else {
                        AttackBase(2, 1f, 1.3f, 0, 85f / 60f, 85f / 60f + intervalPlus);
                    }
                    SuperarmorStart();
                    break;
                case 3:
                    if (attackSubTemp == 1 && level >= 3) {
                        AttackBase(7, 1f, 1.1f, 0, 200f / 60f, 200f / 60f);
                    } else {
                        AttackBase(3, 1f, 1.1f, 0, 120f / 60f, 120f / 60f);
                    }
                    break;
                case 4:
                    AttackBase(4, 0f, 0f, 0, 135f / 60f, 135f / 60f + intervalPlus * 0.5f, 0f);
                    fbStepTime = 30f / 60f;
                    windInterval = 10f;
                    SeparateFromTarget(3f);
                    break;
            }
        }
    }

    void ShiftOnKnockDown() {
        if (knockDownRayPivot) {
            ray.origin = knockDownRayPivot.position;
            ray.direction = knockDownRayPivot.TransformDirection(vecForward);
            if (Physics.Raycast(ray, out raycastHit, 2.5f, fieldLayerMask, QueryTriggerInteraction.Ignore)) {
                cCon.Move(knockDownRayPivot.TransformDirection(vecBack) * (2.5f - raycastHit.distance));
            }
        }
    }

}
