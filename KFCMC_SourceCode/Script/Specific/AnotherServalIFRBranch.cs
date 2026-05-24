using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotherServalIFRBranch : SingletonMonoBehaviour<AnotherServalIFRBranch> {

    public GameObject sleepingIFR;
    public GameObject[] targetIFR;
    public Transform noHomeObjPos;
    public int awakeIndex;
    public int activatedIndex;

    const int conditionProgress = 11;
    const int sleepingConditionID = 23;
    const int sleepingStageNumber = 11;
    const int friendsID = 31;

    protected override void Awake() {
        base.Awake();
        if (GameManager.Instance.save.progress >= conditionProgress && GameManager.Instance.save.friends.Length > friendsID && StageManager.Instance) {
            if (!CharacterManager.Instance.GetFriendsExist(friendsID, false) && (GameManager.Instance.save.friends[friendsID] == 0 || StageManager.Instance.defeatBossSave == sleepingStageNumber)) {
                if (GameManager.Instance.save.config[GameManager.Save.configID_GenerateHomeObjects] == 0 || GameManager.Instance.save.friends[sleepingConditionID] == 0) {
                    sleepingIFR.transform.position = noHomeObjPos.position;
                }
                sleepingIFR.SetActive(true);
                activatedIndex = -1;
            } else if (GameManager.Instance.save.friends[friendsID] != 0 && GameManager.Instance.save.config[GameManager.Save.configID_GenerateHomeObjects] != 0) {
                int randBase = Random.Range(0, targetIFR.Length);
                for (int i = 0; i < targetIFR.Length; i++) {
                    int index = (randBase + i) % targetIFR.Length;
                    ItemCharacter ifrTemp = targetIFR[index].GetComponent<ItemCharacter>();
                    bool check = true;
                    for (int j = 0; j < ifrTemp.homeConditionId.Length && check; j++) {
                        if (GameManager.Instance.save.friends[ifrTemp.homeConditionId[j]] == 0) {
                            check = false;
                        }
                    }
                    if (check) {
                        targetIFR[index].SetActive(true);
                        activatedIndex = index;
                        break;
                    }
                }
            }
        }
    }

}
