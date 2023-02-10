using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Event_LastBattleSecond : SingletonMonoBehaviour<Event_LastBattleSecond> {

    public AudioSource quakeSFX;
    public AudioSource heartSFX;
    public Transform[] eventCamPivot;
    public GameObject textPrefab;
    public Vector2[] textPosition;
    public bool eventNow;
    public AudioSource slashSFX;
    public RuntimeAnimatorController eventAnim;
    public int newMusicNumber;
    public GameObject obstacleForRemove;
    public int innerCoreLightingNumber;
    public int innerCoreAmbientNumber;
    public GameObject innerCorePrefab;
    public GameObject sphericLavaPrefab;
    public GameObject gameoverTimer;
    public GameObject eventBackImagePrefab;

    int progress;
    float elapsedTime;
    Enemy_ImperatrixMundi[] bossBaseSave = new Enemy_ImperatrixMundi[4];
    int bossCount;
    int depthSave;
    const int sandstarEquipStart = 24;
    const int sandstarEquipEnd = 32;
    GameObject[] textInstance = new GameObject[4];
    int lockonSave;
    int servalTalked = -1;
    int kabanTalked = 0;
    int anotherServalTalked = 0;
    bool servalWithKabanTalked;
    bool servalWithAnotherTalked;
    GameObject eventBackImageInstance;
    bool eyeOpened;
    const int kabanID = 1;
    const int anotherServalID = 31;
    const float mapRadius = 27f;
    int jumpMessageIndex = -1;
    float jumpMessageLifeTime;
    const float infinityJumpHeight = 14.5f;

    void NextProgress() {
        progress++;
        elapsedTime = 0f;
    }

    private void Update() {
        elapsedTime += Time.deltaTime;
        switch (progress) {
            case 0:
                PauseController.Instance.SetBlackCurtain(1f, false);
                CharacterManager.Instance.ForceStopForEventAll(20f);
                for (int i = 0; i < CharacterManager.Instance.friends.Length; i++) {
                    if (CharacterManager.Instance.GetFriendsExist(i, false)) {
                        CharacterManager.Instance.friends[i].trans.rotation = CharacterManager.Instance.playerTrans.rotation;
                    }
                }
                NextProgress();
                break;
            case 1:
                PauseController.Instance.SetBlackCurtain(1f, false);
                CameraManager.Instance.SetEventCamera(eventCamPivot[0].position, eventCamPivot[0].eulerAngles, 10f, 0f, 15f);
                NextProgress();
                break;
            case 2:
                if (elapsedTime > 0.25f) {
                    CameraManager.Instance.SetQuake(transform.position, 1, 8, 0f, 5f, 2f, 100f, 200f);
                    if (quakeSFX) {
                        quakeSFX.Play();
                    }
                    PauseController.Instance.SetBlackCurtain(0f, false);
                    NextProgress();
                }
                break;
            case 3:
                break;
            case 10:
                if (elapsedTime > 6f) {
                    CharacterManager.Instance.ReleaseStopForEventAll();
                    CameraManager.Instance.SetEventTimer(0.005f);
                    for (int i = 0; i < bossBaseSave.Length; i++) {
                        if (bossBaseSave[i]) {
                            bossBaseSave[i].BattleStartExternal();
                        }
                    }
                    NextProgress();
                }
                break;
            case 11:
                if (MessageUI.Instance.GetMessageCount(MessageUI.panelType_Speech) == 0) {
                    if (kabanTalked < 1 && CharacterManager.Instance.GetFriendsExist(kabanID, true)) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_KABAN_LAST_01", kabanID, -1);
                        kabanTalked = 1;
                    } else if (anotherServalTalked < 1 && CharacterManager.Instance.GetFriendsExist(anotherServalID, true)) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_LAST_01", anotherServalID, -1);
                        anotherServalTalked = 1;
                    }
                }
                break;
            case 20:
            case 21:
            case 22:
            case 23:
                if (elapsedTime >= 1f) {
                    textInstance[progress - 20] = Instantiate(textPrefab, PauseController.Instance.blackCurtain.canvas.transform);
                    textInstance[progress - 20].GetComponent<RectTransform>().anchoredPosition = textPosition[progress - 20];
                    textInstance[progress - 20].GetComponent<TMP_Text>().text = TextManager.Get("EVENT_LASTBATTLE_" + (progress - 20).ToString("00"));
                    textInstance[progress - 20].transform.SetAsLastSibling();
                    NextProgress();
                }
                break;
            case 24:
                if (elapsedTime >= 2.5f) {
                    for (int i = sandstarEquipStart; i < sandstarEquipEnd; i++) {
                        GameManager.Instance.save.weapon[i] = 1;
                        GameManager.Instance.save.equip[i] = 1;
                    }
                    CharacterManager.Instance.AddSandstar(100, true);
                    CharacterManager.Instance.SetNewPlayer(CharacterManager.playerIndexHyper);
                    CharacterManager.Instance.pCon.SetAnotherAnimatorController(true, eventAnim);
                    CharacterManager.Instance.ForceStopForEventAll(20f);
                    CharacterManager.Instance.playerTrans.position = StageManager.Instance.dungeonController.fixSettings.player.position;
                    CharacterManager.Instance.playerTrans.eulerAngles = StageManager.Instance.dungeonController.fixSettings.player.rotation;
                    CharacterManager.Instance.pCon.SupermanStart(false);
                    for (int i = 0; i < bossBaseSave.Length; i++) {
                        if (bossBaseSave[i]) {
                            bossBaseSave[i].SunEventVolumeIncrease(i == 0 ? 1f : 0f);
                            CameraManager.Instance.SetEventCamera(eventCamPivot[2].position, eventCamPivot[2].eulerAngles, 20f, 0f, 15f);
                        }
                    }
                    if (obstacleForRemove) {
                        obstacleForRemove.transform.position = StageManager.Instance.dungeonController.fixSettings.player.position;
                        obstacleForRemove.SetActive(true);
                    }
                    NextProgress();
                }
                break;
            case 25:
                CharacterManager.Instance.AddSandstar(100, true);
                if (elapsedTime >= 3.8f) {
                    CharacterManager.Instance.pCon.Event_LastBattleSecond();
                    progress++;
                }
                break;
            case 26:
                CharacterManager.Instance.AddSandstar(100, true);
                if (elapsedTime >= 4f) {
                    for (int i = 0; i < bossBaseSave.Length; i++) {
                        if (bossBaseSave[i]) {
                            bossBaseSave[i].SunEventVolumeStop();
                        }
                    }
                    if (slashSFX) {
                        slashSFX.Play();
                    }
                    if (eventBackImageInstance) {
                        Destroy(eventBackImageInstance);
                    }
                    for (int i = 0; i < textInstance.Length; i++) {
                        if (textInstance[i]) {
                            Destroy(textInstance[i]);
                        }
                    }
                    NextProgress();
                }
                break;
            case 27:
                if (elapsedTime >= 0.3f) {
                    CharacterManager.Instance.pCon.SetAttackSpeedExternal(0f);
                }
                CharacterManager.Instance.AddSandstar(100, true);
                if (elapsedTime > 0.75f) {
                    PauseController.Instance.SetBlackCurtain(0f, false);
                    Time.timeScale = 0.5f;
                    NextProgress();
                }
                break;
            case 28:
                CharacterManager.Instance.AddSandstar(100, true);
                if (elapsedTime > 0.75f) {
                    for (int i = 0; i < bossBaseSave.Length; i++) {
                        if (bossBaseSave[i]) {
                            bossBaseSave[i].SunEventBreakSun();
                        }
                    }
                    CharacterManager.Instance.pCon.SetAttackSpeedExternal(1f);
                    CharacterManager.Instance.pCon.SetFaceString("SeriousBlink", 10f);
                    NextProgress();
                }
                break;
            case 29:
                CharacterManager.Instance.AddSandstar(100, true);
                if (elapsedTime > 1.75f && BGM.Instance.GetPlayingIndex() < 0) {
                    BGM.Instance.Play(newMusicNumber, 0f);
                }
                if (elapsedTime > 2f) {
                    if (BGM.Instance.GetPlayingIndex() < 0) {
                        BGM.Instance.Play(newMusicNumber, 0f);
                    }
                    GameManager.Instance.ChangeTimeScale(false);
                    eventCamPivot[3].position = CharacterManager.Instance.playerTrans.position + CharacterManager.Instance.playerTrans.forward * 0.45f + Vector3.up * 0.1f;
                    eventCamPivot[3].LookAt(CharacterManager.Instance.playerTrans.position);
                    CameraManager.Instance.CancelQuake();
                    CameraManager.Instance.SetEventCameraFollowTarget(eventCamPivot[3], 8f, 1.5f, 0.45f);
                    NextProgress();
                }
                break;
            case 30:
                CharacterManager.Instance.AddSandstar(100, true);
                Vector3 posTemp = eventCamPivot[3].position;
                posTemp.y += Time.deltaTime * 0.25f;
                if (posTemp.y > CharacterManager.Instance.playerTrans.position.y + 1.15f) {
                    posTemp.y = CharacterManager.Instance.playerTrans.position.y + 1.15f;
                    if (!eyeOpened) {
                        CharacterManager.Instance.pCon.SetFaceString("Serious", 5f);
                        eyeOpened = true;
                    }
                }
                eventCamPivot[3].position = posTemp;
                if (elapsedTime > 6.5f) {
                    CameraManager.Instance.SetEventTimer(0.002f);
                    CharacterManager.Instance.pCon.SetAnotherAnimatorController(false, null);
                    CharacterManager.Instance.ReleaseStopForEventAll();
                    for (int i = 0; i < bossBaseSave.Length; i++) {
                        if (bossBaseSave[i]) {
                            bossBaseSave[i].SunEventBattleRestart();
                        }
                    }
                    if (obstacleForRemove) {
                        obstacleForRemove.SetActive(false);
                    }
                    RestoreDOF();
                    PauseController.Instance.pauseEnabled = true;
                    if (MessageUI.Instance) {
                        jumpMessageIndex = MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_INFINITEJUMP"), MessageUI.time_Infinity, MessageUI.panelType_Information, MessageUI.slotType_Important);
                        jumpMessageLifeTime = MessageUI.time_Important;
                    }
                    if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.AllWild) != 0) {
                        for (int i = 0; i < GameManager.friendsMax; i++) {
                            if (CharacterManager.Instance.GetFriendsExist(i, true)) {
                                CharacterManager.Instance.friends[i].fBase.SupermanStart(false);
                            }
                        }
                    }
                    CharacterManager.Instance.BossTimeStart();
                    eventNow = false;
                    NextProgress();
                }
                break;
            case 31:
                if (MessageUI.Instance.GetMessageCount(MessageUI.panelType_Speech) == 0) {
                    bool kabanLiving = CharacterManager.Instance.GetFriendsExist(kabanID, true);
                    bool anotherServalLiving = CharacterManager.Instance.GetFriendsExist(anotherServalID, true);
                    if (servalTalked < 0) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_SERVAL_LAST_00", -1, -1);
                        servalTalked = 0;
                    } else if (kabanTalked < 2 && kabanLiving) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_KABAN_LAST_02", kabanID, -1);
                        kabanTalked = 2;
                    } else if (servalWithKabanTalked == false && kabanTalked == 2 && kabanLiving) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_SERVAL_LAST_01", -1, -1);
                        servalWithKabanTalked = true;
                    } else if (anotherServalTalked < 2 && anotherServalLiving) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_LAST_02", anotherServalID, -1);
                        anotherServalTalked = 2;
                    } else if (servalWithAnotherTalked == false && anotherServalTalked == 2 && anotherServalLiving) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_SERVAL_LAST_02", -1, -1);
                        servalWithAnotherTalked = true;
                    } else if (kabanTalked < 3 && kabanLiving) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_KABAN_LAST_03", kabanID, -1);
                        kabanTalked = 3;
                    } else if (anotherServalTalked < 3 && anotherServalLiving) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_LAST_03", anotherServalID, -1);
                        anotherServalTalked = 3;
                    } else if (servalTalked < 3) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_SERVAL_LAST_03", -1, -1);
                        servalTalked = 3;
                    }
                }
                break;
            case 40:
                PauseController.Instance.SetBlackCurtain(Mathf.Clamp01(1.05f - elapsedTime), true);
                if (elapsedTime >= 1.05f) {
                    NextProgress();
                }
                break;
            case 41:
                if (elapsedTime >= 2.5f) {
                    GameObject goalInstance = StageManager.Instance.dungeonController.GetGoalInstance();
                    if (goalInstance) {
                        goalInstance.SetActive(true);
                    }
                    Ambient.Instance.Play(innerCoreAmbientNumber, 1f);
                    if (sphericLavaPrefab) {
                        Instantiate(sphericLavaPrefab, transform);
                    }
                    RandomQuake randomQuake = StageManager.Instance.dungeonController.GetComponent<RandomQuake>();
                    if (randomQuake) {
                        randomQuake.enabled = true;
                        randomQuake.SetQuake(1f, 0.5f, 0f);
                    }
                    if (gameoverTimer) {
                        Instantiate(gameoverTimer, StageManager.Instance.dungeonMother.transform);
                    }
                    NextProgress();
                }
                break;
            case 42:
                if (MessageUI.Instance.GetMessageCount(MessageUI.panelType_Speech) == 0) {
                    if (servalTalked < 5) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_SERVAL_LAST_05", -1, -1);
                        servalTalked = 5;
                    } else if (kabanTalked < 4 && CharacterManager.Instance.GetFriendsExist(kabanID, true)) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_KABAN_LAST_04", kabanID, -1);
                        kabanTalked = 4;
                    } else if (anotherServalTalked < 4 && CharacterManager.Instance.GetFriendsExist(anotherServalID, true)) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_LAST_04", anotherServalID, -1);
                        anotherServalTalked = 4;
                    }
                }
                break;
        }
        if (jumpMessageLifeTime > 0f) {
            jumpMessageLifeTime -= Time.deltaTime;
        }
        if (jumpMessageIndex >= 0 && jumpMessageLifeTime <= 0f && MessageUI.Instance && ((CharacterManager.Instance.playerTrans && CharacterManager.Instance.playerTrans.position.y >= infinityJumpHeight) || progress >= 40)) {
            MessageUI.Instance.DestroySpecificMessage(jumpMessageIndex);
            jumpMessageIndex = -1;
        }
    }

    public void ImperatrixHatching(Enemy_ImperatrixMundi bossBase) {
        if (bossCount < bossBaseSave.Length) {
            bossBaseSave[bossCount] = bossBase;
        }
        if (bossCount == 0) {
            CameraManager.Instance.SetEventCameraTweening(eventCamPivot[1].position, eventCamPivot[1].eulerAngles, 20f, 1f, 25f);
            CameraManager.Instance.CancelQuake();
            if (quakeSFX) {
                quakeSFX.Stop();
            }
            progress = 10;
            elapsedTime = 0f;
        }
        bossCount++;
    }

    void DisableDOF() {
        depthSave = GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField];
        if (depthSave != 0) {
            GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] = 0;
            CameraManager.Instance.CheckDepthTextureMode();
            InstantiatePostProcessingProfile.Instance.QualitySettingsAdjustments();
        }
        lockonSave = CharacterManager.Instance.autoAim;
        if (lockonSave != 0) {
            CharacterManager.Instance.autoAim = 0;
            CharacterManager.Instance.pCon.searchArea[0].SetUnlockTarget();
        }
        if (CanvasCulling.Instance) {
            CanvasCulling.Instance.CheckConfig(CanvasCulling.indexGauge, 0);
        }
    }

    void RestoreDOF() {
        if (depthSave != 0) {
            GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] = depthSave;
            CameraManager.Instance.CheckDepthTextureMode();
            InstantiatePostProcessingProfile.Instance.QualitySettingsAdjustments();
        }
        if (lockonSave != 0) {
            CharacterManager.Instance.autoAim = lockonSave;
        }
        if (CanvasCulling.Instance) {
            CanvasCulling.Instance.CheckConfig();
        }
    }

    void WarpFriends(Vector3 centerPos, float radius) {
        Vector3 secondCenterPos = transform.position;
        for (int i = 0; i < CharacterManager.Instance.friends.Length; i++) {
            if (CharacterManager.Instance.GetFriendsExist(i, true)) {
                bool circleHit = false;
                for (int count = 0; count < 10; count++) {
                    Vector2 randCircle = Random.insideUnitCircle * radius;
                    Vector3 randPos = centerPos;
                    randPos.x += randCircle.x;
                    randPos.z += randCircle.y;
                    if ((randPos - secondCenterPos).sqrMagnitude < mapRadius * mapRadius) {
                        CharacterManager.Instance.friends[i].fBase.Warp(randPos, 0, 0);
                        circleHit = true;
                        break;
                    }
                }
                if (!circleHit) {
                    CharacterManager.Instance.friends[i].fBase.Warp(centerPos, 0, 0);
                }
            }
        }
    }

    public void ImperatrixSunStart(Transform camPivot) {
        if (!eventNow) {
            if (StageManager.Instance) {
                StageManager.Instance.DestroyTaggedObjects("Trap");
                StageManager.Instance.DestroyTaggedObjects("Projectile");
            }
            PauseController.Instance.pauseEnabled = false;
            DisableDOF();
            CharacterManager.Instance.BossTimeStop();
            CameraManager.Instance.SetEventCamera(camPivot.position, camPivot.eulerAngles, 20f, 0f, 15f);
            CharacterManager.Instance.ForceStopForEventAll(20f);
            CharacterManager.Instance.pCon.SupermanEnd(false);
            CharacterManager.Instance.playerTrans.position = StageManager.Instance.dungeonController.fixSettings.player.position;
            CharacterManager.Instance.playerTrans.eulerAngles = StageManager.Instance.dungeonController.fixSettings.player.rotation;
            // CharacterManager.Instance.WarpToCirclePosAll(CharacterManager.Instance.playerTrans.position, 3f + CharacterManager.Instance.GetFriendsCount(false) * 0.25f, false);
            WarpFriends(CharacterManager.Instance.playerTrans.position, 3f + CharacterManager.Instance.GetFriendsCount(false) * 0.2f);
            CharacterManager.Instance.StopFriends();
            for (int i = 0; i < CharacterManager.Instance.friends.Length; i++) {
                if (CharacterManager.Instance.GetFriendsExist(i, true)) {
                    if (CharacterManager.Instance.friends[i].trans) {
                        CharacterManager.Instance.friends[i].trans.rotation = CharacterManager.Instance.playerTrans.rotation;
                    }
                }
            }
            eventNow = true;
        }
    }

    public void ImperatrixSunMiddle(Transform camPivot) {
        CameraManager.Instance.SetEventCameraTweening(camPivot.position, camPivot.eulerAngles, 10f, 3f, 25f);
        BGM.Instance.StopFade(3f);
        CameraManager.Instance.SetQuake(transform.position, 1f, 8, 5f, 5f, 2f, 100f, 200f);
    }

    public void ImperatrixSunBlackout() {
        if (progress < 20) {
            CameraManager.Instance.CancelQuake();
            PauseController.Instance.SetBlackCurtain(1f, false);
            if (heartSFX) {
                heartSFX.Play();
            }
            if (eventBackImagePrefab) {
                eventBackImageInstance = Instantiate(eventBackImagePrefab, PauseController.Instance.blackCurtain.canvas.transform);
                eventBackImageInstance.transform.SetAsLastSibling();
            }
            progress = 20;
            elapsedTime = 1f;
        }
    }

    public void DefeatTalk() {
        CharacterManager.Instance.SetSpecialChat("EVENT_SERVAL_LAST_04", -1, -1);
        servalTalked = 4;
    }

    public void BattleEnd() {
        PauseController.Instance.SetBlackCurtain(1f, true);
        LightingDatabase.Instance.SetLighting(innerCoreLightingNumber);
        if (innerCorePrefab) {
            Instantiate(innerCorePrefab, transform);
        }
        if (StageManager.Instance.dungeonMother) {
            StageManager.Instance.dungeonMother.reaperSettings.enemyID = -1;
        }
        progress = 40;
        elapsedTime = 0f;
    }

    private void OnDestroy() {
        for (int i = 0; i < textInstance.Length; i++) {
            if (textInstance[i]) {
                Destroy(textInstance[i]);
            }
        }
        if (PauseController.Instance) {
            PauseController.Instance.pauseEnabled = true;
            PauseController.Instance.SetBlackCurtain(0f, false);
        }
        if (eventBackImageInstance) {
            Destroy(eventBackImageInstance);
        }
        if (jumpMessageIndex >= 0 && MessageUI.Instance) {
            MessageUI.Instance.DestroySpecificMessage(jumpMessageIndex);
            jumpMessageIndex = -1;
        }
    }
}
