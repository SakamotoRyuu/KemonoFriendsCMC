using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AlligatorClips : EnemyBaseBoss {

    public Transform quakePivot;
    public Transform[] movePivot;
    public bool isRaw;
    public GameObject additionalTrap;
    public Transform[] trapPivot;

    int movingIndex = -1;
    float movingSpeedMul;
    int attackSave = -1;
    float attackSpeed = 1;
    bool attracted;
    bool damageLockon = false;
    bool breakdownFlag;
    const float defaultLightStiffTime = 2.5f;
    const float breakdownLightStiffTime = 1.5f;
    const int effectIndexBreakdown = 8;
    const int attackIndexDying = 7;

    protected override void Awake() {
        base.Awake();
        deadTimer = 3;
        actDistNum = 0;
        if (isRaw) {
            attackedTimeRemainOnDamage = 0.1f;
            attackWaitingLockonRotSpeed = 3f;
        } else {
            attackedTimeRemainOnDamage = 1f;
            attackWaitingLockonRotSpeed = 1.5f;
        }
        killByCriticalFailedKnockAmount = 4000f;
        killByCriticalOnly = true;
        checkGroundPivotHeight = 1f;
        checkGroundTolerance = 2f;
        checkGroundTolerance_Jumping = 2f;
        retargetingConditionTime = 12f;
        retargetingDecayMultiplier = 2f;
        sandstarRawKnockEndurance = 40000;
        sandstarRawKnockEnduranceLight = 5000;
        mustKnockEffectiveEnabled = true;
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled) {
            weakProgress = (nowHP <= GetMaxHP() / 3 ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 0);
        } else {
            weakProgress = (nowHP <= GetMaxHP() / 2 ? 1 : 0);
        }
        if (isRaw) {
            attractionTime = 4f - weakProgress;
        } else {
            attractionTime = 8f - weakProgress;
        }
        attracted = (decoySave == target);
        if (actDistNum != 0) {
            actDistNum = attracted ? 2 : 1;
        }
        if (!sandstarRawEnabled) {
            if (isRaw) {
                maxSpeed = (weakProgress == 2 ? 10 : 9);
            } else {
                maxSpeed = (weakProgress == 2 ? 8 : weakProgress == 1 ? 6 : 5);
            }
        }
        if (state != State.Damage && damageLockon) {
            damageLockon = false;
        }
        if (state == State.Damage && damageLockon) {
            if (isRaw) {
                lockonRotSpeed = 4f;
            } else {
                lockonRotSpeed = 1.5f;
            }
            CommonLockon();
        }
        if (nowHP == 1) {
            lightStiffTime = breakdownLightStiffTime;
        } else {
            lightStiffTime = defaultLightStiffTime;
        }
    }

    void RestoreKnockDown() {
        damageLockon = true;
        if (isRaw) {
            fbStepMaxDist = 10f;
            fbStepTime = 1f;
            fbStepIgnoreY = true;
            SeparateFromTarget(10f);
        }
    }

    public void MoveAttack0() {
        if (state == State.Attack) {
            if (isRaw) {
                movingIndex = 0;
                movingSpeedMul = 1f;
            } else {
                fbStepTime = 40f / 60f / attackSpeed;
                fbStepIgnoreY = true;
                fbStepMaxDist = 12f;
                ApproachOrSeparate(12);
            }
        }
    }

    public void MoveAttack1() {
        if (state == State.Attack) {
            if (isRaw) {
                movingIndex = 1;
                movingSpeedMul = 1f;
            } else {
                fbStepTime = 25f / 60f / attackSpeed;
                fbStepIgnoreY = true;
                fbStepMaxDist = 9f;
                ApproachOrSeparate(9);
            }
        }
    }

    public void MoveAttack4() {
        if (state == State.Attack && target != null) {
            agent.enabled = false;
            SetSpecialMove(GetTargetVector(true, true), GetTargetDistance(false, true, false), 54f / 60f / attackSpeed, EasingType.Linear, ActivateAgent);
        }
    }

    void ActivateAgent() {
        agent.enabled = true;
    }

    public void MoveAttack5_1() {
        if (state == State.Attack) {
            fbStepTime = 30f / 60f / attackSpeed;
            fbStepIgnoreY = true;
            fbStepMaxDist = 6f;
            BackStep(4);
        }
    }

    public void MoveAttack5_2() {
        if (state == State.Attack && target != null) {
            float distance = GetTargetDistance(false, true, false) + 0.5f;
            agent.enabled = false;
            SetSpecialMove(trans.TransformDirection(vecForward), Mathf.Clamp(distance, 4f, 20f), 20f / 60f / attackSpeed, EasingType.SineIn, ActivateAgent);
        }
    }

    void MovingEnd() {
        movingIndex = -1;
    }

    public void QuakeAttack_Short() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePivot.position, 10, 4, 0, 0, 1f, 4f, dissipationDistance_Boss);
        }
    }

    public void QuakeAttack() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePivot.position, 14, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
        }
    }

    public void QuakeHeavyKnock() {
        if (state == State.Damage) {
            CameraManager.Instance.SetQuake(quakePivot.position, 5, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
        }
    }

    protected override void BattleStart() {
        base.BattleStart();
        actDistNum = 1;
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (!battleStarted) {
            BattleStart();
        }
        if (!fixKnockAmount && lastDamagedColorType == damageColor_Critical && attackerCB == CharacterManager.Instance.pCon && enemyCanvasLoaded && enemyCanvasChildObject[(int)EnemyCanvasChild.paperPlane].activeSelf) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_AlligatorClipsKaban, true);
        }
    }

    protected override void KnockLightProcess() {
        if (!isSuperarmor) {
            float restoreTemp = (isRaw ? 1.4f : 1f);
            if (knockRestoreSpeed != restoreTemp) {
                knockRestoreSpeed = restoreTemp;
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
            }
        }
        base.KnockLightProcess();
        if (nowHP == 1 && !isSuperarmor) {
            breakdownFlag = true;
            if (attackedTimeRemain > breakdownLightStiffTime) {
                attackedTimeRemain = breakdownLightStiffTime;
            }
        }
    }

    protected override void KnockHeavyProcess() {
        if (knockRestoreSpeed != 1f) {
            knockRestoreSpeed = 1f;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        }
        base.KnockHeavyProcess();
        if (nowHP == 1) {
            breakdownFlag = true;
            if (attackedTimeRemain > heavyStiffTime) {
                attackedTimeRemain = heavyStiffTime;
            }
        }
    }

    protected override void Start_Process_Dead() {
        base.Start_Process_Dead();
        if (effect[effectIndexBreakdown].instance) {
            Destroy(effect[effectIndexBreakdown].instance);
        }
    }

    void AttackStartSet() {
        for (int i = 0; i < 4; i++) {
            AttackStart(i);
        }
    }

    void AttackEndSet() {
        for (int i = 0; i < 4; i++) {
            AttackEnd(i);
        }
    }

    void AddTrap() {
        if (isRaw && state == State.Attack) {
            ray.direction = vecDown;
            for (int i = 0; i < trapPivot.Length; i++) {
                ray.origin = trapPivot[i].position;
                if (Physics.Raycast(ray, out raycastHit, 4f, fieldLayerMask, QueryTriggerInteraction.Ignore)) {
                    Vector3 groundNormal = raycastHit.normal;
                    if (groundNormal.y > 0f && 75f > Vector3.Angle(groundNormal, vecUp)) {
                        Vector3 pointTemp = raycastHit.point;
                        pointTemp.y += Random.Range(0.0025f, 0.005f);
                        MissingObjectToDestroy missObj = Instantiate(additionalTrap, pointTemp, Quaternion.FromToRotation(vecUp, groundNormal)).GetComponent<MissingObjectToDestroy>();
                        if (missObj) {
                            missObj.SetGameObject(gameObject);
                        }
                    }
                }
            }
        }
    }

    protected override void Attack() {
        resetAgentRadiusOnChangeState = true;
        movingIndex = -1;
        if (breakdownFlag) {
            AttackBase(attackIndexDying, 0, 0, 0, 5.25f, 5.75f, 0, 1, false);
            breakdownFlag = false;
            SuperarmorStart();
            return;
        }
        if (targetTrans) {
            attackSpeed = (weakProgress == 2 ? 1.2f : 1);
            int attackTemp;
            float sqrDistance = GetTargetDistance(true, true, false);
            if (!battleStarted) {
                BattleStart();
                attackTemp = 2;
            } else if (sqrDistance > 40 * 40) {
                attackTemp = 1;
            } else {
                attackTemp = Random.Range(0, 3);
                if (attackTemp == attackSave) {
                    attackTemp = Random.Range(0, 3);
                }
            }
            attackSave = attackTemp;
            float intervalPlus = 0f;
            if (isRaw) {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.3f, 0.5f) : weakProgress == 1 ? Random.Range(0.4f, 0.6f) : Random.Range(0.5f, 0.7f)) + (attracted ? 0.3f : 0); ;
            } else if (sandstarRawEnabled) {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.4f, 0.6f) : weakProgress == 1 ? Random.Range(0.6f, 0.9f) : Random.Range(0.9f, 1.2f)) + (attracted ? 0.5f : 0); ;
            } else {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.5f, 1.1f) : weakProgress == 1 ? Random.Range(1f, 1.6f) : Random.Range(1.5f, 2.1f)) + (attracted ? 0.8f : 0);
            }
            
            switch (attackTemp) {
                case 0:
                    int typeSub;
                    if (sqrDistance > 10 * 10) {
                        typeSub = 0;
                    } else if (sqrDistance < 6 * 6) {
                        typeSub = 1;
                    } else {
                        typeSub = Random.Range(0, 2);
                    }
                    EmitEffect(6);
                    if (typeSub == 0) {
                        AttackBase(weakProgress >= 1 ? 2 : 0, 1, 1.2f, 0, 2f / attackSpeed, 2f / attackSpeed + intervalPlus, 0.25f, attackSpeed);
                    } else {
                        AttackBase(weakProgress >= 1 ? 3 : 1, 1, 1.2f, 0, 1.5f / attackSpeed, 1.5f / attackSpeed + intervalPlus, 0.25f, attackSpeed);
                    }
                    break;
                case 1:
                    AttackBase(4, 1.2f, 3.4f, 0, 4.5f / attackSpeed, 4.5f / attackSpeed + intervalPlus, 0.5f, attackSpeed);
                    agent.radius = 0.2f;
                    break;
                case 2:
                    if (weakProgress >= 2) {
                        AttackBase(6, 1.1f, 2.4f, 0, 5f / attackSpeed, 5f / attackSpeed + intervalPlus, 0.5f, attackSpeed);
                    } else {
                        AttackBase(5, 1.1f, 2.4f, 0, 3f / attackSpeed, 3f / attackSpeed + intervalPlus, 0.5f, attackSpeed);
                    }
                    break;
            }
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (movingIndex >= 0 && movingIndex < movePivot.Length && movePivot[movingIndex] != null) {
            ApproachTransformPivot(movePivot[movingIndex], GetMinmiSpeed() * movingSpeedMul, 0.25f);
        }
    }

    void EmitEffectBreakdown() {
        if (state != State.Dead) {
            EmitEffect(effectIndexBreakdown);
        }
    }

    public void CheckTrophy_QuickEscape() {
        if (state == State.Attack && attackType >= 5) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_AlligatorClipsEscape, true);
        }
    }

    public override float GetKnocked() {
        if (state == State.Attack && attackType == attackIndexDying) {
            return 20000f;
        }
        return base.GetKnocked();
    }
    
}
