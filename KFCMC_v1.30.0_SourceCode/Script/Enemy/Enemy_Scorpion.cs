using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Scorpion : EnemyBase {

    public AttackDetectionSick attackDetectionSick;
    public Transform quakePivot_Mouth;
    public float animSpeedRate = 1;
    public bool roarEnabled = false;
    public float roarSpeed = 2;
    public float roarBackDist = 5;
    public GameObject[] materialObj;
    public Material defaultMaterial;
    public Material specialMaterial;
    public float counterAirY = 1f;
    public float throwConditionDistance = 5f;
    public Transform[] movePivot;
    public CheckTriggerStay counterChecker;

    int attackSave = -1;
    float angryTimeRemain = 0;
    float checkerStayTime;
    int moveIndex = -1;
    float moltTimeRemain;
    const int effThrowReady = 1;
    const int effImpact = 4;
    const float throwSpeedMin = 24f;
    const float throwSpeedUpBorderDistance = 12f;
    const float throwSpeedUpRate = 0.4f;
    static readonly int[] sickProbability = new int[] { 25, 25, 33, 40, 50 };

    protected override void Awake() {
        base.Awake();
        attackWaitingLockonRotSpeed = 2f;
        fbStepEaseType = EasingType.SineInOut;
        destinationUpdateInterval = 0.2f;
        if (roarEnabled) {
            mapChipSize = 1;
        }
        if (roarEnabled) {
            dropRate[0] = 250;
            dropRate[1] = 250;
            stoppingDistanceBattle = 3f;
            normallyRequiredMaxMultiplier = 1.5f;
        } else {
            stoppingDistanceBattle = 1.5f;
        }
    }

    void MoveStart(int index) {
        moveIndex = index;
        LockonStart();
    }

    void MoveEnd() {
        moveIndex = -1;
        LockonEnd();
    }


    protected override void Update_AnimControl() {
        base.Update_AnimControl();
        UpdateAC_Run();
    }

    protected override void UpdateAC_AnimSpeed() {
        float floatTemp = (GetSick(SickType.Mud) ? 0.5f : 1) * animSpeedRate;
        if (animParam.animSpeed != floatTemp) {
            animParam.animSpeed = floatTemp;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AnimSpeed], animParam.animSpeed);
        }
    }

    void RoarAngry() {
        checkerStayTime = 0f;
        if (counterChecker.stayObj) {
            CharacterBase cBaseTemp = counterChecker.stayObj.GetComponentInParent<CharacterBase>();
            if (cBaseTemp) {
                attackerObj = cBaseTemp.gameObject;
                searchArea[0].SetLockTargetFromCharacter(cBaseTemp);
            }
        }
        fbStepTime = 12f / 30f;
        BackStep(roarBackDist);
        if (AttackBase(3, 1, 1, 0, 100f / 30f / roarSpeed, 100f / 30f / roarSpeed, 1, roarSpeed)) {
            SuperarmorStart();
            angryTimeRemain = Random.Range(8f, 10f);
            SetMaterial(specialMaterial);
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (roarEnabled) {
            if (state != State.Damage && angryTimeRemain <= 0f && counterChecker && counterChecker.stayFlag) {
                checkerStayTime += Time.deltaTime;
                if (checkerStayTime >= 0.3f) {
                    RoarAngry();
                }
            } else {
                checkerStayTime = 0f;
            }
        }
    }

    void QuakeRoar() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePivot_Mouth.position, 10, 8, 0, 1, 1f, 4f, 25);
        }
    }

    protected void SetMaterial(Material mat) {
        if (mat) {
            for (int i = 0; i < materialObj.Length; i++) {
                if (materialObj[i]) {
                    materialObj[i].GetComponent<Renderer>().material = mat;
                }
            }
        }
    }

    void MoveAttack(float targetDist, float time) {
        if (targetTrans) {
            float sqrDist = GetTargetDistance(true, true, false);
            fbStepTime = time;
            if (sqrDist < targetDist * targetDist) {
                SeparateFromTarget(targetDist);
            } else if (sqrDist > targetDist * targetDist) {
                StepToTarget(targetDist);
            }
        }
    }

    void MoveSeparate() {
        if (targetTrans) {
            fbStepMaxDist = 4f;
            fbStepTime = 0.5f;
            SeparateFromTarget(roarEnabled ? 7f : 4f);
        }
    }

    void RoarEffect() {
        EmitEffect(0);
        throwing.ThrowCancelAll(true);
    }

    void ThrowStartChangeVelocity(int index) {
        if (throwing && index >= 0 && index < throwing.throwSettings.Length) {
            if (roarEnabled && targetTrans) {
                float distance = Vector3.Distance(targetTrans.position, throwing.throwSettings[index].from.transform.position);
                if (distance > throwSpeedUpBorderDistance) {
                    throwing.throwSettings[index].velocity = throwSpeedMin + (distance - throwSpeedUpBorderDistance) * throwSpeedUpRate;
                } else {
                    throwing.throwSettings[index].velocity = throwSpeedMin;
                }
            } else {
                throwing.throwSettings[index].velocity = throwSpeedMin;
            }
            throwing.ThrowStart(index);
        }
    }

    protected override void SetLevelModifier() {
        if (attackDetectionSick) {
            attackDetectionSick.probability = sickProbability[Mathf.Clamp(level, 0, sickProbability.Length - 1)];
        }
        if (level >= 3) {
            actDistNum = 1;
        } else {
            actDistNum = 0;
        }
        moltTimeRemain = 0f;
        attackSave = -1;
        angryTimeRemain = 0;
        if (roarEnabled) {
            knockRecovery = (knockRecovery + defStats.knockRecovery) * 0.5f;
        }
        SetMaterial(defaultMaterial);
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (roarEnabled) {
            if (angryTimeRemain > 0) {
                angryTimeRemain -= deltaTimeCache;
                if (angryTimeRemain <= 0) {
                    SetMaterial(defaultMaterial);
                }
            }
            if (angryTimeRemain > 0 && !isSuperarmor) {
                SuperarmorStart();
            } else if (angryTimeRemain <= 0 && isSuperarmor) {
                SuperarmorEnd();
            }
        }
        if (moltTimeRemain > 0f) {
            moltTimeRemain -= deltaTimeMove;
            if (moltTimeRemain < 0f) {
                moltTimeRemain = 0f;
            }
        }
        if (level >= 3 && moltTimeRemain < 3f) {
            actDistNum = 1;
        } else {
            actDistNum = 0;
        }
        if (level >= 2 && targetTrans && attackedTimeRemain > 0 && targetTrans.position.y > trans.position.y + 1f) {
            attackedTimeRemain = 0f;
        }
        if (angryTimeRemain > 0) {
            attackedTimeRemainOnDamage = 0f;
        } else {
            attackedTimeRemainOnDamage = 1f;
        }
    }

    protected override void KnockLightProcess() {
        if (!isSuperarmor && knockRestoreSpeed != 1f) {
            knockRestoreSpeed = 1f;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        }
        base.KnockLightProcess();
    }

    void SphereImpact() {
        if (state == State.Attack) {
            EmitEffect(effImpact);
            if (CameraManager.Instance && movePivot[0]) {
                if (roarEnabled) {
                    CameraManager.Instance.SetQuake(movePivot[0].position, 12, 4, 0, 0, 1.5f, 4f, dissipationDistance_Large);
                } else {
                    CameraManager.Instance.SetQuake(movePivot[0].position, 8, 4, 0, 0, 1.5f, 2f, dissipationDistance_Normal);
                }
            }
        }
    }

    protected override void KnockHeavyProcess() {
        if (knockRestoreSpeed != 0.5f) {
            knockRestoreSpeed = 0.5f;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        }
        base.KnockHeavyProcess();
    }

    int GetAttackType() {
        int max = 2;
        if (level >= 3) {
            if (targetTrans && GetTargetDistance(true, true, false) > throwConditionDistance * throwConditionDistance) {
                return 3;
            } else {
                max = (level >= 4 ? 4 : 3);
            }
        }
        int randTemp = Random.Range(0, max);
        if (randTemp >= 2) {
            randTemp += 1;
        }
        return randTemp;
    }

    protected override void Attack() {
        bool isAngry = angryTimeRemain > 0;
        float atkSpBias = (isAngry ? 1.2f : 1f);
        moveIndex = -1;
        if (roarEnabled && angryTimeRemain <= 0f && counterChecker && counterChecker.stayFlag) {
            RoarAngry();
        } else {
            int attackTemp = GetAttackType();
            if (attackTemp == attackSave) {
                attackTemp = GetAttackType();
            }
            if (level >= 2 && targetTrans && targetTrans.position.y > trans.position.y + counterAirY && Random.Range(0, 100) < (attackSave != 2 ? 80 : 67)) {
                attackTemp = 2;
            }
            attackSave = attackTemp;
            switch (attackTemp) {
                case 0:
                    AttackBase(0, 1.05f, roarEnabled ? 1.4f : 1.1f, 0, 45f / 30f / atkSpBias, 45f / 30f / atkSpBias + (isAngry ? 0 : GetAttackInterval(1.5f)), 0f, atkSpBias);
                    MoveAttack(roarEnabled ? 4.3f : 2f, 15f / 30f / atkSpBias);
                    break;
                case 1:
                    AttackBase(1, 1f, roarEnabled ? 1.1f : 0.8f, 0, 45f / 30f / atkSpBias, 45f / 30f / atkSpBias + (isAngry ? 0 : GetAttackInterval(1.5f)), 0f, atkSpBias);
                    MoveAttack(roarEnabled ? 5.1f : 2.4f, 15f / 30f / atkSpBias);
                    break;
                case 2:
                    AttackBase(2, 1.05f, roarEnabled ? 1.4f : 1.1f, 0, 70f / 60f / atkSpBias, 70f / 60f / atkSpBias + (isAngry ? 0 : GetAttackInterval(1f)), 0f, atkSpBias);
                    MoveAttack(roarEnabled ? 5.5f : 3f, 18f / 60f / atkSpBias);
                    break;
                case 3:
                    AttackBase(4, 1f, roarEnabled ? 1.4f : 1.1f, 0, 45f / 30f / atkSpBias, 45f / 30f / atkSpBias + (isAngry ? 0 : GetAttackInterval(1f, -2)), 1f, atkSpBias);
                    moltTimeRemain += 8f;
                    MoveSeparate();
                    throwing.ThrowReady(0);
                    EmitEffect(effThrowReady);
                    break;
                case 4:
                    if (roarEnabled) {
                        atkSpBias *= 0.8f;
                    }
                    AttackBase(5, roarEnabled ? 1.25f : 1.12f, roarEnabled ? 2.1f : 1.5f, 0, (roarEnabled && !isAngry && !IsSuperLevel ? 105f : 90f) / 60f / atkSpBias, (roarEnabled && !isAngry && !IsSuperLevel ? 105f : 90f) / 60f / atkSpBias + (isAngry ? 0 : GetAttackInterval(1f, -3)), 0f, atkSpBias);
                    MoveStart(0);
                    break;
            }
        }
    }

    public override float GetMaxSpeed(bool isWalk = false, bool ignoreSuperman = false, bool ignoreSick = false, bool ignoreConfig = false) {
        return base.GetMaxSpeed(isWalk, ignoreSuperman, ignoreSick, ignoreConfig) * (angryTimeRemain > 0 ? 4f / 3f : 1);
    }

    public override float GetKnocked() {
        return base.GetKnocked() * (angryTimeRemain > 0 ? 3 : 1);
    }

    public override float GetDefense(bool ignoreMultiplier = false) {
        return base.GetDefense(ignoreMultiplier) * (moltTimeRemain > 0.01f ? 0.5f : 1f);
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (moveIndex >= 0 && moveIndex < movePivot.Length && movePivot[moveIndex]) {
            ApproachTransformPivot(movePivot[moveIndex], roarEnabled && angryTimeRemain <= 0 ? 8f : 10f, 0.3f, 0.03f, true);
        }
    }

}
