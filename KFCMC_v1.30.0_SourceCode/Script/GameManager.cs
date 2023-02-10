using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using System.Text;
using UnityEngine.Audio;
using Rewired;
using TMPro;
using DG.Tweening;

public class GameManager : SingletonMonoBehaviour<GameManager> {

    public enum State { title, play };
    public State state;
    public int currentSaveSlot;
    public SteamManager steamManager;

    public const int version = 1300000;
    public const int saveSlotMax = 20;
    public const int configMax = 130;
    public const int levelMax = 100;
    public const int sandstarMin = 3;
    public const int sandstarCondition = 24;
    public const int sandstarMaxUpIndex = 25;
    public const int sandstarMaxUpCount = 7;
    public const int friendsCostMaxUpIndex = 24;
    public const int friendsCostInfinityIndex = 31;
    public const int inventoryMin = 16;
    public const int inventoryMax = 32;
    public const int friendsMax = 32;
    public const int weaponMax = 32;
    public const int storageMax = 120;
    public const int storagePageMax = 4;
    public const int hpUpNFSMax = 10;
    public const int stUpNFSMax = 10;
    public const int inventoryNFSMax = 16;
    public const int hpUpSaleMax = 30;
    public const int stUpSaleMax = 30;
    public const int stageMax = 20;
    public const int documentMax = 20;
    public const int difficultyMax = 4;
    public const int enemyLevelMax = 5;
    public const int enemyMax = 67;
    public const int luckyBeastMax = 12;
    public const int zakoRushMax = 7;
    public const int zakoRushArrayMax = 56;
    public const int armsLockMax = 3;
    public const int armsIDBase = 200;
    public const int progressMax = 12;
    public const int gameClearedProgress = 13;
    public const int documentFragmentID = 14;
    public const int documentFragmentCondition = 8191;
    public const int hpUpId = 221;
    public const int stUpId = 222;
    public const int invUpId = 223;
    public const int hpUpIndex = 21;
    public const int stUpIndex = 22;
    public const int invUpIndex = 23;
    public const int fdKabanID = 83;
    public const int megatonCoinID = 90;
    public const int minmiIDBase = 90;
    public const int minmiBlueID = 94;
    public const int minmiRedID = 95;
    public const int minmiPurpleID = 96;
    public const int minmiBlackID = 97;
    public const int minmiSilverID = 98;
    public const int minmiGoldenID = 99;
    public const int stageReportMax = 8;
    public const int friendsCombinationMax = 5;
    public const int trophyMax = 120;
    public const int trophyArrayMax = 4;
    public const int trophyPageMax = 4;
    public const int itemExperienceArrayMax = 4;

    [System.NonSerialized]
    public double time;
    [System.NonSerialized]
    public double unscaledTime;
    [System.NonSerialized]
    public float realDeltaTime;
    [System.NonSerialized]
    public Player playerInput;
    [System.NonSerialized]
    public bool megatonCoin;
    [System.NonSerialized]
    public float megatonCoinSpeedMul = 1f;
    [System.NonSerialized]
    public bool minmiBlue;
    [System.NonSerialized]
    public bool minmiRed;
    [System.NonSerialized]
    public bool minmiPurple;
    [System.NonSerialized]
    public bool minmiBlack;
    [System.NonSerialized]
    public bool minmiSilver;
    [System.NonSerialized]
    public bool minmiGolden;
    [System.NonSerialized]
    public bool modFlag;
    [System.NonSerialized]
    public bool musicModFlag;
    [System.NonSerialized]
    public bool stageModFlag;
    [System.NonSerialized]
    public int gameOverCount;
    [System.NonSerialized]
    public bool gameOverFlag;
    [System.NonSerialized]
    public int tutorialMemory;
    [System.NonSerialized]
    public bool tutorialContinuousFlag;
    [System.NonSerialized]
    public int levelLimitAuto = 100;
    [System.NonSerialized]
    public int armsStage;
    [System.NonSerialized]
    public int hpUpLimitAuto = 100;
    [System.NonSerialized]
    public int stUpLimitAuto = 100;
    [System.NonSerialized]
    public int inventoryLimitAuto = 100;
    [System.NonSerialized]
    public bool afterClearFlag;
    [System.NonSerialized]
    public bool isSecondLap;
    [System.NonSerialized]
    public float mouseStoppingTime = 10f;
    [System.NonSerialized]
    public Vector3 mousePositionNow;
    [System.NonSerialized]
    public int[] configDefaultValues;
    [System.NonSerialized]
    public bool dontShowTrophyOnStart;
    [System.NonSerialized]
    public bool equipmentLimitEnabled;

    private bool neutral = false;
    private float lastRealTime;
    private float moveCursorTimeRemain;
    private static readonly int[] denotativeMaxFloor = new int[] { 2, 6, 9, 9, 9, 10, 9, 9, 9, 9, 9, 9, 15, 34, 33 };
    private static readonly int[][] armsLockID = {
    new int[] { 10, 8, 5, 9, 7, 4, 11, 6, 3 },
    new int[] { 19, 18, 17, 16, 15, 14, 13, 12 },
    new int[] { 31, 30, 29, 28, 27, 26, 25, 24}
    };
    private int[] expTable;
    private float[] levelStatusTable;
    private string nowSnapshotName = "Snapshot";
    private int speakerMode = 1;
    private int currentLanguage = -1;
    private float timeTemp = 0f;
    private Vector2Int move;
    private Vector2 axisInput;
    private Vector3 mousePositionPre;
    private bool hasFocus;
    private float focusRestTime;
    private bool buttonDownOnFocus;
    private bool cursorPauseSave;

    private const int modErrorMax = 32;
    public enum ModType { Music, Stage };
    public enum SecretType { SingularityLB, UndergroundLB, Toudai, SafePack, SkytreeCleared };
    private ModType[] modErrorTypes = new ModType[modErrorMax];
    private int[] modErrorLines = new int[modErrorMax];
    private int modErrorIndex;
    private int[] volumeSaveArray = new int[3] { -1, -1, -1 };
    private bool configDefaultInited;
    
    public const int languageJapanese = 0;
    public const int languageEnglish = 1;
    public const int languageChinese = 2;
    public const int languageKorean = 3;
    public const int languageMax = 4;
    public const int japarimanID = 70;
    public const int japariman3SetID = 71;
    public const int japariman5SetID = 72;
    public const int difficultyNT = 1;
    public const int difficultyVU = 2;
    public const int difficultyEN = 3;
    public const int difficultyCR = 4;
    public const int stageReport_PlayTime = 0;
    public const int stageReport_Difficulty = 1;
    public const int stageReport_Minmi = 2;
    public const int stageReport_DamageSum = 3;
    public const int stageReport_DamageCount = 4;
    public const int stageReport_FriendsSum = 5;
    public const int stageReport_FriendsCount = 6;
    public const int stageReport_ItemCount = 7;
    public const int playerType_Normal = 0;
    public const int playerType_Another = 1;
    public const int playerType_Hyper = 2;
    public const int playerType_AnotherEscape = 3;
    public const int clearFlag_Normal = 1;
    public const int clearFlag_Another = 2;
    public const int rescueMax = (friendsMax - 1) + hpUpNFSMax + stUpNFSMax + inventoryNFSMax + luckyBeastMax + 1;

    const string volumeName_Se = "SeVolume";
    const string volumeName_System = "SystemVolume";
    const string volumeName_Music = "MusicVolume";
    const string volumeName_Ambient = "AmbientVolume";
    const string str_Save = "Save";
    const string str_LastSaveSlot = "LastSaveSlot";
    const string str_GameCleared = "GameCleared";
    const string str_Snapshot = "Snapshot";
    const string str_DisplayFormat = "DisplayFormat";
    const int tutorialMax = 29;
    const int gameOverTutorialIndex = 4;
    public static readonly int[,] skillTutorialIndex = new int[6, 2] { { 13, 13 }, { 14, 14 }, { 15, 23 }, { 16, 24 }, { 17, 25 }, { 18, 27 } };
    static readonly Vector2Int vec2IntZero = Vector2Int.zero;

    public const int equipmentID_Dodge = 0;
    public const int equipmentID_Escape = 1;
    public const int equipmentID_Gather = 2;
    public const int equipmentID_Attack = 3;
    public const int equipmentID_Defense = 6;
    public const int equipmentID_Speed = 9;
    public const int equipmentID_Jump = 11;
    public const int equipmentID_Combo = 12;
    public const int equipmentID_Pile = 13;
    public const int equipmentID_Spin = 14;
    public const int equipmentID_Wave = 15;
    public const int equipmentID_Screw = 16;
    public const int equipmentID_Bolt = 17;
    public const int equipmentID_Judgement = 18;
    public const int equipmentID_Antimatter = 19;
    public const int equipmentID_Analyzer = 20;

    [System.Serializable]
    public class FloorModInfo {
        public int id;
        public int music;
        public int ambient;
        public int sky;
        public int bossMusic;
        public FloorModInfo(int id) {
            this.id = id;
            this.music = -100;
            this.ambient = -100;
            this.sky = -100;
            this.bossMusic = -100;
        }
    }

    public List<FloorModInfo> floorModInfo;

    [SerializeField]
    public class Save {
        [SerializeField]
        public int dataExist, difficulty, progress, nowStage, nowFloor, exp, expStorage, money, hpUpSale, stUpSale, playerHP, minmi, tutorial, secret, randomSeed, isExpPreserved, unlockCount, playerType, moveByBus, facilityDisabled;
        [SerializeField]
        public int language, seVolume, musicVolume, ambientVolume, equipedHpUp, equipedStUp, equipedInvUp, version, totalPlayTime, parkmanScore, parkmanScoreHard;
        [SerializeField]
        public float sandstar;
        [SerializeField]
        public List<int> items, storage, itemsPreserve;
        [SerializeField]
        public int[] config, friends, friendsLiving, weapon, equip, inventoryNFS, hpUpNFS, stUpNFS, document, reachedFloor, clearDifficulty, defeatEnemy, luckyBeast, zakoRushClearTime, armsLocked, stageReport, friendsCombination, trophy, itemExperience, date;


        public Save() {
            dataExist = 0;
            difficulty = 2;
            language = 0;
            seVolume = 55;
            musicVolume = 50;
            ambientVolume = 50;
            progress = 0;
            nowStage = 0;
            nowFloor = 0;
            exp = 0;
            expStorage = 0;
            hpUpSale = 0;
            stUpSale = 0;
            money = 0;
            equipedHpUp = 0;
            equipedStUp = 0;
            equipedInvUp = 0;
            sandstar = 0f;
            totalPlayTime = 0;
            parkmanScore = 0;
            parkmanScoreHard = 0;
            minmi = 0;
            tutorial = 0;
            secret = 0;
            randomSeed = 0;
            isExpPreserved = 0;
            unlockCount = 0;
            playerType = 0;
            moveByBus = 0;
            items = new List<int>();
            storage = new List<int>();
            itemsPreserve = new List<int>();
            config = new int[configMax];
            friends = new int[friendsMax];
            friendsLiving = new int[friendsMax];
            weapon = new int[weaponMax];
            equip = new int[weaponMax];
            inventoryNFS = new int[inventoryNFSMax];
            hpUpNFS = new int[hpUpNFSMax];
            stUpNFS = new int[stUpNFSMax];
            document = new int[documentMax];
            reachedFloor = new int[stageMax];
            clearDifficulty = new int[stageMax];
            defeatEnemy = new int[enemyMax * enemyLevelMax];
            luckyBeast = new int[luckyBeastMax];
            zakoRushClearTime = new int[zakoRushArrayMax];
            armsLocked = new int[armsLockMax];
            stageReport = new int[stageReportMax];
            friendsCombination = new int[friendsCombinationMax];
            trophy = new int[trophyArrayMax];
            itemExperience = new int[itemExperienceArrayMax];
        }

