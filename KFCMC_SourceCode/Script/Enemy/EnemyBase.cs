using UnityEngine;
using UnityEngine.UI;
using static ItemDatabase;

public class EnemyBase : CharacterBase {

    protected float supermanKnockPlus = 0.3f;
    protected bool cannotDoubleKnockDown;
    protected const int silhouetteConditionId = 8;
    protected const float supermanStartTime = 60f;
    protected const int dropItemMax = 2;
    protected const int riskyCost = 12;
    protected const float dissipationDistance_Normal = 12.5f;
    protected const float dissipationDistance_Large = 25.0f;
    protected const float dissipationDistance_Boss = 50.0f;

    protected static readonly float[] knockDownKnockMul = new float[] { 0.7f, 0.7f, 0.6f, 0.5f, 0.0f };
    protected const int powLevelMax = 4; // レベルによる衝撃耐性などの累乗の最大値

    [System.Serializable]
    public class ChangeTarget {
        public Renderer renderer;
        public int index;
    }

    [System.Serializable]
    public class LevelStatus {
        public Material[] changeMaterial;
        public GameObject[] activateObj;
        public int deadEffectColor = 0;
    }

    [System.Serializable]
    public class EnemyDeath {
        public Effect deadEffect;
        public AudioSourcePool.AudioName deadAudio;
        public Transform pivot;
        public float scale = 0.1f;
        public int colorNum = 0;
        public int numOfPieces = 16;
        public float radius = 0.4f;
        public float force = 1.0f;
    }

    [System.Serializable]
    public class VariableLevelSettings {
        public int nowVariableLevel;
        public float hpMul = 1f;
        public float assumeHPMul = 1f;
        public float attackMul = 1f;
        public float defenseMul = 0f;
        public float expMul = 1f;
    }

    public int enemyID;
    public bool dampNormalDamage = true;
    public Vector3 supermanEffectScale = new Vector3(1, 1, 1);
    public float levelKnockedRate = 1.4f;
    public float levelSpeedRate = 1.15f;
    public float levelAngularRate = 1.1f;
    public float sickEffectScale = 1;
    public bool silhouetteEnabled = false;
    public Renderer[] silhouetteRenderer;
    public Vector3 canvasPos = new Vector3(0, 1, 0);
    public float canvasScale = 1;
    public Transform canvasParent;
    public int mapChipSize;
    public ChangeTarget[] changeTarget;
    public LevelStatus[] levelStatus;
    public EnemyDeath ed;
    public float friendsKnockRate = 1f;
    public int costKnockedBase = 12;
    public bool limitSteal;
    public VariableLevelSettings variableLevelSettings;

    [System.NonSerialized]
    public bool defeatCountEnabled;
    [System.NonSerialized]
    public bool dropExpDisabled;
    [System.NonSerialized]
    public bool summonedByBlackCrystal;
    [System.NonSerialized]
    public bool excludeActionEnemy;

    protected int exp;
    protected bool damageFromPlayerFlag;
    protected GameObject enemyCanvas;
    protected GameObject enemyBalloon;
    protected bool enemyCanvasLoaded;
    protected bool enemyBalloonLoaded;
    protected float attractionTime = 6;
    protected float confuseTime = 3f;
    protected float paperPlaneTolerance;
    protected float confuseTolerance;
    protected float angryFixTime = 0.1f;
    protected float angryAttractionTime = 12f;
    protected float silhouetteUpdateTimeRemain;
    protected float targetExistTime;
    protected bool isDamaged;
    protected bool isPerfectStealth;
    protected bool expForce;
    protected float dropItemVelocity = 5f;
    protected float dropItemBalloonDelay = -1;
    protected float dropItemGetDelay = -1;
    protected float actionCheckTimeRemain;
    protected bool balloonEnabled;
    protected bool silhouetteActive;
    protected int silhouettePropertyID;
    protected float silhouetteSqrDist = 64f;
    protected bool killByCriticalOnly;
    protected float killByCriticalFailedKnockAmount = 100000;
    protected bool deathEffectEnabled = true;
    protected bool supermanEnabled;
    protected float supermanStartTimeRemain;
    protected int supermanEffectNumber;
    protected int supermanAuraNumber;
    protected float seeThroughDistanceSave;
    protected float dontForgetDistanceSave;
    protected float dontTargetingTimeRemain;
    protected Image wrFillImage;
    protected Transform camT;
    protected int[] dropItem = new int[dropItemMax];
    protected int[] dropRate = new int[dropItemMax];
    protected float[,] attackIntervalLevelEffectRate = new float[4, 5] {
        {1f, 1f, 0.9666666f, 0.9333333f, 0.9f },
        {1f, 1f, 0.9333333f, 0.8666666f, 0.8f },
        {1f, 1f, 0.9f, 0.8f, 0.7f },
        {1f, 1f, 0.9f, 0.8f, 0.7f }
    };
    protected int stealedCount;
    protected int stealedMax = 1;
    protected float normallyRequiredMaxMultiplier = 1f;
    protected float attackedTimeRemainReduceRadius = 1000f;
    protected float supermanCheckInterval = 0.1f;
    protected GameObject supermanEffectInstance;
    protected GameObject supermanAuraInstance;
    protected bool minmiGolden;
    protected float[] sickTolerance = new float[sickMax];
    protected bool[] sickByPlayer = new bool[sickMax];
    protected bool fixKnockAmount;
    protected int assumeMaxHP;
    protected bool supermanReservedFlag;
    protected int friendsCostSumSave;
    protected bool isHomoChild;
    protected bool levelFiveEnabled;
    protected bool levelFiveEffecting;
    protected GameObject levelFiveEffectInstance;
    protected CharacterBase homoParentBase;
    protected float attackedTimeRemainReduceOnAngry = 1f;
    protected bool catchupExpDisabled;
    protected bool killByPlayerOnly;
    protected bool isImmortal;
    protected bool isForAmusement;
    protected bool t_AngryFlag;
    protected bool isAttracterPlayer;

    /* Knockゲージ回復抑制 */
    protected float _stopKnockRecoveryTimeRemain;
    protected const float StopKnockRecoveryTime = 1;

    /* コンボ */
    protected int comboRank = -1;
    protected float comboRankExpBonus;

    protected static readonly float[] LineCheckDistancePlusArray = { 0, 1.333333f, 3.333333f, 6.333333f }; //マップチップの半径と連動し、ラインチェッカーの距離を加算する

    protected class SaveDefaultStatus {
        public float knockEndurance;
        public float knockEnduranceLight;
        public float knockRecovery;
        public float maxSpeed;
        public float walkSpeed;
        public float acceleration;
        public float angularSpeed;
        public SaveDefaultStatus() {
            knockEndurance = 100;
            knockEnduranceLight = 10;
            knockRecovery = 25;
            maxSpeed = 5;
            walkSpeed = 3;
            acceleration = 5;
            angularSpeed = 135;
        }
    }

    protected SaveDefaultStatus defStats = new SaveDefaultStatus();

    //protected enum EnemyCanvasChild { backNormal, backWRFill, knockRecovery, knockDiff, knockFill, hpDiff, hpFill, wrFill, paperPlane, ibisSong, margayVoice };
    //protected GameObject[] enemyCanvasChildObject;
    protected EnemyCanvasElements enemyCanvasElements;
    protected EnemyBalloonElements enemyBalloonElements;

    public override int Level {
        get {
            return level;
        }
        set {
            SetLevel(value, false);
        }
    }

