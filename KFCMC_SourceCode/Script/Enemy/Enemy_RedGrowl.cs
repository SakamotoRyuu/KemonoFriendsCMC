using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_RedGrowl : EnemyBaseBoss {

    public Transform quakePivot_Bottom;
    public Transform quakePivot_Mouth;
    public Transform quakePivot_Knock;
    public Transform targetChaser;
    public SupermanSettings[] emissionMatSettings;
    public GameObject defaultDD;
    public GameObject criticalDD;
    public bool isRaw;
    public Transform boltOrigin;
    public Transform boltThrowFrom;

    int attackSave = -1;
    bool targetChaserEnabled = false;
    float cruelTimeRemain = 0;
    float jumpAttackPosSave;
    float judgementTimeElapsed = 20f;
    bool judgementReadyFlag;
    bool isPressAttack;

    const float gravityMultiJumping = 1f;
    const float gravityMultiFalling = 4f;
    const float moveYLowerLimit = -25f;
    const int judgementAttackIndex = 4;
    const int judgementThrowIndex = 4;
    const int volcanoThrowIndex = 5;

    protected override void Awake() {
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
        sandstarRawKnockEndurance = 5500;
        sandstarRawKnockEnduranceLight = 5500f / 3f;
        fireDamageRate = 0;
        if (isRaw) {
            spawnStiffTime = 1f;
            attackedTimeRemainOnDamage = 0.1f;
        } else {
            spawnStiffTime = 0f;
            attackedTimeRemainOnDamage = 1f;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (!battleStarted && target) {
            BattleStart();
        }
        if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled) {
            weakProgress = (nowHP <= GetMaxHP() / 3 ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 0);
        } else {
            weakProgress = (nowHP <= GetMaxHP() / 2 ? 1 : 0);
        }
        cruelTimeRemain -= deltaTimeCache;
        if (emissionMatSettings[1].isSpecial && cruelTimeRemain <= 0) {
            CruelActivate(0);
        }
        if (state != State.Attack) {
            EmissionDeactivate(0);
        }
        if (targetChaserEnabled && state != State.Attack) {
            TargetChaserActivate(0);
        }
        if (targetChaserEnabled && targetChaser && targetTrans) {
            ray.origin = targetTrans.position;
            ray.direction = vecDown;
            if (Physics.Raycast(ray, out raycastHit, 20f, fieldLayerMask, QueryTriggerInteraction.Ignore)) {
                targetChaser.position = raycastHit.point;
            }
        }
        if (state != State.Damage && criticalDD.activeSelf) {
            criticalDD.SetActive(false);
            defaultDD.SetActive(true);
        }
        if (judgementReadyFlag && state != State.Attack && state != State.Jump) {
            judgementReadyFlag = false;
            AttackEnd(judgementAttackIndex);
        }
        if (isPressAttack && state != State.Attack && state != State.Jump) {
            isPressAttack = false;
        }
        if (move.y < moveYLowerLimit) {
            move.y = moveYLowerLimit;
        }
    }

    void CruelActivate(int flag = 1) {
        if (flag != 0) {
            cruelTimeRemain = (weakProgress >= 2 ? 14f : 11f) + (isRaw ? 3f : 0f);
            EmissionActivate(1);
        } else {
            cruelTimeRemain = 0;
            EmissionDeactivate(1);
        }
    }

    void SetEmissionMat(int index, bool flag) {
        if (emissionMatSettings.Length > index) {
            for (int i = 0; i < emissionMatSettings[index].mats.Length; i++) {
                SetForChangeMatSet(emissionMatSettings[index].mats[i], flag);
            }
            emissionMatSettings[index].isSpecial = flag;
        }
    }

    void EmissionActivate(int index) {
        if (!emissionMatSettings[index].isSpecial) {
            SetEmissionMat(index, true);
        }
    }

    void EmissionDeactivate(int index) {
        if (emissionMatSettings[index].isSpecial) {
            SetEmissionMat(index, false);
        }
    }

    void TargetChaserActivate(int flag = 1) {
        targetChaserEnabled = (flag != 0);
    }

    void ThrowStartToReady(int index) {
        throwing.ThrowStart(index);
        throwing.ThrowReady(index);
    }

    public void QuakeAttack(int index) {
        if (state == State.Attack) {
            switch (index) {
                case 2:
                    CameraManager.Instance.SetQuake(quakePivot_Bottom.position, 14, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
                    break;
                case 3:
                    CameraManager.Instance.SetQuake(quakePivot_Mouth.position, 5, 8, 0, 0, 1f, 4f, dissipationDistance_Boss);
                    break;
                case 5:
                    CameraManager.Instance.SetQuake(quakePivot_Mouth.position, 5, 8, 0, 70f / 60f, 1f, 4f, dissipationDistance_Boss);
                    break;
            }
        }
    }

    public void QuakeKnock() {
        if (state == State.Damage) {
            CameraManager.Instance.SetQuake(quakePivot_Knock.position, 5, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
        }
    }

    public void MoveAttack(int index) {
        if (state == State.Attack) {
            switch (index) {
                case 0:
                    fbStepTime = 1f;
                    ForwardOrBackStep(Random.Range(1f, 1.6f));
                    break;
                case 1:
                    fbStepTime = 20f / 60f;
                    ForwardOrBackStep(2.5f);
                    break;
                case 3:
                    fbStepTime = 1f;
                    BackStep(10);
                    break;
                case 4:
                    fbStepTime = 30f / 60f;
                    BackStep(7);
                    break;
            }
        }
    }

    public void JumpAttack() {
        if (state == State.Attack) {
            gravityMultiplier = gravityMultiJumping;
            float powerPlus = 0f;
            if (targetTrans && targetTrans.position.y > trans.position.y + 8f) {
                powerPlus = Mathf.Clamp((targetTrans.position.y - (trans.position.y + 8f)), 0f, 10f);
            }
            Jump(14f + powerPlus);
            if (isRaw) {
                JudgementReady();
            }
        }
    }

    public override void EmitEffectString(string type) {
        base.EmitEffectString(type);
        switch (type) {
            case "Dead":
                EmitEffect(0);
                break;
            case "Jump":
                EmitEffect(1);
                break;
            case "JumpEnd":
                EmitEffect(2);
                break;
            case "Press":
                EmitEffect(3);
                break;
            case "FireBallReady":
                EmitEffect(4);
                break;
            case "FireBallStart":
                EmitEffect(5);
                break;
            case "Roar1":
                EmitEffect(6);
                break;
            case "Roar2":
                EmitEffect(7);
                break;
            case "Roar3":
                EmitEffect(Random.Range(8, 13));
                break;
        }
    }

    protected override void KnockLightProcess() {        
        if (state != State.Damage) {
            if (!isSuperarmor && knockRestoreSpeed != 1f) {
                knockRestoreSpeed = 1f;
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
            }
            base.KnockLightProcess();
            if (!isSuperarmor && move.y > 0f) {
                move.y = 0f;
            }
        }
    }

    protected override void KnockHeavyProcess() {
        if (state != State.Damage || !isDamageHeavy) {
            float knockRestoreTemp = CharacterManager.Instance.riskyIncSqrt;
            if (knockRestoreSpeed != knockRestoreTemp) {
                knockRestoreSpeed = knockRestoreTemp;
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
            }
            if (!fixKnockAmount && lastDamagedColorType == damageColor_Critical && attackerCB == CharacterManager.Instance.pCon && throwing.GetIsReady(0)) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_RedGrowlCounter, true);
            }
            base.KnockHeavyProcess();
            if (move.y > 0f) {
                move.y = 0f;
            }
            agent.radius = 0.1f;
            agent.height = 0.1f;
        }
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (judgementTimeElapsed < 20f) {
            judgementTimeElapsed += deltaTimeCache * CharacterManager.Instance.riskyIncrease;
        }
    }

    void JudgementReady() {
        if (state == State.Attack || state == State.Jump) {
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

    protected override void Attack() {
        resetAgentRadiusOnChangeState = true;
        resetAgentHeightOnChangeState = true;
        if (targetTrans) {
            float animSpeed = 1;
            float intervalPlus = 0f;
            if (isRaw) {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.3f, 0.5f) : weakProgress == 1 ? Random.Range(0.4f, 0.6f) : Random.Range(0.5f, 0.7f));
            } else if (sandstarRawEnabled) {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.4f, 0.6f) : weakProgress == 1 ? Random.Range(0.6f, 0.9f) : Random.Range(0.9f, 1.2f));
            } else {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.5f, 0.9f) : weakProgress == 1 ? Random.Range(1f, 1.5f) : Random.Range(1.5f, 2.0f));
            }
            int attackTemp = 0;
            if (cruelTimeRemain > 0) {
                intervalPlus = 0;
                animSpeed = 1.2f;
            }
            if (groundedFlag) {
                float sqrDistance = GetTargetDistance(true, true, false);
                if (attackSave < 0) {
                    attackTemp = 2;
                } else {
                    int min = sqrDistance > 6 * 6 ? 2 : 0;
                    int max = 5;
                    if (weakProgress == 1 && cruelTimeRemain < -10) {
                        max = 6;
                    } else if (weakProgress == 2 && cruelTimeRemain < -5) {
                        max = 6;
                    }
                    attackTemp = Random.Range(min, max);
                    if (attackTemp == attackSave) {
                        attackTemp = Random.Range(min, max);
                    }
                }
                if (!agent.isOnNavMesh || agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) {
                    attackTemp = 2;
                }
                attackSave = attackTemp;
                switch (attackTemp) {
                    case 0:
                        AttackBase(0, 1, 1.2f, 0, 90f / 60f / animSpeed, 90f / 60f / animSpeed + intervalPlus, 1, animSpeed);
                        break;
                    case 1:
                        bool isRight = (Vector3.Cross(trans.TransformDirection(vecForward), targetTrans.position - trans.position).y > 0);
                        AttackBase(isRight ? 1 : 6, 1.1f, 1.2f, 0, 60f / 60f / animSpeed, 60f / 60f / animSpeed + intervalPlus, 1, animSpeed);
                        break;
                    case 2:
                        AttackBase(2, 0, 0, 0, 45f / 60f / animSpeed, 45f / 60f / animSpeed, 0, animSpeed, false);
                        jumpAttackPosSave = trans.position.y;
                        isPressAttack = true;
                        break;
                    case 3:
                        throwing.target = targetTrans;
                        if (cruelTimeRemain > 0) {
                            AttackBase(8, 1.05f, 1.4f, 0, 200f / 60f / animSpeed, 200f / 60f / animSpeed + intervalPlus, 0, animSpeed);
                        } else {
                            AttackBase(3, 1.05f, 1.4f, 0, 120f / 60f / animSpeed, 120f / 60f / animSpeed + intervalPlus, 0, animSpeed);
                        }
                        break;
                    case 4:
                        AttackBase(4, 1, 0.8f, 0, 240f / 60f / animSpeed, 270f / 60f / animSpeed + intervalPlus, 0, animSpeed, false);
                        break;
                    case 5:
                        AttackBase(5, 1, 1, 0, 130f / 60f, 130f / 60f);
                        break;
                }
            } else {
                if (isPressAttack) {
                    isPressAttack = false;
                    AttackBase(7, 1.2f, 3.4f, 0, 5f, 90f / 60f + intervalPlus, 1, 1, false);
                    if (move.y > -8f) {
                        move.y = -8f;
                    }
                    jumpAttackPosSave = trans.position.y;
                }
            }
        }
    }

    protected override void Update_Transition_Jump() {
        base.Update_Transition_Jump();
        if (isPressAttack) {
            if (stateTime > 0.6f) {
                if (move.y < -5f || (targetTrans == null && move.y < 0f)) {
                    SetState(State.Attack);
                } else if (targetTrans) {
                    float sqrDist = GetTargetDistance(true, true, false);
                    if (state != State.Attack && sqrDist < 1f * 1f && attackedTimeRemain < 0) {
                        SetState(State.Attack);
                    }
                }
            }
            jumpAttackPosSave = trans.position.y;
        }
    }

    protected override void Update_Process_Jump() {
        base.Update_Process_Jump();
        if (targetTrans && isPressAttack && stateTime >= 0.1f) {
            lockonRotSpeed = attackLockonDefaultSpeed * 0.5f;
            float sqrDist = GetTargetDistance(true, true, false);
            if (sqrDist > 0.1f * 0.1f) {
                if (sqrDist > 0.5f * 0.5f) {
                    CommonLockon();
                }
                float speedBias = 1f;
                if (sqrDist < 0.5f * 0.5f) {
                    float dist = GetTargetDistance(false, true, false);
                    speedBias = dist * 2f;
                }
                cCon.Move(GetTargetVector(true, true) * Mathf.Clamp(GetMaxSpeed() * 3f, 0f, 30f) * speedBias * deltaTimeMove);
            }
        }
    }
    
    protected override void AttackContinuous() {
        base.AttackContinuous();
        switch (attackType) {
            case 7:
                gravityMultiplier = gravityMultiFalling;
                if (groundedFlag || stateTime > 3f || (stateTime > 0.6f && trans.position.y >= jumpAttackPosSave)) {
                    attackType = -1;
                    isAnimStopped = false;
                    if (groundedFlag) {
                        EmitEffectString("Press");
                        QuakeAttack(2);
                        if (isRaw) {
                            JudgementStart();
                        }
                    }
                    attackStiffTime = stateTime + 0.6f;
                    nowSpeed = 0;
                    gravityMultiplier = 1;
                } else {
                    attackStiffTime = stateTime + 1.0f;
                    if (attackedTimeRemain < 1.5f) {
                        attackedTimeRemain = 1.5f;
                    }
                }
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], attackType != 7 || !isAnimStopped ? 1 : 0);
                if (Time.timeScale > 0f) {
                    jumpAttackPosSave = trans.position.y;
                }
                break;
        }
    }

}
