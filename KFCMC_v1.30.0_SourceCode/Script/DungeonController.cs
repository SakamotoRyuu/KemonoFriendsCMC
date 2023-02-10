using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using StrOpe = StringOperationUtil.OptimizedStringOperation;

public class DungeonController : MonoBehaviour {

    [System.Serializable]
    public class GroundSettings {
        public bool enabled;
        public GameObject ground;
        public GameObject grass;
        public float grassDensity;
        [System.NonSerialized]
        public Transform container;
    }

    [System.Serializable]
    public class TileSettings {
        public bool enabled;
        public GameObject wallPrefab;
        public bool wallModify;
        public GameObject roomPrefab;
        public bool roomModify;
        public GameObject passagePrefab;
        public bool passageModify;
        public GameObject pillarPrefab;
        public GameObject endOutsidePrefab;
        public GameObject endInsidePrefab;
        public int endThickness = 0;
        public float endHeight = 8f;
        [Range(0, 100)]
        public int endInstantiateProbability = 10;
        public bool passageToWall = false;
        [System.NonSerialized]
        public Transform container;
    }

    [System.Serializable]
    public class GoalSettings {
        public bool enabled;
        public GameObject goalPrefab;
        public int defeatEnemyCondition;
        [System.NonSerialized]
        public GameObject goalInstance;
    }

    [System.Serializable]
    public class TrapSettings {
        public bool enabled;
        public GameObject prefab;
        public float trapCountRatePlus;
        public float heightOffset = 0f;
        public bool avoidWallPassage = false;
        [System.NonSerialized]
        public Transform container;
    }

    [System.Serializable]
    public class TreeFix {
        public bool fixEnabled = false;
        public Vector2Int position;
    }

    [System.Serializable]
    public class TreeSettingsChild {
        public GameObject prefab;
        public float density = 5f;
        public float randomPosRadius = 1f;
        public bool avoidWall = true;
        public bool avoidPassage = true;
        public bool avoidOtherTree = true;
        public bool cannotMoreLay = true;
        public bool isNotRespawnable = true;
        public bool alongWall = false;
        public TreeFix treeFix;
    }

    [System.Serializable]
    public class TreeSettings {
        public bool enabled;
        public TreeSettingsChild[] tree;
        [System.NonSerialized]
        public Transform container;
    }

    [System.Serializable]
    public class ItemSettingsChild {
        public int id;
        [System.NonSerialized]
        public GameObject prefab;
    }

    [System.Serializable]
    public class ItemSettings {
        public bool enabled;
        public ItemSettingsChild[] item;
        [System.NonSerialized]
        public Transform container;
    }

    [System.Serializable]
    public class ChestSettings {
        public bool enabled;
        public int chestCountPlus = 0;
        public int chestRankPointPlus = 0;
        public bool notCallEnemy;
        [System.NonSerialized]
        public Transform container;
    }

    [System.Serializable]
    public class EnemySettingsChild {
        public int id;
        public int level = 1;
        public int maxNum = 20;
        public bool respawnable = true;
        [System.NonSerialized]
        public GameObject prefab;
        [System.NonSerialized]
        public Transform container;
    }

    [System.Serializable]
    public class EnemySettings {
        public bool enabled;
        public EnemySettingsChild[] enemy;
        public bool respawnable = true;
        public bool notActiveDefault = false;
        public bool levelBiasDisabled;
        public int maxNumBias = 0;
        public int pointer;
        [System.NonSerialized]
        public int maxNum;
        [System.NonSerialized]
        public double respawnedTime;
    }

    [System.Serializable]
    public class PosRot {
        public Vector3 position;
        public Vector3 rotation;
    }

    [System.Serializable]
    public class FixSettings {
        public bool enabled = false;
        public bool isWorldPos;
        public PosRot player;
        public PosRot[] enemy;
        public PosRot[] item;
        public Vector3 goalPos;
        public Vector3 shortcutPos;
    }

    public enum DisadvantageType { None, Water, Heat, SandstarRaw, Vacuum };

    public DungeonGenerator generator;
    public GameObject floor;
    public string stageName;
    public bool showFloorNumber = true;
    public string footstepType;
    public bool floorCollider = true;
    public int sceneNumber;
    public int bgmNumber;
    public int bossMusicNumber = -1;
    public int ambientNumber;
    public string snapshotName = "Snapshot";
    public int lightingNumber;
    public bool isBossFloor = false;
    public int generatorLevel = 0;
    public int playerLevelLimit = 100;
    public int armsStageOverride = 0;
    public int playerHpUpLimit = 100;
    public int playerInventoryLimit = 100;
    public int wrSpeedLevel = -1;
    public GroundSettings groundSettings;
    public TileSettings tileSettings;
    public GoalSettings goalSettings;
    public TrapSettings trapSettings;
    public TreeSettings treeSettings;
    public ItemSettings itemSettings;
    public ChestSettings chestSettings;
    public EnemySettings enemySettings;
    public FixSettings fixSettings;
    public bool avoidEnable = true;
    public bool mapEnable = true;
    public bool cameraClippingNear = false;
    public bool dropExpDisabled;
    public bool specialMapCameraEnabled;
    public Vector3 mapCameraPosition;
    public float mapCameraRotation;
    public float mapCameraSize;
    public float referenceHeight;
    public bool isUnderwater;
    public bool isInfinityFar;
    public int slipDamageOverride;
    public int variableLevel;
    public int itemReplaceLevel;
    public bool isPlayerLockonRateMax;
    public bool specialCeilEnabled;
    public bool existWater;
    public bool isXRayFloor;
    public DisadvantageType disadvantageType;
    public bool useGraphAuto;

    [System.NonSerialized]
    public int keyItemRemain;
    [System.NonSerialized]
    public bool keyItemIsLB;
    [System.NonSerialized]
    public bool forceReaper;
    [System.NonSerialized]
    public bool sinWR;
    [System.NonSerialized]
    public bool bossMinmiBlackFlag;
    [System.NonSerialized]
    public bool usedGoldGraph;
    [System.NonSerialized]
    public GameObject shortcutInstance;
    [System.NonSerialized]
    public bool collapseUsed;
    [System.NonSerialized]
    public bool buildUsed;
    [System.NonSerialized]
    public float australopithecusShoutInterval;
    [System.NonSerialized]
    public bool fixEnemyPointer;

    private int[,] map;
    private int[,] mapType;
    private int[,] mapMod;
    private int[,] isPlaced;
    private List<Vector2Int> roomPoints;
    private List<Vector2Int> walkablePoints;
    private Transform[,] tiles;
    private Vector2 offset;
    // private bool setFootstepTypeComplete = false;
    private float updateInterval;
    private GameObject pObj;
    private Transform pTrans;
    private MapChipControl[,] mapWalkables;
    private List<MapChipControl> additionalMapChips;
    private int nullCheckCount;

    private float timeCount;
    private int enemyCountForBias;
    private GameObject reaperEffect;
    private int reaperProgress = 0;
    private Transform trans;
    private Transform enemyContainerSP;
    private float defeatMissionShowTimer;
    private bool defeatMissionCompleted;
    private bool replacedFlag;
    private bool existPlayerHeightWall;
    private Collider[,] playerHeightWallCollider;
    private bool reserveUpdatePlayerHeightWall;
    private bool isActivePlayerHeightWall;
    private Vector2Int playerPointSave = Vector2Int.left;
    private Vector2Int playerPointNow = Vector2Int.zero;
    private float mapCheckInterval;
    private int blackCrystalCountRemain;
    private float disadvantageDamage;
    private bool disadvantageMessageShowed;
    private bool[,] mapCheck = new bool[7, 7];
    private bool mapForceCheckFlag;

    private const int layerPlayer = 1;
    private const int layerGoal = 2;
    private const int layerItem = 3;
    private const int layerTree = 4;
    private const int layerTrap = 5;
    private const int layerChest = 6;
    private const int layerGold = 7;
    private const float chestCountRate = 0.065f;
    private const float trapCountRate = 0.04f;
    private const float trapHeightBias = 0.0025f;
    private const int trapHeightMod = 4;
    private const int mapDenomi = 100000;
    private const int roomDenomi = 1000;
    private const int reaperCondition = 1000;
    private const float defeatEnemyToReaper = 25f;
    private const int standardFloorNumber = 0;
    private const int minmiBlackRate = 4;
    private const float playerHeightWallCondition = 0.05f;
    private const string tag_Untagged = "Untagged";
    private const string tag_Respawn = "Respawn";
    private const string tag_Wall = "Wall";
    private const string tag_Breakable = "Breakable";
    private const string tag_Passage = "Passage";
    private const string tag_Item = "Item";
    private static readonly Vector2 respawnInterval = new Vector2(15f, 45f);
    private static readonly int[] numEnemiesArray = new int[] { 7, 8, 10, 12, 14, 14 };
    private static readonly Vector3 vecZero = Vector3.zero;
    private static readonly Vector3 vecUp = Vector3.up;
    private static readonly Quaternion quaIden = Quaternion.identity;
    private static readonly Vector2Int vec2IntZero = Vector2Int.zero;
    private static readonly Vector2 tileWidth = new Vector2(3f, 3f);

    class WallInfo {
        public int x;
        public int y;
        public int sqrDist;
        public WallInfo(int x, int y, int sqrDist) {
            this.x = x;
            this.y = y;
            this.sqrDist = sqrDist;
        }
    }

    void Awake() {
        trans = transform;
    }

    void Start() {
        if (MessageUI.Instance != null && !string.IsNullOrEmpty(stageName)) {
            string stageMes = "";
            int floorNumTemp = StageManager.Instance.floorNumber + 1;
            if (showFloorNumber) {
                stageMes = StrOpe.i + floorNumTemp.ToString() + TextManager.Get("WORD_FLOOR") + " ";
            }
            if (!string.IsNullOrEmpty(stageName)) {
                stageMes = StrOpe.i + stageMes + TextManager.Get(stageName);
            }
            if (!string.IsNullOrEmpty(stageMes)) {
                MessageUI.Instance.SetMessage(stageMes);
            }
        }
        if (StageManager.Instance != null) {
            StageManager.Instance.ChangeScene(sceneNumber);
            StageManager.Instance.isBossFloor = isBossFloor;
            StageManager.Instance.dropExpDisabled = dropExpDisabled;
            if (CharacterManager.Instance) {
                if (wrSpeedLevel >= 0) {
                    CharacterManager.Instance.SetWRSpeedBias(wrSpeedLevel);
                } else if (StageManager.Instance.dungeonMother && StageManager.Instance.dungeonMother.wrSpeedLevel >= 0) {
                    CharacterManager.Instance.SetWRSpeedBias(StageManager.Instance.dungeonMother.wrSpeedLevel);
                } else {
                    CharacterManager.Instance.SetWRSpeedBias(0);
                }
            }
            if (disadvantageType == DisadvantageType.None && StageManager.Instance.dungeonMother) {
                for (int i = 0; i < StageManager.Instance.dungeonMother.disadvantageProtectMessageShowed.Length; i++) {
                    StageManager.Instance.dungeonMother.disadvantageProtectMessageShowed[i] = false;
                }
            }
        }
        if (LightingDatabase.Instance) {
            LightingDatabase.Instance.SetLighting(lightingNumber);
            CharacterManager.Instance.SetPlayerLightActive();
        }
        PlayBGM();
        updateInterval = 0;
        defeatMissionCompleted = GetDefeatMissionCompleted();
        defeatMissionShowTimer = (defeatMissionCompleted ? -1f : 0.1f);
        GameManager.Instance.levelLimitAuto = playerLevelLimit;
        if (armsStageOverride <= 0) {
            GameManager.Instance.armsStage = StageManager.Instance.stageNumber;
        } else {
            GameManager.Instance.armsStage = armsStageOverride;
        }
        GameManager.Instance.hpUpLimitAuto = playerHpUpLimit;
        GameManager.Instance.stUpLimitAuto = playerHpUpLimit;
        GameManager.Instance.inventoryLimitAuto = playerInventoryLimit;
        CharacterManager.Instance.CheckLimitSet(true, true, true);
        mapForceCheckFlag = true;
    }

