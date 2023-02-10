using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityStandardAssets.Cameras;

public class Enemy_Heliozoa : EnemyBaseBoss
{

    public Transform quakePivot;
    public Transform[] movePivot;
    public Transform coreRot;
    public Transform coreSecondRot;
    public Transform[] corePattern;
    public DamageDetection criticalDD;
    public DamageDetection[] normalDD;
    public GameObject criticalParticle;
    public LaserOption[] laserOption;
    public RaycastToAdjustCapsuleCollider[] raycaster;
    public Transform laserPivot;
    public Transform throwPivot;
    public SearchTargetPriority criticalPriority;
    public GameObject bladeActivateObj;
    public Transform scaleTransform;
    public Projector[] scaleProjectors;
    public Transform swordPivot;
    public Transform meteorPivotY;
    public Transform meteorPivotX;
    public Transform meteorFrom;
    public LookatTarget lookatTarget;
    public Transform nullTarget;
    public Transform lookTargetTrans;
    public GameObject playerJumpingActivateObj;
    public Transform jumpingMoveConditionPoint;
    public Transform jumpingMoveHeightPoint;

    float commonSpeed = 1f;
    int movingIndex = -1;
    float movingSpeedMul;
    int attackSave = -1;
    float attackSpeed = 1;
    bool attracted;
    bool damageLockon;
    float jumpAttackPosSave;
    bool isPressAttack;
    int coreRotIndex;
    bool isCoreCritical;
    bool laserEnabled;
    bool laserLookatEnabled;
    float laserAttackedTimeRemain;
    bool throwLookatEnabled;
    int throwMax;
    bool specialKnockFlag;
    bool heavyKnocked;
    bool healEffectEmitted;
    int heavyKnockConditionDamage;
    float coreCriticalTimeRemain;
    float[] scaleProjectorsDefaultSize;
    float scaleSave;
    float meteorTimeRemain;
    int meteorNumRemain;
    int meteorNumMax;
    float moveLimitSqrDist;
    Vector3 moveStartPosSave;
    bool smoothRotEnabled;
    bool coreUpped;

    const float coreCriticalTimeMax = 20f;
    const float gravityMultiJumping = 1f;
    const float gravityMultiFalling = 4f;
    const float moveYLowerLimit = -25f;
    const int throwMeteorIndex = 33;
    const int attackIndexRam = 8;
    const int attackIndexPress = 9;
    const int attackIndexLaserReady = 10;
    const int attackIndexLaserBody = 12;
    const int attackTypeRam = 0;
    const int attackTypeStingLong = 1;
    const int attackTypeStingShort = 2;
    const int attackTypeJumpStart = 3;
    const int attackTypeJumpEnd = 4;
    const int attackTypeThrow = 5;
    const int attackTypeSword = 6;
    const int attackTypeMeteor = 7;
    const int attackTypeLaser = 8;
    const int attackTypeSatellite = 9;
    const int effDeadSave = 0;
    const int effKnockHeavy = 1;
    const int effStingReady = 2;
    const int effStingStart = 3;
    const int effStingEnd = 4;
    const int effRamStart = 5;
    const int effRamEnd = 6;
    const int effPressStart = 7;
    const int effPressMiddle = 8;
    const int effPressEnd = 9;
    const int effCriticalizeCore = 10;
    const int effHealCore = 11;
    const int effPortal = 12;
    const int effThrowReady = 13;
    const int effThrowStart = 14;
    const int effSwordReady = 15;
    const int effSwordStart = 16;
    const int effSwordEnd = 17;
    const int effMeteorReady = 18;
    const int effMeteorStart = 19;
    const int effLaserCountDown = 20;
    const int rewardMinmiShiftNum = 4;

    static readonly float[] speedPlusArray = new float[] { 0.5f, 0.5f, 0.6f, 0.7f, 0.8f };
    static readonly int[] attackLaserBody = new int[] { 12, 13 };

    protected override void Awake() {
        base.Awake();
        deadTimer = 3.5f;
        actDistNum = 0;
        attackedTimeRemainOnDamage = 0.1f;
        killByCriticalFailedKnockAmount = 10000f;
        killByCriticalOnly = true;
        attackWaitingLockonRotSpeed = 5f;
        checkGroundPivotHeight = 1f;
        checkGroundTolerance = 2f;
        checkGroundTolerance_Jumping = 2f;
        retargetingToPlayer = true;
        sandstarRawKnockEndurance = 1000000;
        sandstarRawKnockEnduranceLight = 6000;
        cannotDoubleKnockDown = true;
        catchupExpDisabled = true;
        checkGroundedLayerMask = LayerMask.GetMask("Field");
        CriticalizeCore(false);
        coreRotIndex = 0;
        if (corePattern[coreRotIndex]) {
            coreRot.localEulerAngles = corePattern[coreRotIndex].localEulerAngles;
        }
        LaserCancel();
        scaleProjectorsDefaultSize = new float[scaleProjectors.Length];
        for (int i = 0; i < scaleProjectors.Length; i++) {
            if (scaleProjectors[i]) {
                scaleProjectorsDefaultSize[i] = scaleProjectors[i].orthographicSize;
            }
        }
    }