        [System.NonSerialized]
        private int expSave = -1, levelSave = -1;
        public int Level {
            get {
                if (exp != expSave) {
                    int level = Instance.GetLevel(exp);
                    expSave = exp;
                    levelSave = level;
                }
                return levelSave;
            }
        }
        public int charWeight {
            get {
                if (language == languageJapanese || language == languageChinese || language == languageKorean) {
                    return 2;
                } else {
                    return 1;
                }
            }
        }
        public int GotFriends {
            get {
                int answer = 0;
                for (int i = 1; i < friendsMax; i++) {
                    if (friends[i] != 0) {
                        answer++;
                    }
                }
                return answer;
            }
        }
        public int GotLuckyBeastCount {
            get {
                int answer = 0;
                for (int i = 0; i < luckyBeast.Length; i++) {
                    if (luckyBeast[i] != 0) {
                        answer++;
                    }
                }
                return answer;
            }
        }
        public int GotInvUpOriginal {
            get {
                int answer = 0;
                for (int i = 0; i < inventoryNFSMax; i++) {
                    if (inventoryNFS[i] != 0) {
                        answer++;
                    }
                }
                return answer;
            }
        }
        public int GotInvUp {
            get {
                int answer = GotInvUpOriginal;
                if (answer > equipedInvUp) {
                    answer = equipedInvUp;
                }
                if (Instance && Instance.equipmentLimitEnabled && answer > Instance.inventoryLimitAuto) {
                    answer = Instance.inventoryLimitAuto;
                }
                return answer;
            }
        }
        public int InventoryMax {
            get {
                if (equip[invUpId - 200] != 0) {
                    return inventoryMin + GotInvUp;
                } else {
                    return inventoryMin;
                }
            }
        }
        public int GotHpUpNFS {
            get {
                int answer = 0;
                for (int i = 0; i < hpUpNFSMax; i++) {
                    if (hpUpNFS[i] != 0) {
                        answer++;
                    }
                }
                return answer;
            }
        }
        public int GotHpUpOriginal {
            get {
                return hpUpSale + GotHpUpNFS;
            }
        }
        public int GotHpUp {
            get {
                int answer = GotHpUpOriginal;
                if (answer > equipedHpUp) {
                    answer = equipedHpUp;
                }
                if (isExpPreserved != 0 && answer > unlockCount + 10) {
                    answer = unlockCount + 10;
                }
                if (Instance && Instance.equipmentLimitEnabled && answer > Instance.hpUpLimitAuto) {
                    answer = Instance.hpUpLimitAuto;
                }
                return answer;
            }
        }
        public int GotStUpNFS {
            get {
                int answer = 0;
                for (int i = 0; i < stUpNFSMax; i++) {
                    if (stUpNFS[i] != 0) {
                        answer++;
                    }
                }
                return answer;
            }
        }
        public int GotStUpOriginal {
            get {
                return stUpSale + GotStUpNFS;
            }
        }
        public int GotStUp {
            get {
                int answer = GotStUpOriginal;
                if (answer > equipedStUp) {
                    answer = equipedStUp;
                }
                if (isExpPreserved != 0 && answer > unlockCount + 10) {
                    answer = unlockCount + 10;
                }
                if (Instance && Instance.equipmentLimitEnabled && answer > Instance.stUpLimitAuto) {
                    answer = Instance.stUpLimitAuto;
                }
                return answer;
            }
        }
        public int SandstarMax {
            get {
                int temp = 0;
                if (GetEquip(sandstarCondition)) {
                    temp = sandstarMin;
                    for (int i = 0; i < sandstarMaxUpCount; i++) {
                        if (GetEquip(sandstarMaxUpIndex + i)) {
                            temp += 1;
                        }
                    }
                }
                return temp;
            }
        }
        public bool GetEquip(int index) {
            if (isExpPreserved != 0 || (Instance && Instance.equipmentLimitEnabled)) {
                if (!IsWeaponLocked(index)) {
                    switch (index) {
                        case 3:
                            return equip[3] != 0 || (equip[4] != 0 && IsWeaponLocked(4)) || (equip[5] != 0 && IsWeaponLocked(4) && IsWeaponLocked(5));
                        case 4:
                            return equip[4] != 0 || (equip[5] != 0 && IsWeaponLocked(5));
                        case 6:
                            return equip[6] != 0 || (equip[7] != 0 && IsWeaponLocked(7)) || (equip[8] != 0 && IsWeaponLocked(7) && IsWeaponLocked(8));
                        case 7:
                            return equip[7] != 0 || (equip[8] != 0 && IsWeaponLocked(8));
                        case 9:
                            return equip[9] != 0 || (equip[10] != 0 && IsWeaponLocked(10));
                        default:
                            return equip[index] != 0;
                    }
                } else {
                    return false;
                }
            } else {
                return equip[index] != 0;
            }
        }
        public bool IsWeaponLocked(int index) {
            if (Instance) {
                switch (index) {
                    case 0:
                    case 1:
                    case 2:
                    case 21:
                    case 22:
                    case 23:
                        return false;
                    case 3:
                        return armsLocked[0] >= 9 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 1);
                    case 6:
                        return armsLocked[0] >= 8 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 1);
                    case 11:
                        return armsLocked[0] >= 7 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 5);
                    case 4:
                        return armsLocked[0] >= 6 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 5);
                    case 7:
                        return armsLocked[0] >= 5 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 5);
                    case 9:
                        return armsLocked[0] >= 4 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 5);
                    case 5:
                        return armsLocked[0] >= 3 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 10);
                    case 8:
                        return armsLocked[0] >= 2 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 10);
                    case 10:
                        return armsLocked[0] >= 1 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 10);
                    case 12:
                        return armsLocked[1] >= 8 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 1);
                    case 13:
                        return armsLocked[1] >= 7 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 1);
                    case 14:
                        return armsLocked[1] >= 6 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 1);
                    case 15:
                        return armsLocked[1] >= 5 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 5);
                    case 16:
                        return armsLocked[1] >= 4 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 5);
                    case 17:
                        return armsLocked[1] >= 3 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 5);
                    case 18:
                        return armsLocked[1] >= 2 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 10);
                    case 19:
                        return armsLocked[1] >= 1 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 12);
                    case 20:
                        return (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 12);
                    case 24:
                        return armsLocked[2] >= 8 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 2);
                    case 25:
                        return armsLocked[2] >= 7 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 3);
                    case 26:
                        return armsLocked[2] >= 6 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 4);
                    case 27:
                        return armsLocked[2] >= 5 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 6);
                    case 28:
                        return armsLocked[2] >= 4 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 7);
                    case 29:
                        return armsLocked[2] >= 3 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 8);
                    case 30:
                        return armsLocked[2] >= 2 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 9);
                    case 31:
                        return armsLocked[2] >= 1 || (Instance.equipmentLimitEnabled && Instance.armsStage > 0 && Instance.armsStage <= 11);
                    default:
                        return false;
                }
            }
            return false;
        }

        public const int configID_PhotoMode = 0;
        public const int configID_FriendsWalkingSpeed = 1;
        public const int configID_FriendsRunningSpeed = 2;
        public const int configID_UseRunButton = 3;
        public const int configID_SearchRadius = 4;
        public const int configID_SearchRadiusBoss = 5;
        public const int configID_ContinuousUseOfItems = 6;
        public const int configID_JaparibunsAutoPacking = 7;
        public const int configID_PullItems = 8;
        public const int configID_LockonReset = 9;
        public const int configID_LockonInFrontOfCamera = 10;
        public const int configID_TreeClimbingAction = 11;
        public const int configID_SittingAction = 12;
        public const int configID_RestingMotion = 13;
        public const int configID_Blinking = 14;

        public const int configID_CameraSensitivity = 20;
        public const int configID_CameraAxisInvert = 21;
        public const int configID_CameraControlButton = 22;
        public const int configID_CameraTurningSpeed = 23;
        public const int configID_SuppressCameraTurning = 24;
        public const int configID_CameraFollowingSpeed = 25;
        public const int configID_FieldOfView = 26;
        public const int configID_CameraTargeting = 27;
        public const int configID_Overlook = 28;
        public const int configID_ClippingDistance = 29;
        public const int configID_ClippingAutoAdjust = 30;
        public const int configID_CameraAxisDefault = 31;
        public const int configID_CameraReturnSpeed = 32;
        public const int configID_CloseUpViewIndoors = 33;

        public const int configID_Gauge = 40;
        public const int configID_ShowEnemyHp = 41;
        public const int configID_LandingPoint = 42;
        public const int configID_Chat = 43;
        public const int configID_ShowArms = 44;
        public const int configID_ShowMessageLog = 45;
        public const int configID_ShowGiraffeBeam = 46;
        public const int configID_ShowCursor = 47;
        public const int configID_TrophyNotification = 48;

        public const int configID_Target = 50;
        public const int configID_DamageSize = 51;
        public const int configID_HpGaugeSize = 52;
        public const int configID_FriendsIconSize = 53;
        public const int configID_MapPos = 54;
        public const int configID_GoldPos = 55;

        public const int configID_ShowGrass = 60;
        public const int configID_DisplaceGrass = 61;
        public const int configID_DynamicBone = 62;
        public const int configID_ClothSimulation = 63;
        public const int configID_FaceToEnemy = 64;
        public const int configID_GenerateDungeonObjects = 65;
        public const int configID_GenerateHomeObjects = 66;

        public const int configID_Antialiasing = 70;
        public const int configID_AmbientOcclusion = 71;
        public const int configID_DepthOfField = 72;
        public const int configID_MotionBlur = 73;
        public const int configID_Bloom = 74;
        public const int configID_ColorGrading = 75;
        public const int configID_Brightness = 76;
        public const int configID_CameraVibration = 77;

        public const int configID_SpeakerMode = 80;
        public const int configID_AcousticEffects = 81;
        public const int configID_HpAlarm = 82;
        public const int configID_StAlarm = 83;

        public const int configID_RunInBackground = 90;
        public const int configID_QualityLevel = 91;
        public const int configID_SystemInformation = 92;
        public const int configID_ScreenShotFileFormat = 93;
        public const int configID_UseMouseForUI = 94;
        public const int configID_CursorLock = 95;
        public const int configID_GamepadVibration = 96;

        public const int configID_BattleAssist = 100;
        public const int configID_GameSpeed = 101;
        public const int configID_ItemAutomaticUse = 102;
        public const int configID_FriendsAutomaticRevive = 103;
        public const int configID_SimplifyDodgeCommand = 104;
        public const int configID_SimplifySkillCommand = 105;
        public const int configID_ObscureFriends = 106;
        public const int configID_MessageTimeRate = 107;

        public const int configID_LevelLimit = 110;
        public const int configID_EquipmentLimit = 111;
        public const int configID_ShowBossTime = 112;
        public const int configID_ShowBossResult = 113;
        public const int configID_ShowPlayTime = 114; //NEW
        public const int configID_FirstPerson = 115; //116
        public const int configID_HellMode = 116; //117
        public const int configID_BossMode = 117;
        public const int configID_DisableTrophy = 118;
        public const int configID_DisableAutoMapping = 119;
        public const int configID_DisablePassiveSkills = 120; //114
        public const int configID_DisableJustDodge = 121; //115
        public const int configID_DisableInvincibility = 122;
        public const int configID_DisableGuts = 123;
        public const int configID_DisableExp = 124;
        public const int configID_DisableItemDrop = 125;


        public int NumOfSpecificItems(int id) {
            return items.Count(n => n == id);
        }
        public int NumOfAllItems() {
            return items.Count;
        }
        public int NumOfSpecificItemsInStorage(int id) {
            return storage.Count(n => n == id);
        }
        public bool HaveSpecificItem(int id) {
            return items.Contains(id);
        }
        public int EndItemID() {
            int itemCount = items.Count;
            if (itemCount > 0) {
                return items[itemCount - 1];
            } else {
                return -1;
            }
        }
        public bool AddItem(int id) {
            if (id < 100) {
                if (itemExperience.Length < itemExperienceArrayMax) {
                    int[] arrayTemp = new int[itemExperienceArrayMax];
                    for (int i = 0; i < itemExperience.Length && i < arrayTemp.Length; i++) {
                        arrayTemp[i] = itemExperience[i];
                    }
                    itemExperience = arrayTemp;
                }
                itemExperience[id / 32] |= (1 << (id % 32));
                if (items.Count < InventoryMax) {
                    items.Add(id);
                    items.Sort();
                    return true;
                } else {
                    return false;
                }
            } else if (id >= 200 && id < 200 + weaponMax) {
                int weaponTemp = weapon[id - 200];
                weapon[id - 200] = 1;
                if (id == hpUpId && hpUpSale < hpUpSaleMax) {
                    hpUpSale += 1;
                    equipedHpUp = GotHpUpOriginal;
                } else if (id == stUpId && stUpSale < stUpSaleMax) {
                    stUpSale += 1;
                    equipedStUp = GotStUpOriginal;
                }
                if (weaponTemp != weapon[id - 200] && CharacterManager.Instance) {
                    CharacterManager.Instance.SetWeapon(id - 200);
                }
            }
            return true;
        }
        public void SetClearStage(int num, bool setProgress = true) {
            if (clearDifficulty.Length < stageMax) {
                int[] newArray = new int[stageMax];
                for (int i = 0; i < clearDifficulty.Length; i++) {
                    newArray[i] = clearDifficulty[i];
                }
                clearDifficulty = newArray;
            }
            if (num >= 0 && num < clearDifficulty.Length && clearDifficulty[num] < difficulty && moveByBus == 0) {
                clearDifficulty[num] = difficulty;
            }
            if (setProgress && progress < num) {
                progress = num;
            }
        }
        public bool IsPossibleAddItem() {
            return items.Count < InventoryMax;
        }
        public bool RemoveItem(int id) {
            return items.Remove(id);
        }
        public bool AddStorage(int id) {
            if (id < 100) {
                if (storage.Count < storageMax) {
                    storage.Add(id);
                    storage.Sort();
                    return true;
                } else {
                    return false;
                }
            }
            return false;
        }
        public bool RemoveStorage(int id) {
            return storage.Remove(id);
        }
        public int NeedExp(int level) {
            if (level <= 1) {
                return 0;
            } else {
                return Instance.expTable[level - 2];
            }
        }
        public int NeedExpToNextLevel {
            get {
                int level = Level;
                return level >= levelMax ? 0 : NeedExp(level + 1) - NeedExp(level);
            }
        }
        public int NowLevelExp {
            get {
                return exp - NeedExp(Level);
            }
        }
        public float ExpRate {
            get {
                int level = Level;
                int minExp = NeedExp(level);
                int maxExp = NeedExp(level + 1);
                return level >= levelMax ? 0 : (exp - minExp) / (float)(maxExp - minExp);
            }
        }
        public void SetRunInBackground() {
            bool answer = (config[configID_RunInBackground] != 0);
            if (Application.runInBackground != answer) {
                Application.runInBackground = answer;
            }
        }
        public void SetShowCursor(bool isPausing, bool isTitle) {
            bool visibleFlag = true;
            switch (config[configID_ShowCursor]) {
                case 0:
                    visibleFlag = false;
                    break;
                case 2:
                    visibleFlag = isTitle || isPausing;
                    break;
                case 3:
                    visibleFlag = isTitle || !isPausing;
                    break;
            }
            if (Cursor.visible != visibleFlag) {
                Cursor.visible = visibleFlag;
            }
            CursorLockMode lockMode = CursorLockMode.None;
            switch (config[configID_CursorLock]) {
                case 1:
                    lockMode = isTitle || isPausing ? CursorLockMode.None : CursorLockMode.Locked;
                    break;
                case 2:
                    lockMode = isTitle || isPausing ? CursorLockMode.None : CursorLockMode.Confined;
                    break;
            }
            if (Cursor.lockState != lockMode) {
                Cursor.lockState = lockMode;
            }
        }
        public void ResetVolumeConfig() {
            seVolume = 55;
            musicVolume = 50;
            ambientVolume = 50;
        }
        public int[] GetInitializedConfigArray() {
            int[] configTemp = new int[configMax];
            configTemp[configID_QualityLevel] = 4;
            configTemp[configID_GamepadVibration] = 2;
            configTemp[configID_SearchRadius] = 5;
            configTemp[configID_SearchRadiusBoss] = 15;
            configTemp[configID_JaparibunsAutoPacking] = 1;
            configTemp[configID_UseRunButton] = 2;
            configTemp[configID_PullItems] = 1;
            configTemp[configID_LockonReset] = 1;
            configTemp[configID_TreeClimbingAction] = 1;
            configTemp[configID_SittingAction] = 2;
            configTemp[configID_Blinking] = 1;

            configTemp[configID_CameraSensitivity] = 5;
            configTemp[configID_CameraControlButton] = 1;
            configTemp[configID_CameraTurningSpeed] = 8;
            configTemp[configID_SuppressCameraTurning] = 5;
            configTemp[configID_CameraFollowingSpeed] = 10;
            configTemp[configID_FieldOfView] = 5;
            configTemp[configID_CameraTargeting] = 2;
            configTemp[configID_CameraAxisDefault] = 3;
            configTemp[configID_CameraVibration] = 2;
            configTemp[configID_Overlook] = 1;
            configTemp[configID_CloseUpViewIndoors] = 1;
            configTemp[configID_Gauge] = 1;
            configTemp[configID_ShowEnemyHp] = 1;
            configTemp[configID_LandingPoint] = 1;
            configTemp[configID_Chat] = 1;
            configTemp[configID_ShowArms] = 1;
            configTemp[configID_ShowGiraffeBeam] = 2;
            configTemp[configID_ShowCursor] = 1;
            configTemp[configID_TrophyNotification] = 1;
            configTemp[configID_Target] = 6;
            configTemp[configID_DamageSize] = 6;
            configTemp[configID_HpGaugeSize] = 6;
            configTemp[configID_FriendsIconSize] = 6;
            configTemp[configID_GoldPos] = 5;
            configTemp[configID_ClippingDistance] = 2;
            configTemp[configID_SpeakerMode] = 1;
            configTemp[configID_AcousticEffects] = 1;
            configTemp[configID_HpAlarm] = 3;
            configTemp[configID_StAlarm] = 3;
            configTemp[configID_PhotoMode] = 1;
            configTemp[configID_UseMouseForUI] = 1;
            configTemp[configID_ObscureFriends] = 1;
            //Quality
            configTemp[configID_ShowGrass] = 1;
            configTemp[configID_DisplaceGrass] = 1;
            configTemp[configID_DynamicBone] = 2;
            configTemp[configID_ClothSimulation] = 2;
            configTemp[configID_FaceToEnemy] = 2;
            configTemp[configID_GenerateDungeonObjects] = 1;
            configTemp[configID_GenerateHomeObjects] = 1;
            configTemp[configID_ClippingAutoAdjust] = 1;
            configTemp[configID_Antialiasing] = 2;
            configTemp[configID_AmbientOcclusion] = 2;
            configTemp[configID_DepthOfField] = 2;
            configTemp[configID_MotionBlur] = 2;
            configTemp[configID_Bloom] = 3;
            configTemp[configID_ColorGrading] = 1;
            return configTemp;
        }
        public void ResetQualityConfig(int quality) {
            config[configID_ShowGrass] = (quality >= 2 ? 1 : quality == 1 ? 2: 0);
            config[configID_DisplaceGrass] = (quality >= 2 ? 1 : 0);
            config[configID_DynamicBone] = (quality >= 2 ? 2 : quality == 1 ? 1 : 0);
            config[configID_ClothSimulation] = (quality >= 1 ? 2 : 0);
            config[configID_FaceToEnemy] = (quality >= 1 ? 2 : 0);
            config[configID_GenerateDungeonObjects] = (quality >= 1 ? 1 : 0);
            config[configID_GenerateHomeObjects] = 1;
            config[configID_ClippingAutoAdjust] = (quality >= 1 ? 1 : 3);
            config[configID_Antialiasing] = (quality >= 2 ? 2 : quality == 1 ? 1 : 0);
            config[configID_AmbientOcclusion] = (quality >= 2 ? 2 : 0);
            config[configID_DepthOfField] = (quality >= 2 ? 2 : 0);
            config[configID_MotionBlur] = (quality >= 2 ? 2 : quality == 1 ? 1 : 0);
            config[configID_Bloom] = (quality >= 1 ? 3 : 0);
            config[configID_ColorGrading] = 1;
        }
        public void ResetConfig() {
            config = GetInitializedConfigArray();
            int systemQuality = QualitySettings.GetQualityLevel();
            ResetQualityConfig(systemQuality);
            config[configID_QualityLevel] = systemQuality;
            SetShowCursor(PauseController.Instance ? PauseController.Instance.GetNowPausing() : false, GameManager.Instance ? GameManager.Instance.state == GameManager.State.title : false);
            SetRunInBackground();
        }
        public void ResetAntiScreensickConfig() {
            if (config[configID_LockonInFrontOfCamera] < 7) {
                config[configID_LockonInFrontOfCamera] = 7;
            }
            if (config[configID_CameraSensitivity] > 3) {
                config[configID_CameraSensitivity] = 3;
            }
            config[configID_CameraTurningSpeed] = 8;
            config[configID_CameraFollowingSpeed] = 10;
            config[configID_Overlook] = 0;
            if (config[configID_SuppressCameraTurning] < 7) {
                config[configID_SuppressCameraTurning] = 7;
            }
            if (config[configID_FieldOfView] < 20) {
                config[configID_FieldOfView] = 20;
            }
            if (config[configID_CameraAxisDefault] < 5) {
                config[configID_CameraAxisDefault] = 5;
            }
            config[configID_CloseUpViewIndoors] = 0;
            config[configID_DepthOfField] = 0;
            config[configID_MotionBlur] = 0;
            if (config[configID_Bloom] > 1) {
                config[configID_Bloom] = 1;
            }
            config[configID_CameraVibration] = 0;
        }
        public void PreserveExp() {
            if (isExpPreserved == 0) {
                expStorage = exp;
                exp = 0;
                if (armsLocked.Length < armsLockMax) {
                    armsLocked = new int[armsLockMax];
                }
                unlockCount = 0;
                armsLocked[0] = 9;
                armsLocked[1] = 8;
                armsLocked[2] = 8;
                int itemsLength = items.Count;                
                if (itemsPreserve == null) {
                    itemsPreserve = new List<int>();
                }
                itemsPreserve.Clear();
                for (int i = itemsLength - 1; i >= 0; i--) {
                    if (items[i] < minmiIDBase) {
                        itemsPreserve.Add(items[i]);
                        items.RemoveAt(i);
                    }
                }
                isExpPreserved = 1;
                // config[configID_LevelLimit] = 0;
                if (CharacterManager.Instance && CharacterManager.Instance.pCon) {
                    CharacterManager.Instance.pCon.LevelUp(Level);
                    CharacterManager.Instance.Heal(0, 100, -1, true, false);
                    CharacterManager.Instance.UpdateExpGage();
                }
                if (PauseController.Instance) {
                    PauseController.Instance.UpdateLevelLimit();
                }
            }
        }
        public void RestoreExp() {
            if (isExpPreserved != 0) {
                exp = expStorage;
                expStorage = 0;
                unlockCount = 0;
                for (int i = 0; i < armsLocked.Length; i++) {
                    armsLocked[i] = 0;
                }
                int itemsLength = items.Count;
                for (int i = itemsLength - 1; i >= 0; i--) {
                    if (items[i] < minmiIDBase) {
                        items.RemoveAt(i);
                    }
                }
                itemsLength = items.Count;
                if (itemsPreserve != null) {
                    int preserveLength = itemsPreserve.Count;
                    for (int i = 0; i < preserveLength && itemsLength + i < inventoryMax; i++) {
                        items.Add(itemsPreserve[i]);
                    }
                    items.Sort();
                }

                isExpPreserved = 0;
                // config[configID_LevelLimit] = 0;
                if (CharacterManager.Instance && CharacterManager.Instance.pCon) {
                    CharacterManager.Instance.pCon.LevelUp(Level);
                    CharacterManager.Instance.Heal(0, 100, -1, true, false);
                    CharacterManager.Instance.UpdateExpGage();
                }
                if (PauseController.Instance) {
                    PauseController.Instance.UpdateLevelLimit();
                }
            }
        }
        public int GetRescueNow() {
            return GotFriends + GotHpUpNFS + GotStUpNFS + GotInvUpOriginal + GotLuckyBeastCount + ((secret & (1 << (int)SecretType.SingularityLB)) != 0 ? 1 : 0);
        }
    }

    public Save save;    
    public AudioMixer audioMixer;

    public void InitSave(int playerType = 0) {
        save = new Save();
        save.playerType = playerType;
        if (volumeSaveArray.Length > 2 && volumeSaveArray[0] >= 0) {
            save.seVolume = volumeSaveArray[0];
            save.musicVolume = volumeSaveArray[1];
            save.ambientVolume = volumeSaveArray[2];
        }
        if (currentLanguage >= 0) {
            save.language = currentLanguage;
        } else {
            save.language = -1;
            InitLanguage();
        }
        save.ResetConfig();
        ApplySpeakerMode();
        ApplyVolume();
        FixProgressWeapon();
        if (IsPlayerAnother) {
            InitializeForPlayerAnother();
        }
        timeTemp = 0;
    }

    public void MeasurePlayTime() {
        if (Time.timeScale > 0) {
            timeTemp += Time.unscaledDeltaTime * Mathf.Min(Time.timeScale, 1f);
            if (timeTemp >= 1f) {
                timeTemp -= 1f;
                save.totalPlayTime += 1;
            }
        }
    }

    void FixVersion() {
        if (save.version < version) {
            if (save.version <= 11) {
                int[] weaponTemp = new int[weaponMax];
                int[] equipTemp = new int[weaponMax];
                for (int i = 0; i < 2; i++) {
                    weaponTemp[i] = 1;
                    equipTemp[i] = 1;
                }
                for (int i = 0; i < 27; i++) {
                    if (i <= 8) {
                        weaponTemp[i + 2] = save.weapon[i];
                        equipTemp[i + 2] = save.equip[i];
                    } else if (i == 9) {
                        if (save.weapon[i] != 0) {
                            save.money += 60;
                        }
                    } else if (i <= 14) {
                        weaponTemp[i + 1] = save.weapon[i];
                        equipTemp[i + 1] = save.equip[i];
                    } else if (i <= 23) {
                        weaponTemp[i + 8] = save.weapon[i];
                        equipTemp[i + 8] = save.equip[i];
                    } else if (i <= 26) {
                        weaponTemp[i - 3] = save.weapon[i];
                        equipTemp[i - 3] = save.equip[i];
                    }
                }
                save.weapon = weaponTemp;
                save.equip = equipTemp;

                int[] inventoryNFSTemp = new int[inventoryNFSMax];
                for (int i = 0; i < inventoryNFSMax; i++) {
                    if (i >= 10) {
                        inventoryNFSTemp[i] = save.inventoryNFS[i];
                    } else {
                        inventoryNFSTemp[i] = 0;
                    }
                }
                save.inventoryNFS = inventoryNFSTemp;

                int[] configTemp = new int[40];
                for (int i = 0; i < 40 - 10; i++) {
                    if (i <= 2) {
                        configTemp[i + 1] = save.config[i];
                    } else if (i <= 7) {
                        configTemp[i + 7] = save.config[i];
                    } else if (i >= 10 && i <= 25) {
                        configTemp[i + 10] = save.config[i];
                    }
                }

                save.config = configTemp;

                save.seVolume = 70;
                save.musicVolume = 60;
                save.config[0] = 0;
                save.config[4] = 1;
                save.config[5] = 1;
                save.config[15] = 1;
                save.difficulty = 2;
            }
            if (save.version <= 12) {
                int[] defeatTemp = new int[320];
                for (int i = 0; i < 320; i++) {
                    if (i % 5 == 0) {
                        defeatTemp[i] = 0;
                    } else {
                        defeatTemp[i] = save.defeatEnemy[i - i / 5 - 1];
                    }
                }
                save.defeatEnemy = defeatTemp;
                save.config[6] = 5;
            }
            if (save.version <= 14) {
                save.config[7] = 0;
            }

            if (save.version <= 16) {
                int[] configTemp = new int[50];
                configTemp[0] = save.config[0];
                configTemp[1] = save.config[1];
                configTemp[2] = save.config[5];
                configTemp[10] = save.config[6];
                configTemp[11] = save.config[7];
                configTemp[12] = 0;
                configTemp[13] = 2;
                configTemp[14] = save.config[4];
                configTemp[15] = save.config[2];
                configTemp[16] = save.config[3];

                for (int i = 20; i < 50; i++) {
                    configTemp[i] = save.config[i - 10];
                }
                save.config = configTemp;
            }

            if (save.version <= 17) {
                int[] inventoryNFSTemp = new int[inventoryNFSMax];
                for (int i = 0; i < inventoryNFSMax; i++) {
                    if (i <= 3) {
                        inventoryNFSTemp[i] = save.inventoryNFS[i + 10];
                    } else {
                        inventoryNFSTemp[i] = 0;
                    }
                }
                save.inventoryNFS = inventoryNFSTemp;
            }

            if (save.version <= 18) {
                int[] weaponTemp = new int[weaponMax];
                int[] equipTemp = new int[weaponMax];
                for (int i = 0; i < weaponMax; i++) {
                    if (i <= 1 || i >= 21) {
                        weaponTemp[i] = save.weapon[i];
                        equipTemp[i] = save.equip[i];
                    } else if (i == 2) {
                        weaponTemp[i] = 1;
                        equipTemp[i] = 1;
                    } else {
                        weaponTemp[i] = save.weapon[i - 1];
                        equipTemp[i] = save.equip[i - 1];
                    }
                }
                save.weapon = weaponTemp;
                save.equip = equipTemp;
            }

            if (save.version <= 22) {
                int[] configTemp = new int[50];
                for (int i = 0; i < 50; i++) {
                    if (i == 13) {
                        configTemp[i] = 5;
                    } else if (i == 14 || i == 15) {
                        configTemp[i] = save.config[i - 1];
                    } else {
                        configTemp[i] = save.config[i];
                    }
                }
                save.config = configTemp;
            }

            if (save.version <= 24) {
                int[] luckyBeastTemp = new int[luckyBeastMax];
                for (int i = 0; i < luckyBeastMax; i++) {
                    if (i < save.progress && i < 5) {
                        luckyBeastTemp[i] = 1;
                    } else {
                        luckyBeastTemp[i] = 0;
                    }
                }
                save.luckyBeast = luckyBeastTemp;
            }

            if (save.version <= 40) {
                save.config[4] = 5;
            }

            if (save.version <= 42) {
                save.ambientVolume = 60;
                int[] configTemp = new int[60];
                for (int i = 0; i < 60; i++) {
                    if (i == 50 || i == 51) {
                        configTemp[i] = 1;
                    } else if (i >= 50) {
                        configTemp[i] = 0;
                    } else {
                        configTemp[i] = save.config[i];
                    }
                }
                save.config = configTemp;
            }

            if (save.version <= 44) {
                save.config[5] = 0;
            }

            if (save.version <= 50) {
                save.config[26] = 0;
                save.totalPlayTime = 0;
                save.storage = new List<int>();
            }

            if (save.version <= 63) {
                save.equipedHpUp = save.GotHpUpOriginal;
                save.equipedStUp = save.GotStUpOriginal;
                save.equipedInvUp = save.GotInvUpOriginal;
            }

            if (save.version <= 65) {
                /*
                save.friendsHP = new int[friendsMax];
                for (int i = 0; i < friendsMax; i++) {
                    save.friendsHP[i] = 0;
                }
                */
                save.playerHP = 0;
            }

            if (save.version <= 66) {
                save.config[27] = 1;
            }

            if (save.version <= 72) {
                save.config[55] = save.config[37];
                save.config[37] = 0;
                if (save.config[32] >= 1) {
                    save.config[32] = 2;
                }
                if (save.config[33] >= 1) {
                    save.config[33] = 2;
                }
                if (save.config[34] >= 1) {
                    save.config[34] = 2;
                }
            }

            if (save.version <= 74) {
                save.config[6] = 0;
                save.config[17] = 0;
            }

            if (save.version <= 80) {
                save.config[7] = 1;
                save.config[18] = 10;
            }

            if (save.version <= 83) {
                int[] configTemp = new int[65];
                for (int i = 0; i < 60; i++) {
                    if (i < save.config.Length) {
                        configTemp[i] = save.config[i];
                    }
                }
                configTemp[6] = save.config[7];
                configTemp[60] = save.config[6];
                configTemp[61] = 0;
                configTemp[62] = 0;
                save.config = configTemp;
            }

            // If you change EnemyMax, change this version.
            if (save.version <= 84) {
                if (save.defeatEnemy.Length < enemyMax * enemyLevelMax) {
                    int[] defeatTemp = new int[enemyMax * enemyLevelMax];
                    for (int i = 0; i < save.defeatEnemy.Length; i++) {
                        defeatTemp[i] = save.defeatEnemy[i];
                    }
                    for (int i = save.defeatEnemy.Length; i < defeatTemp.Length; i++) {
                        defeatTemp[i] = 0;
                    }
                    save.defeatEnemy = defeatTemp;
                }
            }

            if (save.version <= 90) {
                if (save.nowStage == 12) {
                    if (save.nowFloor == 10) {
                        save.nowFloor = 11;
                    } else if (save.nowFloor == 11) {
                        save.nowFloor = 12;
                    } else if (save.nowFloor == 12) {
                        save.nowFloor = 14;
                    } else if (save.nowFloor == 13) {
                        save.nowFloor = 15;
                    }
                }
                save.minmi = save.expStorage;
                save.secret = 0;
            }

            if (save.version <= 91) {
                if (save.tutorial < 22 && save.progress >= 11) {
                    save.tutorial = 22;
                } else if (save.tutorial >= 13) {
                    save.tutorial += 2;
                } else if (save.tutorial == 12) {
                    save.tutorial += 1;
                }
            }

            if (save.version <= 92) {
                save.zakoRushClearTime = new int[zakoRushMax * difficultyMax];
                if (save.tutorial < 11) {
                    save.tutorial += 1;
                }
            }

            if (save.version <= 93) {
                save.parkmanScore = 0;
                save.parkmanScoreHard = 0;
            }

            if (save.version <= 94) {
                int[] configTemp = new int[65];
                for (int i = 0; i < 65; i++) {
                    if (i < 25 || i > 28) {
                        configTemp[i] = save.config[i];
                    }
                }
                configTemp[25] = save.config[26];
                configTemp[26] = save.config[27];
                configTemp[27] = save.config[25];
                configTemp[28] = 2;
                configTemp[46] = 0;
                configTemp[63] = 0;
                save.config = configTemp;
            }

            if (save.version <= 96) {
                int length = save.items.Count;
                if (length > 0) {
                    for (int i = 0; i < length; i++) {
                        if (save.items[i] >= 90 && save.items[i] <= 93) {
                            save.items[i] -= 4;
                        }
                    }
                }
                length = save.storage.Count;
                if (length > 0) {
                    for (int i = 0; i < length; i++) {
                        if (save.storage[i] >= 90 && save.storage[i] <= 93) {
                            save.storage[i] -= 4;
                        }
                    }
                }
            }

            if (save.version <= 97) {
                save.randomSeed = 0;
                save.isExpPreserved = 0;
                save.expStorage = 0;
                save.armsLocked = new int[armsLockMax];
                save.itemsPreserve = new List<int>();
                if (save.config[2] == 1) {
                    save.config[2] = 2;
                }
                if (save.config[15] == 1) {
                    save.config[15] = 2;
                }
            }

            if (save.version <= 99) {
                if (save.progress > 12) {
                    save.progress = 12;
                }
            }

            if (save.version < 9913) {
                if (save.nowStage == 14) {
                    save.unlockCount = save.nowFloor;
                }
                int[] configTemp = new int[70];
                for (int i = 0; i < save.config.Length && i < 70; i++) {
                    switch (i) {
                        case 0:
                        case 1:
                            break;
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                            configTemp[i - 2] = save.config[i];
                            break;
                        case 14:
                        case 15:
                        case 16:
                            configTemp[i + 2] = save.config[i];
                            break;
                        case 17:
                            configTemp[6] = save.config[i];
                            break;
                        case 18:
                            configTemp[14] = save.config[i];
                            break;
                        default:
                            configTemp[i] = save.config[i];
                            break;
                    }
                    configTemp[15] = 0;
                }
                save.config = configTemp;
            }

            if (save.version < 9916) {
                int[] configTemp = new int[70];
                for (int i = 0; i < save.config.Length && i < 70; i++) {
                    switch (i) {
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                            configTemp[i + 1] = save.config[i];
                            break;
                        default:
                            configTemp[i] = save.config[i];
                            break;
                    }
                }
                configTemp[3] = 15;
                save.config = configTemp;
                save.stageReport = new int[stageReportMax];
            }
            if (save.version < 9918) {
                save.playerType = 0;
            }
            if (save.version < 9920) {
                /*
                if (save.showGiraffeBeam >= 1) {
                    save.showGiraffeBeam = 2;
                }
                */
            }
            if (save.version < 10100) {
                if (save.tutorial >= 10) {
                    save.tutorial += 1;
                }
            }
            if (save.version < 10200) {
                if (save.tutorial >= 3) {
                    save.tutorial += 1;
                }
            }

            if (save.version < 10300) {
                if (save.config.Length > 37) {
                    save.config[37] = save.config[27]; // hide walls
                    save.config[27] = 1;
                }
            }
            if (save.version < 10500) {
                if (save.config[20] != 0) {
                    save.config[20] = 2;
                }
            }
            if (save.version < 10600) {
                int[] configTemp = new int[80];
                for (int i = 0; i < save.config.Length && i < 70; i++) {
                    if (i >= 30) {
                        configTemp[i + 10] = save.config[i];
                    } else if (i == 7) {
                        configTemp[34] = save.config[i];
                    } else if (i == 20) {
                        configTemp[30] = save.config[i];
                    } else if (i == 21) {
                        configTemp[22] = save.config[i];
                    } else if (i == 22) {
                        configTemp[20] = save.config[i];
                    } else {
                        configTemp[i] = save.config[i];
                    }
                }
                configTemp[21] = 1;
                configTemp[31] = 2;
                configTemp[32] = 2;
                configTemp[33] = 2;
                configTemp[35] = 5;
                configTemp[62] = 1;
                configTemp[63] = 1;
                save.config = configTemp;
            }
            if (save.version < 10700) {
                if (save.config[62] != 0) {
                    save.config[62] = 3;
                }
                if (save.config[63] != 0) {
                    save.config[63] = 3;
                }
                save.friendsCombination = new int[friendsCombinationMax];
                // save.friendsCombination[0] = save.recordedFriends;
            }
            if (save.version < 10800) {
                save.facilityDisabled = 0;
            }
            if (save.version < 10901) {
                save.config[69] = 1;
            }
            if (save.version < 11100) {
                save.trophy = new int[trophyArrayMax];
                int[] configTemp = new int[90];
                for (int i = 0; i < save.config.Length && i < 80; i++) {
                    if (i >= 70) {
                        configTemp[i + 10] = save.config[i];
                    } else if (i >= 65) {
                        configTemp[i + 5] = save.config[i];
                    } else if (i == 28) {
                        configTemp[57] = save.config[i];
                    } else {
                        configTemp[i] = save.config[i];
                    }
                }
                configTemp[28] = 1;
                save.config = configTemp;
                dontShowTrophyOnStart = true;
            }
            if (save.version < 1110003) {
                if (save.zakoRushClearTime.Length < 40) {
                    int[] timeTemp = new int[40];
                    for (int i = 0; i < save.zakoRushClearTime.Length; i++) {
                        timeTemp[i] = save.zakoRushClearTime[i];
                    }
                    save.zakoRushClearTime = timeTemp;
                }

                if (save.version == 11100 && AnyTrophyExist()) {
                    int[] trophyTemp = new int[4];
                    for (int index = 0; index < 58; index++) {
                        int arrayNum = index / 32;
                        int bitNum = 1 << (index % 32);
                        if ((save.trophy[arrayNum] & bitNum) != 0) {
                            int newIndex = -1;
                            if (index >= 0 && index <= 18) {
                                newIndex = index;
                            } else if (index == 19) {
                                newIndex = 21;
                            } else if (index >= 20 && index <= 42) {
                                newIndex = index + 3;
                            } else if (index == 44) {
                                newIndex = 46;
                            } else if (index == 47) {
                                newIndex = 52;
                            } else if (index == 46) {
                                newIndex = 61;
                            } else if (index == 45) {
                                newIndex = 62;
                            } else if (index == 55) {
                                newIndex = 64;
                            } else if (index == 56) {
                                newIndex = 65;
                            } else if (index == 57) {
                                newIndex = 66;
                            } else if (index >= 48 && index <= 52) {
                                newIndex = index + 64;
                            }
                            if (newIndex >= 0) {
                                int newArrayNum = newIndex / 32;
                                int newBitNum = 1 << (newIndex % 32);
                                trophyTemp[newArrayNum] |= newBitNum;
                            }
                        }
                    }
                    save.trophy = trophyTemp;
                }
            }
            if (save.version < 1130001) {
                save.itemExperience = new int[itemExperienceArrayMax];
                int itemsCount = save.items.Count;
                if (itemsCount > 0) {
                    for (int i = 0; i < itemsCount; i++) {
                        int idTemp = save.items[i];
                        if (idTemp >= 0 && idTemp < 100) {
                            save.itemExperience[idTemp / 32] |= (1 << (idTemp % 32));
                        }
                    }
                }
                int storageCount = save.storage.Count;
                if (storageCount > 0) {
                    for (int i = 0; i < storageCount; i++) {
                        int idTemp = save.storage[i];
                        if (idTemp >= 0 && idTemp < 100) {
                            save.itemExperience[idTemp / 32] |= (1 << (idTemp % 32));
                        }
                    }
                }
                int preserveCount = save.itemsPreserve.Count;
                if (preserveCount > 0) {
                    for (int i = 0; i < preserveCount; i++) {
                        int idTemp = save.itemsPreserve[i];
                        if (idTemp >= 0 && idTemp < 100) {
                            save.itemExperience[idTemp / 32] |= (1 << (idTemp % 32));
                        }
                    }
                }
                if (save.minmi != 0) {
                    for (int i = 0; i < 6; i++) {
                        if ((save.minmi & (1 << i)) != 0) {
                            int idTemp = 94 + i;
                            save.itemExperience[idTemp / 32] |= (1 << (idTemp % 32));
                        }
                    }
                }
            }
            if (save.version < 1170100) {
                if (save.zakoRushClearTime.Length == 40) {
                    int[] zakoTemp = new int[56];
                    for (int i = 0; i < 20; i++) {
                        zakoTemp[i] = save.zakoRushClearTime[i];
                    }
                    for (int i = 20; i < 40; i++) {
                        zakoTemp[i + 8] = save.zakoRushClearTime[i];
                    }
                    save.zakoRushClearTime = zakoTemp;
                }
            }
            if (save.version < 1180103) {
                if (save.config.Length > 33) {
                    for (int i = 0; i < 4; i++) {
                        switch (save.config[30 + i]) {
                            case 1:
                                save.config[30 + i] = 4;
                                break;
                            case 2:
                                save.config[30 + i] = 6;
                                break;
                            case 3:
                                save.config[30 + i] = 8;
                                break;
                        }
                    }
                }
            }
            if (save.version < 1190000) {
                if (save.config.Length < 100) {
                    int[] configTemp = new int[100];
                    for (int i = 0; i < save.config.Length; i++) {
                        configTemp[i] = save.config[i];
                    }
                    save.config = configTemp;
                }
                if (save.version >= 1180103 && save.config.Length >= 100) {
                    int tmp = save.config[93];
                    save.config[93] = save.config[94];
                    save.config[94] = save.config[95];
                    save.config[95] = tmp;
                    if (save.config[93] < 0) {
                        save.config[93] = 0;
                    }
                    if (save.config[94] < 0) {
                        save.config[94] = 0;
                    }
                    if (save.config[95] > 0) {
                        save.config[95] = 0;
                    }
                }
            }
            if (save.version < 1200000) {
                if (save.date == null || save.date.Length < 6) {
                    save.date = new int[6];
                }
            }
            if (save.version < 1200101) {
                if (save.config.Length < 100) {
                    int[] configTemp = new int[100];
                    for (int i = 0; i < save.config.Length; i++) {
                        configTemp[i] = save.config[i];
                    }
                    save.config = configTemp;
                }
                save.config[76] = save.config[0];
                save.config[7] = save.config[6];
                save.config[6] = save.config[5];
                save.config[5] = save.config[4];
                save.config[4] = save.config[3];
                save.config[3] = save.config[2];
                save.config[2] = save.config[1];
                save.config[0] = 0;
                save.config[1] = 0;
                save.config[8] = 1;
            }
            if (save.version < 1200102) {
                if (save.config[13] == 5) {
                    save.config[13] = 8;
                }
                if (save.config[15] == 0) {
                    save.config[15] = 5;
                }
            }
            if (save.version < 1210001) {
                save.config[58] = save.config[57];
                save.config[57] = save.config[17];
                save.config[17] = save.config[16];
                save.config[16] = save.config[15];
                save.config[15] = save.config[14];
                save.config[14] = 0;
                if (save.config[54] == 1) {
                    save.config[54] = 3;
                }
            }
            if (save.version < 1210002) {
                save.config[14] = 0;
            }
            if (save.version < 1230000) {
                if (save.config.Length < 100) {
                    int[] configTemp = new int[100];
                    for (int i = 0; i < save.config.Length; i++) {
                        configTemp[i] = save.config[i];
                    }
                    save.config = configTemp;
                }
                if (save.config[92] != 0) {
                    save.config[92] = 3;
                } else {
                    save.config[92] = 1;
                }
                save.config[96] = save.config[95];
                save.config[95] = 0;
            }
            if (save.version < 1240000) {
                if (save.config.Length < 110) {
                    int[] configTemp = new int[110];
                    for (int i = 0; i < save.config.Length; i++) {
                        configTemp[i] = save.config[i];
                    }
                    save.config = configTemp;
                }
                for (int i = 109; i >= 30; i--) {
                    save.config[i] = save.config[i - 10];
                }
                save.config[19] = save.config[68];
                save.config[20] = save.config[57];
                save.config[21] = 0;
                save.config[22] = 0;
            }
            if (save.version < 1250000) {
                if (save.tutorial >= 21) {
                    save.tutorial += 7;
                } else if (save.tutorial >= 20) {
                    save.tutorial += 6;
                } else if (save.tutorial >= 12) {
                    save.tutorial += 3;
                } else if (save.tutorial >= 1) {
                    save.tutorial += 2;
                }
                if (save.tutorial > 29) {
                    save.tutorial = 29;
                }

                if (save.config.Length < 120) {
                    int[] configTemp = new int[120];
                    for (int i = 0; i < save.config.Length; i++) {
                        configTemp[i] = save.config[i];
                    }
                    save.config = configTemp;
                }
                for (int i = 119; i >= 20; i--) {
                    save.config[i] = save.config[i - 10];
                }
                for (int i = 10; i < 20; i++) {
                    save.config[i] = 0;
                }

                save.config[9] = 0;
                save.config[10] = 1;
                save.config[11] = 2;
                save.config[33] = 1;
                save.config[12] = save.config[105];
                save.config[105] = 0;
                save.config[13] = 1;
                save.config[107] = 0;
            }
            if (save.version < 1260000) {
                for (int i = 14; i >= 1; i--) {
                    save.config[i] = save.config[i - 1];
                }
                save.config[0] = save.config[101];
                for (int i = 101; i <= 106; i++) {
                    save.config[i] = save.config[i + 1];
                }
                save.config[107] = 0;
            }
            if (save.version < 1270000) {
                if (save.config.Length < 120) {
                    int[] configTemp = new int[120];
                    for (int i = 0; i < save.config.Length; i++) {
                        configTemp[i] = save.config[i];
                    }
                    save.config = configTemp;
                }
                for (int i = 108; i >= 102; i--) {
                    save.config[i] = save.config[i - 1];
                }
                save.config[101] = 0;
            }
            if (save.version < 1300000) {
                int[] configTemp = new int[130];
                for (int i = 0; i < 110; i++) {
                    if (i < 100) {
                        configTemp[i] = save.config[i];
                    } else {
                        configTemp[i + 10] = save.config[i];
                    }
                }
                configTemp[100] = save.config[117];
                configTemp[101] = save.config[116];
                configTemp[102] = save.config[113];
                configTemp[103] = save.config[114];
                configTemp[104] = save.config[110];
                configTemp[105] = save.config[111];
                configTemp[106] = save.config[112];
                configTemp[107] = save.config[115];

                configTemp[120] = configTemp[114];
                configTemp[121] = configTemp[115];
                configTemp[115] = configTemp[116];
                configTemp[116] = configTemp[117];
                configTemp[114] = 0;
                configTemp[117] = 0;
                configTemp[119] = 0;
                configTemp[122] = 0;
                configTemp[123] = 0;
                configTemp[124] = 0;
                configTemp[125] = 0;

                if (configTemp[24] == 2) {
                    configTemp[24] = 4;
                } else if (configTemp[24] >= 3) {
                    configTemp[24] = 7;
                }

                save.config = configTemp;
            }
        }

        if (save.config.Length < configMax) {
            int[] configTemp = new int[configMax];
            for (int i = 0; i < save.config.Length; i++) {
                configTemp[i] = save.config[i];
            }
            save.config = configTemp;
        }

    }

    bool AnyTrophyExist() {
        for (int i = 0; i < save.trophy.Length; i++) {
            if (save.trophy[i] != 0) {
                return true;
            }
        }
        return false;
    }

    void ShiftTrophy(int startIndex, int shiftNum) {
        int[] trophyTemp = new int[trophyArrayMax];
        for (int index = 0; index + shiftNum < trophyArrayMax * 32; index++) {
            int arrayNum = index / 32;
            int bitNum = 1 << (index % 32);
            if ((save.trophy[arrayNum] & bitNum) != 0) {
                int newArrayNum = index >= startIndex ? (index + shiftNum) / 32 : arrayNum;
                int newBitNum = index >= startIndex ? 1 << ((index + shiftNum) % 32) : bitNum;
                trophyTemp[newArrayNum] |= newBitNum;
            }
        }
        save.trophy = trophyTemp;
    }

    public void CheckMinmi() {
        bool minmiSilverSave = minmiSilver;
        megatonCoin = save.HaveSpecificItem(megatonCoinID);
        minmiBlue = save.HaveSpecificItem(minmiBlueID);
        minmiRed = save.HaveSpecificItem(minmiRedID);
        minmiPurple = save.HaveSpecificItem(minmiPurpleID);
        minmiBlack = save.HaveSpecificItem(minmiBlackID);
        minmiSilver = save.HaveSpecificItem(minmiSilverID);
        minmiGolden = save.HaveSpecificItem(minmiGoldenID);
        if (megatonCoin) {
            int count = save.NumOfSpecificItems(megatonCoinID);
            if (count <= 1) {
                megatonCoinSpeedMul = 0.9f;
            } else {
                megatonCoinSpeedMul = MyMath.PowInt(0.9f, count);
            }
        } else {
            megatonCoinSpeedMul = 1f;
        }
        if (minmiSilver != minmiSilverSave && Time.timeScale >= 0.75f) {
            Time.timeScale = GetStandardTimeScale();
        }
        if (save.stageReport.Length >= stageReportMax) {
            int temp = save.stageReport[stageReport_Minmi];
            if (!minmiBlue && (temp & (1 << 0)) != 0) {
                temp -= (1 << 0);
            }
            if (!minmiRed && (temp & (1 << 1)) != 0) {
                temp -= (1 << 1);
            }
            if (!minmiPurple && (temp & (1 << 2)) != 0) {
                temp -= (1 << 2);
            }
            if (!minmiBlack && (temp & (1 << 3)) != 0) {
                temp -= (1 << 3);
            }
            if (!minmiSilver && (temp & (1 << 4)) != 0) {
                temp -= (1 << 4);
            }
            if (minmiGolden && (temp & (1 << 5)) == 0) {
                temp += (1 << 5);
            }
            save.stageReport[stageReport_Minmi] = temp;
        }
    }

    void LoadExpTable() {
        string filePath = "Text/exp";        
        TextAsset csv = Resources.Load<TextAsset>(filePath);
        StringReader reader = new StringReader(csv.text);
        expTable = new int[levelMax];
        for (int i = 0; i < levelMax && reader.Peek() > -1; i++) {
            expTable[i] = int.Parse(reader.ReadLine());
        }
    }

    void LoadLevelStatusTable() {
        string filePath = "Text/level";
        TextAsset csv = Resources.Load<TextAsset>(filePath);
        StringReader reader = new StringReader(csv.text);
        levelStatusTable = new float[levelMax];
        for (int i = 0; i < levelMax && reader.Peek() > -1; i++) {
            levelStatusTable[i] = int.Parse(reader.ReadLine()) * 0.001f;
        }
    }

    private void LoadMod() {
        string directoryPath = Application.dataPath + "/mods";
        if (Directory.Exists(directoryPath)) {
            char[] charSeparators = new char[] { '\t' };
            // Stage Part
            bool foundFile = false;
            string csvPath = directoryPath + "/stage.csv";
            if (File.Exists(csvPath)) {
                foundFile = true;
            } else {
                csvPath = directoryPath + "/stage.txt";
                if (File.Exists(csvPath)) {
                    foundFile = true;
                }
            }
            if (foundFile) {
                FileInfo fi = new FileInfo(csvPath);
                try {
                    using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8)) {
                        int stageIndex = -1;
                        int floorIndex = -1;
                        int linesCount = 0;
                        int listIndex = -1;
                        modFlag = true;
                        stageModFlag = true;
                        floorModInfo = new List<FloorModInfo>(64);
                        while (sr.Peek() > -1) {
                            linesCount++;
                            bool modError = false;
                            string[] values = sr.ReadLine().Split(charSeparators, 3, System.StringSplitOptions.None);
                            if (values.Length > 0 && !string.IsNullOrEmpty(values[0])) {
                                if (values.Length < 2) {
                                    modError = true;
                                } else {
                                    switch (values[0]) {
                                        case "STAGE":
                                            if (int.TryParse(values[1], out int stageTemp)) {
                                                stageIndex = stageTemp;
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "FLOOR":
                                            if (int.TryParse(values[1], out int floorTemp)) {
                                                floorIndex = floorTemp - 1;
                                                floorModInfo.Add(new FloorModInfo(stageIndex * 100 + floorIndex));
                                                listIndex++;
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "MUSIC":
                                            if (stageIndex >= 0 && floorIndex >= 0 && listIndex >= 0 && int.TryParse(values[1], out int musicTemp)) {
                                                floorModInfo[listIndex].music = musicTemp - 1;
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "BOSSMUSIC":
                                            if (stageIndex >= 0 && floorIndex >= 0 && listIndex >= 0 && int.TryParse(values[1], out int bossMusicTemp)) {
                                                floorModInfo[listIndex].bossMusic = bossMusicTemp - 1;
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "AMBIENT":
                                            if (stageIndex >= 0 && floorIndex >= 0 && listIndex >= 0 && int.TryParse(values[1], out int ambientTemp)) {
                                                floorModInfo[listIndex].ambient = ambientTemp - 1;
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        case "SKY":
                                            if (stageIndex >= 0 && floorIndex >= 0 && listIndex >= 0 && int.TryParse(values[1], out int skyTemp)) {
                                                floorModInfo[listIndex].sky = skyTemp;
                                            } else {
                                                modError = true;
                                            }
                                            break;
                                        default:
                                            modError = true;
                                            break;
                                    }
                                }
                            }
                            if (modError) {
                                SetModError(ModType.Stage, linesCount);
                            }
                        }
                        if (floorModInfo.Count > 1) {
                            floorModInfo.Sort((a, b) => a.id - b.id);
                        }
                    }
                } catch {
                    SetModError(ModType.Stage, 0);
                }
            }
        }
    }

    public int GetLevel(int exp) {
        for (int i = 0; i < expTable.Length; i++) {
            if (exp < expTable[i]) {
                return i + 1;
            }
        }
        return levelMax;
    }

    public int GetLevelNow {
        get {
            return save.config[Save.configID_LevelLimit] > 0 ? Mathf.Min(save.config[Save.configID_LevelLimit], save.Level) : save.config[Save.configID_LevelLimit] < 0 ? Mathf.Min(levelLimitAuto, save.Level) : save.Level;
        }
    }

    public float GetLevelStatus(int level = 0) {
        return levelStatusTable[level < 0 ? 0 : level >= levelStatusTable.Length ? levelStatusTable.Length - 1 : level];
    }

    public float GetLevelStatusNow() {
        return GetLevelStatus(GetLevelNow - 1);
    }

    public int GetDenotativeMaxFloor(int stageNumber) {
        if (stageNumber >= 0 && stageNumber < denotativeMaxFloor.Length) {
            return denotativeMaxFloor[stageNumber];
        } else {
            return -1;
        }
    }

    public int GetLockedArmsID(int type) {
        if (type >= 0 && type < save.armsLocked.Length) {
            return armsLockID[type][Mathf.Clamp(save.armsLocked[type] - 1, 0, armsLockID[type].Length - 1)];
        }
        return 0;
    }

    public int GetJaparimanCount() {
        return Instance.save.NumOfSpecificItems(japarimanID) + Instance.save.NumOfSpecificItems(japariman3SetID) * 3 + Instance.save.NumOfSpecificItems(japariman5SetID) * 5;
    }

    public int GetLackExp(int targetLevel) {
        if (targetLevel <= 1 || targetLevel >= levelMax) {
            return 0;
        } else if (save.exp >= expTable[targetLevel - 2]) {
            return 0;
        } else {
            return expTable[targetLevel - 2] - save.exp;
        }
    }

    void FixProgressWeapon() {
        if (!IsPlayerAnother) {
            int[,] progressWeapon = new int[11, 2] { { 0, 0 }, { 0, 1 }, { 0, 2 }, { 2, 24 }, { 3, 25 }, { 4, 26 }, { 6, 27 }, { 7, 28 }, { 8, 29 }, { 9, 30 }, { 11, 31 } };
            for (int i = 0; i < 11; i++) {
                if (save.progress >= progressWeapon[i, 0] && save.weapon[progressWeapon[i, 1]] == 0) {
                    save.weapon[progressWeapon[i, 1]] = 1;
                    save.equip[progressWeapon[i, 1]] = 1;
                } else if (save.progress < progressWeapon[i, 0] && save.weapon[progressWeapon[i, 1]] != 0) {
                    save.progress = progressWeapon[i, 0];
                }
            }
        }
    }

    public void InitializeForPlayerAnother() {
        bool equipResetFlag = false;
        if (save.exp < 1006428) {
            save.exp = 1006428;
        }
        if (save.hpUpSale < 30 || save.stUpSale < 30) {
            equipResetFlag = true;
            save.hpUpSale = 30;
            save.stUpSale = 30;
        }
        for (int i = 0; i < weaponMax; i++) {
            if (i != 2 && save.weapon[i] == 0) {
                save.weapon[i] = 1;
                if (i != 3 && i != 4 && i != 6 && i != 7 && i != 9) {
                    save.equip[i] = 1;
                }
            }
        }
        for (int i = 0; i < hpUpNFSMax; i++) {
            if (save.hpUpNFS[i] == 0) {
                save.hpUpNFS[i] = 1;
                equipResetFlag = true;
            }
        }
        for (int i = 0; i < stUpNFSMax; i++) {
            if (save.stUpNFS[i] == 0) {
                save.stUpNFS[i] = 1;
                equipResetFlag = true;
            }
        }
        for (int i = 0; i < inventoryNFSMax; i++) {
            if (save.inventoryNFS[i] == 0) {
                save.inventoryNFS[i] = 1;
                equipResetFlag = true;
            }
        }
        if (equipResetFlag) {
            save.equipedHpUp = hpUpNFSMax + hpUpSaleMax;
            save.equipedStUp = stUpNFSMax + stUpSaleMax;
            save.equipedInvUp = inventoryNFSMax;
        }
    }

    public void DataSave(int saveSlot = 0) {
        if (saveSlot >= 0 && saveSlot < saveSlotMax) {
            currentSaveSlot = saveSlot;
            save.dataExist = 1;
            save.version = version;
            if (save.date == null || save.date.Length < 6) {
                save.date = new int[6];
            }
            System.DateTime dateTime = System.DateTime.Now;
            save.date[0] = dateTime.Year;
            save.date[1] = dateTime.Month;
            save.date[2] = dateTime.Day;
            save.date[3] = dateTime.Hour;
            save.date[4] = dateTime.Minute;
            save.date[5] = dateTime.Second;
            SaveData.SetInt(str_LastSaveSlot, saveSlot);
            SaveData.SetClass<Save>(str_Save + saveSlot.ToString(), save);
            SaveData.Save();
        }
    }

    public void DataLoad(int saveSlot = 0) {
        if (saveSlot >= 0 && saveSlot < saveSlotMax) {
            currentSaveSlot = saveSlot;
            SaveData.SetInt(str_LastSaveSlot, saveSlot);
            save = SaveData.GetClass<Save>(str_Save + saveSlot.ToString(), null);
            if (save == null) {
                save = new Save();
            }
            if (save.dataExist != 0){
                FixVersion();
            }
            FixProgressWeapon();
            save.config[Save.configID_QualityLevel] = QualitySettings.GetQualityLevel();
            CheckMinmi();
            if (save.config.Length < configMax) {
                save.ResetConfig();
            }
            InitLanguage();
            equipmentLimitEnabled = save.config[Save.configID_EquipmentLimit] != 0;
        }
    }

    public void SetClearFlag(int flag) {
        int nowFlag = SaveData.GetInt(str_GameCleared, 0);
        nowFlag |= flag;
        SaveData.SetInt(str_GameCleared, nowFlag);
    }

    public int GetClearFlag() {
        return SaveData.GetInt(str_GameCleared, 0);
    }

    public void SetDisplayFormat(int param) {
        SaveData.SetInt(str_DisplayFormat, param);
    }

    public int GetDisplayFormat() {
        return SaveData.GetInt(str_DisplayFormat, 0);
    }

    public bool GetDefeatSinWRComplete() {
        if (CharacterDatabase.Instance && save.defeatEnemy.Length >= enemyMax * enemyLevelMax) {
            bool answer = true;
            for (int i = 0; i < CharacterDatabase.sinWREnemyIDArray.Length; i++) {
                int enemyIndex = CharacterDatabase.sinWREnemyIDArray[i] * enemyLevelMax + CharacterDatabase.sandstarRawLevel;
                if (save.defeatEnemy[enemyIndex] <= 0) {
                    answer = false;
                    break;
                }
            }
            return answer;
        } else {
            return false;
        }
    }

    public int GetDefeatSum() {
        int sum = 0;
        if (CharacterDatabase.Instance) {
            for (int i = 0; i < enemyMax; i++) {
                for (int j = 0; j < enemyLevelMax; j++) {
                    if (CharacterDatabase.Instance.enemy[i].statusExist[j]){
                        int index = i * enemyLevelMax + j;
                        if (index < save.defeatEnemy.Length && save.defeatEnemy[index] > 0) {
                            sum += save.defeatEnemy[index];
                        }
                    }
                }
            }
        }
        return sum;
    }

    public bool DataCopy(int saveSlot) {
        if (saveSlot >= 0 && saveSlot < saveSlotMax) {
            save = TempLoad(saveSlot);
            if (save != null && save.dataExist != 0) {
                FixVersion();
                Save tempSave;
                for (int i = 1; i < saveSlotMax; i++) {
                    int targetSlot = (saveSlot + i) % saveSlotMax;
                    tempSave = TempLoad(targetSlot);
                    if (tempSave == null || tempSave.dataExist == 0) {
                        DataSave(targetSlot);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void DataDelete(int saveSlot) {
        if (saveSlot >= 0 && saveSlot < saveSlotMax) {
            currentSaveSlot = saveSlot;
            save = new Save();
            SaveData.SetInt(str_LastSaveSlot, saveSlot);
            SaveData.SetClass<Save>(str_Save + saveSlot.ToString(), save);
            SaveData.Save();
        }
    }

    public int GetLastSaveSlot() {
        return SaveData.GetInt(str_LastSaveSlot, -1);
    }

    public Save TempLoad(int saveSlot = 0) {
        return SaveData.GetClass<Save>(str_Save + saveSlot.ToString(), null);
    }

    public void LoadScene(string sceneName) {
        switch (sceneName) {
            case "Title":
                state = State.title;
                break;
            case "Play":
                state = State.play;
                break;
        }
        if (state == State.title) {
            tutorialMemory = 0;
            gameOverCount = 0;
            gameOverFlag = false;
        }
        SceneManager.LoadScene(sceneName);
    }

    public void SetVibration(float motorLevel, float duration, bool isDamaged = false) {
        if (save.config[Save.configID_GamepadVibration] >= 2 || (save.config[Save.configID_GamepadVibration] == 1 && isDamaged)) {
            foreach (Joystick joy in playerInput.controllers.Joysticks) {
                if (!joy.supportsVibration) continue;
                if (joy.vibrationMotorCount > 0) {
                    for (int i = 0; i < joy.vibrationMotorCount; i++) {
                        joy.SetVibration(i, motorLevel, duration);
                    }
                }
            }
        }
    }

    public Vector2Int MoveCursor(bool continuous = false, float horizontalInterval = 0.1f, float verticalInterval = 0.1f) {
        move = vec2IntZero;
        axisInput.x = playerInput.GetAxis(RewiredConsts.Action.Horizontal);
        axisInput.y = playerInput.GetAxis(RewiredConsts.Action.Vertical);
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
                moveCursorTimeRemain -= realDeltaTime;
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
                if (move.x != 0) {
                    moveCursorTimeRemain = horizontalInterval;
                } else if (move.y != 0) {
                    moveCursorTimeRemain = verticalInterval;
                }
            }
        }
        return move;
    }
    
    public void ApplyVolume() {
        if (audioMixer != null) {
            float musicVolRate = 1f;
            if (BGM.Instance) {
                musicVolRate = BGM.Instance.masterVolumeRate;
            }
            audioMixer.SetFloat(volumeName_Se, save.seVolume > 0 ? Mathf.Log(save.seVolume * 0.01f, 2) * 10 : -80);
            audioMixer.SetFloat(volumeName_System, save.seVolume > 0 ? Mathf.Log(save.seVolume * 0.01f, 2) * 10 : -80);
            audioMixer.SetFloat(volumeName_Music, save.musicVolume > 0 ? Mathf.Log(save.musicVolume * 0.01f * musicVolRate, 2) * 10 : -80);
            audioMixer.SetFloat(volumeName_Ambient, save.ambientVolume > 0 ? Mathf.Log(save.ambientVolume * 0.01f, 2) * 10 : -80);
            volumeSaveArray[0] = save.seVolume;
            volumeSaveArray[1] = save.musicVolume;
            volumeSaveArray[2] = save.ambientVolume;
        }
    }

    public bool ApplySpeakerMode() {
        if (speakerMode != save.config[Save.configID_SpeakerMode]) {
            speakerMode = save.config[Save.configID_SpeakerMode];
            AudioConfiguration config = AudioSettings.GetConfiguration();
            switch (speakerMode) {
                case 0:
                    config.speakerMode = AudioSpeakerMode.Mono;
                    break;
                case 1:
                    config.speakerMode = AudioSpeakerMode.Stereo;
                    break;
                case 2:
                    config.speakerMode = AudioSpeakerMode.Quad;
                    break;
                case 3:
                    config.speakerMode = AudioSpeakerMode.Surround;
                    break;
                case 4:
                    config.speakerMode = AudioSpeakerMode.Mode5point1;
                    break;
                case 5:
                    config.speakerMode = AudioSpeakerMode.Mode7point1;
                    break;
            }
            AudioSettings.Reset(config);
            return true;
        }
        return false;
    }

    public void ChangeSnapshot(string snapshotName, float transitionTime = 0f) {
        if (save.config[Save.configID_AcousticEffects] == 0) {
            snapshotName = str_Snapshot;
        }
        if (!string.IsNullOrEmpty(snapshotName) && nowSnapshotName != snapshotName) {
            nowSnapshotName = snapshotName;
            AudioMixerSnapshot[] snapshot = new AudioMixerSnapshot[1];
            float[] amount = new float[] { 1 };
            snapshot[0] = audioMixer.FindSnapshot(snapshotName);
            if (snapshot[0]) {
                audioMixer.TransitionToSnapshots(snapshot, amount, transitionTime);
            }
        }
    }

    public void RestartSnapshot() {
        if (!string.IsNullOrEmpty(nowSnapshotName)) {
            AudioMixerSnapshot[] snapshot = new AudioMixerSnapshot[1];
            float[] amount = new float[] { 1 };
            snapshot[0] = audioMixer.FindSnapshot(nowSnapshotName);
            if (snapshot[0]) {
                audioMixer.TransitionToSnapshots(snapshot, amount, 0.5f);
            }
        }
    }
    
    void CalcRealDeltaTime() {
        if (lastRealTime == 0) {
            lastRealTime = Time.realtimeSinceStartup;
        }
        realDeltaTime = Time.realtimeSinceStartup - lastRealTime;
        lastRealTime = Time.realtimeSinceStartup;
    }

    public void InitLanguage() {
        if (save.language < 0 || save.language >= languageMax) {
            switch (Application.systemLanguage) {
                case SystemLanguage.Japanese:
                    save.language = languageJapanese;
                    break;
                case SystemLanguage.English:
                    save.language = languageEnglish;
                    break;
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    save.language = languageChinese;
                    break;
                case SystemLanguage.Korean:
                    save.language = languageKorean;
                    break;
                default:
                    save.language = languageEnglish;
                    break;
            }
        }
        if (save.language != currentLanguage) {
            TextManager.LANGUAGE managerLanguage;
            switch (save.language) {
                case languageJapanese:
                    managerLanguage = TextManager.LANGUAGE.JAPANESE;
                    break;
                case languageEnglish:
                    managerLanguage = TextManager.LANGUAGE.ENGLISH;
                    break;
                case languageChinese:
                    managerLanguage = TextManager.LANGUAGE.CHINESE;
                    break;
                case languageKorean:
                    managerLanguage = TextManager.LANGUAGE.KOREAN;
                    break;
                default:
                    managerLanguage = TextManager.LANGUAGE.ENGLISH;
                    break;
            }
            TextManager.Init(managerLanguage);
            currentLanguage = save.language;
        }
    }

    public void LoadLastSaveSlot() {
        currentSaveSlot = SaveData.GetInt(str_LastSaveSlot, -1);
        if (currentSaveSlot >= 0) {
            save = TempLoad(currentSaveSlot);
            FixVersion();
        } else {
            save = new Save();
            save.language = -1;
        }
        InitLanguage();
    }

    public int GetJaparibunType(int id) {
        int answer = 0;
        if (id == japarimanID) {
            answer = 1;
        } else if (id == japariman3SetID) {
            answer = 3;
        } else if (id == japariman5SetID) {
            answer = 5;
        }
        return answer;
    }

    public void SetSecret(SecretType secretType) {
        save.secret |= (1 << (int)secretType);
    }

    public bool GetSecret(SecretType secretType) {
        return (save.secret & (1 << (int)secretType)) != 0;
    }

    public bool GetSecretCompleted() {
        int composedFlag = (1 << (int)SecretType.SingularityLB) + (1 << (int)SecretType.UndergroundLB) + (1 << (int)SecretType.Toudai) + (1 << (int)SecretType.SafePack) + (1 << (int)SecretType.SkytreeCleared);
        return (save.secret & composedFlag) == composedFlag;
    }

    public bool GetMinmiCompleted() {
        int composedFlag = (1 << 0) + (1 << 1) + (1 << 2) + (1 << 3) + (1 << 4) + (1 << 5);
        return (save.minmi & composedFlag) == composedFlag;
    }

    public bool GetPerfectCompleted() {
        return (save.progress >= gameClearedProgress && 
            GetMinmiCompleted() &&
            GetSecretCompleted() && 
            GetDocumentCompleted() && 
            GetDefeatSinWRComplete() &&
            save.GetRescueNow() >= rescueMax && 
            (GetClearFlag() & 3) == 3);
    }

    public bool GetDocumentCompleted() {
        if (save.document.Length >= documentMax && (save.document[documentFragmentID] & documentFragmentCondition) == documentFragmentCondition) {
            bool answer = true;
            for (int i = 0; i < save.document.Length; i++) {
                if (save.document[i] == 0) {
                    answer = false;
                    break;
                }
            }
            return answer;
        }
        return false;
    }

    public void SetModError(ModType modType, int modLine) {
        if (modErrorIndex < modErrorMax) {
            modErrorTypes[modErrorIndex] = modType;
            modErrorLines[modErrorIndex] = modLine;
        }
        modErrorIndex++;
    }

    public void GetModErrorText(TMP_Text text) {
        if (modErrorIndex > 0) {
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            int errorMaxTemp = Mathf.Min(modErrorIndex, modErrorMax);
            for (int i = 0; i < errorMaxTemp; i++) {
                if (modErrorTypes[i] == ModType.Music) {
                    sb.Append("Error: mods/music ");
                } else if (modErrorTypes[i] == ModType.Stage) {
                    sb.Append("Error: mods/stage ");
                }
                if (modErrorLines[i] > 0) {
                    sb.AppendFormat("{0}", modErrorLines[i]).AppendLine();
                }
            }
            if (modErrorIndex > modErrorMax) {
                sb.Append("and more...").AppendLine();
            }
            text.text = sb.ToString();
            if (!text.enabled) {
                text.enabled = true;
            }
        }
    }

    public float GetStandardTimeScale() {
        return minmiSilver ? 1.2f : save.difficulty <= difficultyNT && save.config[Save.configID_GameSpeed] < 0 ? Mathf.Clamp(1.0f + save.config[Save.configID_GameSpeed] * 0.01f, 0.8f, 1.0f) : 1.0f;
    }

    public void CheckTutorial() {
        int answer = 0;
        tutorialContinuousFlag = false;
        if (StageManager.Instance && PauseController.Instance) {
            int stageNumber = StageManager.Instance.stageNumber;
            int floorNumber = StageManager.Instance.floorNumber;
            switch (save.progress) {
                case 0:
                    if (save.tutorial < 0) {
                        save.tutorial = 0;
                    }
                    break;
                case 1:
                    if (save.tutorial < 11) {
                        save.tutorial = 11;
                    }
                    break;
                case 2:
                    if (save.tutorial < 18) {
                        save.tutorial = 18;
                    }
                    break;
                case 3:
                    if (save.tutorial < 20) {
                        save.tutorial = 20;
                    }
                    break;
                case 4:
                    if (save.tutorial < 21) {
                        save.tutorial = 21;
                    }
                    break;
                case 5:
                    if (save.tutorial < 22) {
                        save.tutorial = 22;
                    }
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    if (save.tutorial < 26) {
                        save.tutorial = 26;
                    }
                    break;
                case 12:
                case 13:
                    if (save.tutorial < 28) {
                        save.tutorial = 28;
                    }
                    break;
            }
            if (save.progress < 3 && save.difficulty >= 2 && gameOverCount >= 3) {
                gameOverCount = 0;
                answer = gameOverTutorialIndex;
            } else {
                switch (save.tutorial) {
                    case 0:
                        if (stageNumber == 0 && floorNumber == 0 && save.progress == 0) {
                            save.tutorial = answer = 1;
                            tutorialContinuousFlag = true;
                        }
                        break;
                    case 1:
                        if (stageNumber == 0 && floorNumber == 0 && save.progress == 0) {
                            save.tutorial = answer = 2;
                            tutorialContinuousFlag = true;
                        }
                        break;
                    case 2:
                        if (stageNumber == 0 && floorNumber == 0 && save.progress == 0) {
                            save.tutorial = answer = 3;
                            tutorialContinuousFlag = true;
                        }
                        break;
                    case 3:
                        if (stageNumber == 0 && floorNumber == 0 && save.progress == 0) {
                            save.tutorial = answer = 4;
                        }
                        break;
                    case 4:
                        if (stageNumber == 0 && floorNumber == 1 && save.progress == 0) {
                            save.tutorial = answer = 5;
                        }
                        break;
                    case 5:
                        if (stageNumber == 1 && floorNumber == 0) {
                            save.tutorial = answer = 6;
                        }
                        break;
                    case 6:
                        if (stageNumber == 1 && floorNumber == 1) {
                            save.tutorial = answer = 7;
                        }
                        break;
                    case 7:
                        if (stageNumber == 1 && floorNumber == 2) {
                            save.tutorial = answer = 8;
                        }
                        break;
                    case 8:
                        if (stageNumber == 1 && floorNumber == 3) {
                            save.tutorial = answer = 9;
                        }
                        break;
                    case 9:
                        if (stageNumber == 1 && floorNumber == 4) {
                            save.tutorial = answer = 10;
                        }
                        break;
                    case 10:
                        if (stageNumber == 1 && floorNumber == 5) {
                            save.tutorial = answer = 11;
                        }
                        break;
                    case 11:
                        if (stageNumber == 0 && floorNumber == 0 && save.progress == 1) {
                            save.tutorial = answer = 12;
                        }
                        break;
                    case 12:
                    case 13:
                    case 14:
                        if (stageNumber == 2 && floorNumber == 0) {
                            save.tutorial = answer = 15;
                        }
                        break;
                    case 15:
                        if (stageNumber == 2 && floorNumber == 2) {
                            save.tutorial = answer = 16;
                        }
                        break;
                    case 16:
                        if (stageNumber == 2 && floorNumber == 4) {
                            save.tutorial = answer = 17;
                        }
                        break;
                    case 17:
                        if (stageNumber == 2 && floorNumber == 6) {
                            save.tutorial = answer = 18;
                        }
                        break;
                    case 18:
                        if (stageNumber == 3 && floorNumber == 0) {
                            save.tutorial = answer = 19;
                        }
                        break;
                    case 19:
                        if (stageNumber == 3 && floorNumber == 4) {
                            save.tutorial = answer = 20;
                        }
                        break;
                    case 20:
                        if (stageNumber == 4 && floorNumber == 0) {
                            save.tutorial = answer = 21;
                        }
                        break;
                    case 21:
                        if (stageNumber == 5 && floorNumber == 0) {
                            save.tutorial = answer = 22;
                        }
                        break;
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                        if (stageNumber == 6 && floorNumber == 0) {
                            save.tutorial = answer = 26;
                        }
                        break;
                    case 26:
                    case 27:
                        if (stageNumber == 0 && floorNumber == 0 && save.progress == 11) {
                            save.tutorial = answer = 28;
                        }
                        break;
                    case 28:
                        if (stageNumber == 14 && floorNumber == 0) {
                            save.tutorial = answer = 29;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (answer >= 1) {
                if (answer != tutorialMemory) {
                    if (PauseController.Instance) {
                        PauseController.Instance.SetTutorial(answer);
                    }
                    if (answer != gameOverTutorialIndex) {
                        tutorialMemory = answer;
                    }
                }
            } else {
                tutorialMemory = 0;
            }
        }
    }

    public void SetTutorial_SpecialMove(int index) {
        if (save.tutorial < skillTutorialIndex[index, 1]) {
            save.tutorial = skillTutorialIndex[index, 1];
        }
        if (PauseController.Instance) {
            PauseController.Instance.SetTutorial(skillTutorialIndex[index, 1]);
        }
        tutorialMemory = skillTutorialIndex[index, 1];
    }

    public void ChangeTimeScale(bool stop, bool checkMinmiSilver = true) {
        float scaleTemp = stop ? 0 : checkMinmiSilver ? GetStandardTimeScale() : 1;
        if (scaleTemp != Time.timeScale) {
            Time.timeScale = scaleTemp;
            save.SetRunInBackground();
        }
    }

    public void InitConfigDefaultArray() {
        if (!configDefaultInited) {
            configDefaultInited = true;
            configDefaultValues = save.GetInitializedConfigArray();
        }
    }

    public int GetGameClearDifficulty() {
        int minDifficulty = 1;
        if (save.clearDifficulty.Length > 14) {
            minDifficulty = save.difficulty;
            if (!IsPlayerAnother) {
                for (int i = 1; i < 14; i++) {
                    if (save.clearDifficulty[i] < minDifficulty) {
                        minDifficulty = save.clearDifficulty[i];
                    }
                }
            }
        }
        return minDifficulty;
    }

    public bool MouseEnabled => (state == State.title || save.config[Save.configID_UseMouseForUI] != 0);

    public bool MouseCancelling => (!hasFocus || mouseStoppingTime >= 0.1f || !(state == State.title || save.config[Save.configID_UseMouseForUI] != 0));

    public bool IsPlayerAnother => (save.playerType == playerType_Another || save.playerType == playerType_AnotherEscape);

    public bool GetCancelButtonDown {
        get {
            if (playerInput.GetButtonDown(RewiredConsts.Action.Cancel)) {
                return true;
            } else if (MouseEnabled && Input.GetMouseButtonDown(1) &&
                mousePositionNow.x >= 0 &&
                mousePositionNow.y >= 0 &&
                mousePositionNow.x < Screen.width &&
                mousePositionNow.y < Screen.height) {
                return true;
            }
            return false;
        }
    }

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
            playerInput = ReInput.players.GetPlayer(0);
            LoadExpTable();
            LoadLevelStatusTable();
            LoadMod();
            LoadLastSaveSlot();
            DOTween.Init();
            mousePositionPre = Input.mousePosition;
            mouseStoppingTime = 10f;
        }
    }

    private void Start() {
        ApplyVolume();
    }

    private void OnApplicationFocus(bool focus) {
        hasFocus = focus;
        if (focus) {
            buttonDownOnFocus = true;
            focusRestTime = 0.1f;
        }
    }

    private void Update() {
        CalcRealDeltaTime();
        if (Time.timeScale > 0f) {
            time += Time.deltaTime;
        }
        float unscaledDeltaTime = Time.unscaledDeltaTime;
        unscaledTime += unscaledDeltaTime;

        if (mouseStoppingTime < 10f) {
            mouseStoppingTime += unscaledDeltaTime;
        }
        mousePositionNow = Input.mousePosition;
        if (buttonDownOnFocus && !Input.GetMouseButton(0)) {
            buttonDownOnFocus = false;
        }
        if (!hasFocus || buttonDownOnFocus || focusRestTime > 0f) {
            mouseStoppingTime = 10f;
            focusRestTime -= unscaledDeltaTime;
            mousePositionPre = mousePositionNow;
        } else if (mousePositionNow != mousePositionPre || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.mouseScrollDelta.y != 0) {
            mouseStoppingTime = 0f;
            mousePositionPre = mousePositionNow;
        }
        save.SetShowCursor(PauseController.Instance ? PauseController.Instance.GetNowPausing() : SaveController.Instance ? SaveController.Instance.save.canvas.enabled : false, state == State.title);
    }

    public void FixZakoRushTimeLength() {
        if (save.zakoRushClearTime.Length == 40) {
            int[] zakoTemp = new int[zakoRushArrayMax];
            for (int i = 0; i < 20; i++) {
                zakoTemp[i] = save.zakoRushClearTime[i];
            }
            for (int i = 20; i < 40; i++) {
                zakoTemp[i + 8] = save.zakoRushClearTime[i];
            }
            save.zakoRushClearTime = zakoTemp;
        } else {
            int[] zakoTemp = new int[zakoRushArrayMax];
            int length = Mathf.Min(save.zakoRushClearTime.Length, zakoRushArrayMax);
            for (int i = 0; i < length; i++) {
                zakoTemp[i] = save.zakoRushClearTime[i];
            }
            save.zakoRushClearTime = zakoTemp;
        }
    }

    public void SetSteamAchievement(string apiName) {
        if (steamManager != null && SteamManager.Initialized && save.config[Save.configID_DisableTrophy] == 0) {
            SteamManager.SetAchievement(apiName);
        }
    }


}
