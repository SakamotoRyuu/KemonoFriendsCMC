using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class StageManager : SingletonMonoBehaviour<StageManager> {

    [System.Serializable]
    public class StageSettings {
        public string resourcePath;
        public bool isSpecialStage;
        public int[] ifrID;
        public GameManager.SecretType[] secretType;
        public Vector2Int recommendedLevel;
    }

    [System.Serializable]
    public class SceneSetting {
        public bool isStandard;
        public string name;
    }

    [System.Serializable]
    public class RandomSettings {
        public int stageNumber;
        public int floorMax;
        public int[] strongestID;
        public int[] strongerID;
        public bool randomized;
    }

    public class FloorOverride {
        public int generatorLevel;
        public int[] enemyID;
        public int[] strongFlag;
        public FloorOverride(int generatorLevel, int[] enemyID, int[] strongFlag) {
            this.generatorLevel = generatorLevel;
            this.enemyID = enemyID;
            this.strongFlag = strongFlag;
        }
    }

    [System.Serializable]
    public class AdditiveNavMesh {
        public int agentTypeID;
        public GameObject prefab;
        public GameObject instance;
    }

    public StageSettings[] stageSettings;
    public SceneSetting[] sceneSettings;
    // public string[] sceneName;
    public RandomSettings randomSettings;
    public LayerMask roomCheckLayerMask;
    public int anotherReaperMusicNumber;

    private int sceneNumber = -1;
    private GameObject[] taggedObjects;
    public const int stageMax = 12;
    public const int homeStageId = 0;
    public const int specialStageId = 14;
    public const int xRayFloorNumberMin = 5;
    public const int xRayFloorNumberMax = 6;
    public const int infernoStageId = 15;
    public const int amusementParkFloorNumber = 12;
    public GameObject mapCamera;
    public GameObject specialWallPrefab;
    public GameObject specialCeilPrefab;
    public ImageEffectOnRender heatHazeEffect;
    public AdditiveNavMesh[] additiveNavMeshes;

    [System.NonSerialized]
    public int stageNumber;
    [System.NonSerialized]
    public int floorNumber;
    [System.NonSerialized]
    public int floorMax;
    [System.NonSerialized]
    public int bonusFloorNum;
    [System.NonSerialized]
    public DungeonMother dungeonMother;
    [System.NonSerialized]
    public DungeonController dungeonController;
    [System.NonSerialized]
    public int defeatBossSave;
    [System.NonSerialized]
    public bool isBossFloor;
    [System.NonSerialized]
    public bool dropExpDisabled;
    [System.NonSerialized]
    public int slipDamageOverride;
    [System.NonSerialized]
    public int homeFloorNumber = 0;
    [System.NonSerialized]
    public int homeFloorNumberSecond = 1;
    [System.NonSerialized]
    public int mapActivateFlag;
    [System.NonSerialized]
    public Event_GraphBuild graphBuildNowFloor;
    [System.NonSerialized]
    public Event_GraphCollapse graphCollapseNowFloor;

    static readonly int[] containerJudge = new int[] { 0, 9, 18, 28, 40 };
    static readonly int[] goldUnit = new int[] { 0, 5, 20, 100 };
    FloorOverride[] floorOverrides;
    bool currentSceneIsStandard;
    bool shortcutNotificationReserved;

    const int raccoonID = 129;
    const int fennecID = 130;
    const int lbID = 150;
    const int ifrBottom = 100;
    const int inventoryBottom = 151;
    const int randomFloorRepeat = 4;
    const int randomFloorSeparate = 8;
    const int randomEnemyIDRange = 40;
    const int randomEnemyTypesMax = 4;
    public const int normalHomeFloor = 0;
    public const int normalHomeFloorSecond = 1;
    public const int anotherHomeFloor = 9;
    public const int anotherHomeFloorSecond = 11;

    public bool IsHomeStage {
        get {
            return stageNumber == homeStageId;
        }
    }

    public bool IsHomeFloor {
        get {
            return stageNumber == homeStageId && floorNumber == homeFloorNumber;
        }
    }

    public bool IsRandomStage {
        get {
            return stageNumber == randomSettings.stageNumber;
        }
    }

    public bool IsLonesomeServalStage {
        get {
            return stageNumber == 13 || stageNumber == 15;
        }
    }

    public bool IsXRayFloor {
        get {
            return dungeonController && dungeonController.isXRayFloor;
        }
    }

    public bool IsAmusementParkFloor {
        get {
            return stageNumber == homeStageId && floorNumber == amusementParkFloorNumber;
        }
    }

    public int FloorNumberConsiderDenotative {
        get {
            if (floorNumber >= GameManager.Instance.GetDenotativeMaxFloor(stageNumber)) {
                return 0;
            }
            return floorNumber;
        }
    }

    public void ResetHomeFloorNumber(bool isAnother) {
        if (isAnother) {
            homeFloorNumber = anotherHomeFloor;
            homeFloorNumberSecond = anotherHomeFloorSecond;
        } else {
            homeFloorNumber = normalHomeFloor;
            homeFloorNumberSecond = normalHomeFloorSecond;
        }
    }
    
    void SetRandomFloorOverride() {
        floorOverrides = new FloorOverride[randomSettings.floorMax];
        int[,] shuffleID = new int[randomFloorRepeat, randomEnemyIDRange];
        int[,] shuffleGL = new int[randomFloorRepeat, randomFloorSeparate];
        for (int i = 0; i < randomFloorRepeat; i++) {
            for (int j = 0; j < randomEnemyIDRange; j++) {
                shuffleID[i, j] = j;
            }
        }
        for (int i = 0; i < randomFloorRepeat; i++) {
            for (int j = randomEnemyIDRange - 1; j > 0; j--) {
                int r = Random.Range(0, j + 1);
                int temp = shuffleID[i, j];
                shuffleID[i, j] = shuffleID[i, r];
                shuffleID[i, r] = temp;
            }
        }
        for (int i = 0; i < randomFloorRepeat; i++) {
            for (int j = 0; j < randomFloorSeparate; j++) {
                shuffleGL[i, j] = (j + 2) % 5;
            }
        }
        for (int i = 0; i < randomFloorRepeat; i++) {
            for (int j = randomFloorSeparate - 1; j > 0; j--) {
                int r = Random.Range(0, j + 1);
                int temp = shuffleGL[i, j];
                shuffleGL[i, j] = shuffleGL[i, r];
                shuffleGL[i, r] = temp;
            }
        }
        int idPointer = 0;
        for (int i = 0; i < floorOverrides.Length; i++) {
            int glTemp = shuffleGL[i / randomFloorSeparate % randomFloorRepeat, i % randomFloorSeparate];
            int[] enemyID = new int[randomEnemyTypesMax];
            int[] strongFlag = new int[randomEnemyTypesMax];
            for (int j = 0; j < randomEnemyTypesMax; j++) {
                enemyID[j] = shuffleID[Mathf.Clamp(i / randomFloorSeparate, 0, randomFloorRepeat - 1), idPointer % randomEnemyIDRange];
                idPointer++;
                for (int k = 0; k < randomSettings.strongestID.Length; k++) {
                    if (enemyID[j] == randomSettings.strongestID[k]) {
                        strongFlag[j] = 2;
                        break;
                    }
                }
                for (int k = 0; k < randomSettings.strongerID.Length; k++) {
                    if (enemyID[j] == randomSettings.strongerID[k]) {
                        strongFlag[j] = 1;
                    }
                }
            }
            floorOverrides[i] = new FloorOverride(glTemp, enemyID, strongFlag);
        }
        randomSettings.randomized = true;
    }

    public void MoveStage(int index, int floor = 0, int saveSlot = -1, bool sandstarMax = false) {
        DestroyDungeonMother();
        CharacterManager.Instance.ResetForMoveStage();
        if (index == homeStageId) {
            if (GameManager.Instance.save.stageReport.Length >= GameManager.stageReportMax) {
                for (int i = 0; i < GameManager.Instance.save.stageReport.Length; i++) {
                    GameManager.Instance.save.stageReport[i] = 0;
                }
            } else {
                GameManager.Instance.save.stageReport = new int[GameManager.stageReportMax];
            }
        }
        if (sandstarMax) {
            CharacterManager.Instance.AddSandstar(100, true);
        }
        if (GameoverTimer.Instance) {
            Destroy(GameoverTimer.Instance);
        }
        if (DungeonDatabase.Instance) {
            DungeonDatabase.Instance.ResetRecent();
        }
        if (index < stageSettings.Length) {
            stageNumber = index;
            if (stageNumber == homeStageId) {
                GameManager.Instance.save.moveByBus = 0;
            } else {
                defeatBossSave = 0;
            }
            if (IsRandomStage) {
                int tickCount = System.Environment.TickCount;
                if (GameManager.Instance.save.randomSeed == 0) {
                    GameManager.Instance.save.randomSeed = tickCount;
                }
                Random.InitState(GameManager.Instance.save.randomSeed);
                SetRandomFloorOverride();
                Random.InitState(tickCount);
            } else {
                GameManager.Instance.save.randomSeed = 0;
                randomSettings.randomized = false;
            }
            GameObject stagePrefab = Resources.Load(stageSettings[stageNumber].resourcePath, typeof(GameObject)) as GameObject;
            if (stagePrefab) {
                dungeonMother = Instantiate(stagePrefab).GetComponent<DungeonMother>();
                dungeonMother.MoveFloor(floor, saveSlot);
            }
        }
        mapActivateFlag = 0;
    }

    public int GetShortcutFloorIndex() {
        for (int i = 0; i < dungeonMother.shortcutSettings.Length; i++) {
            if (floorNumber == dungeonMother.shortcutSettings[i].startFloor && GameManager.Instance.save.reachedFloor[stageNumber] >= dungeonMother.shortcutSettings[i].endFloor) {
                return i;
            }
        }
        return -1;
    }

    public int GetShortcutEndIndex() {
        for (int i = 0; i < dungeonMother.shortcutSettings.Length; i++) {
            if (floorNumber == dungeonMother.shortcutSettings[i].shortcutFloor) {
                return i;
            }
        }
        return -1;
    }

    public void SetAllMapChipActivate(bool isTemporal) {
        if (isTemporal && mapActivateFlag < 2) {
            mapActivateFlag = 1;
        } else {
            mapActivateFlag = 2;
        }
        if (dungeonController) {
            dungeonController.SetAllChipActivate(true, isTemporal);
        }
    }

    public void DeactivateTemporalChip() {
        if (mapActivateFlag < 2) {
            mapActivateFlag = 0;
        }
        if (dungeonController) {
            dungeonController.DeactivateTemporalChip();
        }
    }

    public void SetAllMapChipDeactivate() {
        mapActivateFlag = 0;
        if (dungeonController) {
            dungeonController.SetAllChipActivate(false, false);
        }
    }

    public GameObject GetGoalInstance() {
        if (dungeonController && dungeonController.goalSettings.enabled && !dungeonController.isBossFloor) {
            return dungeonController.goalSettings.goalInstance;
        } else {
            return null;
        }
    }

    public GameObject GetShortcutInstance() {
        if (dungeonController && dungeonController.goalSettings.enabled && !dungeonController.isBossFloor) {
            return dungeonController.shortcutInstance;
        } else {
            return null;
        }
    }

    public bool ClearTraps() {
        if (dungeonController) {
            return dungeonController.ClearTraps();
        } else {
            return false;
        }
    }

    public int GetSlipDamage(bool isEnemy = false) {
        int answer = 0;
        if (isEnemy) {
            answer = (int)(GameManager.Instance.GetLevelStatusNow() * 40f);
        } else {
            int baseAmount;
            if (slipDamageOverride > 0) {
                baseAmount = slipDamageOverride;
            } else{
                baseAmount = stageNumber;
                baseAmount *= 2;
            }
            answer = Mathf.Max(Mathf.RoundToInt(baseAmount * CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.SlipDamage) * (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.ReduceSlipDamage) != 0 ? 0.75f : 1f)), 1);
        }
        return answer;
    }

    public float GetGraphGoldRank() {
        if (IsRandomStage) {
            return (10 + FloorNumberConsiderDenotative * 5 + 50) * 0.01f;
        }
        return (stageNumber * 10 + FloorNumberConsiderDenotative + 50) * 0.01f;
    }

    public int GetContainerRank(int pointBias = 0) {
        int pointMax = stageNumber * 3 + FloorNumberConsiderDenotative + 1 + pointBias;
        if (IsRandomStage) {
            pointMax = FloorNumberConsiderDenotative * 2 + 4 + pointBias;
        }
        int point = Random.Range(0, Mathf.Clamp(pointMax, 1, 80));
        for (int i = containerJudge.Length - 1; i >= 0; i--) {
            if (point >= containerJudge[i]) {
                return i;
            }
        }
        return -1;
    }

    public int GetContainerRankForSandCat() {
        if (dungeonController && dungeonController.isBossFloor) {
            return GetContainerRank(dungeonController.sinWR ? 40 : 10);
        } else {
            return GetContainerRank();
        }
    }

    public int GetContainerRankForRaccoon(bool sinWR) {
        return GetContainerRank(sinWR ? 40 : 10);
    }

    public float GetContainerNumRate() {
        int floorTemp = FloorNumberConsiderDenotative;
        if (floorTemp == bonusFloorNum) {
            return 1f + (0.5f / 3f);
        } else { 
            return 1f;
        }
    }

    public int GetReaperID() {
        if (dungeonMother) {
            return dungeonMother.reaperSettings.enemyID;
        }
        return -1;
    }

    public int GetReaperBGM() {
        if (dungeonMother) {
            if (dungeonController && dungeonController.bossMusicNumber >= 0) {
                return dungeonController.bossMusicNumber;
            }
            if (GameManager.Instance.IsPlayerAnother && dungeonMother.reaperSettings.appearMusicNumberAnother > 0) {
                return dungeonMother.reaperSettings.appearMusicNumberAnother;
            }
            return dungeonMother.reaperSettings.appearMusicNumber;
        }
        return -1;
    }

    public void DefeatReaper() {
        if (dungeonController) {
            dungeonController.DefeatReaper();
        }
    }

    public bool GetSandstarRawEnabled() {
        if (stageNumber == homeStageId && dungeonController && !dungeonController.enemySettings.enabled) {
            return false;
        } else if (CharacterManager.Instance && CharacterManager.Instance.sandstarRawActivated >= 1) {
            return false;
        } else if (dungeonController && dungeonController.isBossFloor && dungeonController.EnemyCount() <= 0) {
            return false;
        } else {
            return true;
        }
    }

    public void SetReaperEffect() {
        if (dungeonController) {
            dungeonController.SetReaperEffect();
        }
    }

    public void DestroyReaperEffect() {
        if (dungeonController) {
            dungeonController.DestroyReaperEffect();
        }
    }

    public void ActivateSandstarRaw() {
        int hitEbb = 1;
        if (dungeonController) {
            if (dungeonController.isBossFloor) {
                hitEbb = dungeonController.SetSandstarRawForBoss();
                if (hitEbb >= 1) {
                    SetReaperEffect();
                }
            } else {
                dungeonController.forceReaper = true;
            }
            GameManager.Instance.levelLimitAuto = dungeonController.playerLevelLimit = 100;
            GameManager.Instance.hpUpLimitAuto = GameManager.Instance.stUpLimitAuto = dungeonController.playerHpUpLimit = 100;
            GameManager.Instance.inventoryLimitAuto = dungeonController.playerInventoryLimit = 100;
            GameManager.Instance.armsStage = dungeonController.armsStageOverride = 0;
            if (CharacterManager.Instance) {
                CharacterManager.Instance.CheckLimitSet(true, true);
            }
        }
        if (CharacterManager.Instance) {
            CharacterManager.Instance.sandstarRawActivated = hitEbb;
        }
    }

    public int GetGoldRank() {
        const int basicGoldNum = 5;
        int answer = -1;
        int floorTemp = FloorNumberConsiderDenotative;
        int goldSum = 15 + stageNumber * 5 + floorTemp + 1;
        if (floorTemp == bonusFloorNum) {
            goldSum += stageNumber * 5 / 3;
        }
        if (IsRandomStage) {
            goldSum = 20 + floorTemp * 3;
        }
        int price = (int)(goldSum * Random.Range(0.8f, 1.2f) / basicGoldNum);
        if (price <= 0) {
            answer = 0;
        } else if (price > goldUnit[goldUnit.Length - 1]) {
            answer = goldUnit.Length - 1;
        } else {
            for (int i = 0; i < goldUnit.Length; i++) {
                if (price == goldUnit[i]) {
                    answer = i;
                    break;
                }
            }
        }
        if (answer < 0) {
            for (int i = 0; i < goldUnit.Length - 1; i++) {
                if (price > goldUnit[i] && price < goldUnit[i + 1]) {
                    if (Random.value > 1f / (goldUnit[i + 1] - goldUnit[i]) * (price - goldUnit[i])) {
                        answer = i;
                    } else {
                        answer = i + 1;
                    }
                    break;
                }
            }
        }
        if (answer < 1) {
            answer = 1;
        }
        return answer;
    }

    public void SetReachedFloor() {
        if (floorNumber > GameManager.Instance.save.reachedFloor[stageNumber] && GetShortcutEndIndex() < 0 && floorNumber < GameManager.Instance.GetDenotativeMaxFloor(stageNumber)) {
            GameManager.Instance.save.reachedFloor[stageNumber] = floorNumber;
            bool checkShortcut = false;
            for (int i = 0; i < dungeonMother.shortcutSettings.Length && !checkShortcut; i++) {
                if (floorNumber == dungeonMother.shortcutSettings[i].endFloor) {
                    checkShortcut = true;
                }
            }
            if (checkShortcut && SceneChange.Instance) {
                SceneChange.Instance.reserveShortcut = true;
            }
        }
    }

    public void ChangeScene(int sceneNumber) {
        if (this.sceneNumber != sceneNumber) {
            if (this.sceneNumber >= 0 && this.sceneNumber <= sceneSettings.Length) {
                SceneManager.UnloadSceneAsync(sceneSettings[this.sceneNumber].name);
            }
            if (sceneNumber >= 0 && sceneNumber <= sceneSettings.Length) {
                SceneManager.LoadScene(sceneSettings[sceneNumber].name, LoadSceneMode.Additive);
                currentSceneIsStandard = sceneSettings[sceneNumber].isStandard;
            }
            this.sceneNumber = sceneNumber;
        }
    }

    public void DestroyDungeonMother() {
        if (dungeonMother) {
            Destroy(dungeonMother.gameObject);
            dungeonMother = null;
        }
    }

    public void PlayBGM() {
        if (dungeonController) {
            dungeonController.PlayBGM();
        }
    }

    public void CleaningObjects() {
        DestroyTaggedObjects("Effect");
        DestroyTaggedObjects("Projectile");
        DestroyTaggedObjects("Enemy");
        DestroyTaggedObjects("Item");
        DestroyTaggedObjects("Money");
        DestroyTaggedObjects("Trap");
        CleanObjectPool();
    }

    public void DestroyDungeonController() {
        CleaningObjects();
        if (dungeonController) {
            Destroy(dungeonController.gameObject);
            dungeonController = null;
        }
        Resources.UnloadUnusedAssets();
    }

    public void SetAdditiveNavMesh(int agentTypeID) {
        if (currentSceneIsStandard && dungeonController) {
            for (int i = 0; i < additiveNavMeshes.Length; i++) {
                if (additiveNavMeshes[i].agentTypeID == agentTypeID) {
                    if (additiveNavMeshes[i].instance == null) {
                        additiveNavMeshes[i].instance = Instantiate(additiveNavMeshes[i].prefab, dungeonController.transform);
                    }
                    break;
                }
            }
        }
    }

    public void GenerateDungeon(GameObject dungeonPrefab, int floorNum) {
        dungeonController = Instantiate(dungeonPrefab, Vector3.zero, Quaternion.identity).GetComponent<DungeonController>();
        if (dungeonController) {
            if (GameManager.Instance.stageModFlag) {
                int stageFloorID = stageNumber * 100 + floorNum;
                int infoCount = GameManager.Instance.floorModInfo.Count;
                for (int i = 0; i < infoCount; i++) {
                    if (GameManager.Instance.floorModInfo[i].id == stageFloorID) {
                        if (GameManager.Instance.floorModInfo[i].music >= -1) {
                            dungeonController.bgmNumber = GameManager.Instance.floorModInfo[i].music;
                        }
                        if (GameManager.Instance.floorModInfo[i].bossMusic >= -1) {
                            dungeonController.bossMusicNumber = GameManager.Instance.floorModInfo[i].bossMusic;
                        }
                        if (GameManager.Instance.floorModInfo[i].ambient >= -1) {
                            dungeonController.ambientNumber = GameManager.Instance.floorModInfo[i].ambient;
                        }
                        if (GameManager.Instance.floorModInfo[i].sky >= 0) {
                            dungeonController.lightingNumber = GameManager.Instance.floorModInfo[i].sky;
                        }
                        break;
                    }
                }
            }
            if (IsRandomStage && randomSettings.randomized && floorNum < floorOverrides.Length) {
                dungeonController.generatorLevel = floorOverrides[floorNum].generatorLevel;
                for (int i = 0; i < dungeonController.enemySettings.enemy.Length && i < floorOverrides[floorNum].enemyID.Length; i++) {
                    dungeonController.enemySettings.enemy[i].id = floorOverrides[floorNum].enemyID[i];
                    if (floorOverrides[floorNum].strongFlag[i] == 2) {
                        dungeonController.enemySettings.enemy[i].respawnable = false;
                        dungeonController.enemySettings.enemy[i].maxNum = 1;
                    } else if (floorOverrides[floorNum].strongFlag[i] == 1) {
                        dungeonController.enemySettings.enemy[i].respawnable = true;
                        dungeonController.enemySettings.enemy[i].maxNum = 3;
                    } else {
                        dungeonController.enemySettings.enemy[i].respawnable = true;
                        dungeonController.enemySettings.enemy[i].maxNum = 20;
                    }
                    if (dungeonController.fixSettings.enabled) {
                        dungeonController.enemySettings.enemy[i].respawnable = false;
                    }
                }
            }
            dungeonController.Generate();
        }
    }

    public void SetActiveEnemies(bool flag, int index = -1) {
        if (dungeonController) {
            dungeonController.SetActiveEnemies(flag, index);
        }
    }

    public void SetEnemiesLockonPlayer(int index = -1) {
        if (dungeonController) {
            dungeonController.SetEnemiesLockonPlayer(index);
        }
    }

    public void DestroyTaggedObjects(string tag) {
        taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < taggedObjects.Length; i++) {
            Destroy(taggedObjects[i]);
            taggedObjects[i] = null;
        }
    }

    public int CountTaggedObjects(string tag) {
        taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        return taggedObjects.Length;
    }

    public void ReserveObjectPool() {
        if (ObjectPool.Instance) {
            for (int i = 0; i < 6; i++) {
                ObjectPool.Instance.ReserveObject(i, 80);
            }
        }
    }

    public void CleanObjectPool() {
        if (ObjectPool.Instance) {
            ObjectPool.Instance.CleanPoolAll(80);
        }
    }

    public int GetAchievement(int stageId) {
        int rate = 0;
        int ifrLength = stageSettings[stageId].ifrID.Length + stageSettings[stageId].secretType.Length;
        int ifrScoreMax = Mathf.Clamp(ifrLength * 10, 0, 50);
        int floorScoreMax = (stageSettings[stageId].isSpecialStage ? 100 : 90) - ifrScoreMax;
        int reachedFloor = GameManager.Instance.save.reachedFloor[stageId];
        int maxFloor = GameManager.Instance.GetDenotativeMaxFloor(stageId) - 1;
        if (GameManager.Instance.save.reachedFloor[stageId] >= maxFloor) {
            rate += floorScoreMax;
        } else {
            rate += floorScoreMax * reachedFloor / maxFloor;
        }
        if (ifrLength > 0) {
            int ifrScore = 0;
            int index = stageId - 1;
            if (stageSettings[stageId].ifrID.Length > 0) {
                for (int i = 0; i < stageSettings[stageId].ifrID.Length; i++) {
                    int ifrID = stageSettings[stageId].ifrID[i];
                    if (ifrID == lbID && index < GameManager.luckyBeastMax && GameManager.Instance.save.luckyBeast[index] != 0) {
                        ifrScore++;
                    } else if (ifrID == raccoonID) {
                        if (index < GameManager.hpUpNFSMax) {
                            if (GameManager.Instance.save.hpUpNFS[index] != 0) {
                                ifrScore++;
                            }
                        } else {
                            if (GameManager.Instance.save.friends[ifrID - ifrBottom] != 0) {
                                ifrScore++;
                            }
                        }
                    } else if (ifrID == fennecID) {
                        if (index < GameManager.stUpNFSMax) {
                            if (GameManager.Instance.save.stUpNFS[index] != 0) {
                                ifrScore++;
                            }
                        } else {
                            if (GameManager.Instance.save.friends[ifrID - ifrBottom] != 0) {
                                ifrScore++;
                            }
                        }
                    } else if (ifrID >= inventoryBottom && ifrID < inventoryBottom + GameManager.inventoryNFSMax && GameManager.Instance.save.inventoryNFS[ifrID - inventoryBottom] != 0) {
                        ifrScore++;
                    } else if (ifrID >= ifrBottom && ifrID < ifrBottom + GameManager.friendsMax && GameManager.Instance.save.friends[ifrID - ifrBottom] != 0) {
                        ifrScore++;
                    }
                }
            }
            if (stageSettings[stageId].secretType.Length > 0) {
                for (int i = 0; i < stageSettings[stageId].secretType.Length; i++) {
                    if (GameManager.Instance.GetSecret(stageSettings[stageId].secretType[i])) {
                        ifrScore++;
                    }
                }
            }
            if (ifrScore >= ifrLength) {
                rate += ifrScoreMax;
            } else {
                rate += ifrScoreMax * ifrScore / ifrLength;
            }
        }
        if (!stageSettings[stageId].isSpecialStage && GameManager.Instance.save.progress >= stageId) {
            rate += 10;
        }
        return rate;
    }

    public bool CheckPhotoStageCondition() {
        if (IsHomeStage) {
            return true;
        } else if (stageNumber <= GameManager.Instance.save.progress) {
            return true;
        } else if (stageNumber < GameManager.Instance.save.clearDifficulty.Length && GameManager.Instance.save.clearDifficulty[stageNumber] > 0) {
            return true;
        } else if (IsRandomStage && GameManager.Instance.GetSecret(GameManager.SecretType.SkytreeCleared)) {
            return true;
        } else {
            return false;
        }
    }

    public bool IsMappingFloor {
        get {
            return dungeonController && dungeonController.tileSettings.enabled && !dungeonController.isBossFloor;
        }
    }

    public bool IsGraphAutoFloor {
        get {
            if (IsRandomStage) {
                return dungeonController && !dungeonController.isBossFloor && dungeonController.generatorLevel >= 3;
            } else {
                return dungeonController && dungeonController.useGraphAuto;
            }
        }
    }

    public bool IsGraphComplexFloor {
        get {
            return dungeonController && dungeonController.generator && dungeonController.generator.isComplex;
        }
    }

    protected override void Awake() {
        base.Awake();
        MoveStage(GameManager.Instance.save.nowStage, GameManager.Instance.save.nowFloor);
    }

}
