using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy_ImperatrixMundi : EnemyBaseBoss
{
    public EnemyDeath hatchingED;
    public GameObject shieldBody;
    public GameObject shieldEnemyPrefab;
    public GameObject[] scaffoldPrefab;
    public Transform[] shieldEnemyPivot;
    public Transform shieldEnemyParent;
    public GameObject[] criticalDD;
    public DamageDetection shieldDD;
    public Transform searchReferenceTrans;
    public LaserOption[] laserOption;
    public RaycastToAdjustCapsuleCollider[] raycaster;
    public Transform laserPivot;
    public Transform playerPosPivot;
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    public Transform volcanoPivot;
    public GameObject[] poisonNoticePrefab;
    public GameObject[] missilePrefab;
    public Transform[] eventCamPivot;
    public GameObject sunObj;
    public AudioSource sunSE;
    public GameObject brokenSunObj;
    public EnemyDeath sunBreakEffect;
    public AudioClip deadAudioClip;
    public ChangeMatSet deadMatSet;
    public ChangeMatSet burstMatSet;
    public ScrollTexture scrollTexture;
    public GameObject sunIndependentEnemyPrefab;
    public SearchTargetPriority criticalSearchPriority;
    public GameObject celestialStarPrefab;
    public float searchReferenceOffset;
    public Vector2 searchReferenceRange;
    public Collider attackerOnHeadChecker;
    public int[] burstDamage;

    private int eventProgress;
    private bool healEffectEmitted;
    private bool heavyKnocked;
    private GameObject[] scaffoldInstance = new GameObject[4];
    private GameObject[] shieldEnemyInstance = new GameObject[shieldEnemyMax];
    private Enemy_ImperatrixChild[] shieldEnemyBase = new Enemy_ImperatrixChild[shieldEnemyMax];
    private bool[] shieldEnemyLaserAttacking = new bool[shieldEnemyMax];
    private Vector3 searchReferencePos = new Vector3(0f, 4f, 0f);
    private bool laserEnabled;
    private bool laserIsShooting;
    private bool laserLookatEnabled;
    private float laserAttackedTimeRemain;
    private float meteorAttackedTimeRemain;
    private int attackSave = -1;
    private int voiceSave = -1;
    private float volcanoTimer = -1;
    private int volcanoCount = 0;
    private GameObject poisonNoticeInstance;
    private int poisonType = -1;
    private float poisonTimer = -1;
    private int missileReadyCount;
    private int missileStartCount;
    private float missileReadyTimer = -1;
    private float missileStartTimer = -1;
    private int missileType = -1;
    private float sunVolumeIncreaseTimer = -1;
    private float sunVolumeIncreaseMax = 4;
    private float sunVolumeMultiplier = 1f;
    private bool spawnRotationFinished;
    private GameObject sunIndependentEnemyInstance;
    private Enemy_ImperatrixSun sunIndependentBase;
    private float sunIndependentInterval;
    private bool sunAttackTalked;
    private float slowMotionTimeRemain;
    private bool sunSetFlag;
    private Event_GraphBuild graphBuild;
    private float buildBreakTimer;
    private float amusementBattleTimer = 6.25f;
    private int celestialEffectState;
    private GameObject[] celestialObjs = new GameObject[50];
    private int animHash_KnockType;
    private int knockTypeSave = -1;

    private const int searchTargetCritical = 0;
    private const int searchTargetNormal = 1;
    private const int shieldEnemyMax = 12;
    private const float volcanoTimerMax = 1.4f;
    private const int volcanoCountMax = 7;
    private const int missileMax = 9;
    private const int attackTypeLaser = 0;
    private const int attackTypeMeteor = 1;
    private const int attackTypeBomb = 2;
    private const int attackTypeVolcano = 3;
    private const int attackTypeIce = 4;
    private const int attackTypePoison = 5;
    private const int attackTypeMissile = 6;
    private const int attackTypeCelestial = 7;
    private const int attackTypeSunIndependent = 8;
    private const int attackTypeEvent = 9;
    private const int attackIndexLaserReady = 0;
    private const int attackIndexLaserBody = 2;
    private const int throwIndexMeteor = 0;
    private const int throwIndexBomb = 1;
    private const int throwIndexVolcano = 2;
    private const int throwIndexIce = 9;
    private const int throwIndexPoison = 10;
    private const int throwIndexMissile = 13;
    private const int effHatchingReady = 0;
    private const int effCoreHide = 1;
    private const int effCoreShow = 2;
    private const int effKnockLight = 3;
    private const int effKnockHeavy = 4;
    private const int effLaserCountDown = 5;
    private const int effLaserCountDown2 = 6;
    private const int effCommonReady = 7;
    private const int effPoisonStart = 8;
    private const int effDeadSave = 9;
    private const int effSun = 10;
    private const int effDestroyScaffold = 11;
    private const int effCelestialReady = 14;
    private const int effCelestialStart = 15;
    private const int voiceTypeSun = 8;
    private const int attackPowerFinal = 999;
    private const int sunIndependentHPHeal = 100000;
    private const int sunIndependentHPCondition = 200000;
    static readonly int[] attackLaserBody = new int[] { 2, 3 };
    static readonly int[] shieldEnemyMaxProgress = new int[] { 3, 6, 12 };
    static readonly Vector3 celestialStarOffset = new Vector3(0f, 12f, 0f);

    protected override void Awake() {
        base.Awake();
        spawnStiffTime = 30f;
        deadTimer = 3.5f;
        destroyOnDead = false;
        killByCriticalOnly = true;
        isImmortal = true;
        cannotDoubleKnockDown = true;
        actDistNum = 0;
        attackedTimeRemainOnDamage = 2.2f;
        attackLockonDefaultSpeed = 0f;
        attackWaitingLockonRotSpeed = 1.5f;
        attackedTimeRemainReduceOnAngry = 2f;
        checkGroundPivotHeight = 1f;
        checkGroundTolerance = 2f;
        checkGroundTolerance_Jumping = 2f;
        retargetingConditionTime = 0.001f;
        sandstarRawLockonSpeed = 0f;
        sandstarRawKnockEndurance = 1000000;
        sandstarRawKnockEnduranceLight = 30000;
        sandstarRawMaxSpeed = 0f;
        sandstarRawAcceleration = 0f;
        cannotDoubleKnockDown = true;
        catchupExpDisabled = true;
        isCoreShowed = false;
        healEffectEmitted = false;
        coreTimeRemain = 0f;
        coreTimeMax = 40f;
        coreHideDenomi = 8f;
        mapChipSize = 3;
        searchTarget[searchTargetCritical].SetActive(false);
        searchTarget[searchTargetNormal].SetActive(false);
        shieldDD.SetParentCharacterBase(this);
        shieldBody.GetComponent<MissingObjectToDestroy>().SetGameObject(gameObject);
        shieldBody.SetActive(false);
        shieldBody.transform.parent = null;
        supermanEffectNumber = (int)EffectDatabase.id.enemyYK_LastBoss;
        supermanAuraNumber = (int)EffectDatabase.id.enemyYK_AuraBigSuper;
        LaserCancel();
        if (sunObj && sunObj.activeSelf) {
            sunObj.SetActive(false);
        }
        Preload(hatchingED.deadEffect.prefab);
        Preload(effect[effCelestialReady].prefab);
        Preload(effect[effCelestialStart].prefab);
        animHash_KnockType = Animator.StringToHash("KnockType");
    }

    void Preload(GameObject prefab) {
        GameObject objTemp = Instantiate(prefab);
        AudioSource audioTemp = objTemp.GetComponent<AudioSource>();
        if (audioTemp) {
            audioTemp.volume = 0f;
        }
        Destroy(objTemp);
    }

    private void Hatching() {
        if (Event_LastBattleSecond.Instance) {
            Event_LastBattleSecond.Instance.ImperatrixHatching(this);
        }
        if (effect[effHatchingReady].instance) {
            Destroy(effect[effHatchingReady].instance);
        }        
        if (blackMinmiIDRank == 0) {
            BootDeathEffect(hatchingED);
        } else {
            shieldEnemyParent.localEulerAngles = new Vector3(0f, blackMinmiIDRank * 90f, 0f);
        }
    }

    private void BGMStart() {
        if (!isForAmusement) {
            BGM.Instance.Play(bgmNumberBattle);
            Ambient.Instance.Play(-1);
            changeMusicEnabled = false;
            bgmReplayOnEnd = false;
        }
    }

    private void SunEventMiddle() {
        if (Event_LastBattleSecond.Instance) {
            Event_LastBattleSecond.Instance.ImperatrixSunMiddle(eventCamPivot[1]);
        }
        if (audioSource) {
            if (audioSource.isPlaying) {
                audioSource.Stop();
            }
            audioSource.clip = audioClips[voiceTypeSun];
            audioSource.Play();
        }
        if (sunObj) {
            sunObj.SetActive(true);
        }
        if (sunSE) {
            sunSE.volume = 0f;
            sunSE.Play();
        }
        if (sandstarRawEnabled) {
            attackPower = attackPowerFinal;
        }
        sunVolumeIncreaseTimer = 4f;
        sunVolumeIncreaseMax = 4f;
    }

    private void SunEventBlackout() {
        if (Event_LastBattleSecond.Instance) {
            Event_LastBattleSecond.Instance.ImperatrixSunBlackout();
        }
        if (sunSE) {
            sunSE.Stop();
        }
        if (sunObj) {
            sunObj.SetActive(false);
        }
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], 0);
        eventProgress = 2;
    }

    public void SunEventVolumeIncrease(float volumeMultiplier = 1f) {
        if (sunObj) {
            sunObj.SetActive(true);
        }
        if (sunSE) {
            sunSE.volume = 0f;
            sunSE.Play();
        }
        sunVolumeIncreaseTimer = 4f;
        sunVolumeIncreaseMax = 4f;
        sunVolumeMultiplier = volumeMultiplier;
    }

    public void SunEventVolumeStop() {
        sunVolumeIncreaseTimer = -1;
        if (sunObj) {
            sunObj.SetActive(false);
        }
        if (brokenSunObj) {
            brokenSunObj.SetActive(true);
        }
    }

    public void SunEventBreakSun() {
        BootDeathEffect(sunBreakEffect);
        if (brokenSunObj) {
            brokenSunObj.SetActive(false);
        }
        CameraManager.Instance.SetQuake(GetCenterPosition(), 3, 8, 0, 0f, 3f, 50f, 200f);
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], 1);
    }

    public void SunEventBattleRestart() {
        eventProgress = 10;
        attackStiffTime = 0f;
        attackedTimeRemain = 4f;
        coreHideDenomi = sandstarRawEnabled ? 8f : 6f;
        if (blackMinmiIDRank > 0) {
            attackedTimeRemain += blackMinmiIDRank * 0.4f;
        }
        CoreHide();
    }

    protected override void BootDeathEffect(EnemyDeath enemyDeath) {
        for (int i = 1; i < 6; i++) {
            enemyDeath.colorNum = i;
            base.BootDeathEffect(enemyDeath);
            enemyDeath.deadEffect.prefab = null;
        }
    }

    private void CoreHide() {
        for (int i = 0; i < criticalDD.Length; i++) {
            if (criticalDD[i]) {
                criticalDD[i].SetActive(false);
            }
        }
        if (searchTarget[searchTargetCritical]) {
            searchTarget[searchTargetCritical].SetActive(false);
        }
        if (searchTarget[searchTargetNormal]) {
            searchTarget[searchTargetNormal].SetActive(false);
        }
        isCoreShowed = false;
        ResetKnockRemain();
        if (shieldBody && shieldBody.activeSelf == false) {
            shieldBody.transform.localScale = new Vector3(0.0001f, 1f, 0.0001f);
            shieldBody.SetActive(true);
            shieldBody.transform.DOScale(Vector3.one, 1.25f);
        }
        DestroyScaffold();
        float weakTemp = (float)nowHP / GetMaxHP();
        int shieldMaxTemp = 3;
        if (CharacterManager.Instance.playerIndex == CharacterManager.playerIndexHyper && !isForAmusement) {
            if (sandstarRawEnabled) {
                shieldMaxTemp = (weakTemp >= 2f / 3f ? shieldEnemyMaxProgress[0] : weakTemp >= 1f / 3f ? shieldEnemyMaxProgress[1] : shieldEnemyMaxProgress[2]);
            } else {
                shieldMaxTemp = (weakTemp >= 1f / 2f ? shieldEnemyMaxProgress[0] : shieldEnemyMaxProgress[1]);
            }
        } else {
            shieldMaxTemp = shieldEnemyMaxProgress[0];
        }
        for (int i = 0; i < shieldEnemyInstance.Length; i++) {
            if (shieldEnemyInstance[i]) {
                EnemyBase eBase = shieldEnemyInstance[i].GetComponent<EnemyBase>();
                if (eBase) {
                    eBase.ResetHP();
                }
            } else {
                if (i < shieldMaxTemp) {
                    shieldEnemyInstance[i] = Instantiate(shieldEnemyPrefab, shieldEnemyPivot[i].position, shieldEnemyPivot[i].rotation);
                    shieldEnemyInstance[i].GetComponent<MissingObjectToDestroy>().SetGameObject(gameObject);
                    shieldEnemyBase[i] = shieldEnemyInstance[i].GetComponent<Enemy_ImperatrixChild>();
                    shieldEnemyBase[i].SetSearchTargetReference(searchReferenceTrans.gameObject);
                    shieldEnemyBase[i].parentEnemy = this;
                    if (sandstarRawEnabled && !isForAmusement) {
                        shieldEnemyBase[i].SetSandstarRaw();
                    } else if (isForAmusement) {
                        shieldEnemyBase[i].SetForAmusement(null);
                    }
                    shieldEnemyBase[i].SetAttackedTimeRemain(shieldMaxTemp <= 3 || i % 2 == 0 ? 1.02f : 1.04f);
                }
            }
        }
    }

    private void CoreShow() {
        for (int i = 0; i < criticalDD.Length; i++) {
            if (criticalDD[i]) {
                criticalDD[i].SetActive(true);
            }
        }
        if (searchTarget[searchTargetCritical]) {
            searchTarget[searchTargetCritical].SetActive(true);
        }
        if (searchTarget[searchTargetNormal]) {
            searchTarget[searchTargetNormal].SetActive(true);
        }
        isCoreShowed = true;
        ResetKnockRemain();
        coreTimeRemain = coreTimeMax;
        coreShowHP = nowHP;
        coreHideConditionDamage = GetCoreHideConditionDamage();
        healEffectEmitted = false;
        heavyKnocked = false;
        EmitEffect(effCoreShow);
        if (shieldBody) {
            shieldBody.SetActive(false);
        }
        SetScaffolds();
    }

    void CheckHyperScaffold() {
        if (scaffoldInstance[3]) {
            bool playerIsHyper = (CharacterManager.Instance.pCon && CharacterManager.Instance.pCon.isHyper && CharacterManager.Instance.pCon.isSuperman);
            if (scaffoldInstance[3].activeSelf != playerIsHyper) {
                scaffoldInstance[3].SetActive(playerIsHyper);
            }
        }
        if (scaffoldInstance[3] && scaffoldInstance[3].activeSelf) {
            criticalSearchPriority.priority = 20;
            criticalSearchPriority.ignoreMultiplier = true;
        } else {
            criticalSearchPriority.priority = 5;
            criticalSearchPriority.ignoreMultiplier = false;
        }
    }

    void SetScaffolds() {
        int scaffoldMax = CharacterManager.Instance.pCon.isHyper ? 4 : 3;
        for (int i = 0; i < scaffoldMax; i++) {
            if (scaffoldInstance[i] == null) {
                scaffoldInstance[i] = Instantiate(scaffoldPrefab[i], trans.position, quaIden);
            }
        }
        CheckHyperScaffold();
    }

    public override void SetSandstarRaw() {
        base.SetSandstarRaw();
        if (state != State.Dead && isCoreShowed) {
            CoreHide();
        } else if (!isForAmusement) {
            SetSandstarRawForChild();
        }
        coreHideDenomi = 8f;
    }

    private void SetSandstarRawForChild() {
        for (int i = 0; i < shieldEnemyInstance.Length; i++) {
            if (shieldEnemyInstance[i]) {
                shieldEnemyInstance[i].GetComponent<Enemy_ImperatrixChild>().SetSandstarRaw();
            }
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        weakProgress = 0;
        float maxHPTemp = GetMaxHP();
        if (maxHPTemp > 0) {
            float nowHPTemp = nowHP;
            float damageRate = (maxHPTemp - nowHPTemp) / maxHPTemp * CharacterManager.Instance.riskyIncSqrt;
            weakProgress = Mathf.Clamp((int)(damageRate * (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled ? 5f : 4f)), 0, 4);
        }
        if (sandstarRawEnabled) {
            weakProgress += 1;
        }
        if (isForAmusement && !battleStarted) {
            amusementBattleTimer -= deltaTimeCache;
            if (amusementBattleTimer <= 0f) {
                BattleStart();
            }
        }
        attackedTimeRemainOnDamage = 2.5f - weakProgress * 0.2f;
        attractionTime = 6f - weakProgress * 0.5f;
        confuseTime = 3f - weakProgress * 0.25f;
        if (sandstarRawEnabled && !isForAmusement && eventProgress >= 10 && attackPower != attackPowerFinal) {
            attackPower = attackPowerFinal;
        }
        if (isCoreShowed && state != State.Dead) {
            coreTimeRemain -= deltaTimeCache * (coreTimeRemain >= 10f ? CharacterManager.Instance.riskyIncSqrt : 1f);
            if (coreTimeRemain < 2f && (nowHP == 1 || (sunIndependentEnemyInstance && sunIndependentBase && !sunIndependentBase.burstFlag))) {
                coreTimeRemain = 2f;
            }
            if (!healEffectEmitted && coreTimeRemain < 1.25f) {
                EmitEffect(effCoreHide);
                healEffectEmitted = true;
            }
            if (coreTimeRemain < 0) {
                CoreHide();
            }
        }
        if (CharacterManager.Instance.playerIndex != CharacterManager.playerIndexHyper) {
            knockRemainLight = knockEnduranceLight;
        }
        if (state != State.Spawn && !isCoreShowed && (eventProgress <= 0 || eventProgress >= 10)) {
            bool allNull = true;
            for (int i = 0; i < shieldEnemyInstance.Length; i++) {
                if (shieldEnemyInstance[i] != null) {
                    allNull = false;
                    break;
                }
            }
            if (allNull) {
                CoreShow();
            } else {
                for (int i = 0; i < CharacterManager.Instance.friends.Length; i++) {
                    if (CharacterManager.Instance.GetFriendsExist(i, true) && CharacterManager.Instance.friends[i].trans) {
                        float minSqrDist = float.MaxValue;
                        int minIndex = -1;
                        for (int j = 0; j < shieldEnemyInstance.Length; j++) {
                            if (shieldEnemyInstance[j]) {
                                float sqrDistTemp = (shieldEnemyInstance[j].transform.position - CharacterManager.Instance.friends[i].trans.position).sqrMagnitude;
                                if (sqrDistTemp < minSqrDist) {
                                    minSqrDist = sqrDistTemp;
                                    minIndex = j;
                                }
                            }
                        }
                        if (minIndex >= 0) {
                            CharacterManager.Instance.friends[i].fBase.SetLockTargetExternal(shieldEnemyInstance[minIndex].GetComponent<CharacterBase>(), 5f);
                        }
                    }
                }
            }
        }
        if (searchReferenceTrans && CharacterManager.Instance.playerTrans) {
            searchReferencePos.y = Mathf.Clamp(CharacterManager.Instance.playerTrans.position.y - trans.position.y + searchReferenceOffset, searchReferenceRange.x, searchReferenceRange.y);
            searchReferenceTrans.localPosition = searchReferencePos;
        }
        if (state != State.Attack && laserEnabled) {
            LaserCancel();
        }
        if (state != State.Attack && laserIsShooting) {
            laserIsShooting = false;
        }
        if (state == State.Attack && laserIsShooting && StageManager.Instance && StageManager.Instance.graphBuildNowFloor && StageManager.Instance.graphBuildNowFloor.usedFlag) {
            buildBreakTimer += deltaTimeCache;
            if (buildBreakTimer >= 0.5f) {
                buildBreakTimer = 0f;
                StageManager.Instance.graphBuildNowFloor.Recollapse();
            }
        } else {
            buildBreakTimer = 0f;
        }
        if (state == State.Attack && laserLookatEnabled && enemyCanvasChildObject[(int)EnemyCanvasChild.paperPlane].activeSelf == false && enemyCanvasChildObject[(int)EnemyCanvasChild.margayVoice].activeSelf == false) {
            retargetingConditionTime = 0.001f;
            retargetingDecayMultiplier = 1f;
        } else {
            retargetingConditionTime = 8f;
            retargetingDecayMultiplier = 1.6f;
        }
        if (state != State.Spawn && laserAttackedTimeRemain > -100f) {
            laserAttackedTimeRemain -= deltaTimeMove;
        }
        if (state != State.Spawn && meteorAttackedTimeRemain > -100f) {
            meteorAttackedTimeRemain -= deltaTimeMove;
        }
        if (state == State.Damage || state == State.Dead) {
            volcanoTimer = -1f;
        } else if (volcanoCount < volcanoCountMax && volcanoTimer >= 0f && deltaTimeCache > 0f) {
            volcanoTimer -= deltaTimeCache;
            if (Mathf.CeilToInt((volcanoTimerMax - volcanoTimer) / volcanoTimerMax * (volcanoCountMax - 1)) > volcanoCount) {
                throwing.ThrowStart(throwIndexVolcano + volcanoCount);
                volcanoCount++;
            }
        }
        if (state == State.Damage || state == State.Dead) {
            poisonTimer = -1f;
            if (poisonNoticeInstance) {
                Destroy(poisonNoticeInstance);
            }
        } else if (poisonTimer > 0f && deltaTimeCache > 0f) {
            poisonTimer -= deltaTimeCache;
            if (poisonTimer <= 0f && poisonNoticeInstance) {
                int indexTemp = throwIndexPoison + poisonType;
                throwing.throwSettings[indexTemp].from.transform.position = poisonNoticeInstance.transform.position;
                throwing.throwSettings[indexTemp].randomDirection = vecZero;
                throwing.throwSettings[indexTemp].velocity = 18f * 1.4f;
                throwing.throwSettings[indexTemp].randomForceRate = 0f;
                throwing.throwSettings[indexTemp].lookTarget = true;
                throwing.ThrowStart(indexTemp);
                throwing.throwSettings[indexTemp].randomDirection = Vector3.one * 2f;
                throwing.throwSettings[indexTemp].velocity = 18f;
                throwing.throwSettings[indexTemp].randomForceRate = 0.4f;
                throwing.throwSettings[indexTemp].lookTarget = false;
                for (int i = 1; i < 12; i++) {
                    Vector2 circle = Random.insideUnitCircle * 2f;
                    Vector3 posTemp = poisonNoticeInstance.transform.position;
                    posTemp.x += circle.x;
                    posTemp.z += circle.y;
                    throwing.throwSettings[indexTemp].from.transform.position = posTemp;
                    throwing.ThrowStart(indexTemp);
                }
                EmitEffect(effPoisonStart);
            }
        }
        if (state == State.Damage || state == State.Dead) {
            missileReadyTimer = -1f;
            missileStartTimer = -1f;
        } else {
            if (missileReadyTimer > 0f) {
                missileReadyTimer -= deltaTimeCache;
                int tempReadyMax = Mathf.Min((int)((1f - missileReadyTimer) * missileMax), missileMax);
                while (missileReadyCount < tempReadyMax) {
                    throwing.ThrowReady(throwIndexMissile + missileReadyCount);
                    missileReadyCount++;
                }
            }
            if (missileStartTimer > 0f) {
                missileStartTimer -= deltaTimeCache;
                int tempStartMax = Mathf.Min((int)((1f - missileStartTimer) * missileMax), missileMax);
                while (missileStartCount < tempStartMax) {
                    throwing.ThrowStart(throwIndexMissile + missileStartCount);
                    missileStartCount++;
                }
            }
        }
        if (!isForAmusement && eventProgress == 0 && nowHP <= GetMaxHP() * 4 / 5) {
            if (isCoreShowed == true && coreTimeRemain > 10) {
                coreTimeRemain = 10;
            }
            if (isCoreShowed == false && GetCanControl()) {
                eventProgress = 1;
                ClearSick();
                SetState(State.Attack);
            }
        }
        if (targetTrans) {
            if (targetTrans.position.y > trans.position.y + 8f && GetTargetDistance(true, true, false) < 10f * 10f) {
                attackWaitingLockonRotSpeed = 0.375f;
            } else {
                attackWaitingLockonRotSpeed = 1.5f;
            }
        } else {
            attackWaitingLockonRotSpeed = 1.5f;
        }
        if (sunVolumeIncreaseTimer > 0f) {
            sunVolumeIncreaseTimer -= deltaTimeCache;
            if (sunSE) {
                sunSE.volume = Mathf.Clamp01((sunVolumeIncreaseMax - sunVolumeIncreaseTimer) / sunVolumeIncreaseMax) * sunVolumeMultiplier;
            }
        }
        if (sandstarRawEnabled && !isForAmusement && sunIndependentInterval > -100f) {
            sunIndependentInterval -= deltaTimeCache;
        }
        for (int i = 0; i < shieldEnemyLaserAttacking.Length; i++) {
            shieldEnemyLaserAttacking[i] = (shieldEnemyBase[i] && shieldEnemyBase[i].IsLaserAttacking());
        }
        if (slowMotionTimeRemain > 0f) {
            slowMotionTimeRemain -= deltaTimeCache;
            if (slowMotionTimeRemain <= 0f) {
                GameManager.Instance.ChangeTimeScale(false);
                isImmortal = false;
                TakeDamageFixKnock(576000000, GetCenterPosition(), 0, vecZero, CharacterManager.Instance.pCon, damageColor_Hyper, true);
            }
        }
        if (battleStarted && GameManager.Instance.save.difficulty <= 2 && !sandstarRawEnabled) {
            for (int i = 0; i < weakPoints.Length; i++) {
                if (weakPoints[i].instance) {
                    if (weakPoints[i].type == WeakPointType.Other && (!isCoreShowed || state == State.Dead || attackType == attackTypeSunIndependent)) {
                        Destroy(weakPoints[i].instance);
                    }
                } else if (!weakPoints[i].used) {
                    if (weakPoints[i].type == WeakPointType.Other && eventProgress >= 10 && isCoreShowed && state != State.Dead) {
                        weakPoints[i].instance = Instantiate(CharacterManager.Instance.weakPointPrefab[weakPoints[i].prefabIndex], weakPoints[i].pivot);
                        weakPoints[i].used = true;
                    }
                }
            }
        }
        CheckHyperScaffold();
    }

    void QuakeKnockHeavy() {
        if (!isItem) {
            CameraManager.Instance.SetQuake(GetCenterPosition(), 5, 4, 0, 0f, 3f, 15f, 100f);
        }
    }

    void QuakeKnockLight() {
        if (!isItem) {
            CameraManager.Instance.SetQuake(GetCenterPosition(), 5, 4, 0, 0, 1.5f, 15f, 100f);
        }
    }

    void QuakeLong() {
        if (!isItem) {
            CameraManager.Instance.SetQuake(GetCenterPosition(), 2, 8, 2f, 5f, 2f, 100f, 100f);
        }
    }

    void QuakeKnockDead() {
        if (!isItem) {
            CameraManager.Instance.SetQuake(trans.position, 6, 4, 0, 0f, 3f, 30f, 100f);
        }
    }

    protected override void BattleStart() {
        base.BattleStart();
        spawnStiffTime = 0f;
        actDistNum = 1;
        attackedTimeRemain = 4f;
        if (blackMinmiIDRank > 0) {
            attackedTimeRemain += blackMinmiIDRank * 0.4f;
        }
        laserAttackedTimeRemain = 15f;
        CoreHide();
    }

    public void BattleStartExternal() {
        BattleStart();
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (battleStarted) {
            base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (actDistNum != 1) {
            BattleStart();
        }
    }

    public void SetKnockType(int knockType) {
        if (knockType != knockTypeSave) {
            knockTypeSave = knockType;
            anim.SetInteger(animHash_KnockType, knockType);
        }
    }

    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        if (!isSuperarmor) {
            QuakeKnockLight();
            EmitEffect(effKnockLight);
            PlayVoice();
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        heavyKnocked = true;
        if (isCoreShowed && coreTimeRemain > 1.5f) {
            coreTimeRemain = 1.5f;
        }
        QuakeKnockHeavy();
        EmitEffect(effKnockHeavy);
        PlayVoice();
        if (eventProgress >= 10) {
            TakeDamageFixKnock(GetMaxHP() / 20, GetCenterPosition(), 0, vecZero, CharacterManager.Instance.pCon, damageColor_Critical, true);
        }
        if (coreTimeRemain < 2f && nowHP == 1) {
            coreTimeRemain = 2f;
        }
    }

    public override float GetKnocked() {
        return base.GetKnocked() * (!heavyKnocked && isCoreShowed && sunIndependentEnemyInstance == null && nowHP <= GetCoreHideBorder() ? 0 : 1);
    }

    void LaserCancel() {
        laserEnabled = false;
        laserIsShooting = false;
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
        laserIsShooting = false;
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
        laserIsShooting = true;
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
        laserIsShooting = false;
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

    void LaserReadySet() {
        if (state == State.Attack) {
            if (laserPivot) {
                if (targetTrans) {
                    laserPivot.LookAt(targetTrans.position);
                } else {
                    laserPivot.localRotation = quaIden;
                }
            }
            LaserReady();
            AttackStart(attackIndexLaserReady);
            AttackStart(attackIndexLaserReady + 1);
            laserLookatEnabled = true;
        }
    }

    void LaserStartSet() {
        if (state == State.Attack && attackType == attackTypeLaser) {
            LaserStart();
            AttackEnd(attackIndexLaserReady);
            AttackEnd(attackIndexLaserReady + 1);
            AttackStart(attackIndexLaserBody);
            AttackStart(attackIndexLaserBody + 1);
            laserAttackedTimeRemain = 30f;
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

    private void PlayVoice(int index = -1) {
        if (audioSource && !audioSource.isPlaying) {
            int voiceTemp = index;
            if (voiceTemp < 0) {
                voiceTemp = Random.Range(0, audioClips.Length);
                if (voiceTemp == voiceSave) {
                    voiceTemp = (voiceTemp + Random.Range(1, audioClips.Length)) % audioClips.Length;
                }
            }
            voiceSave = voiceTemp;
            audioSource.clip = audioClips[voiceTemp];
            audioSource.Play();
        }
    }

    private void PlayVoice_Spawn() {
        if (blackMinmiIDRank == 0) {
            PlayVoice(0);
        }
    }

    private float GetInterval() {
        if (weakProgress >= 4) {
            return Random.Range(0.3f, 0.6f);
        } else if (weakProgress == 3) {
            return Random.Range(0.9f, 1.2f);
        } else if (weakProgress == 2) {
            return 2f;
        } else if (weakProgress == 1) {
            return 3f;
        } else {
            return 4f;
        }
    }

    private int GetAttackTemp() {
        int answer = 0;
        if (attackSave == -1) {
            answer = 1;
        } else if (laserAttackedTimeRemain < 0f && Random.Range(0, 100) < 50) {
            answer = 0;
        } else {
            answer = Random.Range(1, 8);
            if (answer == 1 && meteorAttackedTimeRemain > 0f) {
                answer = (answer + Random.Range(1, 7)) % 7 + 1;
            }
        }
        return answer;
    }

    protected override void Attack() {
        base.Attack();
        sunSetFlag = false;
        if (eventProgress == 1) {
            AttackBase(attackTypeEvent, 99f, 99f, 0, 30f, 30f, 0, 1, false);
            trans.position = StageManager.Instance.dungeonController.fixSettings.enemy[0].position;
            trans.eulerAngles = StageManager.Instance.dungeonController.fixSettings.enemy[0].rotation;
            if (Event_LastBattleSecond.Instance) {
                Event_LastBattleSecond.Instance.ImperatrixSunStart(eventCamPivot[0]);
            }
            for (int i = 0; i < shieldEnemyInstance.Length; i++) {
                if (shieldEnemyInstance[i]) {
                    Destroy(shieldEnemyInstance[i]);
                }
            }
            if (sunIndependentEnemyInstance) {
                Destroy(sunIndependentEnemyInstance);
            }
            throwing.ThrowCancelAll(false);
            volcanoTimer = -1;
            poisonTimer = -1;
            missileReadyTimer = -1;
            missileStartTimer = -1;
            shieldBody.SetActive(false);
            return;
        }
        if (isCoreShowed) {
            bool sunAttackEnabled = false;
            if (nowHP >= sunIndependentHPCondition) {
                if (sandstarRawEnabled && eventProgress >= 10 && sunIndependentInterval <= 0f && coreTimeRemain >= coreTimeMax * 0.2f && Random.value <= 1.1f + (sunIndependentInterval < -10f ? (sunIndependentInterval + 10f) * -0.01f : 0f) - (coreTimeRemain / coreTimeMax)) {
                    sunIndependentInterval = 30f;
                    sunAttackEnabled = true;
                }
            } else if (nowHP == 1) {
                sunAttackEnabled = true;
                if (coreTimeRemain < 8f) {
                    coreTimeRemain = 8f;
                }
            }
            if (sunAttackEnabled) {
                AttackBase(attackTypeSunIndependent, 99.99f, 200f, 0, 600f / 60f, 600f / 60f, 0, 1, false);
                SuperarmorStart();
                return;
            }
        }
        int attackTemp = GetAttackTemp();
        if (attackTemp == attackSave) {
            attackTemp = GetAttackTemp();
        }
        attackSave = attackTemp;
        float intervalPlus = GetInterval();
        switch (attackTemp) {
            case 0:
                AttackBase(attackTypeLaser, 1.4f, 50f, 0, 370f / 60f, 370f / 60f + intervalPlus, 0, 1, false);
                if (targetTrans) {
                    laserPivot.LookAt(targetTrans.position);
                }
                for (int i = 0; i < attackLaserBody.Length; i++) {
                    attackDetection[attackLaserBody[i]].multiHitInterval = 0.1f;
                }
                laserAttackedTimeRemain = 20f;
                break;
            case 1:
                AttackBase(attackTypeMeteor, 1.2f, 4f, 0, 120f / 60f, 120f / 60f + intervalPlus, 0, 1, false);
                if (CharacterManager.Instance.playerTrans) {
                    Vector3 posTemp = CharacterManager.Instance.playerTrans.position;
                    posTemp.y = trans.position.y;
                    playerPosPivot.position = posTemp;
                }
                meteorAttackedTimeRemain = 12f;
                throwing.ThrowStart(throwIndexMeteor);
                EmitEffect(effCommonReady);
                break;
            case 2:
                AttackBase(attackTypeBomb, 1f, 1.4f, 0, 120f / 60f, 120f / 60f + intervalPlus, 0, 1, false);
                if (CharacterManager.Instance.playerTrans) {
                    playerPosPivot.position = CharacterManager.Instance.playerTrans.position;
                }
                throwing.throwSettings[throwIndexBomb].from.transform.localPosition = vecZero;
                throwing.ThrowStart(throwIndexBomb);
                for (int i = 1; i < 40; i++) {
                    Vector3 randVec = vecZero;
                    randVec.x = Random.Range(-25f, 25f);
                    randVec.y = Random.Range(-16f, 16f);
                    randVec.z = Random.Range(-10f, 10f);
                    throwing.throwSettings[throwIndexBomb].from.transform.localPosition = randVec;
                    throwing.ThrowStart(throwIndexBomb);
                }
                EmitEffect(effCommonReady);
                break;
            case 3:
                AttackBase(attackTypeVolcano, 1.05f, 5f, 0, 120f / 60f, 120f / 60f + intervalPlus, 0, 1, false);
                if (CharacterManager.Instance.playerTrans) {
                    Vector3 posTemp = CharacterManager.Instance.playerTrans.position;
                    posTemp.y = trans.position.y;
                    playerPosPivot.position = posTemp;
                    Vector3 diff = (posTemp - trans.position).normalized;
                    if (diff != vecZero) {
                        volcanoPivot.rotation = Quaternion.LookRotation(posTemp);
                    } else {
                        volcanoPivot.localEulerAngles = new Vector3(0f, Random.Range(-180f, 180f), 0f);
                    }
                }
                volcanoTimer = volcanoTimerMax;
                volcanoCount = 0;
                EmitEffect(effCommonReady);
                break;
            case 4:
                AttackBase(attackTypeIce, 1f, 1.7f, 0, 120f / 60f, 120f / 60f + intervalPlus, 0, 1, false);
                if (CharacterManager.Instance.playerTrans) {
                    playerPosPivot.position = CharacterManager.Instance.playerTrans.position;
                }
                throwing.throwSettings[throwIndexIce].from.transform.localPosition = vecZero;
                throwing.ThrowStart(throwIndexIce);
                for (int i = 1; i < 20; i++) {
                    Vector3 randVec = vecZero;
                    randVec.x = Random.Range(-27f, 27f);
                    randVec.y = Random.Range(-16f, 16f);
                    randVec.z = Random.Range(-10f, 10f);
                    throwing.throwSettings[throwIndexIce].from.transform.localPosition = randVec;
                    throwing.ThrowStart(throwIndexIce);
                }
                EmitEffect(effCommonReady);
                break;
            case 5:
                AttackBase(attackTypePoison, 0.6f, 0.6f, 0, 120f / 60f, 120f / 60f + intervalPlus, 0, 1, false);
                if (CharacterManager.Instance.playerTrans) {
                    Vector3 posTemp = CharacterManager.Instance.playerTrans.position;
                    posTemp.y = trans.position.y;
                    playerPosPivot.position = posTemp;
                }
                poisonType = (poisonType + 1) % poisonNoticePrefab.Length;
                poisonNoticeInstance = Instantiate(poisonNoticePrefab[poisonType], playerPosPivot.position, trans.rotation);
                poisonTimer = 50f / 60f;
                EmitEffect(effCommonReady);
                if (CharacterManager.Instance.playerTrans) {
                    AutoScale autoScale = poisonNoticeInstance.GetComponent<AutoScale>();
                    if (autoScale) {
                        float height = Mathf.Abs(CharacterManager.Instance.playerTrans.position.y - playerPosPivot.position.y);
                        if (height > 8f) {
                            float plusNum = Mathf.Clamp((height - 8f) * 0.5f, 0f, 12f);
                            autoScale.endScale.x += plusNum;
                            autoScale.endScale.z += plusNum;
                        }
                    }
                }
                break;
            case 6:
                AttackBase(attackTypeMissile, 1f, 1.4f, 0, 120f / 60f, 120f / 60f + intervalPlus, 0, 1, false);
                MissileReady();
                EmitEffect(effCommonReady);
                break;
            case 7:
                AttackBase(attackTypeCelestial, 1f, 1.1f, 0, 120f / 60f, 120f / 60f + intervalPlus, 0, 1, false);
                SetCelestialStar();
                EmitEffect(effCommonReady);
                break;
        }
        PlayVoice(-1);
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        switch (attackType) {
            case attackTypeLaser:
                if (laserLookatEnabled && laserPivot && targetTrans) {
                    SmoothRotation(laserPivot, targetTrans.position - laserPivot.position, sandstarRawEnabled && !isForAmusement ? 1.5f : 1f, 0.625f, false);
                }
                break;
            case attackTypeSunIndependent:
                if (sunIndependentEnemyInstance == null || (sunIndependentBase && sunIndependentBase.burstFlag)) {
                    isAnimStopped = false;
                }
                if (sunSetFlag && coreTimeRemain > 3f && (sunIndependentEnemyInstance == null || (sunIndependentBase && sunIndependentBase.burstFlag))) {
                    SetScaffolds();
                    sunSetFlag = false;
                }
                if (isAnimStopped) {
                    if (attackedTimeRemain < 3f) {
                        attackedTimeRemain = 3f;
                    }
                    if (attackStiffTime < stateTime + 3) {
                        attackStiffTime = stateTime + 3f;
                    }
                }
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], isAnimStopped ? 0f : 1f);
                break;
        }
    }

    void LaserMultiHitEnd() {
        if (state == State.Attack) {
            for (int i = 0; i < attackLaserBody.Length; i++) {
                attackDetection[attackLaserBody[i]].multiHitInterval = 0f;
            }
        }
    }

    void MissileReady() {
        missileType = (missileType + 1) % missilePrefab.Length;
        missileReadyCount = 0;
        missileStartCount = 0;
        missileReadyTimer = 1f;
        missileStartTimer = 1.5f;
        for (int i = 0; i < missileMax; i++) {
            throwing.throwSettings[throwIndexMissile + i].prefab = missilePrefab[missileType];
        }
    }

    public override void EmitEffectString(string type) {
        switch (type) {
            case "HatchingReady":
                EmitEffect(effHatchingReady);
                break;
            case "LaserCountDown":
                if (state == State.Attack) {
                    EmitEffect(effLaserCountDown);
                    EmitEffect(effLaserCountDown2);
                }
                break;
        }
    }

    public override float GetLockonRotSpeedRate() {
        return (GetSick(SickType.Slow) ? 0.5f : 1f);
    }

    void DestroyScaffold() {
        for (int i = 0; i < scaffoldInstance.Length; i++) {
            if (scaffoldInstance[i]) {
                Destroy(scaffoldInstance[i]);
            }
        }
    }

    void DestroyScaffoldEffect(int index) {
        if (scaffoldInstance[index]) {
            EmitEffect(effDestroyScaffold + index);
            Destroy(scaffoldInstance[index]);
        }
    }

    protected override void Start_Process_Dead() {
        base.Start_Process_Dead();
        DestroyScaffold();
        for (int i = 0; i < shieldEnemyInstance.Length; i++) {
            if (shieldEnemyInstance[i]) {
                EnemyBase eBaseTemp = shieldEnemyInstance[i].GetComponent<EnemyBase>();
                if (eBaseTemp) {
                    eBaseTemp.ForceDeath();
                }
            }
        }
        if (sunIndependentEnemyInstance) {
            EnemyBase eBaseTemp = sunIndependentEnemyInstance.GetComponent<EnemyBase>();
            if (eBaseTemp) {
                eBaseTemp.ForceDeath();
            }
        }
        if (shieldBody.activeSelf) {
            shieldBody.SetActive(false);
        }
        if (scrollTexture) {
            scrollTexture.enabled = false;
        }
        SetForChangeMatSet(deadMatSet, true);
        if (isLastOne) {
            deadTimer = 3.5f;
            destroyOnDead = false;
            EmitEffect(effDeadSave);
            BGM.Instance.StopFade(4f);
            eventProgress = 20;
        } else {
            deadTimer = 3.5f;
            destroyOnDead = true;
            EmitEffect(effDeadSave);
        }
    }

    protected override void DeadProcess() {
        if (!destroyOnDead && !isForAmusement) {
            SetForChangeMatSet(burstMatSet, true);
            CharacterManager.Instance.ShowBossResult(enemyID, sandstarRawEnabled);
            if (sandstarRawEnabled && !isForAmusement && CharacterManager.Instance.GetBossTimeInteger() <= 300) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_ImperatrixMundiSpeedrun, true);
            }
        }
        if (supermanEffectActivated) {
            SetSupermanEffect(false);
        }
        base.DeadProcess();
    }

    private void SetSunIndependent() {
        sunIndependentEnemyInstance = Instantiate(sunIndependentEnemyPrefab, transform.position, Quaternion.identity);
        sunIndependentEnemyInstance.GetComponent<MissingObjectToDestroy>().SetGameObject(gameObject);
        sunIndependentBase = sunIndependentEnemyInstance.GetComponent<Enemy_ImperatrixSun>();
        sunIndependentBase.SetParentEnemy(this, (20000 + Mathf.Clamp(GameManager.Instance.save.difficulty - 1, 0, 3) * 3000 + (sandstarRawEnabled && !isForAmusement ? 3000 : 0)) * (CharacterManager.Instance.playerIndex == CharacterManager.playerIndexHyper ? 1f : 0.25f));
        sunIndependentBase.SetSearchTargetReference(searchReferenceTrans.gameObject);
        sunIndependentBase.burstDamage = burstDamage[Mathf.Clamp(GameManager.Instance.save.difficulty - 1, 0, 3) + (sandstarRawEnabled ? 4 : 0)];
        sunIndependentInterval = 50f;
        if (audioSource) {
            if (audioSource.isPlaying) {
                audioSource.Stop();
            }
            audioSource.clip = audioClips[voiceTypeSun];
            audioSource.Play();
        }
        EmitEffect(effSun);
        if (!sunAttackTalked) {
            sunAttackTalked = true;
        }
        sunSetFlag = true;
    }

    protected override void Update_Process_Dead() {
        base.Update_Process_Dead();
        switch (eventProgress) {
            case 20:
                if (audioSource && deadAudioClip) {
                    if (audioSource.isPlaying) {
                        audioSource.Stop();
                    }
                    audioSource.clip = deadAudioClip;
                    audioSource.Play();
                }
                eventProgress++;
                break;
            case 21:
                if (stateTime >= 5f) {
                    if (Event_LastBattleSecond.Instance) {
                        Event_LastBattleSecond.Instance.DefeatTalk();
                    }
                    eventProgress++;
                }
                break;
            case 22:
                if (stateTime >= 8f) {
                    if (Event_LastBattleSecond.Instance) {
                        Event_LastBattleSecond.Instance.BattleEnd();
                    }
                    eventProgress++;
                }
                break;
        }
    }

    public void ReceiveSunIndependentBurst() {
        if (nowHP < sunIndependentHPHeal) {
            nowHP = sunIndependentHPHeal;
            AddNowHP(sunIndependentHPHeal - nowHP, GetCenterPosition(), true, damageColor_Heal);
            if (coreTimeRemain > 3f) {
                coreTimeRemain = 3f;
            }
        }
    }

    public void ReceiveSunIndependentDead() {
        if (nowHP <= 1) {
            Time.timeScale = 0.1f;
            slowMotionTimeRemain = 0.125f;
        }
    }

    public int GetLaserAttackingCount() {
        int answer = 0;
        for (int i = 0; i < shieldEnemyLaserAttacking.Length; i++) {
            if (shieldEnemyLaserAttacking[i]) {
                answer++;
            }
        }
        return answer;
    }

    void GraphBuildRecollapse() {
        if (StageManager.Instance && StageManager.Instance.graphBuildNowFloor && StageManager.Instance.graphBuildNowFloor.usedFlag) {
            StageManager.Instance.graphBuildNowFloor.Recollapse();
        }
    }

    void SetCelestialStar() {
        celestialEffectState = 0;
        for (int i = 0; i < celestialObjs.Length; i++) {
            if (celestialObjs[i]) {
                Destroy(celestialObjs[i]);
                celestialObjs[i] = null;
            }
            if (i == 0) {
                if (CharacterManager.Instance.playerTrans) {
                    celestialObjs[i] = Instantiate(celestialStarPrefab, CharacterManager.Instance.playerTrans.position + celestialStarOffset, quaIden);
                    celestialObjs[i].GetComponent<ImperatrixMundi_CelestialStar>().SetParentEnemy(this);
                    celestialObjs[i].GetComponent<SmoothChaseTarget>().SetTarget(CharacterManager.Instance.playerTrans);
                    
                }
            } else if (i < GameManager.friendsMax) {
                if (CharacterManager.Instance.GetFriendsExist(i, true) && CharacterManager.Instance.friends[i].trans) {
                    celestialObjs[i] = Instantiate(celestialStarPrefab, CharacterManager.Instance.friends[i].trans.position + celestialStarOffset, quaIden);
                    celestialObjs[i].GetComponent<ImperatrixMundi_CelestialStar>().SetParentEnemy(this);
                    celestialObjs[i].GetComponent<SmoothChaseTarget>().SetTarget(CharacterManager.Instance.friends[i].trans);
                }
            }
        }
        if (celestialObjs[0]) {
            Vector3 pivotPos = celestialObjs[0].transform.position;
            for (int i = 1; i < celestialObjs.Length; i++) {
                if (celestialObjs[i] == null) {
                    Vector3 randPos = vecZero;
                    for (int r = 3; r > 0; r--) {
                        bool tooNear = false;
                        Vector2 randCircle = Random.insideUnitCircle * 30f;
                        randPos = new Vector3(transform.position.x + randCircle.x, pivotPos.y, transform.position.z + randCircle.y);
                        for (int j = 0; j < celestialObjs.Length; j++) {
                            if (i != j && celestialObjs[j] && (randPos - celestialObjs[j].transform.position).sqrMagnitude < r * 3) {
                                tooNear = true;
                                break;
                            }
                        }
                        if (!tooNear) {
                            celestialObjs[i] = Instantiate(celestialStarPrefab, randPos, quaIden);
                            celestialObjs[i].GetComponent<ImperatrixMundi_CelestialStar>().SetParentEnemy(this);
                            break;
                        }
                    }
                }
                
            }
        }
    }

    public void EmitCelestialReady() {
        if (celestialEffectState < 1) {
            celestialEffectState = 1;
            if (celestialObjs[0]) {
                effect[effCelestialReady].pivot = celestialObjs[0].transform;
            }
            EmitEffect(effCelestialReady);
        }
    }

    public void EmitCelestialStart() {
        if (celestialEffectState < 2) {
            celestialEffectState = 2;
            if (celestialObjs[0]) {
                effect[effCelestialStart].pivot = celestialObjs[0].transform;
            }
            EmitEffect(effCelestialStart);
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        for (int i = 0; i < celestialObjs.Length; i++) {
            if (celestialObjs[i]) {
                Destroy(celestialObjs[i]);
                celestialObjs[i] = null;
            }
        }
    }
    public override bool IsThrowCancelling {
        get {
            return state == State.Dead || (state == State.Attack && attackType == attackTypeEvent);
        }
    }

}
