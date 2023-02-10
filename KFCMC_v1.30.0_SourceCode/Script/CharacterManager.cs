using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using Rewired;
using Coffee.UIEffects;

public class CharacterManager : SingletonMonoBehaviour<CharacterManager> {

    public enum FriendsEffect {
        Attack, Stamina, Knock, Mud, Fire, Ice, LightKnockEndurance, Jump, Defense, XRay, DrownTolerance, JaparibunHealAll,
        WallBreak, IdlingSTHeal, MapCamera, MaxHP, AttackingSTHeal, Speed, JaparibunHunt, PPP, GoalDirection, Heal,
        Needle, KeenNose, SickHeal, ReduceSlipDamage, WRHeal, LongUse, Muteki, Dodge
    }
    public enum EquipEffect {
        Dodge, Escape, Gather, Attack, Knock, Defense, Speed, SpeedInAir, Impact, Jump, Combo, Pile, SpeedRank, Spin, Wave, Screw, Bolt, Judgement, Antimatter, Analyzer, WildRelease, AllWild, CostMove, CostJump, CostAttack, Plasma, Crest, Gravity
    }
    public enum DifficultyEffect {
        DamageZako, DamageBoss, MaxHPZako, MaxHPBoss, CoreHideRate, DampNormalDamage, SlipDamage, NeedleDamage, FriendsKnock, FriendsDodge, LevelUp, ExtendIntervalEnabled, ExtendInterval, Reaper, EnemyWR, WRSpeedBias, DropRate, ServalGuts, IbisHealRate
    }
    public enum BuffType {
        Antidote, Stamina, Speed, Attack, Defense, Knock, Absorb, Stealth, Long, Multi
    };
    public enum LoudSoundType {
        Burst, WallBreak, EnemyLevelUp, EnemyLevelDown, WaterSplash, EnemyDead, EnemyWR, GetItem, GetMoney, ProjectileAlert, Poison, Acid, Slow, Warp, Common, CoreBreak, CoreHeal, ProjectileThrow
    };
    public enum LoudLoopType {
        Fire, Ice
    }

    [System.Serializable]
    public class Effect {
        public GameObject prefab;
        public Vector3 offset;
    }

    [System.Serializable]
    public class Friends {
        public GameObject prefab;
        public GameObject instance;
        public Transform trans;
        public FriendsBase fBase;
        public int hpSave;
        public GameObject slotObject;
        public UIImageFillController uiCon;
        public bool isSeparated;
        public ActivateObject separatedMark;
    }

    [System.Serializable]
    public class Achievement {
        public Canvas canvas;
        public TMP_Text stageName;
        public TMP_Text recommendedLevel;
        public TMP_Text title;
        public TMP_Text rate;
        public TMP_ColorGradient normalColor;
        public TMP_ColorGradient completeColor;
        public Image clearDifficulty;
        public Vector2[] defaultTextPos;
        public Vector2[] clearedTextPos;
    }

    [System.Serializable]
    public class ParkmanSettings {
        public GameObject[] offObj;
        public GameObject[] onObj;
        public TMP_Text zanki;
        public TMP_Text score;
        public TMP_Text ball;
        public Image supermanGauge;
        public Image alert;
        public TMP_ColorGradient scoreNormalColor;
        public TMP_ColorGradient scoreHardColor;
        public bool activated;
    }

    [System.Serializable]
    public class BossResult {
        public Canvas parentCanvas;
        public Canvas canvas;
        public TMP_Text text1;
        public TMP_Text text2;
        public Image[] iconImages;
        public Sprite[] difficultySprites;
        public Sprite sinWRSprite;
        public Sprite[] minmiSprite;
        public int difficultyMin;
        public int friendsCallingCost;
        public int friendsCallingCount;
        public int useItemCount;
        public bool minmiBlueFlag;
        public bool minmiPurpleFlag;
        public bool minmiBlackFlag;
        public bool minmiSilverFlag;
        public bool minmiGoldenFlag;
    }

    public class BuffSlot {
        public Sprite sprite;
        public int effectIndex;
        public bool effectIsCenter;
        public GameObject slotObject;
        public Image slotImage;
        public float remainTime;
        public BuffSlot(int effectIndex, Sprite sprite, bool effectIsCenter = false) {
            this.effectIndex = effectIndex;
            this.sprite = sprite;
            this.effectIsCenter = effectIsCenter;
            slotObject = null;
            slotImage = null;
            remainTime = 0;
        }
    }

    public class DamageInfo {
        public double time;
        public CharacterBase attacker;
        public CharacterBase receiver;
        public DamageInfo(double time = -10000.0, CharacterBase attacker = null, CharacterBase receiver = null) {
            this.time = time;
            this.attacker = attacker;
            this.receiver = receiver;
        }
    }

    public class KillInfo {
        public float timer;
        public EnemyBase attacker;
        public KillInfo(EnemyBase attacker = null, float timer = 0f) {
            this.attacker = attacker;
            this.timer = timer;
        }
    }

    public class LoudLoopInfo {
        public LoudLoopType type;
        public Transform instanceTransform;
        public AudioSource audioSource;
        public LoudLoopInfo(LoudLoopType type, Transform instanceTransform, AudioSource audioSource) {
            this.type = type;
            this.instanceTransform = instanceTransform;
            this.audioSource = audioSource;
        }
    }

    public class FadeTextSet {
        public TMP_Text text;
        public RectTransform rectTrans;
        public float lifeTime;
        public float alphaRate;
        public FadeTextSet(TMP_Text text, RectTransform rectTrans, float lifeTime, float alphaRate = 1f) {
            this.text = text;
            this.rectTrans = rectTrans;
            this.lifeTime = lifeTime;
            this.alphaRate = alphaRate;
        }
    }

    public enum JustDodgeType { Jump, DodgeStep, QuickEscape };
    public enum ActionType { None, Search, Talk, Read, Sit, StandUp, UseHoldDown };

    public const float warpSqrDist = 900f;
    public const float warpSqrDistBoss = 2500f;
    public const float warpDistance = 30f;
    public const float warpDistanceBoss = 50f;
    public const float evadeLimitDistance = 25f;
    public const float evadeLimitDistanceBoss = 40f;
    public const int buffSlotMax = 10;
    public const int jmID = 70;
    public const int jm3ID = 71;
    public const int jm5ID = 72;
    public const int colorTypeHeal = 1;
    public const int riskyCost = 12;
    public const int theoreticalCostMax = 87;
    public const int specialFriendsID = 31;
    public const int minmiBlueIndex = 0;
    public const int minmiRedIndex = 1;
    public const int minmiPurpleIndex = 2;
    public const int minmiBlackIndex = 3;
    public const int minmiSilverIndex = 4;
    public const int minmiGoldenIndex = 5;

    public GameObject[] playerPrefab;
    public int[] playerFaceID;
    public int playerIndex;
    public Friends[] friends;
    public Transform slotPanel;
    public GameObject slotPrefab;
    public Canvas hpstCanvas;
    public Canvas friendsCanvas;
    public Image expFill;
    public TMP_Text expText;
    public TMP_Text levelLimitText;
    public PhasedIconsMother pIconsMother;
    public Canvas readyCanvas;
    public UIShiny readyShiny;
    public Image gutsFill;
    public GameObject gameOver;
    public Transform statusSlotPanel;
    public GameObject statusSlotPrefab;
    public Canvas bossHP;
    public Canvas gold;
    public TMP_Text goldNum;
    public Canvas messageLogCanvas;
    public Canvas analyzerCanvas;
    public TMP_Text[] analyzerTexts;
    public GridLayoutGroup friendsGrid;
    public Camera mapCamera;
    public Canvas numberCanvas;
    public Transform[] damageParentsFriends;
    public Transform[] damageParentsPlayer;
    public Transform justDodgeParent;
    public Transform gutsNormalParent;
    public Transform gutsLastParent;
    public Transform getExpParent;
    public Canvas bossTimeCanvas;
    public Image disableFriendsEffectIcon;
    public TMP_Text bossTimeText;
    public TMP_ColorGradient bossTimeNormal;
    public TMP_ColorGradient bossTimeEnd;
    public Achievement achievement;
    public CommandSlot generalCommandSlot;
    public ParkmanSettings parkmanSettings;
    public BossResult bossResult;
    public float[] wrSpeedBiasArray;
    public Image staminaAlertImage;
    public GameObject[] weakPointPrefab;
    public TMP_Text actionTypeText;
    public Image actionTypeImage;
    public Image helpMarkImage;
    public Sprite[] helpMarkSprites;
    public Image helpEffectImage;
    public Image[] assistExpertMarks;
    public Vector2 assistExpertOrigin;
    public Vector2 assistExpertInterval;
    public Canvas playTimeCanvas;
    public TMP_Text playTimeText;

    [System.NonSerialized]
    public GameObject playerObj;
    [System.NonSerialized]
    public Transform playerTrans;
    [System.NonSerialized]
    public Transform playerLookAt;
    [System.NonSerialized]
    public Transform playerSearchTarget;
    [System.NonSerialized]
    public GameObject playerTarget;
    [System.NonSerialized]
    public Transform playerTargetTrans;
    [System.NonSerialized]
    public Transform playerTargetTransForCamera;
    [System.NonSerialized]
    public CharacterBase playerTargetCBase;
    [System.NonSerialized]
    public Transform playerAudioListener;
    [System.NonSerialized]
    public int autoAim;
    [System.NonSerialized]
    public Vector3 lastLandingPosition;
    [System.NonSerialized]
    public DamageInfo damageRecord;
    [System.NonSerialized]
    public DamageInfo dodgeRecord;
    [System.NonSerialized]
    public KillInfo[] killRecord = new KillInfo[GameManager.friendsMax];
    [System.NonSerialized]
    public int playerDamageSum;
    [System.NonSerialized]
    public int playerDamageCount;
    [System.NonSerialized]
    public bool inertIFR;
    [System.NonSerialized]
    public bool equipChanged;
    [System.NonSerialized]
    public int messageSoundCount;
    [System.NonSerialized]
    public bool isClimbing;
    [System.NonSerialized]
    public int specialHealReported;
    [System.NonSerialized]
    public int defeatEnemyCount;
    [System.NonSerialized]
    public int defeatEnemyCountSSRaw;
    [System.NonSerialized]
    public float staminaBorder;
    [System.NonSerialized]
    public int sandstarRawActivated;
    [System.NonSerialized]
    public int costSumSave;
    [System.NonSerialized]
    public int friendsCountSave;
    [System.NonSerialized]
    public float riskyIncrease = 1f;
    [System.NonSerialized]
    public float riskyDecrease = 1f;
    [System.NonSerialized]
    public float riskyIncSqrt = 1f;
    [System.NonSerialized]
    public float riskyDecSqrt = 1f;
    [System.NonSerialized]
    public float riskyDecSquare = 1f;
    [System.NonSerialized]
    public float riskyDec15 = 1f;
    [System.NonSerialized]
    public float riskyDec25 = 1f;
    [System.NonSerialized]
    public float riskyDecCubicRoot = 1f;
    [System.NonSerialized]
    public float riskyInc15 = 1f;
    [System.NonSerialized]
    public int levelLimit;
    [System.NonSerialized]
    public int friendsEffectDisabled;
    [System.NonSerialized]
    public bool isBossBattle;
    [System.NonSerialized]
    public Vector3 playerCameraPosition;
    [System.NonSerialized]
    public bool bossTimeContinuing;
    [System.NonSerialized]
    public bool bossDefeatedFlag;
    [System.NonSerialized]
    public int sandcatFindNum;
    [System.NonSerialized]
    public int japarimanShortage;
    [System.NonSerialized]
    public bool showGoldContinuous;
    [System.NonSerialized]
    public FriendsBase specialFriendsBase;
    [System.NonSerialized]
    public int multiBossCount;
    [System.NonSerialized]
    public int stealFailedCount;
    [System.NonSerialized]
    public EnemyBase playerTargetEnemy;
    [System.NonSerialized]
    public bool mogisenMultiPlay;
    [System.NonSerialized]
    public bool hellMode;

    private List<int> actionEnemy = new List<int>(256);

    private PlayerController pConInternal;
    public PlayerController pCon {
        get {
            if (!pConInternal) {
                GameObject[] poTemps = GameObject.FindGameObjectsWithTag("Player");
                for (int i = 0; i < poTemps.Length; i++) {
                    pConInternal = poTemps[i].GetComponent<PlayerController>();
                    if (pConInternal) {
                        playerObj = poTemps[i];
                        break;
                    }
                }
                if (!playerObj) {
                    if (GameManager.Instance) {
                        playerIndex = GetOriginallyPlayerIndex;
                    }
                    playerObj = Instantiate(playerPrefab[playerIndex]);
                    pConInternal = playerObj.GetComponent<PlayerController>();
                }
                playerTrans = playerObj.transform;
                playerLookAt = pConInternal.lookAtTarget;
                playerSearchTarget = pConInternal.searchTarget[0].transform;
                playerAudioListener = pConInternal.audioListener;
                lastLandingPosition = playerTrans.position;
            }
            return pConInternal;
        }
        set {
            pConInternal = value;
        }
    }

    StringBuilder sb = new StringBuilder();
    UIImageFillController bossHPController;
    float lastPlayerAttackTimeRemain;
    int expSave = -1;
    float sandstar;
    int id = 0;
    bool playerDead;
    BuffSlot[] buffSlot;
    float checkFriendsDistanceTimeRemain = 0f;
    float goldTimeRemain = 0f;
    float sneezeTimeRemain = 0f;
    float definitelyDodgeTimeRemain;
    bool buffSlotInitialized = false;
    int agentDelayFrameRemain = 0;
    bool agentDelayToEnable = false;
    int sfxPlayed = -1;
    const int loudSoundMax = 18;
    float[] loudSoundVolumeRate = new float[loudSoundMax];
    float[] loudSoundVolumeIncrease = new float[loudSoundMax];
    const float loudSoundVolumeMin = 0.25f;
    const int loudLoopMax = 2;
    LoudLoopInfo[] loudLoopInfo = new LoudLoopInfo[64];
    const float loudLoopVolumeMin = 0.2f;
    const float loudLoopVolumeMax = 0.8f;
    LayerMask appearPosLayerMask;
    CharacterBase.Command generalCommand = CharacterBase.Command.Default;
    bool analyzerEnabled = false;
    int analyzerLanguage = -1;
    int analyzerEnemyID = -1;
    int analyzerNowHP;
    int analyzerMaxHP;
    int analyzerAttackPower;
    float analyzerAttackMultiplier;
    bool analyzerStateIsAttack;
    int analyzerDefense;
    int analyzerLevel;
    bool analyzerIsBoss;
    int analyzerDefeated;
    Camera mainCamera;
    bool numberUiInitialized;
    GameObject[] damagePrefab;
    List<FadeTextSet>[] damageList = new List<FadeTextSet>[damageCanvasMax];
    List<FadeTextSet>[] dodgeList = new List<FadeTextSet>[dodgeTextTypeMax];
    GameObject justDodgeHighPrefab;
    GameObject justDodgeMiddlePrefab;
    GameObject justDodgeLowPrefab;
    GameObject gutsNormalPrefab;
    GameObject gutsLastPrefab;
    GameObject getExpPrefab;
    GameObject defeatRemainPrefab;
    GameObject fieldBuffHPPrefab;
    GameObject fieldBuffSTPrefab;
    GameObject fieldBuffAttackPrefab;
    int[] friendsOrder = new int[32];
    int[] friendsGridSize = new int[33];
    float[] friendsGridScale = new float[33];
    float friendsGridScaleSave = 1f;
    bool friendsSlotChanged;
    int bossTimeInteger;
    float bossTimeDecimal;
    int bossTimeState;
    float nowWRSpeedBias = 1f;
    int t_JustDodgeCount;
    int t_IbisSongDefeatCount;
    int t_TsuchinokoDefeatCount;
    int t_MooseBreakCount;
    int t_PPPDefeatCount;
    int t_GlaucusDodgeCount;
    int t_RedFoxDefeatCount;
    int t_SnowTowerCount;
    int t_CampoFlickerCount;
    int t_GiraffeDefeatCount;
    bool t_QueenSafeArea;
    int t_PaintedWolfCount;
    int t_FennecDefeatCount;
    int t_AustralopithecusCount;

    Player playerInput;
    int comTargetIndex;
    GameObject comTargetSlotObj;
    Vector2 axisInput;
    bool neutral;
    float moveCursorTimeRemain;
    int playTimeSave = -1;

    public const int playerIndexDefault = 0;
    public const int playerIndexParkman = 1;
    public const int playerIndexHyper = 2;
    public const int playerIndexAnother = 3;
    public const int playerIndexAnotherEscape = 4;
    const float interval = 0.0916f;
    const string slash = " / ";
    const string space = " ";
    const string asterisk = "*";
    const string slotHeader = "slot";
    const string analyzerString_CellienName = "CELLIEN_NAME_";
    const string analyzerString_HP = "ANALYZE_HP";
    const string analyzerString_Attack = "ANALYZE_ATTACK";
    const string analyzerString_Defense = "ANALYZE_DEFENSE";
    const string analyzerString_Level = "ANALYZE_LEVEL";
    const string analyzerString_Defeated = "ANALYZE_DEFEATED";
    const string analyzerString_BossLevel = "ANALYZE_BOSSLEVEL_";
    const int sneezeFriendsID = 28;
    const int raccoonID = 29;
    const int fennecID = 30;
    const int sacrificeID = 31;
    const int safetyBombID = 1;
    const int damageColorTypeMax = 9;
    const int damageCanvasMax = 18;
    const int damageSoundTypeMax = 6;
    const int damageSoundMax = 3;
    int[] damageSoundCount = new int[damageSoundTypeMax];
    float mapCameraSizeSave = 80f;
    const float mapCameraDefaultSize = 80f;
    const int mapCameraBigIndex = 6;
    const float lastAttackTimeDuration = 25f;
    const float damageTextLife = 1.2f;
    const int dodgeTextTypeMax = 10;
    const int dodgeText_JustDodgeHigh = 0;
    const int dodgeText_JustDodgeMiddle = 1;
    const int dodgeText_JustDodgeLow = 2;
    const int dodgeText_GutsNormal = 3;
    const int dodgeText_GutsLast = 4;
    const int dodgeText_Exp = 5;
    const int dodgeText_DefeatRemain = 6;
    const int dodgeText_FieldBuffHP = 7;
    const int dodgeText_FieldBuffST = 8;
    const int dodgeText_FieldBuffAttack = 9;
    const float dodgeTextLife = 3f;
    const float dodgeTextAlphaSpeed = 0.34f;

    Ray ray = new Ray();
    RaycastHit raycastHit = new RaycastHit();
    LayerMask fieldLayerMask;
    Vector2 cellSize = Vector2.zero;
    int costSumSaveSave = -1;
    float justDodgeAmount = 1f;
    bool bossTimeInitialized;
    Vector2 damageFrameMin;
    Vector2 damageFrameMax;
    bool friendsRestoreProcessed;
    bool recordsInitialized;
    GameObject hpAlertInstance;
    float checkLoudLoopInterval;
    float staminaAlertTimeRemain;
    float staminaAlertImgInterval;
    float staminaAlertSEInterval;
    GameObject staminaAlertInstance;
    GameObject actionTypeHostage;
    ActionType actionTypeSave;
    float itemAutomaticUseInterval;
    bool graphAutoChecked;
    float graphAutoInterval;
    float helpMarkTime;
    float helpEffectTime;
    Color helpEffectColor = new Color(1f, 1f, 0.6f, 0f);
    Vector2 helpEffectSize;
    int assistExpertStateSave = -1;
    
    static readonly float[] damageSoundVolumeArray = new float[damageSoundMax] { 1f, 0.5f, 0.25f };
    static readonly int[] damageColorToSound = new int[damageColorTypeMax] { 0, 1, 2, 3, 4, 2, 3, 5, 5 };
    static readonly int[] clawID = new int[] { 3, 4, 5 };
    static readonly Vector3 vecZero = Vector3.zero;
    static readonly Vector3 vecUp = Vector3.up;
    static readonly Vector3 vecDown = Vector3.down;
    static readonly Vector3 vecOne = Vector3.one;
    static readonly Color colorWhite = new Color(1f, 1f, 1f, 1f);
    static readonly Color colorRed = new Color(1f, 0.5f, 0.5f);
    static readonly Color colorGray = new Color(0.5f, 0.5f, 0.5f);
    static readonly Color colorBlack = new Color(0f, 0f, 0f);
    static readonly Vector2Int vec2IntZero = Vector2Int.zero;
    static readonly Vector2 vec2Zero = Vector2.zero;
    static readonly Vector3 mapCameraDefaultPosition = new Vector3(0f, 200f, 0f);
    static readonly Vector3 mapCameraDefaultRotation = new Vector3(90f, 0f, 180f);
    static readonly Rect[] mapCameraRects = new Rect[7] {
        new Rect(0.66f, 0.0f, 0.5f, 0.5f),
        new Rect(-0.16f, 0.0f, 0.5f, 0.5f),
        new Rect(0.66f, 0.23f, 0.5f, 0.5f),
        new Rect(-0.16f, 0.23f, 0.5f, 0.5f),
        new Rect(0.66f, 0.46f, 0.5f, 0.5f),
        new Rect(-0.16f, 0.46f, 0.5f, 0.5f),
        new Rect(0f, 0f, 1f, 1f)
    };
    static readonly Vector2[] goldCanvasPosition = new Vector2[7] {
        new Vector2(-230, 56),
        new Vector2(12, 200),
        new Vector2(-230, 0),
        new Vector2(12, 0),
        new Vector2(-230, -92),
        new Vector2(12, -92),
        new Vector2(-100, -92)
    };
    static readonly Vector2[] goldCanvasAnchor = new Vector2[7] {
        new Vector2(1, 0),
        new Vector2(0, 0),
        new Vector2(1, 0.5f),
        new Vector2(0, 0.5f),
        new Vector2(1, 1),
        new Vector2(0, 1),
        new Vector2(0.5f, 1)
    };
    static readonly float[] sizeMulArray = new float[11] { 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f };
    static readonly int[] automaticItemCandidates = new int[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 50, 51, 52, 55, 56, 59, 60, 61, 62, 63, 64, 65 };
    static readonly int[] automaticItemConditions = new int[] { 11, 09, 11, 11, 11, 11, 09, 11, 11, 03, 03, 03, 01, 03, 01, 07, 07, 07, 07, 07, 07 };
    int[] automaticItemArray = new int[automaticItemCandidates.Length];
    // 1=Enemy 2=Boss 4=Kaban 8=Buff
    static readonly int[] autoGraphOther = new int[] { 42, 43 };
    static readonly int[] autoGraphComplex = new int[] { 41, 44, 45 };
    static readonly float[] log2Array = new float[] {
        0.000000f, 0.000000f, 1.000000f, 1.584963f,
        2.000000f, 2.321928f, 2.584963f, 2.807355f,
        3.000000f, 3.169925f, 3.321928f, 3.459432f,
        3.584963f, 3.700440f, 3.807355f, 3.906891f,
        4.000000f, 4.087463f, 4.169925f, 4.247928f,
        4.321928f, 4.392317f, 4.459432f, 4.523562f,
        4.584963f, 4.643856f, 4.700440f, 4.754888f,
        4.807355f, 4.857981f, 4.906891f, 4.954196f
    };

    public static readonly int[] extraFriendsID = new int[] { 31, 32, 33 };
    [System.NonSerialized]
    public int extraFriendsIndex;

    protected override void Awake() {
        base.Awake();
        if (!buffSlotInitialized) {
            LoadBuffSprites();
        }
        InitLoudSound();
        InitRecords();
        appearPosLayerMask = LayerMask.GetMask("Default", "Field", "EnemyCollision", "SecondField", "ThirdField");
        fieldLayerMask = LayerMask.GetMask("Field");
        SetFriendsGridArray();
        damageFrameMin.x = Screen.width * 0.02f;
        damageFrameMin.y = 0f;
        damageFrameMax.x = Screen.width * 0.98f;
        damageFrameMax.y = Screen.height * 0.9f;
    }

