using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Anomalocaris : EnemyBase {

    public GameObject coreObj;
    public GameObject[] weakEffectPrefab;
    public int weakEffectIndex;
    public DamageDetection weakPoint;
    public DamageDetection criticalDD;
    public Transform ringTrans;
    public Transform ringTargetDummy;
    public Transform[] movePivot;
    
    int attackSave = -1;
    int attackSubSave = -1;
    bool coreEnabled = false;
    bool weakPointEnabled = false;
    int throwCount;
    float throwRushTimeRemain;
    float throwRushIntervalRemain;
    float gatlingAttackedTimeRemain;
    bool ringChasingEnabled;
    Vector3 ringPos = Vector3.zero;
    float ringVelocity;
    int movingIndex = -1;
    float movingSpeed;
    const int josouEffectCenterIndex = 5;
    const int josouEffectGroundIndex = 6;
    static readonly int[] rushThrowIndex = new int[] { 0, 1, 1, 1, 1, 1, 1 };

    protected override void Awake() {
        base.Awake();
        isAnimParamDetail = true;
        CoreShowHide(false);
        attackedTimeRemainOnDamage = 0.5f;
    }

    protected override void Start() {
        base.Start();
        actDistNum = Random.Range(0, 2);
        attackWaitingLockonRotSpeed = 2f;
    }

    void CoreShowHide(bool flag) {
        if (!coreEnabled && flag) {
            EmitEffect(weakEffectIndex);
        }
        coreEnabled = flag;
        if (coreObj) {
            coreObj.SetActive(flag);
        }
    }

    void ActivateWeakPoint() {
        if (weakPoint && !weakPointEnabled) {
            weakPoint.knockedRate = 30f;
            weakPoint.colorType = damageColor_Effective;
            weakPointEnabled = true;
        }
    }

    void DeactivateWeakPoint() {
        if (weakPoint && weakPointEnabled) {
            weakPoint.knockedRate = 1f;
            weakPoint.colorType = damageColor_Enemy;
            weakPointEnabled = false;
        }
    }

    void ThrowStartSpecial() {
        int throwMax = (attackSubSave == 0 ? 1 : attackSubSave == 1 ? 3 : 5);
        if (throwCount == 0) {
            throwing.ThrowStart(0);
        } else if (throwCount < throwMax) {
            throwing.ThrowStart(1);
        }
        throwCount++;
        if (throwCount < throwMax) {
            throwing.ThrowReady(1);
        }
    }

    void ThrowRushStart() {
        gatlingAttackedTimeRemain = 14f;
        throwRushTimeRemain = 80f / 60f;
        throwRushIntervalRemain = 0f;
        throwCount = 0;
    }
    
    void RingChaseStart() {
        ringChasingEnabled = true;
    }

    void RingChaseEnd() {
        ringChasingEnabled = false;
    }

    void MoveAttackJosou_0() {
        if (state == State.Attack) {
            fbStepTime = 20f / 60f;
            fbStepMaxDist = 4f;
            fbStepEaseType = EasingType.SineInOut;
            BackStep(3f);
        }
    }

    void MoveAttackJosou_1() {
        LockonEnd();
        if (state == State.Attack) {
            fbStepTime = 100f / 60f;
            fbStepMaxDist = 30f;
            fbStepEaseType = EasingType.SineInOut;
            ForwardStep(-20f);
            EmitEffect(josouEffectCenterIndex);
            EmitEffect(josouEffectGroundIndex);
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (weakPointEnabled && state != State.Attack && state != State.Damage) {
            DeactivateWeakPoint();
        }
        if (gatlingAttackedTimeRemain > 0f) {
            gatlingAttackedTimeRemain -= deltaTimeCache;
        }
        if (throwRushTimeRemain > 0f) {
            throwRushTimeRemain -= deltaTimeMove;
            throwRushIntervalRemain -= deltaTimeMove;
            if (throwRushIntervalRemain <= 0f) {
                throwRushIntervalRemain += 0.12f;
                if (throwCount < 12) {
                    throwing.ThrowStart(rushThrowIndex[throwCount % rushThrowIndex.Length]);
                    throwCount++;
                }
            }
        }
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        if (level <= 1 && !coreEnabled && nowHP > 0 && nowHP <= GetMaxHP() / 2) {
            CoreShowHide(true);
        }
    }

    protected override void SetLevelModifier() {
        if (weakEffectIndex < effect.Length) {
            effect[weakEffectIndex].prefab = weakEffectPrefab[Mathf.Clamp(level, 0, weakEffectPrefab.Length - 1)];
        }
        attackSave = -1;
        actDistNum = 1;
        CoreShowHide(false);
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        if (nowHP > 0) {
            CoreShowHide(true);
            DeactivateWeakPoint();
        }
        if (!fixKnockAmount && lastDamagedColorType == damageColor_Effective && attackerCB && attackerCB == CharacterManager.Instance.pCon && CharacterManager.Instance.pCon.CheckTrophy_IsScrewAttacking()) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_Anomalocaris, true);
        }
        actDistNum = 1;
        throwRushTimeRemain = 0f;
    }

    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        if (!isSuperarmor) {
            actDistNum = 1;
        }
    }

    void MoveAttack0() {
        if (state == State.Attack) {
            /*
            fbStepTime = 8f / 24f;
            fbStepMaxDist = 6f;
            ForwardOrBackStep(3f);
            */
            movingIndex = 0;
            movingSpeed = 1.5f;
        }
    }

    void MoveAttack1() {
        if (state == State.Attack) {
            fbStepTime = 14f / 24f;
            fbStepMaxDist = 3f;
            BackStep(6f);
        }
    }

    void MoveAttack2() {
        if (state == State.Attack) {
            /*
            if (targetTrans) {
                float sqrDist = GetTargetDistance(true, true, false);
                if (sqrDist < 1f * 1f) {
                    fbStepTime = 30f / 60f;
                    fbStepMaxDist = 4f;
                    BackStep(1f);
                } else {
                    SpecialStep(1f, 30f / 60f, 10f, 0f, 0f, true, false);
                }
            }
            */
            movingIndex = 1;
            movingSpeed = 1.2f;
        }
    }

    void MoveEnd() {
        LockonEnd();
        movingIndex = -1;
    }

    bool AngryCondition() {
        return (coreEnabled || nowHP <= GetMaxHP() / 2);
    }

    float GetAttackIntervalSpecial(int type) {
        float baseValue;
        if (type == 0) {
            baseValue = (AngryCondition() ? 0.4f : 1f) + Random.Range(-0.1f, 0.1f);
        } else {
            baseValue = (AngryCondition() ? 0.6f : 1.5f) + Random.Range(-0.1f, 0.1f);
        }
        baseValue *= attackIntervalLevelEffectRate[Mathf.Clamp(GameManager.Instance.save.difficulty - 1, 0, 3), Mathf.Clamp(level, 0, 4)];
        return baseValue;
    }

    protected override void Attack() {
        base.Attack();
        throwCount = 0;
        throwRushTimeRemain = 0f;
        resetAgentRadiusOnChangeState = true;
        ringChasingEnabled = false;
        movingIndex = -1;
        int attackTemp = Random.Range(0, 3);
        int attackSubTemp = 0;
        if (targetTrans) {
            float sqrDist = (targetTrans.position - trans.position).sqrMagnitude;
            if (sqrDist > 6f * 6f && Random.Range(0, 100) < 60) {
                attackTemp = 1;
            }
        }
        if (attackTemp == attackSave) {
            attackTemp = Random.Range(0, 3);
        }
        if (level >= 2 && attackTemp == 1) {
            if (level >= 4 && gatlingAttackedTimeRemain <= 0f && nowHP < GetMaxHP()) {
                attackSubTemp = Random.Range(1, 4);
            } else {
                attackSubTemp = Random.Range(0, level >= 3 ? 3 : 2);
            }
            if (attackSubTemp == attackSubSave) {
                attackSubTemp = Random.Range(0, 2);
            }
        } else if ((level >= 2 && attackTemp == 0) || (level >= 3 && attackTemp == 2)) {
            attackSubTemp = Random.Range(0, 2);
            if (attackSubTemp == attackSubSave) {
                attackSubTemp = Random.Range(0, 2);
            }
        }
        attackSave = attackTemp;
        attackSubSave = attackSubTemp;
        if (attackTemp == 0) {
            if (attackSubTemp == 0) {
                AttackBase(0, 1f, 1.2f, 0f, 34f / 24f, 34f / 24f + GetAttackIntervalSpecial(0), 0f, 1f, true, 12f);
            } else {
                AttackBase(3, 1.05f, 1.4f, 0, 66f / 24f, 66f / 24f + GetAttackIntervalSpecial(1), 0f, 1f, true, 12f);
            }
        } else if (attackTemp == 1) {
            if (attackSubTemp <= 2) {
                AttackBase(1, 1f, 1.1f, 0, 39f / 24f + (coreEnabled || IsSuperLevel ? 0f : 0.3f), 39f / 24f + GetAttackIntervalSpecial(1));
            } else {
                AttackBase(4, 1f, 1.1f, 0, 68f / 24f + (coreEnabled || IsSuperLevel ? 0f : 0.3f), 68f / 24f + 0.3f + GetAttackIntervalSpecial(1));
            }
        } else if (attackTemp == 2) {
            if (attackSubTemp == 0) {
                AttackBase(2, 1.2f, 2.4f, 0, 120f / 60f + (coreEnabled || IsSuperLevel ? 0f : 0.3f), 120f / 60f + GetAttackIntervalSpecial(2), 0, 1, true, 20f);
            } else {
                AttackBase(5, 1.2f, 2.4f, 0, 120f / 60f + (coreEnabled || IsSuperLevel ? 0f : 0.3f), 120f / 60f + GetAttackIntervalSpecial(2), 0, 1, true, 20f);
                if (ringTrans) {
                    ringPos.y = 0.6f;
                    ringTrans.localPosition = ringPos;
                }
            }
            agent.radius = 0.1f;
        }
        actDistNum = Random.Range(0, 2);
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (ringChasingEnabled && ringTrans && deltaTimeMove > 0f && !GetSick(SickType.Stop)) {
            Transform ringTarget = ringTargetDummy;
            if (targetTrans) {
                ringTarget = targetTrans;
            }
            float yTemp = ringTarget.position.y - trans.position.y;
            if (Mathf.Abs(yTemp - ringPos.y) > 0.01f) {
                ringPos.y = Mathf.SmoothDamp(ringPos.y, yTemp, ref ringVelocity, 0.18f);
                ringTrans.localPosition = ringPos;
            }
        }
        if (movingIndex >= 0 && movingIndex < movePivot.Length && movePivot[movingIndex]) {
            ApproachTransformPivot(movePivot[movingIndex], GetMinmiSpeed() * movingSpeed, 0.15f, 0.02f * movingSpeed, true);
        }
    }

    public override float GetMaxSpeed(bool isWalk = false, bool ignoreSuperman = false, bool ignoreSick = false, bool ignoreConfig = false) {
        return base.GetMaxSpeed(isWalk, ignoreSuperman, ignoreSick, ignoreConfig) * (AngryCondition() ? 1.5f : 1f);
    }

    public override float GetAcceleration() {
        return base.GetAcceleration() * (AngryCondition() ? 1.5f : 1f);
    }

    public override float GetAngularSpeed() {
        return base.GetAngularSpeed() * (AngryCondition() ? 1.5f : 1f);
    }

}