    protected override void Awake() {
        base.Awake();
        isEnemy = true;
        defStats.knockEndurance = knockEndurance;
        defStats.knockEnduranceLight = knockEnduranceLight;
        defStats.knockRecovery = knockRecovery = Mathf.Pow(knockEndurance, 1f / 5f) * 6f;
        defStats.maxSpeed = maxSpeed;
        defStats.walkSpeed = walkSpeed;
        defStats.acceleration = acceleration;
        defStats.angularSpeed = angularSpeed;

        superAttackRate = 2f;
        superDefenseRate = 2f;
        superKnockRate = 1f;
        superSpeedRate = 1.5f;
        superAccelerationRate = 2;
        supermanEffectNumber = (int)EffectDatabase.id.enemyYK;
        supermanAuraNumber = (int)EffectDatabase.id.enemyYK_Aura;
        level = 0;
        destroyOnDead = true;
        deadTimer = 0;
        supermanStartTimeRemain = supermanStartTime;
        silhouettePropertyID = Shader.PropertyToID("_UseRim");
        checkGroundedLayerMask = LayerMask.GetMask("Default", "Field", "InvisibleWall", "SecondField", "ThirdField");
        spawnStiffTime = 1f;
        dodgeRemain = dodgePower;
        dodgeDistance = 5f;
        dodgeMutekiTime = 0.05f;
        dodgeDamageHealMax = dodgePower;
        consumeStamina = false;
        colorTypeDamage = damageColor_Enemy;
        for (int i = 0; i < dropItemMax; i++) {
            dropRate[i] = (int)CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.DropRate, forceDefaultDifficulty);
        }
        targetHateEnabled = true;
        defeatCountEnabled = true;
        LoadEnemyCanvas();
    }

    protected override void Start() {
        base.Start();
        GetMainCamera();
    }

    protected virtual void ReservePieces() {
        if (ObjectPool.Instance && ed.numOfPieces > 0) {
            ObjectPool.Instance.ReserveObject(ed.colorNum, ed.numOfPieces);
        }
    }

    protected bool GetMainCamera() {
        if (CameraManager.Instance) {
            CameraManager.Instance.SetMainCameraTransform(ref camT);
        } else {
            Camera mainCamera = Camera.main;
            if (mainCamera) {
                camT = mainCamera.transform;
            }
        }
        return camT != null;
    }

    protected virtual void ChangeSilhouette(bool flag) {
        for (int i = 0; i < silhouetteRenderer.Length; i++) {
            Material[] mats = silhouetteRenderer[i].materials;
            for (int j = 0; j < mats.Length; j++) {
                mats[j].SetFloat(silhouettePropertyID, flag ? 1 : 0);
            }
            silhouetteRenderer[i].materials = mats;
        }
    }

    protected virtual void CheckSilhouette() {
        if (silhouetteEnabled && centerPivot) {
            silhouetteUpdateTimeRemain -= Time.deltaTime;
            if (silhouetteUpdateTimeRemain < 0f) {
                silhouetteUpdateTimeRemain = 0.5f;
                bool cond = CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.XRay) != 0;
                if (cond && (camT || GetMainCamera())) {
                    cond = ((camT.position - GetCenterPosition()).sqrMagnitude > silhouetteSqrDist && Physics.Linecast(camT.position, GetCenterPosition(), fieldLayerMask));
                }
                if (silhouetteActive && !cond) {
                    silhouetteActive = false;
                    ChangeSilhouette(silhouetteActive);
                } else if (!silhouetteActive && cond) {
                    silhouetteActive = true;
                    ChangeSilhouette(silhouetteActive);
                }
            }
        }
    }

    protected override bool CheckSickEffect(SickType sickType, int databaseIndex, bool isCenter = false) {
        if (sickEffectScale > 1.2f && sickType == SickType.Fire) {
            databaseIndex = (int)EffectDatabase.id.sickFireEnemy;
        }
        if (base.CheckSickEffect(sickType, databaseIndex, isCenter)) {
            if (sickEffectScale != 1f) {
                sickEffectInstance[(int)sickType].transform.localScale *= sickEffectScale;
                if (sickEffectScale > 3.2f) {
                    AudioSource sickAudio = sickEffectInstance[(int)sickType].GetComponent<AudioSource>();
                    if (sickAudio) {
                        sickAudio.minDistance = sickEffectScale * 0.625f;
                    }
                }
            }
            return true;
        }
        return false;
    }

    protected virtual void LoadEnemyCanvas()
    {
        enemyCanvas = Instantiate(CharacterDatabase.Instance.ui.enemyCanvas, canvasParent ? canvasParent : transform);
        enemyCanvasElements = enemyCanvas.GetComponent<EnemyCanvasElements>();
        wrFillImage = enemyCanvasElements.WrFill.GetComponent<Image>();
        SetEnemyCanvasWRGauge(false);

        enemyBalloon = Instantiate(CharacterDatabase.Instance.ui.enemyBalloon, canvasParent ? canvasParent : transform);
        enemyBalloonElements = enemyBalloon.GetComponent<EnemyBalloonElements>();
        enemyBalloonElements.PaperPlane.SetActive(false);
        enemyBalloonElements.IbisSong.SetActive(false);
        enemyBalloonElements.MargayVoice.SetActive(false);

        if (canvasScale != 1)
        {
            UIOperationCanvasOverlayEnemyHP uio1 = enemyCanvas.GetComponentInChildren<UIOperationCanvasOverlayEnemyHP>();
            uio1.scaleMultiplier *= canvasScale;

            UIOperationCanvas_InverseScaling uio2 = enemyBalloon.GetComponentInChildren<UIOperationCanvas_InverseScaling>();
            uio2.scaleMultiplier *= canvasScale;
        }

        if (mapChipSize >= 0 && mapChipSize < LineCheckDistancePlusArray.Length && LineCheckDistancePlusArray[mapChipSize] != 0)
        {
            LineCheckerWithCameraOrPlayer lineChecker = enemyCanvas.GetComponent<LineCheckerWithCameraOrPlayer>();
            if (lineChecker)
            {
                lineChecker.AddDistance(LineCheckDistancePlusArray[mapChipSize]);
            }
        }

        enemyCanvasLoaded = true;
        enemyBalloonLoaded = true;
    }

    protected override GameObject SetMapChip()
    {
        GameObject mapChipObj = null;
        if (mapChipSize >= 0)
        {
            if (mapChipSize == 0)
            {
                mapChipObj = Instantiate(MapDatabase.Instance.prefab[MapDatabase.enemy], trans);
            }
            else if (mapChipSize == 1)
            {
                mapChipObj = Instantiate(MapDatabase.Instance.prefab[MapDatabase.enemyL], trans);
            }
            else if (mapChipSize == 2)
            {
                mapChipObj = Instantiate(MapDatabase.Instance.prefab[MapDatabase.enemyXL], trans);
            }
            else
            {
                mapChipObj = Instantiate(MapDatabase.Instance.prefab[MapDatabase.enemyXXL], trans);
            }
        }
        return mapChipObj;
    }

    protected virtual bool CheckAttackerIsPlayer(CharacterBase attacker) {
        return attacker && attacker == CharacterManager.Instance.pCon;
    }

    protected virtual bool CheckAttackerIsSpecialFriends(CharacterBase attacker) {
        return attacker && attacker == CharacterManager.Instance.specialFriendsBase;
    }

    protected bool CheckSickKillEnabled()
    {
        for (int i = 0; i < sickByPlayer.Length; i++)
        {
            if (sickByPlayer[i])
            {
                return true;
            }
        }
        return false;
    }

    protected void SetLevelFiveEffect(bool flag) {
        if (flag) {
            if (levelFiveEffectInstance == null) {
                levelFiveEffectInstance = Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.levelFive], centerPivot ? centerPivot : transform);
                if (sickEffectScale != 1f) {
                    levelFiveEffectInstance.transform.localScale *= sickEffectScale;
                }
            }
        } else {
            if (levelFiveEffectInstance) {
                Destroy(levelFiveEffectInstance);
            }
        }
        levelFiveEffecting = flag;
    }

    protected void SetStatusLevel100() {
        int vlTemp = 99;
        if (vlTemp < CharacterDatabase.Instance.variableStatus.Length) {
            if (dampNormalDamage) {
                maxHP = Mathf.RoundToInt(CharacterDatabase.Instance.variableStatus[vlTemp].haveCoreHP * variableLevelSettings.hpMul);
            } else {
                maxHP = Mathf.RoundToInt(CharacterDatabase.Instance.variableStatus[vlTemp].noCoreHP * variableLevelSettings.hpMul);
            }
            assumeMaxHP = Mathf.RoundToInt(CharacterDatabase.Instance.variableStatus[vlTemp].noCoreHP * variableLevelSettings.assumeHPMul);
            attackPower = Mathf.RoundToInt(CharacterDatabase.Instance.variableStatus[vlTemp].attack * variableLevelSettings.attackMul);
            defensePower = Mathf.RoundToInt(attackPower * variableLevelSettings.defenseMul);
            exp = Mathf.RoundToInt(20000 * variableLevelSettings.expMul);
        }
    }

    protected void SetStatusUpperJewel()
    {
        maxHP = Mathf.RoundToInt(maxHP * 1.25f);
        attackPower = Mathf.RoundToInt(attackPower * 1.25f);
        defensePower = Mathf.RoundToInt(defensePower * 1.25f);
        exp = Mathf.RoundToInt(exp * 1.5f);
    }

    public virtual void SetLevel(int newLevel, bool effectFlag = false, bool isLevelUp = true, int variableLevel = 0) {
        if ((!isBoss || !isLevelUp) && state != State.Dead) {
            bool levelEqual = (level == newLevel);
            int maxLevel = Mathf.Min(CharacterDatabase.Instance.enemy[enemyID].maxLevel, StageManager.Instance.GetEnemyLevelMax);
            level = newLevel;
            if (level < 0) {
                level = 0;
            } else if (level > maxLevel) {
                if (!isLevelUp) {
                    levelFiveEnabled = true;
                }
                level = maxLevel;
            }
            int powLevelKnock = Mathf.Clamp(level, 0, powLevelMax);
            int powLevelSpeed = powLevelKnock;
            if (StageManager.Instance.IsTribulationsStage && !isBoss && variableLevel >= 1 && variableLevel <= CharacterDatabase.Instance.variableStatus.Length)
            {
                int powLevelTemp = variableLevel / 20 + 1;
                powLevelKnock = Mathf.Clamp(powLevelTemp, 0, powLevelMax);
                powLevelSpeed = powLevelTemp;
            }

            float knockedRate = Mathf.Max(MyMath.PowInt(levelKnockedRate, powLevelKnock - 1), 1f);
            float speedRate = Mathf.Max(MyMath.PowInt(levelSpeedRate, powLevelSpeed - 1), 1f);
            float angularRate = Mathf.Max(MyMath.PowInt(levelAngularRate, powLevelSpeed - 1), 1f);
            if (!isBoss && variableLevel >= 1 && variableLevel <= CharacterDatabase.Instance.variableStatus.Length) {
                variableLevelSettings.nowVariableLevel = variableLevel;
                int vlTemp = variableLevel - 1;
                if (dampNormalDamage) {
                    maxHP = Mathf.RoundToInt(CharacterDatabase.Instance.variableStatus[vlTemp].haveCoreHP * variableLevelSettings.hpMul);
                } else {
                    maxHP = Mathf.RoundToInt(CharacterDatabase.Instance.variableStatus[vlTemp].noCoreHP * variableLevelSettings.hpMul);
                }
                assumeMaxHP = Mathf.RoundToInt(CharacterDatabase.Instance.variableStatus[vlTemp].noCoreHP * variableLevelSettings.assumeHPMul);
                attackPower = Mathf.RoundToInt(CharacterDatabase.Instance.variableStatus[vlTemp].attack * variableLevelSettings.attackMul);
                defensePower = Mathf.RoundToInt(attackPower * variableLevelSettings.defenseMul);
                exp = Mathf.RoundToInt(CharacterDatabase.Instance.variableStatus[vlTemp].exp * variableLevelSettings.expMul);
                dropItem[0] = CharacterDatabase.Instance.enemy[enemyID].status[level].item0;
                dropItem[1] = CharacterDatabase.Instance.enemy[enemyID].status[level].item1;
                if (!(dropItem[1] >= 0 && dropItem[1] <= 99)) {
                    if (isHomoChild) {
                        dropItem[1] = 319;
                    } else {
                        if (variableLevel <= 18) {
                            dropItem[1] = 315;
                        } else if (variableLevel <= 36) {
                            dropItem[1] = 316;
                        } else if (variableLevel <= 54) {
                            dropItem[1] = 317;
                        } else if (variableLevel <= 72) {
                            dropItem[1] = 318;
                        } else {
                            dropItem[1] = 319;
                        }
                    }
                }
                SetLevelFiveEffect(vlTemp >= 99);
            } else {
                variableLevelSettings.nowVariableLevel = 0;
                if (CharacterDatabase.Instance.enemy[enemyID].statusExist[level]) {
                    maxHP = CharacterDatabase.Instance.enemy[enemyID].status[level].hp;
                    attackPower = CharacterDatabase.Instance.enemy[enemyID].status[level].attack;
                    defensePower = CharacterDatabase.Instance.enemy[enemyID].status[level].defense;
                    exp = CharacterDatabase.Instance.enemy[enemyID].status[level].exp;
                    dropItem[0] = CharacterDatabase.Instance.enemy[enemyID].status[level].item0;
                    dropItem[1] = CharacterDatabase.Instance.enemy[enemyID].status[level].item1;
                    int assumeLevel = CharacterDatabase.Instance.enemy[enemyID].status[level].assumeLevel;
                    if (assumeLevel >= 1 && assumeLevel <= CharacterDatabase.Instance.variableStatus.Length) {
                        assumeMaxHP = Mathf.RoundToInt(CharacterDatabase.Instance.variableStatus[assumeLevel - 1].noCoreHP * variableLevelSettings.assumeHPMul);
                    } else {
                        assumeMaxHP = maxHP;
                    }
                    if (!isBoss && levelFiveEnabled && newLevel >= maxLevel) {
                        if (level <= 4)
                        {
                            SetStatusLevel100();
                        } else
                        {
                            SetStatusUpperJewel();
                        }
                        SetLevelFiveEffect(true);
                    } else {
                        SetLevelFiveEffect(false);
                    }
                }
            }
            knockEndurance = defStats.knockEndurance * knockedRate;
            knockEnduranceLight = defStats.knockEnduranceLight * knockedRate;
            knockRecovery = defStats.knockRecovery * knockedRate;
            if (variableLevelSettings.nowVariableLevel >= 1) {
                float knockLimit = 100 * variableLevelSettings.nowVariableLevel;
                if (knockEndurance > knockLimit) {
                    float limitRate = knockLimit / knockEndurance;
                    knockEndurance = knockLimit;
                    knockEnduranceLight *= limitRate;
                    knockRecovery = Mathf.Pow(knockEndurance, 1f / 5f) * 6f;
                }
            }
            maxSpeed = defStats.maxSpeed * speedRate;
            walkSpeed = defStats.walkSpeed * speedRate;
            acceleration = defStats.acceleration * speedRate;
            angularSpeed = defStats.angularSpeed * angularRate;
            if (!isLevelUp || !levelEqual) {
                nowHP = maxHP;
            }
            nowST = maxST;
            knockRemain = knockEndurance;
            knockRemainLight = knockEnduranceLight;
            isDamaged = false;
            lightKnockCount = 0;
            heavyKnockCount = 0;
            comboRank = -1;
            comboRankExpBonus = 0;
            if (!isSuperman) {
                if (supermanEnabled) {
                    if (enemyCanvasLoaded)
                    {
                        SetEnemyCanvasWRGauge(false);
                        if (wrFillImage) {
                            wrFillImage.fillAmount = 0f;
                        }
                    }
                    supermanEnabled = false;
                }
                supermanStartTimeRemain = supermanStartTime;
            }

            if (level < levelStatus.Length) {
                if (levelStatus[level].deadEffectColor >= 0) {
                    ed.colorNum = levelStatus[level].deadEffectColor;
                    ReservePieces();
                }
                int changeMax = System.Math.Min(changeTarget.Length, levelStatus[level].changeMaterial.Length);
                for (int i = 0; i < changeMax; i++) {
                    Material[] materials = changeTarget[i].renderer.materials;
                    materials[changeTarget[i].index] = levelStatus[level].changeMaterial[i];
                    changeTarget[i].renderer.materials = materials;
                }
                silhouetteActive = false;
                ScrollTexture[] scrollTexture = GetComponentsInChildren<ScrollTexture>();
                for (int i = 0; i < scrollTexture.Length; i++) {
                    scrollTexture[i].ResetMaterial();
                }
            }
            for (int i = 0; i < levelStatus.Length; i++) {
                if (levelStatus[i].activateObj.Length > 0) {
                    bool toActive = (i == level);
                    for (int j = 0; j < levelStatus[i].activateObj.Length; j++) {
                        if (levelStatus[i].activateObj[j] && levelStatus[i].activateObj[j].activeSelf != toActive) {
                            levelStatus[i].activateObj[j].SetActive(toActive);
                        }
                    }
                }
            }
            if (effectFlag) {
                if (isLevelUp) {
                    Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.enemyLevelUp], trans);
                } else {
                    Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.enemyLevelDown], trans);
                }
            }
            SetState(State.Spawn);
            SetLevelModifier();
        }
    }

    protected virtual void SetLevelModifier() { }

    public virtual void LevelUp() {
        if (!isBoss) {
            SetLevel(level + 1, true, true, variableLevelSettings.nowVariableLevel >= 1 ? Mathf.Clamp(Mathf.Min(variableLevelSettings.nowVariableLevel + 10, variableLevelSettings.nowVariableLevel * 2), 1, CharacterDatabase.Instance.variableStatus.Length) : 0);
        }
    }

    public virtual void LevelDown() {
        if (!isBoss) {
            SetLevel(level - 1, true, false, variableLevelSettings.nowVariableLevel >= 1 ? Mathf.Clamp(variableLevelSettings.nowVariableLevel - 10, 1, CharacterDatabase.Instance.variableStatus.Length) : 0);
        }
    }

    protected override void Start_Process_Wait() {
        base.Start_Process_Wait();
        arrived = true;
    }
    
    protected override void Update_Targeting() {
        if (dontTargetingTimeRemain <= 0f) {
            base.Update_Targeting();
            if (isHomoChild && target && homoParentBase && homoParentBase.GetNowTarget() != target) {
                target = homoParentBase.GetNowTarget();
                targetTrans = homoParentBase.GetNowTargetTrans();
                targetRadius = homoParentBase.GetNowTargetRadius();
                targetSearchAreaIndex = 0;
            }
            if (target && target != decoySave && !isDamaged && CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Stealth)) {
                SetTargetNull();
            }
        } else {
            SetTargetNull();
        }
        if (target && target != decoySave) {
            targetExistTime += deltaTimeCache;
        } else {
            targetExistTime = 0f;
        }
    }

    protected bool IsAttracted => (enemyBalloonLoaded && (enemyBalloonElements.PaperPlane.activeSelf || enemyBalloonElements.MargayVoice.activeSelf));

    protected bool IsIbisSongAngry => (enemyBalloonLoaded && enemyBalloonElements.IbisSong.activeSelf);

    protected override void Update_TimeCount() {
        bool attracted = IsAttracted;
        bool angry = (enemyBalloonLoaded && enemyBalloonElements.IbisSong.activeSelf);
        attackedTimeRemainReduceMultiplier = angry ? attackedTimeRemainReduceOnAngry : attracted ? 0.85f : 1f;
        sickHealSpeed = isBoss ? 2f : 1f;
        if (GameManager.Instance.minmiPurple) {
            attackedTimeRemainReduceMultiplier *= Mathf.Max(CharacterManager.Instance.riskyIncrease, 7.25f);
        } else if (CharacterManager.Instance.costSumSave > riskyCost) {
            attackedTimeRemainReduceMultiplier *= (notAffectedByRisky ? CharacterManager.Instance.riskyIncSqrt : CharacterManager.Instance.riskyIncrease);
        }
        if (attackedTimeRemainReduceMultiplier > 1f && attackedTimeRemainReduceRadius < 100f) {
            if (targetTrans == null || GetTargetDistance(true, true, false) > attackedTimeRemainReduceRadius * attackedTimeRemainReduceRadius) {
                attackedTimeRemainReduceMultiplier = 1f;
            }
        }
        if (CharacterManager.Instance.costSumSave > riskyCost && !notAffectedByRisky) {
            sickHealSpeed *= CharacterManager.Instance.riskyIncSqrt;
        }
        base.Update_TimeCount();
        actionCheckTimeRemain -= deltaTimeCache;
        if (dontTargetingTimeRemain > 0f) {
            dontTargetingTimeRemain -= deltaTimeCache;
            if (dontTargetingTimeRemain <= 0f && enemyCanvas) {
                enemyBalloonElements.MargayVoice.SetActive(false);
            }
        }
        if (paperPlaneTolerance > 0f) {
            paperPlaneTolerance -= deltaTimeCache;
            if (paperPlaneTolerance < 0f) {
                paperPlaneTolerance = 0f;
            }
        }
        if (confuseTolerance > 0f) {
            confuseTolerance -= deltaTimeCache;
            if (confuseTolerance < 0f) {
                confuseTolerance = 0f;
            }
        }
        for (int i = 0; i < sickTolerance.Length; i++) {
            if (sickTolerance[i] > 0f && sickRemainTime[i] <= 0f) {
                sickTolerance[i] -= deltaTimeCache;
                if (sickTolerance[i] < 0f) {
                    sickTolerance[i] = 0f;
                }
            }
        }
        for (int i = 0; i < sickRemainTime.Length; i++)
        {
            if (sickRemainTime[i] <= 0f)
            {
                sickByPlayer[i] = false;
            }
        }
        if (!isBoss && CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.ExtendIntervalEnabled, forceDefaultDifficulty) != 0) {
            attackedTimeRemain += deltaTimeMove * CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.ExtendInterval, forceDefaultDifficulty);
        }
        if (supermanEnabled && !isSuperman && supermanStartTimeRemain > 0f) {
            float difference = deltaTimeCache * (20 / Mathf.Clamp(CharacterManager.Instance.GetNormallyRequiredAttackCount_Bias(GetAssumeMaxHP()) + 1, 2f, 20f * normallyRequiredMaxMultiplier)) * (attracted ? 0.75f : 1f);
            if (CharacterManager.Instance.costSumSave > riskyCost && !notAffectedByRisky) {
                difference *= CharacterManager.Instance.riskyInc15;
            }
            if (supermanReservedFlag) {
                supermanStartTimeRemain -= 1000;
            } else {
                supermanStartTimeRemain -= difference * CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.WRSpeedBias, forceDefaultDifficulty);
            }
            if (wrFillImage) {
                wrFillImage.fillAmount = Mathf.Clamp01((supermanStartTime - supermanStartTimeRemain) / supermanStartTime);
            }
            if (supermanStartTimeRemain <= 0f && enemyCanvas) {
                SupermanStart();
            }
        }
        if (isSuperman) {
            attackedTimeRemain -= deltaTimeMove;
        }

        // Knockゲージ回復一時停止残り時間を減らす
        if (_stopKnockRecoveryTimeRemain > 0f)
        {
            _stopKnockRecoveryTimeRemain -= deltaTimeMove;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (!minmiGolden && GameManager.Instance.minmiGolden) {
            minmiGolden = true;
        }
        if (actionCheckTimeRemain <= 0f && !excludeActionEnemy) {
            actionCheckTimeRemain = 0.25f;
            if (targetTrans) {
                CharacterManager.Instance.AddActionEnemy(characterId);
            } else {
                CharacterManager.Instance.RemoveActionEnemy(characterId);
            }
        }
        /*
        if (enemyCanvas && enemyBalloonElements.IbisSong.activeSelf) {
            targetHateClearTime = 12.0;
        } else {
            targetHateClearTime = 6.0;
        }
        */
        if (!isBoss && !isSuperman) {
            supermanCheckInterval -= deltaTimeCache;
            if (supermanCheckInterval <= 0f || CharacterManager.Instance.costSumSave != friendsCostSumSave) {
                supermanCheckInterval = 0.1f;
                friendsCostSumSave = CharacterManager.Instance.costSumSave;
                if (((!forceDefaultDifficulty && CharacterManager.Instance.costSumSave > riskyCost && !notAffectedByRisky) || 
                    CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.EnemyWR, forceDefaultDifficulty) != 0) && 
                    !supermanEnabled && target && targetSearchAreaIndex == 0 && enemyCanvas) {
                    if (enemyBalloonElements.PaperPlane.activeSelf == false && enemyBalloonElements.MargayVoice.activeSelf == false && searchArea.Length > 0 && (CharacterManager.Instance.costSumSave > riskyCost || searchArea[0].GetPlayerFindable) && searchArea[0].GetAnyFound) {
                        SetEnemyCanvasWRGauge(true);
                        supermanEnabled = true;
                    }
                }
            }
        }
        if (isHomoChild && state != State.Dead && (homoParentBase == null || homoParentBase.IsDead)) {
            dropExpDisabled = true;
            ForceDeath();
        }
        if (nowHP == 1 && (killByPlayerOnly || GameManager.Instance.save.config[GameManager.Save.configID_BossMode] != 0))
        {
            paperPlaneTolerance = 0f;
            confuseTolerance = 0f;
            ResetSickToleranceAll();
        }
    }

    int RandomizeContentItemID(int itemID) {
        if (itemID >= 316 && itemID <= 319 && Random.Range(0, 100) < 50) {
            return Random.Range(315, itemID);
        }
        return itemID;
    }

    public void GiveItem(int itemID) {
        bool isParenting = (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.itemSettings.container);
        int replaceLevel = (StageManager.Instance && StageManager.Instance.dungeonController ? StageManager.Instance.dungeonController.itemReplaceLevel : 0);
        itemID = RandomizeContentItemID(itemID);
        ItemDatabase.Instance.GiveItem(itemID, trans.position, dropItemVelocity, dropItemBalloonDelay, dropItemGetDelay, 0f, isParenting ? StageManager.Instance.dungeonController.itemSettings.container : null, replaceLevel);
        CharacterManager.Instance.CheckJaparimanShortage(itemID);
    }

    public void GiveItemForSteal(int itemID, Vector3 pivotPos) {
        bool isParenting = (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.itemSettings.container);
        int replaceLevel = (StageManager.Instance && StageManager.Instance.dungeonController ? StageManager.Instance.dungeonController.itemReplaceLevel : 0);
        Vector3 posTemp = trans.position;
        posTemp.y = pivotPos.y;
        itemID = RandomizeContentItemID(itemID);
        ItemDatabase.Instance.GiveItem(itemID, pivotPos + (posTemp - pivotPos).normalized * 0.2f, dropItemVelocity, dropItemBalloonDelay, dropItemGetDelay, 0f, isParenting ? StageManager.Instance.dungeonController.itemSettings.container : null, replaceLevel);
        CharacterManager.Instance.CheckJaparimanShortage(itemID);
    }

    protected void CountDefeat() {
        int defeatID = enemyID * GameManager.enemyLevelMax + level;
        if (defeatID >= 0 && defeatID < GameManager.Instance.save.defeatEnemy.Length) {
            GameManager.Instance.save.defeatEnemy[defeatID] += 1;
            if (TrophyManager.Instance) {
                TrophyManager.Instance.CheckTrophy_DefeatMany();
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_DefeatAllType);
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_CompleteEnemyBook);
            }
        }
    }

    protected override void DeadProcess() {
        bool dropped = false;
        bool supermanReward = (isSuperman && (CharacterManager.Instance.costSumSave <= riskyCost || notAffectedByRisky));
        Vector3 centerPosition = GetCenterPosition();
        float centerHeight = centerPosition.y - trans.position.y;
        if (supermanReward || isHomoChild) {
            if (dropRate[0] > 0 && dropRate[0] < 500) {
                dropRate[0] = 500;
            }
            if (dropRate[1] > 0 && dropRate[1] < 1000) {
                dropRate[1] = 1000;
            }
        }
        if (summonedByBlackCrystal && StageManager.Instance.dungeonController) {
            StageManager.Instance.dungeonController.ReportDefeatBlackCrystal();
        }
        if (isSuperman) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_DefeatWR, true);
        }
        if (expForce || CharacterManager.Instance.GetPlayerAttacked()) {
            if (!StageManager.Instance.dropExpDisabled && !dropExpDisabled && (isBoss || GameManager.Instance.save.config[GameManager.Save.configID_DisableItemDrop] == 0)) {
                for (int i = 0; i < dropItem.Length; i++) {
                    if (dropItem[i] >= 0 && (dropRate[i] >= 1000 || Random.Range(0, 1000) < dropRate[i])) {
                        GiveItem(dropItem[i]);
                        dropped = true;
                        break;
                    }
                }
                if (!isBoss && !dropped) {
                    if (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.JaparibunHunt) != 0) {
                        if (Random.Range(0, 1000) < 150) {
                            GiveItem((int)ItemID.Japariman);
                            dropped = true;
                        }
                    } else if (defeatCountEnabled && CharacterManager.Instance.japarimanShortage > 0) {
                        if (Random.Range(0, 1000) < 50) {
                            GiveItem((int)ItemID.Japariman);
                            dropped = true;
                        }
                    }
                }
            }
            if (defeatCountEnabled) {
                bool showDefeatFlag = (StageManager.Instance && StageManager.Instance.dungeonController && !StageManager.Instance.dungeonController.GetDefeatMissionCompleted());
                CharacterManager.Instance.defeatEnemyCountSSRaw += (isSuperman ? 2 : 1);
                CharacterManager.Instance.defeatEnemyCount += 1;
                if (showDefeatFlag) {
                    //CharacterManager.Instance.ShowDefeatRemain(trans.position + centerTemp + vecDown * Mathf.Min(0.5f, centerTemp.y));
                    CharacterManager.Instance.ShowDefeatRemain(centerPosition + vecDown * Mathf.Min(0.75f, Mathf.Max(centerHeight, 0)));
                }
            }
            CountDefeat();
            CharacterManager.Instance.AddSandstar(CharacterManager.Instance.GetSandstarIncrease_Dead(GetMaxHP()), isBoss);
        }
        CharacterManager.Instance.RemoveActionEnemy(characterId);
        if (!StageManager.Instance.dropExpDisabled && !dropExpDisabled && GameManager.Instance.save.config[GameManager.Save.configID_DisableExp] == 0) {
            float expMul = isBoss ? 1 : 1 + Mathf.Max(comboRankExpBonus, supermanReward ? 2 : 0);
            int expTemp = Mathf.RoundToInt(exp * expMul);
            if (!catchupExpDisabled && GameManager.Instance.save.difficulty <= GameManager.difficultyNT) {
                int lackExp = GameManager.Instance.GetLackExp(GameManager.Instance.levelLimitAuto);
                if (lackExp > expTemp) {
                    expTemp += Mathf.Clamp(Mathf.RoundToInt((lackExp - expTemp) * 0.004f), 0, expTemp);
                }
            }
            if (CharacterManager.Instance.AddExp(expTemp, expForce)) {
                //CharacterManager.Instance.ShowGetExp(trans.position + centerTemp + vecDown * Mathf.Min(0.5f, centerTemp.y), expTemp, comboRank);
                CharacterManager.Instance.ShowGetExp(GetCenterPosition() + vecDown * Mathf.Min(0.75f, Mathf.Max(centerHeight, 0)), expTemp, isBoss ? -1 : comboRank);
            }
        }
        if (deathEffectEnabled) {
            BootDeathEffect(ed);
        }
        base.DeadProcess();
    }

    public virtual void ForceDeath() {
        CharacterManager.Instance.RemoveActionEnemy(characterId);
        if (deathEffectEnabled) {
            BootDeathEffect(ed);
        }
        base.DeadProcess();
    }

    protected override void Update_FaceControl() {
        base.Update_FaceControl();
        if (enemyCanvas) {
            if (balloonEnabled && (!target || target != decoySave)) {
                enemyBalloonElements.PaperPlane.SetActive(false);
                enemyBalloonElements.IbisSong.SetActive(false);
                balloonEnabled = false;
            }
        }
        CheckSilhouette();
    }

    public void SetDropRate(int dropRate = 0) {
        for (int i = 0; i < dropItemMax; i++) {
            this.dropRate[i] = dropRate;
        }
    }

    public virtual int GetStealItemID() {
        if (stealedCount < stealedMax && !StageManager.Instance.IsXRayFloor) {
            if (dropItem.Length > 0) {
                int itemTemp = -1;
                if (limitSteal) {
                    if (Random.Range(0, 100) < 50 + CharacterManager.Instance.stealFailedCount * 10) {
                        itemTemp = dropItem[Random.Range(0, dropItem.Length)];
                        CharacterManager.Instance.stealFailedCount = 0;
                    } else {
                        itemTemp = (Random.Range(0, 2) == 0 ? (int)ItemDatabase.ItemID.HealStar : (int)ItemDatabase.ItemID.SandstarBlock);
                        CharacterManager.Instance.stealFailedCount++;
                    }
                } else {
                    itemTemp = dropItem[Random.Range(0, dropItem.Length)];
                }
                if (itemTemp >= 0) {
                    stealedCount++;
                    return itemTemp;
                }
            }
        }
        return (Random.Range(0, 2) == 0 ? (int)ItemDatabase.ItemID.HealStar : (int)ItemDatabase.ItemID.SandstarBlock);
    }

    public bool GetCanStealItem() {
        return stealedCount < stealedMax;
    }

    protected override float GetCost(CostType type) {
        return 0;
    }

    protected virtual float GetFriendsKnockRate(float multiplier = 1)
    {
        float knockRateTemp = 1f;
        if (costKnockedBase > 0 && CharacterManager.Instance.costSumSave > costKnockedBase)
        {
            knockRateTemp = (float)costKnockedBase / CharacterManager.Instance.costSumSave;
        }
        if (friendsKnockRate < 1f)
        {
            knockRateTemp *= friendsKnockRate * CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.FriendsKnock);
        }
        knockRateTemp *= multiplier;
        return Mathf.Clamp01(knockRateTemp);
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        bool attackerIsPlayer = CheckAttackerIsPlayer(attacker);
        bool attackerIsSpecialFriends = CheckAttackerIsSpecialFriends(attacker);
        bool assisterIsPlayer = !attackerIsPlayer && attacker && (
            (isAttracterPlayer && (IsAttracted || IsIbisSongAngry)) // Kaban, Ibis, Margay
            || attacker.IsBuffByPlayer // Campo Flicker
            || CheckSickKillEnabled()); // Alpaca, Plairie Dog

        isDamaged = true;
        if (attackerIsPlayer) {
            CharacterManager.Instance.PlayerAttackStamp();
            if (!fixKnockAmount && attacker && StageManager.Instance.IsComboRankEnabled)
            {
                attacker.AddComboPoint(colorType);
                int comboRankTemp = CharacterManager.Instance.GetComboRank();
                if (comboRankTemp > comboRank)
                {
                    comboRank = comboRankTemp;
                    comboRankExpBonus = CharacterManager.Instance.GetComboRankExpBonus();
                }
            }
        } else {
            if (!fixKnockAmount) {
                knockAmount *= GetFriendsKnockRate(attackerIsSpecialFriends ? 2 : 1);
            }
        }
        if (!fixKnockAmount && GetSick(SickType.Acid) && (GetDefenseNoEffected() == 0 || colorType == damageColor_Critical || colorType == damageColor_Hyper)) {
            damage += Mathf.RoundToInt(damage * 0.1f);
        }
        if (dampNormalDamage && colorType != damageColor_Critical && colorType != damageColor_Hyper && damage > 0) {
            damage = Mathf.Max(1, Mathf.RoundToInt(damage * CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.DampNormalDamage, forceDefaultDifficulty)));
        }
        if (isSuperman && damage > 0) {
            damage = Mathf.Max(1, damage / 2);
        }

        if (!fixKnockAmount) {
            if (isSuperman && attacker != null && !attacker.isSuperman) {
                knockAmount = 0;
            } else if (!isBoss && state == State.Damage && isDamageHeavy) {
                if (cannotDoubleKnockDown) {
                    knockAmount = 0;
                } else {
                    knockAmount *= Mathf.Clamp01(knockDownKnockMul[Mathf.Clamp(level, 0, knockDownKnockMul.Length - 1)] + (attacker != null && attacker.isSuperman ? supermanKnockPlus : 0f));
                }
            }
        }
        if ((killByCriticalOnly || (GameManager.Instance.save.config[GameManager.Save.configID_BossMode] != 0 && dampNormalDamage)) && damage >= nowHP && !(colorType == damageColor_Critical || colorType == damageColor_Hyper)) {
            damage = nowHP - 1;
            if (state != State.Damage && !isImmortal) {
                knockAmount = killByCriticalFailedKnockAmount;
            }
        }
        if ((killByPlayerOnly || GameManager.Instance.save.config[GameManager.Save.configID_BossMode] != 0)
            && damage >= nowHP
            && attacker
            && !attackerIsPlayer
            && !assisterIsPlayer) { 
            damage = nowHP - 1;
        }
        if (isImmortal && damage >= nowHP) {
            damage = Mathf.Max(nowHP - 1, 0);
        }
        if (dontTargetingTimeRemain > 0 && targetFixingTimeRemain <= 0f)
        {
            dontTargetingTimeRemain = 0;
            if (enemyCanvas)
            {
                enemyBalloonElements.MargayVoice.SetActive(false);
            }
        }
        int nowHPSave = nowHP;
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        if (attacker || fixKnockAmount) {
            float sandstarAddRate = (
                attackerIsPlayer ? CharacterManager.Instance.pCon.GetSandstarHealMultiplierForAttack :
                assisterIsPlayer ? CharacterManager.Instance.pCon.GetSandstarHealMultiplierForAssist :
                fixKnockAmount || attackerIsSpecialFriends ? 1f :
                0.5f);
            if (CharacterManager.Instance.costSumSave > riskyCost) {
                if (attackerIsPlayer || fixKnockAmount) {
                    sandstarAddRate *= CharacterManager.Instance.riskyDecrease;
                } else {
                    sandstarAddRate *= CharacterManager.Instance.riskyDec25;
                }
            }
            if (sandstarAddRate > 0f) {
                CharacterManager.Instance.AddSandstar(CharacterManager.Instance.GetSandstarIncrease(nowHPSave, GetMaxHP(), damage, sandstarAddRate));
            }
            CharacterManager.Instance.InquiryDamage(damage, fixKnockAmount ? CharacterManager.Instance.pCon : attacker, this);
        }
        if (!fixKnockAmount && damage >= 200000) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_HugeDamage, true);
        }

        if (attackerIsPlayer) // 攻撃者がプレイヤーキャラの場合
        {
            _stopKnockRecoveryTimeRemain = StopKnockRecoveryTime; // Knockゲージの回復を一時停止
        }
    }

    public virtual void TakeDamageFixKnock(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        fixKnockAmount = true;
        TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        fixKnockAmount = false;
    }

    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        if (!isSuperarmor) {
            paperPlaneTolerance -= 2f;
            if (paperPlaneTolerance < 0f) {
                paperPlaneTolerance = 0f;
            }
            confuseTolerance -= 2f;
            if (confuseTolerance < 0f) {
                confuseTolerance = 0f;
            }
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        paperPlaneTolerance -= 12f;
        if (paperPlaneTolerance < 0f) {
            paperPlaneTolerance = 0f;
        }
        confuseTolerance -= 12f;
        if (confuseTolerance < 0f) {
            confuseTolerance = 0f;
        }
    }

    public override void Attraction(GameObject decoy, AttractionType type, bool lockForce = false, float targetFixingTime = 0f, bool isAttracterPlayer = false) {
        float attractionTemp = (type == AttractionType.IbisSong ? angryAttractionTime : attractionTime);
        this.isAttracterPlayer = isAttracterPlayer;
        if (isAttracterPlayer)
        {
            CharacterManager.Instance.PlayerAttackStamp();
        }
        if (attractionTemp > 0) {
            float timeMul = 1f;
            if (type == AttractionType.PaperPlane) {

                if (enemyBalloonLoaded && enemyBalloonElements.MargayVoice.activeSelf && dontTargetingTimeRemain >= 0.5f) {
                    return;
                }

                if (paperPlaneTolerance > 0f) {
                    timeMul *= Mathf.Lerp(1f, 0.25f, Mathf.Clamp01(paperPlaneTolerance * 0.0625f));
                }
                paperPlaneTolerance += 8f;
                if (paperPlaneTolerance > 24f) {
                    paperPlaneTolerance = 24f;
                }
            }
            for (int i = 0; i < searchArea.Length; i++) {
                if (searchArea[i] != null && (lockForce || !searchArea[i].isLocking)) {
                    if (searchArea[i].SetLockTarget(decoy, attractionTemp * timeMul)) {
                        targetFixingTimeRemain = Mathf.Min(attractionTemp, targetFixingTime) * timeMul;
                    }
                }
            }
            float attackRemainReset = Mathf.Min(0.4f, targetFixingTime) * timeMul;
            if (attackedTimeRemain < attackRemainReset) {
                attackedTimeRemain = attackRemainReset;
            }
            if (state == State.Wait) {
                SetState(State.Chase);
            }
            dontTargetingTimeRemain = 0;
            decoySave = decoy;
            ClearTargetHate();
            Update_Targeting();
            if (enemyCanvas) {
                switch (type) {
                    case AttractionType.PaperPlane:
                        enemyBalloonElements.IbisSong.SetActive(false);
                        enemyBalloonElements.MargayVoice.SetActive(false);
                        enemyBalloonElements.PaperPlane.SetActive(true);
                        break;
                    case AttractionType.IbisSong:
                        enemyBalloonElements.PaperPlane.SetActive(false);
                        enemyBalloonElements.MargayVoice.SetActive(false);
                        enemyBalloonElements.IbisSong.SetActive(true);
                        break;
                }
            }
            balloonEnabled = true;
        }
    }

    public virtual void MakeAngry(GameObject decoyObject, CharacterBase attacker = null, bool isAttracterPlayer = false) {
        if (isBoss || !IsAttracted) {
            Attraction(decoyObject, AttractionType.IbisSong, true, angryFixTime, isAttracterPlayer);
            if (targetHateEnabled && attacker) {
                RegisterTargetHate(attacker, CharacterManager.Instance.GetNormalKnockAmount() * (isBoss ? 4f : 1f));
            }
            t_AngryFlag = true;
        }
        if (isAttracterPlayer)
        {
            CharacterManager.Instance.PlayerAttackStamp();
        }
        this.isAttracterPlayer = isAttracterPlayer;
    }

    public virtual bool SetConcentratedAttack(Vector3 parentPosition, GameObject decoy, CharacterBase attacker = null) {
        bool answer = false;
        if (decoy && state != State.Dead) {
            if (isBoss || !IsAttracted) {
                float attractionTemp = angryAttractionTime;
                if (attractionTemp > 0) {
                    for (int i = 0; i < searchArea.Length; i++) {
                        if (searchArea[i] != null) {
                            if (searchArea[i].SetLockTarget(decoy, attractionTemp)) {
                                targetFixingTimeRemain = Mathf.Min(attractionTemp, angryFixTime);
                            }
                        }
                    }
                    float attackRemainReset = Mathf.Min(0.4f, angryFixTime);
                    if (attackedTimeRemain < attackRemainReset) {
                        attackedTimeRemain = attackRemainReset;
                    }
                    if (state == State.Wait) {
                        SetState(State.Chase);
                    }
                    dontTargetingTimeRemain = 0;
                    decoySave = decoy;
                    ClearTargetHate();
                    Update_Targeting();
                    if (targetHateEnabled && attacker) {
                        RegisterTargetHate(attacker, CharacterManager.Instance.GetNormalKnockAmount() * (isBoss ? 4f : 1f));
                    }
                    answer = true;
                }
            }
        } else if (state == State.Wait && agent && agent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathInvalid) {
            destination = parentPosition;
            agent.SetDestination(destination);
            arrived = false;
            stateTime = 0f;
            answer = true;
        }
        if (answer) {
            int maxHPTemp = GetMaxHP();
            if (nowHP < maxHPTemp) {
                AddNowHP(Mathf.Min(Mathf.RoundToInt(maxHPTemp * 0.25f * CharacterManager.Instance.riskyIncrease), maxHPTemp - nowHP), GetCenterPosition(), true, damageColor_Heal);
            }
        }
        return answer;
    }

    public virtual void Confuse(bool isAttracterPlayer = false) {
        this.isAttracterPlayer = isAttracterPlayer;
        if (isAttracterPlayer)
        {
            CharacterManager.Instance.PlayerAttackStamp();
        }
        if (confuseTime > 0) {
            float timeMul = 1f;
            /*
            if (CharacterManager.Instance.costSumSave > riskyCost && !notAffectedByRisky) {
                timeMul = CharacterManager.Instance.riskyDecSqrt;
            }
            */
            if (confuseTolerance > 0f) {
                timeMul *= Mathf.Lerp(1f, 0.25f, Mathf.Clamp01(confuseTolerance * 0.0625f));
            }
            confuseTolerance += 8f;
            if (confuseTolerance > 24f) {
                confuseTolerance = 24f;
            }
            dontTargetingTimeRemain = confuseTime * timeMul;
            targetFixingTimeRemain = Mathf.Min(confuseTime, 0.6f) * timeMul;
            for (int i = 0; i < searchArea.Length; i++) {
                searchArea[i].SetUnlockTarget();
                if (!isBoss) {
                    searchArea[i].SetNotWatchout();
                }
            }
            Update_Targeting();
            if (agent && agent.isOnNavMesh) {
                agent.ResetPath();
            }
            if (enemyCanvas) {
                enemyBalloonElements.MargayVoice.SetActive(true);
            }
        }
    }

    public virtual void ReceiveWolfHowling(bool byPlayer)
    {
        SetSick(SickType.Frightened, 9f);
        if (byPlayer)
        {
            sickByPlayer[(int)SickType.Frightened] = true;
            CharacterManager.Instance.PlayerAttackStamp();
        }
    }

    protected virtual void BootDeathEffect(EnemyDeath enemyDeath) {
        if (enemyDeath.deadEffect != null) {
            EmitEffectClass(enemyDeath.deadEffect);
        }
        Vector3 pivot = (enemyDeath.pivot != null ? enemyDeath.pivot.position : GetCenterPosition());
        if (enemyDeath.deadAudio != AudioSourcePool.AudioName.None) {
            AudioSourcePool.Instance.Play(enemyDeath.deadAudio, pivot);
        }
        GameObject pieceInstance;
        Vector3 direction;
        Vector3 forceRandom;
        float mass = enemyDeath.scale * enemyDeath.scale * enemyDeath.scale * 1000;
        float forceBias = mass * enemyDeath.force * 500f;
        int appropriateNum = ObjectPool.Instance.GetAppropriateNum(enemyDeath.numOfPieces);
        for (int i = 0; i < appropriateNum; i++) {
            pieceInstance = ObjectPool.Instance.GetObject(enemyDeath.colorNum);
            if (pieceInstance) {
                direction = Random.insideUnitSphere;
                forceRandom = Random.insideUnitSphere;
                pieceInstance.transform.position = pivot + direction * enemyDeath.radius;
                pieceInstance.GetComponent<CellienPiece>().SetParam(enemyDeath.scale, mass, (direction * 0.9f + forceRandom * 0.1f) * forceBias, pivot);
            } else {
                break;
            }
        }
    }
    protected virtual void SetSupermanEffect(bool flag = true) {
        if (flag) {
            if (supermanEffectInstance == null) {
                supermanEffectInstance = Instantiate(EffectDatabase.Instance.prefab[supermanEffectNumber], centerPivot ? centerPivot : trans);
            }
            if (supermanEffectScale != vecZero) {
                supermanEffectInstance.transform.localScale = supermanEffectScale;
            }
            if (supermanAuraInstance == null) {
                supermanAuraInstance = Instantiate(EffectDatabase.Instance.prefab[supermanAuraNumber], trans);
            }
            if (supermanAuraInstance && !isBoss) {
                ActivateAndPositionToRaycastHit raycasterTemp = supermanAuraInstance.GetComponent<ActivateAndPositionToRaycastHit>();
                if (raycasterTemp) {
                    raycasterTemp.offset = new Vector3(0f, 0.041f + Random.Range(0f, 0.002f), 0f);
                }
            }
        } else {
            if (supermanEffectInstance) {
                Destroy(supermanEffectInstance);
                supermanEffectInstance = null;
            }
            if (supermanAuraInstance) {
                Destroy(supermanAuraInstance);
                supermanAuraInstance = null;
            }
        }
    }

    public virtual void SetForDictionary(bool toSuperman, int layer, Transform dynamicBoneReferenceObject) {
        SetSupermanEffect(toSuperman);
        SupermanSetObj(toSuperman);
        SupermanSetMaterial(toSuperman);
        if (toSuperman) {
            if (supermanEffectInstance) {
                SetLayerChildren(supermanEffectInstance, layer);
            }
            if (supermanAuraInstance) {
                SetLayerChildren(supermanAuraInstance, layer);
            }
        }
        isItem = true;
    }


    protected virtual float GetAttackInterval(float baseValue, int levelBias = 0, float levelEffectRate = 1f, float randomRange = 0.2f) {
        int levelTemp = Mathf.Clamp(level + levelBias, 0, 4);        
        if (levelEffectRate > 0f) {
            if (levelEffectRate >= 1f) {
                baseValue *= attackIntervalLevelEffectRate[Mathf.Clamp(GameManager.Instance.save.difficulty - 1, 0, 3), levelTemp];
            } else {
                baseValue *= Mathf.Lerp(1f, attackIntervalLevelEffectRate[Mathf.Clamp(GameManager.Instance.save.difficulty - 1, 0, 3), levelTemp], levelEffectRate);
            }
        }
        if (randomRange > 0f) {
            baseValue += Random.Range(randomRange * -0.5f, randomRange * 0.5f);
        }
        return baseValue;
    }

    public override void SupermanStart(bool effectEnable = true) {
        if (!isSuperman) {
            SetSupermanEffect();
        }
        nowHP += GetMaxHP();
        supermanStartTimeRemain = -100f;
        base.SupermanStart();
        if (nowHP > GetMaxHP()) {
            nowHP = GetMaxHP();
        }
        SetEnemyCanvasWRGauge(false);
        for (int i = 0; i < searchArea.Length; i++) {
            seeThroughDistanceSave = searchArea[i].seeThroughDistance;
            dontForgetDistanceSave = searchArea[i].dontForgetDistance;
            searchArea[i].seeThroughDistance = 1000;
            searchArea[i].dontForgetDistance = 1000;
        }
    }

    public override void SupermanEnd(bool effectEnable = true) {
        if (!isBoss && isSuperman) {
            for (int i = 0; i < searchArea.Length; i++) {
                if (seeThroughDistanceSave > 0) {
                    searchArea[i].seeThroughDistance = seeThroughDistanceSave;
                }
                if (dontForgetDistanceSave > 0) {
                    searchArea[i].dontForgetDistance = dontForgetDistanceSave;
                }
            }
            if (supermanEffectInstance) {
                Destroy(supermanEffectInstance);
            }
            if (supermanAuraInstance) {
                Destroy(supermanAuraInstance);
            }
            SetEnemyCanvasWRGauge(false);
        }
        base.SupermanEnd(effectEnable);
        if (nowHP > GetMaxHP()) {
            nowHP = GetMaxHP();
        }
        supermanEnabled = false;
        supermanStartTimeRemain = supermanStartTime;
    }

    protected void SetEnemyCanvasWRGauge(bool wrGaugeEnabled)
    {
        if (enemyCanvas)
        {
            enemyCanvasElements.BackNormal.SetActive(!wrGaugeEnabled);
            enemyCanvasElements.BackWRFill.SetActive(wrGaugeEnabled);
            enemyCanvasElements.WrFill.SetActive(wrGaugeEnabled);
        }
    }

    public override int GetMaxHP() {
        return Mathf.RoundToInt(base.GetMaxHP() * (isBoss ? CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.MaxHPBoss, forceDefaultDifficulty) : CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.MaxHPZako, forceDefaultDifficulty)));
    }
    public int GetAssumeMaxHP() {
        return Mathf.RoundToInt(assumeMaxHP * (isBoss ? CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.MaxHPBoss, forceDefaultDifficulty) : CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.MaxHPZako, forceDefaultDifficulty)));
    }
    public override float GetMaxSpeed(bool isWalk = false, bool ignoreSuperman = false, bool ignoreSick = false, bool ignoreConfig = false) {
        return base.GetMaxSpeed(isWalk, ignoreSuperman, ignoreSick, ignoreConfig) * (isWalk ? 1f : GameManager.Instance.minmiPurple ? 3f : notAffectedByRisky ? 1f : CharacterManager.Instance.riskyIncSqrt);
    }
    public float GetMinmiSpeed() {
        return maxSpeed * (GameManager.Instance.minmiPurple ? 3f : 1f);
    }
    public override float GetAcceleration() {
        return base.GetAcceleration() * (GameManager.Instance.minmiPurple ? 9f : notAffectedByRisky ? 1f : CharacterManager.Instance.riskyIncrease);
    }
    public override float GetAngularSpeed() {
        return base.GetAngularSpeed() * (GameManager.Instance.minmiPurple ? 3f : notAffectedByRisky ? 1f : CharacterManager.Instance.riskyIncSqrt);
    }
    public override float GetLockonRotSpeedRate() {
        return base.GetLockonRotSpeedRate() * (GameManager.Instance.minmiPurple ? 3f : notAffectedByRisky ? 1f : CharacterManager.Instance.riskyIncSqrt);
    }
    public override bool GetCanDodge() {
        if (targetExistTime < 0.4f) {
            return false;
        } else {
            return base.GetCanDodge();
        }
    }
    public override void SetSick(SickType sickType, float duration, AttackDetection attacker = null) {
        int index = (int)sickType;
        if (sickTolerance[index] > 0f) {
            duration *= Mathf.Lerp(1f, 0.5f, Mathf.Clamp01(sickTolerance[index] * 0.03125f));
        }
        base.SetSick(sickType, duration, attacker);
        sickTolerance[index] += duration * (sickType == SickType.Stop || sickType == SickType.Slow ? 3f : sickType == SickType.Frightened ? 1.5f : 1f);
        if (sickTolerance[index] > 40f) {
            sickTolerance[index] = 40f;
        }
        if (duration > 0f && attacker && attacker.parentCBase && attacker.parentCBase.isPlayer)
        {
            sickByPlayer[index] = true;
        }
    }
    public void ResetSickTolerance(SickType sickType) {
        sickTolerance[(int)sickType] = 0f;
    }
    public void ResetSickToleranceAll() {
        for (int i = 0; i < sickTolerance.Length; i++) {
            sickTolerance[i] = 0f;
        }
    }

    public virtual void SetForAmusement(Amusement_Mogisen amusement) {
        isForAmusement = true;
    }

    public virtual void SetFalling() {
        if (isBoss) {
            Warp(StageManager.Instance.dungeonController.GetRespawnPosClosest(trans.position), 1f, 0f);
        } else {
            if (state != State.Dead) {
                dropExpDisabled = true;
                ForceDeath();
            }
        }
    }

    protected override void GetRoveDestination() {
        if (isHomoChild && homoParentBase) {
            destination = homoParentBase.transform.position;
        } else {
            base.GetRoveDestination();
        }
    }

    public void ReserveSuperman() {
        if (!isSuperman) {
            supermanEnabled = true;
            supermanReservedFlag = true;
        }
    }

    public void SetHomoParent(CharacterBase parentBase) {
        if (!isHomoChild && !isBoss && state != State.Dead) {
            isHomoChild = true;            
            homoParentBase = parentBase;
            SetLevel(level, false, false, 100);
            roveInterval = 1f;
            ReserveSuperman();
        }
    }

    public override int GetEnemyID() {
        return enemyID;
    }

    protected bool IsSuperLevel => (isSuperman || levelFiveEffecting);

    /* Knockゲージ回復速度レートについて、Knock回復一時停止中は0を返す */
    protected override float GetKnockRecoverySpeedRate()
    {
        if (_stopKnockRecoveryTimeRemain > 0)
        {
            return 0;
        }
        else
        {
            return base.GetKnockRecoverySpeedRate();
        }
    }

}
