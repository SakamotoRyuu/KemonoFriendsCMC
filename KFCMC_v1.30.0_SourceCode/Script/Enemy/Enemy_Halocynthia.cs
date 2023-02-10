using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Halocynthia : EnemyBase {
    
    public GameObject[] corePoints;
    public ParticleSystem[] coreParticles;
    public CheckTriggerStay fieldChecker;
    public DamageDetection[] criticalDD;
    public Transform[] bombPivot;
    public GameObject[] bombInstance;
    public GameObject bombPrefab;
    public float[] bombNotExistingTime;

    static readonly float[] damageRateArray = new float[5] { 4.0f, 4.0f, 3.833333f, 3.666666f, 3.5f };
    static readonly float[] jumpSpeedArray = new float[5] { 11f, 11f, 11f * 1.1f, 11f * 1.21f, 11f * 1.331f };

    const float gravityMultiJumping = 1f;
    const float gravityMultiFalling = 3f;
    const float moveYLowerLimit = -25f;
    const int effectIndexJumpStart = 2;
    const int effectIndexJumpMiddle = 3;
    const int effectIndexJumpEnd = 4;
    const int attackIndexJumpStart = 1;
    const int attackIndexJumpEnd = 4;

    float jumpAttackPosSave;
    float jumpSpeed = 11f;
    int attackSave = -1;
    int nowPoint = -1;
    float damagedTimeRemain;
    float setBombTimer;

    protected override void Awake() {
        base.Awake();
        resetAgentRadiusOnChangeState = true;
        attackedTimeRemainOnDamage = 0.5f;
        attackWaitingLockonRotSpeed = 0.5f;
    }
    
    void QuakeAttack() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(trans.position, 5, 4, 0, 0, 1.5f, 2f, dissipationDistance_Normal);
        }
    }

    void SetCorePoint() {
        if (corePoints.Length > 0) {
            bool particlePlayFlag = false;
            if (nowPoint < 0) {
                nowPoint = Random.Range(0, corePoints.Length);
            } else {
                nowPoint = (nowPoint + Random.Range(1, corePoints.Length)) % corePoints.Length;
                particlePlayFlag = true;
            }
            for (int i = 0; i < corePoints.Length; i++) {
                if (corePoints[i]) {
                    corePoints[i].SetActive(i == nowPoint);
                }
            }
            if (particlePlayFlag && coreParticles.Length > nowPoint&& coreParticles[nowPoint]) {
                coreParticles[nowPoint].Play();
            }
            if (level >= 3) {
                setBombTimer = 1f;
            }
        }
    }

    protected override void SetLevelModifier() {
        nowPoint = -1;
        for (int i = 0; i < bombNotExistingTime.Length; i++) {
            bombNotExistingTime[i] = 0f;
        }
        SetCorePoint();
        for (int i = 0; i < criticalDD.Length; i++) {
            if (criticalDD[i]) {
                criticalDD[i].damageRate = damageRateArray[Mathf.Clamp(level, 0, damageRateArray.Length - 1)];
            }
        }
        if (level <= 2) {
            DestroyBomb();
            setBombTimer = 0f;
        }
        jumpSpeed = jumpSpeedArray[Mathf.Clamp(level, 0, jumpSpeedArray.Length - 1)];
        attackSave = -1;
    }

    void DestroyBomb() {
        for (int i = 0; i < bombInstance.Length; i++) {
            if (bombInstance[i] != null) {
                Destroy(bombInstance[i]);
            }
        }
    }

    void SetBomb(int exceptIndex) {
        for (int i = 0; i < bombPivot.Length; i++) {
            if (i == exceptIndex){
                if (bombInstance[i] != null) {
                    Destroy(bombInstance[i]);
                }
            } else if (bombNotExistingTime[i] >= 0f && bombInstance[i] == null) {
                bombInstance[i] = Instantiate(bombPrefab, bombPivot[i]);
                bombNotExistingTime[i] = Random.Range(-6f, -10f);
            }
        }
    }

    protected override void Update_Transition_Jump() {
        base.Update_Transition_Jump();
        if (stateTime > 0.6f) {
            if (move.y < -5f || (targetTrans == null && move.y < 0f)) {
                SetState(State.Attack);
            } else if (targetTrans) {
                float sqrDist = GetTargetDistance(true, true, false);
                if (state != State.Attack && sqrDist <= 0.5f * 0.5f) {
                    SetState(State.Attack);
                }
            }
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        DestroyBomb();
    }

    void SetCorePointWithCondition() {
        if (level >= 2) {
            SetCorePoint();
        }
    }

    void JumpAttack() {
        gravityMultiplier = gravityMultiJumping;
        float plus = 0f;
        if (targetTrans) {
            plus = Mathf.Clamp((targetTrans.position.y - trans.position.y - 2f) * 0.5f, 0f, 4f);
        }
        Jump(14f + plus);
        EmitEffect(effectIndexJumpStart);
        cCon.radius = 0.1f;
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (targetTrans && damagedTimeRemain > 0f) {
            actDistNum = 2;
        } else if (targetTrans && Mathf.Abs(targetTrans.position.y - trans.position.y) > 4f) {
            actDistNum = 1;
        } else {
            actDistNum = 0;
        }
        for (int i = 0; i < bombInstance.Length && i < bombNotExistingTime.Length; i++) {
            if (bombNotExistingTime[i] < 9f && bombInstance[i] == null) {
                bombNotExistingTime[i] += deltaTimeCache;
                if (level >= 3 && i != nowPoint && bombNotExistingTime[i] >= 9f) {
                    bombInstance[i] = Instantiate(bombPrefab, bombPivot[i]);
                    bombNotExistingTime[i] = Random.Range(-6f, -10f);
                }
            }
        }
        if (setBombTimer > 0f) {
            setBombTimer -= deltaTimeMove;
            if (level >= 3 && setBombTimer <= 0f) {
                SetBomb(nowPoint);
            }
        }
        if (move.y < moveYLowerLimit) {
            move.y = moveYLowerLimit;
        }
    }

    protected override void Update_Process_Jump() {
        base.Update_Process_Jump();
        if (targetTrans) {
            lockonRotSpeed = 6;
            float sqrDist = GetTargetDistance(true, true, false);
            if (sqrDist > 0.1f * 0.1f) {
                if (sqrDist > 0.4f * 0.4f) {
                    CommonLockon();
                }
                if (fieldChecker && !fieldChecker.stayFlag) {
                    float speedBias = 1f;
                    if (sqrDist < 0.5f * 0.5f) {
                        float dist = GetTargetDistance(false, true, false);
                        speedBias = dist * 2f;
                    }
                    cCon.Move(GetTargetVector(true, true) * jumpSpeed * speedBias * deltaTimeMove);
                }
            }
        }
        jumpAttackPosSave = trans.position.y;
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (damagedTimeRemain > 0f) {
            damagedTimeRemain -= deltaTimeCache;
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        damagedTimeRemain = 4f;
    }

    int GetAttackType() {
        int answer = 3;
        if (targetTrans) {
            float height = targetTrans.position.y - trans.position.y;
            if (trans.position.y < -2f && height > 4f) {
                answer = 2;
            } else if (height > 5f && Random.Range(0, 100) < 75) {
                answer = 2;
            } else {
                float sqrDist = GetTargetDistance(true, true, false);
                if (sqrDist < (level >= 4 ? 6f * 6f : 3f * 3f)) {
                    answer = Random.Range(1, 4);
                } else if (sqrDist < 15f * 15f){
                    answer = Random.Range(2, 4);
                } else {
                    answer = 2;
                }
            }
        }
        return answer;
    }

    protected override void Attack() {
        base.Attack();
        if (groundedFlag) {
            int attackTemp = GetAttackType();
            if (attackTemp == attackSave) {
                attackTemp = GetAttackType();
            }
            attackSave = attackTemp;
            float sqrDist = GetTargetDistance(true, true, false);
            switch (attackTemp) {
                case 0:
                case 1:
                    if (level >= 4 && (sqrDist >= 3f * 3f || Random.Range(0, 100) < 50)) {
                        AttackBase(5, 1.2f, 2.1f, 0, 95f / 60f / 0.8f, 95f / 60 / 0.8f + GetAttackInterval(0.8f, -3), 1f, 0.8f);
                    } else {
                        AttackBase(0, 1f, 1.3f, 0, 95f / 60f, 95f / 60 + GetAttackInterval(0.8f));
                    }
                    break;
                case 2:
                    AttackBase(attackIndexJumpStart, 0, 0, 0, 45f / 60f, 45f / 60f, 0, 1f, false);
                    jumpAttackPosSave = trans.position.y;
                    break;
                case 3:
                    if (sqrDist >= 4f * 4f || Random.Range(0, 100) < 50) {
                        AttackBase(2, 0.1f, 0.6f, 0, 90f / 60f, 90f / 60f + GetAttackInterval(1.5f));
                    } else {
                        AttackBase(3, 0.1f, 0.6f, 0, 70f / 60f, 70f / 60f + GetAttackInterval(1.5f));
                    }
                    break;
            }
        } else {
            AttackBase(attackIndexJumpEnd, 1.1f, 1.7f, 0, 5f, 90f / 60f + GetAttackInterval(1.5f), 1, 1, false);
            if (move.y > -1f) {
                move.y = -1f;
            }
            jumpAttackPosSave = trans.position.y;
            cCon.radius = 0.1f;
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        switch (attackType) {
            case attackIndexJumpEnd:
                gravityMultiplier = gravityMultiFalling;
                if (groundedFlag || stateTime > 3f || (stateTime > 0.6f && trans.position.y >= jumpAttackPosSave)) {
                    attackType = -1;
                    isAnimStopped = false;
                    if (groundedFlag) {
                        EmitEffect(effectIndexJumpEnd);
                        QuakeAttack();
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
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], attackType != attackIndexJumpEnd || !isAnimStopped ? 1 : 0);
                if (Time.timeScale > 0f) {
                    jumpAttackPosSave = trans.position.y;
                }
                break;
        }
    }

}
