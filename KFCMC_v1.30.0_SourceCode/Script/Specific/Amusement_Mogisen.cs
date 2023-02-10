using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Amusement_Mogisen : MonoBehaviour {

    [System.Serializable]
    public class EnemyChild {
        public int id;
        public int level;
        public int count;
    }

    [System.Serializable]
    public class EnemySet {
        public EnemyChild[] child;
    }

    [System.Serializable]
    public class HealSet {
        public int borderProgress;
        public int healHP;
    }

    public int mogisenRank;
    public int overrideSlipDamage;
    public int playerLevelLimit = 100;
    public int armsStageOverride;
    public int playerHpUpLimit;
    public HealSet[] healSet;
    public string dicKey;
    public GameObject readyTextPrefab;
    public GameObject startTextPrefab;
    public GameObject finishFailedTextPrefab;
    public GameObject finishSuccessTextPrefab;
    public GameObject resultTextPrefab;
    public Vector3 firstPos;
    public Vector3 pivotPos;
    public Vector3 returnedPlayerPos;
    public Vector3 returnedPlayerRot;
    public float[] radius;
    public EnemySet[] enemySet;
    public string steamAchievementName;

    int progress;
    int state;
    int clearTime;
    float elapsedTime;
    int newRecordFlag;
    bool gameOverFlag;
    bool[] friendsSave;
    int angSwitch;
    int radiusSwitch;
    int saveSlot = -1;
    int answer;
    StringBuilder sb = new StringBuilder();

    int hpSave;
    float stSave;
    float sandstarSave;
    int pauseWait;

    List<EnemyBase> eBaseList = new List<EnemyBase>();
    EnemyBase eBaseTemp;
    GameObject textInstance;
    const int rewardConditionRank = 4;
    const int rewardItemIndex = 95;
    const int rewardMinmiShiftNum = 1;
    const int baseFloorNum = 4; 

    private void Awake() {
        bool[] enemyLoaded = new bool[GameManager.enemyMax];
        for (int i = 0; i < enemySet.Length; i++) {
            for (int j = 0; j < enemySet[i].child.Length; j++) {
                int id = enemySet[i].child[j].id;
                if (!enemyLoaded[id]) {
                    enemyLoaded[id] = true;
                    GameObject tempEnemy = Instantiate(CharacterDatabase.Instance.GetEnemy(id));
                    if (tempEnemy) {
                        Destroy(tempEnemy);
                    }
                }
            }
        }
        if (CharacterManager.Instance) {
            friendsSave = new bool[GameManager.friendsMax];
            for (int i = 0; i < friendsSave.Length; i++) {
                friendsSave[i] = CharacterManager.Instance.GetFriendsExist(i, true);
            }
            if (!CharacterManager.Instance.mogisenMultiPlay) {
                CharacterManager.Instance.FriendsClearAll(false);
            }

            CharacterManager.Instance.SetAnimDeadSpecialAll(true);
        }
        if (StageManager.Instance && StageManager.Instance.dungeonController) {
            StageManager.Instance.dungeonController.playerLevelLimit = playerLevelLimit;
            StageManager.Instance.dungeonController.armsStageOverride = armsStageOverride;
            StageManager.Instance.dungeonController.playerHpUpLimit = playerHpUpLimit;
        }
    }

    private void Start() {
        if (MessageUI.Instance) {
            MessageUI.Instance.SetMessage(TextManager.Get("AMUSEMENT_ZAKORUSH") + " " + TextManager.Get(dicKey));
        }
        eBaseList.Clear();
        eBaseTemp = StageManager.Instance.dungeonController.SummonSpecificEnemy(enemySet[0].child[0].id, enemySet[0].child[0].level, firstPos);
        if (eBaseTemp) {
            eBaseTemp.ForceStopForEvent(3f);
            eBaseTemp.SetForAmusement(this);
            eBaseList.Add(eBaseTemp);
        }
        clearTime = 0;
        newRecordFlag = 0;
        gameOverFlag = false;
        angSwitch = 0;
        radiusSwitch = 0;
        elapsedTime = 0;
        state = 0;
        progress = 1;
        if (PauseController.Instance) {
            PauseController.Instance.friendsDisabled = true;
            PauseController.Instance.itemDisabled = true;
            PauseController.Instance.equipChangeDisabled = true;
            PauseController.Instance.gameOverDisabled = true;
        }
        if (StageManager.Instance) {
            StageManager.Instance.slipDamageOverride = overrideSlipDamage;
        }
        if (CharacterManager.Instance) {
            CharacterManager.Instance.AddSandstar(-100, true);
            CharacterManager.Instance.ForceStopForEventAll(3f);
            CharacterManager.Instance.BossTimeInit();
            for (int i = 0; i < CharacterManager.Instance.friends.Length; i++) {
                if (CharacterManager.Instance.GetFriendsExist(i)) {
                    CharacterManager.Instance.friends[i].trans.rotation = CharacterManager.Instance.playerTrans.rotation;
                }
            }
        }
    }

    private void Update() {
        if (PauseController.Instance) {
            if (PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
        }
        switch (state) {
            case 0:
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 1.5f) {
                    Vector2 pos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                    if (textInstance) {
                        Destroy(textInstance);
                    }
                    textInstance = Instantiate(readyTextPrefab, PauseController.Instance.offPauseCanvas.transform);
                    textInstance.GetComponent<FadeOutText>().SetText(TextManager.Get("AMUSEMENT_READY"), pos, 1f, -1f, 0);
                    elapsedTime = 0f;
                    state++;
                }
                break;
            case 1:
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 1f) {
                    Vector2 pos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                    if (textInstance) {
                        Destroy(textInstance);
                    }
                    textInstance = Instantiate(startTextPrefab, PauseController.Instance.offPauseCanvas.transform);
                    textInstance.GetComponent<FadeOutText>().SetText(TextManager.Get("AMUSEMENT_START"), pos, 1f, -1f, 0);
                    if (eBaseList.Count > 0 && eBaseList[0] != null) {
                        eBaseList[0].ReleaseStopForEvent();
                    }
                    CharacterManager.Instance.ReleaseStopForEventAll();
                    CharacterManager.Instance.BossTimeStart();
                    elapsedTime = 0f;
                    state++;
                }
                break;
            case 2:
                if (Time.timeScale > 0f) {
                    int length = eBaseList.Count;
                    for (int i = 0; i < length; i++) {
                        if (eBaseList[i] == null) {
                            eBaseList.RemoveAt(i);
                            i--;
                            length--;
                        }
                    }
                    if (CharacterManager.Instance.pCon.GetNowHP() <= 0) {
                        gameOverFlag = true;
                    }
                    if (gameOverFlag) {
                        CharacterManager.Instance.BossTimeEnd();
                        if (GameManager.Instance.minmiGolden) {
                            newRecordFlag = 2;
                        }
                        Vector2 pos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                        if (textInstance) {
                            Destroy(textInstance);
                        }
                        textInstance = Instantiate(finishFailedTextPrefab, PauseController.Instance.offPauseCanvas.transform);
                        textInstance.GetComponent<FadeOutText>().SetText(TextManager.Get("AMUSEMENT_FINISH"), pos, 1f, -1f, 0);
                        elapsedTime = 0f;
                        state++;
                    } else if (length <= 0) {
                        if (progress < enemySet.Length) {
                            CharacterManager.Instance.ClearSickAll();
                            int healTemp = 0;
                            for (int i = healSet.Length - 1; i >= 0; i--) {
                                if (progress > healSet[i].borderProgress) {
                                    healTemp = healSet[i].healHP;
                                    break;
                                }
                            }
                            CharacterManager.Instance.HealAsMuchAsNeededAll(healTemp, true);                            
                            CallEnemy();
                        } else {
                            CharacterManager.Instance.BossTimeEnd();
                            clearTime = CharacterManager.Instance.GetBossTime100();
                            int saveIndex = mogisenRank * GameManager.difficultyMax + Mathf.Clamp(GameManager.Instance.save.difficulty - 1, 0, GameManager.difficultyMax - 1) + (CharacterManager.Instance.mogisenMultiPlay ? GameManager.zakoRushArrayMax / 2 : 0);
                            if (GameManager.Instance.save.zakoRushClearTime.Length < GameManager.zakoRushArrayMax) {
                                GameManager.Instance.FixZakoRushTimeLength();
                            }
                            if (GameManager.Instance.minmiGolden) {
                                newRecordFlag = 2;
                            } else if (clearTime > 0 && (GameManager.Instance.save.zakoRushClearTime[saveIndex] <= 0 || clearTime < GameManager.Instance.save.zakoRushClearTime[saveIndex])) {
                                GameManager.Instance.save.zakoRushClearTime[saveIndex] = clearTime;
                                newRecordFlag = 1;
                            }
                            if (mogisenRank == rewardConditionRank) {
                                bool isParenting = (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.itemSettings.container);
                                ItemDatabase.Instance.GiveItem(rewardItemIndex, pivotPos + Vector3.up, 5f, 1.2f, 1.2f, 1.2f, isParenting ? StageManager.Instance.dungeonController.itemSettings.container : null);
                                GameManager.Instance.save.minmi |= (1 << rewardMinmiShiftNum);
                            }
                            Vector2 pos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                            if (textInstance) {
                                Destroy(textInstance);
                            }
                            hpSave = CharacterManager.Instance.pCon.GetNowHP();
                            stSave = CharacterManager.Instance.pCon.GetNowST();
                            sandstarSave = CharacterManager.Instance.GetSandstar();
                            textInstance = Instantiate(finishSuccessTextPrefab, PauseController.Instance.offPauseCanvas.transform);
                            textInstance.GetComponent<FadeOutText>().SetText(TextManager.Get("AMUSEMENT_FINISH"), pos, 1f, -1f, 0);
                            CharacterManager.Instance.SetAllFriendsWinAction();
                            elapsedTime = 0f;
                            state++;
                            if (GameManager.Instance.GetLevelNow <= playerLevelLimit) {
                                TrophyManager.Instance.CheckTrophy(TrophyManager.t_TrainingClearLimited + mogisenRank);
                            }
                            if (mogisenRank == 4 && GameManager.Instance.minmiRed) {
                                TrophyManager.Instance.CheckTrophy(TrophyManager.t_TrainingClearRedMinmi);
                            }
                            if (!string.IsNullOrEmpty(steamAchievementName)) {
                                GameManager.Instance.SetSteamAchievement(steamAchievementName);
                            }
                        }
                    }
                }
                break;
            case 3:
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 1f) {
                    if (textInstance) {
                        Destroy(textInstance);
                    }
                    textInstance = Instantiate(resultTextPrefab, PauseController.Instance.offPauseCanvas.transform);
                    Amusement_ResultText result = textInstance.GetComponent<Amusement_ResultText>();
                    if (result) {
                        if (gameOverFlag) {
                            result.SetText(dicKey, mogisenRank, 0, progress - 1, enemySet.Length, newRecordFlag, hpSave, stSave, sandstarSave);
                        } else {
                            result.SetText(dicKey, mogisenRank, clearTime, progress, enemySet.Length, newRecordFlag, hpSave, stSave, sandstarSave);
                        }
                    }
                    elapsedTime = 0f;
                    state++;
                }
                break;
            case 4:
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 0.5f) {
                    elapsedTime = 0f;
                    state++;
                }
                break;
            case 5:
                if (pauseWait <= 0 && !PauseController.Instance.GetPauseEnabled() && !PauseController.Instance.IsPhotoPausing && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                    PauseController.Instance.SetChoices(3, true, TextManager.Get("AMUSEMENT_ZAKORUSH") + " " + TextManager.Get(dicKey), "AMUSEMENT_RETURN", "AMUSEMENT_SAVERETURN", "AMUSEMENT_RETRY");
                    state++;
                }
                break;
            case 6:
                switch (PauseController.Instance.ChoicesControl()) {
                    case -2:
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        PauseController.Instance.CancelChoices();
                        PauseController.Instance.HideCaution();
                        state -= 1;
                        break;
                    case 0:
                        UISE.Instance.Play(UISE.SoundName.submit);
                        PauseController.Instance.CancelChoices(false);
                        PauseController.Instance.HideCaution();
                        SceneChange.Instance.StartEyeCatch();
                        saveSlot = -1;
                        state += 2;
                        break;
                    case 1:
                        UISE.Instance.Play(UISE.SoundName.submit);
                        PauseController.Instance.CancelChoices(false);
                        PauseController.Instance.HideCaution();
                        SaveController.Instance.permitEmptySlot = true;
                        SaveController.Instance.Activate();
                        saveSlot = -1;
                        state += 1;
                        break;
                    case 2:
                        UISE.Instance.Play(UISE.SoundName.submit);
                        PauseController.Instance.CancelChoices();
                        PauseController.Instance.HideCaution();
                        SceneChange.Instance.StartEyeCatch();
                        saveSlot = -1;
                        state += 3;
                        break;
                }
                break;
            case 7:
                saveSlot = -1;
                answer = SaveController.Instance.SaveControlExternal();
                if (answer >= 0 && answer < GameManager.saveSlotMax) {
                    saveSlot = answer;
                    state += 1;
                } else if (answer < -1) {
                    PauseController.Instance.CancelChoices();
                    state -= 2;
                }
                break;
            case 8:
                if (SceneChange.Instance.GetEyeCatch()) {
                    if (textInstance) {
                        Destroy(textInstance);
                    }
                    PauseController.Instance.CancelChoices();
                    if (StageManager.Instance && StageManager.Instance.dungeonMother) {
                        PauseController.Instance.friendsDisabled = false;
                        PauseController.Instance.itemDisabled = false;
                        PauseController.Instance.equipChangeDisabled = false;
                        PauseController.Instance.gameOverDisabled = false;
                        StageManager.Instance.slipDamageOverride = 0;
                        CharacterManager.Instance.ResetForMoveStage();
                        StageManager.Instance.dungeonMother.MoveFloor(baseFloorNum, saveSlot, false);
                        if (CharacterManager.Instance.playerTrans) {
                            CharacterManager.Instance.playerTrans.position = returnedPlayerPos;
                            CharacterManager.Instance.playerTrans.eulerAngles = returnedPlayerRot;
                            if (CameraManager.Instance) {
                                CameraManager.Instance.ResetCameraFixPos();
                            }
                        }
                        ReadyForRestoreFriends();
                        CharacterManager.Instance.RestoreFriends();
                    }
                    SceneChange.Instance.EndEyeCatch();
                }
                break;
            case 9:
                if (SceneChange.Instance.GetEyeCatch()) {
                    if (textInstance) {
                        Destroy(textInstance);
                    }
                    PauseController.Instance.CancelChoices();
                    eBaseList.Clear();
                    StageManager.Instance.CleaningObjects();
                    CharacterManager.Instance.ResetForRetry();
                    CharacterManager.Instance.ResetForMoveFloor();
                    if (CharacterManager.Instance.playerObj && CharacterManager.Instance.playerTrans && StageManager.Instance.dungeonController) {
                        CharacterManager.Instance.playerObj.SetActive(false);
                        CharacterManager.Instance.playerTrans.position = StageManager.Instance.dungeonController.fixSettings.player.position;
                        CharacterManager.Instance.playerTrans.eulerAngles = StageManager.Instance.dungeonController.fixSettings.player.rotation;
                        CharacterManager.Instance.playerObj.SetActive(true);
                        CharacterManager.Instance.PlaceFriendsAroundPlayer();
                    }
                    if (CameraManager.Instance) {
                        CameraManager.Instance.smallRate = 0f;
                        CameraManager.Instance.SetYAxisRate();
                        CameraManager.Instance.ResetCameraFixPos();
                    }
                    Start();
                    SceneChange.Instance.EndEyeCatch();
                }
                break;
        }
    }

    public void Retry() {
        state = 9;
    }

    public bool Cleared => (state > 2);

    public void ReadyForRestoreFriends() {
        for (int i = 0; i < friendsSave.Length; i++) {
            GameManager.Instance.save.friendsLiving[i] = friendsSave[i] ? 1 : 0;
        }
    }

    private void SaveCancelCommon() {
        SaveController.Instance.Deactivate();
        PauseController.Instance.CancelChoices();
        state = 1;
    }

    void CallEnemy() {
        int countSum = 0;
        int countBias = GameManager.Instance.minmiBlack ? 4 : 1;
        int levelBias = GameManager.Instance.minmiRed ? 1 : 0;
        for (int i = 0; i < enemySet[progress].child.Length; i++) {
            countSum += enemySet[progress].child[i].count * countBias;
        }
        if (countSum == 1) {
            int levelTemp = enemySet[progress].child[0].level + levelBias;
            eBaseTemp = StageManager.Instance.dungeonController.SummonSpecificEnemy(enemySet[progress].child[0].id, levelTemp, pivotPos);
            if (eBaseTemp) {
                eBaseTemp.SetForAmusement(this);
                eBaseList.Add(eBaseTemp);
            }
        } else if (countSum > 1) {
            float angDiff = 360f / countSum;
            float angStart = (countSum == 3 ? 90f : angDiff * 0.5f) * angSwitch;
            int countNow = 0;
            for (int i = 0; i < enemySet[progress].child.Length; i++) {
                for (int j = 0; j < enemySet[progress].child[i].count * countBias; j++) {
                    Vector3 posTemp = pivotPos;
                    float angle = (angStart + angDiff * countNow) * Mathf.Deg2Rad;
                    posTemp.x += Mathf.Sin(angle) * radius[radiusSwitch];
                    posTemp.z += Mathf.Cos(angle) * radius[radiusSwitch];
                    int levelTemp = enemySet[progress].child[i].level + levelBias;
                    eBaseTemp = StageManager.Instance.dungeonController.SummonSpecificEnemy(enemySet[progress].child[i].id, levelTemp, posTemp);
                    if (eBaseTemp) {
                        eBaseTemp.SetForAmusement(this);
                        eBaseTemp.LookAtIgnoreY(CharacterManager.Instance.playerTrans.position);
                        eBaseTemp.RegisterTargetHate(CharacterManager.Instance.pCon, 40f);
                        eBaseList.Add(eBaseTemp);
                    }
                    countNow++;
                }
            }
            angSwitch = (angSwitch + 1) % 4;
            radiusSwitch = (radiusSwitch + 1) % radius.Length;
        }
        progress++;
        if (MessageUI.Instance) {
            MessageUI.Instance.SetMessage(sb.Clear().Append(TextManager.Get("AMUSEMENT_PROGRESS")).Append(" ").AppendFormat("{0} / {1}", progress - 1, enemySet.Length).ToString());
        }
    }

    public void AddEnemy(EnemyBase eBase) {
        eBaseList.Add(eBase);
    }

    private void OnDestroy() {
        if (textInstance) {
            Destroy(textInstance);
        }
        if (CharacterManager.Instance && AnimHash.Instance) {
            CharacterManager.Instance.SetAnimDeadSpecialAll(false);
        }
    }

}
