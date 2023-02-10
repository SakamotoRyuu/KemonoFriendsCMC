using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_CeckyBeast : EnemyBaseBoss {

    public Transform quakePivot;
    public GameObject cellienCore;
    public GameObject coreDetection;
    public GameObject[] defaultDetection;
    public GameObject[] anotherDetection;
    public GameObject searchTargetNormal;
    public GameObject searchTargetCore;
    public Transform hideRayPivot;
    public bool isRaw;
    public GameObject additionalTrap;
    public Transform trapPivot;

    int attackSave = -1;
    int saveWeakProgress = -1;
    float attackSpeed = 1;
    int throwReadyIndex = 0;
    int throwStartIndex = 0;
    int throwMax = 1;
    bool healEffectEmitted = false;
    bool heavyKnocked = false;
    float throwRushTimeRemain = 0f;
    float throwRushIntervalRemain = 0f;
    int rushCount = 0;
    LayerMask hideLayerMask;
    float hideTime;
    float takeDamagedTimeRemain = 0f;
    bool angryAttackEnabled = false;
    float gatlingAttackedTimeRemain;
    bool breakdownFlag;
    bool t_AngryThrowed;

    const int coreShowEffectIndex = 14;
    const int coreHideEffectIndex = 15;
    const int dropKeyStageNumber = 7;
    const int dropKeyID = 326;
    const float angryHideTimeLimit = 4f;
    const float takeDamagedTimeMax = 4f;
    const int effectIndexBreakdown = 18;
    const int attackIndexDying = 8;
    static readonly int[] rushThrowIndex = new int[] { 0, 5, 5, 5, 5, 5, 5 };
    static readonly int[] rushThrowIndexSuper = new int[] { 6, 7, 7, 7, 7, 7, 7 };

    protected override void Awake() {
        base.Awake();
        deadTimer = 3;
        attackWaitingLockonRotSpeed = 1.2f;
        CoreHide();
        isCoreShowed = false;
        healEffectEmitted = false;
        coreTimeRemain = 0;
        dropItemVelocity = 1f;
        dropItemBalloonDelay = 2.5f;
        dropItemGetDelay = 2.5f;
        checkGroundPivotHeight = 1f;
        checkGroundTolerance = 2f;
        checkGroundTolerance_Jumping = 2f;
        hideTime = 0f;
        // coreHideDenomi = 6f;
        coreHideDenomi = 5.5f;
        coreTimeMax = 22f;
        hideLayerMask = LayerMask.GetMask("Field", "CameraObstacle", "SecondField");
        sandstarRawKnockEndurance = 1000000;
        sandstarRawKnockEnduranceLight = 5000;
        killByCriticalOnly = true;
        if (isRaw) {
            spawnStiffTime = 1f;
            attackedTimeRemainOnDamage = 0.1f;
        } else {
            spawnStiffTime = 0f;
            attackedTimeRemainOnDamage = 1.5f;
        }
    }

    private void CoreHide() {
        coreDetection.SetActive(false);
        cellienCore.SetActive(false);
        for (int i = 0; i < defaultDetection.Length; i++) {
            defaultDetection[i].SetActive(true);
        }
        for (int i = 0; i < anotherDetection.Length; i++) {
            anotherDetection[i].SetActive(false);
        }
        isCoreShowed = false;
        knockRemainLight = knockEnduranceLight;
    }

    private void CoreShow() {
        cellienCore.SetActive(true);
        coreDetection.SetActive(true);
        for (int i = 0; i < defaultDetection.Length; i++) {
            defaultDetection[i].SetActive(false);
        }
        for (int i = 0; i < anotherDetection.Length; i++) {
            anotherDetection[i].SetActive(true);
        }
        isCoreShowed = true;
        coreTimeRemain = coreTimeMax;
        coreShowHP = nowHP;
        coreHideConditionDamage = GetCoreHideConditionDamage();
        healEffectEmitted = false;
        heavyKnocked = false;
        EmitEffect(coreShowEffectIndex);
    }

    public override void SetSandstarRaw() {
        base.SetSandstarRaw();
        if (state != State.Dead) {
            coreHideDenomi = 8f;
        }
    }

    protected override void SetLevelModifier() {
        if (StageManager.Instance.stageNumber == dropKeyStageNumber) {
            dropItem[0] = dropKeyID;
            SetDropRate(10000);
        } else {
            SetDropRate(0);
        }
    }

    void ThrowRushStart() {
        throwRushTimeRemain = 1.38f;
        throwRushIntervalRemain = 0f;
        rushCount = 0;
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        knockRemain = knockEndurance;
        if (targetTrans) {
            if (Physics.Linecast(hideRayPivot.position, targetTrans.position, hideLayerMask, QueryTriggerInteraction.Ignore)) {
                hideTime += deltaTimeCache;
            } else {
                hideTime -= deltaTimeCache * 2f;
            }
            if (hideTime > angryHideTimeLimit + 2f) {
                hideTime = angryHideTimeLimit + 2f;
            } else if (hideTime < 0f) {
                hideTime = 0f;
            }
        } else {
            hideTime = 0f;
        }
        if (CharacterManager.Instance.specialHealReported > 0) {
            takeDamagedTimeRemain = takeDamagedTimeMax;
        }
        if (takeDamagedTimeRemain > 0f) {
            takeDamagedTimeRemain -= deltaTimeCache;
        }
        if (gatlingAttackedTimeRemain > 0f) {
            gatlingAttackedTimeRemain -= deltaTimeCache;
        }
        angryAttackEnabled = (hideTime >= angryHideTimeLimit && takeDamagedTimeRemain > 0f);
        if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled) {
            weakProgress = ((angryAttackEnabled || nowHP <= GetMaxHP() / 3) ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 1);
        } else {
            weakProgress = (angryAttackEnabled ? 2 : nowHP <= GetMaxHP() / 2 ? 1 : 0);
        }
        if (isRaw) {
            attractionTime = 4f - weakProgress;
        } else {
            attractionTime = 8f - weakProgress;
        }
        attackSpeed = (weakProgress == 2 ? 1.2f : 1);
        if (!sandstarRawEnabled) {
            if (isRaw) {
                maxSpeed = (weakProgress == 2 ? 10f : 9f);
            } else {
                maxSpeed = (weakProgress == 2 ? 9f : weakProgress == 1 ? 7.5f : 6f);
            }
        }
        if (saveWeakProgress != weakProgress) {
            saveWeakProgress = weakProgress;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AnimSpeed], maxSpeed / 9f);
        }
        if (isCoreShowed && state != State.Dead) {
            coreTimeRemain -= deltaTimeCache * (coreTimeRemain >= 1f && nowHP > 1 ? CharacterManager.Instance.riskyIncSqrt : 1f);
            if (!healEffectEmitted && coreTimeRemain < 1) {
                EmitEffect(coreHideEffectIndex);
                healEffectEmitted = true;
            }
            if (coreTimeRemain < 0) {
                CoreHide();
            }
        }
        if (state == State.Attack && throwRushTimeRemain > 0f) {
            throwRushTimeRemain -= deltaTimeMove;
            throwRushIntervalRemain -= deltaTimeMove;
            if (throwRushIntervalRemain <= 0f) {
                throwRushIntervalRemain += 0.12f;
                if (rushCount < 12) {
                    throwing.ThrowStart(angryAttackEnabled ? rushThrowIndexSuper[rushCount % rushThrowIndex.Length] : rushThrowIndex[rushCount % rushThrowIndex.Length]);
                    rushCount++;
                    if (angryAttackEnabled && !t_AngryThrowed) {
                        t_AngryThrowed = true;
                        TrophyManager.Instance.CheckTrophy(TrophyManager.t_CeckyBeast, true);
                    }
                }
            }
        }
        if (searchTargetNormal.activeSelf == isCoreShowed) {
            searchTargetNormal.SetActive(!isCoreShowed);
        }
        if (searchTargetCore.activeSelf != isCoreShowed) {
            searchTargetCore.SetActive(isCoreShowed);
        }
    }

    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        if (!isCoreShowed && state != State.Dead) {
            CoreShow();
        }
        if (isCoreShowed && nowHP == 1 && !isSuperarmor) {
            breakdownFlag = true;
            if (attackedTimeRemain > lightStiffTime) {
                attackedTimeRemain = lightStiffTime;
            }
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        heavyKnocked = true;
        if (isCoreShowed && coreTimeRemain > 1.25f) {
            coreTimeRemain = 1.25f;
        }
        if (isCoreShowed && nowHP == 1) {
            breakdownFlag = true;
            if (attackedTimeRemain > heavyStiffTime) {
                attackedTimeRemain = heavyStiffTime;
            }
        }
        throwRushTimeRemain = 0f;
    }

    public override float GetKnocked() {
        if (state == State.Attack && attackType == attackIndexDying) {
            return 20000f;
        }
        return base.GetKnocked() * (!heavyKnocked && isCoreShowed && nowHP <= GetCoreHideBorder() ? 0 : 1);
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        takeDamagedTimeRemain = takeDamagedTimeMax;
    }

    void MoveAttack0() {
        if (state == State.Attack) {
            SpecialStep(0f, 40f / 60f / attackSpeed, 20f, 0f, 0f, true, false, EasingType.SineOut);
        }
    }

    void MoveAttack1() {
        if (state == State.Attack) {
            fbStepTime = 30f / 60f / attackSpeed;
            fbStepMaxDist = 6f;
            fbStepEaseType = EasingType.SineInOut;
            ForwardOrBackStep(6f);
        }
    }

    void MoveAttack2_0() {
        if (state == State.Attack) {
            SpecialStep(0f, 33f / 60f / attackSpeed, 10f, 0f, 0f, true, false, EasingType.SineOut);
        }
    }

    void MoveAttack2_1() {
        if (state == State.Attack) {
            SpecialStep(0f, 26f / 60f / attackSpeed, 10f, 0f, 0f, true, false, EasingType.SineOut);
        }
    }

    void MoveAttack3() {
        if (state == State.Attack) {
            fbStepTime = 25f / 60f / attackSpeed;
            fbStepMaxDist = 4f;
            fbStepEaseType = EasingType.SineInOut;
            ForwardOrBackStep(5f);
        }
    }

    void MoveAttack4_0() {
        if (state == State.Attack) {
            fbStepTime = 20f / 60f / attackSpeed;
            fbStepMaxDist = 4f;
            fbStepEaseType = EasingType.SineInOut;
            BackStep(3f);
        }
    }

    void MoveAttack4_1() {
        if (state == State.Attack) {
            if (targetTrans) {
                fbStepTime = 100f / 60f / attackSpeed;
                fbStepMaxDist = 30f;
                fbStepEaseType = EasingType.SineInOut;
                ForwardStep(-20f);
            } else {
                SetSpecialMove(trans.TransformDirection(Vector3.forward), 20f, 100f / 60f / attackSpeed, EasingType.SineInOut);
            }
        }
    }

    void MoveAttack5() {
        if (state == State.Attack) {
            fbStepTime = 25f / 60f / attackSpeed;
            fbStepMaxDist = 4f;
            fbStepEaseType = EasingType.SineInOut;
            BackStep(8f);
        }
    }

    void QuakeAttack0() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePivot.position, 14, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
        }
    }

    void QuakeAttack2() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePivot.position, 8, 4, 0, 0, 0.5f, 4f, dissipationDistance_Boss);
            AddTrap();
        }
    }

    void QuakeKnockHeavy() {
        if (state == State.Damage) {
            CameraManager.Instance.SetQuake(quakePivot.position, 5, 4, 0, 0, 1.5f, 4f, dissipationDistance_Boss);
        }
    }

    void SpecialThrowReady() {
        if (throwReadyIndex < throwMax) {
            throwing.ThrowReady(throwReadyIndex);
            throwReadyIndex++;
        }
    }

    void SpecialThrowStart() {
        if (throwStartIndex < throwMax) {
            throwing.ThrowStart(throwStartIndex);
            throwStartIndex++;
        }
    }

    protected override void BattleStart() {
        base.BattleStart();
        actDistNum = 1;
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (actDistNum != 1) {
            BattleStart();
        }
    }

    protected override void Start_Process_Dead() {
        base.Start_Process_Dead();
        if (effect[effectIndexBreakdown].instance) {
            Destroy(effect[effectIndexBreakdown].instance);
        }
    }

    protected override void DeadProcess() {
        if (!isLastOne) {
            dropItem[0] = -1;
            SetDropRate(0);
        }
        base.DeadProcess();
    }

    void AddTrap() {
        if (isRaw && additionalTrap && trapPivot) {
            ray.direction = vecDown;
            ray.origin = trapPivot.position;
            if (Physics.Raycast(ray, out raycastHit, 3f, fieldLayerMask, QueryTriggerInteraction.Ignore)) {
                Vector3 groundNormal = raycastHit.normal;
                if (groundNormal.y > 0f && 60f > Vector3.Angle(groundNormal, vecUp)) {
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
    
    protected override void Attack() {
        resetAgentRadiusOnChangeState = true;
        throwRushTimeRemain = 0f;
        if (breakdownFlag) {
            AttackBase(attackIndexDying, 0, 0, 0, 3.25f, 3.5f, 0, 1, false);
            breakdownFlag = false;
            SuperarmorStart();
            return;
        }
        if (targetTrans) {
            float sqrDist = GetTargetDistance(true, true, false);
            int attackTemp = 0;
            if (!battleStarted) {
                BattleStart();
                attackTemp = 0;
            } else if (sqrDist > 20 * 20) {
                int temp = Random.Range(0, 3);
                if (temp == 2 && attackSave == 5) {
                    temp = Random.Range(0, 2);
                }
                if (temp == 0) {
                    attackTemp = 0;
                } else if (temp == 1) {
                    attackTemp = 4;
                } else if (temp == 2) {
                    attackTemp = 5;
                }
            } else {
                attackTemp = Random.Range(0, 6);
                if (attackTemp == attackSave) {
                    attackTemp = Random.Range(0, 5);
                }
            }
            if (angryAttackEnabled) {
                gatlingAttackedTimeRemain = 0f;
                attackTemp = 5;
            }
            attackSave = attackTemp;
            float intervalPlus = 0f;
            if (isRaw) {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.3f, 0.5f) : weakProgress == 1 ? Random.Range(0.4f, 0.6f) : Random.Range(0.5f, 0.7f));
            } else if (sandstarRawEnabled) {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.4f, 0.6f) : weakProgress == 1 ? Random.Range(0.6f, 0.9f) : Random.Range(0.9f, 1.2f));
            } else {
                intervalPlus = (weakProgress == 2 ? Random.Range(0.8f, 1.0f) : weakProgress == 1 ? Random.Range(1f, 1.35f) : Random.Range(1.3f, 1.8f));
            }
            if ((attackTemp == 1 || attackTemp == 3)) {
                intervalPlus *= 0.75f;
            }
            switch (attackTemp) {
                case 0:
                    AttackBase(0, 1.2f, 3.4f, 0, 110f / 60f / attackSpeed + 0.5f, 110f / 60f / attackSpeed + intervalPlus, 1, attackSpeed);
                    agent.radius = 0.1f;
                    break;
                case 1:
                    AttackBase(1, 1f, 1.3f, 0, 110f / 60f / attackSpeed, 110f / 60f / attackSpeed + intervalPlus, 1f, attackSpeed, true, 10f);
                    break;
                case 2:
                    AttackBase(weakProgress >= 2 ? 6 : 2, 1.1f, 3f, 0, 190f / 60f / attackSpeed, 190f / 60f / attackSpeed + intervalPlus, 1f, attackSpeed);
                    agent.radius = 0.1f;
                    break;
                case 3:
                    AttackBase(3, 1f, 1.4f, 0, 105f / 60f / attackSpeed, 105f / 60f / attackSpeed + intervalPlus, 1f, attackSpeed);
                    break;
                case 4:
                    AttackBase(4, 1.05f, 1.3f, 0, 170f / 60f / attackSpeed, 170f / 60f / attackSpeed + intervalPlus, 1f, attackSpeed);
                    break;
                case 5:
                    if (weakProgress <= 1 || gatlingAttackedTimeRemain > 0f) {
                        throwReadyIndex = 0;
                        throwStartIndex = 0;
                        throwMax = (nowHP <= GetMaxHP() * 5 / 9 ? 5 : nowHP <= GetMaxHP() * 7 / 9 ? 3 : 1);
                        AttackBase(5, 1f, 1.3f, 0, 95f / 60f, 95f / 60f + intervalPlus, 1f, 1f);
                    } else {
                        gatlingAttackedTimeRemain = 10f;
                        AttackBase(7, 1f, 3.4f, 0f, 165f / 60f, 165f / 60f + intervalPlus);
                    }
                    break;
            }
        }
    }

    void EmitEffectBreakdown() {
        if (state != State.Dead) {
            EmitEffect(effectIndexBreakdown);
        }
    }

}