    public int GetOriginallyPlayerIndex {
        get {
            switch (GameManager.Instance.save.playerType) {
                case GameManager.playerType_Normal:
                    return playerIndexDefault;
                case GameManager.playerType_Another:
                    return playerIndexAnother;
                case GameManager.playerType_Hyper:
                    return playerIndexHyper;
                case GameManager.playerType_AnotherEscape:
                    return playerIndexAnotherEscape;
                default:
                    return playerIndexDefault;
            }
        }
    }

    void InitRecords() {
        if (recordsInitialized) {
            damageRecord.time = -10000.0;
            damageRecord.attacker = null;
            damageRecord.receiver = null;
            dodgeRecord.time = -10000.0;
            dodgeRecord.attacker = null;
            dodgeRecord.receiver = null;
            for (int i = 0; i < killRecord.Length; i++) {
                killRecord[i].attacker = null;
                killRecord[i].timer = 0f;
            }
        } else {
            damageRecord = new DamageInfo(-10000.0, null, null);
            dodgeRecord = new DamageInfo(-10000.0, null, null);
            for (int i = 0; i < killRecord.Length; i++) {
                killRecord[i] = new KillInfo(null, 0f);
            }
            for (int i = 0; i < loudLoopInfo.Length; i++) {
                loudLoopInfo[i] = new LoudLoopInfo(LoudLoopType.Fire, null, null);
            }
            recordsInitialized = true;
        }
    }

    public void SetKillRecord(EnemyBase enemyBase) {
        if (recordsInitialized) {
            for (int i = 0; i < killRecord.Length; i++) {
                if (killRecord[i].attacker == null) {
                    killRecord[i].attacker = enemyBase;
                    killRecord[i].timer = 0.5f;
                    break;
                }
            }
        }
    }

    public void SetNewPlayer(int newPlayerIndex = 0) {
        Vector3 defaultPosition = Vector3.zero;
        Quaternion defaultRotation = Quaternion.identity;
        if (playerTrans) {
            defaultPosition = playerTrans.position;
            defaultRotation = playerTrans.rotation;
        }
        if (playerObj) {
            Destroy(playerObj);
        }
        playerIndex = newPlayerIndex;
        playerObj = Instantiate(playerPrefab[playerIndex], defaultPosition, defaultRotation);
        pConInternal = playerObj.GetComponent<PlayerController>();
        playerTrans = playerObj.transform;
        playerLookAt = pConInternal.lookAtTarget;
        playerSearchTarget = pConInternal.searchTarget[0].transform;
        playerAudioListener = pConInternal.audioListener;
        lastLandingPosition = playerTrans.position;
        if (CameraManager.Instance) {
            CameraManager.Instance.SetHorizontalDamping(GameManager.Instance.save.config[GameManager.Save.configID_CameraTurningSpeed]);
        }
        if (PauseController.Instance) {
            PauseController.Instance.ResetPlayerController();
        }
        if (pIconsMother) {
            if (playerIndex == playerIndexHyper) {
                pIconsMother.SetSpriteType(1);
            } else {
                pIconsMother.SetSpriteType(0);
            }
        }
        if (StageManager.Instance) {
            StageManager.Instance.ResetHomeFloorNumber(GameManager.Instance.IsPlayerAnother);
        }
    }

    public void PlayerDied(bool gameOverEffectEnabled = true) {
        if (!playerDead) {
            playerDead = true;
            DestroyStatusSlot();
            if (gameOverEffectEnabled) {
                Instantiate(gameOver);
            }
            PauseController.Instance.pauseEnabled = false;
            GameManager.Instance.gameOverFlag = true;
            t_IbisSongDefeatCount = 0;
            t_TsuchinokoDefeatCount = 0;
        }
    }
    
    void Start() {
        if (FootstepManager.Instance) {
            FootstepManager.Instance.Init();
            FootstepManager.Instance.LoadFiles();
        }
        levelLimit = GameManager.Instance.save.config[GameManager.Save.configID_LevelLimit];
        autoAim = (GameManager.Instance.save.config[GameManager.Save.configID_LockonReset] <= 1 ? 1 : 0);
        UpdateMapCameraRect();
        UpdateGoldPos();
        UpdateHPSTCanvasSize();
        UpdateFriendsGridScale();
        UpdateLevelLimit();
        UpdateFriendsEffectDisabled();
        UpdateHellMode();
        UpdateExpGage();
        pCon = pCon;
        playerInput = GameManager.Instance.playerInput;
        RestoreFriends();
        playerDead = false;
        UpdateSandstarMax();
        if (bossHP) {
            bossHPController = bossHP.gameObject.GetComponent<UIImageFillController>();
        }
        if (StageManager.Instance) {
            StageManager.Instance.ResetHomeFloorNumber(GameManager.Instance.IsPlayerAnother);
        }
        GetMainCamera();
        GetNumberUIPrefab();
        if (pIconsMother) {
            if (playerIndex == playerIndexHyper) {
                pIconsMother.SetSpriteType(1);
            } else {
                pIconsMother.SetSpriteType(0);
            }
        }
    }

    void InitLoudSound() {
        for (int i = 0; i < loudSoundVolumeRate.Length; i++) {
            loudSoundVolumeRate[i] = 1f;
        }
        loudSoundVolumeIncrease[(int)LoudSoundType.Burst] = 0.75f / 0.5f;
        loudSoundVolumeIncrease[(int)LoudSoundType.WallBreak] = 0.75f / 0.375f;
        loudSoundVolumeIncrease[(int)LoudSoundType.EnemyLevelUp] = 0.75f / 0.5f;
        loudSoundVolumeIncrease[(int)LoudSoundType.EnemyLevelDown] = 0.75f / 0.5f;
        loudSoundVolumeIncrease[(int)LoudSoundType.WaterSplash] = 0.75f / 0.375f;
        loudSoundVolumeIncrease[(int)LoudSoundType.EnemyDead] = 0.75f / 0.375f;
        loudSoundVolumeIncrease[(int)LoudSoundType.EnemyWR] = 0.75f / 0.375f;
        loudSoundVolumeIncrease[(int)LoudSoundType.GetItem] = 0.75f / 0.14f;
        loudSoundVolumeIncrease[(int)LoudSoundType.GetMoney] = 0.75f / 0.07f;
        loudSoundVolumeIncrease[(int)LoudSoundType.ProjectileAlert] = 0.75f / 0.15f;
        loudSoundVolumeIncrease[(int)LoudSoundType.Poison] = 0.75f / 0.03f;
        loudSoundVolumeIncrease[(int)LoudSoundType.Acid] = 0.75f / 0.03f;
        loudSoundVolumeIncrease[(int)LoudSoundType.Slow] = 0.75f / 0.03f;
        loudSoundVolumeIncrease[(int)LoudSoundType.Warp] = 0.75f / 0.03f;
        loudSoundVolumeIncrease[(int)LoudSoundType.Common] = 0.75f / 0.03f;
        loudSoundVolumeIncrease[(int)LoudSoundType.CoreBreak] = 0.75f / 0.03f;
        loudSoundVolumeIncrease[(int)LoudSoundType.CoreHeal] = 0.75f / 0.03f;
        loudSoundVolumeIncrease[(int)LoudSoundType.ProjectileThrow] = 0.75f / 0.03f;
    }
    
    void Update() {
        float deltaTimeCache = Time.deltaTime;
        float unscaledDeltaTimeCache = Time.unscaledDeltaTime;
        sfxPlayed = -1;
        CheckCostSum();
        UpdateSandstarDecrease();
        if (!playerDead) {
            checkFriendsDistanceTimeRemain -= deltaTimeCache;
            if (checkFriendsDistanceTimeRemain <= 0f) {
                checkFriendsDistanceTimeRemain = interval;
                bool soundEnabled = true;
                float sqrDist;
                float condDist = (StageManager.Instance && StageManager.Instance.isBossFloor ? warpSqrDistBoss : warpSqrDist);
                for (int i = 0; i < friends.Length; i++) {
                    if (playerObj && friends[i].instance && friends[i].fBase) {
                        sqrDist = GetPlayerBetweenFriendsSqrMag(i);
                        if (friends[i].isSeparated) {
                            if (sqrDist < condDist && !Physics.Linecast(friends[i].trans.position + vecUp, playerTrans.position + vecUp, fieldLayerMask, QueryTriggerInteraction.Ignore)) {
                                SetFriendsSeparated(i, false);
                            }
                        } else {
                            if (sqrDist > condDist && friends[i].fBase.GetAutoGatherEnabled() && friends[i].fBase.GetNowHP() > 0 && pCon && !pCon.GetDrowning()) {
                                WarpToPlayerPos(i, soundEnabled);
                                soundEnabled = false;
                            }
                        }
                    }
                }
            }
            if (lastPlayerAttackTimeRemain > 0) {
                lastPlayerAttackTimeRemain -= deltaTimeCache;
            }
            if (PauseController.Instance.pauseEnabled && !PauseController.Instance.pauseGame && Time.timeScale > 0f && playerIndex != playerIndexParkman && pCon && pCon.controlEnabled) {
                ChangeCommand();
            }
            UpdateStatusSlot();
            UpdateAnalyzer();
        }
        if (!bossHPController.cBase && bossHP.enabled) {
            bossHP.enabled = false;
        }
        if (goldTimeRemain > 0f) {
            goldTimeRemain -= deltaTimeCache;
        }
        if (gold.enabled) {
            if (GameManager.Instance.save.config[GameManager.Save.configID_GoldPos] >= goldCanvasPosition.Length || (goldTimeRemain <= 0 && !showGoldContinuous)) {
                gold.enabled = false;
            }
        }
        if (agentDelayFrameRemain > 0) {
            agentDelayFrameRemain--;
            if (agentDelayFrameRemain == 0) {
                SetFriendsAgentEnabledInternal(agentDelayToEnable);
            }
        }
        if (messageLogCanvas.enabled != (GameManager.Instance.save.config[GameManager.Save.configID_ShowMessageLog] != 0)) {
            messageLogCanvas.enabled = (GameManager.Instance.save.config[GameManager.Save.configID_ShowMessageLog] != 0);
        }
        if (friendsSlotChanged) {
            UpdateFriendsGridScale();
            friendsSlotChanged = false;
        }
        if (deltaTimeCache > 0) {
            if (sneezeTimeRemain > 0f) {
                sneezeTimeRemain -= deltaTimeCache;
            }
            if (definitelyDodgeTimeRemain > 0f) {
                definitelyDodgeTimeRemain -= deltaTimeCache;
            }
            for (int i = 0; i < killRecord.Length; i++) {
                if (killRecord[i].attacker != null) {
                    killRecord[i].timer -= deltaTimeCache;
                    if (killRecord[i].timer <= 0f) {
                        killRecord[i].attacker.LevelUp();
                        killRecord[i].attacker = null;
                    }
                }
            }
        }
        for (int i = 0; i < loudSoundMax; i++) {
            if (loudSoundVolumeRate[i] != 1f) {
                loudSoundVolumeRate[i] = Mathf.Clamp(loudSoundVolumeRate[i] + unscaledDeltaTimeCache * loudSoundVolumeIncrease[i], loudSoundVolumeMin, 1.0f);
            }
        }
        if (playerAudioListener != null && recordsInitialized) {
            checkLoudLoopInterval -= unscaledDeltaTimeCache;
            if (checkLoudLoopInterval <= 0f) {
                checkLoudLoopInterval = 0.025f;
                for (int type = 0; type < loudLoopMax; type++) {
                    float minDist = float.MaxValue;
                    int minIndex = -1;
                    for (int i = 0; i < loudLoopInfo.Length; i++) {
                        if (loudLoopInfo[i].instanceTransform && loudLoopInfo[i].audioSource && (int)loudLoopInfo[i].type == type) {
                            loudLoopInfo[i].audioSource.volume = loudLoopVolumeMin;
                            float distTemp = (loudLoopInfo[i].instanceTransform.position - playerAudioListener.position).sqrMagnitude;
                            if (distTemp < minDist) {
                                minDist = distTemp;
                                minIndex = i;
                            }
                        }
                    }
                    if (minIndex >= 0) {
                        loudLoopInfo[minIndex].audioSource.volume = loudLoopVolumeMax;
                    }
                }
            }
        }
        if (actionTypeSave != ActionType.None && actionTypeHostage == null) {
            SetActionType(ActionType.None, null);
        }
        //ItemAutomaticUse
        if (GameManager.Instance.save.config[GameManager.Save.configID_ItemAutomaticUse] == 1) {
            itemAutomaticUseInterval -= deltaTimeCache * (pCon && pCon.GetNowHP() < GetGutsBorder() ? 2f : 1f);
            if (itemAutomaticUseInterval <= 0f && playerTarget) {
                bool enemyFlag = GetActionEnemyCount() >= 4;
                if (enemyFlag || isBossBattle) {
                    bool kabanFlag = GetFriendsExist(1, true);
                    bool noBuffFlag = GetBuffCount(true) == 0;
                    for (int i = 0; i < automaticItemArray.Length; i++) {
                        int cond = automaticItemConditions[i];
                        if ( (((cond & 1) != 0 && enemyFlag) || ((cond & 2) != 0 && isBossBattle)) && ((cond & 4) == 0 || kabanFlag) && ((cond & 8) == 0 || noBuffFlag) ) {
                            automaticItemArray[i] = automaticItemCandidates[i];
                        } else {
                            automaticItemArray[i] = -1;
                        }
                    }
                    for (int i = automaticItemArray.Length - 1; i > 0; i--) {
                        int r = Random.Range(0, i + 1);
                        int tmp = automaticItemArray[i];
                        automaticItemArray[i] = automaticItemArray[r];
                        automaticItemArray[r] = tmp;
                    }
                    for (int i = 0; i < automaticItemArray.Length; i++) {
                        if (automaticItemArray[i] >= 0 && GameManager.Instance.save.HaveSpecificItem(automaticItemArray[i])) {
                            PauseController.Instance.ItemAutomaticUse(automaticItemArray[i]);
                            itemAutomaticUseInterval = 40f;
                            break;
                        }
                    }
                }
            }
        } else {
            graphAutoChecked = true;
        }
        if (!graphAutoChecked && !SceneChange.Instance.GetIsProcessing && !PauseController.Instance.pauseGame) {
            graphAutoInterval -= Time.deltaTime;
            if (graphAutoInterval <= 0f) {
                graphAutoChecked = true;
                if (StageManager.Instance && StageManager.Instance.IsGraphAutoFloor) {
                    for (int i = 0; i < autoGraphOther.Length; i++) {
                        if (GameManager.Instance.save.HaveSpecificItem(autoGraphOther[i])) {
                            PauseController.Instance.ItemAutomaticUse(autoGraphOther[i]);
                        }
                    }
                    if (StageManager.Instance.IsGraphComplexFloor) {
                        for (int i = 0; i < autoGraphComplex.Length; i++) {
                            if (GameManager.Instance.save.HaveSpecificItem(autoGraphComplex[i])) {
                                PauseController.Instance.ItemAutomaticUse(autoGraphComplex[i]);
                                break;
                            }
                        }
                    }
                }
            }
        }
        if (TrophyManager.Instance) {
            if (t_IbisSongDefeatCount > 0 && (playerDead || !GetFriendsExist(5))) {
                t_IbisSongDefeatCount = 0;
            }
            if (t_TsuchinokoDefeatCount > 0 && (playerDead || !GetFriendsExist(8))) {
                t_TsuchinokoDefeatCount = 0;
            }
            if (t_MooseBreakCount > 100 && (playerDead || !GetFriendsExist(11))) {
                t_MooseBreakCount = 0;
            }
            if (t_PPPDefeatCount > 0 && (playerDead || !GetFriendsExist(16) || !GetFriendsExist(17) || !GetFriendsExist(18) || !GetFriendsExist(19) || !GetFriendsExist(20))) {
                t_PPPDefeatCount = 0;
            }
            if (t_GlaucusDodgeCount > 0 && playerDead) {
                t_GlaucusDodgeCount = 0;
            }
            if (t_RedFoxDefeatCount > 0 && (playerDead || !GetFriendsExist(21))) {
                t_RedFoxDefeatCount = 0;
            }
            if (t_SnowTowerCount > 0 && playerDead) {
                t_SnowTowerCount = 0;
            }
            if (t_CampoFlickerCount > 0 && (playerDead || !GetFriendsExist(23))) {
                t_CampoFlickerCount = 0;
            }
            if (t_GiraffeDefeatCount > 0 && (playerDead || !GetFriendsExist(25))) {
                t_GiraffeDefeatCount = 0;
            }
            if (t_QueenSafeArea && !isBossBattle) {
                t_QueenSafeArea = false;
            }
            if (t_PaintedWolfCount > 0 && (playerDead || !GetFriendsExist(28))) {
                t_PaintedWolfCount = 0;
            }
            if (t_FennecDefeatCount > 0 && (playerDead || !GetFriendsExist(30))) {
                t_FennecDefeatCount = 0;
            }
            if (t_AustralopithecusCount > 0 && playerDead) {
                t_AustralopithecusCount = 0;
            }
        }
    }

    private void LateUpdate() {
        messageSoundCount = 0;
        if (specialHealReported > 0) {
            specialHealReported--;
        }
        for (int i = 0; i < damageSoundTypeMax; i++) {
            if (damageSoundCount[i] > 0) {
                damageSoundCount[i]--;
            }
        }
        if (GameManager.Instance.state == GameManager.State.play) {
            bool showBossTime = (GameManager.Instance.save.config[GameManager.Save.configID_ShowBossTime] != 0 || StageManager.Instance.IsXRayFloor);
            if (bossTimeState == 2 && Time.timeScale > 0f) {
                if (Time.timeScale >= 0.75f) {
                    bossTimeDecimal += Time.unscaledDeltaTime;
                } else {
                    bossTimeDecimal += Time.deltaTime;
                }
                while (bossTimeDecimal >= 1f) {
                    bossTimeInteger += 1;
                    bossTimeDecimal -= 1f;
                }
                if (showBossTime) {
                    SetBossTimeText();
                }
                if (bossResult.difficultyMin > GameManager.Instance.save.difficulty) {
                    bossResult.difficultyMin = GameManager.Instance.save.difficulty;
                }
                if (bossResult.minmiBlueFlag && !GameManager.Instance.minmiBlue) {
                    bossResult.minmiBlueFlag = false;
                }
                if (bossResult.minmiPurpleFlag && !GameManager.Instance.minmiPurple) {
                    bossResult.minmiPurpleFlag = false;
                }
                if (bossResult.minmiBlackFlag && StageManager.Instance && StageManager.Instance.dungeonController && !StageManager.Instance.dungeonController.bossMinmiBlackFlag) {
                    bossResult.minmiBlackFlag = false;
                }
                if (bossResult.minmiSilverFlag && !GameManager.Instance.minmiSilver) {
                    bossResult.minmiSilverFlag = false;
                }
                if (!bossResult.minmiGoldenFlag && GameManager.Instance.minmiGolden) {
                    bossResult.minmiGoldenFlag = true;
                }
            }
            if (showBossTime) {
                if (bossTimeCanvas.enabled != (bossTimeState >= 1)) {
                    bossTimeCanvas.enabled = (bossTimeState >= 1);
                    if (bossTimeCanvas.enabled) {
                        SetBossTimeText();
                    }
                }
            } else {
                if (bossTimeCanvas.enabled) {
                    bossTimeCanvas.enabled = false;
                }
            }
            if (playTimeCanvas.enabled != (GameManager.Instance.save.config[GameManager.Save.configID_ShowPlayTime] != 0)) {
                playTimeCanvas.enabled = (GameManager.Instance.save.config[GameManager.Save.configID_ShowPlayTime] != 0);
            }
            if (playTimeCanvas.enabled) {
                if (playTimeSave != GameManager.Instance.save.totalPlayTime) {
                    playTimeSave = GameManager.Instance.save.totalPlayTime;
                    playTimeText.text = sb.Clear().Append((playTimeSave / 3600).ToString("0")).Append(":").Append((playTimeSave % 3600 / 60).ToString("00")).Append("\'").Append((playTimeSave % 60).ToString("00")).Append("\"").ToString();
                }
            }
            bool showBossResultFlag = (GameManager.Instance.save.config[GameManager.Save.configID_ShowBossResult] != 0);
            if (bossResult.parentCanvas.enabled != showBossResultFlag) {
                bossResult.parentCanvas.enabled = showBossResultFlag;
            }
        }
        if (staminaAlertSEInterval > 0f) {
            staminaAlertSEInterval -= Time.deltaTime;
        }
        if (staminaAlertImage) {
            if (staminaAlertTimeRemain > 0f) {
                if (pCon && pCon.GetNowST() >= Mathf.Max(pCon.GetMaxST() * 0.5f, staminaBorder)) {
                    CancelStaminaAlert();
                } else {
                    staminaAlertTimeRemain -= Time.deltaTime;
                    staminaAlertImgInterval -= Time.deltaTime;
                    if (staminaAlertTimeRemain > 0f && staminaAlertImgInterval <= 0f) {
                        staminaAlertImgInterval = 0.25f;
                        staminaAlertImage.enabled = !staminaAlertImage.enabled;
                    }
                }
            }
            if (staminaAlertTimeRemain <= 0f && staminaAlertImage.enabled) {
                staminaAlertImage.enabled = false;
            }
        }
        if (StageManager.Instance && SceneChange.Instance) {
            int spriteIndex = (StageManager.Instance.IsRandomStage ? 1 : 0);
            if (StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.keyItemRemain > 0 && !SceneChange.Instance.GetIsEnding) {
                helpMarkTime += Time.deltaTime * (MessageUI.Instance ? MessageUI.Instance.GetMessageTimeSpeed() : 1f);
            } else {
                helpMarkTime = 0f;
            }
            bool showHelpMark = (helpMarkTime >= 0.1f);
            if (helpMarkImage.sprite != helpMarkSprites[spriteIndex]) {
                helpMarkImage.sprite = helpMarkSprites[spriteIndex];
            }
            if (helpMarkImage.enabled != showHelpMark) {
                helpMarkImage.enabled = showHelpMark;
                helpEffectImage.enabled = showHelpMark;
                helpEffectTime = 0f;
            }
            if (helpEffectImage.enabled) {
                if (helpEffectTime >= 1f) {
                    helpEffectImage.enabled = false;
                } else {
                    helpEffectColor.a = Mathf.Clamp01(Easing.SineIn(helpEffectTime, 1f, 1.5f, 0f));
                    helpEffectImage.color = helpEffectColor;
                    float sizeTemp = Easing.CubicOut(helpEffectTime, 1f, 20f, 120f);
                    helpEffectSize.x = helpEffectSize.y = sizeTemp;
                    helpEffectImage.rectTransform.sizeDelta = helpEffectSize;
                    helpEffectTime += Time.deltaTime;
                }
            }
        }
        if (assistExpertMarks.Length >= 4) {
            int assistExpertState = 0;
            if (GameManager.Instance.save.config[GameManager.Save.configID_BattleAssist] != 0 && GameManager.Instance.save.difficulty <= GameManager.difficultyNT) {
                assistExpertState += (1 << 0);
            }
            if (GameManager.Instance.save.config[GameManager.Save.configID_GameSpeed] != 0 && GameManager.Instance.save.difficulty <= GameManager.difficultyNT && GameManager.Instance.minmiSilver == false) {
                assistExpertState += (1 << 1);
            }
            if (GameManager.Instance.save.config[GameManager.Save.configID_HellMode] != 0) {
                assistExpertState += (1 << 2);
            }
            if (GameManager.Instance.save.config[GameManager.Save.configID_BossMode] != 0) {
                assistExpertState += (1 << 3);
            }
            if (GameManager.Instance.save.config[GameManager.Save.configID_DisablePassiveSkills] != 0) {
                assistExpertState += (1 << 4);
            }
            if (GameManager.Instance.save.config[GameManager.Save.configID_DisableJustDodge] != 0) {
                assistExpertState += (1 << 5);
            }
            if (GameManager.Instance.save.config[GameManager.Save.configID_DisableInvincibility] != 0) {
                assistExpertState += (1 << 6);
            }
            if (GameManager.Instance.save.config[GameManager.Save.configID_DisableGuts] != 0) {
                assistExpertState += (1 << 7);
            }
            if (GameManager.Instance.save.config[GameManager.Save.configID_DisableExp] != 0) {
                assistExpertState += (1 << 8);
            }
            if (GameManager.Instance.save.config[GameManager.Save.configID_DisableItemDrop] != 0) {
                assistExpertState += (1 << 9);
            }
            if (assistExpertState != assistExpertStateSave) {
                assistExpertStateSave = assistExpertState;
                Vector2 posTemp = assistExpertOrigin;
                for (int i = 0; i < assistExpertMarks.Length; i++) {
                    bool enableTemp = (assistExpertState & (1 << i)) != 0;
                    assistExpertMarks[i].enabled = enableTemp;
                    assistExpertMarks[i].rectTransform.anchoredPosition = posTemp;
                    if (enableTemp) {
                        posTemp += assistExpertInterval;
                    }
                }
            }
        }

        UpdateDamageText();
        UpdateDodgeText();
    }

