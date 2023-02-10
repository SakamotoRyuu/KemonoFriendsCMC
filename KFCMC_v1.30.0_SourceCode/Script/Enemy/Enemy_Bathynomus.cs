using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class Enemy_Bathynomus : EnemyBase {
    
    public Transform rayFrom;
    public AttackDetectionParticle acidDetection;
    public LookatTarget lookatTarget;
    public Transform nullTarget;
    public DamageDetection criticalDD;

    bool quakeFlag = false;
    int attackSave = -1;
    int attackSubSave = -1;
    float knockStopRemainTime = 0f;
    float knockStopStateTime = 0f;
    float stiffPlus = 0.5f;
    bool attackMovingFlag;
    int coreShowHP = 0;
    float torpedoAttackedTimeRemain;
    float continuousQuakeInterval;

    static readonly float[] damageRateArray = new float[5] { 4f, 4f, 3.833333f, 3.666666f, 3.5f };
    static readonly float[] knockStopArray = new float[5] { 5f, 5f, 4f, 3f, 2f };
    static readonly int[] sickProbability = new int[] { 25, 33, 40, 50, 100 };
    static readonly float[] stiffPlusArray = new float[] { 0.5f, 0.5f, 0.3333333f, 0.1666666f, 0f };
    static readonly int[] dropRateArray = new int[5] { 120, 120, 120, 120, 200 };

    protected override void Awake() {
        base.Awake();
        attackWaitingLockonRotSpeed = 1f;
        defStats.knockRecovery = knockRecovery = 0f;
        supermanKnockPlus = 0f;
        cannotDoubleKnockDown = true;
    }
    
    protected override void SetLevelModifier() {
        if (acidDetection) {
            acidDetection.probability = sickProbability[Mathf.Clamp(level, 0, sickProbability.Length - 1)];
        }
        dropRate[0] = dropRateArray[Mathf.Clamp(level, 0, dropRateArray.Length - 1)];
        if (criticalDD) {
            criticalDD.damageRate = damageRateArray[Mathf.Clamp(level, 0, damageRateArray.Length - 1)];
        }
        stiffPlus = stiffPlusArray[Mathf.Clamp(level, 0, stiffPlusArray.Length - 1)];
        attackSave = -1;
    }

    protected void AttackMove_Back() {
        SetSpecialMove(trans.TransformDirection(Vector3.back), 2f, 30f / 60f, EasingType.SineInOut);
    }

    protected void AttackMove_Forward() {
        SetSpecialMove(trans.TransformDirection(Vector3.forward), 14f, 60f / 60f, EasingType.SineIn);
        attackMovingFlag = true;
    }

    protected void AttackMove_ForwardSuper() {
        SetSpecialMove(trans.TransformDirection(Vector3.forward), 25f, 60f / 60f, EasingType.SineIn);
        attackMovingFlag = true;
    }

    void ChangeLockonRotSpeed() {
        if (level >= 2 && Random.Range(0, 2) == 1) {
            lockonRotSpeed *= 0.8f;
        } else {
            LockonEnd();
        }
    }

    void AttackEndRush(int index) {
        if (state == State.Attack) {
            QuakeAttack();
            specialMoveDuration = 0f;
            attackMovingFlag = false;
            if (Physics.Raycast(new Ray(rayFrom.position, rayFrom.TransformDirection(vecForward)), 1.5f, fieldLayerMask, QueryTriggerInteraction.Ignore)) {
                EmitEffect(attackType == 6 ? 7 : 1);
            } else {
                EmitEffect(attackType == 6 ? 8 : 2);
            }
        }
        AttackEnd(index);        
    }

    protected void AttackMove_Jump() {
        if (targetTrans) {
            SpecialStep(0f, 40f / 60f, 10f, 0f, 0f, true, false, EasingType.SineOut);
        }
    }

    void QuakeAttack() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(trans.position, 10, 4, 0, 0, 1.5f, 2f, dissipationDistance_Normal);
        }
    }

    void SetQuakeFlag(int flag) {
        quakeFlag = (flag != 0);
        continuousQuakeInterval = 0f;
    }

    void KnockStop() {
        knockStopRemainTime = knockStopArray[Mathf.Clamp(level, 0, knockStopArray.Length - 1)];
        knockRestoreSpeed = 0f;
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 0f);
        knockStopStateTime = stateTime;
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (!fixKnockAmount && knockRemain <= 0f && state == State.Attack && lastDamagedColorType == damageColor_Critical && attackerCB && attackerCB == CharacterManager.Instance.pCon) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_Bathynomus, true);
        }
    }

    protected override void KnockHeavyProcess() {
        if (state != State.Damage || !isDamageHeavy) {
            base.KnockHeavyProcess();
            knockRestoreSpeed = 1f;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 1f);
            coreShowHP = nowHP;
            agent.radius = 0.1f;
            agent.height = 0.1f;
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
                if (attackType == 6) {
                    CameraManager.Instance.SetQuake(trans.position, 4, 4, 0, 0.05f, 0f, 2.5f, dissipationDistance_Normal * 1.5f);
                } else {
                    CameraManager.Instance.SetQuake(trans.position, 3, 4, 0, 0.05f, 0f, 1.5f, dissipationDistance_Normal);
                }
            }
        }
        if (lookatTarget) {
            if (targetTrans) {
                lookatTarget.SetTarget(targetTrans);
            } else {
                lookatTarget.SetTarget(nullTarget);
            }
        }
        if (knockStopRemainTime > 0f) {
            if (state == State.Damage) {
                stateTime = knockStopStateTime;
                knockStopRemainTime -= deltaTimeCache;
                knockRemain = knockEndurance;
                knockRemainLight = knockEnduranceLight;
                if (nowHP <= coreShowHP - GetMaxHP() / 5) {
                    knockStopRemainTime = -1f;
                }
                if (knockStopRemainTime <= 0f) {
                    knockRestoreSpeed = 1f;
                    anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 1f);
                }
            } else {
                knockStopRemainTime = 0f;
            }
        }
        if (torpedoAttackedTimeRemain > 0f) {
            torpedoAttackedTimeRemain -= deltaTimeCache;
        }
        actDistNum = (level >= 3 && torpedoAttackedTimeRemain <= 0f ? 1 : 0);
    }

    protected override void Attack() {
        base.Attack();
        attackMovingFlag = false;
        resetAgentRadiusOnChangeState = true;
        resetAgentHeightOnChangeState = true;
        int attackTemp = Random.Range(0, 3);
        int attackSubTemp = 0;
        if (attackSave == attackTemp) {
            attackTemp = Random.Range(0, 3);
        }
        if (level >= 2) {
            if (attackTemp == 0 && level >= 4) {
                attackSubTemp = Random.Range(0, 3);
                if (attackSubTemp == attackSubSave) {
                    attackSubTemp = Random.Range(0, 2);
                }
            } else if (attackTemp == 2 && level < 3) {
                attackSubTemp = 0;
            } else {
                attackSubTemp = Random.Range(0, 2);
                if (attackSubTemp == attackSubSave) {
                    attackSubTemp = Random.Range(0, 2);
                }
            }
        }
        if (level >= 3 && actDistNum == 1 && targetTrans && GetTargetDistance(true, true, false) > 9.5f * 9.5f) {
            attackTemp = 2;
            attackSubTemp = 1;
        }
        attackSave = attackTemp;
        attackSubSave = attackSubTemp;
        float stiffTemp = stiffPlus;
        if (IsSuperLevel) {
            stiffTemp = 0f;
        }
        switch (attackTemp) {
            case 0:
                if (attackSubTemp == 0) {
                    AttackBase(0, 1, 1.4f, 0, 150f / 60f + stiffTemp, 150f / 60f + stiffTemp + GetAttackInterval(1.5f));
                } else if (attackSubTemp == 1) {
                    AttackBase(3, 1, 1.4f, 0, 150f / 60f + stiffTemp, 150f / 60f + stiffTemp + GetAttackInterval(1.5f, -1));
                } else {
                    AttackBase(6, 1.25f, 3f, 0, 180f / 60f + stiffTemp, 180f / 60f + stiffTemp + GetAttackInterval(1.5f, -3));
                    SuperarmorStart();
                }
                break;
            case 1:
                if (attackSubTemp == 0) {
                    AttackBase(1, 1.05f, 2.4f, 0, 110f / 60f + stiffTemp, 110f / 60f + stiffTemp + GetAttackInterval(1.5f));
                } else {
                    AttackBase(4, 1.05f, 2.4f, 0, 110f / 60f + stiffTemp, 110f / 60f + stiffTemp + GetAttackInterval(1.5f, -1));
                }
                agent.radius = 0.05f;
                break;
            case 2:
                if (attackSubTemp == 0) {
                    AttackBase(2, 0.6f, 0.6f, 0, 80f / 60f + stiffTemp, 80f / 60f + stiffTemp + GetAttackInterval(1.5f));
                } else {
                    AttackBase(5, 1f, 1.2f, 0, 80f / 60f + stiffTemp, 80f / 60f + stiffTemp + GetAttackInterval(1.5f));
                    torpedoAttackedTimeRemain = Random.Range(5f, 7f);
                }
                break;
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (attackMovingFlag && isLockon && specialMoveDuration > 0f) {
            specialMoveDirection = trans.TransformDirection(Vector3.forward);
        }
    }
}
