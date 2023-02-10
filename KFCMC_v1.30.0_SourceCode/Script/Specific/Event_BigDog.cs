using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_BigDog : MonoBehaviour
{

    public bool slbActedFlag;
    public bool allFriendsActivateFlag;
    public bool collapsedFlag;

    int eventState;
    int totalDamageSave;
    bool[] friendsSave;
    int stageFloor;
    bool sandstarRawSave;
    float elapsedTime;
    GameObject[] enemyObjs;
    bool bigdogRestartedFlag;
    float bossTimeRestartTimeRemain;
    
    public void SetBigDogSave(int totalDamage, bool sandstarRaw) {
        if (PauseController.Instance.IsPhotoPausing) {
            PauseController.Instance.PausePhoto(false);
        }
        GameObject[] itemObjs = GameObject.FindGameObjectsWithTag("Item");
        for (int i = 0; i < itemObjs.Length; i++) {
            ItemCharacter_SLB itemTemp = itemObjs[i].GetComponent<ItemCharacter_SLB>();
            if (itemTemp != null) {
                slbActedFlag = itemTemp.GetActedFlag();
                allFriendsActivateFlag = itemTemp.GetEventFlag();
            }
        }
        totalDamageSave = totalDamage;
        friendsSave = new bool[GameManager.friendsMax];
        for (int i = 0; i < friendsSave.Length; i++) {
            friendsSave[i] = CharacterManager.Instance.GetFriendsExist(i, true);
        }
        sandstarRawSave = sandstarRaw;
        stageFloor = StageManager.Instance.stageNumber * 100 + StageManager.Instance.floorNumber;
        CharacterManager.Instance.bossTimeContinuing = true;
        CharacterManager.Instance.FriendsClearAll(false);
        eventState = 1;
        StageManager.Instance.dungeonMother.MoveFloor(1, -1, true);
        PauseController.Instance.friendsDisabled = true;
        CameraManager.Instance.SetEventTimer(0.01f);
        CharacterManager.Instance.pCon.ForceStopForEvent(3f);
        bossTimeRestartTimeRemain = 3f;
        if (StageManager.Instance && StageManager.Instance.graphCollapseNowFloor) {
            collapsedFlag = StageManager.Instance.graphCollapseNowFloor.collapsedFlag;
        }
    }

    public void DefeatInside(bool sandstarRawEnabled) {
        if (PauseController.Instance.IsPhotoPausing) {
            PauseController.Instance.PausePhoto(false);
        }
        if (!sandstarRawSave && sandstarRawEnabled) {
            totalDamageSave = 0;
            sandstarRawSave = sandstarRawEnabled;
        }
        PauseController.Instance.SetBlackCurtain(1f, true);
        stageFloor = StageManager.Instance.stageNumber * 100 + StageManager.Instance.floorNumber;
        PauseController.Instance.friendsDisabled = false;
        for (int i = 0; i < friendsSave.Length; i++) {
            GameManager.Instance.save.friendsLiving[i] = (allFriendsActivateFlag || friendsSave[i] ? 1 : 0);
        }
        elapsedTime = 0f;
        eventState = 4;
        StageManager.Instance.dungeonMother.MoveFloor(1, -1, true);
        CharacterManager.Instance.RestoreFriends();

    }

    public void BigDogRestart(ref int totalDamage, ref bool sandstarRawEnabled) {
        totalDamage = totalDamageSave;
        sandstarRawEnabled = sandstarRawSave;
        bigdogRestartedFlag = true;
        CharacterManager.Instance.bossTimeContinuing = false;
    }    
    
    private void Update() {
        if (StageManager.Instance == null || StageManager.Instance.IsHomeStage || (eventState >= 6 && bigdogRestartedFlag)) {
            Destroy(gameObject);
        } else if (StageManager.Instance.dungeonController != null) {
            switch (eventState) {
                case 1:
                    if (stageFloor != StageManager.Instance.stageNumber * 100 + StageManager.Instance.floorNumber) {
                        if (sandstarRawSave) {
                            StageManager.Instance.ActivateSandstarRaw();
                        }
                        stageFloor = StageManager.Instance.stageNumber * 100 + StageManager.Instance.floorNumber;
                        elapsedTime = 0f;
                        eventState++;
                    }
                    break;
                case 2:
                    elapsedTime += Time.deltaTime;
                    PauseController.Instance.SetBlackCurtain((4f - elapsedTime) * 0.5f, false);
                    if (elapsedTime >= 4f) {
                        elapsedTime = 0f;
                        eventState++;
                    }
                    break;
                case 3:
                    break;
                case 4:
                    PauseController.Instance.pauseEnabled = false;
                    elapsedTime += Time.deltaTime;
                    PauseController.Instance.SetBlackCurtain(1f - elapsedTime, true);
                    if (elapsedTime >= 1f) {
                        elapsedTime = 0f;
                        eventState++;
                    }
                    break;
                case 5:
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime >= 4f) {
                        if (allFriendsActivateFlag && TrophyManager.Instance) {
                            TrophyManager.Instance.CheckTrophy(TrophyManager.t_BigDogAllFriends, true);
                        }
                        eventState++;
                        Destroy(gameObject);
                    }
                    break;
                case 6:
                    break;
            }
        }
        if (eventState <= 3 && bossTimeRestartTimeRemain > 0f) {
            bossTimeRestartTimeRemain -= Time.deltaTime;
            if (bossTimeRestartTimeRemain <= 0f) {
                CharacterManager.Instance.BossTimeStart();
            }
        }
    }

}