    void CheckCostSum() {
        costSumSave = GetFriendsCostSum();
        friendsCountSave = GetFriendsCount(false);
        if (costSumSave != costSumSaveSave) {
            costSumSaveSave = costSumSave;
            if (costSumSave > riskyCost) {
                riskyIncrease = (float)costSumSave / riskyCost;
                riskyDecrease = (float)riskyCost / costSumSave;
                riskyIncSqrt = Mathf.Sqrt(riskyIncrease);
                riskyDecSqrt = Mathf.Sqrt(riskyDecrease);
                riskyDec15 = riskyDecrease * riskyDecSqrt;
                riskyDecSquare = riskyDecrease * riskyDecrease;
                riskyDec25 = riskyDecSquare * riskyDecSqrt;
                riskyDecCubicRoot = Mathf.Pow(riskyDecrease, 1f / 3f);
                riskyInc15 = riskyIncrease * riskyIncSqrt;
            } else {
                riskyIncrease = 1f;
                riskyDecrease = 1f;
                riskyIncSqrt = 1f;
                riskyDecSqrt = 1f;
                riskyDecSquare = 1f;
                riskyDec15 = 1f;
                riskyDec25 = 1f;
                riskyDecCubicRoot = 1f;
                riskyInc15 = 1f;
            }
        }
    }

    void SetFriendsGridArray() {
        float rate = (float)Screen.width / Screen.height;
        float baseRate = 1280f / 720f;
        float baseWidth = 1280f * (rate / baseRate) - 204f;
        bool sizeOver = false;
        friendsGridSize[0] = 92;
        friendsGridScale[0] = 1f;
        for (int i = 1; i < friendsGridSize.Length; i++) {
            if (sizeOver) {
                friendsGridSize[i] = friendsGridSize[i - 1];
                friendsGridScale[i] = friendsGridScale[i - 1];
            } else {
                int sizeTemp = Mathf.Min(92, (int)(baseWidth / i));
                if (sizeTemp <= 65) {
                    friendsGridSize[i] = friendsGridSize[i - 1];
                    friendsGridScale[i] = friendsGridScale[i - 1];
                    sizeOver = true;
                } else {
                    friendsGridSize[i] = sizeTemp;
                    if (sizeTemp >= 92) {
                        friendsGridScale[i] = 1f;
                    } else {
                        friendsGridScale[i] = sizeTemp / 92f;
                    }
                }
            }
        }
    }

    public void UpdateMapCameraRect() {
        if (mapCamera) {
            int mapPos = GameManager.Instance.save.config[GameManager.Save.configID_MapPos];
            if (mapPos >= 0 && mapPos < mapCameraRects.Length) {
                mapCamera.rect = mapCameraRects[mapPos];
                float sizeTemp = mapCameraSizeSave;
                if (GameManager.Instance.save.config[GameManager.Save.configID_MapPos] == mapCameraBigIndex) {
                    sizeTemp *= 2;
                }
                if (mapCamera.orthographicSize != sizeTemp) {
                    mapCamera.orthographicSize = sizeTemp;
                }
                if (!mapCamera.gameObject.activeSelf) {
                    mapCamera.gameObject.SetActive(true);
                }
            } else {
                if (mapCamera.gameObject.activeSelf) {
                    mapCamera.gameObject.SetActive(false);
                }
            }
        }
    }

    public void UpdateGoldPos() {
        if (gold) {
            int goldPos = GameManager.Instance.save.config[GameManager.Save.configID_GoldPos];
            if (goldPos >= 0 && goldPos < goldCanvasPosition.Length) {
                RectTransform goldRect = gold.GetComponent<RectTransform>();
                goldRect.anchorMin = goldCanvasAnchor[goldPos];
                goldRect.anchorMax = goldCanvasAnchor[goldPos];
                goldRect.anchoredPosition = goldCanvasPosition[goldPos];
                if (!gold.enabled && (goldTimeRemain > 0f || showGoldContinuous)) {
                    gold.enabled = true;
                }
            } else {
                if (gold.enabled) {
                    gold.enabled = false;
                }
            }
        }
    }

    public void ResetMapCamera() {
        if (mapCamera){
            if (mapCamera.transform.position != mapCameraDefaultPosition) {
                mapCamera.transform.position = mapCameraDefaultPosition;
            }
            if (mapCamera.transform.eulerAngles != mapCameraDefaultRotation) {
                mapCamera.transform.eulerAngles = mapCameraDefaultRotation;
            }
            mapCameraSizeSave = mapCameraDefaultSize;
            float sizeTemp = mapCameraSizeSave;
            if (GameManager.Instance.save.config[GameManager.Save.configID_MapPos] == mapCameraBigIndex) {
                sizeTemp *= 2;
            }
            if (mapCamera.orthographicSize != sizeTemp) {
                mapCamera.orthographicSize = sizeTemp;
            }
        }
    }

    public void UpdateHPSTCanvasSize() {
        if (GameManager.Instance.save.config[GameManager.Save.configID_HpGaugeSize] > 0) {
            hpstCanvas.GetComponent<RectTransform>().localScale = vecOne * sizeMulArray[Mathf.Clamp(GameManager.Instance.save.config[GameManager.Save.configID_HpGaugeSize] - 1, 0, 10)];
            if (!hpstCanvas.enabled) {
                hpstCanvas.enabled = true;
            }
        } else {
            if (hpstCanvas.enabled) {
                hpstCanvas.enabled = false;
            }
        }
    }

    public void SetMapCamera(Vector3 position, float rotation, float size) {
        if (mapCamera) {
            Vector3 vecTemp = new Vector3(90f, 0f, rotation);
            mapCamera.transform.localPosition = position;
            mapCamera.transform.localEulerAngles = vecTemp;
            mapCameraSizeSave = size;
            float sizeTemp = mapCameraSizeSave;
            if (GameManager.Instance.save.config[GameManager.Save.configID_MapPos] == mapCameraBigIndex) {
                sizeTemp *= 2;
            }
            if (mapCamera.orthographicSize != sizeTemp) {
                mapCamera.orthographicSize = sizeTemp;
            }
        }
    }

    public float GetLoudSoundVolumeRate(LoudSoundType loudSoundType, float decrease = 0.25f) {
        int index = (int)loudSoundType;
        float answer = loudSoundVolumeRate[index];
        loudSoundVolumeRate[index] = Mathf.Clamp01(loudSoundVolumeRate[index] - decrease);
        return answer;
    }

    public void SetPlayerTarget(GameObject newTarget, float targetRadius) {
        if (newTarget != null) {
            playerTarget = newTarget;
            playerTargetTrans = newTarget.transform;
            playerTargetCBase = newTarget.GetComponentInParent<CharacterBase>();
            if (playerTargetCBase && playerTargetCBase.isEnemy) {
                playerTargetEnemy = playerTargetCBase.GetComponent<EnemyBase>();
                if (playerTargetEnemy) {
                    bool isBoss = playerTargetEnemy.isBoss;
                    CameraManager.Instance.SetTarget(playerTargetTrans, Mathf.Max(isBoss ? 1f : 0.1f, targetRadius + 0.5f), isBoss ? 1.5f : 1f);
                }
            }
        } else {
            playerTarget = null;
            playerTargetTrans = null;
            playerTargetCBase = null;
            playerTargetEnemy = null;
        }
    }

    public int levelForStatus {
        get {
            int levelTemp = GameManager.Instance.save.Level - 1;
            if (levelTemp < 0) {
                levelTemp = 0;
            } else if (levelTemp >= GameManager.levelMax) {
                levelTemp = GameManager.levelMax - 1;
            }
            return levelTemp;
        }
    }

    public int GetClawType() {
        for (int i = clawID.Length - 1; i >= 0; i--) {
            if (GameManager.Instance.save.GetEquip(clawID[i])) {
                return i + 1;
            }
        }
        return 0;
    }

    public int GetEquipedWeaponRank() {
        int[] equip = GameManager.Instance.save.equip;
        if (equip[19] != 0 || equip[20] != 0) {
            return 4;
        } else if (equip[5] != 0 || equip[8] != 0 || equip[10] != 0 || equip[18] != 0) {
            return 3;
        } else if (equip[4] != 0 || equip[7] != 0 || equip[9] != 0 || equip[11] != 0 || equip[15] != 0 || equip[16] != 0 || equip[17] != 0) {
            return 2;
        } else if (equip[3] != 0 || equip[6] != 0 || equip[12] != 0 || equip[13] != 0 || equip[14] != 0) {
            return 1;
        } else {
            return 0;
        }
    }

    public float GetDamageSoundVolume(int damageColorType) {
        float answer;
        int soundType = damageColorToSound[damageColorType];
        if (damageSoundCount[soundType] >= damageSoundMax) {
            answer = 0f;
        } else {
            answer = damageSoundVolumeArray[damageSoundCount[soundType]];
            damageSoundCount[soundType]++;
        }
        return answer;
    }

    void LoadBuffSprites() {
        if (!buffSlotInitialized) {
            buffSlot = new BuffSlot[buffSlotMax];
            buffSlot[0] = new BuffSlot((int)EffectDatabase.id.itemAntidote, Resources.Load("Buff/buff_antidote", typeof(Sprite)) as Sprite, true);
            buffSlot[1] = new BuffSlot((int)EffectDatabase.id.itemBuffStamina, Resources.Load("Buff/buff_stamina", typeof(Sprite)) as Sprite);
            buffSlot[2] = new BuffSlot((int)EffectDatabase.id.itemBuffSpeed, Resources.Load("Buff/buff_speed", typeof(Sprite)) as Sprite);
            buffSlot[3] = new BuffSlot((int)EffectDatabase.id.itemBuffAttack, Resources.Load("Buff/buff_attack", typeof(Sprite)) as Sprite);
            buffSlot[4] = new BuffSlot((int)EffectDatabase.id.itemBuffDefense, Resources.Load("Buff/buff_defense", typeof(Sprite)) as Sprite);
            buffSlot[5] = new BuffSlot((int)EffectDatabase.id.itemBuffKnock, Resources.Load("Buff/buff_knock", typeof(Sprite)) as Sprite);
            buffSlot[6] = new BuffSlot((int)EffectDatabase.id.itemBuffAbsorb, Resources.Load("Buff/buff_absorb", typeof(Sprite)) as Sprite);
            buffSlot[7] = new BuffSlot((int)EffectDatabase.id.itemBuffStealth, Resources.Load("Buff/buff_stealth", typeof(Sprite)) as Sprite);
            buffSlot[8] = new BuffSlot((int)EffectDatabase.id.itemBuffLong, Resources.Load("Buff/buff_long", typeof(Sprite)) as Sprite);
            buffSlot[9] = new BuffSlot((int)EffectDatabase.id.itemBuffMulti, Resources.Load("Buff/buff_multi", typeof(Sprite)) as Sprite);
        }
        buffSlotInitialized = true;
    }

    public void RestoreFriends() {
        for (int i = 0; i < friendsOrder.Length; i++) {
            friendsOrder[i] = -1;
        }
        for (int i = 0; i < friends.Length; i++) {
            if (GameManager.Instance.save.friendsLiving[i] != 0) {
                if (friends[i].fBase && friends[i].fBase.GetNowHP() <= 0) {
                    Erase(i, false);
                }
                if (!friends[i].fBase) {
                    ChangeFriends(i, false, false, true);
                }
                if (friends[i].fBase) {
                    friends[i].fBase.isRestored = true;
                }
            }
        }
        ResetHP();
        sandstar = GameManager.Instance.save.sandstar;
        AddSandstar(0);
        CheckCostSum();
        friendsRestoreProcessed = true;
    }