    /*
    protected override void SetLevelModifier() {
        dropItem[0] = dropID_EX;
        SetDropRate(10000);
    }
    */

    void SetScale(float scale, float duration) {
        scaleSave = scale;
        if (scaleTransform) {
            scaleTransform.DOScale(scale, duration).SetEase(Ease.InOutSine);
        }
        for (int i = 0; i < scaleProjectors.Length; i++) {
            scaleProjectors[i].orthographicSize = scaleProjectorsDefaultSize[i] * scale;
        }
    }

    void SetScaleDefault() {
        if (scaleSave != 1f) {
            SetScale(1f, 0.8f);
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        if (scaleTransform) {
            scaleTransform.DOKill();
        }
    }

    Vector3 GetConsiderFallTargetPosition(Vector3 targetPos, Vector3 fromPos, float velocity) {
        if (velocity != 0f) {
            Vector3 targetXZ = targetPos;
            targetXZ.y = fromPos.y;
            float distance = Vector3.Distance(targetPos, fromPos);
            float reachTime = distance / velocity;
            float fallDist = -0.5f * Physics.gravity.y * reachTime * reachTime;
            targetPos.y += Mathf.Clamp(fallDist, 0, Vector3.Distance(targetXZ, fromPos));
        }
        return targetPos;
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        knockRemain = knockEndurance;
        if (GameManager.Instance.save.difficulty >= 2) {
            weakProgress = (nowHP <= GetMaxHP() / 3 ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 0);
        } else {
            weakProgress = (nowHP <= GetMaxHP() / 2 ? 1 : 0);
        }
        float nowHPTemp = nowHP;
        float maxHPTemp = GetMaxHP();
        if (sandstarRawEnabled) {
            commonSpeed = 1f + speedPlusArray[Mathf.Clamp(GameManager.Instance.save.difficulty, 0, speedPlusArray.Length - 1)];
        } else {
            if (maxHPTemp >= 1f) {
                commonSpeed = 1f + speedPlusArray[Mathf.Clamp(GameManager.Instance.save.difficulty, 0, speedPlusArray.Length - 1)] * ((maxHPTemp - nowHPTemp) / maxHPTemp);
            } else {
                commonSpeed = 1f;
            }
        }
        attractionTime = 4f / commonSpeed;
        attracted = (decoySave == target);
        if (actDistNum != 0) {
            actDistNum = attracted ? 2 : 1;
        }
        maxSpeed = (sandstarRawEnabled ? 13.5f : 9f) * commonSpeed;
        acceleration = maxSpeed * 2f;
        if (state != State.Attack && state != State.Jump) {
            gravityMultiplier = 1f;
            if (scaleSave != 1f) {
                SetScaleDefault();
            }
        }
        if (isPressAttack && state != State.Attack && state != State.Jump) {
            isPressAttack = false;
        }
        if (state != State.Attack && laserEnabled) {
            LaserCancel();
        }
        if (state != State.Damage && damageLockon) {
            damageLockon = false;
        }
        if (state == State.Damage && damageLockon) {
            lockonRotSpeed = 4f;
            CommonLockon();
        }
        if (state == State.Attack && laserLookatEnabled) {
            retargetingConditionTime = 0.001f;
        } else {
            retargetingConditionTime = 8f;
        }
        if ((state != State.Attack || attackType != attackTypeThrow) && throwing.GetAnyReady()) {
            throwing.ThrowCancelAll(true);
        }
        if (Time.timeScale > 0f) {
            if (isCoreCritical && coreCriticalTimeRemain > 0f) {
                coreCriticalTimeRemain -= deltaTimeCache * CharacterManager.Instance.riskyIncSqrt;
                if (!healEffectEmitted && coreCriticalTimeRemain <= 1f) {
                    EmitHealEffect();
                }
                if (coreCriticalTimeRemain <= 0f) {
                    ResetCore();
                }
            }
        }
        if (move.y < moveYLowerLimit) {
            move.y = moveYLowerLimit;
        }
        if (targetTrans) {
            lookTargetTrans.position = GetConsiderFallTargetPosition(targetTrans.position, meteorPivotX.position, throwing.throwSettings[throwMeteorIndex].velocity);
            lookatTarget.SetTarget(lookTargetTrans);
        } else {
            lookatTarget.SetTarget(nullTarget);
        }
        laserAttackedTimeRemain -= deltaTimeMove;
        if (playerJumpingActivateObj && CharacterManager.Instance.pCon) {
            bool toActive = CharacterManager.Instance.pCon.IsJumping;
            if (playerJumpingActivateObj.activeSelf != toActive) {
                playerJumpingActivateObj.SetActive(toActive);
            }
        }
        if (state != State.Attack && coreUpped) {
            CoreRestoreForRamAttack();
        }
        if (battleStarted && GameManager.Instance.save.difficulty <= 2 && !sandstarRawEnabled) {
            for (int i = 0; i < weakPoints.Length; i++) {
                if (weakPoints[i].instance) {
                    if (weakPoints[i].type == WeakPointType.Other && !isCoreCritical) { 
                        Destroy(weakPoints[i].instance);
                    }
                } else if (!weakPoints[i].used) {
                    if (weakPoints[i].type == WeakPointType.Other && isCoreCritical) {
                        weakPoints[i].instance = Instantiate(CharacterManager.Instance.weakPointPrefab[weakPoints[i].prefabIndex], weakPoints[i].pivot);
                        weakPoints[i].used = true;
                    }
                }
            }
        }
    }

    void RestoreKnockDown() {
        damageLockon = true;
        fbStepMaxDist = 10f;
        fbStepTime = 1f;
        fbStepIgnoreY = true;
        SeparateFromTarget(10f);
    }

    public override void EmitEffectString(string type) {
        switch (type) {
            case "DeadSave":
                EmitEffect(effDeadSave);
                break;
            case "KnockHeavy":
                EmitEffect(effKnockHeavy);
                break;
            case "StingReady":
                if (state == State.Attack && (attackType == attackTypeStingLong || attackType == attackTypeStingShort)) {
                    EmitEffect(effStingReady);
                }
                break;
            case "StingStart":
                if (state == State.Attack && (attackType == attackTypeStingLong || attackType == attackTypeStingShort)) {
                    EmitEffect(effStingStart);
                }
                break;
            case "StingEnd":
                if (state == State.Attack && (attackType == attackTypeStingLong || attackType == attackTypeStingShort)) {
                    EmitEffect(effStingEnd);
                }
                break;
            case "RamStart":
                if (state == State.Attack && attackType == attackTypeRam) {
                    EmitEffect(effRamStart);
                }
                break;
            case "PressStart":
                EmitEffect(effPressStart);
                break;
            case "PressMiddle":
                EmitEffect(effPressMiddle);
                break;
            case "PressEnd":
                if (state == State.Attack && attackType == attackTypeJumpEnd) {
                    EmitEffect(effPressEnd);
                }
                break;
            case "CriticalizeCore":
                EmitEffect(effCriticalizeCore);
                break;
            case "Portal":
                EmitEffect(effPortal);
                break;
            case "SwordReady":
                if (state == State.Attack) {
                    EmitEffect(effSwordReady);
                }
                break;
            case "SwordStart":
                if (state == State.Attack) {
                    EmitEffect(effSwordStart);
                }
                break;
            case "SwordEnd":
                if (state == State.Attack) {
                    EmitEffect(effSwordEnd);
                    QuakeAttack_Short();
                }
                break;
            case "LaserCountDown":
                if (state == State.Attack) {
                    EmitEffect(effLaserCountDown);
                    EmitEffect(effLaserCountDown + 1);
                }
                break;
        }
    }

    void MoveAttack_StingLong() {
        if (state == State.Attack && movePivot[0]) {
            movingIndex = 0;
            movingSpeedMul = 1f;
            moveLimitSqrDist = (movePivot[0].position - trans.position).sqrMagnitude;
            moveStartPosSave = trans.position;
        }
    }

    void MoveAttack_StingShort() {
        if (state == State.Attack && movePivot[1]) {
            movingIndex = 1;
            movingSpeedMul = 1f;
            moveLimitSqrDist = (movePivot[1].position - trans.position).sqrMagnitude;
            moveStartPosSave = trans.position;
        }
    }

    void MoveAttack_JumpEnd() {
        if (state == State.Attack) {
            movingIndex = 2;
            movingSpeedMul = 0.25f;
        }
    }

    void ActivateAgent() {
        agent.enabled = true;
    }

    void MoveAttackRam_1() {
        if (state == State.Attack) {
            fbStepTime = 30f / 60f / attackSpeed;
            fbStepIgnoreY = true;
            fbStepMaxDist = 6f;
            SeparateFromTarget(4f);
        }
    }

    void MoveAttackRam_2() {
        LockonEnd();
        if (state == State.Attack) {
            EmitEffect(effRamStart);
            float distance = 1f;
            if (target) {
                distance = GetTargetDistance(false, true, false) + 0.7f;
            }
            agent.enabled = false;
            SetSpecialMove(trans.TransformDirection(vecForward), Mathf.Clamp(distance, 4f, 20f), 25f / 60f / attackSpeed, EasingType.SineIn, ActivateAgent);
            AttackStart(attackIndexRam);
        }
    }

    void MoveAttackRamEnd() {
        specialMoveDuration = 0f;
        EmitEffect(effRamEnd);
        AttackEnd(attackIndexRam);
    }

    void CriticalizeCore(bool flag) {
        if (criticalDD) {
            if (flag) {
                criticalDD.damageRate = 50f;
                criticalDD.knockedRate = 20f;
                criticalDD.colorType = damageColor_Critical;
                for (int i = 0; i < normalDD.Length; i++) {
                    if (normalDD[i]) {
                        normalDD[i].colorType = damageColor_Enemy;
                    }
                }
                heavyKnocked = false;
                heavyKnockConditionDamage = 0;
                coreCriticalTimeRemain = coreCriticalTimeMax;
                healEffectEmitted = false;
            } else {
                criticalDD.damageRate = 2.5f;
                criticalDD.knockedRate = 0.5f;
                criticalDD.colorType = damageColor_Enemy;
                for (int i = 0; i < normalDD.Length; i++) {
                    if (normalDD[i]) {
                        normalDD[i].colorType = damageColor_Effective;
                    }
                }
            }
        }
        if (criticalParticle) {
            criticalParticle.SetActive(flag);
        }
        if (criticalPriority) {
            criticalPriority.priority = flag ? 8 : 0;
        }
        knockRemainLight = knockEnduranceLight;
        isCoreCritical = flag;
    }

    void EmitHealEffect() {
        if (isCoreCritical && !healEffectEmitted) {
            effect[effHealCore].pivot.position = effect[effCriticalizeCore].pivot.position;
            EmitEffect(effHealCore);
            healEffectEmitted = true;
        }
    }

    void ResetCore() {
        EmitHealEffect();
        if (coreRot) {
            Vector3 eulerTemp = new Vector3(Random.Range(0f, 120f), Random.Range(-180f, 180f), 0f);
            if (eulerTemp.x >= 108f) {
                eulerTemp.x = 120f;
            }
            if ((eulerTemp.x > 72f && eulerTemp.x < 108f && eulerTemp.y > -18f && eulerTemp.y < 18f) || (eulerTemp.x > 72f && (eulerTemp.y < -108f || eulerTemp.y > 108f))) {
                coreRotIndex = (coreRotIndex + 1) % corePattern.Length;
                if (corePattern[coreRotIndex]) {
                    eulerTemp = corePattern[coreRotIndex].localEulerAngles;
                }
            }
            coreRot.localEulerAngles = eulerTemp;
        }
        if (isCoreCritical) {
            CriticalizeCore(false);
        }
    }
    
    void MovingEnd() {
        movingIndex = -1;
        LockonEnd();
    }

    void JumpAttack() {
        if (state == State.Attack) {
            gravityMultiplier = gravityMultiJumping;
            float powerPlus = 0f;
            if (targetTrans && targetTrans.position.y > trans.position.y + 8f) {
                powerPlus = Mathf.Clamp((targetTrans.position.y - (trans.position.y + 8f)), 0f, 10f);
            }
            EmitEffect(effPressStart);
            Jump(14f + powerPlus);
        }
    }

    void QuakeAttack_Short() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePivot.position, 10, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
        }
    }

    void QuakeAttack() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePivot.position, 14, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
        }
    }

    void QuakeHeavyKnock() {
        if (state == State.Damage) {
            CameraManager.Instance.SetQuake(quakePivot.position, 5, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
        }
        EmitEffect(effKnockHeavy);
    }

    protected override void BattleStart() {
        base.BattleStart();
        actDistNum = 1;
        if (anim) {
            anim.speed = 1f;
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (!battleStarted) {
            BattleStart();
        }
    }

    protected override void KnockLightProcess() {
        if (!isSuperarmor) {
            meteorNumRemain = 0;
            if (knockRestoreSpeed != commonSpeed) {
                knockRestoreSpeed = commonSpeed;
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
            }
            if (!isCoreCritical) {
                CriticalizeCore(true);
                EmitEffect(effCriticalizeCore);
            }
        }
        base.KnockLightProcess();
    }

    protected override void KnockHeavyProcess() {
        if (specialKnockFlag) {
            knockRestoreSpeed = 1f;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
            anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Climb], true);
            specialKnockFlag = false;
        } else {
            knockRestoreSpeed = commonSpeed;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
            anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Climb], false);
            heavyKnocked = true;
        }
        EmitHealEffect();
        base.KnockHeavyProcess();
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (isCoreCritical && colorType == damageColor_Critical && GetCanTakeDamage(penetrate)) {
            heavyKnockConditionDamage += damage;
        }
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    public override float GetKnocked() {
        return base.GetKnocked() * (!heavyKnocked && isCoreCritical && heavyKnockConditionDamage >= 500000 ? 0 : 1);
    }

    protected override void DeadProcess() {
        if (!isForAmusement) {
            GameManager.Instance.save.minmi |= (1 << rewardMinmiShiftNum);
            if (isLastOne) {
                GameManager.Instance.SetSecret(GameManager.SecretType.SkytreeCleared);
                GameManager.Instance.save.SetClearStage(StageManager.Instance.stageNumber, false);
                if (sandstarRawEnabled && CharacterManager.Instance.bossResult.minmiSilverFlag) {
                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_HeliozoaSilver, true);
                }
            }
        }
        base.DeadProcess();
    }

    void AttackStartStingSet() {
        for (int i = 0; i < 8; i++) {
            AttackStart(i);
        }
    }

    void AttackEndStingSet() {
        for (int i = 0; i < 8; i++) {
            AttackEnd(i);
        }
    }

    void LaserCancel() {
        laserEnabled = false;
        for (int i = 0; i < laserOption.Length; i++) {
            if (laserOption[i]) {
                laserOption[i].CancelLaser();
            }
        }
        for (int i = 0; i < raycaster.Length; i++) {
            if (raycaster[i]) {
                raycaster[i].Deactivate();
            }
        }
    }

    void LaserReady() {
        laserEnabled = true;
        for (int i = 0; i < laserOption.Length; i++) {
            if (laserOption[i]) {
                laserOption[i].LightFlickeringChargeStart();
            }
        }
        for (int i = 0; i < raycaster.Length; i++) {
            if (raycaster[i]) {
                raycaster[i].hitEffectEnabled = false;
                raycaster[i].Activate();
            }
        }
    }

    void LaserStart() {
        laserEnabled = true;
        for (int i = 0; i < laserOption.Length; i++) {
            if (laserOption[i]) {
                laserOption[i].LightFlickeringChargeEnd();
            }
        }
        for (int i = 0; i < raycaster.Length; i++) {
            if (raycaster[i]) {
                raycaster[i].hitEffectEnabled = true;
                raycaster[i].Activate();
            }
        }
    }

    void LaserEnd() {
        laserEnabled = false;
        for (int i = 0; i < laserOption.Length; i++) {
            if (laserOption[i]) {
                laserOption[i].LightFlickeringBlastEnd();
            }
        }
        for (int i = 0; i < raycaster.Length; i++) {
            if (raycaster[i]) {
                raycaster[i].Deactivate();
            }
        }
    }

    int GetAttackTemp() {
        int attackTemp = 0;
        int max = (laserAttackedTimeRemain <= 0f ? 7 : 6);
        attackTemp = Random.Range(0, max);
        if (laserAttackedTimeRemain <= -10f && Random.Range(0, 2) == 0) {
            attackTemp = 6;
        }
        return attackTemp;
    }
    
    void LaserReadySet() {
        if (state == State.Attack) {
            LaserReady();
            AttackStart(attackIndexLaserReady);
            AttackStart(attackIndexLaserReady + 1);
            laserLookatEnabled = true;
            if (laserPivot) {
                laserPivot.localRotation = quaIden;
            }
        }
    }

    void LaserStartSet() {
        if (state == State.Attack && attackType == attackTypeLaser) {
            LaserStart();
            AttackEnd(attackIndexLaserReady);
            AttackEnd(attackIndexLaserReady + 1);
            AttackStart(attackIndexLaserBody);
            AttackStart(attackIndexLaserBody + 1);
        }
    }

    void LaserEndSet() {
        if (state == State.Attack) {
            LaserEnd();
            AttackEnd(attackIndexLaserBody);
            AttackEnd(attackIndexLaserBody + 1);
            laserLookatEnabled = false;
        }
    }

    void ThrowReadySet() {
        if (state == State.Attack && attackType == attackTypeThrow) {
            EmitEffect(effThrowReady);
            if (throwPivot) {
                if (targetTrans) {
                    Vector3 targetPos = targetTrans.position;
                    Vector3 fromPos = throwPivot.position;
                    if (targetPos.y <  fromPos.y - 1f && (targetPos - fromPos).sqrMagnitude < 5f * 5f) {
                        targetPos.y += Mathf.Min(fromPos.y - 1f - targetPos.y, 0.4f);
                    }
                    Quaternion lookRot = Quaternion.LookRotation(targetPos - fromPos);
                    throwPivot.rotation = lookRot;
                } else {
                    throwPivot.localRotation = quaIden;
                }
            }
            throwLookatEnabled = true;
            throwMax = (weakProgress >= 2 ? 33 : weakProgress == 1 ? 17 : 9);
            for (int i = 0; i < throwMax; i++) {
                throwing.ThrowReady(i);
            }
        }
    }

    void ThrowStartSet() {
        if (state == State.Attack && attackType == attackTypeThrow) {
            EmitEffect(effThrowStart);
            throwLookatEnabled = false;
            for (int i = 0; i < throwMax; i++) {
                throwing.ThrowStart(i);
            }
        }
    }

    void MeteorStart() {
        if (state == State.Attack) {
            meteorNumRemain = meteorNumMax = (weakProgress >= 2 ? 8 : weakProgress == 1 ? 6 : 4);
            meteorTimeRemain = 35f / 60f;
            throwing.throwSettings[throwMeteorIndex].randomDirection = (0.1f + meteorNumMax * 0.1f) * Vector3.one;
        }
    }

    void MeteorThrow() {
        if (state == State.Attack && meteorNumRemain > 0 && meteorNumMax > 0 && meteorTimeRemain <= 35f / 60f * meteorNumRemain / meteorNumMax) {
            meteorNumRemain--;
            if (!GetSick(SickType.Stop)) {
                EmitEffect(effMeteorStart);
                int meteorCluster = meteorNumMax / 2;
                for (int i = 0; i < meteorCluster; i++) {
                    Vector2 randCircle = Random.insideUnitCircle * 2.5f;
                    Vector3 randVec3 = vecZero;
                    randVec3.x = randCircle.x;
                    randVec3.y = randCircle.y;
                    meteorFrom.localPosition = randVec3;
                    throwing.throwSettings[throwMeteorIndex].angularVelocity = Random.insideUnitSphere * 4f;
                    throwing.ThrowStart(throwMeteorIndex);
                }
            }
            if (meteorNumRemain <= 0) {
                LockonEnd();
            }
        }
    }

    void SmoothRotEnd() {
        smoothRotEnabled = false;
    }
    
    protected override void Attack() {
        resetAgentRadiusOnChangeState = true;
        throwLookatEnabled = false;
        laserLookatEnabled = false;
        movingIndex = -1;
        meteorNumRemain = 0;
        moveLimitSqrDist = 0;
        smoothRotEnabled = false;
        if (targetTrans) {
            attackSpeed = commonSpeed;
            float intervalPlus = 0f;
            intervalPlus = Random.Range(0.6f, 1.2f) / Mathf.Max(commonSpeed, 1f);
            if (groundedFlag) {
                int attackTemp = 0;
                int typeSub = 0;
                float sqrDistance = GetTargetDistance(true, true, false);
                if (!battleStarted) {
                    BattleStart();
                    attackTemp = 2;
                } else {
                    attackTemp = GetAttackTemp();
                    if (attackTemp == attackSave) {
                        attackTemp = GetAttackTemp();
                    }
                    if ((attackSave == 1 && attackTemp == 1) || (attackSave == 5 && attackTemp == 5)) {
                        attackTemp = MyMath.RandomRangeExclude(0, 6, attackSave);
                    }
                }
                attackSave = attackTemp;

                switch (attackTemp) {
                    case 0: //Sting
                        if (sqrDistance > 10 * 10) {
                            typeSub = 0;
                        } else if (sqrDistance < 6 * 6) {
                            typeSub = 1;
                        } else {
                            typeSub = Random.Range(0, 2);
                        }
                        EmitEffect(effStingReady);
                        if (typeSub == 0) {
                            AttackBase(attackTypeStingLong, 1, 1.2f, 0, 125f / 60f / attackSpeed, 125f / 60f / attackSpeed + intervalPlus, 0.25f, attackSpeed);
                        } else {
                            AttackBase(attackTypeStingShort, 1, 1.2f, 0, 99f / 60f / attackSpeed, 99f / 60f / attackSpeed + intervalPlus, 0.25f, attackSpeed);
                        }
                        break;
                    case 1: //JumpStart
                        AttackBase(attackTypeJumpStart, 0, 0, 0, 70f / 60f / attackSpeed + 0.125f, 70f / 60f / attackSpeed + 0.25f, 0, attackSpeed, false);
                        jumpAttackPosSave = trans.position.y;
                        isPressAttack = true;
                        if (bladeActivateObj.activeSelf != (weakProgress >= 1)) {
                            bladeActivateObj.SetActive(weakProgress >= 1);
                        }
                        if (weakProgress >= 2) {
                            SetScale(1.5f, 1f / commonSpeed);
                        }
                        break;
                    case 2: //Ram
                        AttackBase(attackTypeRam, 1.1f, 2.4f, 0, 180f / 60f / attackSpeed, 180f / 60f / attackSpeed + intervalPlus, 0.25f, attackSpeed);
                        break;
                    case 3://Throw
                        AttackBase(attackTypeThrow, 1f, 1.2f, 0, 153f / 60f / attackSpeed, 153f / 60f / attackSpeed + intervalPlus, 0.25f, attackSpeed, false);
                        break;
                    case 4://Sword
                        AttackBase(attackTypeSword, 1.2f, 2.1f, 0, 150f / 60f / attackSpeed, 150f / 60f / attackSpeed + intervalPlus, 0.25f, attackSpeed, false);
                        EmitEffect(effSwordReady);
                        smoothRotEnabled = true;
                        swordPivot.localEulerAngles = vecZero;
                        if (targetTrans) {
                            Vector3 targetPos = targetTrans.position;
                            targetPos.y = 0f;
                            if (targetPos.sqrMagnitude > 0f) {
                                swordPivot.LookAt(targetPos);
                            } else {
                                swordPivot.localEulerAngles = vecZero;
                            }
                        } else {
                            swordPivot.localEulerAngles = vecZero;
                        }
                        break;
                    case 5://Meteor
                        AttackBase(attackTypeMeteor, 1f, 1.6f, 0, 135f / 60f, 135f / 60f + intervalPlus, 0, 1f, true, attackLockonDefaultSpeed);
                        EmitEffect(effMeteorReady);
                        smoothRotEnabled = true;
                        if (targetTrans) {
                            Vector3 targetPos = targetTrans.position;
                            targetPos.y = 0f;
                            if (targetPos.sqrMagnitude > 0f) {
                                meteorPivotY.LookAt(targetPos);
                            } else {
                                meteorPivotY.localEulerAngles = vecZero;
                            }
                        } else {
                            meteorPivotY.localEulerAngles = vecZero;
                        }
                        if (targetTrans) {
                            fbStepMaxDist = 7.5f;
                            fbStepTime = 45f / 60f;
                            SeparateFromTarget(12f);
                        }
                        break;
                    case 6://Laser
                        AttackBase(attackTypeLaser, 1.4f, 50f, 0, 270f / 60f, 270f / 60f + intervalPlus + 0.5f, 0, 1, false);
                        laserAttackedTimeRemain = 12f;
                        LaserReadySet();
                        for (int i = 0; i < attackLaserBody.Length; i++) {
                            attackDetection[attackLaserBody[i]].multiHitInterval = 0.1f;
                        }
                        break;
                }
            } else {
                if (isPressAttack) { //JumpEnd
                    isPressAttack = false;
                    AttackBase(attackTypeJumpEnd, 1.2f, 3.4f, 0, 5f, 120f / 60f + intervalPlus, 1, 1, false);
                    if (move.y > -8f) {
                        move.y = -8f;
                    }
                    jumpAttackPosSave = trans.position.y;
                    EmitEffect(effPressMiddle);
                    SuperarmorStart();
                    MoveAttack_JumpEnd();
                    AttackStart(attackIndexPress);
                }
            }
        }
    }

    void SlowLockonRotSpeed() {
        if (state == State.Attack) {
            lockonRotSpeed *= 0.3f;
        }
    }

    bool IsPlayerOnHead() {
        if (CharacterManager.Instance.playerTrans) { 
            return (CharacterManager.Instance.playerTrans.position.y > jumpingMoveConditionPoint.position.y && MyMath.SqrMagnitudeIgnoreY(trans.position, CharacterManager.Instance.playerTrans.position) < MyMath.SqrMagnitudeIgnoreY(trans.position, jumpingMoveConditionPoint.position));
        }
        return false;
    }

    protected override void Update_Transition_Jump() {
        base.Update_Transition_Jump();
        if (isPressAttack) {
            if (stateTime > 0.6f) {
                if (move.y < -5f || (targetTrans == null && move.y < 0f)) {
                    SetState(State.Attack);
                } else if (targetTrans && state != State.Attack && GetTargetDistance(true, true, false) < scaleSave * scaleSave && attackedTimeRemain < 0) {
                    SetState(State.Attack);
                } else if (state != State.Attack && IsPlayerOnHead()) {
                    SetState(State.Attack);
                }
            }
            jumpAttackPosSave = trans.position.y;
        }
    }

    protected override void Update_Process_Jump() {
        base.Update_Process_Jump();
        if (targetTrans && isPressAttack && stateTime >= 0.1f) {
            lockonRotSpeed = attackLockonDefaultSpeed * 0.5f * commonSpeed;
            float sqrDist = GetTargetDistance(true, true, false);
            if (sqrDist > 0.1f * 0.1f) {
                float distCondition = 0.5f * scaleSave;
                if (sqrDist > distCondition * distCondition) {
                    CommonLockon();
                }
                float speedBias = 1f;
                if (distCondition > 0f && sqrDist < distCondition * distCondition) {
                    float dist = GetTargetDistance(false, true, false);
                    speedBias = Mathf.Clamp(dist * (1f / distCondition), 0f, 1f);
                }
                if (IsPlayerOnHead()) {
                    if (CharacterManager.Instance.playerTrans && CharacterManager.Instance.playerTrans.position.y < jumpingMoveHeightPoint.position.y) {
                        speedBias = 0f;
                    } else {
                        speedBias *= 0.5f;
                    }
                }
                cCon.Move(GetTargetVector(true, true) * Mathf.Clamp(GetMaxSpeed() * 3f, 0f, 30f) * speedBias * deltaTimeMove);
            }
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (movingIndex >= 0 && movingIndex < movePivot.Length && movePivot[movingIndex] != null) {
            if (moveLimitSqrDist > 0 && (trans.position - moveStartPosSave).sqrMagnitude > moveLimitSqrDist) {
                movingIndex = -1;
            } else {
                ApproachTransformPivot(movePivot[movingIndex], GetMinmiSpeed() * movingSpeedMul, 0.25f);
            }
        }
        switch (attackType) {
            case attackTypeJumpEnd:
                gravityMultiplier = gravityMultiFalling;
                if (groundedFlag || stateTime > 3f || (stateTime > 0.6f && trans.position.y >= jumpAttackPosSave)) {
                    attackType = -1;
                    isAnimStopped = false;
                    if (groundedFlag && quakePivot) {
                        ray.origin = trans.position + vecUp;
                        ray.direction = vecDown;
                        if (Physics.Raycast(ray, out raycastHit, 2f, checkGroundedLayerMask, QueryTriggerInteraction.Ignore)) {
                            quakePivot.position = raycastHit.point;
                        } else {
                            quakePivot.position = trans.position;
                        }
                        EmitEffect(effPressEnd);
                        QuakeAttack();
                        quakePivot.position = trans.position;
                    }
                    MovingEnd();
                    attackStiffTime = stateTime + 100f / 60f / attackSpeed;
                    attackedTimeRemain = (100f / 60f + Random.Range(0.5f, 1f)) / attackSpeed;
                    nowSpeed = 0;
                    gravityMultiplier = 1;
                } else {
                    attackStiffTime = stateTime + 1.0f;
                    if (attackedTimeRemain < 1.5f) {
                        attackedTimeRemain = 1.5f;
                    }
                }
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], attackType < 0 || !isAnimStopped ? attackSpeed : 0);
                if (Time.timeScale > 0f) {
                    jumpAttackPosSave = trans.position.y;
                }
                break;
            case attackTypeThrow:
                if (throwLookatEnabled && throwPivot && targetTrans && (targetTrans.position - throwPivot.position).sqrMagnitude > 5f * 5f) {
                    Vector3 fromPosition = throwing.throwSettings[0].from.transform.position;
                    Vector3 targetPos = GetConsiderFallTargetPosition(targetTrans.position, fromPosition, throwing.throwSettings[0].velocity);
                    SmoothRotation(throwPivot, targetPos - fromPosition, 6f * commonSpeed);
                }
                break;
            case attackTypeSword:
                if (smoothRotEnabled && targetTrans && swordPivot) {
                    SmoothRotation(swordPivot, GetTargetVector(true, true, false), lockonRotSpeed * 3.6f, 1f / 6f, true);
                }
                break;
            case attackTypeMeteor:
                if (smoothRotEnabled && targetTrans && meteorPivotY) {
                    SmoothRotation(meteorPivotY, GetTargetVector(true, true, false), lockonRotSpeed * 3.6f * Mathf.Min(GetLockonRotSpeedRate(), 2f), 1f / 6f, true);
                }
                if (meteorNumRemain > 0) {
                    MeteorThrow();
                    meteorTimeRemain -= deltaTimeMove;
                }
                break;
            case attackTypeLaser:
                if (laserLookatEnabled && laserPivot && targetTrans) {
                    SmoothRotation(laserPivot, targetTrans.position - laserPivot.position, 3f * commonSpeed, sandstarRawEnabled ? 1f : 0.875f);
                    for (int i = 0; i < attackLaserBody.Length; i++) {
                        attackDetection[attackLaserBody[i]].multiHitInterval = 0.1f;
                    }
                }
                break;
        }
    }

    void SpawnStop() {
        if (!battleStarted && anim) {
            anim.speed = 0f;
        }
    }

    public override void SetFalling() {
        trans.position = vecZero;
        specialKnockFlag = true;
        KnockHeavyProcess();
        move = vecZero;
        specialMoveDuration = 0f;
        if (isCoreCritical && coreCriticalTimeRemain > 0f) {
            coreCriticalTimeRemain = coreCriticalTimeMax;
        }
    }

    void SpecialDamage() {
        int damageTemp = 300000;
        if (damageTemp > 0) {
            AddNowHP(-damageTemp, centerPivot.position, true, damageColor_Critical);
        }
        if (nowHP <= 0) {
            SetState(State.Dead);
        }
    }

    void LaserMultiHitEnd() {
        if (state == State.Attack) {
            for (int i = 0; i < attackLaserBody.Length; i++) {
                attackDetection[attackLaserBody[i]].multiHitInterval = 0f;
            }
        }
    }

    void CoreUpForRamAttack() {
        Vector3 rotEuler = coreRot.localEulerAngles;
        if (attackSpeed > 0f && rotEuler.x >= 59f && rotEuler.x <= 61f && rotEuler.y > 105f && rotEuler.y < 255f && rotEuler.z >= 179f && rotEuler.z <= 181f) {
            Vector3 rotEnd = new Vector3(-10f * (75f - Mathf.Abs(180f - rotEuler.y)) / 75f, 0f, 0f);
            coreSecondRot.DOLocalRotate(rotEnd, (7f / 60f) / attackSpeed);
            coreUpped = true;
        }
    }

    void CoreRestoreForRamAttack() {
        if (coreUpped && attackSpeed > 0f) {
            coreSecondRot.DOLocalRotate(vecZero, (21f / 60f) / attackSpeed);
            coreUpped = false;
        }
    }

}
