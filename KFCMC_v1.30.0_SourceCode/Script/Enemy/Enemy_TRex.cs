using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_TRex : EnemyBase {

    public AudioSource[] footAudioSource;
    public Transform[] movePivot;
    public Transform[] quakePivot;
    public GameObject[] coreObj;
    public ParticleSystem[] coreParticles;
    public LaserOption laserOption;
    public RaycastToAdjustCapsuleCollider raycaster;
    public Transform targetChaser;
    public CheckTriggerStay backChecker;

    int coreHPSave;
    int coreIndex = -1;
    int movingIndex = -1;
    float movingSpeedMul = 1f;
    int attackSave = -1;
    float changeCoreReservedTimeRemain;
    float meteorAttackedTimeRemain;
    float laserAttackedTimeRemain;
    bool laserEnabled;
    bool targetFound;

    float throwRushTimeRemain;
    float throwRushIntervalRemain;
    int rushCount;
    int firePoleCount;
    bool targetChaserEnabled;

    const int effRightFootMove = 2;
    const int effLeftFootMove = 3;
    const int effJumpSmall = 4;
    const int effJumpBig = 5;
    const int effPressSmall = 6;
    const int effPressBig = 7;
    const int effSpin = 9;
    const int effMeteor = 10;
    const int effKnockDown = 12;

    const int atkLaserDummy = 4;
    const int atkLaserBody = 5;

    static readonly int[] coreChangeDivide = new int[] { 5, 5, 6, 7, 8 };
    static readonly float[] laserLockonSpeedArray = new float[] { 2.5f, 2.5f, 2.833333f, 3.166666f, 3.5f };
    static readonly float[] knockRestoreArray = new float[] { 1f, 1f, 1.1f, 1.21f, 1.331f };
    static readonly float[] intervalPlusArray = new float[] { 0.8f, 0.8f, 0.7f, 0.6f, 0.5f };

    protected override void Awake() {
        base.Awake();
        attackWaitingLockonRotSpeed = 0.7f;
        stoppingDistanceBattle = 4f;
        mapChipSize = 1;
        normallyRequiredMaxMultiplier = 1.5f;
    }

    protected override void Update_AnimControl() {
        base.Update_AnimControl();
        UpdateAC_Run();
    }

    protected override void SetLevelModifier() {
        coreIndex = -1;
        ChangeCore();
        targetFound = false;
        laserAttackedTimeRemain = 0f;
        meteorAttackedTimeRemain = 0f;
        dropRate[0] = 250;
        dropRate[1] = 250;
        knockRecovery = (knockRecovery + defStats.knockRecovery) * 0.5f;
        knockRestoreSpeed = knockRestoreArray[Mathf.Clamp(level, 0, knockRestoreArray.Length - 1)];
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
    }

    void ChangeCore() {
        changeCoreReservedTimeRemain = -1f;
        coreHPSave = nowHP;
        if (coreObj.Length > 1) {
            bool particlePlayFlag = false;
            if (coreIndex < 0) {
                coreIndex = Random.Range(0, coreObj.Length);
            } else {
                coreIndex = (coreIndex + 1) % coreObj.Length;
                particlePlayFlag = true;
            }
            for (int i = 0; i < coreObj.Length; i++) {
                if (coreObj[i]) {
                    coreObj[i].SetActive(i == coreIndex);
                }
            }
            if (particlePlayFlag && coreParticles.Length > coreIndex && coreParticles[coreIndex]) {
                coreParticles[coreIndex].Play();
            }
        }
    }

    void MoveAttackJawStart() {
        if (state == State.Attack) {
            movingIndex = 0;
            movingSpeedMul = 0.8f;
            EmitEffect(effLeftFootMove);
            EmitEffect(effJumpSmall);
        }
    }

    void MoveAttackJawEnd() {
        if (state == State.Attack) {
            movingIndex = -1;
            EmitEffect(effPressSmall);
            CameraManager.Instance.SetQuake(quakePivot[0].position, 8, 4, 0, 0, 1f, 4f, dissipationDistance_Large);
        }
    }

    void MoveAttackJumpStart() {
        if (state == State.Attack) {
            lockonRotSpeed = 2f;
            movingIndex = 1;
            movingSpeedMul = 1f;
            EmitEffect(effLeftFootMove);
            EmitEffect(effRightFootMove);
            EmitEffect(effJumpBig);
        }
    }

    void MoveAttackJumpEnd() {
        if (state == State.Attack) {
            movingIndex = -1;
            EmitEffect(effPressBig);
            CameraManager.Instance.SetQuake(quakePivot[1].position, 12, 4, 0, 0, 1.5f, 4f, dissipationDistance_Large);
        }
    }

    void MoveAttackTail() {
        if (state == State.Attack) {
            EmitEffect(effSpin);
            if (targetTrans) {
                float sqrDist = GetTargetDistance(true, true, false);
                fbStepTime = 9f / 42f;
                fbStepMaxDist = 5f;
                fbStepEaseType = EasingType.SineInOut;
                if (sqrDist < 3f * 3f) {
                    SeparateFromTarget(3f);
                } else if (sqrDist > 4f * 4f) {
                    StepToTarget(4f);
                }
            }
        }
    }

    void MoveAttackLaser() {
        fbStepTime = 16f / 24f;
        fbStepMaxDist = 6f;
        fbStepEaseType = EasingType.SineInOut;
        SeparateFromTarget(6f);
    }

    void Meteor() {
        if (state == State.Attack) {
            EmitEffect(effMeteor);
            throwing.ThrowStart(0);
        }
    }

    public void KnockDownEvent() {
        if (state == State.Damage) {
            EmitEffect(effKnockDown);
            CameraManager.Instance.SetQuake(quakePivot[2].position, 8, 4, 0, 0, 1.5f, 4f, dissipationDistance_Large);
        }
    }

    public override int GetStealItemID() {
        int answer = base.GetStealItemID();
        if (answer == 29) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_Raccoon, true);
        }
        return answer;
    }

    void LaserCancel() {
        laserEnabled = false;
        laserOption.CancelLaser();
        raycaster.Deactivate();
        AttackEnd(atkLaserDummy);
        AttackEnd(atkLaserBody);
    }

    void LaserReady() {
        laserEnabled = true;
        laserOption.LightFlickeringChargeStart();
        raycaster.hitEffectEnabled = false;
        raycaster.Activate();
        AttackStart(atkLaserDummy);
    }

    void LaserStart() {
        laserEnabled = true;
        laserOption.LightFlickeringChargeEnd();
        raycaster.hitEffectEnabled = true;
        raycaster.Activate();
        AttackEnd(atkLaserDummy);
        AttackStart(atkLaserBody);
        if (state == State.Attack && level >= 3) {
            throwRushTimeRemain = 2f;
            throwRushIntervalRemain = 0f;
            rushCount = 0;
        }
    }

    void LaserEnd() {
        laserEnabled = false;
        laserOption.LightFlickeringBlastEnd();
        raycaster.Deactivate();
        AttackEnd(atkLaserBody);
        throwRushTimeRemain = 0f;
    }

    void TargetChaserActivate(int flag = 1) {
        targetChaserEnabled = (flag != 0 && level >= 4);
    }

    void FootSound(int index) {
        int audioIndex = Random.Range(0, 3) + index * 3;
        if (audioIndex >= 0 && audioIndex < footAudioSource.Length && footAudioSource[audioIndex]) {
            footAudioSource[audioIndex].Play();
        }
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        Vector3 centerPos = GetCenterPosition();
        float sqrDist = (new Vector3(centerPos.x, position.y, centerPos.z) - position).sqrMagnitude;
        int direction = 2;
        float angle = Vector3.Cross(trans.TransformDirection(vecForward), position - centerPos).y;
        if (angle >= -0.25f && angle <= 0.25f) {
            direction = 2;
        } else if (angle < -0.25f && angle > -0.75f) {
            direction = -1;
        } else if (angle > 0.25f && angle < 0.75f) {
            direction = 1;
        } else {
            direction = 0;
        }
        anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.StepDirection], direction);
        if (state == State.Damage && isDamageHeavy) {
            knockAmount = 0;
        }
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        if (changeCoreReservedTimeRemain <= 0f && colorType == damageColor_Critical && coreHPSave - nowHP >= GetMaxHP() / coreChangeDivide[Mathf.Clamp(level, 0, coreChangeDivide.Length)]) {
            changeCoreReservedTimeRemain = 0.2f;
        }
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (target) {
            targetFound = true;
        }
        if (targetFound) { 
            laserAttackedTimeRemain -= deltaTimeCache;
            meteorAttackedTimeRemain -= deltaTimeCache;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (state != State.Attack && laserEnabled) {
            LaserCancel();
        }
        if (state == State.Attack && laserEnabled) {
            if (backChecker && backChecker.gameObject.activeSelf && backChecker.stayFlag && backChecker.stayObj == target) {
                lockonRotSpeed = 0f;
            } else {
                lockonRotSpeed = laserLockonSpeedArray[Mathf.Clamp(level, 0, laserLockonSpeedArray.Length - 1)];
            }
        }
        if (laserAttackedTimeRemain <= -10f) {
            actDistNum = 1;
        } else {
            actDistNum = 0;
        }
        if (state == State.Attack && throwRushTimeRemain > 0f) {
            throwRushTimeRemain -= deltaTimeMove;
            throwRushIntervalRemain -= deltaTimeMove;
            if (throwRushIntervalRemain <= 0f) {
                throwRushIntervalRemain += 0.12f;
                throwing.ThrowStart(1);
                rushCount++;
            }
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
        if (changeCoreReservedTimeRemain > 0f) {
            changeCoreReservedTimeRemain -= deltaTimeCache;
            if (changeCoreReservedTimeRemain <= 0f) {
                ChangeCore();
            }
        }
    }

    int GetAttackType() {
        int attackTemp = Random.Range(0, level >= 2 ? 5 : 4);
        if (attackTemp == attackSave || (attackTemp == 3 && laserAttackedTimeRemain > 0f) || (attackTemp == 4 && meteorAttackedTimeRemain > 0f)) {
            attackTemp = Random.Range(0, 3);
        }
        if (laserAttackedTimeRemain <= -10f && (GetTargetDistance(true) >= 10f || Random.Range(0, 100) < 50)) {
            attackTemp = 3;
        } else if (level >= 2 && meteorAttackedTimeRemain <= -10f && Random.Range(0, 100) < 50) {
            attackTemp = 4;
        }
        attackSave = attackTemp;
        return attackTemp;
    }

    void ThrowStartFirePole() {
        if (level >= 4 && 2 + firePoleCount < throwing.throwSettings.Length) {
            throwing.ThrowStart(2 + firePoleCount);
            firePoleCount++;
        }
    }

    protected override void Attack() {
        base.Attack();
        movingIndex = -1;
        throwRushTimeRemain = 0f;
        firePoleCount = 0;
        float baseInterval = 0.4f;
        float maxHPTemp = GetMaxHP();
        if (maxHPTemp >= 1f) {
            if (nowHP <= maxHPTemp * 0.333334f) {
                baseInterval = 0f;
            } else {
                baseInterval += Mathf.Clamp01(nowHP / maxHPTemp) * intervalPlusArray[Mathf.Clamp(level, 0, intervalPlusArray.Length - 1)];
            }
        }
        int attackTemp = GetAttackType();
        float intervalTemp = GetAttackInterval(baseInterval);
        float addStiff = (IsSuperLevel ? 0f : 0.3f);
        if (attackTemp == 0) {
            AttackBase(0, 1f, 1.7f, 0, 90f / 36f + Mathf.Min(intervalTemp, addStiff), 90f / 36f + intervalTemp, 0f);
        } else if (attackTemp == 1) {
            intervalTemp = GetAttackInterval(baseInterval + 0.3f);
            AttackBase(1, 1.2f, 2.2f, 0, 62f / 36f + Mathf.Min(intervalTemp, addStiff + 0.2f), 62f / 36f + intervalTemp, 0f);
        } else if (attackTemp == 2) {
            AttackBase(2, 1f, 1.1f, 0, 154f / 42f + Mathf.Min(intervalTemp, addStiff), 154f / 42f + intervalTemp, 0.5f, 1f, true, 4f);
        } else if (attackTemp == 3) {
            AttackBase(3, 1f, 50f, 0, 270f / 60f + Mathf.Min(intervalTemp, addStiff), 270f / 60f + intervalTemp, 0f, 1f, true, laserLockonSpeedArray[Mathf.Clamp(level, 0, laserLockonSpeedArray.Length - 1)]);
            laserAttackedTimeRemain = 16f;
            MoveAttackLaser();
            LaserReady();
        } else if (attackTemp == 4) {
            AttackBase(4, 1.4f, 4f, 0, 101f / 30f + Mathf.Min(intervalTemp, addStiff), 101f / 30f + intervalTemp, 0.5f, 1f, false);
            meteorAttackedTimeRemain = 20f;
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (movingIndex >= 0 && movingIndex < movePivot.Length && movePivot[movingIndex] != null) {
            ApproachTransformPivot(movePivot[movingIndex], GetMinmiSpeed() * movingSpeedMul, 0.5f);
        }
    }

}
