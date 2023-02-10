using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Glaucus : EnemyBaseBoss {
    
    public Transform quakeHeavyKnockPivot;
    public Transform quakePressAttackPivot;
    public Transform[] movePivot;
    public bool isRaw;

    int attackSave = -1;
    float attackSpeed = 1;
    float knockStopStateTime = 0f;
    int throwReservedIndex = 0;
    int attackTypeSub = 0;
    int moveIndex = -1;
    float throwRushTimeRemain;
    float throwRushIntervalRemain;
    int rushCount;
    float lastDamagedTime;
    const int rushThrowIndex = 9;

    protected override void Awake() {
        base.Awake();
        deadTimer = 3;
        attackedTimeRemainOnDamage = 1f;
        attackWaitingLockonRotSpeed = 2f;
        attackLockonDefaultSpeed = 5f;
        sandstarRawLockonSpeed = 10f;
        checkGroundPivotHeight = 1f;
        checkGroundTolerance = 2f;
        checkGroundTolerance_Jumping = 2f;
        sandstarRawKnockEndurance = 5000;
        sandstarRawKnockEnduranceLight = 5000f / 3f;
        killByCriticalOnly = true;
        coreShowHP = GetMaxHP();
        if (isRaw) {
            coreTimeMax = 6f;
        } else {
            coreTimeMax = 8f;
        }
        // coreHideDenomi = 7.5f;
        coreHideDenomi = 5.5f;
    }



    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (!battleStarted && target) {
            lastDamagedTime = 0f;
            BattleStart();
        }
        if (isRaw) {
            attackedTimeRemainOnDamage = 0.1f;
        } else {
            attackedTimeRemainOnDamage = (weakProgress >= 2 ? 0.5f : weakProgress == 1 ? 0.8f : 1f);
        }
        if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled) {
            weakProgress = (nowHP <= GetMaxHP() / 3 ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 0);
        } else {
            weakProgress = (nowHP <= GetMaxHP() / 2 ? 1 : 0);
        }
        attackSpeed = (weakProgress == 2 ? 1.15f : 1);
        if (coreTimeRemain > 0f) {
            if (state == State.Damage) {
                stateTime = knockStopStateTime;
                coreTimeRemain -= deltaTimeCache * (nowHP > 1 ? CharacterManager.Instance.riskyIncSqrt : 1f);
                knockRemain = knockEndurance;
                knockRemainLight = knockEnduranceLight;
                if (nowHP <= GetCoreHideBorder()) {
                    coreTimeRemain = -1f;
                }
                if (coreTimeRemain <= 0f) {
                    knockRestoreSpeed = 1f;
                    attackedTimeRemain = 0f;
                    anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 1f);
                }
            } else {
                coreTimeRemain = 0f;
            }
        }
        if (state == State.Attack && throwRushTimeRemain > 0f) {
            throwRushTimeRemain -= deltaTimeMove;
            throwRushIntervalRemain -= deltaTimeMove;
            if (throwRushIntervalRemain <= 0f) {
                throwRushIntervalRemain += 0.0916667f;
                if (rushCount < 17) {
                    throwing.ThrowStart(rushThrowIndex);
                    rushCount++;
                }
            }
        }
        if (battleStarted && lastDamagedTime < 20f) {
            lastDamagedTime += deltaTimeCache;
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        lastDamagedTime = 0f;
    }

    void QuakeHeavyKnock() {
        if (state == State.Damage) {
            CameraManager.Instance.SetQuake(quakeHeavyKnockPivot.position, 5, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
        }
    }

    void QuakeAttackPress() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePressAttackPivot.position, 14, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
        }
    }

    void ThrowSpecialReady() {
        if (attackTypeSub == 1) {
            throwing.ThrowReady(throwReservedIndex);
        }
    }

    void ThrowSpecial() {
        throwing.ThrowStart(throwReservedIndex);
    }

    protected override void KnockLightProcess() {
        if (state != State.Damage) {
            base.KnockLightProcess();
            if (!isSuperarmor) {
                knockRestoreSpeed = 1f;
                throwRushTimeRemain = 0f;
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 1f);
            }
        }
    }

    protected override void KnockHeavyProcess() {
        if (state != State.Damage || !isDamageHeavy) {
            base.KnockHeavyProcess();
            knockRestoreSpeed = 1f;
            throwRushTimeRemain = 0f;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 1f);
            coreShowHP = nowHP;
            coreHideConditionDamage = GetCoreHideConditionDamage();
            agent.radius = 0.1f;
            agent.height = 0.1f;
        }
    }

    public override void SetSandstarRaw() {
        base.SetSandstarRaw();
        if (state != State.Dead) {
            coreHideDenomi = 8f;
            coreTimeMax = 3f;
            lastDamagedTime = 0f;
            if (coreTimeRemain > 0.01f) {
                coreTimeRemain = 0.01f;
            }
        }
    }

    void MoveAttack0() {
        if (state != State.Damage) {
            if (attackTypeSub == 2) {
                SpecialStep(0f, 0.666666f, 6f, 0, 0f, true, true);
            } else {
                fbStepMaxDist = 6f;
                fbStepTime = 0.66666f;
                BackStep(6f);
            }
        }
    }

    void MoveAttack1() {
        if (state != State.Damage) {
            fbStepMaxDist = 4f;
            fbStepTime = 0.5f;
            BackStep(6f);
        }
    }

    void MoveAttack2() {
        if (targetTrans && GetTargetDistance(true, true, false) < 4f * 4f) {
            fbStepMaxDist = 10f;
            fbStepTime = 0.5f / attackSpeed;
            ApproachOrSeparate(4f);
        } else {
            float dist = 0f;
            if (targetTrans) {
                dist = GetTargetDistance(false, true, false);
            }
            SpecialStep(4f, Mathf.Clamp((dist - 4f) * 0.09f, 0.5f, 0.9f) / attackSpeed, 20f, 0, 0, true, false);
        }
    }

    void MoveStart(int index) {
        moveIndex = index;
    }

    void MoveEnd() {
        moveIndex = -1;
        LockonEnd();
    }
    
    void KnockStop() {
        coreTimeRemain = coreTimeMax;
        knockRestoreSpeed = 0f;
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 0f);
        knockStopStateTime = stateTime;
    }

    protected override void Attack() {
        resetAgentRadiusOnChangeState = true;
        resetAgentHeightOnChangeState = true;
        moveIndex = -1;
        throwRushTimeRemain = 0f;
        if (targetTrans) {
            int tempMax = 5;
            if ((targetTrans.position - trans.position).sqrMagnitude < 12f * 12f) {
                tempMax = 6;
            }
            if (weakProgress >= 2) {
                tempMax = 7;
            }
            int attackTemp = Random.Range(0, tempMax);
            if (attackTemp == attackSave) {
                attackTemp = Random.Range(0, tempMax);
            }
            if (attackTemp == attackSave && (attackTemp == 4 || attackTemp == 6)) {
                if ((targetTrans.position - trans.position).sqrMagnitude < 12f * 12f) {
                    attackTemp = Random.Range(0, 5);
                    if (attackTemp == 4) {
                        attackTemp = 5;
                    }
                } else {
                    attackTemp = Random.Range(0, 4);
                }
            }
            attackSave = attackTemp;
            float intervalPlus = (weakProgress == 2 ? Random.Range(0.4f, 0.6f) : weakProgress == 1 ? Random.Range(0.6f, 0.9f) : Random.Range(0.9f, 1.2f));
            bool hpHalf = (weakProgress >= 1);
            attackTypeSub = attackTemp;
            if (battleStarted && lastDamagedTime > 10f && targetTrans && GetTargetDistance(true, true, false) > 20f * 20f) {
                intervalPlus = 3f;
            }
            switch (attackTemp) {
                case 0:
                    throwReservedIndex = hpHalf ? 2: 1;
                    agent.radius = 0.1f;
                    AttackBase(0, 1, 1.1f, 0, (160f / 60f) / attackSpeed, (160f / 60f) / attackSpeed + intervalPlus, 1, attackSpeed);
                    break;
                case 1:
                    throwReservedIndex = hpHalf ? 4 : 3;
                    agent.radius = 0.1f;
                    AttackBase(0, 1, 1.1f, 0, (160f / 60f) / attackSpeed, (160f / 60f) / attackSpeed + intervalPlus, 1, attackSpeed);
                    break;
                case 2:
                    throwReservedIndex = hpHalf ? 6 : 5;
                    agent.radius = 0.1f;
                    AttackBase(0, 1, 1.1f, 0, (160f / 60f) / attackSpeed, (160f / 60f) / attackSpeed + intervalPlus, 1, attackSpeed);
                    break;
                case 3:
                    throwReservedIndex = hpHalf ? 8 : 7;
                    agent.radius = 0.1f;
                    AttackBase(0, 1, 1.1f, 0, (160f / 60f) / attackSpeed, (160f / 60f) / attackSpeed + intervalPlus, 1, attackSpeed);
                    break;
                case 4:
                    AttackBase(1, 1.1f, 1.7f, 0, (140f / 60f) / attackSpeed + 0.5f, (140f / 60f) / attackSpeed + 0.5f + intervalPlus, 1, attackSpeed);
                    break;
                case 5:
                    if (weakProgress <= 1) {
                        AttackBase(2, 1.24f, 2.4f, 0, 220f / 60f, 220f / 60f + intervalPlus, 1, 1, true, attackLockonDefaultSpeed * 2f);
                    } else {
                        AttackBase(3, 1.24f, 2.4f, 0, (260f / 60f) / attackSpeed, (260f / 60f) / attackSpeed + intervalPlus, 1, attackSpeed, true, attackLockonDefaultSpeed * 2f);
                    }
                    break;
                case 6:
                    agent.radius = 0.1f;
                    AttackBase(4, 1, 1.1f, 0, 230f / 60f, 230f / 60f + intervalPlus, 1f, 1f, true, attackLockonDefaultSpeed * 2f);
                    SuperarmorStart();
                    break;
            }
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (moveIndex >= 0 && moveIndex < movePivot.Length && movePivot[moveIndex]) {
            ApproachTransformPivot(movePivot[moveIndex], GetMinmiSpeed() * 1.2f * attackSpeed, 1f, 0.2f, true);
        }
    }

    void ThrowRushStart() {
        throwRushTimeRemain = 90f / 60f;
        throwRushIntervalRemain = 0f;
        rushCount = 0;
    }

    void ThrowRushEnd() {
        throwRushTimeRemain = 0f;
    }

    public override float GetAcceleration() {
        float acceleration = base.GetAcceleration();
        if (acceleration < 12f && targetTrans && GetTargetDistance(true, true, false) > 20f * 20f) {
            acceleration = 12f;
        }
        return acceleration;
    }

}
