using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ammonitida : EnemyBase {

    public GameObject[] cores;
    public DamageDetection[] criticalDD;
    public ParticleSystem[] coreParticles;
    public Transform shield;
    public Transform shieldCenter;
    
    int attackSave = -1;
    int attackSubSave = -1;
    bool shieldMoving;
    float projectileAttackedTimeRemain;
    float shieldAttackedTimeRemain;
    float stiffPlus = 0.4f;
    bool approachingFlag;
    int coreShowIndex;
    const int upperCoreIndex = 0;
    static readonly float[] damageRateArray = new float[5] { 4f, 4f, 3.833333f, 3.666666f, 3.5f };
    static readonly float[] stiffPlusArray = new float[] { 0.4f, 0.4f, 0.2666666f, 0.1333333f, 0f };

    protected override void Awake() {
        base.Awake();
        coreShowIndex = Random.Range(0, cores.Length);
        for (int i = 0; i < cores.Length; i++) {
            cores[i].SetActive(i == coreShowIndex);
        }
        attackWaitingLockonRotSpeed = 1.5f;
        isAnimParamDetail = true;
    }

    protected override void SetLevelModifier() {
        for (int i = 0; i < criticalDD.Length; i++) {
            if (criticalDD[i]) {
                criticalDD[i].damageRate = damageRateArray[Mathf.Clamp(level, 0, damageRateArray.Length - 1)];
            }
        }
        stiffPlus = stiffPlusArray[Mathf.Clamp(level, 0, stiffPlusArray.Length - 1)];
        attackSave = -1;
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (projectileAttackedTimeRemain > 0f) {
            projectileAttackedTimeRemain -= deltaTimeMove;
        }
        if (shieldAttackedTimeRemain > 0f) {
            shieldAttackedTimeRemain -= deltaTimeMove;
        }
        if (state == State.Attack && shieldMoving && targetTrans) {
            // shield.rotation = Quaternion.Slerp(shield.rotation, Quaternion.LookRotation(targetTrans.position - shield.position), deltaTimeCache * 3f);
            SmoothRotation(shield, targetTrans.position - shield.position, 3f);
        }
        if (level >= 2 && coreShowIndex == upperCoreIndex && shieldAttackedTimeRemain <= 0f && state != State.Attack && attackedTimeRemain > 0f && targetTrans && GetTargetDistance(true, true, false) < 2f * 2f && targetTrans.position.y > trans.position.y + 3f) {
            attackedTimeRemain = 0f;
        }
        actDistNum = (projectileAttackedTimeRemain > 0f ? 0 : 1);
    }

    void ShieldMoveEnd() {
        shieldMoving = false;
    }

    void ThrowReadyAll() {
        for (int i = 0; i < throwing.throwSettings.Length; i++) {
            throwing.ThrowReady(i);
        }
    }
    
    void ApproachEnd() {
        approachingFlag = false;
        LockonEnd();
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        coreShowIndex = (coreShowIndex + 1) % cores.Length;
        for (int i = 0; i < cores.Length; i++) {
            cores[i].SetActive(i == coreShowIndex);
        }
        if (coreParticles.Length > coreShowIndex && coreParticles[coreShowIndex]) {
            coreParticles[coreShowIndex].Play();
        }
    }

    void MoveAttack0() {
        if (state == State.Attack && targetTrans) {
            float dist = 4f;
            if ((targetTrans.position - trans.position).sqrMagnitude > dist * dist) {
                SpecialStep(dist, 15f / 24f, 4, 0, 0, true, false);
            } else {
                fbStepTime = 15f / 24f;
                fbStepMaxDist = 4f;
                BackStep(dist);
            }
        }
    }

    void MoveAttack1() {
        if (state == State.Attack) {
            fbStepTime = 15f / 24f;
            fbStepMaxDist = 5f;
            BackStep(7f);
        }
    }

    void MoveAttack2() {
        if (state == State.Attack) {
            SetSpecialMove(trans.TransformDirection(vecForward), 12f, 18f / 24f, EasingType.SineInOut);
        }
    }

    int GetAttackRandom() {
        if (level >= 2 && coreShowIndex == upperCoreIndex && Random.Range(0, 100) < 80 - shieldAttackedTimeRemain * 10f && targetTrans && GetTargetDistance(true, true, false) < 2f * 2f && targetTrans.position.y > trans.position.y + 3f) {
            return 3;
        }
        if (level <= 1) {
            return Random.Range(0, 3);
        } else if (level <= 3){
            return Random.Range(0, 4);
        } else {
            return Random.Range(0, 5);
        }
    }

    protected override void Attack() {
        base.Attack();
        shieldMoving = false;
        approachingFlag = false;
        int attackTemp = GetAttackRandom();
        if (attackTemp == attackSave) {
            attackTemp = GetAttackRandom();
        }
        if (targetTrans && GetTargetDistance(true, true, false) > 10f * 10f) {
            attackTemp = 1;
        }
        int attackSubTemp = 0;
        if ((level >= 3 && attackTemp == 1) || (level >= 3 && attackTemp == 2)) {
            attackSubTemp = Random.Range(0, 2);
            if (attackSubTemp == attackSubSave) {
                attackSubTemp = Random.Range(0, 2);
            }
        }
        attackSave = attackTemp;
        attackSubSave = attackSubTemp;
        float stiffTemp = stiffPlus;
        if (IsSuperLevel) {
            stiffTemp = 0f;
        }
        switch (attackTemp) {
            case 0:
                AttackBase(0, 1, 0.8f, 0, 40f / 24f, 40f / 24f + GetAttackInterval(1f));
                break;
            case 1:
                if (attackSubTemp == 1) {
                    AttackBase(4, 1.05f, 1.2f, 0, 42f / 24f + stiffTemp, 42f / 24f + stiffTemp + GetAttackInterval(1f, -2));
                } else {
                    AttackBase(1, 1.05f, 1.2f, 0, 40f / 24f + stiffTemp, 40f / 24f + stiffTemp + GetAttackInterval(1f));
                }
                projectileAttackedTimeRemain = Random.Range(5f, 7f);
                break;
            case 2:
                if (attackSubTemp == 1) {
                    AttackBase(5, 1.15f, 1.7f, 0, 62f / 24f + 0.2f + stiffTemp * 2f, 62f / 24f + 0.2f + stiffTemp * 2f + GetAttackInterval(1f, -2));
                } else {
                    AttackBase(2, 1.15f, 1.7f, 0, 62f / 24f + 0.2f + stiffTemp * 2f, 62f / 24f + 0.2f + stiffTemp * 2f + GetAttackInterval(1f));
                }
                break;
            case 3:
                if (targetTrans) {
                    shield.rotation = Quaternion.LookRotation(targetTrans.position - shield.position);
                } else {
                    shield.localRotation = quaIden;
                }
                shield.position = shieldCenter.position;
                shieldMoving = true;
                shieldAttackedTimeRemain = 8f;
                AttackBase(3, 1.0f, 1.1f, 0, 68f / 30f, 68f / 30f + GetAttackInterval(1f, -1), 1f, 1f, false);
                break;
            case 4:
                AttackBase(6, 1.2f, 1.5f, 0, 68f / 24f, 68f / 24f + GetAttackInterval(1f, -3), 0f, 1f, true, 3f);
                shield.position = shieldCenter.position;
                shield.localRotation = quaIden;
                approachingFlag = true;
                break;
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (approachingFlag) {
            Continuous_Approach(GetMaxSpeed(false, false, true) * 1.1f, 1f, 0.1f, true, false);
        }
    }

}