    public void StopFriends() {
        if (pCon) {
            pCon.ResetAgent();
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.ResetAgent();
            }
        }
    }

    public void ForceStopForEventAll(float time) {
        if (pCon) {
            pCon.ForceStopForEvent(time);
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.ForceStopForEvent(time);
            }
        }
    }

    public void ReleaseStopForEventAll() {
        if (pCon) {
            pCon.ReleaseStopForEvent();
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.ReleaseStopForEvent();
            }
        }
    }

    void SetFriendsSeparated(int index, bool flag) {
        if (index >= 0 && index < friends.Length && friends[index].separatedMark) {
            friends[index].isSeparated = flag;
            friends[index].separatedMark.Activate(flag);
        }
    }

    public bool GetFriendsSeparated(int index) {
        if (GetFriendsExist(index, true)) {
            return friends[index].isSeparated;
        }
        return false;
    }

    public void CheckFriendsSeparated(int index) {
        if (index >= 0 && index < friends.Length && friends[index].fBase && !friends[index].isSeparated) {
            if (GetPlayerBetweenFriendsSqrMag(index) > warpSqrDist || Physics.Linecast(friends[index].trans.position + vecUp, playerTrans.position + vecUp, fieldLayerMask, QueryTriggerInteraction.Ignore)) {
                SetFriendsSeparated(index, true);
            }
        }
    }

    public void CheckFriendsSeparatedAll() {
        for (int i = 0; i < friends.Length; i++) {
            CheckFriendsSeparated(i);
        }
    }

    void SetFriendsAgentEnabledInternal(bool toEnable) {
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.SetAgentEnabled(toEnable);
            }
        }
    }

    public void SetFriendsAgentEnabled(bool toEnable = false, int delayFrames = 0) {
        if (delayFrames <= 0) {
            SetFriendsAgentEnabledInternal(toEnable);
        } else {
            agentDelayFrameRemain = delayFrames;
            agentDelayToEnable = toEnable;
        }
    }

    public bool WarpToPlayerPos(int index, bool soundEnable = true, int numerator = 0, int denominator = -1, bool considerSeparated = true, bool emitEffectEnabled = true, bool sameDirectionPlayer = false) {
        if (playerTrans && friends[index].fBase && (!considerSeparated || !friends[index].isSeparated)) {
            if (sameDirectionPlayer && playerTrans && friends[index].fBase.IsRotationEnabled()) {
                Vector3 eulerTemp = playerTrans.eulerAngles;
                eulerTemp.x = 0f;
                eulerTemp.z = 0f;
                friends[index].fBase.transform.eulerAngles = eulerTemp;
            }
            friends[index].fBase.Warp(GetFriendsAppearPos(numerator, denominator), 0.5f, 0f);
            if (emitEffectEnabled) {
                EmitEffect((int)EffectDatabase.id.friendsWarp, index);
                if (soundEnable) {
                    EmitEffect((int)EffectDatabase.id.friendsWarpSound, index, false);
                }
            }
            return true;
        }
        return false;
    }

    public bool WarpToPlayerPosAll(bool considerSeparated = true, bool emitEffectEnabled = true, bool sameDirectionPlayer = false) {
        bool sounded = false;
        int numerator = 0;
        int denominator = GetFriendsCount();
        if (denominator > 0) {
            for (int i = 0; i < GameManager.friendsMax; i++) {
                if (friends[i].fBase && WarpToPlayerPos(i, !sounded, numerator, denominator, considerSeparated, emitEffectEnabled, sameDirectionPlayer)) {
                    sounded = true;
                    numerator++;
                }
            }
        }
        return sounded;
    }

    public void WarpToCirclePosAll(Vector3 centerPos, float radius, bool lookAtCenter = true, float disableControlTime = 0f, float gravityZeroTime = 0f) {
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                Vector2 randCircle = Random.insideUnitCircle * radius;
                Vector3 randPos = centerPos;
                randPos.x += randCircle.x;
                randPos.z += randCircle.y;
                friends[i].fBase.Warp(randPos, disableControlTime, gravityZeroTime);
                if (lookAtCenter) {
                    friends[i].fBase.LookAtIgnoreY(centerPos);
                }
            }
        }
    }

    public void SuperWarp(bool resetCamera = false, bool effectEnabled = true) {
        Vector3 oldPosition = playerTrans.position;
        pCon.Teleport();
        if (effectEnabled) {
            EmitEffect((int)EffectDatabase.id.itemWarp);
        }
        WarpToPlayerPosAll(false);
        if (resetCamera && CameraManager.Instance) {
            CameraManager.Instance.ResetCameraFixPos();
            CameraManager.Instance.FreeLook_OnTargetObjectWarped(playerTrans.position - oldPosition);
        }
    }

    public void SuperWarp(Vector3 position, bool resetCamera = false, bool effectEnabled = true) {
        Vector3 oldPosition = playerTrans.position;
        pCon.Warp(position, 0f, 0f);
        if (effectEnabled) {
            EmitEffect((int)EffectDatabase.id.itemWarp);
        }
        WarpToPlayerPosAll(false);
        if (resetCamera && CameraManager.Instance) {
            CameraManager.Instance.ResetCameraFixPos();
            CameraManager.Instance.FreeLook_OnTargetObjectWarped(playerTrans.position - oldPosition);
        }
    }

    public float GetPlayerBetweenFriendsSqrMag(int index) {
        if (playerObj && friends[index].instance) {
            Vector3 playerPos = playerTrans.position;
            Vector3 friendsPos = friends[index].trans.position;
            playerPos.y = 0f;
            friendsPos.y = 0f;
            return (playerPos - friendsPos).sqrMagnitude;
        } else {
            return 0f;
        }
    }

    public void InquiryDamage(int damage, CharacterBase attacker, CharacterBase receiver) {
        if (GetBuff(BuffType.Absorb)) {
            if (attacker == pCon) {
                pCon.AddNowHP((int)Mathf.Max(1f, pCon.GetMaxHP() / pCon.GetAttack(true) * damage / 30f), pCon.GetCenterPosition(), true, colorTypeHeal);
            } else {
                for (int i = 0; i < friends.Length; i++) {
                    if (friends[i].fBase && attacker == friends[i].fBase) {
                        friends[i].fBase.AddNowHP((int)Mathf.Max(1f, friends[i].fBase.GetMaxHP() / friends[i].fBase.GetAttack(true) * damage / 30f), friends[i].fBase.GetCenterPosition(), true, colorTypeHeal);
                        break;
                    }
                }
            }
        }
    }

    public void SetSpecialChat(string talkName, int index, float time = 5f) {
        if (MessageUI.Instance) {
            if (index < 0) {
                if (pCon) {
                    MessageBackColor mesBackColor = pCon.GetComponent<MessageBackColor>();
                    if (mesBackColor) {
                        MessageUI.Instance.SetMessageOptional(TextManager.Get(talkName), mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType, playerFaceID[playerIndex], time, 1, 1, true);
                        MessageUI.Instance.SetMessageLog(TextManager.Get(talkName), playerFaceID[playerIndex]);
                    }
                }
            } else {
                if (GetFriendsExist(index, true)) {
                    MessageBackColor mesBackColor = friends[index].fBase.GetComponent<MessageBackColor>();
                    if (mesBackColor) {
                        MessageUI.Instance.SetMessageOptional(TextManager.Get(talkName), mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType, 100 + index, time, 1, 1, true);
                        MessageUI.Instance.SetMessageLog(TextManager.Get(talkName), 100 + index);
                    }
                }
            }
        }
    }

    public int IssueID() {
        if (id >= int.MaxValue) {
            id = 1;
        }
        id++;
        return id;
    }

    public float GetNormalKnockAmount() {
        if (pCon) {
            return pCon.GetKnock(true);
        } else {
            return 40f;
        }
    }

    public FriendsBase GetFriendsBase(int index) {
        if (index >= 0 && index < friends.Length) {
            return friends[index].fBase;
        }
        return null;
    }

    void UpdateSandstarDecrease() {
        if (pCon && Time.timeScale > 0 && !parkmanSettings.activated) {
            if (pCon.isSuperman) {
                if (!GameManager.Instance.minmiGolden) {
                    AddSandstar(Time.deltaTime * (-60f / 200f), true);
                }
                if (sandstar <= 0 && PauseController.Instance && GameManager.Instance.save.config[GameManager.Save.configID_ItemAutomaticUse] == 1) {
                    if ((isBossBattle || GetActionEnemyCount() >= 4) && GameManager.Instance.save.HaveSpecificItem(PauseController.itemSandstarID)) {
                        PauseController.Instance.ItemAutomaticUse(PauseController.itemSandstarID);
                    }
                }
                if (sandstar <= 0) {
                    ResetWR(true);
                }
            }
            if (GameManager.Instance.minmiGolden) {
                AddSandstar(Time.deltaTime * 2f, true);
            }
        }
    }

    public void UpdateSandstarMax() {
        pIconsMother.AmountMax = GameManager.Instance.save.SandstarMax;
    }
    public void UpdateSandstarReady() {
        if (pCon && !pCon.isSuperman && sandstar >= GameManager.sandstarMin && GetEquipEffect(EquipEffect.WildRelease) != 0) {
            if (!readyCanvas.enabled) {
                EmitEffect((int)EffectDatabase.id.friendsYKReady);
                readyCanvas.enabled = true;
            }
            if (sandstar >= GameManager.Instance.save.SandstarMax) {
                if (!readyShiny.enabled) {
                    readyShiny.enabled = true;
                    readyShiny.Play(true);
                }
                float shinyDuration = 1f;
                if (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.existWater) {
                    shinyDuration = 2f;
                }
                if (readyShiny.effectPlayer.duration != shinyDuration) {
                    readyShiny.effectPlayer.duration = shinyDuration;
                }
            } else {
                if (readyShiny.enabled) {
                    readyShiny.enabled = false;
                }
            }
        } else {
            if (readyCanvas.enabled) {
                readyCanvas.enabled = false;
            }
            if (readyShiny.enabled) {
                readyShiny.enabled = false;
            }
        }
    }

    public bool AddSandstar(float amount, bool force = false, int effectNum = -1, bool toCenterPivot = false, bool sureToEmitEffect = false) {
        bool answer = false;
        float ssSave = sandstar;
        int sandstarMax = GameManager.Instance.save.SandstarMax;
        if (force || (pCon && !pCon.isSuperman && !pCon.IsSupermanCooltime() && lastPlayerAttackTimeRemain > 0)) {
            sandstar += amount;
        }
        if (sandstar < 0) {
            sandstar = 0;
        } else if (sandstar > sandstarMax) {
            sandstar = sandstarMax;
        }
        answer = (sandstar != ssSave);
        if (answer || sureToEmitEffect) {
            EmitEffect(effectNum, -1, true, toCenterPivot);
        }
        if (pIconsMother) {
            pIconsMother.Amount = sandstar;
        }
        UpdateSandstarReady();
        return answer;
    }

    public float GetSandstar() {
        return sandstar;
    }
    public bool GetSandstarIsMax() {
        return sandstar >= GameManager.Instance.save.SandstarMax;
    }
    public int GetMaxHPPlus() {
        if (GameManager.Instance.save.GetEquip(GameManager.hpUpIndex)) {
            return GameManager.Instance.save.GotHpUp;
        } else {
            return 0;
        }
    }
    public float GetMaxSTPlus() {
        if (GameManager.Instance.save.GetEquip(GameManager.stUpIndex)) {
            return GameManager.Instance.save.GotStUp;
        } else {
            return 0;
        }
    }

    public int GetFriendsCostSum() {
        int answer = 0;
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase && friends[i].fBase.GetNowHP() > 0) {
                // answer += CharacterDatabase.Instance.friends[i].cost;
                answer += GetFriendsCost(i);
            }
        }
        return answer;
    }

    public int GetFriendsCount(bool askLive = true) {
        int answer = 0;
        for (int i = 0; i < friends.Length; i++) {
            if (GetFriendsExist(i, askLive)) {
                answer += 1;
            }
        }
        return answer;
    }

    public int GetFriendsLimit(bool considerRisky = false) {
        int answer = 4;
        if (GameManager.Instance.save.GetEquip(GameManager.friendsCostInfinityIndex)) {
            answer = considerRisky ? riskyCost : 10000;
        } else {
            for (int i = 0; i < 7; i++) {
                if (GameManager.Instance.save.GetEquip(GameManager.friendsCostMaxUpIndex + i)) {
                    answer += 1;
                }
            }
        }
        return answer;
    }

    public int GetFriendsSummonRemain() {
        return GetFriendsLimit() - GetFriendsCostSum();
    }

    int GetLivingFriendsOnSaveCostSum() {
        int answer = 0;
        int length = Mathf.Min(friends.Length, CharacterDatabase.Instance.friends.Length, GameManager.Instance.save.friendsLiving.Length);
        for (int i = 0; i < length; i++) {
            if (GameManager.Instance.save.friendsLiving[i] != 0) {
                answer += CharacterDatabase.Instance.friends[i].cost;
            }
        }
        return answer;
    }

    public int GetJaparimanShortage() {
        return Mathf.Max(GetFriendsLimit(true) - (friendsRestoreProcessed ? costSumSave : GetLivingFriendsOnSaveCostSum()) - GameManager.Instance.GetJaparimanCount(), 0);
    }

    public void CheckJaparimanShortage(int itemID) {
        switch (itemID) {
            case GameManager.japarimanID:
                japarimanShortage -= 1;
                break;
            case GameManager.japariman3SetID:
                japarimanShortage -= 3;
                break;
            case GameManager.japariman5SetID:
                japarimanShortage -= 5;
                break;
        }
        if (japarimanShortage < 0) {
            japarimanShortage = 0;
        }
    }

    public bool GetAnyFriendsExist() {
        for (int i = 0; i < friends.Length; i++) {
            if (GetFriendsExist(i, false)) {
                return true;
            }
        }
        return false;
    }

    public bool GetAnyFriendsLiving() {
        for (int i = 0; i < friends.Length; i++) {
            if (GetFriendsExist(i, true)) {
                return true;
            }
        }
        return false;
    }

    public bool GetFriendsEffectable(int index) {
        if (friendsEffectDisabled == 0 && GetFriendsExist(index, true)) {
            return (friends[index].fBase.GetAutoGatherEnabled() || GetPlayerBetweenFriendsSqrMag(index) <= warpSqrDist) && !friends[index].isSeparated;
        } else {
            return false;
        }
    }

    public bool GetSafetyBombEnabled() {
        if (GetFriendsExist(safetyBombID, true)) {
            return (friends[safetyBombID].fBase.GetAutoGatherEnabled() || GetPlayerBetweenFriendsSqrMag(safetyBombID) <= warpSqrDist) && !friends[safetyBombID].isSeparated;
        } else {
            return false;
        }
    }

    public float GetFriendsEffect(FriendsEffect type) {
        float answer = 0;
        switch (type) {
            case FriendsEffect.Attack:
                answer += GetFriendsEffectable(17) ? 0.03f : 0;
                if (sneezeTimeRemain > 0f && GetFriendsEffectable(sneezeFriendsID)) {
                    answer += 0.12f;
                }
                break;
            case FriendsEffect.Stamina:
                answer = GetFriendsEffectable(1) ? 0.1f : 0;
                break;
            case FriendsEffect.Knock:
                answer = GetFriendsEffectable(2) ? 0.08f : 0;
                break;
            case FriendsEffect.Mud:
                answer = GetFriendsEffectable(3) ? 1 : 0;
                break;
            case FriendsEffect.Fire:
                answer = GetFriendsEffectable(7) ? 1 : 0;
                break;
            case FriendsEffect.Ice:
                answer = GetFriendsEffectable(20) ? 1 : 0;
                break;
            case FriendsEffect.LightKnockEndurance:
                answer += GetFriendsEffectable(4) ? 20f : 0;
                answer += GetFriendsEffectable(25) ? 20f : 0;
                break;
            case FriendsEffect.Jump:
                answer = GetFriendsEffectable(5) ? 1 : 0;
                break;
            case FriendsEffect.Defense:
                answer = GetFriendsEffectable(6) ? 0.1f : 0;
                break;
            case FriendsEffect.XRay:
                answer = GetFriendsEffectable(8) || StageManager.Instance.IsXRayFloor ? 1 : 0;
                break;
            case FriendsEffect.DrownTolerance:
                answer = GetFriendsEffectable(9) ? 1 : 0;
                break;
            case FriendsEffect.JaparibunHealAll:
                answer = GetFriendsEffectable(10) ? 1 : 0;
                break;
            case FriendsEffect.WallBreak:
                answer = GetFriendsEffectable(11) ? 1 : 0;
                break;
            case FriendsEffect.IdlingSTHeal:
                answer = GetFriendsEffectable(12) ? 1 : 0;
                break;
            case FriendsEffect.MapCamera:
                answer = GetFriendsEffectable(15) ? 1 : 0;
                break;
            case FriendsEffect.MaxHP:
                answer = GetFriendsEffectable(16) ? 0.1f : 0;
                break;
            case FriendsEffect.Speed:
                answer = GetFriendsEffectable(18) ? 0.1f : 0;
                break;
            case FriendsEffect.JaparibunHunt:
                answer = GetFriendsEffectable(19) ? 1 : 0;
                break;
            case FriendsEffect.PPP:
                answer = GetFriendsExist(16, true) && GetFriendsExist(17, true) && GetFriendsExist(18, true) && GetFriendsExist(19, true) && GetFriendsExist(20, true) ? 1 : 0;
                break;
            case FriendsEffect.GoalDirection:
                answer = GetFriendsEffectable(21) ? 1 : 0;
                break;
            case FriendsEffect.Heal:
                answer = GetFriendsEffectable(22) ? 0.1f : 0;
                break;
            case FriendsEffect.Needle:
                answer = GetFriendsEffectable(23) ? 1 : 0;
                break;
            case FriendsEffect.KeenNose:
                answer = GetFriendsEffectable(24) ? 1 : 0;
                break;
            case FriendsEffect.SickHeal:
                answer = GetFriendsEffectable(26) ? 4f / 3f : 1f;
                break;
            case FriendsEffect.ReduceSlipDamage:
                answer = GetFriendsEffectable(27) ? 1 : 0;
                break;
            case FriendsEffect.WRHeal:
                answer = GetFriendsEffectable(29) ? 1.2f : 1f;
                break;
            case FriendsEffect.LongUse:
                answer = GetFriendsEffectable(30) ? 0.9090909f : 1f;
                break;
            case FriendsEffect.Muteki:
                answer = GetFriendsEffectable(13) ? 1 : 0;
                break;
            case FriendsEffect.Dodge:
                answer = GetFriendsEffectable(14) ? 1 : 0;
                break;
        }
        return answer;
    }

    public void SetFriendsLost(int id, bool isSacrifice = false) {
        if (id >= 0) {
            if (MessageUI.Instance != null && ItemDatabase.Instance != null) {
                int itemID = id + 100;
                MessageUI.Instance.SetMessage(
                    sb.Clear()
                    .Append(TextManager.Get("QUOTE_START"))
                    .Append(ItemDatabase.Instance.GetItemName(itemID))
                    .Append(TextManager.Get("QUOTE_END"))
                    .Append(TextManager.Get(isSacrifice ? "MESSAGE_SACRIFICE" : "MESSAGE_FRIENDSLOST")).ToString());
            }
        }
        if (id == sneezeFriendsID) {
            sneezeTimeRemain = 0f;
        }
        if (id == fennecID && GetFriendsExist(raccoonID, true)) {
            Friends_Raccoon raccoon = friends[raccoonID].fBase.GetComponent<Friends_Raccoon>();
            if (raccoon) {
                raccoon.SetAngry(true);
            }
        }
        if (id == raccoonID && GetFriendsExist(fennecID, true)) {
            Friends_Fennec fennec = friends[fennecID].fBase.GetComponent<Friends_Fennec>();
            if (fennec) {
                fennec.SetAngry(true);
            }
        }
    }

    public bool IsPassiveSneeze {
        get {
            return sneezeTimeRemain > 0f && GetFriendsEffectable(sneezeFriendsID);
        }
    }

    public void SetWeapon(int id) {
        if (GameManager.Instance.save.weapon[id] != 0) {
            GameManager.Instance.save.equip[id] = (GameManager.Instance.save.equip[id] + 1) % 2;
            if (GameManager.Instance.save.equip[id] != 0) {
                switch (id) {
                    case 3:
                        GameManager.Instance.save.equip[4] = 0;
                        GameManager.Instance.save.equip[5] = 0;
                        break;
                    case 4:
                        GameManager.Instance.save.equip[3] = 0;
                        GameManager.Instance.save.equip[5] = 0;
                        break;
                    case 5:
                        GameManager.Instance.save.equip[3] = 0;
                        GameManager.Instance.save.equip[4] = 0;
                        break;
                    case 6:
                        GameManager.Instance.save.equip[7] = 0;
                        GameManager.Instance.save.equip[8] = 0;
                        break;
                    case 7:
                        GameManager.Instance.save.equip[6] = 0;
                        GameManager.Instance.save.equip[8] = 0;
                        break;
                    case 8:
                        GameManager.Instance.save.equip[6] = 0;
                        GameManager.Instance.save.equip[7] = 0;
                        break;
                    case 9:
                        GameManager.Instance.save.equip[10] = 0;
                        break;
                    case 10:
                        GameManager.Instance.save.equip[9] = 0;
                        break;
                }
            }
            if (id >= GameManager.sandstarCondition) {
                UpdateSandstarMax();
                AddSandstar(0);
            }
            if (pCon) {
                staminaBorder = pCon.GetStaminaBorder();
            }
        }
    }

    public float GetEquipEffect(EquipEffect type) {
        float answer = 0f;
        switch (type) {
            case EquipEffect.Dodge:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Dodge) ? 1 : 0;
                break;
            case EquipEffect.Escape:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Escape) ? 1 : 0;
                break;
            case EquipEffect.Gather:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Gather) ? 1 : 0;
                break;
            case EquipEffect.Attack:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Attack + 2) ? 1f: GameManager.Instance.save.GetEquip(GameManager.equipmentID_Attack + 1) ? 0.6f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Attack) ? 0.3f : 0f;
                break;
            case EquipEffect.Knock:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Attack + 2) ? 1f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Attack + 1) ? 0.6f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Attack) ? 0.3f : 0f;
                break;
            case EquipEffect.Defense:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Defense + 2) ? 1f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Defense + 1) ? 0.6f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Defense) ? 0.3f : 0f;
                break;
            case EquipEffect.Speed:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Speed + 1) ? 0.5f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Speed) ? 0.25f : 0f;
                break;
            case EquipEffect.SpeedInAir:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Speed + 1) ? 0.1666667f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Speed) ? 0.125f : 0f;
                break;
            case EquipEffect.Impact:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Speed + 1) ? 1 : 0;
                break;
            case EquipEffect.Jump:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Jump) ? 1 : 0;
                break;
            case EquipEffect.Combo:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Combo) ? 1 : 0;
                break;
            case EquipEffect.Pile:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Pile) ? 1 : 0;
                break;
            case EquipEffect.SpeedRank:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Speed + 1) ? 2 : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Speed) ? 1 : 0;
                break;
            case EquipEffect.Spin:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Spin) ? 1 : 0;
                break;
            case EquipEffect.Wave:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Wave) ? 1 : 0;
                break;
            case EquipEffect.Screw:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Screw) ? 1 : 0;
                break;
            case EquipEffect.Bolt:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Bolt) ? 1 : 0;
                break;
            case EquipEffect.Judgement:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Judgement) ? 1 : 0;
                break;
            case EquipEffect.Antimatter:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Antimatter) ? 1 : 0;
                break;
            case EquipEffect.Analyzer:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Analyzer) ? 1 : 0;
                break;
            case EquipEffect.WildRelease:
                answer = GameManager.Instance.save.GetEquip(GameManager.sandstarCondition) ? 1 : 0;
                break;
            case EquipEffect.Crest:
                for (int i = GameManager.sandstarMaxUpIndex; i < GameManager.sandstarMaxUpIndex + 6; i++) {
                    if (GameManager.Instance.save.GetEquip(i)) {
                        answer += 1;
                    }
                }
                break;
            case EquipEffect.AllWild:
                answer = GameManager.Instance.save.GetEquip(GameManager.friendsCostInfinityIndex) ? 1 : 0;
                break;
            case EquipEffect.CostMove:
                answer += GameManager.Instance.save.GetEquip(GameManager.equipmentID_Defense + 2) ? 0.5f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Defense + 1) ? 0.3f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Defense) ? 0.15f : 0;
                answer += GameManager.Instance.save.GetEquip(GameManager.equipmentID_Speed + 1) ? 0.8f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Speed) ? 0.4f : 0f;
                break;
            case EquipEffect.CostJump:
                answer += GameManager.Instance.save.GetEquip(GameManager.equipmentID_Defense + 2) ? 0.5f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Defense + 1) ? 0.3f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Defense) ? 0.15f : 0;
                answer += GameManager.Instance.save.GetEquip(GameManager.equipmentID_Speed + 1) ? 0.2666667f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Speed) ? 0.2f : 0f;
                break;
            case EquipEffect.CostAttack:
                answer += GameManager.Instance.save.GetEquip(GameManager.equipmentID_Attack + 2) ? 1f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Attack + 1) ? 0.6f : GameManager.Instance.save.GetEquip(GameManager.equipmentID_Attack) ? 0.3f : 0f;
                break;
            case EquipEffect.Plasma:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Attack + 2) ? 1 : 0;
                break;
            case EquipEffect.Gravity:
                answer = GameManager.Instance.save.GetEquip(GameManager.equipmentID_Defense + 2) ? 1 : 0;
                break;
        }
        return answer;
    }

    public float GetPowerBalance() {
        return ((float)GetActionEnemyCount() * 4f) / ((float)costSumSave + 4f);
    }

    public float GetDifficultyEffect(DifficultyEffect type, bool forceDefaultDifficulty = false) {
        float answer = 1f;
        int difficulty = GameManager.Instance.save.difficulty;
        if (forceDefaultDifficulty) {
            difficulty = 2;
        }
        switch (type) {
            case DifficultyEffect.DamageZako:
                if (difficulty <= 1) {
                    answer = 0.7f;
                } else if (difficulty == 2) {
                    answer = 1f;
                } else if (difficulty == 3) {
                    answer = 1.5f;
                } else if (difficulty >= 4) {
                    answer = 2f;
                }
                break;
            case DifficultyEffect.DamageBoss:
                if (difficulty <= 1) {
                    answer = 0.7f;
                } else if (difficulty == 2) {
                    answer = 1f;
                } else if (difficulty == 3) {
                    answer = 2f;
                } else if (difficulty >= 4) {
                    answer = 3.5f;
                }
                break;
            case DifficultyEffect.MaxHPZako:
                if (difficulty <= 1) {
                    answer = 0.8f;
                } else if (difficulty == 2) {
                    answer = 1f;
                } else if (difficulty == 3) {
                    answer = 1.2f;
                } else if (difficulty >= 4) {
                    answer = 1.4f;
                }
                break;
            case DifficultyEffect.MaxHPBoss:
                if (difficulty <= 1) {
                    answer = 0.8f;
                } else if (difficulty == 2) {
                    answer = 1f;
                } else if (difficulty == 3) {
                    answer = 1.5f;
                } else if (difficulty >= 4) {
                    answer = 2f;
                }
                break;
            case DifficultyEffect.CoreHideRate:
                if (difficulty <= 1) {
                    answer = 0.8f;
                } else if (difficulty == 2) {
                    answer = 1f;
                } else if (difficulty == 3) {
                    answer = 1.2f;
                } else if (difficulty >= 4) {
                    answer = 1.4f;
                }
                break;
            case DifficultyEffect.DampNormalDamage:
                if (difficulty == 3) {
                    answer = 0.8f;
                } else if (difficulty >= 4) {
                    answer = 0.5f;
                }
                break;
            case DifficultyEffect.SlipDamage:
                if (difficulty >= 3) {
                    answer = 2;
                }
                break;
            case DifficultyEffect.NeedleDamage:
                if (difficulty >= 3) {
                    answer = 0.25f;
                } else {
                    answer = 0.2f;
                }
                break;
            case DifficultyEffect.FriendsKnock:
                if (difficulty <= 1) {
                    answer = 1.5f;
                } else {
                    answer = 1f;
                }
                break;
            case DifficultyEffect.FriendsDodge:
                //Test
                if (difficulty <= 2) {
                    answer = 0.5f;
                } else if (difficulty == 3) {
                    answer = 0.75f;
                } else {
                    answer = 1f;
                }
                break;
            case DifficultyEffect.LevelUp:
                if (difficulty <= 2) {
                    answer = 0;
                }
                break;
            case DifficultyEffect.ExtendIntervalEnabled:
                if (difficulty >= 2) {
                    answer = 0;
                }
                break;
            case DifficultyEffect.ExtendInterval:
                if (difficulty >= 2) {
                    answer = 0;
                } else {
                    answer = Mathf.Clamp((GetPowerBalance() - 1f) * 0.1f, 0f, 0.5f);
                }
                break;
            case DifficultyEffect.Reaper:
                if (difficulty <= 2) {
                    answer = 0;
                }
                break;
            case DifficultyEffect.EnemyWR:
                if (difficulty <= 3) {
                    answer = 0;
                }
                break;
            case DifficultyEffect.WRSpeedBias:
                answer = Mathf.Clamp(1f - (GetPowerBalance() - 1f) * 0.1f, 0.5f, 1f);
                break;
            case DifficultyEffect.DropRate:
                answer = 35;
                break;
            case DifficultyEffect.ServalGuts:
                answer = 3f;
                break;
            case DifficultyEffect.IbisHealRate:
                if (difficulty >= 4) {
                    answer = 0.08f;
                } else if (difficulty == 3) {
                    answer = 0.07f;
                } else {
                    answer = 0.06f;
                }
                break;
        }
        return answer;
    }

    public void CheckItemOverflow() {
        int itemCount = GameManager.Instance.save.NumOfAllItems();
        int inventoryMax = GameManager.Instance.save.InventoryMax;
        int denominator = itemCount - inventoryMax;
        if (denominator > 0 && ItemDatabase.Instance && pCon) {
            int[] itemArray = new int[denominator];
            for (int i = 0; i < denominator; i++) {
                itemArray[i] = GameManager.Instance.save.EndItemID();
                if (itemArray[i] >= 0) {
                    GameManager.Instance.save.items.Remove(itemArray[i]);
                }
            }
            ItemDatabase.Instance.GiveItem(itemArray, pCon.transform, 1);
        }
    }

    public void CheckHPSTOverflow() {
        if (pCon) {
            if (pCon.GetNowHP() > pCon.GetMaxHP()) {
                pCon.SetNowHP(pCon.GetMaxHP());
            }
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                if (friends[i].fBase.GetNowHP() > pCon.GetMaxHP()) {
                    friends[i].fBase.SetNowHP(friends[i].fBase.GetMaxHP());
                }
            }
        }
    }

    public bool CheckDamaged() {
        if (pCon && pCon.GetNowHP() < pCon.GetMaxHP()) {
            return true;
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                if (friends[i].fBase.GetNowHP() < friends[i].fBase.GetMaxHP()) {
                    return true;
                }
            }
        }
        return false;
    }
    public void CheckLimitSet(bool checkLevel, bool checkEquipment, bool resetHP = false) {
        if (checkLevel) {
            LevelReset();
            UpdateLevelLimit();
        }
        if (checkEquipment) {
            CheckWeaponAll();
            UpdateSandstarMax();
            CheckItemOverflow();
            CheckHPSTOverflow();
        }
        if (resetHP) {
            ResetHP();
        }
    }

    public void Heal(int amount, int percent, int effectNum = -1, bool allFlag = true, bool showDamage = true, bool toCenterPivot = false, bool friendsEffectable = false, bool attenuate = false) {
        float healRate = 1f;
        if (friendsEffectable) {
            healRate += Instance.GetFriendsEffect(FriendsEffect.Heal);
        }
        if (pCon) {
            pCon.AddNowHP(Mathf.RoundToInt(Mathf.Max(amount, pCon.GetMaxHP() * percent / 100) * healRate), pCon.GetCenterPosition(), showDamage, colorTypeHeal);
            EmitEffect(effectNum, -1, true, toCenterPivot);
        }
        if (allFlag) {
            float attenuateRate = 1f;
            if (attenuate) {
                attenuateRate = riskyDecSqrt;
            }
            for (int i = 0; i < friends.Length; i++) {
                if (friends[i].fBase) {
                    friends[i].fBase.AddNowHP(Mathf.RoundToInt(Mathf.Max(amount, friends[i].fBase.GetMaxHP() * percent / 100) * healRate * attenuateRate), friends[i].fBase.GetCenterPosition(), showDamage, colorTypeHeal);
                    EmitEffect(effectNum, i, true, toCenterPivot, true);
                }
            }
        }
    }    

    public int PreCalcHealAmount(int amount, int percent, bool friendsEffectable) {
        float healRate = 1f;
        if (friendsEffectable) {
            healRate += Instance.GetFriendsEffect(FriendsEffect.Heal);
        }
        if (pCon) {
            return Mathf.RoundToInt(Mathf.Max(amount, pCon.GetMaxHP() * percent / 100) * healRate);
        } else {
            return Mathf.RoundToInt(amount * healRate);
        }
    }


    public void ClearSickAll() {
        if (pCon) {
            pCon.ClearSick();
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.ClearSick();
            }
        }
    }

    public void HealAsMuchAsNeededAll(int maxAmount, bool attenuate = false) {
        int nowHPTemp = 0;
        int maxHPTemp = 0;
        int effectNum = (int)EffectDatabase.id.itemHeal01;
        bool sfxDisablize = false;
        if (pCon) {
            nowHPTemp = pCon.GetNowHP();
            maxHPTemp = pCon.GetMaxHP();
            if (nowHPTemp > 0 && nowHPTemp < maxHPTemp) {
                pCon.AddNowHP(Mathf.Min(maxHPTemp - nowHPTemp, maxAmount), pCon.GetCenterPosition(), true, colorTypeHeal);
                EmitEffect(effectNum, -1, true, true, sfxDisablize);
                sfxDisablize = true;
            }
        }
        maxAmount = Mathf.RoundToInt(maxAmount * riskyDecSqrt);
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                nowHPTemp = friends[i].fBase.GetNowHP();
                maxHPTemp = friends[i].fBase.GetMaxHP();
                if (nowHPTemp > 0 && nowHPTemp < maxHPTemp) {
                    friends[i].fBase.AddNowHP(Mathf.Min(maxHPTemp - nowHPTemp, maxAmount), friends[i].fBase.GetCenterPosition(), true, colorTypeHeal);
                    EmitEffect(effectNum, -1, true, true, sfxDisablize);
                    sfxDisablize = true;
                }
            }
        }
    }

    public float PlayerAttackPowerBase {
        get {
            if (pCon) {
                return pCon.GetAttackNoEffected();
            } else {
                return 10f;
            }
        }
    }
    
    public float GetNormallyRequiredAttackCount(float enemyHP) {
        return Mathf.Max(enemyHP / PlayerAttackPowerBase, 1);
    }

    public float GetNormallyRequiredAttackCount_Bias(float enemyHP) {
        return Mathf.Max(enemyHP / (PlayerAttackPowerBase * nowWRSpeedBias), 1);
    }

    public bool GetCharacterIsPlayer(CharacterBase cBase) {
        return (pCon && cBase && pCon == cBase);
    }

    public float GetSandstarIncrease(float enemyNowHP, float enemyMaxHP, float damage, float addRate) {
        return Mathf.Max(0.04f, Mathf.Min(damage, enemyNowHP) / enemyMaxHP * GetNormallyRequiredAttackCount(enemyMaxHP) * 0.08f) * addRate;
    }

    public float GetSandstarIncrease_Dead(float enemyHP) {
        return Mathf.Max(1, enemyHP / PlayerAttackPowerBase) * 0.02f;
    }

    public void SetBuff(BuffType buffType, float duration = 40f) {
        buffSlot[(int)buffType].remainTime = duration;
        if (buffType == BuffType.Antidote) {
            EmitEffect((int)EffectDatabase.id.itemAntidoteSE, -1, false);
        } else {
            EmitEffect((int)EffectDatabase.id.itemBuffSE, -1, false);
        }
        if (!TrophyManager.Instance.IsTrophyHad(TrophyManager.t_Chimera)) {
            bool answer = true;
            for (int i = 1; i < buffSlot.Length; i++) {
                if (buffSlot[i].remainTime <= 0f) {
                    answer = false;
                    break;
                }
            }
            if (answer) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_Chimera, true);
            }
        }
    }

    public bool GetBuff(BuffType buffType) {
        return buffSlotInitialized && buffSlot[(int)buffType].remainTime > 0;
    }

    public bool GetBuff(int buffIndex) {
        return buffSlotInitialized && buffSlot[buffIndex].remainTime > 0;
    }

    public int GetBuffCount(bool excludeAntidote = false) {
        int count = 0;
        if (buffSlotInitialized) {
            for (int i = excludeAntidote ? 1 : 0; i < buffSlot.Length; i++) {
                if (buffSlot[i].remainTime > 0) {
                    count++;
                }
            }
        }
        return count;
    }

    public bool GetFriendsInjured(int exceptID = -1) {
        bool answer = false;
        if (pCon) {
            answer = (pCon.GetNowHP() < pCon.GetMaxHP());
        }
        if (!answer) {
            for (int i = 0; !answer && i < friends.Length; i++) {
                if (i != exceptID && friends[i].fBase) {
                    answer = (friends[i].fBase.GetNowHP() < friends[i].fBase.GetMaxHP());
                }
            }
        }
        return answer;
    }

    public GameObject GetBuffEffect(int buffIndex) {
        if (EffectDatabase.Instance) {
            return EffectDatabase.Instance.prefab[buffSlot[buffIndex].effectIndex];
        } else {
            return null;
        }
    }

    public bool GetBuffIsCenter(int buffIndex) {
        return buffSlotInitialized && buffSlot[buffIndex].effectIsCenter;
    }

    public void ClearBuff() {
        if (!buffSlotInitialized) {
            LoadBuffSprites();
        } else {
            for (int i = 0; i < buffSlot.Length; i++) {
                buffSlot[i].remainTime = 0;
            }
        }
    }

    public GameObject EmitEffect(int effectNum, int friendsId = -1, bool parenting = true, bool toCenterPivot = false, bool sfxDisablize = false) {
        Transform temp = null;
        GameObject obj = null;
        if (effectNum == sfxPlayed) {
            sfxDisablize = true;
        }
        if (effectNum >= 0) {
            if (friendsId < 0 && pCon) {
                if (toCenterPivot) {
                    temp = pCon.centerPivot;
                } else {
                    temp = playerTrans;
                }
            } else if (friendsId >= 0 && friendsId < friends.Length && friends[friendsId].fBase) {
                if (toCenterPivot) {
                    temp = friends[friendsId].fBase.centerPivot;
                } else {
                    temp = friends[friendsId].trans;
                }
            }
            if (temp) {
                obj = Instantiate(EffectDatabase.Instance.prefab[effectNum], temp.position, temp.rotation, parenting ? temp : null);
                if (sfxDisablize) {
                    AudioSource aSrc = obj.GetComponentInChildren<AudioSource>();
                    if (aSrc) {
                        aSrc.enabled = false;
                    }
                } else {
                    sfxPlayed = effectNum;
                }
            }
        }
        return obj;
    }

    public void ResetHP() {
        if (pCon) {
            pCon.ResetHP();
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.ResetHP();
            }
        }
    }

    public void ResetST() {
        if (pCon) {
            pCon.SetNowST(pCon.GetMaxST());
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.SetNowST(friends[i].fBase.GetMaxST());
            }
        }
    }

    public void SupermanStart() {
        if (pCon) {
            pCon.SupermanStart(true);
        }
        if (GetEquipEffect(EquipEffect.AllWild) != 0) {
            for (int i = 0; i < friends.Length; i++) {
                if (friends[i].fBase && !friends[i].fBase.IsDead) {
                    friends[i].fBase.SupermanStart(true);
                }
            }
        }
    }

    public void ResetWR(bool effectEnabled = false) {
        if (pCon) {
            pCon.SupermanEnd(effectEnabled);
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase && !friends[i].fBase.IsDead) {
                friends[i].fBase.SupermanEnd(effectEnabled);
            }
        }
    }

    public void ResetGuts() {
        if (pCon) {
            pCon.ResetGuts();
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.ResetGuts();
            }
        }
    }

    public void ForceCanvasEnabled(int index, int value) {
        CanvasCulling canvasCulling = GetComponent<CanvasCulling>();
        if (canvasCulling) {
            canvasCulling.CheckConfig(index, value);
        }
    }

    public void SetWRSpeedBias(int speedLevel) {
        nowWRSpeedBias = wrSpeedBiasArray[Mathf.Clamp(speedLevel, 0, wrSpeedBiasArray.Length - 1)];
    }

    public void SetEventMutekiTimeAll(float time) {
        if (pCon) {
            pCon.SetEventMutekiTime(time);
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.SetEventMutekiTime(time);
            }
        }
    }

    public void PlayerRevive() {
        if (pCon) {
            pCon.SetNowHP(pCon.GetMaxHP());
            pCon.ResetGuts();
        }
        ResetForMoveFloor();
        playerDead = false;
    }

    public void ResetForRetry() {
        bossTimeContinuing = false;
        bossTimeInitialized = false;
        if (pCon) {
            pCon.SetNowHP(pCon.GetMaxHP());
            pCon.SetSearchTargetActive(true);
            pCon.ResetGuts();
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.SetNowHP(friends[i].fBase.GetMaxHP());
                friends[i].fBase.SetSearchTargetActive(true);
                friends[i].fBase.ResetGuts();
            }
        }        
        AddSandstar(-100, true);
        playerDead = false;
    }    

    public void ResetForMoveStage() {
        bossTimeContinuing = false;
        FriendsClearAll();
        if (pCon) {
            pCon.SetNowHP(pCon.GetMaxHP());
            pCon.SetSearchTargetActive(true);
            pCon.ResetGuts();
        }
        AddSandstar(-100, true);
        playerDead = false;
        if (numberUiInitialized) {
            for (int canvasIndex = 0; canvasIndex < damageCanvasMax; canvasIndex++) {
                int count = damageList[canvasIndex].Count;
                if (count > 0) {
                    for (int i = 0; i < count; i++) {
                        if (damageList[canvasIndex][i].text) {
                            Destroy(damageList[canvasIndex][i].text.gameObject);
                        }
                    }
                    damageList[canvasIndex].Clear();
                }
            }
            for (int dodgeType = 0; dodgeType < dodgeTextTypeMax; dodgeType++) {
                int count = dodgeList[dodgeType].Count;
                if (count > 0) {
                    for (int i = 0; i < count; i++) {
                        if (dodgeList[dodgeType][i].text) {
                            Destroy(dodgeList[dodgeType][i].text.gameObject);
                        }
                    }
                    dodgeList[dodgeType].Clear();
                }
            }
        }
    }
    
    public void ResetForMoveFloor() {
        Heal(0, 100, -1, true, false);
        ClearBuff();
        if (!bossTimeContinuing) {
            BossTimeHide();
            playerDamageSum = 0;
            playerDamageCount = 0;
            bossResult.useItemCount = 0;
            bossDefeatedFlag = false;
        }
        if (bossResult.canvas.enabled) {
            bossResult.canvas.enabled = false;
        }
        AchievementHide();
        if (pCon) {
            pCon.ResetFootMaterial();
        }
        if (CameraManager.Instance) {
            CameraManager.Instance.CancelQuake();
        }
        if (PauseController.Instance) {
            PauseController.Instance.itemDisabled = false;
            PauseController.Instance.friendsDisabled = false;
            PauseController.Instance.equipChangeDisabled = false;
            PauseController.Instance.gameOverDisabled = false;
        }
        if (StageManager.Instance) {
            StageManager.Instance.slipDamageOverride = 0;
        }
        if (MessageUI.Instance) {
            MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_Speech);
        }
        if (SceneChange.Instance) {
            SceneChange.Instance.reserveShortcut = false;
        }
        ResetST();
        ResetWR();
        ResetGuts();
        CancelStaminaAlert();
        SetEventMutekiTimeAll(0.5f);
        ClearActionEnemy();
        bossTimeInitialized = false;
        defeatEnemyCount = 0;
        defeatEnemyCountSSRaw = 0;
        sandcatFindNum = 0;
        // raccoonStealNumForLimit = 0;
        SetGeneralCommand(CharacterBase.Command.Default);
        inertIFR = false;
        sandstarRawActivated = 0;
        isClimbing = false;
        isBossBattle = false;
        SetBossBattleDistance(false);
        justDodgeAmount = 1f;
        multiBossCount = 0;
        stealFailedCount = 0;
        t_JustDodgeCount = 0;
        itemAutomaticUseInterval = 1f;
        graphAutoChecked = false;
        graphAutoInterval = 0.5f;
        InitRecords();
        PlayerAttackStamp();
        japarimanShortage = GetJaparimanShortage();
        if (numberUiInitialized) {
            for (int canvasIndex = 0; canvasIndex < damageCanvasMax; canvasIndex++) {
                int count = damageList[canvasIndex].Count;
                if (count > 32) {
                    for (int i = 32; i < count; i++) {
                        if (damageList[canvasIndex][i].text) {
                            Destroy(damageList[canvasIndex][i].text.gameObject);
                        }
                    }
                    damageList[canvasIndex].RemoveRange(32, count - 32);
                    count = damageList[canvasIndex].Count;
                }
                if (count > 0) {
                    for (int i = 0; i < count; i++) {
                        if (damageList[canvasIndex][i].text && damageList[canvasIndex][i].text.enabled) {
                            damageList[canvasIndex][i].text.enabled = false;
                        }
                    }
                }
            }
            for (int dodgeType = 0; dodgeType < dodgeTextTypeMax; dodgeType++) {
                int count = dodgeList[dodgeType].Count;
                if (count > 32) {
                    for (int i = 32; i < count; i++) {
                        if (dodgeList[dodgeType][i].text) {
                            Destroy(dodgeList[dodgeType][i].text.gameObject);
                        }
                    }
                    dodgeList[dodgeType].RemoveRange(32, count - 32);
                    count = dodgeList[dodgeType].Count;
                }
                if (count > 0) {
                    for (int i = 0; i < count; i++) {
                        if (dodgeList[dodgeType][i].text && dodgeList[dodgeType][i].text.enabled) {
                            dodgeList[dodgeType][i].text.enabled = false;
                        }
                    }
                }
            }
        }
        if (GameManager.Instance.save.config[GameManager.Save.configID_LockonReset] != 0) {
            autoAim = (GameManager.Instance.save.config[GameManager.Save.configID_LockonReset] <= 1 ? 1 : 0);
        }
        GameManager.Instance.save.SetRunInBackground();
    }

    public void SetGeneralCommand(CharacterBase.Command newCommand, bool messageEnabled = false) {
        generalCommand = newCommand;
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.SetCommand(generalCommand);
            }
        }
        if (messageEnabled) {
            MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_CommandAttack);
            MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_CommandEvade);
            MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_CommandIgnore);
            MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_CommandFree);
            sb.Clear()
                .Append(TextManager.Get(newCommand == CharacterBase.Command.Default ? "CHOICE_ATTACK" : newCommand == CharacterBase.Command.Evade ? "CHOICE_EVADE" : newCommand == CharacterBase.Command.Ignore ? "CHOICE_IGNORE" : "CHOICE_FREE"))
                .Append(" -> ")
                .Append(TextManager.Get("ITEM_NAME_100"));
            MessageUI.Instance.SetMessage(sb.ToString(), MessageUI.time_Default, MessageUI.panelType_Information, newCommand == CharacterBase.Command.Default ? MessageUI.slotType_CommandAttack : newCommand == CharacterBase.Command.Evade ? MessageUI.slotType_CommandEvade : newCommand == CharacterBase.Command.Ignore ? MessageUI.slotType_CommandIgnore : MessageUI.slotType_CommandFree);
        }
        if (generalCommand == CharacterBase.Command.Default) {
            generalCommandSlot.text.text = "";
        } else {
            generalCommandSlot.SetCommandText(generalCommand);
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase && friends[i].slotObject) {
                CommandSlot commandSlot = friends[i].slotObject.GetComponent<CommandSlot>();
                if (commandSlot) {
                    CharacterBase.Command friendsCommand = friends[i].fBase.GetCommand();
                    if (friendsCommand == generalCommand) {
                        commandSlot.text.text = "";
                    } else {
                        commandSlot.SetCommandText(friendsCommand);
                    }
                }
            }
        }
    }

    public void SetSpecificCommand(int index, CharacterBase.Command newCommand, bool messageEnabled = false) {
        if (index >= 0 && index < friends.Length && friends[index].fBase) {
            friends[index].fBase.SetCommand(newCommand);
            if (messageEnabled) {
                MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_CommandAttack);
                MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_CommandEvade);
                MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_CommandIgnore);
                MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_CommandFree);
                sb.Clear()
                    .Append(TextManager.Get(newCommand == CharacterBase.Command.Default ? "CHOICE_ATTACK" : newCommand == CharacterBase.Command.Evade ? "CHOICE_EVADE" : newCommand == CharacterBase.Command.Ignore ? "CHOICE_IGNORE" : "CHOICE_FREE"))
                    .Append(" -> ")
                    .Append(TextManager.Get("ITEM_NAME_" + (index + 100).ToString("000")));
                MessageUI.Instance.SetMessage(sb.ToString(), MessageUI.time_Default, MessageUI.panelType_Information, newCommand == CharacterBase.Command.Default ? MessageUI.slotType_CommandAttack : newCommand == CharacterBase.Command.Evade ? MessageUI.slotType_CommandEvade : newCommand == CharacterBase.Command.Ignore ? MessageUI.slotType_CommandIgnore : MessageUI.slotType_CommandFree);
            }
            if (friends[index].slotObject) {
                CommandSlot commandSlot = friends[index].slotObject.GetComponent<CommandSlot>();
                if (commandSlot) {
                    if (newCommand == generalCommand) {
                        commandSlot.text.text = "";
                    } else {
                        commandSlot.SetCommandText(newCommand);
                    }
                }
            }
        }
    }

    public void ResetCommandTexts() {
        if (generalCommandSlot) {
            if (generalCommand == CharacterBase.Command.Default) {
                generalCommandSlot.text.text = "";
            } else {
                generalCommandSlot.ResetCommandText();
            }
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase && friends[i].slotObject) {
                CommandSlot commandSlot = friends[i].slotObject.GetComponent<CommandSlot>();
                if (commandSlot) {
                    CharacterBase.Command friendsCommand = friends[i].fBase.GetCommand();
                    if (friendsCommand == generalCommand) {
                        commandSlot.text.text = "";
                    } else {
                        commandSlot.ResetCommandText();
                    }
                }
            }
        }
    }

    public void PlayerAttackStamp() {
        lastPlayerAttackTimeRemain = lastAttackTimeDuration;
    }

    public float GetElapsedTimeFromPlayerLastAttack() {
        float durationTemp = lastAttackTimeDuration;
        return Mathf.Clamp(durationTemp - lastPlayerAttackTimeRemain, 0f, durationTemp);
    }

    public void JustDodgeAmountPlus(float amount) {
        if (amount > 0f && pCon) {
            float playerKnock = pCon.GetKnock(true);
            if (playerKnock > 0f) {
                justDodgeAmount += amount / playerKnock * 0.125f;
                if (justDodgeAmount > 1f) {
                    justDodgeAmount = 1f;
                }
            }
        }
    }

    public bool GetPlayerAttacked() {
        return (lastPlayerAttackTimeRemain > 0);
    }

    public int GetPlayerMaxHP() {
        return (pCon ? pCon.GetMaxHP() : 0);
    }

    public float GetSuperHealHyper() {
        return (pCon && pCon.isHyper ? 2f : 1f);
    }

    public int GetPlayerMaxHPNoEffected() {
        return (pCon ? pCon.GetMaxHPNoEffected() : 0);
    }

    public int GetGutsBorder() {
        return GetPlayerMaxHPNoEffected() * 3 / 10;
    }

    public bool AddExp(int exp, bool force = false) {
        if (GetPlayerAttacked() || force) {
            if (pCon && pCon.IsDead) {
                return false;
            }
            int nowLevel = GameManager.Instance.save.Level;
            GameManager.Instance.save.exp += exp;
            if (GameManager.Instance.save.exp < 0) {
                GameManager.Instance.save.exp = 0;
            } else if (GameManager.Instance.save.exp > 2000000000) {
                GameManager.Instance.save.exp = 2000000000;
            }
            UpdateExpGage();
            int newLevel = GameManager.Instance.save.Level;
            if (newLevel > nowLevel && pCon) {
                EmitEffect((int)EffectDatabase.id.friendsLevelUp);
                if (MessageUI.Instance) {
                    MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_LEVELUP"), MessageUI.time_Important);
                }
            }
            if (newLevel != nowLevel) {
                if (pCon) {
                    pCon.LevelUp(newLevel);
                }
                for (int i = 0; i < friends.Length; i++) {
                    if (friends[i].fBase) {
                        friends[i].fBase.LevelUp(newLevel);
                    }
                }
                UpdateLevelLimit();
                if (newLevel >= GameManager.levelMax && TrophyManager.Instance) {
                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_LevelMax, true);
                }
            }
            return true;
        } else {
            return false;
        }
    }

    public void LevelReset() {
        int nowLevel = GameManager.Instance.save.Level;
        if (pCon) {
            pCon.Level = nowLevel;
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.Level = nowLevel;
            }
        }
    }

    public void UpdateExpGage() {
        if (GameManager.Instance.save.exp != expSave) {
            expSave = GameManager.Instance.save.exp;
            if (expFill) {
                expFill.fillAmount = GameManager.Instance.save.ExpRate;
            }
            if (expText) {
                expText.text = sb.Clear()
                    .Append(GameManager.Instance.save.NowLevelExp)
                    .Append(slash)
                    .Append(GameManager.Instance.save.NeedExpToNextLevel).ToString();
            }
        }
    }

    public void UpdateLevelLimit() {
        if (levelLimit != 0) {
            if (expText && expText.enabled) {
                expText.enabled = false;
            }
            if (levelLimitText) {
                sb.Clear();
                if (levelLimit > 0) {
                    sb.Append(TextManager.Get("WORD_LEVELLIMIT"));
                    sb.Append(" ").Append(Mathf.Min(GameManager.Instance.save.Level, levelLimit));
                } else {
                    sb.Append(TextManager.Get("WORD_LEVELLIMIT_AUTO"));
                    sb.Append(" ").Append(Mathf.Min(GameManager.Instance.save.Level, GameManager.Instance.levelLimitAuto));
                }
                levelLimitText.text = sb.ToString();
                if (!levelLimitText.enabled) {
                    levelLimitText.enabled = true;
                }
            }
        } else {
            if (expText && !expText.enabled) {
                expText.enabled = true;
            }
            if (levelLimitText && levelLimitText.enabled) {
                levelLimitText.enabled = false;
            }
        }
    }

    public void UpdateFriendsEffectDisabled() {
        if (friendsEffectDisabled != GameManager.Instance.save.config[GameManager.Save.configID_DisablePassiveSkills]) {
            friendsEffectDisabled = GameManager.Instance.save.config[GameManager.Save.configID_DisablePassiveSkills];
            if (disableFriendsEffectIcon && disableFriendsEffectIcon.enabled != (friendsEffectDisabled != 0)) {
                disableFriendsEffectIcon.enabled = (friendsEffectDisabled != 0);
            }
            CheckFriendsHPOver();
        }
    }

    public void UpdateHellMode() {
        hellMode = (GameManager.Instance.save.config[GameManager.Save.configID_HellMode] != 0);
        if (hellMode) {
            if (pCon && pCon.GetNowHP() > 1) {
                pCon.SetNowHP(1);
            }
            for (int i = 0; i < friends.Length; i++) {
                if (friends[i].fBase && friends[i].fBase.GetNowHP() > 1) {
                    friends[i].fBase.SetNowHP(1);
                }
            }
        }
    }

    public void CheckFriendsHPOver() {
        if (pCon) {
            int nowHP = pCon.GetNowHP();
            float nowST = pCon.GetNowST();
            if (nowHP > pCon.GetMaxHP()) {
                pCon.SetNowHP(nowHP);
            }
            if (nowST > pCon.GetMaxST()) {
                pCon.SetNowST(nowST);
            }
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                int nowHP = friends[i].fBase.GetNowHP();
                float nowST = friends[i].fBase.GetNowST();
                if (nowHP > friends[i].fBase.GetMaxHP()) {
                    friends[i].fBase.SetNowHP(nowHP);
                }
                if (nowST > friends[i].fBase.GetMaxST()) {
                    friends[i].fBase.SetNowST(nowST);
                }
            }
        }
    }

    public bool GetFriendsExist(int number, bool askLive = false) {
        return number >= 0 && number < friends.Length && friends[number].fBase && (!askLive || friends[number].fBase.GetNowHP() > 0);
    }

    public int GetFriendsOrder(int orderIndex) {
        if (orderIndex >= 0 && orderIndex < friendsOrder.Length) {
            int orderTemp = 0;
            for (int i = 0; i < friendsOrder.Length; i++) {
                if (friendsOrder[i] >= 0) {
                    if (orderTemp == orderIndex) {
                        return friendsOrder[i];
                    } else {
                        orderTemp++;
                    }
                }
            }
        }
        return -1;
    }

    public void UpdateFriendsGridScale() {
        if (GameManager.Instance.save.config[GameManager.Save.configID_FriendsIconSize] > 0) {
            if (!friendsCanvas.enabled) {
                friendsCanvas.enabled = true;
            }
            int slotCount = 0;
            for (int i = 0; i < friends.Length; i++) {
                if (friends[i].slotObject) {
                    slotCount++;
                }
            }
            float sizeMul = sizeMulArray[Mathf.Clamp(GameManager.Instance.save.config[GameManager.Save.configID_FriendsIconSize] - 1, 0, 10)];
            if (slotCount >= 0 && slotCount < friendsGridScale.Length) {
                friendsGridScaleSave = friendsGridScale[slotCount] * sizeMul;
                int childCount = slotPanel.childCount;
                RectTransform slotRect;
                for (int i = 0; i < childCount; i++) {
                    slotRect = slotPanel.GetChild(i).GetComponent<RectTransform>();
                    if (slotRect.localScale.x != friendsGridScaleSave) {
                        slotRect.localScale = vecOne * friendsGridScaleSave;
                    }
                }
                cellSize.y = cellSize.x = friendsGridSize[slotCount] * sizeMul;
                friendsGrid.cellSize = cellSize;
            }
        } else {
            if (friendsCanvas.enabled) {
                friendsCanvas.enabled = false;
            }
        }
    }

    void CreateSlot(int index) {
        if (friends[index].fBase && ItemDatabase.Instance) {
            if (friends[index].slotObject) {
                Destroy(friends[index].slotObject);
                friends[index].slotObject = null;
            }
            friends[index].slotObject = Instantiate(slotPrefab, slotPanel);
            friends[index].uiCon = friends[index].slotObject.GetComponent<UIImageFillController>();
            friends[index].separatedMark = friends[index].slotObject.GetComponent<ActivateObject>();
            if (friends[index].uiCon) {
                friends[index].uiCon.cBase = friends[index].fBase;
                friends[index].uiCon.frameImage.sprite = ItemDatabase.Instance.GetItemData(100 + GetFriendsID(index)).image;
            }
            friends[index].isSeparated = false;
            for (int i = 0; i < friendsOrder.Length; i++) {
                if (friendsOrder[i] < 0) {
                    friendsOrder[i] = index;
                    break;
                }
            }
            friendsSlotChanged = true;
        }
    }

    void UpdateStatusSlot() {
        float deltaTimeCache = Time.deltaTime;
        bool multiplied = false;
        for (int i = 0; i < buffSlot.Length; i++) {
            if (buffSlot[i].remainTime > 0f) {
                if (!multiplied) {
                    deltaTimeCache *= GetFriendsEffect(FriendsEffect.LongUse) * riskyIncSqrt;
                    multiplied = true;
                }
                buffSlot[i].remainTime -= deltaTimeCache;
            }
            if (buffSlot[i].remainTime > 0 && !buffSlot[i].slotObject) {
                buffSlot[i].slotObject = Instantiate(statusSlotPrefab, statusSlotPanel);
                buffSlot[i].slotImage = buffSlot[i].slotObject.GetComponent<Image>();
                buffSlot[i].slotImage.sprite = buffSlot[i].sprite;
                buffSlot[i].slotImage.fillAmount = 1f;
            } else if (buffSlot[i].slotObject) {
                if (buffSlot[i].remainTime <= 0f) {
                    Destroy(buffSlot[i].slotObject);
                    buffSlot[i].slotObject = null;
                } else if (buffSlot[i].slotImage){
                    if (buffSlot[i].remainTime < 10f) {
                        buffSlot[i].slotImage.fillAmount = buffSlot[i].remainTime * 0.1f;
                    } else if (buffSlot[i].slotImage.fillAmount != 1f) {
                        buffSlot[i].slotImage.fillAmount = 1f;
                    }
                }
            }
        }
    }

    void DestroyStatusSlot() {
        for (int i = 0; i < buffSlot.Length; i++) {
            if (buffSlot[i].slotObject) {
                Destroy(buffSlot[i].slotObject);
                buffSlot[i].slotObject = null;
            }
        }
    }

    void DeleteSlot(int index) {
        if (friends[index].fBase) {
            if (friends[index].slotObject) {
                if (comTargetIndex > 0) {
                    if (comTargetSlotObj == friends[index].slotObject) {
                        comTargetSlotObj = null;
                        comTargetIndex = 0;
                    } else {
                        for (int i = 0; i < comTargetIndex - 1; i++) {
                            if (slotPanel.GetChild(i).gameObject == friends[index].slotObject) {
                                comTargetIndex--;
                                break;
                            }
                        }
                    }
                }
                Destroy(friends[index].slotObject);
                friends[index].slotObject = null;
            }
            for (int i = 0; i < friendsOrder.Length; i++) {
                if (friendsOrder[i] == index) {
                    friendsOrder[i] = -1;
                    break;
                }
            }
            friendsSlotChanged = true;
        }
    }

    public void UseJapariman(int num) {
        int jmCount;
        jmCount = GameManager.Instance.save.NumOfSpecificItems(jmID);
        for (int i = 0; i < jmCount && num > 0; i++) {
            GameManager.Instance.save.RemoveItem(jmID);
            num -= 1;
        }
        if (num > 0) {
            jmCount = GameManager.Instance.save.NumOfSpecificItems(jm3ID);
            for (int i = 0; i < jmCount && num > 0; i++) {
                GameManager.Instance.save.RemoveItem(jm3ID);
                num -= 3;
            }
        }
        if (num > 0) {
            jmCount = GameManager.Instance.save.NumOfSpecificItems(jm5ID);
            for (int i = 0; i < jmCount && num > 0; i++) {
                GameManager.Instance.save.RemoveItem(jm5ID);
                num -= 5;
            }
        }
        if (num < 0) {
            num *= -1;
            if (num >= 3) {
                GameManager.Instance.save.AddItem(jm3ID);
                num -= 3;
            }
            while (num > 0 && GameManager.Instance.save.IsPossibleAddItem()) {
                GameManager.Instance.save.AddItem(jmID);
                num -= 1;
            }
            while (num > 0) {
                ItemDatabase.Instance.GiveItem(jmID, playerTrans.position, 1);
                num -= 1;
            }
        }
    }

    public Vector3 GetFriendsAppearPos(int numerator = 0, int denominator = -1) {
        if (playerTrans) {
            float angle = 0;
            if (denominator > 0) {
                angle = 360 * numerator / denominator + 90 + 180 / denominator;
                angle -= playerTrans.eulerAngles.y;
            } else {
                angle = Random.Range(0f, 360f);
            }
            float radian = angle * Mathf.Deg2Rad;
            Vector3 answer = playerTrans.position;
            answer.x += Mathf.Cos(radian) * 0.5f;
            answer.z += Mathf.Sin(radian) * 0.5f;
            ray.origin = answer + vecUp;
            ray.direction = vecDown;
            if (Physics.Raycast(ray, out raycastHit, 1f, appearPosLayerMask, QueryTriggerInteraction.Ignore)) {
                if (raycastHit.point.y > answer.y) {
                    answer = raycastHit.point;
                }
            }
            return answer;
        }
        else {
            return vecZero;
        }
    }

    public int CheckFriendsReviveAbility(int index, bool costFlag = true, bool ignoreLimit = false) {
        int answer = -3;
        if (index >= 0 && index < GameManager.friendsMax) {
            int costNum = GetFriendsCost(index);
            if (costFlag && GameManager.Instance.GetJaparimanCount() < costNum) {
                answer = -2;
            } else if (!ignoreLimit && GetFriendsSummonRemain() < costNum) {
                answer = -1;
            } else if (GetFriendsExist(index, false)) {
                answer = 0;
            } else {
                answer = 1;
            }
        }
        return answer;
    }

    public int ChangeFriends(int index, bool effectFlag = true, bool costFlag = true, bool ignoreLimit = false, int numerator = 0, int denominator = -1) {
        int answer = -3;
        //cost limit -1
        //japaribun shortage -2
        index = Mathf.Clamp(index, 0, friends.Length - 1);
        int id = index;
        if (extraFriendsIndex > 0 && id == extraFriendsID[0]) {
            id = extraFriendsID[extraFriendsIndex];
            friends[index].prefab = null;
        }
        if (playerTrans && friends[index].fBase == null) {
            if (GameManager.Instance.save.friends[index] != 0 && !friends[index].instance) {
                int costNum = CharacterDatabase.Instance.friends[id].cost;
                if (costFlag && GameManager.Instance.GetJaparimanCount() < costNum) {
                    answer = -2;
                } else if (!ignoreLimit && GetFriendsSummonRemain() < costNum) {
                    answer = -1;
                } else {
                    if (friends[index].prefab == null) {
                        friends[index].prefab = CharacterDatabase.Instance.GetFriends(id);
                    }
                    if (friends[index].prefab) {
                        friends[index].instance = Instantiate(friends[index].prefab, GetFriendsAppearPos(numerator, denominator), playerTrans.rotation);
                        if (friends[index].instance) {
                            friends[index].trans = friends[index].instance.transform;
                            friends[index].trans.LookAt(playerTrans);
                            friends[index].fBase = friends[index].instance.GetComponent<FriendsBase>();
                            if (index == specialFriendsID) {
                                specialFriendsBase = friends[index].fBase;
                            }
                            if (friends[index].fBase) {
                                if (costFlag) {
                                    UseJapariman(costNum);
                                }
                                bossResult.friendsCallingCost += costNum;
                                bossResult.friendsCallingCount += 1;
                                if (effectFlag && GameManager.Instance.save.stageReport.Length >= GameManager.stageReportMax) {
                                    GameManager.Instance.save.stageReport[GameManager.stageReport_FriendsSum] += costNum;
                                    GameManager.Instance.save.stageReport[GameManager.stageReport_FriendsCount] += 1;
                                }
                                friends[index].fBase.SetNowHP(friends[index].fBase.GetMaxHP());
                                friends[index].fBase.SetNowST(friends[index].fBase.GetMaxST());
                                friends[index].fBase.SetCommand(generalCommand);
                                if (effectFlag) {
                                    EmitEffect((int)EffectDatabase.id.friendsAppear, index);
                                }
                                if (pCon && pCon.isSuperman && GetEquipEffect(EquipEffect.AllWild) != 0) {
                                    friends[index].fBase.SupermanStart(true);
                                }
                                CreateSlot(index);
                                answer = 1;
                            } else {
                                Erase(index);
                            }
                        }
                    }
                }
            }
            return answer;
        } else {
            Erase(index);
            return 0;
        }
    }

    public bool IsFriendsActive(int index) {
        return friends[index].fBase;
    }

    public void Erase(int index, bool effectFlag = true) {
        if (friends[index].instance) {
            if (effectFlag) {
                EmitEffect((int)EffectDatabase.id.friendsDisappear, index, false);
            }
            DeleteSlot(index);
            Destroy(friends[index].instance);
            friends[index].instance = null;
            friends[index].fBase = null;
        }
    }

    public void FriendsClearAll(bool effectFlag = false) {
        for (int i = 0; i < friends.Length; i++) {
            Erase(i, false);
        }
    }

    public void SetAllFriendsCloth(int param) {
        if (pCon) {
            pCon.SetClothEnabled(param);
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.SetClothEnabled(param);
            }
        }
    }

    public void SetAllFriendsDynamicBone(int param) {
        if (pCon) {
            pCon.SetDynamicBoneEnabled(param);
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.SetDynamicBoneEnabled(param);
            }
        }
    }

    public void SetAllFriendsHeadLook(int param) {
        if (pCon) {
            pCon.SetHeadLookControllerEnabled(param);
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.SetHeadLookControllerEnabled(param);
            }
        }
    }

    public void SetAllFriendsWinAction(bool gravityZero = false, float forceGroundedTimePlus = 0f) {
        if (pCon && pCon.IsDead) {
            return;
        }
        if (pCon) {
            pCon.WinAction(gravityZero, forceGroundedTimePlus);
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.WinAction(gravityZero, forceGroundedTimePlus);
            }
        }
    }

    public void SelfErase(GameObject selfObj) {
        for (int i = 0; i < friends.Length; i++) {
            if (selfObj == friends[i].instance) {
                Erase(i);
                break;
            }
        }
    }
    
    public void WriteFriendsLiving() {
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase && friends[i].fBase.GetNowHP() > 0) {
                GameManager.Instance.save.friendsLiving[i] = 1;
            } else {
                GameManager.Instance.save.friendsLiving[i] = 0;
            }
        }
        GameManager.Instance.save.sandstar = sandstar;
    }

    public float GetBetweenPlayerDistance(Vector3 position, bool isSqrDist, bool ignoreY) {
        if (playerTrans) {
            if (ignoreY) {
                position.y = playerTrans.position.y;
            }
            if (isSqrDist) {
                return (position - playerTrans.position).sqrMagnitude;
            } else {
                return Vector3.Distance(position, playerTrans.position);
            }
        } else {
            return 0f;
        }
    }

    public Vector3 GetToPlayerVector(Vector3 fromPosition, bool ignoreY, bool normalize) {
        if (playerTrans) {
            if (ignoreY) {
                fromPosition.y = playerTrans.position.y;
            }
            if (normalize) {
                return (playerTrans.position - fromPosition).normalized;
            } else {
                return playerTrans.position - fromPosition;
            }
        } else {
            return vecZero;
        }
    }

    void SetBossBattleDistance(bool flag) {
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.ChangeActionDistance(flag);
            }
        }
    }

    public void SetBossHP(CharacterBase characterBase) {
        if (bossHPController) {
            if (characterBase) {
                bossHPController.cBase = characterBase;
                bossHP.enabled = true;
                isBossBattle = true;
                SetBossBattleDistance(true);
            } else {
                bossHP.enabled = false;
                bossHPController.cBase = null;
                isBossBattle = false;
                SetBossBattleDistance(false);
            }
        }
    }

    public void CleanDeadFriends() {
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase && friends[i].fBase.GetNowHP() <= 0) {
                Erase(i, false);
            }
        }
    }

    public GameObject GetFriendsObject(int index) {
        if (index < friends.Length && index >= 0 && friends[index].instance) {
            return friends[index].instance;
        } else {
            return null;
        }
    }

    public int GetPlayerID() {
        if (pCon) {
            return pCon.characterId;
        } else {
            return 1;
        }
    }

    public bool GetPlayerLive() {
        if (pCon) {
            return pCon.GetNowHP() > 0;
        } else {
            return false;
        }
    }

    public void SetPlayerUpdateEnabled(bool flag) {
        if (pCon) {
            pCon.controlEnabled = flag;
        }
    }
    
    public void TimeStop(bool stop) {
        GameManager.Instance.ChangeTimeScale(stop);
        SetPlayerUpdateEnabled(!stop);
    }

    public void ShowGold() {
        if (gold) {
            goldNum.text = GameManager.Instance.save.money.ToString();
            if (GameManager.Instance.save.config[GameManager.Save.configID_GoldPos] < goldCanvasPosition.Length) {
                goldTimeRemain = 3;
                gold.enabled = true;
            }
        }
    }

    public void HideGold() {
        if (gold) {
            goldTimeRemain = 0;
            showGoldContinuous = false;
            gold.enabled = false;
        }
    }

    public void SetFaceSpecificFriend(int id, FriendsBase.FaceName faceName, float timer = 5f) {
        if (id < 0) {
            if (pCon) {
                pCon.SetFaceSpecial(faceName, timer);
            }
        } else {
            if (GetFriendsExist(id, true)) {
                friends[id].fBase.SetFaceSpecial(faceName, timer);
            }
        }
    }

    public void SetFaceSpecificFriendString(int id, string faceName, float timer = 5f) {
        if (id < 0) {
            if (pCon) {
                pCon.SetFaceString(faceName, timer);
            }
        } else {
            if (GetFriendsExist(id, true)) {
                friends[id].fBase.SetFaceString(faceName, timer);
            }
        }
    }

    public void EveryFriendsSmile() {
        if (pCon) {
            pCon.SetFaceSmile();
        }
        for (int i = 0; i < friends.Length; i++) {
            if (GetFriendsExist(i, true)) {
                friends[i].fBase.SetFaceSmile();
            }
        }
    }

    public void EveryFriendsFear() {
        if (pCon) {
            pCon.SetFaceFear();
        }
        for (int i = 0; i < friends.Length; i++) {
            if (GetFriendsExist(i, true)) {
                friends[i].fBase.SetFaceFear();
            }
        }
    }

    public void AddActionEnemy(int id) {
        if (!actionEnemy.Contains(id)) {
            actionEnemy.Add(id);
        }
    }

    public void RemoveActionEnemy(int id) {
        if (actionEnemy.Contains(id)) {
            actionEnemy.Remove(id);
        }
    }

    public int GetActionEnemyCount() {
        return actionEnemy.Count;
    }

    public void ClearActionEnemy() {
        actionEnemy.Clear();
    }

    public void SetPlayerLightActive() {
        if (pCon && LightingDatabase.Instance) {
            pCon.SetPlayerLightActive(LightingDatabase.Instance.PlayerLight, LightingDatabase.Instance.LightType);
        }
    }

    public void SetPlayerLightActiveTemporal(bool flag, int type) {
        if (pCon) {
            pCon.SetPlayerLightActive(flag, type);
        }
    }

    public int GetIbisSongHeal() {
        if (GetFriendsExist(5, true) && pCon) {
            return Mathf.RoundToInt(pCon.GetMaxHP() * GetDifficultyEffect(DifficultyEffect.IbisHealRate) * riskyDecCubicRoot);
        } else {
            return 0;
        }
    }

    public void SetPlayerGutsRate(float rate) {
        bool flag = false;
        if (rate >= 1f) {
            flag = false;
            gutsFill.fillAmount = 0f;
            gutsFill.color = colorRed;
        } else {
            flag = true;
            gutsFill.fillAmount = Mathf.Clamp01(1f - rate);
            if (rate <= 0f) {
                gutsFill.color = colorBlack;
            } else {
                gutsFill.color = colorGray;
            }
        }
        if (gutsFill.enabled != flag) {
            gutsFill.enabled = flag;
        }
    }
    
    bool GetMainCamera() {
        if (mainCamera == null) {
            if (CameraManager.Instance) {
                CameraManager.Instance.SetMainCamera(ref mainCamera);
            } else {
                mainCamera = Camera.main;
            }
        }
        return mainCamera != null;
    }

    void GetNumberUIPrefab() {
        damagePrefab = new GameObject[damageColorTypeMax];
        for (int i = 0; i < damagePrefab.Length; i++) {
            damagePrefab[i] = CharacterDatabase.Instance.GetDamagePrefab(i);
        }
        for (int i = 0; i < damageCanvasMax; i++) {
            damageList[i] = new List<FadeTextSet>(128);
        }
        for (int i = 0; i < dodgeTextTypeMax; i++) {
            dodgeList[i] = new List<FadeTextSet>(128);
        }
        justDodgeHighPrefab = CharacterDatabase.Instance.ui.justDodgeHigh;
        justDodgeMiddlePrefab = CharacterDatabase.Instance.ui.justDodgeMiddle;
        justDodgeLowPrefab = CharacterDatabase.Instance.ui.justDodgeLow;
        gutsNormalPrefab = CharacterDatabase.Instance.ui.gutsNormal;
        gutsLastPrefab = CharacterDatabase.Instance.ui.gutsLast;
        getExpPrefab = CharacterDatabase.Instance.ui.getExp;
        defeatRemainPrefab = CharacterDatabase.Instance.ui.defeatRemain;
        fieldBuffHPPrefab = CharacterDatabase.Instance.ui.fieldBuffHP;
        fieldBuffSTPrefab = CharacterDatabase.Instance.ui.fieldBuffST;
        fieldBuffAttackPrefab = CharacterDatabase.Instance.ui.fieldBuffAttack;
        numberUiInitialized = true;
    }
    
    public void ShowDamage(ref Vector3 position, int num, int colorType = 0, bool isPlayer = false) {
        int damageSize = GameManager.Instance.save.config[GameManager.Save.configID_DamageSize];
        if (damageSize > 0 && (mainCamera || GetMainCamera())) {
            Vector3 sizeTemp = vecOne;
            float sizeFloat = sizeMulArray[Mathf.Clamp(damageSize - 1, 0, 10)];

            if (!isPlayer && GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] != 0) {
                sizeFloat *= Mathf.Clamp01(1.25f - log2Array[Mathf.Clamp(friendsCountSave, 0, log2Array.Length - 1)] * 0.1f);
                if (GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] >= 2) {
                    sizeFloat *= 0.8f;
                }
            }
            if (sizeFloat != 1f) {
                sizeTemp *= sizeFloat;
            }
            Vector2 pos = RectTransformUtility.WorldToScreenPoint(mainCamera, position);
            if (pos.x < damageFrameMin.x) {
                pos.x = damageFrameMin.x;
            } else if (pos.x > damageFrameMax.x) {
                pos.x = damageFrameMax.x;
            }
            if (pos.y < damageFrameMin.y) {
                pos.y = damageFrameMin.y;
            } else if (pos.y > damageFrameMax.y) {
                pos.y = damageFrameMax.y;
            }

            int canvasIndex = isPlayer ? colorType + damageColorTypeMax : colorType;
            int listCount = damageList[canvasIndex].Count;
            bool found = false;
            if (listCount > 0) {
                for (int i = 0; i < listCount; i++) {
                    if (damageList[canvasIndex][i].text == null || damageList[canvasIndex][i].rectTrans == null) {
                        damageList[canvasIndex].RemoveAt(i);
                        i--;
                        listCount--;
                    } else if (!damageList[canvasIndex][i].text.enabled) {

                        if (damageList[canvasIndex][i].rectTrans.localScale != sizeTemp) {
                            damageList[canvasIndex][i].rectTrans.localScale = sizeTemp;
                        }

                        damageList[canvasIndex][i].rectTrans.position = pos;
                        damageList[canvasIndex][i].text.text = num.ToString();
                        damageList[canvasIndex][i].text.color = colorWhite;
                        damageList[canvasIndex][i].lifeTime = damageTextLife;
                        damageList[canvasIndex][i].text.enabled = true;
                        found = true;
                        break;
                    }
                }
            }
            if (!found) {
                if (listCount < 128) {
                    GameObject damageInstance = Instantiate(damagePrefab[colorType], isPlayer ? damageParentsPlayer[colorType] : damageParentsFriends[colorType]);
                    RectTransform rectTrans = damageInstance.GetComponent<RectTransform>();
                    
                    if (rectTrans.localScale != sizeTemp) {
                       rectTrans.localScale = sizeTemp;
                    }

                    rectTrans.position = pos;
                    TMP_Text text = damageInstance.GetComponent<TMP_Text>();
                    text.text = num.ToString();
                    damageList[canvasIndex].Add(new FadeTextSet(text, rectTrans, damageTextLife, 1f));
                } else {
                    int minIndex = -1;
                    float minLife = float.MaxValue;
                    for (int i = 0; i < listCount; i++) {
                        if (damageList[canvasIndex][i].lifeTime < minLife) {
                            minLife = damageList[canvasIndex][i].lifeTime;
                            minIndex = i;
                        }
                    }
                    if (minIndex >= 0) {
                        
                        if (damageList[canvasIndex][minIndex].rectTrans.localScale != sizeTemp) {
                            damageList[canvasIndex][minIndex].rectTrans.localScale = sizeTemp;
                        }

                        damageList[canvasIndex][minIndex].rectTrans.position = pos;
                        damageList[canvasIndex][minIndex].text.text = num.ToString();
                        damageList[canvasIndex][minIndex].text.color = colorWhite;
                        damageList[canvasIndex][minIndex].lifeTime = damageTextLife;
                        damageList[canvasIndex][minIndex].text.enabled = true;
                    }
                }
            }
        }
    }

    public void ChangeDamageSize() {
        float playerSize = sizeMulArray[Mathf.Clamp(GameManager.Instance.save.config[GameManager.Save.configID_DamageSize] - 1, 0, 10)];
        float friendsSize = playerSize;
        if (GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] != 0) {
            friendsSize *= Mathf.Clamp01(1.25f - log2Array[Mathf.Clamp(friendsCountSave, 0, log2Array.Length - 1)] * 0.1f);
            if (GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] >= 2) {
                friendsSize *= 0.8f;
            }
        }
        Vector3 playerVec = vecOne * playerSize;
        Vector3 friendsVec = vecOne * friendsSize;
        for (int canvasIndex = 0; canvasIndex < damageCanvasMax; canvasIndex++) {
            int listCount = damageList[canvasIndex].Count;
            if (listCount > 0) {
                for (int i = 0; i < listCount; i++) {
                    if (damageList[canvasIndex][i].text == null || damageList[canvasIndex][i].rectTrans == null) {
                        damageList[canvasIndex].RemoveAt(i);
                        i--;
                        listCount--;
                    } else {
                        damageList[canvasIndex][i].rectTrans.localScale = canvasIndex < damageColorTypeMax ? friendsVec : playerVec;
                    }
                }
            }
        }
    }

    void UpdateDamageText() {
        if (Time.timeScale > 0f) {
            float deltaTimeCache = Time.deltaTime;
            Color colorTemp = colorWhite;
            for (int canvasIndex = 0; canvasIndex < damageCanvasMax; canvasIndex++) {
                int listCount = damageList[canvasIndex].Count;
                if (listCount > 0) {
                    for (int i = 0; i < listCount; i++) {
                        if (damageList[canvasIndex][i].text == null || damageList[canvasIndex][i].rectTrans == null) {
                            damageList[canvasIndex].RemoveAt(i);
                            i--;
                            listCount--;
                        } else {
                            damageList[canvasIndex][i].lifeTime -= deltaTimeCache;
                            if (damageList[canvasIndex][i].lifeTime <= 0f) {
                                damageList[canvasIndex][i].text.enabled = false;
                            } else if (damageList[canvasIndex][i].lifeTime < 1f) {
                                damageList[canvasIndex][i].rectTrans.Translate(100f * deltaTimeCache * vecUp);
                                colorTemp.a = damageList[canvasIndex][i].lifeTime;
                                damageList[canvasIndex][i].text.color = colorTemp;
                            }
                        }
                    }
                }
            }
        }
    }

    void SetDodgeTextContent(TMP_Text text, int type, int expNum = 0) {
        switch (type) {
            case dodgeText_Exp:
                text.text = StringUtils.Format("{0}Exp", expNum);
                break;
            case dodgeText_DefeatRemain:
                if (StageManager.Instance && StageManager.Instance.dungeonController) {
                    text.text = sb.Clear().AppendLine().Append(StageManager.Instance.dungeonController.GetDefeatMissionConditionText()).ToString();
                }
                break;
        }
    }

    void SetDodgeText(int type, Vector3 pos, int expNum = 0, float lifeTimeRate = 1f, float alphaRate = 1f) {
        int listCount = dodgeList[type].Count;
        bool found = false;
        if (listCount > 0) {
            for (int i = 0; i < listCount; i++) {
                if (dodgeList[type][i].text == null || dodgeList[type][i].rectTrans == null) {
                    dodgeList[type].RemoveAt(i);
                    i--;
                    listCount--;
                } else if (!dodgeList[type][i].text.enabled) {
                    dodgeList[type][i].rectTrans.position = pos;
                    dodgeList[type][i].text.color = colorWhite;
                    dodgeList[type][i].lifeTime = dodgeTextLife * lifeTimeRate;
                    dodgeList[type][i].alphaRate = dodgeTextAlphaSpeed * alphaRate;
                    SetDodgeTextContent(dodgeList[type][i].text, type, expNum);
                    dodgeList[type][i].text.enabled = true;
                    found = true;
                    break;
                }
            }
        }
        if (!found) {
            if (listCount < 128) {
                GameObject dodgeInstance;
                switch (type) {
                    case dodgeText_JustDodgeHigh:
                        dodgeInstance = Instantiate(justDodgeHighPrefab, justDodgeParent);
                        break;
                    case dodgeText_JustDodgeMiddle:
                        dodgeInstance = Instantiate(justDodgeMiddlePrefab, justDodgeParent);
                        break;
                    case dodgeText_JustDodgeLow:
                        dodgeInstance = Instantiate(justDodgeLowPrefab, justDodgeParent);
                        break;
                    case dodgeText_GutsNormal:
                        dodgeInstance = Instantiate(gutsNormalPrefab, gutsNormalParent);
                        break;
                    case dodgeText_GutsLast:
                        dodgeInstance = Instantiate(gutsLastPrefab, gutsLastParent);
                        break;
                    case dodgeText_DefeatRemain:
                        dodgeInstance = Instantiate(defeatRemainPrefab, getExpParent);
                        break;
                    case dodgeText_FieldBuffHP:
                        dodgeInstance = Instantiate(fieldBuffHPPrefab, getExpParent);
                        break;
                    case dodgeText_FieldBuffST:
                        dodgeInstance = Instantiate(fieldBuffSTPrefab, getExpParent);
                        break;
                    case dodgeText_FieldBuffAttack:
                        dodgeInstance = Instantiate(fieldBuffAttackPrefab, getExpParent);
                        break;
                    case dodgeText_Exp:
                    default:
                        dodgeInstance = Instantiate(getExpPrefab, getExpParent);
                        break;
                }
                RectTransform rectTrans = dodgeInstance.GetComponent<RectTransform>();
                rectTrans.position = pos;
                TMP_Text text = dodgeInstance.GetComponent<TMP_Text>();
                SetDodgeTextContent(text, type, expNum);

                dodgeList[type].Add(new FadeTextSet(text, rectTrans, dodgeTextLife * lifeTimeRate, alphaRate));
            } else {
                int minIndex = -1;
                float minLife = float.MaxValue;
                for (int i = 0; i < listCount; i++) {
                    if (dodgeList[type][i].lifeTime < minLife) {
                        minLife = dodgeList[type][i].lifeTime;
                        minIndex = i;
                    }
                }
                if (minIndex >= 0) {
                    dodgeList[type][minIndex].rectTrans.position = pos;
                    dodgeList[type][minIndex].text.color = colorWhite;
                    dodgeList[type][minIndex].lifeTime = dodgeTextLife * lifeTimeRate;
                    dodgeList[type][minIndex].alphaRate = alphaRate;
                    SetDodgeTextContent(dodgeList[type][minIndex].text, type, expNum);
                    dodgeList[type][minIndex].text.enabled = true;
                }
            }
        }
    }

    void UpdateDodgeText() {
        if (Time.timeScale > 0f) {
            float deltaTimeCache = Time.deltaTime;
            Color colorTemp = colorWhite;
            for (int type = 0; type < dodgeTextTypeMax; type++) {
                int listCount = dodgeList[type].Count;
                if (listCount > 0) {
                    for (int i = 0; i < listCount; i++) {
                        if (dodgeList[type][i].text == null || dodgeList[type][i].rectTrans == null) {
                            dodgeList[type].RemoveAt(i);
                            i--;
                            listCount--;
                        } else {
                            dodgeList[type][i].lifeTime -= deltaTimeCache;
                            if (dodgeList[type][i].lifeTime <= 0f) {
                                dodgeList[type][i].text.enabled = false;
                            } else {
                                dodgeList[type][i].rectTrans.Translate(50f * deltaTimeCache * vecUp);
                                colorTemp.a = Mathf.Clamp01(dodgeList[type][i].lifeTime * dodgeList[type][i].alphaRate);
                                dodgeList[type][i].text.color = colorTemp;
                            }
                        }
                    }
                }
            }
        }
    }

    public void ShowJustDodge(JustDodgeType justDodgeType, AttackDetection attacker = null) {
        if (pCon && playerTrans && pCon.GetNowHP() > 0) {
            if (mainCamera || GetMainCamera()) {
                Vector2 pos = RectTransformUtility.WorldToScreenPoint(mainCamera, playerTrans.position + vecUp * 0.5f);
                if (pos.y < 0f) {
                    pos.y = 0f;
                }
                SetDodgeText(justDodgeAmount >= 1f ? dodgeText_JustDodgeHigh : justDodgeAmount > 0f ? dodgeText_JustDodgeMiddle : dodgeText_JustDodgeLow, pos);
            }

            if (justDodgeAmount >= 1f) {
                definitelyDodgeTimeRemain = 0.5f;
            }
            pCon.HealForJustDodge(justDodgeAmount);
            for (int i = 0; i < friends.Length; i++) {
                if (friends[i].fBase) {
                    friends[i].fBase.HealForJustDodge(justDodgeAmount);
                }
            }
            justDodgeAmount -= 0.5f;
            if (justDodgeAmount < 0f) {
                justDodgeAmount = 0f;
            }
            if (attacker && attacker.parentCBase) {
                dodgeRecord.time = GameManager.Instance.time;
                dodgeRecord.attacker = attacker.parentCBase;
                dodgeRecord.receiver = pCon;
            }
            t_JustDodgeCount++;
            if (t_JustDodgeCount >= 10 && TrophyManager.Instance) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_JustDodgeContinuous, true);
            }
            if (GetFriendsEffectable(sneezeFriendsID)) {
                sneezeTimeRemain = 10f;
                if (GetFriendsExist(sneezeFriendsID, true)) {
                    Friends_PaintedWolf sneezeTemp = friends[sneezeFriendsID].fBase.GetComponent<Friends_PaintedWolf>();
                    if (sneezeTemp) {
                        sneezeTemp.SneezeSerif();
                        CheckTrophy_PaintedWolf();
                    }
                }
            }
            if (TrophyManager.Instance && attacker && attacker.parentCBase && attacker.parentCBase.isEnemy) {
                int enemyID = attacker.parentCBase.GetEnemyID();
                switch (enemyID) {
                    case 19:
                        if (!TrophyManager.Instance.IsTrophyHad(TrophyManager.t_AureliaJumpDodge) && justDodgeType == JustDodgeType.Jump) {
                            Enemy_Aurelia enemyBase = attacker.parentCBase.GetComponent<Enemy_Aurelia>();
                            if (enemyBase) {
                                enemyBase.CheckTrophy_JustDodge(attacker);
                            }
                        }
                        break;
                    case 40:
                    case 55:
                        if (!TrophyManager.Instance.IsTrophyHad(TrophyManager.t_AlligatorClipsEscape) && justDodgeType == JustDodgeType.QuickEscape) {
                            Enemy_AlligatorClips enemyBase = attacker.parentCBase.GetComponent<Enemy_AlligatorClips>();
                            if (enemyBase) {
                                enemyBase.CheckTrophy_QuickEscape();
                            }
                        }
                        break;
                    case 41:
                    case 45:
                    case 50:
                    case 51:
                    case 52:
                        if (true) {
                            Enemy_DarkServal enemyBase = attacker.parentCBase.GetComponent<Enemy_DarkServal>();
                            if (enemyBase) {
                                enemyBase.CheckTrophy_Pile();
                                if (attacker.reportType == AttackDetection.ReportType.DarkWave) {
                                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_DarkServalWave);
                                } else if (attacker.reportType == AttackDetection.ReportType.DarkInferno) {
                                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_DarkServalInferno);
                                }
                            }
                        }
                        break;
                    case 42:
                    case 56:
                        if (!TrophyManager.Instance.IsTrophyHad(TrophyManager.t_B2Needle)) {
                            Enemy_B2 enemyBase = attacker.parentCBase.GetComponent<Enemy_B2>();
                            if (enemyBase) {
                                enemyBase.CheckTrophy_NeedleAttacking(attacker);
                            }
                        }
                        break;
                    case 47:
                        t_GlaucusDodgeCount++;
                        if (t_GlaucusDodgeCount >= 10) {
                            TrophyManager.Instance.CheckTrophy(TrophyManager.t_Glaucus, true);
                            t_GlaucusDodgeCount = 0;
                        }
                        break;
                    case 48:
                    case 59:
                        if (!TrophyManager.Instance.IsTrophyHad(TrophyManager.t_SnowTower)) {
                            Enemy_Snowman enemyBase = attacker.parentCBase.GetComponent<Enemy_Snowman>();
                            if (enemyBase && enemyBase.CheckTrophy_IceBomb(attacker)) {
                                t_SnowTowerCount++;
                                if (t_SnowTowerCount >= 3) {
                                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_SnowTower, true);
                                    t_SnowTowerCount = 0;
                                }
                            }
                        }
                        break;
                    case 39:
                        if (!TrophyManager.Instance.IsTrophyHad(TrophyManager.t_Australopithecus)) {
                            Enemy_Australopithecus enemyBase = attacker.parentCBase.GetComponent<Enemy_Australopithecus>();
                            if (enemyBase && enemyBase.Level == 4) {
                                t_AustralopithecusCount++;
                                if (t_AustralopithecusCount >= 10) {
                                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_Australopithecus, true);
                                    t_AustralopithecusCount = 0;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }

    public void ShowGuts(Vector3 position, bool isLast = false) {
        if (mainCamera || GetMainCamera()) {
            Vector2 pos = RectTransformUtility.WorldToScreenPoint(mainCamera, position);
            if (pos.y < 0f) {
                pos.y = 0f;
            }
            SetDodgeText(isLast ? dodgeText_GutsLast : dodgeText_GutsNormal, pos);
        }
    }

    public void ShowGetExp(Vector3 position, int exp) {
        if ((mainCamera || GetMainCamera()) && exp > 0) {
            Vector2 pos = RectTransformUtility.WorldToScreenPoint(mainCamera, position);
            if (pos.y < 0f) {
                pos.y = 0f;
            }
            SetDodgeText(dodgeText_Exp, pos, exp);
        }
    }

    public void ShowDefeatRemain(Vector3 position) {
        if (mainCamera || GetMainCamera()) {
            Vector2 pos = RectTransformUtility.WorldToScreenPoint(mainCamera, position);
            if (pos.y < 0f) {
                pos.y = 0f;
            }
            SetDodgeText(dodgeText_DefeatRemain, pos);
        }
    }

    public void ShowFieldBuff(Vector3 position, FriendsBase.FieldBuffType type, bool isPlayer) {
        if (isPlayer || GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] <= 2) {
            Vector2 pos = RectTransformUtility.WorldToScreenPoint(mainCamera, position);
            if (pos.y < 0f) {
                pos.y = 0f;
            }
            float sizeFloat = 0.75f;
            if (!isPlayer && GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] != 0) {
                sizeFloat = (GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] == 1 ? 0.575f : 0.4f) * Mathf.Clamp01(1.25f - log2Array[Mathf.Clamp(friendsCountSave, 0, log2Array.Length - 1)] * 0.1f);
            }
            SetDodgeText(dodgeText_FieldBuffHP + (int)type, pos, 0, sizeFloat, 1f / 0.8f);
        }
    }

    public void CheckWeaponAll() {
        if (pCon) {
            pCon.checkWeaponPreservedFlag = true;
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.checkWeaponPreservedFlag = true;
            }
        }
    }

    void UpdateAnalyzer() {
        bool enableTemp = (GetEquipEffect(EquipEffect.Analyzer) != 0 && playerTargetEnemy != null && playerTargetEnemy.enemyID >= 0);
        if (enableTemp) {
            int languageTemp = GameManager.Instance.save.language;
            int enemyIDTemp = playerTargetEnemy.enemyID;
            int nowHPTemp = playerTargetEnemy.GetNowHP();
            int maxHPTemp = playerTargetEnemy.GetMaxHP();
            int attackTemp = (int)playerTargetEnemy.GetAttack(true);
            float multiplierTemp = playerTargetEnemy.GetAttackPowerMultiplier();
            bool stateTemp = playerTargetEnemy.IsAttacking();
            int defenseTemp = (int)playerTargetEnemy.GetDefense(true);
            int levelTemp = playerTargetEnemy.Level;
            bool isBossTemp = playerTargetEnemy.isBoss;
            bool variableTemp = playerTargetEnemy.variableLevelSettings.nowVariableLevel > 0;
            int defeatedTemp = 0;
            int defeatID = enemyIDTemp * GameManager.enemyLevelMax + levelTemp;
            bool attackShortenFlag = (languageTemp == GameManager.languageChinese && attackTemp >= 1000);
            bool defenseShortenFlag = (languageTemp == GameManager.languageChinese && defenseTemp >= 1000);
            if (defeatID >= 0 && defeatID < GameManager.Instance.save.defeatEnemy.Length) {
                defeatedTemp = GameManager.Instance.save.defeatEnemy[defeatID];
                if (defeatedTemp > 99999) {
                    defeatedTemp = 99999;
                }
            }
            if (enemyIDTemp != analyzerEnemyID) {
                analyzerEnemyID = enemyIDTemp;
                analyzerTexts[0].text = TextManager.Get(analyzerString_CellienName + analyzerEnemyID.ToString("00"));
            }
            if (languageTemp != analyzerLanguage) {
                analyzerTexts[1].text = TextManager.Get(analyzerString_HP);
            }
            if (nowHPTemp != analyzerNowHP || maxHPTemp != analyzerMaxHP) {
                analyzerNowHP = nowHPTemp;
                analyzerMaxHP = maxHPTemp;
                analyzerTexts[2].text = sb.Clear().Append(analyzerNowHP).Append(slash).Append(analyzerMaxHP).ToString();
            }
            if (attackTemp != analyzerAttackPower || multiplierTemp != analyzerAttackMultiplier || stateTemp != analyzerStateIsAttack || languageTemp != analyzerLanguage) {
                analyzerAttackPower = attackTemp;
                analyzerAttackMultiplier = multiplierTemp;
                analyzerStateIsAttack = stateTemp;
                if (analyzerStateIsAttack) {
                    analyzerTexts[3].text = sb.Clear().Append(TextManager.Get(analyzerString_Attack)).Append(attackShortenFlag ? "" : space).Append(analyzerAttackPower).Append(asterisk).Append(analyzerAttackMultiplier.ToString("0.00")).ToString();
                } else {
                    analyzerTexts[3].text = sb.Clear().Append(TextManager.Get(analyzerString_Attack)).Append(attackShortenFlag ? "" : space).Append(analyzerAttackPower).ToString();
                }
            }
            if (defenseTemp != analyzerDefense || languageTemp != analyzerLanguage) {
                analyzerDefense = defenseTemp;
                analyzerTexts[4].text = sb.Clear().Append(TextManager.Get(analyzerString_Defense)).Append(defenseShortenFlag ? "" : space).Append(analyzerDefense).ToString();
            }
            if (levelTemp != analyzerLevel || isBossTemp != analyzerIsBoss || languageTemp != analyzerLanguage) {
                analyzerLevel = levelTemp;
                analyzerIsBoss = isBossTemp;
                if (analyzerIsBoss) {
                    analyzerTexts[5].text = TextManager.Get(sb.Clear().Append(analyzerString_BossLevel).Append(analyzerLevel).ToString());
                } else {
                    sb.Clear().Append(TextManager.Get(analyzerString_Level)).Append(space).Append(analyzerLevel);
                    if (variableTemp) {
                        sb.Append("?");
                    }
                    analyzerTexts[5].text = sb.ToString();
                }
            }
            if (defeatedTemp != analyzerDefeated || languageTemp != analyzerLanguage) {
                analyzerDefeated = defeatedTemp;
                analyzerTexts[6].text = sb.Clear().Append(TextManager.Get(analyzerString_Defeated)).Append(space).Append(analyzerDefeated).ToString();
            }
            analyzerLanguage = languageTemp;
        }
        if (analyzerEnabled != enableTemp) {
            analyzerEnabled = enableTemp;
            analyzerCanvas.enabled = analyzerEnabled;
        }
    }

    public void SacrificeOnPlayerResurrection() {
        if (GetFriendsExist(31, true)) {
            Friends_AnotherServal asTemp = friends[31].fBase.GetComponent<Friends_AnotherServal>();
            if (asTemp) {
                asTemp.SacrificeOnPlayerResurrection();
            }
        }
    }

    public void SetEndBossMessage(bool sandstarRawEnabled) {
        if (sandstarRawEnabled) {
            MessageUI.Instance.SetMessage(TextManager.Get(bossResult.difficultyMin >= GameManager.difficultyCR ? "MESSAGE_DEFEAT_SINWR_CR" : "MESSAGE_DEFEAT_SINWR"), MessageUI.time_Important);
        } else {
            MessageUI.Instance.SetMessage(TextManager.Get(bossResult.difficultyMin >= GameManager.difficultyCR ? "MESSAGE_DEFEAT_BOSS_CR" : "MESSAGE_DEFEAT_BOSS"), MessageUI.time_Important);
        }
    }

    void SetBossTimeText() {
        int decimalTemp = (int)(bossTimeDecimal * 100);
        if (bossTimeInteger >= 3600) {
            bossTimeText.text = sb.Clear().Append((bossTimeInteger / 3600).ToString("00")).Append(":").Append((bossTimeInteger % 3600 / 60).ToString("00")).Append("\'").Append((bossTimeInteger % 60).ToString("00")).Append("\"").ToString();
        } else {
            bossTimeText.text = sb.Clear().Append((bossTimeInteger / 60).ToString("00")).Append("\'").Append((bossTimeInteger % 60).ToString("00")).Append("\"").Append(decimalTemp.ToString("00")).ToString();
        }
    }

    public bool DefinitelyDodge() {
        return definitelyDodgeTimeRemain > 0f;
    }

    public void BossTimeInit() {
        if (!bossTimeContinuing && !bossTimeInitialized) {
            bossTimeInteger = 0;
            bossTimeDecimal = 0f;
            bossTimeState = 1;
            bossTimeText.colorGradientPreset = bossTimeNormal;
            SetBossTimeText();
            bossTimeInitialized = true;
        }
    }

    public void BossTimeStart() {
        if (bossTimeState == 0) {
            BossTimeInit();
        }
        bossTimeState = 2;
        bossTimeText.colorGradientPreset = bossTimeNormal;
    }

    public void BossTimeEnd() {
        if (!bossTimeContinuing) {
            bossTimeState = 3;
            bossTimeText.colorGradientPreset = bossTimeEnd;
            SetBossTimeText();
        }
    }

    public void BossTimeHide() {
        bossTimeState = 0;
        bossTimeInitialized = false;
    }

    public void BossTimeStop() {
        bossTimeState = 1;
    }

    public int GetBossTime100() {
        return bossTimeInteger * 100 + (int)(bossTimeDecimal * 100);
    }

    public int GetBossTimeInteger() {
        return bossTimeInteger;
    }

    public void InitBossResult() {
        playerDamageSum = 0;
        playerDamageCount = 0;
        bossResult.difficultyMin = GameManager.Instance.save.difficulty;
        bossResult.friendsCallingCost = GetFriendsCostSum();
        bossResult.friendsCallingCount = GetFriendsCount();
        bossResult.minmiBlueFlag = GameManager.Instance.minmiBlue;
        bossResult.minmiPurpleFlag = GameManager.Instance.minmiPurple;
        bossResult.minmiBlackFlag = (StageManager.Instance && StageManager.Instance.dungeonController ? StageManager.Instance.dungeonController.bossMinmiBlackFlag : false);
        bossResult.minmiSilverFlag = GameManager.Instance.minmiSilver;
        bossResult.minmiGoldenFlag = GameManager.Instance.minmiGolden;
        bossResult.useItemCount = GetBuffCount(false);
        if (bossResult.canvas.enabled) {
            bossResult.canvas.enabled = false;
        }
    }

    public void ShowBossResult(int enemyID, bool sinWR) {
        if (enemyID >= 0) {
            int iconCount = 0;
            bossResult.iconImages[iconCount].sprite = bossResult.difficultySprites[Mathf.Clamp(bossResult.difficultyMin - 1, 0, bossResult.difficultySprites.Length - 1)];
            iconCount++;
            if (sinWR) {
                bossResult.iconImages[iconCount].sprite = bossResult.sinWRSprite;
                iconCount++;
            }
            if (bossResult.minmiBlueFlag) {
                bossResult.iconImages[iconCount].sprite = bossResult.minmiSprite[minmiBlueIndex];
                iconCount++;
            }
            if (bossResult.minmiPurpleFlag) {
                bossResult.iconImages[iconCount].sprite = bossResult.minmiSprite[minmiPurpleIndex];
                iconCount++;
            }
            if (bossResult.minmiBlackFlag) {
                bossResult.iconImages[iconCount].sprite = bossResult.minmiSprite[minmiBlackIndex];
                iconCount++;
            }
            if (bossResult.minmiSilverFlag) {
                bossResult.iconImages[iconCount].sprite = bossResult.minmiSprite[minmiSilverIndex];
                iconCount++;
            }
            if (bossResult.minmiGoldenFlag) {
                bossResult.iconImages[iconCount].sprite = bossResult.minmiSprite[minmiGoldenIndex];
                iconCount++;
            }
            for (int i = 0; i < bossResult.iconImages.Length; i++) {
                bossResult.iconImages[i].enabled = (i < iconCount);
            }
            bossResult.text1.text =
                sb.Clear().
                Append(TextManager.Get("RESULT_NAME")).AppendLine().
                Append(TextManager.Get(analyzerString_CellienName + enemyID.ToString("00"))).AppendLine().
                Append(TextManager.Get("RESULT_DIFFICULTY")).ToString();
            bossResult.text2.text =
                sb.Clear().
                Append(TextManager.Get("RESULT_RECEIVEDDAMAGE")).AppendLine().
                AppendFormat("{0} ({1} {2})", playerDamageSum, playerDamageCount, TextManager.Get("AMUSEMENT_TIMES")).AppendLine().
                Append(TextManager.Get("RESULT_FRIENDS")).AppendLine().
                AppendFormat("{0} {1} ({2} {3})", TextManager.Get("RESULT_COST"), bossResult.friendsCallingCost, bossResult.friendsCallingCount, TextManager.Get("RESULT_TIMES")).AppendLine().
                Append(TextManager.Get("RESULT_ITEM")).AppendLine().
                AppendFormat("{0} {1}", bossResult.useItemCount, TextManager.Get("RESULT_TIMES")).ToString();
            bossResult.canvas.enabled = true;
        }
    }

    public void ShowStageResult(int stageNumber) {
        if (GameManager.Instance.save.stageReport.Length >= GameManager.stageReportMax && GameManager.Instance.save.stageReport[GameManager.stageReport_PlayTime] > 0) {
            bossTimeInteger = GameManager.Instance.save.totalPlayTime - GameManager.Instance.save.stageReport[GameManager.stageReport_PlayTime];
            bossTimeDecimal = 0;
            bossTimeState = 3;
            bossTimeText.colorGradientPreset = bossTimeEnd;
            SetBossTimeText();

            int iconCount = 0;
            bossResult.iconImages[iconCount].sprite = bossResult.difficultySprites[Mathf.Clamp(GameManager.Instance.save.difficulty - 1, 0, bossResult.difficultySprites.Length)];
            iconCount++;
            for (int i = 0; i < bossResult.minmiSprite.Length; i++) {
                if ((GameManager.Instance.save.stageReport[GameManager.stageReport_Minmi] & (1 << i)) != 0) {
                    bossResult.iconImages[iconCount].sprite = bossResult.minmiSprite[i];
                    iconCount++;
                }
            }
            for (int i = 0; i < bossResult.iconImages.Length; i++) {
                bossResult.iconImages[i].enabled = (i < iconCount);
            }
            bossResult.text1.text =
                sb.Clear().
                Append(TextManager.Get("RESULT_NAME")).AppendLine().
                Append(TextManager.Get("STAGE_" + stageNumber.ToString("00"))).AppendLine().
                Append(TextManager.Get("RESULT_DIFFICULTY")).ToString();
            bossResult.text2.text =
                sb.Clear().
                Append(TextManager.Get("RESULT_RECEIVEDDAMAGE")).AppendLine().
                AppendFormat("{0} ({1} {2})", GameManager.Instance.save.stageReport[GameManager.stageReport_DamageSum], GameManager.Instance.save.stageReport[GameManager.stageReport_DamageCount], TextManager.Get("AMUSEMENT_TIMES")).AppendLine().
                Append(TextManager.Get("RESULT_FRIENDS")).AppendLine().
                AppendFormat("{0} {1} ({2} {3})", TextManager.Get("RESULT_COST"), GameManager.Instance.save.stageReport[GameManager.stageReport_FriendsSum], GameManager.Instance.save.stageReport[GameManager.stageReport_FriendsCount], TextManager.Get("RESULT_TIMES")).AppendLine().
                Append(TextManager.Get("RESULT_ITEM")).AppendLine().
                AppendFormat("{0} {1}", GameManager.Instance.save.stageReport[GameManager.stageReport_ItemCount], TextManager.Get("RESULT_TIMES")).ToString();
            bossResult.canvas.enabled = true;
        }
    }

    public void AchievementShow(string stageName, int rate, int stageNumber) {
        achievement.stageName.text = TextManager.Get(stageName);
        Vector2Int levelRange = vec2IntZero;
        if (StageManager.Instance && stageNumber >= 0 && stageNumber < StageManager.Instance.stageSettings.Length) {
            levelRange = StageManager.Instance.stageSettings[stageNumber].recommendedLevel;
        }
        if (levelRange.x <= 0 && levelRange.y <= 0) {
            achievement.recommendedLevel.text = sb.Clear().Append(TextManager.Get("WORD_RCMDLEVEL")).Append(" ").Append(TextManager.Get("WORD_RCMDLEVEL_UNKNOWN")).ToString();
        } else {
            achievement.recommendedLevel.text = sb.Clear().Append(TextManager.Get("WORD_RCMDLEVEL")).Append(" ").Append(levelRange.x > 0 ? levelRange.x.ToString() : TextManager.Get("WORD_RCMDLEVEL_UNKNOWN")).Append(GameManager.Instance.save.language == GameManager.languageJapanese ? "～" : "-").Append(levelRange.y > 0 ? levelRange.y.ToString() : TextManager.Get("WORD_RCMDLEVEL_UNKNOWN")).ToString();
        }
        achievement.title.text = TextManager.Get("WORD_ACHIEVEMENTRATE");
        if (rate >= 100) {
            achievement.rate.colorGradientPreset = achievement.completeColor;
        } else {
            achievement.rate.colorGradientPreset = achievement.normalColor;
        }
        if (stageNumber < GameManager.Instance.save.clearDifficulty.Length && GameManager.Instance.save.clearDifficulty[stageNumber] > 0) {
            if (achievement.clearedTextPos.Length > 1) {
                achievement.title.rectTransform.anchoredPosition = achievement.clearedTextPos[0];
                achievement.rate.rectTransform.anchoredPosition = achievement.clearedTextPos[1];
            }
            achievement.clearDifficulty.sprite = bossResult.difficultySprites[Mathf.Clamp(GameManager.Instance.save.clearDifficulty[stageNumber] - 1, 0, bossResult.difficultySprites.Length - 1)];
            achievement.clearDifficulty.enabled = true;
        } else {
            if (achievement.defaultTextPos.Length > 1) {
                achievement.title.rectTransform.anchoredPosition = achievement.defaultTextPos[0];
                achievement.rate.rectTransform.anchoredPosition = achievement.defaultTextPos[1];
            }
            achievement.clearDifficulty.enabled = false;
            achievement.clearDifficulty.sprite = null;
        }
        achievement.rate.text = sb.Clear().AppendFormat("{0}%", rate).ToString();
        if (!achievement.canvas.enabled) {
            achievement.canvas.enabled = true;
        }
    }

    public void AchievementHide() {
        if (achievement.canvas.enabled) {
            achievement.canvas.enabled = false;
        }
    }


    public void MoveCursorCommand(out Vector2Int move, bool continuous = false, float interval = 0.1f) {
        move = vec2IntZero;
        axisInput = vec2Zero;
        axisInput.x = playerInput.GetAxis(RewiredConsts.Action.Command_Horizontal);
        axisInput.y = playerInput.GetAxis(RewiredConsts.Action.Command_Vertical);
        if (axisInput.x >= -0.6f && axisInput.x <= 0.6f && axisInput.y >= -0.6f && axisInput.y <= 0.6f) {
            neutral = true;
        }
        if (neutral) {
            if (axisInput.x < -0.7f) {
                move.x = -1;
                neutral = false;
            } else if (axisInput.x > 0.7f) {
                move.x = 1;
                neutral = false;
            }
            if (axisInput.y < -0.7f) {
                move.y = 1;
                neutral = false;
            } else if (axisInput.y > 0.7f) {
                move.y = -1;
                neutral = false;
            }
        }
        if (continuous) {
            if (move != vec2IntZero) {
                moveCursorTimeRemain = 0.3f;
            }
            if (moveCursorTimeRemain > 0) {
                moveCursorTimeRemain -= Time.unscaledDeltaTime;
            }
            if (moveCursorTimeRemain <= 0) {
                if (axisInput.x < -0.7f) {
                    move.x = -1;
                } else if (axisInput.x > 0.7f) {
                    move.x = 1;
                }
                if (axisInput.y < -0.7f) {
                    move.y = 1;
                } else if (axisInput.y > 0.7f) {
                    move.y = -1;
                }
                if (move != vec2IntZero) {
                    moveCursorTimeRemain = interval;
                }
            }
        }
    }

    CharacterBase.Command NextCommand(CharacterBase.Command nowCommand, int move) {
        int nextCommandIndex = ((int)nowCommand + move + 4) % 4;
        if (nextCommandIndex == 0) {
            return CharacterBase.Command.Default;
        } else if (nextCommandIndex == 1) {
            return CharacterBase.Command.Evade;
        } else if (nextCommandIndex == 2) {
            return CharacterBase.Command.Ignore;
        }
        return CharacterBase.Command.Free;
    }

    void ChangeCommand() {
        MoveCursorCommand(out Vector2Int move, true, 0.1f);
        if (move.x != 0) {
            int childCount = slotPanel.childCount;
            if (childCount > 0) {
                comTargetIndex = (comTargetIndex + move.x + childCount + 1) % (childCount + 1);
                comTargetSlotObj = comTargetIndex <= 0 ? null : slotPanel.GetChild(comTargetIndex - 1).gameObject;
                UISE.Instance.Play(UISE.SoundName.move);
                for (int i = 0; i < childCount; i++) {
                    CommandSlot commandSlot = slotPanel.GetChild(i).GetComponent<CommandSlot>();
                    if (commandSlot) {
                        if (i == comTargetIndex - 1) {
                            if (!commandSlot.targetMark.activeSelf) {
                                commandSlot.targetMark.SetActive(true);
                            }
                        } else {
                            if (commandSlot.targetMark.activeSelf) {
                                commandSlot.targetMark.SetActive(false);
                            }
                        }
                    }
                }
            } else {
                comTargetIndex = 0;
            }
        }
        if (move.y != 0) {
            if (comTargetIndex <= 0) {
                SetGeneralCommand(NextCommand(generalCommand, move.y), true);
                UISE.Instance.Play(UISE.SoundName.use);
            } else if (comTargetSlotObj != null) {
                UIImageFillController uiFill = comTargetSlotObj.GetComponent<UIImageFillController>();
                if (uiFill && uiFill.cBase) {
                    FriendsBase fBaseTemp = uiFill.cBase.GetComponent<FriendsBase>();
                    SetSpecificCommand(fBaseTemp.friendsId, NextCommand(fBaseTemp.GetCommand(), move.y), true);
                    UISE.Instance.Play(UISE.SoundName.use);
                }
            }
        }
    }

    public MessageBackColor GetMessageBackColor(int index = -1) {
        if (index >= 0 && index < GameManager.friendsMax) {
            if (friends[index].fBase != null) {
                return friends[index].fBase.GetMessageBackColor();
            }
        } else {
            if (pCon) {
                return pCon.GetMessageBackColor();
            }
        }
        return null;
    }

    public void SetForParkman(bool flag) {
        if (parkmanSettings.activated != flag) {
            parkmanSettings.activated = flag;
            if (flag) {
                parkmanSettings.score.colorGradientPreset = GameManager.Instance.minmiPurple ? parkmanSettings.scoreHardColor : parkmanSettings.scoreNormalColor;
            }
            for (int i = 0; i < parkmanSettings.offObj.Length; i++) {
                parkmanSettings.offObj[i].SetActive(!flag);
            }
            for (int i = 0; i < parkmanSettings.onObj.Length; i++) {
                parkmanSettings.onObj[i].SetActive(flag);
            }
        }
    }

    public void AddLoudLoopInfo(LoudLoopType loudLoopType, Transform selfTransform, AudioSource selfAudio) {
        if (recordsInitialized) {
            for (int i = 0; i < loudLoopInfo.Length; i++) {
                if (loudLoopInfo[i].instanceTransform == null || loudLoopInfo[i].audioSource == null) {
                    loudLoopInfo[i].type = loudLoopType;
                    loudLoopInfo[i].instanceTransform = selfTransform;
                    loudLoopInfo[i].audioSource = selfAudio;
                    break;
                }
            }
        }
    }

    public void UseItemCountIncrement() {
        bossResult.useItemCount++;
        if (GameManager.Instance.save.stageReport.Length >= GameManager.stageReportMax) {
            GameManager.Instance.save.stageReport[GameManager.stageReport_ItemCount]++;
        }
    }

    public void PlaceFriendsAroundPlayer() {
        if (playerTrans) {
            Vector3 pivot = playerTrans.position;
            Vector3 temp = vecZero;
            for (int i = 0; i < friends.Length; i++) {
                if (GetFriendsExist(i)) { 
                    Vector2 circle = Random.insideUnitCircle;
                    friends[i].instance.SetActive(false);
                    if (friends[i].fBase) {
                        friends[i].fBase.SetAgentEnabled(false);
                    }
                    temp.x = circle.x;
                    temp.z = circle.y;
                    friends[i].instance.transform.position = pivot + temp;
                    friends[i].instance.SetActive(true);
                }
            }
        }
    }

    public void AgentOffOnAll() {
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.ResetAgent();
            }
        }
    }
    
    public void SetStaminaAlert() {
        if (staminaAlertTimeRemain <= 0f) {
            staminaAlertImgInterval = 0.25f;
            if (staminaAlertImage && !staminaAlertImage.enabled) {
                staminaAlertImage.enabled = true;
            }
        }
        if (staminaAlertSEInterval <= 0f) {
            if (staminaAlertInstance) {
                Destroy(staminaAlertInstance);
            }
            int stAlarmParam = GameManager.Instance.save.config[GameManager.Save.configID_StAlarm];
            if (stAlarmParam != 0) {
                staminaAlertSEInterval = 0.3f;
                staminaAlertInstance = Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.alarmST]);
                AudioSource audioTemp = staminaAlertInstance.GetComponent<AudioSource>();
                audioTemp.volume = (stAlarmParam == 1 ? 4f / 6f : stAlarmParam == 2 ? 5f / 6f : 1f);
                audioTemp.Play();
            }
        }
        staminaAlertTimeRemain = 1.25f;
    }

    public void CancelStaminaAlert() {
        staminaAlertTimeRemain = 0f;
        staminaAlertImgInterval = 0f;
        if (staminaAlertImage && staminaAlertImage.enabled) {
            staminaAlertImage.enabled = false;
        }
    }

    public void SetHPAlert() {
        if (hpAlertInstance) {
            Destroy(hpAlertInstance);
        }
        int hpAlarmParam = GameManager.Instance.save.config[GameManager.Save.configID_HpAlarm];
        if (hpAlarmParam != 0) {
            hpAlertInstance = Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.alarmHP]);
            AudioSource audioTemp = hpAlertInstance.GetComponent<AudioSource>();
            audioTemp.volume = (hpAlarmParam == 1 ? 0.6f : hpAlarmParam == 2 ? 0.8f : 1f);
            audioTemp.Play();
        }
    }

    public void CheckTrophy_Ibis() {
        if (!playerDead && GetFriendsExist(5)) {
            t_IbisSongDefeatCount++;
            if (t_IbisSongDefeatCount >= 5) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_Ibis, true);
                t_IbisSongDefeatCount = 0;
            }
        }
    }

    public void CheckTrophy_Tsuchinoko() {
        if (!playerDead) {
            t_TsuchinokoDefeatCount++;
            if (t_TsuchinokoDefeatCount >= 5) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_PlugAdapterTsuchinoko, true);
                t_TsuchinokoDefeatCount = 0;
            }
        }
    }

    public void CheckTrophy_Moose() {
        if (!playerDead) {
            t_MooseBreakCount++;
            if (t_MooseBreakCount >= 50) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_Moose, true);
                t_MooseBreakCount = 0;
            }
        }
    }

    public void CheckTrophy_PPP() {
        if (!playerDead) {
            t_PPPDefeatCount++;
            if (t_PPPDefeatCount >= 5) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_ClionePPP, true);
                t_PPPDefeatCount = 0;
            }
        }
    }

    public void CheckTrophy_RedFox() {
        if (!playerDead) {
            t_RedFoxDefeatCount++;
            if (t_RedFoxDefeatCount >= 5) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_SmartBallRedFox, true);
                t_RedFoxDefeatCount = 0;
            }
        }
    }

    public void CheckTrophy_ResetCountByDamage() {
        t_SnowTowerCount = 0;
    }

    public void CheckTrophy_CampoFlicker() {
        if (!playerDead) {
            t_CampoFlickerCount++;
            if (t_CampoFlickerCount >= 10) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_CampoFlicker, true);
                t_CampoFlickerCount = 0;
            }
        }
    }

    public void CheckTrophy_Giraffe(CharacterBase attackerCB) {
        if (!TrophyManager.Instance.IsTrophyHad(TrophyManager.t_Giraffe) && !playerDead && GetBuff(BuffType.Long) && GetFriendsExist(25) && attackerCB == friends[25].fBase) { 
            t_GiraffeDefeatCount++;
            if (t_GiraffeDefeatCount >= 3) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_Giraffe, true);
                t_GiraffeDefeatCount = 0;
            }
        }
    }

    public void CheckTrophy_Gather_ForPlayer() {
        if (!playerDead && t_QueenSafeArea && GetAnyFriendsExist()) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_Queen, true);
        }
    }

    public void CheckTrophy_Gather_ForQueen(bool flag) {
        t_QueenSafeArea = flag;
    }

    public void CheckTrophy_BrownBear(CharacterBase attackerCB) {
        if (!TrophyManager.Instance.IsTrophyHad(TrophyManager.t_BrownBear) && !playerDead && GetBuff(BuffType.Stamina) && GetBuff(BuffType.Attack) && GetFriendsExist(27) && attackerCB == friends[27].fBase) {
            Friends_BrownBear friendsTemp = friends[27].fBase.GetComponent<Friends_BrownBear>();
            if (friendsTemp && friendsTemp.CheckTrophy_IsSkillAttacking()) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_BrownBear, true);
            }
        }
    }

    public void CheckTrophy_PaintedWolf() {
        if (!TrophyManager.Instance.IsTrophyHad(TrophyManager.t_PaintedWolf)) {
            t_PaintedWolfCount++;
            if (t_PaintedWolfCount >= 10) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_PaintedWolf, true);
                t_PaintedWolfCount = 0;
            }
        }
    }

    public void CheckTrophy_Fennec() {
        if (!TrophyManager.Instance.IsTrophyHad(TrophyManager.t_Fennec)) {
            t_FennecDefeatCount++;
            if (t_FennecDefeatCount >= 3) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_Fennec, true);
                t_FennecDefeatCount = 0;
            }
        }
    }

    public void DestroyAllFriendsScaffold() {
        if (pCon) {
            pCon.SetScaffoldEffect(false);
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.SetScaffoldEffect(false);
            }
        }
    }

    public void SetAnimDeadSpecialAll(bool flag) {
        if (pCon) {
            pCon.SetAnimDeadSpecial(flag);
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.SetAnimDeadSpecial(flag);
            }
        }
    }

    public void SetPositionHeightMaxAll(float maxHeight) {
        if (playerTrans && playerTrans.position.y > maxHeight) {
            Vector3 posTemp = playerTrans.position;
            posTemp.y = maxHeight;
            playerTrans.position = posTemp;
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].trans && friends[i].trans.position.y > maxHeight) {
                Vector3 posTemp = friends[i].trans.position;
                posTemp.y = maxHeight;
                friends[i].trans.position = posTemp;
            }
        }
        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyObjs.Length > 0) {
            for (int i = 0; i < enemyObjs.Length; i++) {
                if (enemyObjs[i].transform.position.y > maxHeight) {
                    Vector3 posTemp = enemyObjs[i].transform.position;
                    posTemp.y = maxHeight;
                    enemyObjs[i].transform.position = posTemp;
                }
            }
        }
    }

    public void DisadvantageDamage(int damageAmount) {
        if (pCon) {
            pCon.DisadvantageDamage(damageAmount);
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.DisadvantageDamage(damageAmount);
            }
        }
    }

    public bool IsPlayerWildReleasing => pCon && pCon.isSuperman;

    public void SetActionType(ActionType actionType, GameObject hostageObj) {
        if (actionType == ActionType.None && hostageObj && actionTypeHostage && hostageObj != actionTypeHostage) {
            return;
        }
        SetActionTypeText(actionType);
        actionTypeSave = actionType;
        if (actionType == ActionType.None) {
            actionTypeHostage = null;
            actionTypeImage.enabled = false;
        } else {
            actionTypeHostage = hostageObj;
            actionTypeImage.enabled = true;
        }
    }

    public ActionType GetActionType() {
        return actionTypeSave;
    }

    void SetActionTypeText(ActionType actionType) {
        switch (actionType) {
            case ActionType.None:
                actionTypeText.text = "";
                break;
            case ActionType.Search:
                actionTypeText.text = TextManager.Get("ACTION_SEARCH");
                break;
            case ActionType.Talk:
                actionTypeText.text = TextManager.Get("ACTION_TALK");
                break;
            case ActionType.Read:
                actionTypeText.text = TextManager.Get("ACTION_READ");
                break;
            case ActionType.Sit:
                actionTypeText.text = TextManager.Get("ACTION_SIT");
                break;
            case ActionType.StandUp:
                actionTypeText.text = TextManager.Get("ACTION_STANDUP");
                break;
            case ActionType.UseHoldDown:
                actionTypeText.text = TextManager.Get("ACTION_USEHOLDDOWN");
                break;
        }
        if (actionType != ActionType.None) {
            Vector2 sizeTemp = actionTypeText.GetPreferredValues();
            sizeTemp.x = Mathf.Clamp(sizeTemp.x + 40f, 104f, 170f);
            sizeTemp.y = Mathf.Clamp(sizeTemp.y + 30f, 58f, 88f);
            actionTypeImage.rectTransform.sizeDelta = sizeTemp;
        }
    }

    public void ResetActionTypeTextLanguage() {
        SetActionTypeText(actionTypeSave);
    }

    public GameObject GetPlayerPotentialTarget() {
        if (playerTarget) {
            return playerTarget;
        } else if (pCon) {
            return pCon.searchArea[0].GetNowTarget();
        }
        return null;
    }

    public float GetObscureRateAudio() {
        float answer = 1f;
        switch (GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends]) {
            case 0:
            case 1:
            default:
                break;
            case 2:
                if (friendsCountSave > 6) {
                    answer = Mathf.Lerp(1f, 0.6f, (friendsCountSave - 6.0f) / 25.0f);
                }
                break;
            case 3:
                answer = 0.6f;
                break;
        }
        return answer;
    }

    public void ResetFaciemBlinkAll() {
        if (pCon) {
            pCon.ResetFaciemBlink();
        }
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i].fBase) {
                friends[i].fBase.ResetFaciemBlink();
            }
        }
    }

    public int GetFriendsCost(int index) {
        if (extraFriendsIndex > 0 && index == extraFriendsID[0]) {
            index = extraFriendsID[extraFriendsIndex];
        }
        return CharacterDatabase.Instance.friends[index].cost;
    }

    public int GetFriendsID(int index) {
        if (extraFriendsIndex > 0 && index == extraFriendsID[0]) {
            index = extraFriendsID[extraFriendsIndex];
        }
        return index;
    }

}
