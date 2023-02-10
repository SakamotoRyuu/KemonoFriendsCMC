using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Desmodesmus : EnemyBase {

    public Transform[] linePivot;
    public LineRenderer[] lineRenderer;
    public LayerMask startCheckMask;

    int attackSave = -1;
    int lineMax = 1;
    float actRemainTime = 0;
    float lockonSpeedSpecify = 1;
    LayerMask lineLayerMask;
    CharacterBase cBaseHit;
    bool movingFlag;
    bool adjustDirectionReserved;
    static readonly float[] lockonSpeedArray = new float[] { 0.4f, 0.4f, 0.6f, 0.9f, 1.3f };
    static readonly float[] lineMaxDistArray = new float[] { 15f, 15f, 20f, 20f, 20f };
    static readonly float[] attackedTimeRemainOnDamageArray = new float[] { 1f, 1f, 0.8f, 0.5f, 0f };
    static readonly Vector3 pivotOffset = new Vector3(0f, 0.35f, 0f);
    const float lineLengthBias = 1.08f;
    const string targetTag = "PlayerDamageDetection";

    protected override void Awake() {
        base.Awake();
        fbStepTime = 0.35f;
        fbStepMaxDist = 10f;
        superSpeedRate = 4;
        superAccelerationRate = 4;
        superAngularRate = 4;
        roveInterval = -1;
        commonLockonTowardsMultiplier = 4f;
        lineLayerMask = LayerMask.GetMask("Field", "P-DmgDetect", "EnemyCollision", "InvisibleWall", "SecondField");
    }

    protected override void Start() {
        if (!targetHateInitialized) {
            adjustDirectionReserved = true;
        }
        base.Start();
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (actDistNum == 0 && isSuperman) {
            SetOffensive();
        }
        if (actDistNum != 0 && !isSuperman) {
            actRemainTime -= deltaTimeCache;
            if (actRemainTime <= 0) {
                actDistNum = 0;
            }
        }
    }

    protected override void SetLevelModifier() {
        lockonSpeedSpecify = lockonSpeedArray[Mathf.Clamp(level, 0, lockonSpeedArray.Length - 1)];
        attackWaitingLockonRotSpeed = lockonSpeedArray[Mathf.Clamp(level, 0, lockonSpeedArray.Length - 1)] * 0.4f;
        if (level >= 3) {
            lineMax = 2;
        } else {
            lineMax = 1;
        }
        attackedTimeRemainOnDamage = attackedTimeRemainOnDamageArray[Mathf.Clamp(level, 0, attackedTimeRemainOnDamageArray.Length - 1)];
    }

    private void LateUpdate() {
        for (int i = 0; i < linePivot.Length; i++) {
            if (GetCanControl() && attackedTimeRemain <= 0 && i < lineMax) {
                if (!lineRenderer[i].enabled) {
                    lineRenderer[i].enabled = true;
                }
                ray.origin = linePivot[i].position;
                ray.direction = linePivot[i].TransformDirection(vecForward);
                if (Physics.Raycast(ray, out raycastHit, lineMaxDistArray[level], lineLayerMask, QueryTriggerInteraction.Ignore)) {
                    lineRenderer[i].SetPosition(1, vecForward * (raycastHit.distance * lineLengthBias));
                    if (raycastHit.collider.CompareTag(targetTag)) {
                        cBaseHit = GetComponentInParent<CharacterBase>();
                        if (cBaseHit && cBaseHit.searchTarget.Length > 0) {
                            CounterAttack(cBaseHit.searchTarget[0]);
                        }
                    }
                } else {
                    lineRenderer[i].SetPosition(1, vecForward * (lineMaxDistArray[level] * lineLengthBias));
                }
            } else {
                if (lineRenderer[i].enabled) {
                    lineRenderer[i].enabled = false;
                }
            }
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (level >= 4) {
            SetOffensive();
        }
        if (adjustDirectionReserved) {
            adjustDirectionReserved = false;
            LookNoWallDirection(lineMaxDistArray[Mathf.Clamp(level, 0, lineMaxDistArray.Length - 1)], 0.35f, startCheckMask);
        }
    }

    void SetOffensive() {
        actDistNum = 1;
        actRemainTime = 15f;
    }

    void MoveAttack0() {
        fbStepMaxDist = lineMaxDistArray[Mathf.Clamp(level, 0, lineMaxDistArray.Length - 1)];
        fbStepTime = Mathf.Clamp(GetTargetDistance(false, true, false) / fbStepMaxDist * 0.5f, 0.175f, 0.35f);
        StepToTarget(0.4f);
    }

    void MoveEscape() {
        lockonRotSpeed = 10f;
        fbStepMaxDist = 10f;
        fbStepTime = Mathf.Clamp((10f - GetTargetDistance(false, true, false)) / 10f * 0.5f, 0.175f, 0.35f);
        SeparateFromTarget(10f);
    }

    void MoveStart() {
        movingFlag = true;
        lockonRotSpeed = 3f;
        LockonStart();
    }

    void MoveEnd() {
        movingFlag = false;
        LockonEnd();
    }

    void AttackStartSet(int multiFlag) {
        float multiHitInterval = (multiFlag != 0 ? 0.4f : 0f);
        for (int i = 0; i < attackDetection.Length; i++) {
            if (attackDetection[i]) {
                attackDetection[i].multiHitInterval = multiHitInterval;
            }
        }
        AttackStart(0);
        AttackStart(1);
    }

    void AttackEndSet() {
        AttackEnd(0);
        AttackEnd(1);
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        SetOffensive();
    }

    public override void Attraction(GameObject decoy, AttractionType type, bool lockForce = false, float targetFixingTime = 0f) {
        base.Attraction(decoy, type, lockForce, targetFixingTime);
        SetOffensive();
    }

    int GetAttackRandom() {
        int answer = 0;
        if (level >= 4) {
            answer = Random.Range(0, 4);
            if (answer == 0 && targetTrans && GetTargetDistance(true, true, false) <= 4f) {
                answer = 3;
            }
        } else if (level >= 3) {
            answer = Random.Range(0, 3);
        } else if (level == 2) {
            answer = Random.Range(0, 2);
        } else {
            answer = 0;
        }
        return answer;
    }

    protected override void Attack() {
        base.Attack();
        for (int i = 0; i < attackDetection.Length; i++) {
            if (attackDetection[i]) {
                attackDetection[i].multiHitInterval = 0f;
            }
        }
        SetOffensive();
        movingFlag = false;
        int attackTemp = GetAttackRandom();
        if (attackTemp == attackSave) {
            attackTemp = GetAttackRandom();
        }
        attackSave = attackTemp;
        switch (attackTemp) {
            case 0:
                AttackBase(0, 1, 1.2f, 0, 50f / 30f, 50f / 30f + GetAttackInterval(1f), 0, 1, true, lockonSpeedSpecify);
                break;
            case 1:
                AttackBase(1, 1, 1.2f, 0, 60f / 30f, 60f / 30f + GetAttackInterval(1f, -1), 0, 1, true, lockonSpeedSpecify);
                break;
            case 2:
                AttackBase(2, 1, 1.2f, 0, 60f / 30f, 60f / 30f + GetAttackInterval(1f, -2), 0, 1, true, lockonSpeedSpecify);
                EmitEffect(3);
                break;
            case 3:
                AttackBase(3, 1, 1.2f, 0, 75f / 30f, 75f / 30f + GetAttackInterval(1f, -3), 0, 1, true, lockonSpeedSpecify);
                break;
        }
        lockonSpeedSpecify = lockonSpeedArray[level];
    }

    void ResetLockonRotSpeed() {
        lockonSpeedSpecify = lockonSpeedArray[level];
        lockonRotSpeed = lockonSpeedSpecify;
    }

    void ChangeLockon() {
        lockonRotSpeed = 10f;
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (movingFlag && targetTrans) {
            cCon.Move(21f * deltaTimeMove * trans.TransformDirection(vecForward));
        }
    }

    public override void CounterAttack(GameObject attackerObject = null, bool isProjectile = false) {
        base.CounterAttack(attackerObject);
        if (state != State.Attack) {
            searchArea[0].SetLockTarget(attackerObject, 1);
            Update_Targeting();
            lockonSpeedSpecify = 10;
            SetState(State.Attack);
        }
    }

}
