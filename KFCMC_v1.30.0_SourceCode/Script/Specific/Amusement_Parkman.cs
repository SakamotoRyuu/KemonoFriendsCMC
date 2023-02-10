using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class Amusement_Parkman : MonoBehaviour
{

    public GameObject titleTextPrefab;
    public Transform titleCameraPivot;
    public GameObject ballPrefab;
    public GameObject japaribunPrefab;
    public GameObject blockPrefab;
    public GameObject coinPrefab;
    public GameObject coinBigPrefab;
    public GameObject warpPrefab;
    public GameObject lightPrefab;
    public GameObject lavaPrefab;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public GameObject teleportPointPrefab;
    public int[] lightingIndex;
    public AudioSource alartAudio;
    public GameObject levelTextPrefab;
    public GameObject clearTextPrefab;
    public GameObject scoreTextPrefab;
    public GameObject oneupTextPrefab;
    public GameObject gameoverTextPrefab;
    public GameObject goalEffectPrefab;
    public GameObject goalPrefab;
    public GameObject endingEffectPrefab;
    public GameObject endingTextPrefab;
    public int goalLightingIndex;
    public int returnedFloorNum;
    public Vector3 returnedPlayerPos;
    public Vector3 returnedPlayerRot;
    public Vector3 returnedPlayerPosNHO;
    public Vector3 returnedPlayerRotNHO;
    public GameObject redFoxSpecialTalk;
    public GameObject redFoxCheatTalk;

    public enum ItemType { Ball, Japaribun, Coin};
    public bool IsSuperman => supermanTimeRemain > 0f;
    public bool IsLightActive { get; set; }
    public bool IsEnemyActive { get; set; }

    const int rewardItemIndex = 96;
    const int rewardMinmiShiftNum = 2;
    const int redfoxBorder = 50000;
    const int enemyMax = 4;
    const int ballMod = 1;
    const int japaribunMod = 2;
    const int blockMod = 3;
    const int coinMod = 4;
    const int warpMod = 5;
    const int lightMod = 6;
    const int lavaMod = 7;
    const int enemyMod = 8;
    const int teleportPointMod = 9;
    const int passageDisappearLevel = 2;
    const int ballDisappearLevel = 3;
    const int noMapLevel = 4;
    const int lavaLevel = 5;
    const int bossLevel = 5;
    const int levelMax = 5;
    const float supermanTimeMax = 15f;
    const int clearBonusNum = 3000;
    const int readyingState = 2;
    const int bodyState = 4;
    const int defeatedState = 20;
    const int returnState = 102;
    static readonly Quaternion quaIden = Quaternion.identity;

    int state;
    int level;
    int zanki;
    int extendScore;
    int comboCount = 0;
    float elapsedTime;
    float clearTime;
    int score;
    bool gameOverFlag;
    bool[] friendsSave;
    int saveSlot = -1;
    GameObject textInstance;
    Camera mainCamera;
    StringBuilder sb = new StringBuilder();
    int pauseWait;
    int ballCount;
    int ballMax;
    GameObject coinInstance;
    float supermanTimeRemain;
    float deltaTimeCache;
    Transform container;
    int[] comboScoreArray = new int[4] { 200, 400, 800, 1600 };
    float[] playerSpeed = new float[5] { 10.5f, 11.0f, 11.5f, 12.0f, 12.5f};
    float[] enemySpeed = new float[5] { 9.5f, 10.5f, 11.5f, 12.5f, 13.5f };
    int[] coinScore = new int[5] { 400, 500, 700, 900, 1500 };
    List<Vector3> coinPosList = new List<Vector3>();
    Vector3[] enemyPosArray = new Vector3[enemyMax];
    Vector3 enemyBossPos;
    Enemy_ParkmanCellien[] enemyBaseArray = new Enemy_ParkmanCellien[enemyMax];
    Enemy_ParkmanBoss enemyBossBase;
    bool walkableActiveSave;
    bool isWalkableGot;
    bool defeatScoreDisabled;
    bool alartSave;
    bool retryFlag;
    Color alartColor = new Color(1f, 1f, 1f, 0f);
    float alartingTime;
    GameObject[] walkableObjects = new GameObject[0];
    GameObject goalInstance;
    bool clearedFlag;
    bool scoreTopFlag;
    int answer;
    int cheatCommandProgress;
    bool cheatFlag;
    Vector2Int moveCursor;
    bool cheatTalkFlag;
    static readonly Vector2Int vec2IntZero = Vector2Int.zero;

    void Awake() {
        if (CharacterManager.Instance) {
            friendsSave = new bool[GameManager.friendsMax];
            for (int i = 0; i < friendsSave.Length; i++) {
                friendsSave[i] = CharacterManager.Instance.GetFriendsExist(i, true);
            }
            CharacterManager.Instance.FriendsClearAll(false);
        }
    }

    private void Start() {
        clearTime = 0;
        zanki = cheatFlag ? 99 : 3;
        score = 0;
        extendScore = 0;
        comboCount = 0;
        gameOverFlag = false;
        elapsedTime = 0;
        state = 0;
        level = 0;
        defeatScoreDisabled = false;
        if (PauseController.Instance) {
            PauseController.Instance.friendsDisabled = true;
            PauseController.Instance.itemDisabled = true;
            PauseController.Instance.equipChangeDisabled = true;
            PauseController.Instance.gameOverDisabled = true;
        }
        if (CharacterManager.Instance) {
            CharacterManager.Instance.BossTimeHide();
            CharacterManager.Instance.AddSandstar(-100, true);
            CharacterManager.Instance.pCon.ForceStopForEvent(3f);
            CharacterManager.Instance.SetForParkman(true);
            CharacterManager.Instance.parkmanSettings.onObj[0].SetActive(false);
            CharacterManager.Instance.parkmanSettings.score.colorGradientPreset = (GameManager.Instance.minmiPurple ? CharacterManager.Instance.parkmanSettings.scoreHardColor : CharacterManager.Instance.parkmanSettings.scoreNormalColor);
            CharacterManager.Instance.parkmanSettings.score.text = score.ToString();
            CharacterManager.Instance.parkmanSettings.zanki.text = zanki.ToString();
            CharacterManager.Instance.parkmanSettings.ball.text = "";
            CharacterManager.Instance.parkmanSettings.supermanGauge.fillAmount = 0f;
            ActivateAlart(false);
        }
        if (LightingDatabase.Instance.nowLightingNumber != lightingIndex[level]) {
            LightingDatabase.Instance.SetLighting(lightingIndex[level]);
            CharacterManager.Instance.SetPlayerLightActive();
        }
        if (!isWalkableGot) {
            isWalkableGot = true;
            walkableObjects = GameObject.FindGameObjectsWithTag("MapWalkable");
        }
        if (container && container.gameObject) {
            Destroy(container.gameObject);
        }
        GetMainCamera();
        textInstance = Instantiate(titleTextPrefab, PauseController.Instance.offPauseCanvas.transform);
        CameraManager.Instance.SetEventCameraFollowTarget(titleCameraPivot, 500000, 0f, Vector3.Distance(titleCameraPivot.position, Vector3.zero));
        StageManager.Instance.mapActivateFlag = 0;
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

    void Update() {
        if (PauseController.Instance) {
            if (PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
        }
        switch (state) {
            case 0://Title
                if (CharacterManager.Instance.pCon) {
                    CharacterManager.Instance.pCon.ForceStopForEvent(1f);
                }
                CameraManager.Instance.SetEventCameraFollowTarget(titleCameraPivot, 500000, 0f, Vector3.Distance(titleCameraPivot.position, Vector3.zero));
                if (pauseWait <= 0 && !PauseController.Instance.pauseGame && !PauseController.Instance.IsPhotoPausing) {
                    if (!cheatFlag) {
                        moveCursor = GameManager.Instance.MoveCursor(false);
                        if (moveCursor != vec2IntZero) {
                            switch (cheatCommandProgress) {
                                case 0:
                                case 1:
                                    if (moveCursor.x == 0 && moveCursor.y == -1) {
                                        cheatCommandProgress++;
                                    } else {
                                        cheatCommandProgress = 0;
                                    }
                                    break;
                                case 2:
                                case 3:
                                    if (moveCursor.x == 0 && moveCursor.y == 1) {
                                        cheatCommandProgress++;
                                    } else {
                                        cheatCommandProgress = 0;
                                    }
                                    break;
                                case 4:
                                case 6:
                                    if (moveCursor.x == -1 && moveCursor.y == 0) {
                                        cheatCommandProgress++;
                                    } else {
                                        cheatCommandProgress = 0;
                                    }
                                    break;
                                case 5:
                                case 7:
                                    if (moveCursor.x == 1 && moveCursor.y == 0) {
                                        cheatCommandProgress++;
                                    } else {
                                        cheatCommandProgress = 0;
                                    }
                                    break;
                                default:
                                    cheatCommandProgress = 0;
                                    break;
                            }
                        }
                        if (!cheatFlag && cheatCommandProgress == 8) {
                            if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Cancel)) {
                                cheatCommandProgress = 9;
                            } else if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                                cheatCommandProgress = 0;
                            }
                        }
                    }
                    if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Cancel)) {
                        PauseController.Instance.SetChoices(3, false, TextManager.Get(GameManager.Instance.minmiPurple ? "AMUSEMENT_PARKMAN_HARD" : "AMUSEMENT_PARKMAN"), "AMUSEMENT_GAMESTART", "AMUSEMENT_RETURN", "AMUSEMENT_SAVERETURN");
                        state = 1;
                        elapsedTime = 0f;
                    }
                }
                break;
            case 1://Title Choices
                if (CameraManager.Instance) {
                    CameraManager.Instance.SetEventTimer(1.5f);
                }
                if (CharacterManager.Instance.pCon) {
                    CharacterManager.Instance.pCon.ForceStopForEvent(1.5f);
                }
                saveSlot = -1;
                switch (PauseController.Instance.ChoicesControl()) {
                    case -2:
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        PauseController.Instance.CancelChoices();
                        PauseController.Instance.HideCaution();
                        state -= 1;
                        cheatCommandProgress = 0;
                        break;
                    case 0:
                        level = 1;
                        score = 0;
                        comboCount = 0;
                        supermanTimeRemain = 0;
                        extendScore = 0;
                        UISE.Instance.Play(UISE.SoundName.use);
                        SceneChange.Instance.StartEyeCatch(false);
                        state += 1;
                        if (!cheatFlag && cheatCommandProgress == 9) {
                            UISE.Instance.Play(UISE.SoundName.delete);
                            cheatCommandProgress = 0;
                            cheatFlag = true;
                            zanki = 99;
                        }
                        break;
                    case 1:
                        UISE.Instance.Play(UISE.SoundName.submit);
                        PauseController.Instance.CancelChoices(false);
                        PauseController.Instance.HideCaution();
                        SceneChange.Instance.StartEyeCatch();
                        saveSlot = -1;
                        state = 101;
                        cheatCommandProgress = 0;
                        break;
                    case 2:
                        UISE.Instance.Play(UISE.SoundName.submit);
                        PauseController.Instance.CancelChoices(false);
                        PauseController.Instance.HideCaution();
                        SaveController.Instance.permitEmptySlot = true;
                        SaveController.Instance.Activate();
                        saveSlot = -1;
                        state = 100;
                        cheatCommandProgress = 0;
                        break;
                }
                break;
            case readyingState://Readying
                if (CameraManager.Instance) {
                    CameraManager.Instance.SetEventTimer(1.5f);
                }
                if (CharacterManager.Instance.pCon) {
                    CharacterManager.Instance.pCon.ForceStopForEvent(1.5f);
                }
                if (SceneChange.Instance.GetEyeCatch()) {
                    PauseController.Instance.CancelChoices();
                    CharacterManager.Instance.pCon.SetNowHP(CharacterManager.Instance.pCon.GetMaxHP());
                    CharacterManager.Instance.ResetGuts();
                    ReadyLevel(retryFlag);
                    retryFlag = false;
                    SceneChange.Instance.EndEyeCatch();
                    if (CameraManager.Instance) {
                        CameraManager.Instance.SetEventTimer(1.5f);
                    }
                    if (CharacterManager.Instance.pCon) {
                        CharacterManager.Instance.pCon.ForceStopForEvent(1.5f);
                    }
                    state += 1;
                    elapsedTime = 0f;
                }
                break;
            case 3:
                if (CharacterManager.Instance.pCon) {
                    CharacterManager.Instance.pCon.ForceStopForEvent(1.5f);
                }
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 1.25f) {
                    state += 1;
                    elapsedTime = 0f;
                    IsEnemyActive = true;
                    CharacterManager.Instance.BossTimeStart();
                    if (CharacterManager.Instance.pCon) {
                        CharacterManager.Instance.pCon.ReleaseStopForEvent();
                    }
                }
                break;
            case bodyState://Body
                deltaTimeCache = Time.deltaTime;
                clearTime += deltaTimeCache;
                for (int i = 0; i < enemyMax; i++) {
                    if (enemyBaseArray[i] == null) {
                        SetEnemy(i, enemyPosArray[i], 15f);
                    }
                }
                if (isWalkableGot) {
                    if (walkableActiveSave != IsPassageVisible) {
                        walkableActiveSave = IsPassageVisible;
                        for (int i = 0; i < walkableObjects.Length; i++) {
                            if (walkableObjects[i]) {
                                walkableObjects[i].SetActive(walkableActiveSave);
                            }
                        }
                    }
                }
                if (CharacterManager.Instance.pCon.GetNowHP() <= 0) {
                    if (supermanTimeRemain > 0f) {
                        supermanTimeRemain = 0f;
                        CharacterManager.Instance.parkmanSettings.supermanGauge.fillAmount = 0f;
                        CharacterManager.Instance.pCon.SupermanEnd(true);
                        comboCount = 0;
                    }
                    ActivateAlart(false);
                    zanki -= 1;
                    comboCount = 0;
                    CharacterManager.Instance.parkmanSettings.zanki.text = Mathf.Max(0, zanki).ToString();
                    elapsedTime = 0f;
                    state = defeatedState;
                } else {
                    bool alartTemp = false;
                    if (supermanTimeRemain <= 0f) {
                        for (int i = 0; i < enemyBaseArray.Length; i++) {
                            if (enemyBaseArray[i] != null && enemyBaseArray[i].GetTargetExist()) {
                                alartTemp = true;
                                break;
                            }
                        }
                    }
                    if (enemyBossBase != null && enemyBossBase.GetTargetExist()) {
                        alartTemp = true;
                    }
                    if (alartSave != alartTemp) {
                        ActivateAlart(alartTemp);
                    }
                    if (alartSave) {
                        alartingTime += deltaTimeCache;
                        if (alartingTime >= 0.5f) {
                            alartingTime -= 0.5f;
                        }
                        alartColor.a = (alartingTime < 0.25f ? Easing.SineOut(alartingTime, 0.25f, 0f, 1f) : 1f - Easing.SineIn(alartingTime - 0.25f, 0.25f, 0f, 1f));
                        CharacterManager.Instance.parkmanSettings.alert.color = alartColor;
                    }
                }
                if (supermanTimeRemain > 0f) {
                    supermanTimeRemain -= deltaTimeCache;
                    if (gameOverFlag) {
                        supermanTimeRemain = -1;
                    }
                    CharacterManager.Instance.parkmanSettings.supermanGauge.fillAmount = Mathf.Clamp01(supermanTimeRemain / supermanTimeMax);
                    if (supermanTimeRemain <= 0f && CharacterManager.Instance.pCon) {
                        CharacterManager.Instance.pCon.SupermanEnd(true);
                        comboCount = 0;
                    }
                }
                if (ballCount >= ballMax) {
                    IsEnemyActive = false;
                    ActivateAlart(false);
                    CharacterManager.Instance.BossTimeEnd();
                    TimeBonus();
                    if (CharacterManager.Instance.pCon) {
                        CharacterManager.Instance.pCon.SupermanEnd();
                        CharacterManager.Instance.pCon.ForceStopForEvent(3f);
                        supermanTimeRemain = 0f;
                        comboCount = 0;
                        CharacterManager.Instance.parkmanSettings.supermanGauge.fillAmount = 0f;
                    }
                    for (int i = 0; i < enemyMax; i++) {
                        if (enemyBaseArray[i] != null) {
                            enemyBaseArray[i].ForceStopForEvent(5f);
                        }
                        if (enemyBossBase != null) {
                            enemyBossBase.ForceStopForEvent(5f);
                        }
                    }
                    if (textInstance) {
                        Destroy(textInstance);
                    }
                    textInstance = Instantiate(clearTextPrefab, PauseController.Instance.offPauseCanvas.transform);
                    if (level < levelMax) {
                        textInstance.GetComponent<FadeOutText>().SetText(sb.Clear().Append(TextManager.Get("WORD_LEVEL")).Append(" ").Append(level).Append(" ").Append(TextManager.Get("AMUSEMENT_CLEAR")).ToString(), new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), 3f, -1f, 0);
                    } else {
                        textInstance.GetComponent<FadeOutText>().SetText(TextManager.Get("AMUSEMENT_ALLCLEAR"), new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), 3f, -1f, 0);
                    }
                    state += 1;
                }
                break;
            case 5://Level Cleared Waiting
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 2f) {
                    retryFlag = false;
                    if (level < levelMax) {
                        level++;
                        SceneChange.Instance.StartEyeCatch(false);
                        state = readyingState;
                    } else {
                        defeatScoreDisabled = true;
                        for (int i = 0; i < enemyMax; i++) {
                            if (enemyBaseArray[i] != null) {
                                enemyBaseArray[i].TakeDamageFixKnock(50000, enemyBaseArray[i].GetCenterPosition(), 50000, (enemyBaseArray[i].transform.position - CharacterManager.Instance.playerTrans.position).normalized, null, 3, true);
                            }
                        }
                        if (enemyBossBase != null) {
                            enemyBossBase.TakeDamageFixKnock(50000, enemyBossBase.GetCenterPosition(), 50000, (enemyBossBase.transform.position - CharacterManager.Instance.playerTrans.position).normalized, null, 3, true);
                        }
                        defeatScoreDisabled = false;
                        ClearBonus();
                        if (StageManager.Instance.dungeonController) {
                            Instantiate(goalEffectPrefab, CharacterManager.Instance.playerTrans.position, Quaternion.identity);
                            CameraManager.Instance.SetQuake(CharacterManager.Instance.playerTrans.position, 8, 0.2f, 2f);
                            if (StageManager.Instance.dungeonController.groundSettings.container) {
                                StageManager.Instance.dungeonController.groundSettings.container.gameObject.SetActive(false);
                            }
                            if (StageManager.Instance.dungeonController.tileSettings.container) {
                                StageManager.Instance.dungeonController.tileSettings.container.gameObject.SetActive(false);
                            }
                            StageManager.Instance.SetAllMapChipDeactivate();
                            if (container && container.gameObject) {
                                Destroy(container.gameObject);
                            }
                            goalInstance = Instantiate(goalPrefab, CharacterManager.Instance.playerTrans.position, CharacterManager.Instance.playerTrans.rotation);
                        }
                        BGM.Instance.Stop();
                        GameManager.Instance.ChangeSnapshot("Snapshot", 0f);
                        LightingDatabase.Instance.SetLighting(goalLightingIndex);
                        CharacterManager.Instance.SetPlayerLightActive();
                        CharacterManager.Instance.pCon.ForceStopForEvent(5f);
                        PauseController.Instance.pauseEnabled = false;
                        clearedFlag = true;
                        GameManager.Instance.save.minmi |= (1 << rewardMinmiShiftNum);
                        elapsedTime = 0f;
                        state += 1;
                    }
                }
                break;
            case 6:
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 3f) {
                    if (CameraManager.Instance) {
                        PlayerController_Parkman parkmanController = CharacterManager.Instance.pCon.GetComponent<PlayerController_Parkman>();
                        if (parkmanController) {
                            CameraManager.Instance.SetEventCameraFollowTarget(parkmanController.cameraFacePivot, 5f, 1.5f, 1f);
                        }
                    }
                    elapsedTime = 0f;
                    state += 1;
                }
                break;
            case 7:
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 1.5f) {
                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_ParkmanClear);
                    if (score >= redfoxBorder) {
                        TrophyManager.Instance.CheckTrophy(TrophyManager.t_ParkmanHighScore);
                        scoreTopFlag = true;
                    }
                    CharacterManager.Instance.pCon.WinAction();
                    Instantiate(endingEffectPrefab);
                    elapsedTime = 0f;
                    state += 1;
                }
                break;
            case 8:
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 1.5f) {
                    if (textInstance) {
                        Destroy(textInstance);
                    }
                    textInstance = Instantiate(endingTextPrefab, PauseController.Instance.offPauseCanvas.transform);
                    textInstance.GetComponent<FadeOutText>().SetText(TextManager.Get("AMUSEMENT_CONGRATS"), new Vector2(Screen.width * 0.5f, Screen.height * 0.2f), 5f, -1f, 0);
                    Time.timeScale = 0f;
                    elapsedTime = 0f;
                    cheatTalkFlag = false;
                    state += 1;
                }
                break;
            case 9:
                elapsedTime += Time.unscaledDeltaTime;
                if (elapsedTime >= 4.5f) {
                    SceneChange.Instance.StartEyeCatch(false);
                    elapsedTime = 0f;
                    state += 1;
                }
                break;
            case 10://Clear Move
                if (SceneChange.Instance.GetEyeCatch()) {
                    if (textInstance) {
                        Destroy(textInstance);
                    }
                    if (StageManager.Instance.dungeonController.groundSettings.container) {
                        StageManager.Instance.dungeonController.groundSettings.container.gameObject.SetActive(true);
                    }
                    if (StageManager.Instance.dungeonController.tileSettings.container) {
                        StageManager.Instance.dungeonController.tileSettings.container.gameObject.SetActive(true);
                    }
                    PauseController.Instance.pauseEnabled = true;
                    StageManager.Instance.dungeonController.PlayBGM();
                    state = returnState;
                }
                break;
            case defeatedState://20 //Defeated
                PauseController.Instance.pauseEnabled = false;
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 1.5f) {
                    if (zanki >= 0) {
                        SceneChange.Instance.StartEyeCatch(false);
                        elapsedTime = 0f;
                        retryFlag = true;
                        state = readyingState;
                    } else {
                        if (textInstance) {
                            Destroy(textInstance);
                        }
                        textInstance = Instantiate(gameoverTextPrefab, PauseController.Instance.offPauseCanvas.transform);
                        textInstance.GetComponent<FadeOutText>().SetText(TextManager.Get("AMUSEMENT_GAMEOVER"), new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), 5f, -1f, 0);
                        cheatTalkFlag = true;
                        state += 1;
                        elapsedTime = 0f;
                    }
                }
                break;
            case 21://GameOver
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 4f && pauseWait <= 0 && !PauseController.Instance.pauseGame && !PauseController.Instance.IsPhotoPausing) {
                    SceneChange.Instance.StartEyeCatch(false);
                    if (textInstance) {
                        Destroy(textInstance);
                    }
                    state += 1;
                    elapsedTime = 0f;
                }
                break;
            case 22:
                if (SceneChange.Instance.GetEyeCatch()) {
                    PauseController.Instance.pauseEnabled = true;
                    CharacterManager.Instance.pCon.SetNowHP(CharacterManager.Instance.pCon.GetMaxHP());
                    CharacterManager.Instance.ResetGuts();
                    state = returnState;
                }
                break;
            case 100:
                if (CameraManager.Instance) {
                    CameraManager.Instance.SetEventTimer(1.5f);
                }
                if (CharacterManager.Instance.pCon) {
                    CharacterManager.Instance.pCon.ForceStopForEvent(1.5f);
                }
                saveSlot = -1;
                answer = SaveController.Instance.SaveControlExternal();
                if (answer >= 0 && answer < GameManager.saveSlotMax) {
                    saveSlot = answer;
                    state += 1;
                } else if (answer < -1) {
                    PauseController.Instance.CancelChoices();
                    state = 0;
                }
                break;
            case 101: //Back
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
                        CharacterManager.Instance.ResetForMoveStage();
                        CharacterManager.Instance.SetForParkman(false);
                        CharacterManager.Instance.SetNewPlayer(CharacterManager.Instance.GetOriginallyPlayerIndex);
                        StageManager.Instance.dungeonMother.MoveFloor(returnedFloorNum, saveSlot, false);
                        if (CharacterManager.Instance.playerTrans) {
                            if (GameManager.Instance.save.config[GameManager.Save.configID_GenerateHomeObjects] != 0) {
                                CharacterManager.Instance.playerTrans.position = returnedPlayerPos;
                                CharacterManager.Instance.playerTrans.eulerAngles = returnedPlayerRot;
                            } else {
                                CharacterManager.Instance.playerTrans.position = returnedPlayerPosNHO;
                                CharacterManager.Instance.playerTrans.eulerAngles = returnedPlayerRotNHO;
                            }
                            if (clearedFlag) {
                                bool isParenting = (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.itemSettings.container);
                                ItemDatabase.Instance.GiveItem(rewardItemIndex, (GameManager.Instance.save.config[GameManager.Save.configID_GenerateHomeObjects] != 0 ? returnedPlayerPos : returnedPlayerPosNHO) + Vector3.up, 5f, 1.2f, 1.2f, 1.2f, isParenting ? StageManager.Instance.dungeonController.itemSettings.container : null);
                            }
                            if (CameraManager.Instance) {
                                CameraManager.Instance.ResetCameraFixPos();
                            }
                        }
                        ReadyForRestoreFriends();
                        CharacterManager.Instance.RestoreFriends();
                    }
                    if (scoreTopFlag) {
                        Instantiate(redFoxSpecialTalk);
                    } else if (cheatTalkFlag) {
                        Instantiate(redFoxCheatTalk);
                    }
                    SceneChange.Instance.EndEyeCatch();
                }
                break;
            case 102: //ReturnToTitle
                if (SceneChange.Instance.GetEyeCatch()) {
                    if (textInstance) {
                        Destroy(textInstance);
                    }
                    PauseController.Instance.CancelChoices();
                    StageManager.Instance.CleaningObjects();
                    CharacterManager.Instance.ResetForMoveStage();
                    CharacterManager.Instance.ResetForMoveFloor();
                    if (CharacterManager.Instance.playerObj && CharacterManager.Instance.playerTrans && StageManager.Instance.dungeonController) {
                        CharacterManager.Instance.playerObj.SetActive(false);
                        CharacterManager.Instance.playerTrans.position = StageManager.Instance.dungeonController.fixSettings.player.position;
                        CharacterManager.Instance.playerTrans.eulerAngles = StageManager.Instance.dungeonController.fixSettings.player.rotation;
                        CharacterManager.Instance.playerObj.SetActive(true);
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
            default:
                break;
        }
    }

    Transform SetContainer(string objName) {
        GameObject instance = new GameObject(objName);
        instance.transform.position = transform.position;
        instance.transform.SetParent(transform);
        return instance.transform;
    }

    void SetTextScore() {
        if (cheatFlag) {
            CharacterManager.Instance.parkmanSettings.score.text = sb.Clear().Append("<s>").Append(score).Append("</s>").ToString();
        } else {
            CharacterManager.Instance.parkmanSettings.score.text = score.ToString();
        }
    }

    void ReadyLevel(bool isRetry) {
        DungeonGenerator_ReadFile generator = StageManager.Instance.dungeonController.GetComponent<DungeonGenerator_ReadFile>();
        int width = generator.width;
        int height = generator.height;
        int enemyIndex = 0;
        comboCount = 0;
        IsLightActive = false;
        gameOverFlag = false;
        defeatScoreDisabled = false;
        if (level < lightingIndex.Length && LightingDatabase.Instance.nowLightingNumber != lightingIndex[level]) {
            LightingDatabase.Instance.SetLighting(lightingIndex[level]);
            CharacterManager.Instance.SetPlayerLightActive();
        }
        if (level < noMapLevel) {
            StageManager.Instance.SetAllMapChipActivate(false);
        } else {
            StageManager.Instance.SetAllMapChipDeactivate();
        }
        if (isRetry) {
            if (coinInstance) {
                Destroy(coinInstance);
            }
            for (int i = 0; i < enemyMax; i++) {
                if (enemyBaseArray[i] != null) {
                    Destroy(enemyBaseArray[i].gameObject);
                    SetEnemy(i, enemyPosArray[i], i * 5f);
                }
            }
            if (enemyBossBase != null) {
                Destroy(enemyBossBase.gameObject);
                SetBoss(enemyBossPos);
            }
        } else {
            coinPosList.Clear();
            ballCount = 0;
            ballMax = 0;
            if (container && container.gameObject) {
                Destroy(container.gameObject);
            }
            container = SetContainer("Container");
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    Vector3 pos = StageManager.Instance.dungeonController.GetTilePosition(i, j);
                    switch (generator.mapMod[i, j]) {
                        case 0:
                            break;
                        case ballMod:
                            Instantiate(ballPrefab, pos, quaIden, container).GetComponent<Amusement_ParkmanBall>().eventParent = this;
                            ballMax++;
                            break;
                        case japaribunMod:
                            Instantiate(japaribunPrefab, pos, quaIden, container).GetComponent<Amusement_ParkmanBall>().eventParent = this;
                            break;
                        case blockMod:
                            Instantiate(blockPrefab, pos, quaIden, container);
                            break;
                        case coinMod:
                            coinPosList.Add(pos);
                            break;
                        case warpMod:
                            if (level < lavaLevel) {
                                Instantiate(warpPrefab, pos, quaIden, container);
                            }
                            break;
                        case lightMod:
                            if (level >= passageDisappearLevel) {
                                Instantiate(lightPrefab, pos, quaIden, container).GetComponent<Amusement_ParkmanLight>().eventParent = this;
                            }
                            Instantiate(ballPrefab, pos, quaIden, container).GetComponent<Amusement_ParkmanBall>().eventParent = this;
                            ballMax++;
                            break;
                        case lavaMod:
                            if (level >= lavaLevel) {
                                Instantiate(lavaPrefab, pos, quaIden, container);
                            }
                            break;
                        case enemyMod:
                            if (enemyIndex < enemyMax) {
                                SetEnemy(enemyIndex, pos, enemyIndex * 5);
                                enemyPosArray[enemyIndex] = pos;
                                enemyIndex++;
                            }
                            break;
                        case teleportPointMod:
                            Instantiate(teleportPointPrefab, pos, quaIden, container);
                            break;
                    }
                }
            }
            if (level >= bossLevel) {
                enemyBossPos = Vector3.zero;
                for (int i = 0; i < enemyPosArray.Length; i++) {
                    enemyBossPos += enemyPosArray[i];
                }
                enemyBossPos /= enemyPosArray.Length;
                SetBoss(enemyBossPos);
            }
        }
        CharacterManager.Instance.parkmanSettings.onObj[0].SetActive(true);
        SetTextScore();
        CharacterManager.Instance.parkmanSettings.zanki.text = Mathf.Max(0, zanki).ToString();
        CharacterManager.Instance.parkmanSettings.ball.text = sb.Clear().AppendFormat("{0} / {1}", ballCount, ballMax).ToString();
        CharacterManager.Instance.playerTrans.position = StageManager.Instance.dungeonController.fixSettings.player.position;
        CharacterManager.Instance.playerTrans.eulerAngles = StageManager.Instance.dungeonController.fixSettings.player.rotation;
        CharacterManager.Instance.pCon.maxSpeed = playerSpeed[Mathf.Clamp(level - 1, 0, playerSpeed.Length - 1)];
        CharacterManager.Instance.pCon.acceleration = playerSpeed[Mathf.Clamp(level - 1, 0, playerSpeed.Length - 1)] * 2f;
        if (!retryFlag) {
            CharacterManager.Instance.BossTimeHide();
            CharacterManager.Instance.BossTimeInit();
        }
        if (CameraManager.Instance) {
            CameraManager.Instance.ResetCameraFixPos();
        }
        if (textInstance) {
            Destroy(textInstance);
        }
        textInstance = Instantiate(levelTextPrefab, PauseController.Instance.offPauseCanvas.transform);
        textInstance.GetComponent<FadeOutText>().SetText(sb.Clear().Append(TextManager.Get("WORD_LEVEL")).Append(" ").Append(level).ToString(), new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), 1.5f, -1f, 0);
    }

    void SetEnemy(int index, Vector3 position, float waitTime) {
        Enemy_ParkmanCellien enemyTemp = Instantiate(enemyPrefab, position, quaIden, container).GetComponent<Enemy_ParkmanCellien>();
        float speed = enemySpeed[Mathf.Clamp(level - 1, 0, enemySpeed.Length - 1)];
        enemyTemp.matColor = index;
        enemyTemp.waitTimeRemain = waitTime;
        enemyTemp.eventParent = this;
        enemyTemp.maxSpeed = speed;
        enemyTemp.walkSpeed = speed;
        enemyTemp.acceleration = speed * 2f;
        enemyTemp.angularSpeed = 270f * (speed / enemySpeed[0]);
        enemyBaseArray[index] = enemyTemp;
    }

    void SetBoss(Vector3 position) {
        Enemy_ParkmanBoss enemyTemp = Instantiate(bossPrefab, position, quaIden, container).GetComponent<Enemy_ParkmanBoss>();
        float speed = enemySpeed[Mathf.Clamp(level - 1, 0, enemySpeed.Length - 1)];
        enemyTemp.eventParent = this;
        enemyTemp.maxSpeed = speed;
        enemyTemp.walkSpeed = speed;
        enemyTemp.acceleration = speed * 2f;
        enemyTemp.angularSpeed = 270f * (speed / enemySpeed[0]);
        enemyBossBase = enemyTemp;
    }

    public void ReadyForRestoreFriends() {
        for (int i = 0; i < friendsSave.Length; i++) {
            GameManager.Instance.save.friendsLiving[i] = friendsSave[i] ? 1 : 0;
        }
    }

    public void DefeatEnemy(Vector3 position) {
        if (!defeatScoreDisabled) {
            int plusTemp = comboScoreArray[Mathf.Clamp(comboCount, 0, comboScoreArray.Length - 1)];
            score += plusTemp;
            CheckExtend(plusTemp);
            SetTextScore();
            comboCount++;
            if (mainCamera || GetMainCamera()) {
                Vector2 pos = RectTransformUtility.WorldToScreenPoint(mainCamera, position);
                if (pos.y < 0f) {
                    pos.y = 0f;
                }
                Instantiate(scoreTextPrefab, CharacterManager.Instance.getExpParent).GetComponent<FadeOutText>().SetText(plusTemp.ToString(), pos, 0f, 3f, 50f);
            }
        }
    }

    void TimeBonus() {
        int timeBonus = Mathf.Clamp(3000 - CharacterManager.Instance.GetBossTime100() / 10, 0, 3000);
        if (timeBonus > 0) {
            if (mainCamera || GetMainCamera()) {
                Vector2 pos = RectTransformUtility.WorldToScreenPoint(mainCamera, CharacterManager.Instance.pCon.GetCenterPosition());
                if (pos.y < 0f) {
                    pos.y = 0f;
                }
                Instantiate(scoreTextPrefab, CharacterManager.Instance.getExpParent).GetComponent<FadeOutText>().SetText(sb.Clear().Append(TextManager.Get("AMUSEMENT_TIMEBONUS")).AppendLine().Append(timeBonus).ToString(), pos, 0f, 3f, 50f);
            }
            score += timeBonus;
            CheckExtend(timeBonus);
            SetTextScore();
        }
    }

    void ClearBonus() {
        if (mainCamera || GetMainCamera()) {
            Vector2 pos = RectTransformUtility.WorldToScreenPoint(mainCamera, CharacterManager.Instance.pCon.GetCenterPosition());
            if (pos.y < 0f) {
                pos.y = 0f;
            }
            Instantiate(scoreTextPrefab, CharacterManager.Instance.getExpParent).GetComponent<FadeOutText>().SetText(sb.Clear().Append(TextManager.Get("AMUSEMENT_CLEARBONUS")).AppendLine().Append(clearBonusNum).ToString(), pos, 0f, 3f, 50f);
        }
        score += clearBonusNum;
        CheckExtend(clearBonusNum);
        SetTextScore();
    }

    public void SetLightActive(bool flag) {
        IsLightActive = flag;
        int levelTemp = (flag ? 0 : level);
        if (level < lightingIndex.Length && LightingDatabase.Instance.nowLightingNumber != lightingIndex[levelTemp]) {
            LightingDatabase.Instance.SetLighting(lightingIndex[levelTemp]);
            CharacterManager.Instance.SetPlayerLightActive();
        }
        if (level < noMapLevel || flag) {
            StageManager.Instance.SetAllMapChipActivate(true);
        } else {
            StageManager.Instance.DeactivateTemporalChip();
        }
    }

    public bool IsPassageVisible => (level < passageDisappearLevel || IsLightActive);
    public bool IsBallVisible => (level < ballDisappearLevel || IsLightActive);

    public void ReceiveItemGet(ItemType itemType, Vector3 position) {
        int scorePlusTemp = 0;
        switch (itemType) {
            case ItemType.Ball:
                scorePlusTemp = 10;
                ballCount++;
                CharacterManager.Instance.parkmanSettings.ball.text = sb.Clear().AppendFormat("{0} / {1}", ballCount, ballMax).ToString();
                if (ballCount == 70 || ballCount == 170) {
                    if (coinInstance) {
                        Destroy(coinInstance);
                    }
                    float maxSqrDist = -1f;
                    int minIndex = -1;
                    int length = coinPosList.Count;
                    for (int i = 0; i < length; i++) {
                        float sqrDistTemp = (CharacterManager.Instance.playerTrans.position - coinPosList[i]).sqrMagnitude;
                        if (sqrDistTemp > maxSqrDist) {
                            maxSqrDist = sqrDistTemp;
                            minIndex = i;
                        }
                    }
                    if (minIndex >= 0) {
                        coinInstance = Instantiate(level >= 5 ? coinBigPrefab : coinPrefab, coinPosList[minIndex], quaIden, container);
                        coinInstance.GetComponent<Amusement_ParkmanBall>().eventParent = this;
                    }
                }
                break;
            case ItemType.Japaribun:
                scorePlusTemp = 50;
                CharacterManager.Instance.pCon.SupermanStart();
                supermanTimeRemain = supermanTimeMax;
                break;
            case ItemType.Coin:
                scorePlusTemp = coinScore[Mathf.Clamp(level - 1, 0, coinScore.Length - 1)];
                if (mainCamera || GetMainCamera()) {
                    Vector2 pos = RectTransformUtility.WorldToScreenPoint(mainCamera, position);
                    if (pos.y < 0f) {
                        pos.y = 0f;
                    }
                    Instantiate(scoreTextPrefab, CharacterManager.Instance.getExpParent).GetComponent<FadeOutText>().SetText(scorePlusTemp.ToString(), pos, 0f, 3f, 50f);
                }
                break;
        }
        score += scorePlusTemp;
        CheckExtend(scorePlusTemp);
        SetTextScore();
    }

    void CheckExtend(int scorePlus) {
        if (!cheatFlag) {
            if (GameManager.Instance.minmiPurple) {
                if (score > GameManager.Instance.save.parkmanScoreHard) {
                    GameManager.Instance.save.parkmanScoreHard = score;
                }
            } else {
                if (score > GameManager.Instance.save.parkmanScore) {
                    GameManager.Instance.save.parkmanScore = score;
                }
            }
            if (score >= redfoxBorder && !scoreTopFlag) {
                scoreTopFlag = true;
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_ParkmanHighScore);
            }
        }
        extendScore += scorePlus;
        if (extendScore >= 10000) {
            extendScore -= 10000;
            zanki += 1;
            CharacterManager.Instance.parkmanSettings.zanki.text = Mathf.Max(0, zanki).ToString();
            if (mainCamera || GetMainCamera()) {
                Vector2 pos = RectTransformUtility.WorldToScreenPoint(mainCamera, CharacterManager.Instance.pCon.GetCenterPosition());
                if (pos.y < 0f) {
                    pos.y = 0f;
                }
                Instantiate(oneupTextPrefab, CharacterManager.Instance.getExpParent).GetComponent<FadeOutText>().SetText(TextManager.Get("AMUSEMENT_LIFEPLUS"), pos, 0f, 4f, 35f);
            }
        }
    }

    void ActivateAlart(bool flag) {
        if (alartSave != flag) {
            alartSave = flag;
            alartingTime = 0f;
            alartColor.a = 0f;
            CharacterManager.Instance.parkmanSettings.alert.color = alartColor;
            if (alartSave) {
                alartAudio.Play();
            } else {
                alartAudio.Stop();
            }
        }
    }

    private void OnDestroy() {
        if (textInstance) {
            Destroy(textInstance);
        }
    }
}
