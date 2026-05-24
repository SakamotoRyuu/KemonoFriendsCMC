using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_LastBattle : SingletonMonoBehaviour<Event_LastBattle> {

    public int lightingNumber1;
    public GameObject[] activateObj1;
    public GameObject[] deactivateObj1;
    public Transform playerPivot1;
    public int nextBattleFloor;

    int progress;
    float elapsedTime;
    Enemy_Empress[] empressBase = new Enemy_Empress[4];
    int stageFloor;
    bool sandstarRawSave;
    int kabanTalked = -1;
    int anotherServalTalked = -1;
    int empressCount;
    const int kabanID = 1;
    const int anotherServalID = 31;
    static readonly Vector3 battleMapPosition = new Vector3(0f, 200f, 0f);
    const float battleMapRotation = 180f;
    const float battleMapSize = 60f;

    void NextProgress() {
        progress++;
        elapsedTime = 0f;
    }

    public void BattleReady(Enemy_Empress eBase) {
        if (empressCount < empressBase.Length) {
            empressBase[empressCount] = eBase;
        }
        if (empressCount == 0) {
            NextProgress();
        }
        empressCount++;
    }

    private void Update() {
        elapsedTime += Time.deltaTime;
        switch (progress) {
            case 0:
                break;
            case 1:
                if (PauseController.Instance) {
                    PauseController.Instance.SetBlackCurtain(elapsedTime * 5f, true);
                }
                if (elapsedTime >= 0.2f) {
                    for (int i = 0; i < activateObj1.Length; i++) {
                        if (activateObj1[i]) {
                            activateObj1[i].SetActive(true);
                        }
                    }
                    for (int i = 0; i < deactivateObj1.Length; i++) {
                        if (deactivateObj1[i]) {
                            deactivateObj1[i].SetActive(false);
                        }
                    }
                    if (LightingDatabase.Instance) {
                        LightingDatabase.Instance.SetLighting(lightingNumber1);
                    }
                    if (CharacterManager.Instance && CharacterManager.Instance.playerTrans && playerPivot1) {
                        CharacterManager.Instance.playerTrans.SetPositionAndRotation(playerPivot1.position, playerPivot1.rotation);
                        CharacterManager.Instance.WarpToPlayerPosAll(false, false);
                    }
                    for (int i = 0; i < empressBase.Length; i++) {
                        if (empressBase[i]) {
                            empressBase[i].BattleStartExternal();
                        }
                    }
                    if (StageManager.Instance && StageManager.Instance.dungeonController && CharacterManager.Instance) {
                        StageManager.Instance.dungeonController.mapCameraPosition = battleMapPosition;
                        StageManager.Instance.dungeonController.mapCameraRotation = battleMapRotation;
                        StageManager.Instance.dungeonController.mapCameraSize = battleMapSize;
                        CharacterManager.Instance.SetMapCamera(battleMapPosition, battleMapRotation, battleMapSize);
                    }
                    NextProgress();
                }
                break;
            case 2:
                if (PauseController.Instance) {
                    PauseController.Instance.SetBlackCurtain(Mathf.Clamp01(1f - elapsedTime * 4f), true);
                }
                if (elapsedTime >= 0.25f) {
                    PauseController.Instance.SetBlackCurtain(0f, false);
                    NextProgress();
                }
                break;
            case 3:
                if (MessageUI.Instance.GetMessageCount(1) == 0) {
                    if (kabanTalked < 0 && CharacterManager.Instance.GetFriendsExist(kabanID, true)) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_KABAN_LAST_00", kabanID, -1);
                        kabanTalked = 0;
                    } else if (anotherServalTalked < 0 && CharacterManager.Instance.GetFriendsExist(anotherServalID, true)) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_LAST_00", anotherServalID, -1);
                        anotherServalTalked = 0;
                    }
                }
                break;
            case 4:
                if (StageManager.Instance == null || StageManager.Instance.IsHomeStage) {
                    Destroy(gameObject);
                } else if (StageManager.Instance.dungeonController && stageFloor != StageManager.Instance.stageNumber * 100 + StageManager.Instance.floorNumber && !GameManager.Instance.IsPlayerAnother) {
                    if (sandstarRawSave) {
                        StageManager.Instance.ActivateSandstarRaw();
                    }
                    NextProgress();
                }
                break;
            case 5:
                Destroy(gameObject);
                break;
        }
    }

    public void MoveNextBattle(bool sandstarRaw) {
        DontDestroyOnLoad(gameObject);
        if (PauseController.Instance.IsPhotoPausing) {
            PauseController.Instance.PausePhoto(false);
        }
        sandstarRawSave = sandstarRaw;
        stageFloor = StageManager.Instance.stageNumber * 100 + StageManager.Instance.floorNumber;
        StageManager.Instance.dungeonMother.MoveFloor(nextBattleFloor, -1, false);
        // CameraManager.Instance.SetEventTimer(0.01f);
        CharacterManager.Instance.ForceStopForEventAll(3f);
        progress = 4;
    }

    private void OnDestroy() {
        if (PauseController.Instance && progress <= 3) {
            PauseController.Instance.SetBlackCurtain(0f, false);
        }
    }

}
