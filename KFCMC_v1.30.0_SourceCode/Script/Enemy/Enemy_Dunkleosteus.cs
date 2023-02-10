using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dunkleosteus : EnemyBase {

    [System.Serializable]
    public class ObjectSet {
        public GameObject[] activateObj;
        public GameObject[] deactivateObj;
    }

    public GameObject[] corePoints;
    public ObjectSet[] coreAddObj;
    public ParticleSystem[] coreParticles;
    public Transform[] quakePivot;
    public Transform boltOrigin;
    public Transform boltThrowFrom;

    int attackSave = -1;
    int nowPoint = -1;
    int isMoving;
    bool judgementReadyFlag;
    float judgementTimeElapsed;
    float ghostAttackedTimeRemain = -1f;

    static readonly float[] intervalPlusArray = new float[] { 0.8f, 0.8f, 0.7f, 0.6f, 0.5f };
    static readonly float[] knockRestoreArray = new float[] { 1f, 1f, 1.2f, 1.4f, 1.6f };
    const int judgementAttackIndex = 5;
    const int judgementThrowIndex = 1;
    const int volcanoThrowIndex = 2;
    const int headCoreIndexL = 0;
    const int headCoreIndexR = 1;

    protected override void Awake() {
        base.Awake();
        resetAgentRadiusOnChangeState = true;
        attackWaitingLockonRotSpeed = 0.7f;
        stoppingDistanceBattle = 4f;
        mapChipSize = 1;
        normallyRequiredMaxMultiplier = 1.5f;
    }

    void SetCorePoint() {
        if (corePoints.Length > 0) {
            bool particlePlayFlag = false;
            if (nowPoint < 0) {
                nowPoint = Random.Range(0, corePoints.Length);
            } else {
                nowPoint = (nowPoint + 1) % corePoints.Length;
                particlePlayFlag = true;
            }
            for (int i = 0; i < corePoints.Length; i++) {
                if (corePoints[i]) {
                    corePoints[i].SetActive(i == nowPoint);
                }
            }
            if (particlePlayFlag && coreParticles.Length > nowPoint && coreParticles[nowPoint]) {
                coreParticles[nowPoint].Play();
            }
        }
        if (nowPoint < coreAddObj.Length) {
            for (int i = 0; i < coreAddObj[nowPoint].activateObj.Length; i++) {
                if (coreAddObj[nowPoint].activateObj[i]) {
                    coreAddObj[nowPoint].activateObj[i].SetActive(true);
                }
            }
            for (int i = 0; i < coreAddObj[nowPoint].deactivateObj.Length; i++) {
                if (coreAddObj[nowPoint].deactivateObj[i]) {
                    coreAddObj[nowPoint].deactivateObj[i].SetActive(false);
                }
            }
        }
    }

    protected override void SetLevelModifier() {
        nowPoint = -1;
        SetCorePoint();
        dropRate[0] = 250;
        dropRate[1] = 250;
        knockRecovery = (knockRecovery + defStats.knockRecovery) * 0.5f;
        knockRestoreSpeed = knockRestoreArray[Mathf.Clamp(level, 0, knockRestoreArray.Length - 1)];
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
    }

    void MoveAttack(int index) {
        float sqrDist = GetTargetDistance(true, true, false);
        switch (index) {
            case 0:
                if (targetTrans && targetTrans.position.y > GetCenterPosition().y + 0.5f && sqrDist < 3.4f * 3.4f) {
                    fbStepTime = 12f / 60f;
                    fbStepMaxDist = 4f;
                    SeparateFromTarget(3.4f);
                } else if (sqrDist < 2.6f * 2.6f) {
                    fbStepTime = 12f / 60f;
                    fbStepMaxDist = 4f;
                    SeparateFromTarget(2.6f);
                } else {
                    SpecialStep(2.6f, 12f / 60f, 4f, 0f, 0f, true, false);
                }
                break;
            case 1:
            case 2:
                SpecialStep(3.8f, 25f / 60f, 3f, 0f, 0f, true, false);
                break;
            case 3:
                SpecialStep(0f, 20f / 60f, 3f, 0f, 0f, true, false);
                break;
            case 4:
                if (sqrDist < 3f * 3f) {
                    fbStepTime = 30f / 60f;
                    fbStepMaxDist = 4f;
                    SeparateFromTarget(3f);
                } else {
                    SpecialStep(3.5f, 30f / 60f, 4.5f, 0f, 0f, true, false);
                }
                break;
            case 8:
                if (sqrDist < 7f * 7f) {
                    fbStepTime = 30f / 60f;
                    fbStepMaxDist = 4f;
                    SeparateFromTarget(7f);
                }
                break;
        }
    }

    void QuakeAttack(int index) {
        if (state == State.Attack) {
            switch (index) {
                case 0:
                    CameraManager.Instance.SetQuake(quakePivot[0].position, 12, 4, 0, 0, 1.5f, 3f, dissipationDistance_Large);
                    break;
                case 1:
                    CameraManager.Instance.SetQuake(quakePivot[1].position, 12, 4, 0, 0, 1.5f, 3f, dissipationDistance_Large);
                    break;
                case 2:
                    CameraManager.Instance.SetQuake(quakePivot[1].position, 10, 4, 0, 0, 1.5f, 3f, dissipationDistance_Large);
                    break;
            }
        }
    }
    
    void MovingFlag(int flag) {
        isMoving = flag;
    }

    int GetAttackType() {
        return Random.Range(0, level >= 3 && ghostAttackedTimeRemain <= 0f ? 5 : 4);
    }

    void JudgementReady() {
        if (level >= 4 && state == State.Attack) {
            judgementReadyFlag = true;
            AttackStart(judgementAttackIndex);
        }
    }

    void JudgementStart() {
        if (judgementReadyFlag) {
            judgementReadyFlag = false;
            AttackEnd(judgementAttackIndex);
            SetTransformPositionToGround(boltOrigin, boltThrowFrom, 0.5f);
            throwing.ThrowStart(judgementThrowIndex);
            Vector3 posTemp = vecZero;
            Vector2 randCircle;
            int maxNum = Mathf.Clamp((int)(judgementTimeElapsed * 2.5f), 5, 20);
            for (int i = 0; i < maxNum; i++) {
                randCircle = Random.insideUnitCircle;
                posTemp.x = randCircle.x;
                posTemp.z = randCircle.y;
                throwing.throwSettings[volcanoThrowIndex].from.transform.localPosition = posTemp;
                throwing.ThrowStart(volcanoThrowIndex);
            }
            judgementTimeElapsed = 0f;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (judgementReadyFlag && (level < 4 || state != State.Attack)) {
            judgementReadyFlag = false;
            AttackEnd(judgementAttackIndex);
        }
    }

    protected override void Update_TimeCount() {
        if (judgementTimeElapsed < 20f) {
            judgementTimeElapsed += deltaTimeCache * CharacterManager.Instance.riskyIncrease;
        }
        if (ghostAttackedTimeRemain > 0f) {
            ghostAttackedTimeRemain -= deltaTimeCache;
        }
        base.Update_TimeCount();
    }

    protected override void DeadProcess() {
        base.DeadProcess();
        if (!fixKnockAmount && lastDamagedColorType == damageColor_Critical) {
            CharacterManager.Instance.CheckTrophy_BrownBear(attackerCB);
        }
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        Vector3 centerPos = GetCenterPosition();
        float sqrDist = (new Vector3(centerPos.x, position.y, centerPos.z) - position).sqrMagnitude;
        int direction = 2;
        if (sqrDist < 0.8f * 0.8f && position.y > centerPos.y + 0.6f) {
            direction = 0;
        } else {
            float angle = Vector3.Cross(trans.TransformDirection(vecForward), position - centerPos).y;
            if (angle < -0.2f) {
                direction = -1;
            } else if (angle > 0.2f) {
                direction = 1;
            }
        }
        anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.StepDirection], direction);
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    protected override void Attack() {
        base.Attack();
        int attackTemp = GetAttackType();
        float baseInterval = 0.5f;
        float maxHPTemp = GetMaxHP();
        if (maxHPTemp >= 1f) {
            baseInterval += Mathf.Clamp01(nowHP / maxHPTemp) * intervalPlusArray[Mathf.Clamp(level, 0, intervalPlusArray.Length - 1)];
        }
        isMoving = 0;
        if (attackTemp == attackSave) {
            attackTemp = GetAttackType();
        }
        attackSave = attackTemp;
        float addStiff = (IsSuperLevel ? 0f : 0.3f);
        if (attackTemp == 0) {
            float intervalTemp = GetAttackInterval(baseInterval * 0.7f);
            AttackBase(0, 1.1f, 1.2f, 0, 90f / 60f + Mathf.Min(intervalTemp, addStiff), 90f / 60f + intervalTemp, 0);
        } else if (attackTemp == 1) {
            int attackSub = Random.Range(0, 2);
            if (targetTrans) {
                float angle = Vector3.Cross(trans.TransformDirection(vecForward), targetTrans.position - trans.position).y;
                if (angle < 0) {
                    attackSub = 0;
                } else if (angle > 0) {
                    attackSub = 1;
                }
            }
            if (level >= 2 && Random.Range(0, 100) < 50) {
                float intervalTemp = GetAttackInterval(baseInterval, -1);
                AttackBase(6 + attackSub, 1f, 1.4f, 0, 145f / 60f + Mathf.Min(intervalTemp, addStiff), 145f / 60f + intervalTemp, 1, 1, false);
            } else {
                float intervalTemp = GetAttackInterval(baseInterval);
                AttackBase(1 + attackSub, 1f, 1.4f, 0, 120f / 60f + Mathf.Min(intervalTemp, addStiff), 120f / 60f + intervalTemp, 1, 1, false);
            }
        } else if (attackTemp == 2) {
            agent.radius = 0.1f;
            float intervalTemp = GetAttackInterval(baseInterval);
            AttackBase(3, 1.25f, 2.2f, 0, 145f / 60f + Mathf.Min(intervalTemp, addStiff), 145f / 60f + intervalTemp, 0, 1);
        } else if (attackTemp == 3) {
            if (level >= 2 && Random.Range(0, 100) < 50) {
                float intervalTemp = GetAttackInterval(baseInterval, -1);
                AttackBase(5, 1.15f, 1.7f, 0, 255f / 60f + Mathf.Min(intervalTemp, addStiff), 255f / 60f + intervalTemp);
            } else {
                float intervalTemp = GetAttackInterval(baseInterval);
                AttackBase(4, 1.15f, 1.7f, 0, 170f / 60f + Mathf.Min(intervalTemp, addStiff), 170f / 60f + intervalTemp);
            }
        } else if (attackTemp == 4) {
            float intervalTemp = GetAttackInterval(baseInterval);
            AttackBase(8, 1f, 0.8f, 0, 120f / 60f + Mathf.Min(intervalTemp, addStiff), 120f / 60f + intervalTemp, 0f);
            ghostAttackedTimeRemain = 12f;
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (isMoving != 0 && targetTrans) {
            Continuous_Approach(GetMaxSpeed(false, false, true), 4.4f, 3.8f, true, false);
        }
    }

}
