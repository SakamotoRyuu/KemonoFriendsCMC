using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy_Snowman : EnemyBaseBoss {

    public Transform rootTrans;
    public Transform[] rootPlus;
    public Transform quakePivot_Bottom;
    public Transform quakePivot_Nose;
    public Transform quakePivot_Sword;
    public Transform quakePivot_Bomb;
    public Transform quakePivot_Knock;
    public GameObject[] navMeshPrefab;
    public Transform lineCastFrom;
    public CheckTriggerStay fieldChecker;
    public GameObject noseIceParent;
    public FixScale coreFixScale;
    public Projector bottomProjector;
    public bool isRaw;

    int attackSave = -1;
    int scaleProgress = 0;
    float jumpAttackPosSave;
    float rootScale = 1f;
    float jumpSpeed = 20f;
    float knockStopStateTime = 0f;
    float throwRushTimeRemain = 0f;
    float throwRushIntervalRemain = 0f;
    float defaultFaceHeight;
    static readonly float[] scaleArray = new float[] { 1f, 1.4f, 1.96f };
    static readonly float[] jumpSpeedArray = new float[] { 20f, 25f, 30f };
    bool lineCastHit = false;
    int swordType = 0;
    float scalingTimeRemain;
    bool isPressAttack;
    float attackSpeed = 1f;

    GameObject navMeshInstance;

    const float gravityMultiJumping = 1f;
    const float gravityMultiFalling = 4f;
    const float moveYLowerLimit = -25f;

    // 0 - NoseAttack
    // 1 - SpinAttack
    // 2 - IceSword
    // 3 - JumpAttack
    // 4 - IceLance
    // 5 - IceBomb
    // 6 - Superman

    protected override void Awake() {
        base.Awake();
        coreFixScale.ForceAwake();
        deadTimer = 3;
        attackWaitingLockonRotSpeed = 1.5f;
        defaultFaceHeight = faceHeight;
        checkGroundPivotHeight = 1f;
        checkGroundTolerance = 1.6f;
        checkGroundTolerance_Jumping = 1.45f;
        sandstarRawKnockEndurance = 5500;
        sandstarRawKnockEnduranceLight = 5500f / 3f;
        sandstarRawMaxSpeed = 0f;
        sandstarRawAcceleration = 0f;
        killByCriticalOnly = true;
        coreShowHP = GetMaxHP();
        // coreHideDenomi = 7.5f;
        coreHideDenomi = 5.5f;
        fireDamageRate = 2;
        if (isRaw) {
            attackedTimeRemainOnDamage = 0.1f;
            coreTimeMax = 6.5f;
            spawnStiffTime = 1f;
        } else {
            attackedTimeRemainOnDamage = 0.5f;
            coreTimeMax = 8f;
            spawnStiffTime = 0f;
        }
    }

    void NavMeshDestroy() {
        if (navMeshInstance) {
            Destroy(navMeshInstance);
            navMeshInstance = null;
        }
    }

    void NavMeshActivate() {
        NavMeshDestroy();
        if (navMeshPrefab.Length > scaleProgress && navMeshPrefab[scaleProgress]) {
            navMeshInstance = Instantiate(navMeshPrefab[scaleProgress], trans.position, trans.rotation, trans);
            specialMoveDuration = 0f;
        }
    }

    void LookTargetForThrow() {
        if (targetTrans) {
            throwing.throwSettings[1].from.transform.LookAt(targetTrans);
        }
    }
    
    protected override void OnDestroy() {
        base.OnDestroy();
        if (rootTrans) {
            rootTrans.DOKill();
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (actDistNum == 0 && target != null) {
            BattleStart();
        }
        if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled) {
            weakProgress = (nowHP <= GetMaxHP() / 3 ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 0);
        } else {
            weakProgress = (nowHP <= GetMaxHP() / 2 ? 1 : 0);
        }
        attackSpeed = 1f;
        if (scaleProgress < weakProgress && GetCanControl() && state != State.Jump && groundedFlag) {
            scaleProgress = weakProgress;
            rootScale = scaleArray[scaleProgress];
            jumpSpeed = jumpSpeedArray[scaleProgress];
            for (int i = 0; i < rootPlus.Length; i++) {
                if (rootPlus[i]) {
                    rootPlus[i].localScale = new Vector3(rootScale, rootScale, rootScale);
                }
            }
            if (rootTrans) {
                rootTrans.DOScale(rootScale, 1.5f).SetEase(Ease.InOutSine);
            }
            if (bottomProjector) {
                bottomProjector.orthographicSize = 3f * rootScale;
            }
            disableControlTimeRemain = 2.2f;
            scalingTimeRemain = 2.2f;
            attackedTimeRemain = 2.2f;
            EmitEffectString("Scale");
            QuakeScale();
            faceHeight = defaultFaceHeight * Mathf.Sqrt(rootScale);
            enemyCanvas.anchoredPosition3D = canvasPos * rootScale;
            if (isRaw) {
                quickAttackRadius = 5f * rootScale;
            }
        }
        if (scalingTimeRemain > 0f) {
            scalingTimeRemain -= deltaTimeCache;
            if (attackedTimeRemain < scalingTimeRemain) {
                attackedTimeRemain = scalingTimeRemain;
            }
        }

        if (throwRushTimeRemain > 0f) {
            if (state == State.Attack) {
                throwRushTimeRemain -= deltaTimeCache;
                throwRushIntervalRemain -= deltaTimeCache;
                if (targetTrans) {
                    throwing.throwSettings[1].from.transform.rotation = Quaternion.Slerp(throwing.throwSettings[1].from.transform.rotation, Quaternion.LookRotation(targetTrans.position - throwing.throwSettings[1].from.transform.position), 10f * deltaTimeCache);
                }
                if (throwRushIntervalRemain <= 0f) {
                    throwRushIntervalRemain += 0.15f;
                    int throwNumMax = (weakProgress >= 2 ? 3 : 1);
                    for (int i = 1; i <= throwNumMax; i++) {
                        throwing.ThrowStart(i);
                    }
                }
            } else {
                throwRushTimeRemain = 0f;
            }
        }

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
        if (state != State.Damage && navMeshInstance) {
            NavMeshDestroy();
        }
        if (isPressAttack && state != State.Attack && state != State.Jump) {
            isPressAttack = false;
        }
        if (!groundedFlag && !isPressAttack && attackedTimeRemain < 1f) {
            attackedTimeRemain = 1f;
        }
        if (GetCanControl() && targetTrans && lineCastFrom) {
            lineCastHit = Physics.Linecast(lineCastFrom.position, targetTrans.position, fieldLayerMask, QueryTriggerInteraction.Ignore);
        }
        if (move.y < moveYLowerLimit) {
            move.y = moveYLowerLimit;
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
    }

    void ThrowRushStart() {
        throwRushTimeRemain = 60f / 60f;
        throwRushIntervalRemain = 0f;
    }

    void ThrowBombReady() {
        throwing.ThrowReady(0);
    }

    void ThrowBombStart() {
        if (targetTrans) {
            float dist = Vector3.Distance(targetTrans.position, throwing.throwSettings[0].from.transform.position);
            throwing.throwSettings[0].velocity = 22f + dist * 0.5f;
        } else {
            throwing.throwSettings[0].velocity = 22f;
        }
        throwing.ThrowStart(0);
    }

    public void QuakeAttack(int index) {
        if (state == State.Attack) {
            switch (index) {
                case 0:
                    CameraManager.Instance.SetQuake(quakePivot_Nose.position, 10, 4, 0, 0, 1f, 3f * rootScale, dissipationDistance_Boss);
                    break;
                case 2:
                    CameraManager.Instance.SetQuake(quakePivot_Sword.position, 12, 4, 0, 0, 1.5f, 3f * rootScale, dissipationDistance_Boss);
                    break;
                case 3:
                    CameraManager.Instance.SetQuake(quakePivot_Bottom.position, 14, 4, 0, 0, 1.5f, 3f * rootScale, dissipationDistance_Boss);
                    break;
                case 5:
                    CameraManager.Instance.SetQuake(quakePivot_Bomb.position, 5, 8, 0, 0, 1f, 3f * rootScale, dissipationDistance_Boss);
                    break;
                case 21:
                    CameraManager.Instance.SetQuake(quakePivot_Sword.position, 10, 4, 0, 0, 1.5f, 3f * rootScale, dissipationDistance_Boss);
                    break;
            }
        }
    }

    public void QuakeKnock() {
        if (!isItem) {
            CameraManager.Instance.SetQuake(quakePivot_Knock.position, 5, 4, 0, 0, 1.5f, 3f * rootScale, dissipationDistance_Boss);
        }
    }

    void QuakeScale() {
        if (!isItem) {
            CameraManager.Instance.SetQuake(GetCenterPosition(), 5, 4, 0.5f, 0.5f, 0.5f, 3f * rootScale, dissipationDistance_Boss);
        }
    }

    void SetLockonRotSpeed(int param) {
        lockonRotSpeed = param;
    }
    
    public void MoveAttack(int index) {
        if (state == State.Attack) {
            float sqrDist = 5f;
            if (targetTrans) {
                sqrDist = GetTargetDistance(true, true, false);
            }
            switch (index) {
                case 0:
                    fbStepTime = 15f / 60f;
                    fbStepMaxDist = 4f * rootScale * CharacterManager.Instance.riskyIncSqrt;
                    if (sqrDist < MyMath.Square(3f * rootScale)) {
                        ApproachOrSeparate(3f * rootScale);
                    } else {
                        SpecialStep(3f * rootScale, fbStepTime, fbStepMaxDist, 0f, 0f, true, false);
                    }
                    break;
                case 1:
                    SpecialStep(0.5f * rootScale, 40f / 60f, 10f * CharacterManager.Instance.riskyIncSqrt, 5f, 5f, true, true);
                    break;
                case 2:
                    if (sqrDist < MyMath.Square(4f * rootScale)) {
                        fbStepTime = 30f / 60f;
                        fbStepMaxDist = 4f * rootScale * CharacterManager.Instance.riskyIncSqrt;
                        SeparateFromTarget(4f * rootScale);
                    } else {
                        SpecialStep(6f * rootScale, 30f / 60f, 5f * rootScale * CharacterManager.Instance.riskyIncSqrt, 0f, 0f, true, false);
                    }
                    break;
                case 3:
                    fbStepTime = 25f / 60f;
                    fbStepMaxDist = 4f * rootScale * CharacterManager.Instance.riskyIncSqrt;
                    SeparateFromTarget(4f);
                    break;
                case 8:
                    if (sqrDist < MyMath.Square(4f * rootScale)) {
                        fbStepTime = 15f / 60f;
                        fbStepMaxDist = 2f * rootScale * CharacterManager.Instance.riskyIncSqrt;
                        SeparateFromTarget(4f * rootScale);
                    } else {
                        SpecialStep(6f * rootScale, 15f / 60f, 2.5f * rootScale * CharacterManager.Instance.riskyIncSqrt, 0f, 0f, true, false);
                    }
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
            Jump(14 + powerPlus);
            cCon.radius = 0.2f;
        }
    }

    public override void EmitEffect(int index) {
        base.EmitEffect(index);
        if (effect[index].instance && !effect[index].parenting) {
            effect[index].instance.transform.localScale *= rootScale;
        }
    }

    public override void EmitEffectString(string type) {
        base.EmitEffectString(type);
        switch (type) {
            case "Dead":
                EmitEffect(0);
                break;
            case "NoseReady":
                EmitEffect(1);
                break;
            case "NoseEnd":
                EmitEffect(2);
                break;
            case "SpinReady":
                EmitEffect(3);
                break;
            case "SpinStart":
                EmitEffect(4);
                break;
            case "SwordReady":
                EmitEffect(5);
                break;
            case "SwordStart":
                EmitEffect(6);
                break;
            case "SwordEnd":
                EmitEffect(7);
                break;
            case "SwordBreak":
                EmitEffect(8);
                break;
            case "JumpStart":
                EmitEffect(9);
                break;
            case "JumpMiddle":
                EmitEffect(10);
                break;
            case "Press":
                EmitEffect(11);
                break;
            case "LanceReady":
                EmitEffect(12);
                break;
            case "BombReady":
                EmitEffect(13);
                break;
            case "KnockHeavy":
                EmitEffect(14);
                break;
            case "KnockRecover":
                EmitEffect(15);
                break;
            case "Scale":
                EmitEffect(16);
                break;
            case "BlockBreak":
                EmitEffect(17);
                break;
            case "SwordSpin":
                EmitEffect(4);
                break;
            case "NoseIceBreak":
                if (noseIceParent.activeSelf) {
                    EmitEffect(18);
                }
                break;
            case "SwordReadyFix":
                EmitEffect(19);
                break;
            case "SwordBreakFix":
                EmitEffect(20);
                break;
        }
    }

    protected override void KnockLightProcess() {
        if (state != State.Damage) {
            base.KnockLightProcess();
            if (!isSuperarmor) {
                if (move.y > 0f) {
                    move.y = 0f;
                }
                knockRestoreSpeed = 1f;
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 1f);
                throwRushTimeRemain = 0f;
                if (!groundedFlag && attackedTimeRemain < 1f) {
                    isPressAttack = false;
                    attackedTimeRemain = 1f;
                }
            }
        }
    }

    protected override void KnockHeavyProcess() {
        if (state != State.Damage || !isDamageHeavy) {
            base.KnockHeavyProcess();
            if (move.y > 0f) {
                move.y = 0f;
            }
            agent.radius = 0.1f;
            agent.height = 0.1f;
            knockRestoreSpeed = 1f;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], 1f);
            coreShowHP = nowHP;
            coreHideConditionDamage = GetCoreHideConditionDamage();
            throwRushTimeRemain = 0f;
        }
    }

    public override void SetSandstarRaw() {
        base.SetSandstarRaw();
        if (state != State.Dead) {
            coreHideDenomi = 10f;
            coreTimeMax = 3.5f;
        }
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        cannotKnockDown = (scalingTimeRemain > 0f);
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
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
        isJumpingAttack = false;
        if (!battleStarted) {
            BattleStart();
        }
        if (targetTrans) {
            float intervalPlus = 0f;
            if (isRaw) {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.3f, 0.5f) : weakProgress == 1 ? Random.Range(0.4f, 0.6f) : Random.Range(0.5f, 0.7f));
            } else {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.4f, 0.6f) : weakProgress == 1 ? Random.Range(0.6f, 0.9f) : Random.Range(0.9f, 1.2f));
            }
            int attackTemp = 0;
            float attackBias = (scaleProgress >= 2 ? 1.08f : scaleProgress >= 1 ? 1.04f : 1f);

            if (groundedFlag) {
                float sqrDistance = GetTargetDistance(true, true, false);
                if (attackSave < 0) {
                    attackTemp = 3;
                } else {
                    int min = (sqrDistance <= MyMath.Square(7f * rootScale) ? 0 : sqrDistance <= MyMath.Square((lineCastHit ? 8f : 12f) * rootScale) ? 1 : 3);
                    int max = (lineCastHit ? 4 : 6);
                    attackTemp = Random.Range(min, max);
                    if (attackTemp == attackSave) {
                        attackTemp = Random.Range(min, max);
                    }
                }
                if (!agent.isOnNavMesh || agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) {
                    attackTemp = 3;
                }
                attackSave = attackTemp;
                switch (attackTemp) {
                    case 0:
                        if (noseIceParent.activeSelf != (weakProgress >= 1)) {
                            noseIceParent.SetActive(weakProgress >= 1);
                        }
                        AttackBase(0, 1.12f * attackBias, 1.4f * attackBias, 0, 125f / 60f / attackSpeed, 125f / 60f / attackSpeed + intervalPlus, 0, attackSpeed);
                        break;
                    case 1:
                        AttackBase(1, 1f * attackBias, 1.2f * attackBias, 0, 190f / 60f / attackSpeed, 190f / 60f / attackSpeed + intervalPlus, 0, attackSpeed, false);
                        break;
                    case 2:
                        if (weakProgress <= 1) {
                            AttackBase(2, 1.08f * attackBias, 1.4f * attackBias, 0, 100f / 60f / attackSpeed, 100f / 60f / attackSpeed + intervalPlus, 0, attackSpeed, true, attackLockonDefaultSpeed * 1.2f);
                        } else {
                            swordType = (swordType + 1) % 2;
                            if (swordType == 1) {
                                AttackBase(8, 1.08f * attackBias, 1.4f * attackBias, 0, 210f / 60f / attackSpeed, 210f / 60f / attackSpeed + intervalPlus, 0, attackSpeed, true, attackLockonDefaultSpeed * 1.2f);
                            } else {
                                AttackBase(9, 1.08f * attackBias, 1.4f * attackBias, 0, 230f / 60f / attackSpeed, 230f / 60f / attackSpeed + intervalPlus, 0, attackSpeed, true, attackLockonDefaultSpeed * 1.2f);
                            }
                        }
                        break;
                    case 3:
                        AttackBase(3, 0, 0, 0, 45f / 60f, 45f / 60f, 0, 1f, false);
                        isPressAttack = true;
                        jumpAttackPosSave = trans.position.y;
                        break;
                    case 4:
                        AttackBase(4, 0.95f, 1f, 0, 140f / 60f, 140f / 60f + intervalPlus, 0, 1);
                        break;
                    case 5:
                        if (weakProgress <= 0) {
                            AttackBase(5, 1.05f, 1.1f, 0, 120f / 60f / attackSpeed, 120f / 60f / attackSpeed + intervalPlus, 0, attackSpeed, true, attackLockonDefaultSpeed * 0.5f);
                        } else {
                            AttackBase(6, 1.05f, 1.1f, 0, 235f / 60f / attackSpeed, 235f / 60f / attackSpeed + intervalPlus, 0, attackSpeed, true, attackLockonDefaultSpeed * 0.5f);
                        }
                        break;
                }
            } else {
                if (isPressAttack) {
                    isPressAttack = false;
                    isJumpingAttack = true;
                    AttackBase(7, 1.2f * attackBias, 3.4f * attackBias, 0, 5f, 90f / 60f + intervalPlus, 1, 1);
                    if (move.y > -8f) {
                        move.y = -8f;
                    }
                    jumpAttackPosSave = trans.position.y;
                    cCon.radius = 0.2f;
                }
            }
        }
    }

    protected override void Start_Process_Jump() {
        if (isPressAttack) {
            base.Start_Process_Jump();
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
                    if (state != State.Attack && sqrDist < 0.7f * 0.7f && attackedTimeRemain < 0) {
                        SetState(State.Attack);
                    }
                }
            }
            jumpAttackPosSave = trans.position.y;
        }
    }

    protected override void Update_Process_Jump() {
        base.Update_Process_Jump();
        if (isPressAttack && targetTrans && stateTime >= 0.1f) {
            lockonRotSpeed = attackLockonDefaultSpeed * (isRaw ? 1f : 0.6f);
            float sqrDist = GetTargetDistance(true, true, false);
            if (sqrDist > 0.1f * 0.1f) {
                if (sqrDist > 0.5f * 0.5f) {
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
                        QuakeAttack(3);
                    }
                    LockonEnd();
                    attackStiffTime = stateTime + 0.6f;
                    nowSpeed = 0;
                    gravityMultiplier = 1;
                } else {
                    if (isRaw && targetTrans) {
                        lockonRotSpeed = attackLockonDefaultSpeed * 1f;
                        float sqrDist = GetTargetDistance(true, true, false);
                        if (sqrDist > 2f * 2f) {
                            CommonLockon();
                        }
                    }
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

    public bool CheckTrophy_IceBomb(AttackDetection attacker) {
        AttackDetectionProjectile attackProjectile = attacker.GetComponent<AttackDetectionProjectile>();
        return attackProjectile != null;
    }

}
