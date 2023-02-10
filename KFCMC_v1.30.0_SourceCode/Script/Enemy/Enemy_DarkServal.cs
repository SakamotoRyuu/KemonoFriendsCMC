using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class Enemy_DarkServal : EnemyBaseBoss {
    
    public GameObject cellienCore;
    public GameObject coreDetection;
    public GameObject defaultDetection;
    public EnemyDeath weakEffect;
    public XWeaponTrail jumpTrail;
    public DamageDetection criticalDD;
    public GameObject defCoreParticleObj;
    public GameObject weakCoreParticleObj;
    public GameObject dodgeDetection;
    public Transform boltOrigin;
    public Transform boltThrowFrom;
    public int cruelKnockEnduranceLight = 10000;
    public int exLevel = 0;
    public bool isReaper = false;
    public bool isSpecial = false;
    public Transform deadEventCamPivot;
    public GameObject prefabOnDestroy;
    public GameObject deadEffectForMulti;

    int attackIndex;
    int attackIndexSave = -1;
    bool isAnimEnd = false;
    CapsuleCollider enemyCapCol;
    bool isCombo = false;
    bool healEffectEmitted = false;
    float jumpAttackPosSave;
    bool isParticleChanged = false;
    int dodgeDamageRemain;
    bool heavyKnocked = false;
    int jumpAttackType = 0;
    DynamicBone[] dynamicBones;
    bool dynamicBoneEnabled = true;
    bool attackParticlePlaying = false;
    float obstacledTime;
    float forceJumpTime;
    LayerMask invisibleWallLayerMask;
    float stepDistBase = 2.4f;
    bool spinMoving;
    float spinMovingTime;
    int specialSpawnProgress;
    float specialSpawnTime;
    float despairTime;
    bool despairActivated;
    bool isWalking;
    float judgementTimeElapsed = 20f;
    float judgementReadyingTimeRemain;
    float cannotDodgeTimeRemain;

    static readonly int[] dodgeDivideArray = new int[] { 12, 12, 18, 24, 30 };
    const int effectPile = 0;
    const int effectSpin = 1;
    const int effectHeal = 2;
    const int effectJump = 3;
    const int effectEscape = 4;
    const int effectDodge = 5;
    const int effectQuarter = 6;
    const int effectWave = 7;
    const int effectBolt = 8;
    const int effectSpinV2 = 9;
    const int effectSpawn = 10;
    const int effectSlash = 11;
    const int effectPileSuper = 12;
    const int effectSpinSuper = 13;
    const int effectDeadSave = 14;

    const int screwDet = 2;

    const int waveThrowIndex = 0;
    const int spinThrowIndex = 1;
    const int boltThrowIndex = 2;
    const int waveSuperThrowIndex = 3;
    const int spinSuperThrowIndex = 4;
    const int boltSuperThrowIndex = 5;
    const int volcanoThrowIndex = 6;
    const int slashRightThrowIndex = 7;
    const int slashLeftThrowIndex = 8;

    const int dropKeyStageNumber = 2;
    const int dropKeyID = 224;
    const int dropKeyID_EX = 338;
    const float obstacledCondition = 8f;
    const float forceJumpCondition = 6f;

    protected override void Awake() {
        base.Awake();
        if (isSpecial) {
            spawnStiffTime = 120f / 30f;
            deadTimer = 105f / 30f;
            destroyOnDead = true;
        } else {
            deadTimer = 0;
            destroyOnDead = false;
        }
        isAnimParamDetail = true;
        CoreHide();
        isCoreShowed = false;
        healEffectEmitted = false;
        coreTimeRemain = 0f;
        coreTimeMax = 18f;
        retargetingConditionTime = 4f;
        retargetingDecayMultiplier = 1f;
        dropItemVelocity = 1f;
        dropItemBalloonDelay = 5f;
        dropItemGetDelay = 5f;
        spawnStiffTime = 0.05f;
        dodgeDistance = 5f;
        dodgeMutekiTime = 0.25f;
        attackingDodgeEnabled = true;
        roveInterval = 8f;
        angryFixTime = 2f;
        enableContinuousLightKnock = true;
        if (isReaper) {
            changeMusicEnabled = false;
            attractionTime = 0.4f;
            confuseTime = 0.4f;
        } else {
            attractionTime = 1.5f;
            confuseTime = 1.5f;
        }
        if (exLevel <= 1) {
            stepDistBase = 2.4f;
            coreHideDenomi = 5f;
            quickAttackReduceRate = 0f;
            quickAttackRadius = -1f;
        } else if (exLevel == 2) {
            stepDistBase = 3.2f;
            coreHideDenomi = 5.5f;
            quickAttackReduceRate = 0.5f;
            quickAttackRadius = 1f;
        } else {
            stepDistBase = 4f;
            coreHideDenomi = 6f;
            quickAttackReduceRate = 1f;
            quickAttackRadius = 1f;
        }
        if (isReaper || sandstarRawEnabled) {
            quickAttackRadius = 1f;
            quickAttackReduceRate = 1f;
        }
        if (exLevel <= 2) {
            criticalDD.damageRate = 4f;
            agentActionDistance[0].attack.y = 14f;
        } else {
            criticalDD.damageRate = 3f;
            agentActionDistance[0].attack.y = 20f;
        }
        stealedMax = 4;
        sandstarRawKnockEndurance = 1000000;
        sandstarRawKnockEnduranceLight = 4000 + (exLevel - 1) * 500;
        supermanEffectNumber = (int)EffectDatabase.id.enemyYK_Man;
        supermanAuraNumber = (int)EffectDatabase.id.enemyYK_Aura;
        if (isReaper) {
            SetSupermanEffect();
            SupermanSetMaterial(true);
        }
        dynamicBones = GetComponents<DynamicBone>();
        invisibleWallLayerMask = LayerMask.GetMask("InvisibleWall");
        if (isSpecial) {
            bgmReplayOnEnd = false;
            winActionEnabled = false;
            ActivateSearches(false);
        }
    }

    protected override void Start() {
        base.Start();
        if (dodgeDetection && (isReaper || CharacterManager.Instance.GetFriendsCount() == 0)) {
            dodgeDetection.SetActive(false);
        }
    }

    void ActivateSearches(bool flag) {
        for (int i = 0; i < searchTarget.Length; i++) {
            searchTarget[i].SetActive(flag);
        }
        for (int i = 0; i < searchArea.Length; i++) {
            searchArea[i].enabled = flag;
            }
    }

    private void CoreHide() {
        coreDetection.SetActive(false);
        cellienCore.SetActive(false);
        defaultDetection.SetActive(true);
        isCoreShowed = false;
        ResetKnockRemain();
    }

    private void CoreShow() {
        cellienCore.SetActive(true);
        coreDetection.SetActive(true);
        defaultDetection.SetActive(false);
        isCoreShowed = true;
        coreTimeRemain = coreTimeMax;
        coreShowHP = nowHP;
        coreHideConditionDamage = GetCoreHideConditionDamage();
        healEffectEmitted = false;
        heavyKnocked = false;
        BootDeathEffect(weakEffect);
    }

    public override int GetStealItemID() {
        if (stealedCount < stealedMax) {
            stealedCount++;
            return (stealedCount == stealedMax ? 57 : 53);
        }
        return healStarID;
    }

    public override void SetSandstarRaw() {
        if (!isReaper) {
            base.SetSandstarRaw();
            if (state != State.Dead) {
                cruelKnockEnduranceLight = 40000 + (exLevel - 1) * 5000;
                coreHideDenomi = (exLevel <= 2 ? 8f : 12f);
                if (weakProgress >= 2) {
                    weakProgress = 0;
                    defCoreParticleObj.SetActive(true);
                    weakCoreParticleObj.SetActive(false);
                    isParticleChanged = false;
                }
                if (isCoreShowed) {
                    CoreHide();
                }
            }
        }
        quickAttackRadius = 1f;
        quickAttackReduceRate = 1f;
    }

    protected override void BattleStart() {
        base.BattleStart();
        searchArea[0].seeThroughDistance = 1000;
        searchArea[0].dontForgetDistance = 1000;
    }

    protected override void BattleEnd() {
        base.BattleEnd();
        if (isLastOne) {
            if (isReaper && StageManager.Instance) {
                StageManager.Instance.DefeatReaper();
            }
        } else if (deadEffectForMulti) {
            ed.deadEffect.prefab = deadEffectForMulti;
        }
    }

    public override float GetLightKnockEndurance() {
        return (weakProgress >= 2 ? cruelKnockEnduranceLight : knockEnduranceLight);
    }

    protected override void SetLevelModifier() {
        if (!isReaper && StageManager.Instance.stageNumber == dropKeyStageNumber) {
            dropItem[0] = dropKeyID;
            SetDropRate(10000);
        }
        if (isReaper) {
            dropItem[0] = dropKeyID_EX;
            SetDropRate(10000);
        }
        dodgeDamageRemain = GetDodgeDamageRemainMax();
        if (dodgeDetection) {
            dodgeDetection.SetActive(false);
        }
    }

    void CheckDynamicBone() {
        if (dynamicBones.Length > 0) {
            bool flag = (GameManager.Instance.save.config[GameManager.Save.configID_DynamicBone] >= 2);
            if (dynamicBoneEnabled != flag) {
                dynamicBoneEnabled = flag;
                for (int i = 0; i < dynamicBones.Length; i++) {
                    dynamicBones[i].enabled = dynamicBoneEnabled;
                }
            }
        }
    }

    void SetEventCameraStart() {
        if (CameraManager.Instance) {
            CameraManager.Instance.SetEventCamera(trans.position + trans.TransformDirection(vecForward) * 3f + vecUp, Quaternion.LookRotation(trans.TransformDirection(vecBack)).eulerAngles, 120f / 30f, 0.6f, 3f);
        }
    }

    void SetEventCameraDead() {
        if (CharacterManager.Instance.pCon) {
            CharacterManager.Instance.pCon.SetEventMutekiTime(5f);
        }
        if (CameraManager.Instance) {
            if (deadEventCamPivot) {
                CameraManager.Instance.SetEventCameraFollowTarget(deadEventCamPivot, 180f / 30f, 0.75f, 1.8f);
            } else {
                CameraManager.Instance.SetEventCamera(trans.position + trans.TransformDirection(vecForward) * 1.8f + vecUp * 0.8f, Quaternion.LookRotation(trans.TransformDirection(vecBack)).eulerAngles, 180f / 30f, 0.75f, 1.8f);
            }
        }
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (judgementTimeElapsed < 20f) {
            judgementTimeElapsed += deltaTimeCache * CharacterManager.Instance.riskyIncrease * (GameManager.Instance.minmiPurple ? 1.5f : 1f);
        }
        if (judgementReadyingTimeRemain > 0f) {
            judgementReadyingTimeRemain -= deltaTimeCache;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        knockRemain = knockEndurance;
        if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled) {
            weakProgress = (nowHP <= GetMaxHP() / 4 ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 0);
        } else {
            weakProgress = (nowHP <= GetMaxHP() / 4 ? 2 : 0);
        }
        if (isSpecial) {
            if (specialSpawnProgress == 0) {
                specialSpawnProgress = 1;
                specialSpawnTime = 0f;
                mutekiTimeRemain = 120f / 30f;
                disableControlTimeRemain = 120f / 30f;
                if (!isForAmusement) {
                    Input.ResetInputAxes();
                    BGM.Instance.Stop();
                    Ambient.Instance.Stop();
                    CharacterManager.Instance.StopFriends();
                    CharacterManager.Instance.SetPlayerUpdateEnabled(false);
                    if (PauseController.Instance) {
                        PauseController.Instance.pauseEnabled = false;
                    }
                }
                anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleType], 0);
                anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleMotion]);
                if (!isForAmusement) {
                    SetEventCameraStart();
                }
            } else if (specialSpawnProgress == 1) {
                specialSpawnTime += deltaTimeCache;
                if (specialSpawnTime >= 120f / 30f) {
                    specialSpawnProgress = 2;
                    mutekiTimeRemain = 0f;
                    disableControlTimeRemain = 0f;
                    ActivateSearches(true);
                    if (!isForAmusement) {
                        CharacterManager.Instance.SetPlayerUpdateEnabled(true);
                        if (PauseController.Instance) {
                            PauseController.Instance.pauseEnabled = true;
                        }
                    }
                    anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.IdleType], -1);
                    BattleStart();
                }
            }
        }
        if (!battleStarted && target) {
            BattleStart();
        }
        if (!isReaper) {
            if (isCoreShowed && state != State.Dead) { 
                if (weakProgress <= 1) {
                        coreTimeRemain -= deltaTimeCache * (coreTimeRemain >= 1f ? CharacterManager.Instance.riskyIncSqrt : 1f);
                        if (!healEffectEmitted && coreTimeRemain < 1) {
                            EmitEffectString("Heal");
                            healEffectEmitted = true;
                        }
                        if (coreTimeRemain < 0) {
                            CoreHide();
                        }
                } else {
                    knockRemain = knockEndurance;
                }
            }
            if (weakProgress == 2 && !isParticleChanged) {
                defCoreParticleObj.SetActive(false);
                weakCoreParticleObj.SetActive(true);
                EmitEffect(effectQuarter);
                isParticleChanged = true;
                if (!isCoreShowed) {
                    CoreShow();
                }
            }
        } else {
            knockRemainLight = GetLightKnockEndurance();
            knockRemain = GetHeavyKnockEndurance();
        }
        if (state != State.Attack) {
            gravityMultiplier = 1;
            if (attackParticlePlaying) {
                ParticleStopAll();
            }
            if (spinMoving) {
                spinMoving = false;
            }
            if (isWalking && targetTrans) {
                float sqrDist = GetTargetDistance(true, true, false);
                if (sqrDist >= 15f * 15f) {
                    isWalking = false;
                }
            }
        }
        if (state == State.Jump) {
            if (jumpTrail) {
                jumpTrail.Activate();
            }
        } else {
            if (jumpTrail) {
                jumpTrail.StopSmoothly(0.1f);
            }
        }
        if (attackDetection.Length > screwDet && attackDetection[screwDet] && state != State.Jump && attackDetection[screwDet].attackEnabled) {
            AttackEnd(screwDet);
        }
        if ((state == State.Wait || state == State.Chase) && CharacterManager.Instance.playerSearchTarget) {
            Vector3 selfPivot = trans.position;
            Vector3 targetPivot = CharacterManager.Instance.playerSearchTarget.position;
            //ForceJump
            if (targetPivot.y >= selfPivot.y + 4f && nowSpeed < 1f) {
                forceJumpTime += deltaTimeMove;
            } else {
                forceJumpTime -= deltaTimeMove * 2f;
            }
            forceJumpTime = Mathf.Clamp(forceJumpTime, 0f, forceJumpCondition + 1f);
            //Obstacle
            if (nowHP < GetMaxHP()) {
                selfPivot.y += 1f;
                targetPivot.y = selfPivot.y;
                if (Physics.Linecast(selfPivot, targetPivot, invisibleWallLayerMask)) {
                    obstacledTime += deltaTimeMove;
                } else {
                    obstacledTime -= deltaTimeMove * 2f;
                }
                obstacledTime = Mathf.Clamp(obstacledTime, 0f, obstacledCondition + 1f);
            }
            actDistNum = (obstacledTime >= obstacledCondition || forceJumpTime >= forceJumpCondition) ? 1 : 0;
        }
        if (!despairActivated && !isForAmusement) {
            if (!CharacterManager.Instance.GetPlayerLive() && CharacterManager.Instance.GetFriendsCount(true) <= 0) {
                despairTime += deltaTimeCache;
                if (despairTime >= 0.5f && state == State.Wait && groundedFlag) {
                    DespairAction();
                }
            } else {
                despairTime = 0f;
            }
        }
        if (cannotDodgeTimeRemain > 0f) {
            cannotDodgeTimeRemain -= deltaTimeMove;
        }
        CheckDynamicBone();
    }

    protected override void MoveControlChild_Gravity() {
        base.MoveControlChild_Gravity();
    }

    protected override void Update_MoveControl_ChildAgentSpeed() {
        if (exLevel >= 3 && isWalking) {
            float speedTemp = GetMaxSpeed(true);
            if (agent.speed != speedTemp) {
                agent.speed = speedTemp;
            }
        } else {
            base.Update_MoveControl_ChildAgentSpeed();
        }
    }

    protected override void Update_Process_Dead() {
        base.Update_Process_Dead();
        if (stateTime > 4) {
            Destroy(gameObject);
        }
    }

    protected override void Start_Process_Damage() {
        base.Start_Process_Damage();
        isCombo = false;
    }

    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        if (!isCoreShowed && state != State.Dead && !isReaper) {
            CoreShow();
        }
        if (cannotDodgeTimeRemain < 0.8f) {
            cannotDodgeTimeRemain = 0.8f;
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        heavyKnocked = true;
        if (isCoreShowed && coreTimeRemain > 1.25f) {
            coreTimeRemain = 1.25f;
        }
        if (cannotDodgeTimeRemain < 3f) {
            cannotDodgeTimeRemain = 3f;
        }
    }

    public override float GetKnocked() {
        return base.GetKnocked() * (!heavyKnocked && isCoreShowed && weakProgress <= 1 && nowHP <= GetCoreHideBorder() ? 0 : 1);
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (isReaper) {
            knockAmount = 0;
        }
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        int costSum = CharacterManager.Instance.costSumSave;
        if (dodgeDetection) {
            if (isReaper) {
                if (dodgeDetection.activeSelf) {
                    dodgeDetection.SetActive(false);
                }
            } else {
                if (!dodgeDetection.activeSelf && GameManager.Instance.save.difficulty >= 2 && costSum > 0) {
                    dodgeDamageRemain -= damage * (10 + costSum) * (weakProgress == 2 ? 2 : 1);
                    if (dodgeDamageRemain <= 0) {
                        dodgeDetection.SetActive(true);
                    }
                }
            }
        }
    }

    public override void EmitEffectString(string type) {
        switch (type) {
            case "Heal":
                EmitEffect(effectHeal);
                break;
            case "Escape":
                EmitEffect(effectEscape);
                break;
            case "Dodge":
                EmitEffect(effectDodge);
                break;
            case "Wave":
                EmitEffect(effectWave);
                break;
            case "Spawn":
                SetSupermanEffect();
                SupermanSetMaterial(true);
                SupermanSetObj(true);
                EmitEffect(effectSpawn);
                break;
            case "DeadSave":
                DeadEvent();
                EmitEffect(effectDeadSave);
                break;
        }
    }

    void DeadEvent() {
        if (isLastOne && !isItem) {
            specialMoveDuration = 0f;
            move = vecZero;
            agent.radius = 2.5f;
            if (!isForAmusement) {
                BGM.Instance.Stop();
                SetEventCameraDead();
                CharacterManager.Instance.ShowBossResult(enemyID, sandstarRawEnabled);
            }
        }
    }

    float GetAttackIntervalSpecial() {
        float interval;
        if (weakProgress >= 2 || (enemyCanvasLoaded && enemyCanvas.transform.GetChild((int)EnemyCanvasChild.ibisSong).gameObject.activeSelf)){
            interval = Random.Range(0.1f, 0.3f);
        } else {
            interval = (weakProgress == 1 ? Random.Range(1.5f, 1.7f) : Random.Range(1.8f, 2.0f));
        }
        return interval;
    }

    public override void MakeAngry(GameObject decoyObject, CharacterBase attacker = null) {
        base.MakeAngry(decoyObject, attacker);
        if (state == State.Attack) {
            if (attackedTimeRemain < attackStiffTime - stateTime + 0.3f) {
                attackedTimeRemain = attackStiffTime - stateTime + 0.3f;
            }
        } else {
            if (attackedTimeRemain < 0.3f) {
                attackedTimeRemain = 0.3f;
            }
        }
    }

    void AttackStep(float time, float maxDist, bool directionAdjust = false) {
        fbStepTime = time;
        fbStepMaxDist = maxDist;
        // ForwardStep(0);
        StepToTarget(0f);
        specialMoveDirectionAdjustEnabled = directionAdjust;
    }

    int GetDodgeDamageRemainMax() {
        return maxHP / dodgeDivideArray[sandstarRawEnabled ? 4 : exLevel] * 10;
    }

    void ScrewStart() {
        AttackStart(screwDet);
    }

    void ScrewEnd() {
        AttackEnd(screwDet);
    }

    protected override void Attack() {
        attackingDodgeEnabled = true;
        if (exLevel >= 3 && weakProgress <= 1) {
            if (targetTrans) {
                float sqrDist = GetTargetDistance(true, true, false);
                if (sqrDist >= 7f * 7f) {
                    isWalking = false;
                } else {
                    isWalking = !isWalking;
                }
            }
        } else {
            isWalking = false;
        }
        if (!targetTrans && CharacterManager.Instance.playerSearchTarget) {
            targetTrans = CharacterManager.Instance.playerSearchTarget;
            searchArea[0].SetLockTarget(targetTrans.gameObject, 2f);
        }
        if (targetTrans) {
            float distance = GetTargetDistance(false, true, false);
            float distHeight = targetTrans.position.y - trans.position.y;
            float intervalPlus = GetAttackIntervalSpecial();
            bool isEx = (exLevel >= 3);
            isAnimStopped = false;
            isAnimEnd = false;
            if (groundedFlag) {
                if (!isCombo) {
                    if (exLevel <= 1) {
                        if (attackIndexSave < 0) {
                            attackIndex = 2;
                        } else if (distance >= 5 && Random.Range(0, 100) < 75) {
                            attackIndex = 2;
                        } else if (((groundedFlag && !groundedFlag_ForAgent ) || distHeight >= 2) && Random.Range(0, 100) < 75) {
                            attackIndex = 1;
                        } else {
                            attackIndex = Random.Range(0, 4);
                            if (attackIndex == attackIndexSave) {
                                attackIndex = Random.Range(0, 4);
                            }
                        }
                    } else {
                        if (attackIndexSave < 0) {
                            attackIndex = 2;
                        } else if (distance >= 5 && Random.Range(0, 100) < 75) {
                            int randTemp = Random.Range(0, exLevel <= 2 ? 2 : 3);
                            if (randTemp == 0) {
                                attackIndex = 2;
                            } else if (randTemp == 1) {
                                attackIndex = 4;
                            } else {
                                attackIndex = 3;
                            }
                        } else if (distHeight >= 2 && Random.Range(0, 100) < 75) {
                            attackIndex = 1;
                        } else {
                            attackIndex = Random.Range(0, 5);
                            if (attackIndex == attackIndexSave) {
                                attackIndex = Random.Range(0, 5);
                            }
                        }
                    }
                    if (forceJumpTime >= forceJumpCondition) {
                        attackIndex = 1;
                        forceJumpTime = 0f;
                    } else if (obstacledTime >= obstacledCondition) {
                        attackIndex = 4;
                        obstacledTime = 0f;
                    }
                    attackIndexSave = attackIndex;
                }
                if (attackIndex == 1) {
                    Jump(9);
                    attackedTimeRemain = 0.6f;
                    judgementReadyingTimeRemain = 0.3f;
                    attackProcess = 0;
                } else if (attackIndex == 2) {
                    attackBiasValue = 0;
                    if (AttackBase(5, isEx ? 1.15f : 1.1f, 1.7f, 0, 1.2f, 1.2f + intervalPlus, 0)) {
                        ParticlePlay(0);
                        EmitEffect(effectPile);
                        if (exLevel >= 3) {
                            EmitEffect(effectPileSuper);
                        }
                        attackProcess = 0;
                    }
                } else if (attackIndex == 3) {
                    attackBiasValue = 0;
                    float spinSp = 3f / 4f;
                    float spinInterval = (exLevel <= 2 ? 28f : 58f) / 30f / spinSp;
                    if (AttackBase(6, 1.1f, 1.5f, 0, spinInterval, spinInterval + intervalPlus, 0.5f, spinSp)) {
                        ParticlePlay(0);
                        ParticlePlay(1);
                        AttackStep(0.4f, 4f);
                        attackProcess = 0;
                        if (exLevel <= 1) {
                            EmitEffect(effectSpin);
                        } else if (exLevel == 2) {
                            EmitEffect(effectSpinV2);
                        } else {
                            EmitEffect(effectSpinSuper);
                        }
                    }
                } else if (attackIndex == 4) {
                    float waveSp = 0.8f;
                    if (AttackBase(7, 1f, 1.2f, 0, 30f / 30f / waveSp, 30f / 30f / waveSp + intervalPlus, 0, waveSp)) {
                        ParticlePlay(0);
                        EmitEffectString("Wave");
                        attackProcess = 0;
                    }
                } else if (attackProcess == 0) {
                    if (AttackBase(0, 1, 1.1f, 0, isEx ? 0.4333f : 0.5f, isEx ? 0.4333f : 0.5f, 1, 0.9f)) {
                        ParticlePlay(0);
                        AttackStep(isEx ? 0.3333f : 0.4f, stepDistBase);
                        attackProcess = 1;
                        isCombo = true;
                    }
                } else if (attackProcess == 1) {
                    if (AttackBase(1, 1, 1.1f, 0, isEx ? 0.4333f : 0.5f, isEx ? 0.4333f : 0.5f, 1, 0.9f)) {
                        ParticlePlay(1);
                        AttackStep(isEx ? 0.3333f : 0.4f, stepDistBase);
                        attackProcess = 2;
                    }
                } else if (attackProcess == 2) {
                    if (AttackBase(2, 1, 1.1f, 0, isEx ? 0.5333f : 0.6f, isEx ? 0.5333f : 0.6f, 1, 0.8f)) {
                        ParticlePlay(0);
                        ParticlePlay(1);
                        AttackStep(isEx ? 0.3333f : 0.4f, stepDistBase);
                        attackProcess = 3;
                    }
                } else if (attackProcess == 3) {
                    if (AttackBase(3, isEx ? 1.15f : 1.1f, 2.4f, 0, 1.2f, 1.2f + intervalPlus, 1, 0.8f, true, attackLockonDefaultSpeed * 1.5f)) {
                        ParticlePlay(0);
                        ParticlePlay(1);
                        AttackStep(0.4f, stepDistBase + 2f, true);
                        attackProcess = 0;
                        isCombo = false;
                    }
                }
            } else {
                if (jumpAttackType == 8) {
                    if (AttackBase(8, isEx ? 1.3f : 1.25f, isEx ? 5f : 2.4f, 0, 5f, 2f, 0, 1, true, 5)) {
                        attackingDodgeEnabled = false;
                        ParticlePlay(0);
                        EmitEffect(effectBolt);
                        attackProcess = 0;
                    }
                } else {
                    if (AttackBase(4, 1.2f, 2.4f, 0, 5f, 2f, 0, 1, true, 5)) {
                        attackingDodgeEnabled = false;
                        ParticlePlay(0);
                        attackProcess = 0;
                    }
                }
                jumpAttackPosSave = trans.position.y;
            }
        }
    }

    protected override void Jump(float power) {
        if (exLevel >= 2) {
            attackPowerMultiplier = 1f;
            knockPowerMultiplier = 1f;
            anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], 2);
        } else {
            anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], 0);
        }
        base.Jump(power);
        EmitEffect(effectJump);
        if (exLevel >= 2 && (weakProgress >= 2 || Random.Range(0, 2) == 0 || exLevel >= 3)) {
            jumpAttackType = 8;
        } else {
            jumpAttackType = 4;
        }
    }

    void SpinMoveStart() {
        spinMovingTime = 0f;
        spinMoving = true;
    }

    void SpinMoveEnd() {
        spinMovingTime = 0f;
        spinMoving = false;
    }

    protected override void Update_Transition_Jump() {
        base.Update_Transition_Jump();
        if (targetTrans && attackedTimeRemain < 0f && judgementReadyingTimeRemain <= 0f) {
            float sqrDist = GetTargetDistance(true, true, false);
            float conditionDist = (jumpAttackType == 8 ? 1.25f : 0.75f);
            if (state != State.Attack && sqrDist < MyMath.Square(conditionDist)) {
                SetState(State.Attack);
            }
        }
    }

    float GetAirMaxSpeed() {
        return maxSpeed * GetSickSpeedRate() * Mathf.Clamp(GameManager.Instance.minmiPurple ? 3f : CharacterManager.Instance.riskyIncSqrt, 1f, 1.5f);
    }

    protected override void Update_Process_Jump() {
        base.Update_Process_Jump();
        if (targetTrans) {
            lockonRotSpeed = attackLockonDefaultSpeed * 0.5f;
            if (GetTargetDistance(true, true, false) > 0.1f * 0.1f) {
                CommonLockon();
                cCon.Move(GetTargetVector(true, true) * GetAirMaxSpeed() * (stateTime < 0.5f ? stateTime * 2f : 1f) * deltaTimeMove);
            }
        }
    }

    public override void SideStep(int direction, float distance = 5, float mutekiTime = 0f, bool changeState = true) {
        base.SideStep(direction, distance, mutekiTime, changeState);
        attackedTimeRemain = 0;
        if (dodgeDetection) {
            dodgeDetection.SetActive(false);
        }
        dodgeDamageRemain = GetDodgeDamageRemainMax();
        EmitEffectString("Dodge");
    }

    void ThrowSlash() {
        if (exLevel >= 3) {
            EmitEffect(effectSlash);
            throwing.ThrowStart(slashRightThrowIndex);
            throwing.ThrowStart(slashLeftThrowIndex);
        }
    }

    void ThrowWave() {
        if (exLevel <= 2) {
            throwing.ThrowStart(waveThrowIndex);
        } else {
            throwing.ThrowStart(waveSuperThrowIndex);
        }
    }

    void ThrowSpin() {
        if (exLevel == 2) {
            throwing.ThrowReady(spinThrowIndex);
        } else if (exLevel >= 3) {
            throwing.ThrowReady(spinSuperThrowIndex);
        }
    }

    void LightningBolt() {
        SetTransformPositionToGround(boltOrigin, boltThrowFrom, 0.5f);
        if (exLevel <= 2) {
            throwing.ThrowStart(boltThrowIndex);
        } else {
            throwing.ThrowStart(boltSuperThrowIndex);
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

    void ChangeFaceCry() {
        fCon.SetFace("Cry", 0.2f);
    }

    void DespairAction() {
        despairActivated = true;
        if (state != State.Dead) {
            ResetTriggerOnDamage();
            disableControlTimeRemain = 150f / 30f;
            mutekiTimeRemain = 150f / 30f;
            ChangeFaceCry();
            anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Smile]);
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        switch (attackType) {
            case 4:
            case 8:
                gravityMultiplier = 3f;
                if (groundedFlag || stateTime > 3f || (stateTime > 0.6f && Time.timeScale > 0f && trans.position.y >= jumpAttackPosSave)) {
                    if (attackType == 8) {
                        LightningBolt();
                    }
                    float plusTime = (attackType == 4 ? 4f / 30f : 16f / 30f);
                    attackType = -8;
                    attackProcess = 1;
                    attackStiffTime = stateTime + plusTime + 0.15f;
                    attackedTimeRemain = plusTime + GetAttackIntervalSpecial();
                    nowSpeed = 0;
                    gravityMultiplier = 1;
                    attackingDodgeEnabled = true;
                } else {
                    attackStiffTime = stateTime + 1.0f;
                    if (attackedTimeRemain < 1.5f) {
                        attackedTimeRemain = 1.5f;
                    }
                }
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], attackType != 4 || !isAnimStopped ? 1 : 0);
                if (Time.timeScale > 0f) {
                    jumpAttackPosSave = trans.position.y;
                }
                break;
            case 5:
                float spRate = isSuperman ? 4f / 3f : 1;
                bool isEx = (exLevel >= 3);
                float maxSpTemp = GetMaxSpeed(false, false, false, true);
                float pileMaxSpeed = maxSpTemp > 0 ? maxSpTemp + (isEx ? 10f : 6f) : 0f;
                if (!isAnimEnd) {
                    if (stateTime > 1.0f) {
                        isAnimEnd = true;
                    } else if (targetTrans) {
                        if ((targetTrans.position - (trans.position + vecUp)).sqrMagnitude < MyMath.Square(targetRadius + 0.6f)) {
                            isAnimEnd = true;
                        }
                    }
                }
                if (!isAnimEnd) {
                    attackStiffTime = stateTime + 0.6f;
                    cCon.Move(trans.TransformDirection(vecForward) * pileMaxSpeed * (stateTime < 0.3f ? stateTime / 0.3f : 1) * deltaTimeMove);
                } else {
                    agent.velocity = vecZero;
                }
                if (!isAnimStopped) {
                    lockonRotSpeed = 10f + (exLevel - 1);
                } else if (!isAnimEnd) {
                    lockonRotSpeed = 10f + (exLevel - 1) - Mathf.Max(0f, nowSpeed / pileMaxSpeed * 8f);
                } else {
                    lockonRotSpeed = 0f;
                }
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], isAnimEnd || !isAnimStopped ? spRate : 0f);
                break;
            case 6:
                if (spinMoving) {
                    spinMovingTime += deltaTimeCache;
                    lockonRotSpeed = 5f;
                    cCon.Move(trans.TransformDirection(vecForward) * GetMaxSpeed(false, false, false, true) * 0.9f * Mathf.Clamp01(spinMovingTime * 3f) * deltaTimeMove);
                }
                break;
        }
    }

    protected override void ParticlePlay(int index) {
        base.ParticlePlay(index);
        attackParticlePlaying = true;
    }

    protected override void ParticleStopAll() {
        base.ParticleStopAll();
        attackParticlePlaying = false;
    }

    protected override void DeadProcess() {
        if (isLastOne) {
            if (isSpecial && prefabOnDestroy && StageManager.Instance.dungeonController) {
                Instantiate(prefabOnDestroy, trans.position, trans.rotation, StageManager.Instance.dungeonController.transform);
                if (exLevel == 3 && !isForAmusement && CharacterManager.Instance.GetFriendsExist(31, true)) {
                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_AnotherAndDark, true);
                }
                if (exLevel == 3 && !isForAmusement && sandstarRawEnabled && CharacterManager.Instance.bossResult.minmiPurpleFlag) {
                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_DarkServalPurple, true);
                }
            }
        } else {
            dropItem[0] = -1;
            SetDropRate(0);
        }
        if (isReaper && exLevel == 1) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_DefeatDS1, true);
        }
        if (isReaper && exLevel == 2) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_DefeatDS2, true);
        }
        base.DeadProcess();
    }

    public override bool GetCanDodge() {
        if (cannotDodgeTimeRemain > 0f || state == State.Jump || (state == State.Attack && attackingDodgeEnabled == false)) {
            return false;
        }
        return base.GetCanDodge();
    }

    public void CheckTrophy_Pile() {
        if (state == State.Attack && attackType == 5) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_DarkServalPile, true);
        }
    }

}