    void Update() {
        float deltaTimeCache = Time.deltaTime;
        timeCount += deltaTimeCache;
        if (updateInterval <= 0f) {
            updateInterval += 1f;
            if ((forceReaper || CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.Reaper) != 0 || StageManager.Instance.IsRandomStage) && enemySettings.respawnable && !isBossFloor && StageManager.Instance.GetReaperID() >= 0) {
                float condition = 0f;
                if (forceReaper) {
                    condition = reaperCondition * 2;
                } else if (StageManager.Instance.stageNumber != StageManager.homeStageId) {
                    condition = timeCount + CharacterManager.Instance.defeatEnemyCountSSRaw * defeatEnemyToReaper * (GetMinmiBlackEnabled() ? 1f / (float)minmiBlackRate : 1f);
                }
                switch (reaperProgress) {
                    case 0:
                        if (condition > reaperCondition - 300) {
                            if (MessageUI.Instance) {
                                MessageUI.Instance.SetMessage(TextManager.Get("CAUTION_REAPER_00"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Caution);
                            }
                            UISE.Instance.Play(UISE.SoundName.caution);
                            reaperProgress = 1;
                        }
                        break;
                    case 1:
                        if (condition > reaperCondition - 150) {
                            if (MessageUI.Instance) {
                                MessageUI.Instance.SetMessage(TextManager.Get("CAUTION_REAPER_01"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Caution);
                            }
                            if (EffectDatabase.Instance) {
                                reaperEffect = Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.sandstarRawCloud], trans.position, quaIden, trans);
                            }
                            UISE.Instance.Play(UISE.SoundName.caution);
                            reaperProgress = 2;
                        }
                        break;
                    case 2:
                        if (condition > reaperCondition) {
                            if (MessageUI.Instance) {
                                MessageUI.Instance.SetMessage(TextManager.Get("CAUTION_REAPER_02"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Caution);
                            }
                            int bgmTemp = StageManager.Instance.GetReaperBGM();
                            if (bgmTemp >= 0) {
                                bgmNumber = bgmTemp;
                                PlayBGM();
                                Ambient.Instance.Play(-1);
                            }
                            int enemyTemp = StageManager.Instance.GetReaperID();
                            if (enemyTemp >= 0) {
                                int reaperNum = 1;
                                if (GameManager.Instance.minmiBlack && GameManager.Instance.playerInput.GetButton(RewiredConsts.Action.Special) && GameManager.Instance.playerInput.GetButton(RewiredConsts.Action.Jump)) {
                                    reaperNum = 4;
                                    Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.blackMinmiGlitch], transform);
                                    bossMinmiBlackFlag = true;
                                }
                                for (int i = 0; i < reaperNum; i++) {
                                    SummonSpecificEnemy(enemyTemp, 0, 200, generator ? Mathf.Clamp(Mathf.Max(generator.width * tileWidth.x, generator.height * tileWidth.y) * 0.4f, 20f, 100f) : 20f);
                                }
                                for (int i = 0; i < enemySettings.enemy.Length; i++) {
                                    enemySettings.enemy[i].respawnable = false;
                                }
                                enemySettings.respawnable = false;
                            }
                            reaperProgress = 3;
                            if (CharacterManager.Instance) {
                                CharacterManager.Instance.sandstarRawActivated = 1;
                            }
                        }
                        break;
                }
            }
            if (enemySettings.enabled && enemySettings.respawnable) {
                RespawnEnemies();
            }
        }
        if (goalSettings.defeatEnemyCondition > 0 && !defeatMissionCompleted && !SceneChange.Instance.GetIsProcessing && !PauseController.Instance.pauseGame) {
            bool defeatMissionTemp = GetDefeatMissionCompleted();
            if (defeatMissionTemp) {
                defeatMissionCompleted = true;
                defeatMissionShowTimer = -1f;
                Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.defeatMissionCompleteSFX]);
                if (MessageUI.Instance) {
                    MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_DEFEATCONDITION_2"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Mission);
                }
            } else if (defeatMissionShowTimer > 0f) {
                defeatMissionShowTimer -= deltaTimeCache;
                if (defeatMissionShowTimer <= 0f) {
                    Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.defeatMissionStartSFX]);
                    if (MessageUI.Instance) {
                        MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_DEFEATCONDITION_0") + StageManager.Instance.dungeonController.goalSettings.defeatEnemyCondition + TextManager.Get("MESSAGE_DEFEATCONDITION_1"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Mission);
                    }
                }
            }
        }
        if (existPlayerHeightWall && tileSettings.enabled && tileSettings.container && CharacterManager.Instance.playerTrans) {
            bool toActive = (CharacterManager.Instance.playerTrans.position.y < tileSettings.container.position.y + playerHeightWallCondition);
            if (reserveUpdatePlayerHeightWall || isActivePlayerHeightWall != toActive) {
                reserveUpdatePlayerHeightWall = false;
                isActivePlayerHeightWall = toActive;
                for (int x = 0; x < generator.width; x++) {
                    for (int y = 0; y < generator.height; y++) {
                        if (playerHeightWallCollider[x, y]) {
                            playerHeightWallCollider[x, y].enabled = toActive;
                        }
                    }
                }
            }
        }
        if (StageManager.Instance.IsMappingFloor && StageManager.Instance.mapActivateFlag == 0 && CharacterManager.Instance.playerTrans) {
            Vector3 playerPosition = CharacterManager.Instance.playerTrans.position;
            playerPointNow.x = Mathf.RoundToInt((playerPosition.x - offset.x) / tileWidth.x);
            playerPointNow.y = Mathf.RoundToInt((playerPosition.z - offset.y) / tileWidth.y);
            if (GameManager.Instance.save.config[GameManager.Save.configID_DisableAutoMapping] == 0 && (playerPointNow != playerPointSave || mapForceCheckFlag)) {
                mapCheckInterval = 0f;
                playerPointSave = playerPointNow;
                mapForceCheckFlag = false;

                if (MapDatabase.Instance) {
                    int px = playerPointNow.x;
                    int py = playerPointNow.y;
                    int width = generator.width;
                    int height = generator.height;
                    for (int i = 0; i < 7; i++) {
                        for (int j = 0; j < 7; j++) {
                            mapCheck[i, j] = false;
                            int x = px + i - 3;
                            int y = py + j - 3;
                            if (x >= 0 && x < width && y >= 0 && y < height && mapWalkables[x, y]) {
                                bool okFlag = false;
                                int index = j * 7 + i;
                                if (MapDatabase.Instance.routes[index].disabled) {
                                    okFlag = false;
                                } else if (MapDatabase.Instance.routes[index].way.Length > 0) {
                                    okFlag = false;
                                    for (int a = 0; a < MapDatabase.Instance.routes[index].way.Length; a++) {
                                        bool tempFlag = true;
                                        for (int b = 0; b < MapDatabase.Instance.routes[index].way[a].step.Length; b++) {
                                            int xTemp = px + MapDatabase.Instance.routes[index].way[a].step[b].x;
                                            int yTemp = py + MapDatabase.Instance.routes[index].way[a].step[b].y;
                                            if (!(xTemp >= 0 && xTemp < width && yTemp >= 0 && yTemp < height && mapWalkables[xTemp, yTemp])) {
                                                tempFlag = false;
                                                break;
                                            }
                                        }
                                        if (tempFlag) {
                                            okFlag = true;
                                            break;
                                        }
                                    }
                                } else {
                                    okFlag = true;
                                }
                                if (okFlag) {
                                    mapCheck[i, j] = true;
                                    if (!mapWalkables[x, y].chipRenderer.enabled) {
                                        mapWalkables[x, y].chipRenderer.enabled = true;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            mapCheckInterval -= deltaTimeCache;
            if (mapCheckInterval <= 0f) {
                mapCheckInterval = 0.2f;
                playerPosition.y = 0f;
                int additionalMapLength = additionalMapChips.Count;
                for (int i = 0; i < additionalMapLength; i++) {
                    if (additionalMapChips[i]) {
                        bool nowActive = additionalMapChips[i].chipRenderer.enabled;
                        if (additionalMapChips[i].isStatic) {
                            if (!nowActive) {
                                int xAdd = Mathf.RoundToInt((additionalMapChips[i].transform.position.x - offset.x) / tileWidth.x) - playerPointNow.x + 3;
                                int yAdd = Mathf.RoundToInt((additionalMapChips[i].transform.position.z - offset.y) / tileWidth.y) - playerPointNow.y + 3;
                                if (xAdd >= 0 && xAdd < 7 && yAdd >= 0 && yAdd < 7 && mapCheck[xAdd, yAdd]) {
                                    additionalMapChips[i].chipRenderer.enabled = true;
                                }
                            }
                        } else {
                            Vector3 chipPosition = additionalMapChips[i].transform.position;
                            chipPosition.y = 0f;
                            float condition = (nowActive ? 19.5f : 13.5f) + additionalMapChips[i].radius;
                            bool toActive = GameManager.Instance.save.config[GameManager.Save.configID_DisableAutoMapping] == 0 && (chipPosition - playerPosition).sqrMagnitude <= condition * condition;
                            if (nowActive != toActive) {
                                additionalMapChips[i].chipRenderer.enabled = toActive;
                                if (toActive) {
                                    additionalMapChips[i].temporaryFlag = false;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (PauseController.Instance && !PauseController.Instance.pauseGame) {
            if (disadvantageType != DisadvantageType.None) {
                float damageRate = 0f;
                switch (disadvantageType) {
                    case DisadvantageType.Water:
                        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.AllWild) == 0) {
                            damageRate = 5f;
                        }
                        break;
                    case DisadvantageType.Vacuum:
                        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.AllWild) == 0) {
                            damageRate = 10f;
                        }
                        break;
                    case DisadvantageType.SandstarRaw:
                        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.AllWild) == 0) {
                            damageRate = 10f;
                        }
                        break;
                    case DisadvantageType.Heat:
                        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Gravity) == 0) {
                            damageRate = 10f;
                        }
                        break;
                }
                if (damageRate > 0f) {
                    disadvantageDamage += deltaTimeCache * damageRate;
                    if (disadvantageDamage > 1f) {
                        int damageNum = (int)disadvantageDamage;
                        disadvantageDamage -= damageNum;
                        CharacterManager.Instance.DisadvantageDamage(damageNum);
                    }
                    if (!disadvantageMessageShowed && timeCount >= 1f && MessageUI.Instance && !SceneChange.Instance.GetIsProcessing && !PauseController.Instance.pauseGame && PauseController.Instance.GetBlackCurtainAmount <= 0f) {
                        disadvantageMessageShowed = true;
                        UISE.Instance.Play(UISE.SoundName.caution);
                        switch (disadvantageType) {
                            case DisadvantageType.Water:
                                MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_DISADV_WATER"), MessageUI.time_Infinity, MessageUI.panelType_Information, MessageUI.slotType_Disadvantage);
                                break;
                            case DisadvantageType.Vacuum:
                                MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_DISADV_VACUUM"), MessageUI.time_Infinity, MessageUI.panelType_Information, MessageUI.slotType_Disadvantage);
                                break;
                            case DisadvantageType.Heat:
                                MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_DISADV_HEAT"), MessageUI.time_Infinity, MessageUI.panelType_Information, MessageUI.slotType_Disadvantage);
                                break;
                            case DisadvantageType.SandstarRaw:
                                MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_DISADV_SANDSTARRAW"), MessageUI.time_Infinity, MessageUI.panelType_Information, MessageUI.slotType_Disadvantage);
                                break;
                        }
                        if (StageManager.Instance && StageManager.Instance.dungeonMother) {
                            StageManager.Instance.dungeonMother.disadvantageProtectMessageShowed[(int)disadvantageType] = false;
                        }
                    }
                } else {
                    if (disadvantageMessageShowed) {
                        disadvantageMessageShowed = false;
                        MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_Disadvantage);
                    }
                    if (timeCount >= 1f && StageManager.Instance && StageManager.Instance.dungeonMother && StageManager.Instance.dungeonMother.disadvantageProtectMessageShowed[(int)disadvantageType] == false && MessageUI.Instance && !SceneChange.Instance.GetIsProcessing && !PauseController.Instance.pauseGame && PauseController.Instance.GetBlackCurtainAmount <= 0f) {
                        StageManager.Instance.dungeonMother.disadvantageProtectMessageShowed[(int)disadvantageType] = true;
                        CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.protect, -1, true, true);
                        for (int i = 1; i < GameManager.friendsMax; i++) {
                            CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.protectNoSfx, i, true, true);
                        }
                        switch (disadvantageType) {
                            case DisadvantageType.Water:
                                MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_PROTECT_WATER"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Important);
                                break;
                            case DisadvantageType.Vacuum:
                                MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_PROTECT_VACUUM"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Important);
                                break;
                            case DisadvantageType.Heat:
                                MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_PROTECT_HEAT"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Lucky);
                                break;
                            case DisadvantageType.SandstarRaw:
                                MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_PROTECT_SANDSTARRAW"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Important);
                                break;
                        }
                    }
                }
            }
            bool heatHazeEnabled = (disadvantageType == DisadvantageType.Heat && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Gravity) == 0);
            if (StageManager.Instance.heatHazeEffect && StageManager.Instance.heatHazeEffect.enabled != heatHazeEnabled) {
                StageManager.Instance.heatHazeEffect.enabled = heatHazeEnabled;
            }
        }
        if (australopithecusShoutInterval > 0f) {
            australopithecusShoutInterval -= deltaTimeCache;
        }
        updateInterval -= deltaTimeCache;
    }

    public bool GetDefeatMissionCompleted() {
        if (GameManager.Instance.save.difficulty >= GameManager.difficultyEN && CharacterManager.Instance.defeatEnemyCount < goalSettings.defeatEnemyCondition) {
            return false;
        }
        return true;
    }

    public string GetDefeatMissionConditionText() {
        return string.Format("{0} / {1}", CharacterManager.Instance.defeatEnemyCount, goalSettings.defeatEnemyCondition);
    }

    public void SetReaperEffect() {
        if (EffectDatabase.Instance && !reaperEffect) {
            reaperEffect = Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.sandstarRawCloud], trans.position, quaIden, trans);
        }
    }

    public void DestroyReaperEffect() {
        if (reaperEffect) {
            Destroy(reaperEffect);
            reaperEffect = null;
        }
    }

    public void DefeatReaper() {
        bgmNumber = -1;
        PlayBGM();
        DestroyReaperEffect();
    }

    public void Generate() {
        if (generator == null) {
            generator = GetComponent<DungeonGenerator>();
            if (generator == null && DungeonDatabase.Instance != null) {
                if (generatorLevel >= 4 && GameManager.Instance.save.difficulty <= GameManager.difficultyNT) {
                    generatorLevel = 3;
                }
                generator = DungeonDatabase.Instance.GetGenerator(generatorLevel);
            }
        }
        if (GameManager.Instance.minmiBlack && EffectDatabase.Instance && isBossFloor && enemySettings.enabled) {
            if (GameManager.Instance.playerInput.GetButton(RewiredConsts.Action.Special) && GameManager.Instance.playerInput.GetButton(RewiredConsts.Action.Jump)) {
                Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.blackMinmiGlitch], transform);
                bossMinmiBlackFlag = true;
            }
        }
        if (SwitchWallFadeManager.Instance) {
            SwitchWallFadeManager.Instance.ClearList();
        }
        if (SwitchWallBackManager.Instance) {
            SwitchWallBackManager.Instance.ClearList();
        }
        if (itemReplaceLevel <= 0 && GameManager.Instance.IsPlayerAnother) {
            itemReplaceLevel = ItemDatabase.replaceRows;
        }
        generator.passageToWall = tileSettings.passageToWall;
        generator.Generate();
        map = generator.GetMap();
        mapType = new int[generator.width, generator.height];
        mapMod = new int[generator.width, generator.height];
        if (tileSettings.enabled) {
            tiles = new Transform[generator.width, generator.height];
            mapWalkables = new MapChipControl[generator.width, generator.height];
        }
        additionalMapChips = new List<MapChipControl>(256);
        isPlaced = new int[generator.width, generator.height];
        roomPoints = new List<Vector2Int>(generator.width * generator.height);
        walkablePoints = new List<Vector2Int>(generator.width * generator.height);
        offset = new Vector2(generator.width * -0.5f * tileWidth.x, generator.height * -0.5f * tileWidth.y);
        Vector2Int point = vec2IntZero;
        for (int x = 0; x < generator.width; x++) {
            point.x = x;
            for (int y = 0; y < generator.height; y++) {
                mapType[x, y] = map[x, y] / mapDenomi;
                mapMod[x, y] = map[x, y] % roomDenomi;
                if (mapType[x, y] == 1) {
                    point.y = y;
                    roomPoints.Add(point);
                    walkablePoints.Add(point);
                } else if (mapType[x, y] == 2) {
                    point.y = y;
                    walkablePoints.Add(point);
                }
            }
        }
        if (groundSettings.enabled) {
            GenerateGround();
        }
        if (tileSettings.enabled) {
            GenerateTiles();
        }
        int playerSpecifiedRoom = -200;
        int goalSpecifiedRoom = -200;
        if (generator.specifiedRooms.Length > 1) {
            int rand = Random.Range(0, generator.specifiedRooms.Length);
            playerSpecifiedRoom = generator.specifiedRooms[rand];
            goalSpecifiedRoom = generator.specifiedRooms[(rand + 1) % generator.specifiedRooms.Length];
        }
        int startRoomNum = PlacePlayerObject(playerSpecifiedRoom + 100) / roomDenomi;
        if (goalSettings.enabled) {
            GenerateGoal(startRoomNum, goalSpecifiedRoom + 100);
        }
        int shortcutIndex = StageManager.Instance.GetShortcutFloorIndex();
        if (shortcutIndex >= 0) {
            GenerateShortcutStart(startRoomNum, shortcutIndex, goalSpecifiedRoom + 100);
        }
        itemSettings.container = SetContainer("ItemContainer");
        if (itemSettings.enabled) {
            GenerateItems();
        }
        if (treeSettings.enabled) {
            GenerateTrees();
        }
        if (trapSettings.enabled) {
            GenerateTraps();
        }
        if (chestSettings.enabled) {
            GenerateChests();
            if (StageManager.Instance && StageManager.Instance.IsRandomStage == false && !GameManager.Instance.IsPlayerAnother) {
                GenerateGolds();
            }
        }
        //respawns = GameObject.FindGameObjectsWithTag("Respawn");
        if (enemySettings.enabled) {
            if (StageManager.Instance) {
                StageManager.Instance.ReserveObjectPool();
            }
            GenerateEnemies();
        }
        if (CameraManager.Instance) {
            CameraManager.Instance.flexibleClippingNear = cameraClippingNear;
            CameraManager.Instance.SetClippingFar(isInfinityFar ? 100000 : 5000);
        }
        if (CharacterManager.Instance) {
            if (specialMapCameraEnabled) {
                CharacterManager.Instance.SetMapCamera(mapCameraPosition, mapCameraRotation, mapCameraSize);
            } else {
                CharacterManager.Instance.ResetMapCamera();
            }
        }
        if (slipDamageOverride > 0) {
            StageManager.Instance.slipDamageOverride = slipDamageOverride;
        }
    }

    public void PlayBGM() {
        BGM.Instance.Play(bgmNumber);
        Ambient.Instance.Play(ambientNumber);
        SetDefaultSnapshot(0f);
    }

    public void SetDefaultSnapshot(float transitionTime = 0f) {
        GameManager.Instance.ChangeSnapshot(snapshotName, transitionTime);
    }

    bool FillSpawnPoint(Vector2Int pos, int layer, bool cannotMoreLay, bool isNotRespawnable) {
        if (pos.x >= 0 && pos.x < generator.width && pos.y >= 0 && pos.y < generator.height) {
            if (cannotMoreLay) {
                isPlaced[pos.x, pos.y] = 100;
            } else {
                isPlaced[pos.x, pos.y] = layer;
            }
            if (isNotRespawnable) {
                if (tiles[pos.x, pos.y] != null && tiles[pos.x, pos.y].CompareTag(tag_Respawn)) {
                    tiles[pos.x, pos.y].tag = tag_Untagged;
                }
            }
            return true;
        } else {
            return false;
        }
    }

    Vector2Int GetSpawnPoint(int layer, bool cannotMoreLay = false, bool isNotRespawnable = false, bool avoidWall = false, bool avoidPassage = false, bool avoidOtherItem = false, int avoidRoomNum = -1, float avoidPlayerDistance = 0f, bool alongWall = false, int specifiedRoomNum = -1) {
        bool check = false;
        bool abandon = false;
        Vector2Int p = vec2IntZero;
        int index = 0;
        int max = roomPoints.Count;
        ShuffleList(roomPoints);
        for (int i = 0; i < max * 2 && !check; i++) {
            if (i < max) {
                index = i;
                abandon = false;
            } else {
                index = i % max;
                abandon = true;
            }
            p = roomPoints[index];
            if (isPlaced[p.x, p.y] < layer) {
                check = true;
                if (!abandon) {
                    if (avoidEnable && (avoidWall || avoidPassage || avoidOtherItem || alongWall)) {
                        bool wallFlag = false;
                        bool passageFlag = false;
                        bool otherItemFlag = false;
                        for (int k = -1; k <= 1; k++) {
                            for (int l = -1; l <= 1; l++) {
                                if (p.x + k >= 0 && p.x + k < generator.width && p.y + l >= 0 && p.y + l < generator.height && !(k == 0 && l == 0)) {
                                    if (mapType[p.x + k, p.y + l] == 0) {
                                        wallFlag = true;
                                    } else if (mapType[p.x + k, p.y + l] == 2) {
                                        passageFlag = true;
                                    }
                                    if (isPlaced[p.x + k, p.y + l] >= layer) {
                                        otherItemFlag = true;
                                    }
                                }
                            }
                        }
                        if ((avoidWall && wallFlag) || (avoidPassage && passageFlag) || (avoidOtherItem && otherItemFlag) || (alongWall && !wallFlag)) {
                            check = false;
                        }
                    }
                    if (check && avoidRoomNum >= 0) {
                        if (map[p.x, p.y] / roomDenomi == avoidRoomNum) {
                            check = false;
                        }
                    }
                    if (check && specifiedRoomNum >= 0) {
                        if (map[p.x, p.y] / roomDenomi != specifiedRoomNum) {
                            check = false;
                        }
                    }
                    if (check && avoidPlayerDistance > 0 && pTrans) {
                        if ((tiles[p.x, p.y].position - pTrans.position).sqrMagnitude < avoidPlayerDistance * avoidPlayerDistance) {
                            check = false;
                        }
                    }
                }
            }
        }
        if (check) {
            if (cannotMoreLay) {
                isPlaced[p.x, p.y] = 100;
            } else {
                isPlaced[p.x, p.y] = layer;
            }
            if (isNotRespawnable) {
                if (tiles[p.x, p.y] != null && tiles[p.x, p.y].CompareTag(tag_Respawn)) {
                    tiles[p.x, p.y].tag = tag_Untagged;
                }
            }
        } else {
            p.x = -1;
            p.y = -1;
        }
        return p;
    }

    Transform SetContainer(string objName) {
        GameObject instance = new GameObject(objName) {
            isStatic = true
        };
        instance.transform.position = trans.position;
        instance.transform.SetParent(trans);
        return instance.transform;
    }

    public void ShuffleList(List<Vector2Int> list) {
        int n = list.Count;
        Vector2Int temp;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    public void DigMap(Vector2Int pos) {
        if (tileSettings.enabled && generator && pos.x >= 0 && pos.x < generator.width && pos.y >= 0 && pos.y < generator.height) {
            mapForceCheckFlag = true;
            Vector3 posAct = Vector3.zero;
            if (tileSettings.container) {
                posAct = tileSettings.container.position + new Vector3(pos.x * tileWidth.x + offset.x, 0, pos.y * tileWidth.y + offset.y);
            }
            map[pos.x, pos.y] = DungeonGenerator.passageBase;
            mapType[pos.x, pos.y] = 2;
            walkablePoints.Add(pos);
            generator.SetMapPoint(pos.x, pos.y, DungeonGenerator.passageBase);
            if (tiles[pos.x, pos.y].gameObject) {
                Destroy(tiles[pos.x, pos.y].gameObject);
            }
            if (tileSettings.passagePrefab) {
                tiles[pos.x, pos.y] = Instantiate(tileSettings.passagePrefab, posAct, quaIden, tileSettings.container ? tileSettings.container : null).transform;
                mapWalkables[pos.x, pos.y] = Instantiate(MapDatabase.Instance.prefab[MapDatabase.walkable], tiles[pos.x, pos.y]).GetComponent<MapChipControl>();
                if (StageManager.Instance.mapActivateFlag != 0) {
                    mapWalkables[pos.x, pos.y].chipRenderer.enabled = true;
                    if (StageManager.Instance.mapActivateFlag == 1) {
                        mapWalkables[pos.x, pos.y].temporaryFlag = true;
                    }
                }
                if (tileSettings.passageModify) {
                    WallControl wallConPassage = tiles[pos.x, pos.y].GetComponent<WallControl>();
                    if (wallConPassage) {
                        int wallFlag = generator.ResetWallModifyPoint(pos.x, pos.y);
                        if (wallFlag >= 0) {
                            wallConPassage.SetWall(wallFlag);
                            wallConPassage.mapPosition.x = pos.x;
                            wallConPassage.mapPosition.y = pos.y;
                        }
                    }
                }
            }
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    if (!(i == 0 && j == 0) && pos.x + i >= 0 && pos.x + i < generator.width && pos.y + j >= 0 && pos.y + j < generator.height) {
                        int wallFlag = generator.ResetWallModifyPoint(pos.x + i, pos.y + j);
                        if (wallFlag >= 0 && wallFlag < DungeonGenerator.roomBase) {
                            mapMod[pos.x + i, pos.y + j] = wallFlag;
                        }
                        if (wallFlag >= 0 && tiles[pos.x + i, pos.y + j]) {
                            WallControl wallCon = tiles[pos.x + i, pos.y + j].GetComponent<WallControl>();
                            if (wallCon) {
                                wallCon.DestroyChild();
                                wallCon.SetWall(wallFlag);
                            }
                        }
                    }
                }
            }
        }
    }

    public bool BreakWalls(bool isReplace = false) {
        bool existPP = tileSettings.passagePrefab;
        bool breaked = false;
        Vector2Int vec2IntTemp = vec2IntZero;
        if (isReplace && replacedFlag) {
            return false;
        }
        if (tileSettings.enabled) {
            mapForceCheckFlag = true;
            Vector3 playerPos = CharacterManager.Instance.playerTrans.position;
            if (tileSettings.container) {
                playerPos.y = tileSettings.container.position.y;
            }
            Transform breakParent = Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.empty], trans).transform;
            var wallList = new List<WallInfo>(generator.width * generator.height);
            for (int x = 0; x < generator.width; x++) {
                for (int y = 0; y < generator.height; y++) {
                    if (tiles[x, y] && tiles[x, y].gameObject.CompareTag(tag_Wall)) {
                        WallControl wallCon = tiles[x, y].GetComponent<WallControl>();
                        if (wallCon) {
                            wallList.Add(new WallInfo(x, y, (int)((playerPos - tiles[x, y].position).sqrMagnitude * (wallCon.wallFlag == 255 ? 4f : 1f))));
                        }
                    }
                }
            }
            wallList.Sort((a, b) => a.sqrDist - b.sqrDist);
            int listMax = Mathf.Min(wallList.Count, 100);
            for (int i = 0; i < listMax; i++) {
                WallControl wallCon = tiles[wallList[i].x, wallList[i].y].GetComponent<WallControl>();
                if (wallCon) {
                    wallCon.EmitBreakEffectParent(ref breakParent);
                }
            }
            if (goalSettings.enabled && goalSettings.goalInstance) {
                ActivateWithPlayerDistance activateWPD = goalSettings.goalInstance.GetComponentInChildren<ActivateWithPlayerDistance>();
                if (activateWPD) {
                    activateWPD.conditionDistance = 500;
                    activateWPD.conditionCameraHeight = 500;
                }
            }
            if (isReplace) {
                tileSettings.wallPrefab = StageManager.Instance.specialWallPrefab;
                tileSettings.wallModify = true;
                replacedFlag = true;
            }
            if (tileSettings.passageToWall) {
                generator.passageToWall = tileSettings.passageToWall = false;
                generator.ModifyWalls();
                map = generator.GetMap();
                for (int x = 0; x < generator.width; x++) {
                    for (int y = 0; y < generator.height; y++) {
                        mapMod[x, y] = map[x, y] % roomDenomi;
                    }
                }
            }
            for (int x = 0; x < generator.width; x++) {
                for (int y = 0; y < generator.height; y++) {
                    if (tiles[x, y] && tiles[x, y].gameObject.CompareTag(tag_Wall)) {
                        Vector3 pos = tiles[x, y].position;
                        Destroy(tiles[x, y].gameObject);
                        breaked = true;
                        if (isReplace) {
                            GameObject spWallObj = Instantiate(tileSettings.wallPrefab, pos, quaIden, tileSettings.container ? tileSettings.container : null);
                            tiles[x, y] = spWallObj.transform;
                            WallControl wallConSp = spWallObj.GetComponent<WallControl>();
                            if (wallConSp) {
                                wallConSp.SetWall(mapMod[x, y]);
                                wallConSp.mapPosition.x = x;
                                wallConSp.mapPosition.y = y;
                            }
                        } else if (existPP) {
                            map[x, y] = DungeonGenerator.passageBase;
                            generator.SetMapPoint(x, y, DungeonGenerator.passageBase);
                            if (tileSettings.passageModify) {
                                if (x >= 3 && x < generator.width - 3 && y >= 3 && y < generator.height - 3) {
                                    tiles[x, y] = Instantiate(tileSettings.roomPrefab, pos, quaIden, tileSettings.container ? tileSettings.container : null).transform;
                                    vec2IntTemp.x = x;
                                    vec2IntTemp.y = y;
                                    roomPoints.Add(vec2IntTemp);
                                } else {
                                    tiles[x, y] = Instantiate(tileSettings.passagePrefab, pos, quaIden, tileSettings.container ? tileSettings.container : null).transform;
                                    tiles[x, y].tag = tag_Respawn;
                                    vec2IntTemp.x = x;
                                    vec2IntTemp.y = y;
                                    roomPoints.Add(vec2IntTemp);
                                    WallControl wallConPassage = tiles[x, y].GetComponent<WallControl>();
                                    if (wallConPassage) {
                                        wallConPassage.SetWall(255);
                                        wallConPassage.mapPosition.x = x;
                                        wallConPassage.mapPosition.y = y;
                                    }
                                }
                            } else {
                                tiles[x, y] = Instantiate(tileSettings.passagePrefab, pos, quaIden, tileSettings.container ? tileSettings.container : null).transform;
                                tiles[x, y].tag = tag_Respawn;
                                vec2IntTemp.x = x;
                                vec2IntTemp.y = y;
                                roomPoints.Add(vec2IntTemp);
                            }
                            mapWalkables[x, y] = Instantiate(MapDatabase.Instance.prefab[MapDatabase.walkable], tiles[x, y]).GetComponent<MapChipControl>();
                            if (StageManager.Instance.mapActivateFlag != 0) {
                                mapWalkables[x, y].chipRenderer.enabled = true;
                                if (StageManager.Instance.mapActivateFlag == 1) {
                                    mapWalkables[x, y].temporaryFlag = true;
                                }
                            }
                        }
                    }
                }
            }
            GameObject[] breakableObj = GameObject.FindGameObjectsWithTag(tag_Breakable);
            for (int i = 0; i < breakableObj.Length; i++) {
                Destroy(breakableObj[i]);
                breaked = true;
            }
            GameObject[] itemObj = GameObject.FindGameObjectsWithTag(tag_Item);
            for (int i = 0; i < itemObj.Length; i++) {
                Vector3 itemPosition = itemObj[i].transform.position;
                if (itemPosition.y < 0f) {
                    Rigidbody itemRb = itemObj[i].GetComponent<Rigidbody>();
                    if (itemRb) {
                        itemRb.MovePosition(new Vector3(itemPosition.x, 0.5f, itemPosition.z));
                    }
                }
            }
            if (breaked && SwitchWallFadeManager.Instance) {
                SwitchWallFadeManager.Instance.ClearList();
            }
            if (breaked && SwitchWallBackManager.Instance) {
                SwitchWallBackManager.Instance.ClearList();
            }
        }
        if (!isReplace) {
            if (StageManager.Instance.graphCollapseNowFloor && StageManager.Instance.graphCollapseNowFloor.Collapse()) {
                breaked = true;
            }
        } else {
            if (StageManager.Instance.graphBuildNowFloor && StageManager.Instance.graphBuildNowFloor.Execute()) {
                breaked = true;
            }
        }
        if (isReplace && specialCeilEnabled && CharacterManager.Instance) {
            CharacterManager.Instance.SetPositionHeightMaxAll(3.5f);
            Instantiate(StageManager.Instance.specialCeilPrefab, transform);
        }
        if (breaked) {
            if (isReplace) {
                buildUsed = true;
            } else {
                collapseUsed = true;
            }
        }
        return breaked;
    }

    public Vector3 GetRespawnPos(string targetTag = "Respawn") {
        if (tileSettings.enabled) {
            return GetRespawnPosAvoidable();
        } else {
            return GetRespawnPosTag(targetTag);
        }
    }

    public Vector3 GetRespawnPosTag(string targetTag = "Respawn") {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);
        if (taggedObjects.Length > 0) {
            return taggedObjects[Random.Range(0, taggedObjects.Length)].transform.position;
        }
        return Vector3.zero;
    }

    public Vector3 GetRespawnPosClosest(Vector3 pivotPos) {
        float sqrDistMin = float.MaxValue;
        if (tileSettings.enabled) {
            int max = roomPoints.Count;
            Vector2Int pMin = Vector2Int.zero;
            for (int i = 0; i < max; i++) {
                Vector2Int p = roomPoints[i];
                if (tiles[p.x, p.y] && tiles[p.x, p.y].CompareTag(tag_Respawn)) {
                    float sqrDistTemp = (tiles[p.x, p.y].position - pivotPos).sqrMagnitude;
                    if (sqrDistTemp < sqrDistMin) {
                        sqrDistMin = sqrDistTemp;
                        pMin = p;
                    }
                }
            }
            return tiles[pMin.x, pMin.y].position;
        } else {
            return pivotPos;
        }
    }

    public Vector3 GetRespawnPosAvoidable(bool avoidOthers = false, float avoidDistance = 1f, string avoidTag = "", float avoidPlayerDistance = 0f) {
        int max = roomPoints.Count;
        int index = Random.Range(0, max);
        ShuffleList(roomPoints);
        GameObject[] taggedObjects = new GameObject[0];
        if (avoidTag != "") {
            taggedObjects = GameObject.FindGameObjectsWithTag(avoidTag);
        }
        float avoidDistanceSqr = avoidDistance * avoidDistance;
        float avoidPlayerDistanceSqr = avoidPlayerDistance * avoidPlayerDistance;
        for (int i = 0; i < max; i++) {
            Vector2Int p = roomPoints[i];
            bool hitCheck = false;
            if (!tiles[p.x, p.y].CompareTag(tag_Respawn)) {
                hitCheck = true;
            }
            if (avoidOthers) {
                for (int j = 0; j < taggedObjects.Length && !hitCheck; j++) {
                    if ((tiles[p.x, p.y].position - taggedObjects[j].transform.position).sqrMagnitude < avoidDistanceSqr) {
                        hitCheck = true;
                    }
                }
            }
            if (!hitCheck && avoidPlayerDistance > 0f && pTrans) {
                if ((tiles[p.x, p.y].position - pTrans.position).sqrMagnitude < avoidPlayerDistanceSqr) {
                    hitCheck = true;
                }
            }
            if (!hitCheck) {
                index = i;
                break;
            }
        }
        return tiles[roomPoints[index].x, roomPoints[index].y].position;
    }

    public Vector3 GetWalkablePosAroundPlayer(float maxRadius, float minRadius = 0f) {
        int max = walkablePoints.Count;
        if (max > 0) {
            int index = Random.Range(0, max);
            ShuffleList(walkablePoints);
            if (pTrans) {
                Vector3 playerPosTemp = pTrans.position;
                Vector2Int p;
                bool hitCheck;
                for (int i = 0; i < max; i++) {
                    p = walkablePoints[i];
                    hitCheck = false;
                    if (tiles[p.x, p.y].CompareTag(tag_Respawn) == false && tiles[p.x, p.y].CompareTag(tag_Passage) == false) {
                        hitCheck = true;
                    }
                    if (!hitCheck) {
                        playerPosTemp.y = tiles[p.x, p.y].position.y;
                        float sqrDist = (tiles[p.x, p.y].position - playerPosTemp).sqrMagnitude;
                        if (sqrDist > maxRadius * maxRadius) {
                            hitCheck = true;
                        } else if (minRadius > 0f && sqrDist < minRadius * minRadius) {
                            hitCheck = true;
                        }
                    }
                    if (!hitCheck) {
                        index = i;
                        break;
                    }
                }
            }
            return tiles[walkablePoints[index].x, walkablePoints[index].y].position;
        } else {
            GameObject[] respawnObjs = GameObject.FindGameObjectsWithTag(tag_Respawn);
            if (respawnObjs.Length > 0) {
                return respawnObjs[Random.Range(0, respawnObjs.Length)].transform.position;
            }
        }
        return pTrans.position;
    }

    public Vector3 GetRespawnPosRoom(int roomNumber) {
        int index = Random.Range(0, roomPoints.Count);
        ShuffleList(roomPoints);
        int max = roomPoints.Count;
        Vector2Int p;
        bool hitCheck;
        for (int i = 0; i < max; i++) {
            p = roomPoints[i];
            hitCheck = false;
            if (tiles[p.x, p.y].CompareTag(tag_Respawn)) {
                hitCheck = true;
            }
            if (!hitCheck && map[p.x, p.y] != roomNumber) {
                hitCheck = true;
            }
            if (!hitCheck) {
                index = i;
                break;
            }
        }
        return tiles[roomPoints[index].x, roomPoints[index].y].position;
    }

    bool IsWallInside(int x, int y, int thickness = 1) {
        for (int i = -thickness; i <= thickness; i++) {
            for (int j = -thickness; j <= thickness; j++) {
                if ((i != 0 || j != 0) && x + i >= 0 && x + i < generator.width && y + j >= 0 && y + j < generator.height && mapType[x + i, y + j] >= 1) {
                    return false;
                }
            }
        }
        return true;
    }

    void GenerateTiles() {
        tileSettings.container = SetContainer("TileContainer");
        GameObject tileObj;
        WallControl wallCon;
        Vector3 pivot = tileSettings.container.position;
        Vector3 temp = vecZero;
        Vector3 pos;
        Vector2Int point = vec2IntZero;
        for (int x = 0; x < generator.width; x++) {
            temp.x = x * tileWidth.x + offset.x;
            for (int y = 0; y < generator.height; y++) {
                int tileType = mapType[x, y];
                int tileMod = mapMod[x, y];
                temp.z = y * tileWidth.y + offset.y;
                pos = pivot + temp;
                if (tileType == 1) {
                    tileObj = Instantiate(tileSettings.roomPrefab, pos, quaIden, tileSettings.container);
                    tileObj.tag = tag_Respawn;
                    if (tileSettings.roomModify) {
                        wallCon = tileObj.GetComponent<WallControl>();
                        if (wallCon) {
                            wallCon.SetWall(tileMod);
                            wallCon.mapPosition.x = x;
                            wallCon.mapPosition.y = y;
                        }
                    }
                } else if (tileType == 2) {
                    tileObj = Instantiate(tileSettings.passagePrefab, pos, quaIden, tileSettings.container);
                    if (tileSettings.passageModify) {
                        wallCon = tileObj.GetComponent<WallControl>();
                        if (wallCon) {
                            wallCon.SetWall(tileMod);
                            wallCon.mapPosition.x = x;
                            wallCon.mapPosition.y = y;
                        }
                    }
                } else {
                    tileObj = Instantiate(tileSettings.wallPrefab, pos, quaIden, tileSettings.container);
                    if (tileSettings.wallModify) {
                        wallCon = tileObj.GetComponent<WallControl>();
                        if (wallCon) {
                            wallCon.SetWall(tileMod);
                            wallCon.mapPosition.x = x;
                            wallCon.mapPosition.y = y;
                        }
                    }
                }
                tiles[x, y] = tileObj.transform;
                if (mapEnable && tileType >= 1) {
                    mapWalkables[x, y] = Instantiate(MapDatabase.Instance.prefab[MapDatabase.walkable], tiles[x, y]).GetComponent<MapChipControl>();
                }
            }
        }
        if (tileSettings.wallModify) {
            Vector2 endPoint;
            temp.y = 0f;
            for (int i = 0; i < 4; i++) {
                endPoint.x = (i == 1 ? -1 : i == 2 ? generator.width : (generator.width - 1) * 0.5f);
                endPoint.y = (i == 0 ? -1 : i == 3 ? generator.height : (generator.height - 1) * 0.5f);
                temp.x = endPoint.x * tileWidth.x + offset.x;
                temp.z = endPoint.y * tileWidth.y + offset.y;
                tileObj = Instantiate(tileSettings.wallPrefab, pivot + temp, quaIden, tileSettings.container);
                wallCon = tileObj.GetComponent<WallControl>();
                if (wallCon) {
                    wallCon.SetEnd(i * 2 + 2);
                    wallCon.mapPosition.x = -1;
                    wallCon.mapPosition.y = -1;
                }
            }
        }
        if (tileSettings.pillarPrefab) {
            temp.y = 0f;
            for (int i = 0; i <= generator.width; i++) {
                temp.x = i * tileWidth.x + offset.x - tileWidth.x * 0.5f;
                for (int j = 0; j <= generator.height; j++) {
                    temp.z = j * tileWidth.y + offset.y - tileWidth.y * 0.5f;
                    int wallCount = 0;
                    for (int k = -1; k < 1; k++) {
                        for (int l = -1; l < 1; l++) {
                            if (i + k >= 0 && i + k < generator.width && j + l >= 0 && j + l < generator.height) {
                                if (mapType[i + k, j + l] == 0) {
                                    wallCount++;
                                }
                            } else {
                                // wallCount++;
                            }
                        }
                    }
                    if (wallCount >= 1 && wallCount <= 3) {
                        Instantiate(tileSettings.pillarPrefab, pivot + temp, quaIden, tileSettings.container);
                    }
                }
            }
        }
        if (tileSettings.endThickness > 0) {
            temp.y = tileSettings.endHeight;
            for (int i = 0 - tileSettings.endThickness; i < generator.width + tileSettings.endThickness; i++) {
                temp.x = i * tileWidth.x + offset.x;
                for (int j = 0 - tileSettings.endThickness; j < generator.height + tileSettings.endThickness; j++) {
                    if (!(i >= 0 && i < generator.width && j >= 0 && j < generator.height) && Random.Range(0, 100) < tileSettings.endInstantiateProbability) {
                        temp.z = j * tileWidth.y + offset.y;
                        int prefabType = 0;
                        bool isOutside = (i >= -1 && i <= generator.width && j >= -1 && j <= generator.height);
                        if (tileSettings.endOutsidePrefab && isOutside) {
                            prefabType = 1;
                        } else if (tileSettings.endInsidePrefab && !isOutside) {
                            prefabType = 2;
                        }
                        if (prefabType >= 1) {
                            Instantiate(prefabType == 1 ? tileSettings.endOutsidePrefab : tileSettings.endInsidePrefab, pivot + temp, quaIden, tileSettings.container);
                        }
                    }
                }
            }
        }
    }

    int PlacePlayerObject(int specifiedRoom = -1) {
        if (CharacterManager.Instance != null) {
            CharacterManager.Instance.StopFriends();
            pObj = CharacterManager.Instance.playerObj;
            pTrans = CharacterManager.Instance.playerTrans;
            if (pObj) {
                pObj.SetActive(false);
                Vector2Int point = vec2IntZero;
                Vector3 rot = vecZero;
                if (fixSettings.enabled) {
                    if (!fixSettings.isWorldPos) {
                        point.x = (int)fixSettings.player.position.x;
                        point.y = (int)fixSettings.player.position.y;
                        isPlaced[point.x, point.y] = 100;
                    }
                    rot = fixSettings.player.rotation;
                } else {
                    point = GetSpawnPoint(layerPlayer, true, false, true, true, true, -1, 0, false, specifiedRoom);
                }
                if (point.x >= 0) {
                    Vector3 pos;
                    if (fixSettings.enabled && fixSettings.isWorldPos) {
                        pos = fixSettings.player.position;
                    } else {
                        pos = tiles[point.x, point.y].position;
                    }
                    pTrans.position = pos;
                    pTrans.eulerAngles = rot;
                    pObj.SetActive(true);
                    PlaceFriendsObject();
                    if (fixSettings.enabled && fixSettings.isWorldPos) {
                        return -1;
                    } else {
                        return map[point.x, point.y];
                    }
                }
            }
        }
        return -1;
    }

    void PlaceFriendsObject() {
        CharacterManager.Instance.CleanDeadFriends();
        CharacterManager.Instance.PlaceFriendsAroundPlayer();
    }

    public Vector3 GetPlayerPos() {
        return pTrans.position;
    }

    public GameObject GetGoalInstance() {
        return goalSettings.goalInstance;
    }

    void GenerateGoal(int avoidRoom = -1, int specifiedRoom = -1) {
        Vector2Int point = vec2IntZero;
        if (fixSettings.enabled) {
            if (!fixSettings.isWorldPos) {
                point = new Vector2Int((int)fixSettings.goalPos.x, (int)fixSettings.goalPos.y);
                isPlaced[point.x, point.y] = 100;
            }
        } else {
            bool avoidEnable = generator.goalAvoidEnable;
            point = GetSpawnPoint(layerGoal, true, true, avoidEnable, avoidEnable, avoidEnable, generator.GetRoomCount() > 1 ? avoidRoom : -1, specifiedRoom >= 0 ? 0 : Mathf.Max(tileWidth.x * generator.width / 3f, tileWidth.y * generator.height / 3f), false, specifiedRoom);
        }
        if (point.x >= 0) {
            if (goalSettings.goalInstance != null) {
                Destroy(goalSettings.goalInstance);
                goalSettings.goalInstance = null;
            }
            Vector3 pos;
            if (fixSettings.enabled && fixSettings.isWorldPos) {
                pos = fixSettings.goalPos;
            } else {
                pos = tiles[point.x, point.y].position;
            }
            goalSettings.goalInstance = Instantiate(goalSettings.goalPrefab, pos, quaIden, trans);
        }
    }

    void GenerateShortcutStart(int avoidRoom = -1, int index = 0, int specifiedRoom = -1) {
        Vector2Int point = vec2IntZero;
        if (fixSettings.enabled) {
            if (!fixSettings.isWorldPos) {
                point = new Vector2Int((int)fixSettings.shortcutPos.x, (int)fixSettings.shortcutPos.y);
                isPlaced[point.x, point.y] = 100;
            }
        } else {
            bool avoidEnable = generator.goalAvoidEnable;
            point = GetSpawnPoint(layerGoal, true, true, avoidEnable, avoidEnable, avoidEnable, generator.GetRoomCount() > 1 ? avoidRoom : -1, specifiedRoom >= 0 ? 0 : Mathf.Max(tileWidth.x * generator.width / 3f, tileWidth.y * generator.height / 3f), false, specifiedRoom);
        }
        if (point.x >= 0) {
            Vector3 pos;
            if (fixSettings.enabled && fixSettings.isWorldPos) {
                pos = fixSettings.shortcutPos;
            } else {
                pos = tiles[point.x, point.y].position;
            }
            shortcutInstance = Instantiate(StageManager.Instance.dungeonMother.shortcutSettings[index].shortcutStartPrefab, pos, quaIden, trans);
        }
    }

    void GenerateItems() {
        if (itemSettings.item.Length > 0 && ItemDatabase.Instance) {
            if (itemSettings.container == null) {
                itemSettings.container = SetContainer("ItemContainer");
            }
            Vector3 spawnPos = vecZero;
            Quaternion rot;
            for (int i = 0; i < itemSettings.item.Length; i++) {
                if (!itemSettings.item[i].prefab) {
                    itemSettings.item[i].prefab = ItemDatabase.Instance.GetItemPrefab(itemSettings.item[i].id);
                }
                if (itemSettings.item[i].prefab) {
                    rot = quaIden;
                    if (fixSettings.enabled && i < fixSettings.item.Length) {
                        if (fixSettings.isWorldPos) {
                            spawnPos = fixSettings.item[i].position;
                        } else {
                            spawnPos = tiles[(int)fixSettings.item[i].position.x, (int)fixSettings.item[i].position.y].position;
                            isPlaced[(int)fixSettings.item[i].position.x, (int)fixSettings.item[i].position.y] = 100;
                        }
                        rot = Quaternion.Euler(fixSettings.item[i].rotation);
                    } else {
                        Vector2Int point = GetSpawnPoint(layerItem, true, false, false, false, true, -1, 20);
                        if (point.x >= 0) {
                            spawnPos = tiles[point.x, point.y].position;
                        }
                    }
                    Instantiate(itemSettings.item[i].prefab, spawnPos, rot, itemSettings.container);
                }
            }
        }
    }

    void GenerateTrees() {
        if (treeSettings.container == null) {
            treeSettings.container = SetContainer("TreeContainer");
        }
        Vector3 randPos = vecZero;
        int roomPointsCount = roomPoints.Count;
        for (int i = 0; i < treeSettings.tree.Length; i++) {
            if (treeSettings.tree[i].treeFix.fixEnabled) {
                if (FillSpawnPoint(treeSettings.tree[i].treeFix.position, 4, treeSettings.tree[i].cannotMoreLay, treeSettings.tree[i].isNotRespawnable)) {
                    Instantiate(treeSettings.tree[i].prefab, tiles[treeSettings.tree[i].treeFix.position.x, treeSettings.tree[i].treeFix.position.y].position, quaIden, treeSettings.container);
                }
            } else {
                int generateNum = (int)(roomPointsCount * treeSettings.tree[i].density * 0.01f * Random.Range(0.9f, 1.1f));
                for (int j = 0; j < generateNum; j++) {
                    Vector2Int point = GetSpawnPoint(layerTree, treeSettings.tree[i].cannotMoreLay, treeSettings.tree[i].isNotRespawnable, treeSettings.tree[i].avoidWall, treeSettings.tree[i].avoidPassage, treeSettings.tree[i].avoidOtherTree, -1, 0, treeSettings.tree[i].alongWall);
                    if (point.x >= 0) {
                        if (treeSettings.tree[i].randomPosRadius > 0f) {
                            Vector2 randCircle = Random.insideUnitCircle;
                            randPos.x = randCircle.x;
                            randPos.z = randCircle.y;
                            Instantiate(treeSettings.tree[i].prefab, tiles[point.x, point.y].position + randPos * treeSettings.tree[i].randomPosRadius, quaIden, treeSettings.container);
                        } else {
                            Instantiate(treeSettings.tree[i].prefab, tiles[point.x, point.y].position, quaIden, treeSettings.container);
                        }
                    }
                }
            }
        }
    }

    void GenerateTraps() {
        trapSettings.container = SetContainer("TrapContainer");
        int generateCount = (int)(roomPoints.Count * (trapCountRate + trapSettings.trapCountRatePlus));
        Vector3 offset = vecZero;
        for (int i = 0; i < generateCount; i++) {
            Vector2Int point = GetSpawnPoint(layerTrap, false, false, false, true, false, -1, 5);
            if (point.x >= 0) {
                int mod = (point.x + point.y * 2) % trapHeightMod;
                offset.y = trapHeightBias * mod + trapSettings.heightOffset;
                Instantiate(trapSettings.prefab, tiles[point.x, point.y].position + offset, quaIden, trapSettings.container);
            }
        }
    }

    public void GenerateObjectOnWalkablePoint(GameObject prefab, float density, float randomPosRadius) {
        if (treeSettings.container == null) {
            treeSettings.container = SetContainer("TreeContainer");
        }
        Vector3 randPos = vecZero;
        int walkablePointsCount = walkablePoints.Count;
        int generateNum = Mathf.Clamp((int)(walkablePointsCount * density * 0.01f * Random.Range(0.9f, 1.1f)), 0, walkablePointsCount);
        ShuffleList(walkablePoints);
        for (int j = 0; j < generateNum; j++) {
            Vector2Int point = walkablePoints[j];
            if (point.x >= 0) { 
                if (randomPosRadius > 0f) {
                    Vector2 randCircle = Random.insideUnitCircle;
                    randPos.x = randCircle.x;
                    randPos.z = randCircle.y;
                    Instantiate(prefab, tiles[point.x, point.y].position + randPos * randomPosRadius, quaIden, treeSettings.container);
                } else {
                    Instantiate(prefab, tiles[point.x, point.y].position, quaIden, treeSettings.container);
                }
            }
        }
    }

    void GenerateGround() {
        groundSettings.container = SetContainer("GroundContainer");
        if (groundSettings.ground) {
            Instantiate(groundSettings.ground, vecZero, quaIden, groundSettings.container); ;
        }
        if (groundSettings.grass) {
            DungeonGenerator.RoomInfo roomInfo;
            GameObject grassInst;
            GrassControl grassCon;
            for (int i = 0; i < 100; i++) {
                roomInfo = generator.GetRoomInfo(i);
                if (roomInfo.size.x > 0) {
                    grassInst = Instantiate(groundSettings.grass, new Vector3((roomInfo.origin.x + roomInfo.size.x * 0.5f - 0.5f) * tileWidth.x + offset.x, 0, (roomInfo.origin.y + roomInfo.size.y * 0.5f - 0.5f) * tileWidth.y + offset.y), quaIden, groundSettings.container);
                    grassCon = grassInst.GetComponent<GrassControl>();
                    if (grassCon) {
                        grassCon.SetGrass(i, roomInfo.size.x * tileWidth.x, roomInfo.size.y * tileWidth.y, groundSettings.grassDensity);
                    }
                } else {
                    break;
                }
            }
        }
    }

    void GenerateChests() {
        if (chestSettings.container == null) {
            chestSettings.container = SetContainer("ChestContainer");
        }
        int num = Mathf.RoundToInt(Mathf.Sqrt(generator.width * generator.height) * Random.Range(1f, 4f / 3f) * chestCountRate * StageManager.Instance.GetContainerNumRate());
        num += chestSettings.chestCountPlus;
        if (num > 0) {
            for (int i = 0; i < num; i++) {
                Vector2Int point = GetSpawnPoint(layerChest, true, false, false, true);
                if (point.x >= 0) {
                    Instantiate(ItemDatabase.Instance.GetContainerPrefab(StageManager.Instance.GetContainerRank(chestSettings.chestRankPointPlus)), tiles[point.x, point.y].position, quaIden, chestSettings.container);
                }
            }
        }
    }

    public void GenerateGolds(float numRate = 1, bool randomize = true, float yOffsetMin = 0f, float yOffsetMax = 0f) {
        if (chestSettings.container == null) {
            chestSettings.container = SetContainer("ChestContainer");
        }
        int num = Mathf.RoundToInt(Mathf.Sqrt(generator.width * generator.height) * (randomize ? Random.Range(2f / 3f, 4f / 3f) : 1f) * numRate * 0.25f);
        Vector3 offset = vecZero;
        Vector2Int point;
        bool enableOffset = (yOffsetMin != 0f || yOffsetMax != 0f);
        if (num > 0) {
            for (int i = 0; i < num; i++) {
                point = GetSpawnPoint(layerGold, true);
                if (point.x >= 0) {
                    if (enableOffset) {
                        offset.y = Random.Range(yOffsetMin, yOffsetMax);
                    }
                    Instantiate(ItemDatabase.Instance.GetGoldPrefab(StageManager.Instance.GetGoldRank()), tiles[point.x, point.y].position + offset, quaIden, chestSettings.container);
                }
            }
        }
    }

    bool SpawnEnemy(bool isStartup = false, bool isSummon = false, int specifyRoomNumber = -1, float specifyRadius = 10f, bool summonedByBlackCrystal = false) {
        bool numCheck = false;
        int escapeCount = 0;
        int maxNumMul = 1;
        if (GetMinmiBlackEnabled()) {
            maxNumMul = minmiBlackRate;
        }
        bool levelUp = (CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.LevelUp) != 0 && enemySettings.respawnable);
        Vector3 spawnPos;
        Vector3 rot = vecZero;
        EnemyBase eBase;
        enemySettings.respawnedTime = GameManager.Instance.time;
        while (!numCheck && escapeCount < enemySettings.enemy.Length) {
            if (enemySettings.enemy[enemySettings.pointer].container && (isStartup || enemySettings.enemy[enemySettings.pointer].respawnable) && (isSummon || enemySettings.enemy[enemySettings.pointer].container.childCount < enemySettings.enemy[enemySettings.pointer].maxNum * maxNumMul)) {
                numCheck = true;
            } else {
                enemySettings.pointer = (enemySettings.pointer + 1) % enemySettings.enemy.Length;
                escapeCount++;
            }
        }
        if (numCheck) {
            if (!enemySettings.enemy[enemySettings.pointer].prefab) {
                enemySettings.enemy[enemySettings.pointer].prefab = CharacterDatabase.Instance.GetEnemy(enemySettings.enemy[enemySettings.pointer].id);
            }
            if (enemySettings.enemy[enemySettings.pointer].prefab) {
                if (fixSettings.enabled && enemySettings.pointer < fixSettings.enemy.Length && !isSummon) {
                    if (fixSettings.isWorldPos) {
                        spawnPos = fixSettings.enemy[enemySettings.pointer].position;
                    } else {
                        spawnPos = tiles[(int)fixSettings.enemy[enemySettings.pointer].position.x, (int)fixSettings.enemy[enemySettings.pointer].position.y].position;
                    }
                } else {
                    if (isSummon) {
                        if (specifyRoomNumber >= 0) {
                            spawnPos = GetRespawnPosRoom(specifyRoomNumber);
                        } else {
                            spawnPos = GetWalkablePosAroundPlayer(specifyRadius);
                        }
                    } else {
                        if (tileSettings.enabled) {
                            spawnPos = GetRespawnPosAvoidable(true, 1f, "Enemy", 20f);
                        } else {
                            spawnPos = GetRespawnPosTag();
                        }
                    }
                }
                eBase = Instantiate(enemySettings.enemy[enemySettings.pointer].prefab, spawnPos, quaIden, enemySettings.enemy[enemySettings.pointer].container).GetComponent<EnemyBase>();
                if (eBase != null) {
                    if (pTrans) {
                        Vector3 playerPos = pTrans.position;
                        if (isSummon || (isStartup && generator && generator.isMonsterHouse && MyMath.SqrMagnitudeIgnoreY(playerPos, eBase.transform.position) <= 50f * 50f && !Physics.Linecast(playerPos + vecUp, eBase.transform.position + vecUp, StageManager.Instance.roomCheckLayerMask, QueryTriggerInteraction.Collide))) {
                            eBase.LookAtIgnoreY(playerPos);
                            if (CharacterManager.Instance.pCon) {
                                eBase.RegisterTargetHate(CharacterManager.Instance.pCon, 10f);
                            }
                        } else {
                            if (fixSettings.enabled && enemySettings.pointer < fixSettings.enemy.Length) {
                                eBase.transform.eulerAngles = fixSettings.enemy[enemySettings.pointer].rotation;
                            } else {
                                Vector3 diff = eBase.transform.position - playerPos;
                                if (diff.sqrMagnitude < 25f * 25f && (diff.x != 0f || diff.z != 0f)) {
                                    diff.y = 0f;
                                    rot.y = Quaternion.LookRotation(diff).eulerAngles.y + Random.Range(-90f, 90f);
                                } else {
                                    rot.y = Random.Range(0f, 360f);
                                }
                                eBase.transform.Rotate(rot, Space.World);
                            }
                        }
                    }
                    int levelBias = 0;
                    if (!eBase.isBoss) {
                        if (levelUp && enemySettings.respawnable && enemySettings.enemy[enemySettings.pointer].respawnable && !enemySettings.levelBiasDisabled) {
                            if (enemyCountForBias % 15 == 0) {
                                levelBias += 1;
                            }
                            enemyCountForBias++;
                        }
                        if (GameManager.Instance.minmiRed) {
                            levelBias += 1;
                        }
                    }
                    eBase.enemyID = enemySettings.enemy[enemySettings.pointer].id;
                    eBase.summonedByBlackCrystal = summonedByBlackCrystal;
                    eBase.SetLevel(enemySettings.enemy[enemySettings.pointer].level + levelBias, false, false, variableLevel <= 0 ? 0 : Mathf.Clamp(Mathf.Min(variableLevel + levelBias * 10, variableLevel * (levelBias + 1)), 1, CharacterDatabase.Instance.variableStatus.Length));
                }
                if (!fixEnemyPointer) {
                    enemySettings.pointer = (enemySettings.pointer + 1) % enemySettings.enemy.Length;
                }
                return true;
            }
        }
        return false;
    }

    public void PlusEnemyPointer(int plusNum) {
        enemySettings.pointer = (enemySettings.pointer + enemySettings.enemy.Length + plusNum) % enemySettings.enemy.Length;
    }

    public void SetActiveEnemies(bool flag, int index = -1, bool lookAtPlayer = false) {
        if (index < 0) {
            for (int i = 0; i < enemySettings.enemy.Length; i++) {
                enemySettings.enemy[i].container.gameObject.SetActive(flag);
            }
        } else if (index < enemySettings.enemy.Length) {
            enemySettings.enemy[index].container.gameObject.SetActive(flag);
        }
    }

    public void SetEnemiesLockonPlayer(int index = -1) {
        if (CharacterManager.Instance.pCon) {
            if (index < 0) {
                for (int i = 0; i < enemySettings.enemy.Length; i++) {
                    for (int j = 0; j < enemySettings.enemy[i].container.childCount; j++) {
                        EnemyBase enemyTemp = enemySettings.enemy[i].container.GetChild(j).GetComponent<EnemyBase>();
                        if (enemyTemp) {
                            enemyTemp.ClearTargetHate();
                            enemyTemp.RegisterTargetHate(CharacterManager.Instance.pCon, 10f);
                        }
                    }
                }
            } else if (index < enemySettings.enemy.Length) {
                for (int j = 0; j < enemySettings.enemy[index].container.childCount; j++) {
                    EnemyBase enemyTemp = enemySettings.enemy[index].container.GetChild(j).GetComponent<EnemyBase>();
                    if (enemyTemp) {
                        enemyTemp.ClearTargetHate();
                        enemyTemp.RegisterTargetHate(CharacterManager.Instance.pCon, 10f);
                    }
                }
            }
        }
    }

    public void SpawnEnemyPos(Vector3 pos, float radius = 0.5f, int num = 1) {
        if (GetMinmiBlackEnabled()) {
            num *= minmiBlackRate;
        }
        for (int i = 0; i < num; i++) {
            Vector2 circle = Random.insideUnitCircle * radius;
            Vector3 posTemp = pos;
            posTemp.x += circle.x;
            posTemp.z += circle.y;
            if (!enemySettings.enemy[enemySettings.pointer].prefab) {
                enemySettings.enemy[enemySettings.pointer].prefab = CharacterDatabase.Instance.GetEnemy(enemySettings.enemy[enemySettings.pointer].id);
            }
            if (enemySettings.enemy[enemySettings.pointer].prefab) {
                EnemyBase eBase = Instantiate(enemySettings.enemy[enemySettings.pointer].prefab, posTemp, quaIden, enemySettings.enemy[enemySettings.pointer].container).GetComponent<EnemyBase>();
                if (eBase != null) {
                    int levelBias = 0;
                    if (!eBase.isBoss) {
                        if (CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.LevelUp) != 0 && enemySettings.respawnable && enemySettings.enemy[enemySettings.pointer].respawnable && !enemySettings.levelBiasDisabled) {
                            if (enemyCountForBias % 15 == 0) {
                                levelBias += 1;
                            }
                            enemyCountForBias++;
                        }
                        if (GameManager.Instance.minmiRed) {
                            levelBias += 1;
                        }
                        eBase.transform.LookAt(pTrans);
                        eBase.enemyID = enemySettings.enemy[enemySettings.pointer].id;
                        eBase.SetLevel(enemySettings.enemy[enemySettings.pointer].level + levelBias, false, false, variableLevel <= 0 ? 0 : Mathf.Clamp(variableLevel + levelBias * 10, 1, CharacterDatabase.Instance.variableStatus.Length));
                    }
                }
                enemySettings.pointer = (enemySettings.pointer + 1) % enemySettings.enemy.Length;
            }
        }
    }

    void GenerateEnemies() {
        enemySettings.pointer = 0;
        if (generator.level >= 0 && generator.level < numEnemiesArray.Length) {
            enemySettings.maxNum = numEnemiesArray[generator.level];
        } else {
            enemySettings.maxNum = numEnemiesArray[0];
        }
        if (enemySettings.maxNumBias != 0) {
            enemySettings.maxNum = enemySettings.maxNum * (100 + enemySettings.maxNumBias) / 100;
        }
        int childMaxNumSum = 0;
        for (int i = 0; i < enemySettings.enemy.Length; i++) {
            childMaxNumSum += enemySettings.enemy[i].maxNum;
        }
        if (enemySettings.maxNum > childMaxNumSum) {
            enemySettings.maxNum = childMaxNumSum;
        }
        for (int i = 0; i < enemySettings.enemy.Length; i++) {
            enemySettings.enemy[i].container = SetContainer("EnemyContainer" + i.ToString());
        }
        int maxNumTemp = enemySettings.maxNum;
        if (GetMinmiBlackEnabled()) {
            maxNumTemp *= minmiBlackRate;
        }
        for (int i = 0; i < maxNumTemp; i++) {
            if (!SpawnEnemy(true)) {
                break;
            }
        }
        if (enemySettings.notActiveDefault) {
            SetActiveEnemies(false);
        }
    }

    void RespawnEnemies() {
        int enemyCount = EnemyCount();
        float respawnIntervalTemp = 0f;
        int maxNumTemp = enemySettings.maxNum;
        if (GetMinmiBlackEnabled()) {
            maxNumTemp *= minmiBlackRate;
        }
        if (enemyCount > 0 && maxNumTemp > 0) {
            respawnIntervalTemp = Mathf.Lerp(respawnInterval.x, respawnInterval.y, Mathf.Clamp01(enemyCount * 2f / maxNumTemp));
            if (GetMinmiBlackEnabled()) {
                respawnIntervalTemp /= minmiBlackRate;
            }
        }
        if (enemyCount < maxNumTemp) {
            if (GameManager.Instance.time - enemySettings.respawnedTime > respawnIntervalTemp) {
                SpawnEnemy();
            }
        } else {
            enemySettings.respawnedTime = GameManager.Instance.time;
        }
    }

    public int EnemyCount(bool askActive = false) {
        int enemyCount = 0;
        for (int i = 0; i < enemySettings.enemy.Length; i++) {
            if (enemySettings.enemy[i].container != null && (!askActive || enemySettings.enemy[i].container.gameObject.activeSelf)) {
                enemyCount += enemySettings.enemy[i].container.childCount;
            }
        }
        if (enemyContainerSP != null) {
            enemyCount += enemyContainerSP.childCount;
        }
        return enemyCount;
    }

    public int EnemyCountDistance(Vector3 centerPosition, float distance, bool askActive = false) {
        int enemyCount = 0;
        float sqrDist = distance * distance;
        for (int i = 0; i < enemySettings.enemy.Length; i++) {
            if (enemySettings.enemy[i].container != null && (!askActive || enemySettings.enemy[i].container.gameObject.activeSelf)) {
                int childCount = enemySettings.enemy[i].container.childCount;
                if (childCount > 0) {
                    for (int j = 0; j < childCount; j++) {
                        if ((enemySettings.enemy[i].container.GetChild(j).position - centerPosition).sqrMagnitude <= sqrDist) {
                            enemyCount++;
                        }
                    }
                }
            }
        }
        return enemyCount;
    }

    public int SetSandstarRawForBoss() {
        int answer = 0;
        for (int i = 0; i < enemySettings.enemy.Length; i++) {
            if (enemySettings.enemy[i].container != null) {
                int count = enemySettings.enemy[i].container.childCount;
                for (int j = 0; j < count; j++) {
                    EnemyBaseBoss ebb = enemySettings.enemy[i].container.GetChild(j).GetComponent<EnemyBaseBoss>();
                    if (ebb) {
                        ebb.SetSandstarRaw();
                        answer++;
                    }
                }
            }
        }
        if (answer >= 1) {
            sinWR = true;
        }
        return answer;
    }

    public void SummonEnemies(int num, int roomNumber = -1, float radius = 10f, bool summonedByBlackCrystal = false) {
        if (GetMinmiBlackEnabled()) {
            num *= minmiBlackRate;
        }
        for (int i = 0; i < num; i++) {
            if (SpawnEnemy(false, true, roomNumber, radius, summonedByBlackCrystal) && summonedByBlackCrystal) {
                blackCrystalCountRemain++;
            }
        }
    }

    public void ReportDefeatBlackCrystal() {
        if (blackCrystalCountRemain > 0) {
            blackCrystalCountRemain--;
            if (blackCrystalCountRemain == 0) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_BlackCrystal, true);
            }
        }
    }

    public EnemyBase SummonSpecificEnemy(int id, int level, Vector3 position) {
        EnemyBase eBase;
        GameObject ePrefab;
        ePrefab = CharacterDatabase.Instance.GetEnemy(id);
        if (ePrefab) {
            if (enemyContainerSP == null) {
                enemyContainerSP = SetContainer("EnemyContainerSP");
            }
            eBase = Instantiate(ePrefab, position, quaIden, enemyContainerSP).GetComponent<EnemyBase>();
            if (eBase != null) {
                if (pTrans) {
                    eBase.LookAtIgnoreY(pTrans.position);
                }
                eBase.enemyID = id;
                eBase.SetLevel(level, false, false, variableLevel);
                return eBase;
            }
        }
        return null;
    }

    public EnemyBase SummonSpecificEnemy(int id, int level, float maxRadius, float minRadius = 0f) {
        return SummonSpecificEnemy(id, level, GetWalkablePosAroundPlayer(maxRadius, minRadius));
    }

    public bool ClearTraps() {
        GameObject[] trapObjects = GameObject.FindGameObjectsWithTag("Trap");
        if (trapObjects.Length > 0) {
            int effectID = (int)EffectDatabase.id.clearTraps;
            for (int i = 0; i < trapObjects.Length; i++) {
                Instantiate(EffectDatabase.Instance.prefab[effectID], trapObjects[i].transform.position, quaIden);
                Destroy(trapObjects[i]);
            }
            return true;
        }
        return false;
    }

    bool GetMinmiBlackEnabled() {
        return bossMinmiBlackFlag || (!isBossFloor && GameManager.Instance.minmiBlack);
    }

    public Vector3 GetTilePosition(int x, int y) {
        if (x >= 0 && x < generator.width && y >= 0 && y < generator.height && tiles[x, y]) {
            return tiles[x, y].position;
        }
        return vecZero;
    }

    public void SetPlayerHeightWall(int x, int y, Collider wallCollider) {
        if (x >= 0 && x < generator.width && y >= 0 && y < generator.height) {
            if (!existPlayerHeightWall) {
                existPlayerHeightWall = true;
                playerHeightWallCollider = new Collider[generator.width, generator.height];
            }
            playerHeightWallCollider[x, y] = wallCollider;
            reserveUpdatePlayerHeightWall = true;
        }
    }

    public void SetAdditionalMapChip(MapChipControl mapChipControl) {
        nullCheckCount++;
        if (nullCheckCount >= 64) {
            nullCheckCount = 0;
            int listLength = additionalMapChips.Count;
            for (int i = listLength - 1; i >= 0; i--) {
                if (!additionalMapChips[i]) {
                    additionalMapChips.RemoveAt(i);
                }
            }
        }
        additionalMapChips.Add(mapChipControl);
        if (StageManager.Instance.mapActivateFlag != 0) {
            mapChipControl.chipRenderer.enabled = true;
        }
    }

    public void SetAllChipActivate(bool toActive, bool isTemporal) {
        if (tileSettings.enabled) {
            for (int i = 0; i < generator.width; i++) {
                for (int j = 0; j < generator.height; j++) {
                    if (mapWalkables[i, j] && mapWalkables[i, j].chipRenderer.enabled != toActive) {
                        mapWalkables[i, j].chipRenderer.enabled = toActive;
                        mapWalkables[i, j].temporaryFlag = isTemporal;
                    }
                }
            }
        }
        int chipLength = additionalMapChips.Count;
        for (int i = 0; i < chipLength; i++) {
            if (additionalMapChips[i] && additionalMapChips[i].chipRenderer.enabled != toActive) {
                additionalMapChips[i].chipRenderer.enabled = toActive;
                additionalMapChips[i].temporaryFlag = isTemporal;
            }
        }
    }

    public void DeactivateTemporalChip() {
        if (tileSettings.enabled) {
            for (int i = 0; i < generator.width; i++) {
                for (int j = 0; j < generator.height; j++) {
                    if (mapWalkables[i, j] && mapWalkables[i, j].temporaryFlag && mapWalkables[i, j].chipRenderer.enabled) {
                        mapWalkables[i, j].chipRenderer.enabled = false;
                        mapWalkables[i, j].temporaryFlag = false;
                    }
                }
            }
        }
        int chipLength = additionalMapChips.Count;
        for (int i = 0; i < chipLength; i++) {
            if (additionalMapChips[i] && additionalMapChips[i].temporaryFlag && additionalMapChips[i].chipRenderer.enabled) {
                additionalMapChips[i].chipRenderer.enabled = false;
                additionalMapChips[i].temporaryFlag = false;
            }
        }
    }

    private void OnDestroy() {
        if (GameManager.Instance && MessageUI.Instance && disadvantageMessageShowed) {
            disadvantageMessageShowed = false;
            MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_Disadvantage);
        }
    }

}
