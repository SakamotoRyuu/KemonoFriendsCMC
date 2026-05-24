using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusRideTrigger : MonoBehaviour {

    public GameObject busParent;
    public GameObject carPrefab;
    public GameObject[] inheritObjs;
    public FriendsListOnTrigger friendsListOnTrigger;
    public ActivateExceptHome activateExceptHome;
    int pauseWait = 2;
    int actionType;
    const int facilityID = 24;

    private void Update() {
        if (PauseController.Instance) {
            if (PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
            if (PauseController.Instance.returnToLibraryProcessing) {
                enabled = false;
            }
        }
        if (CharacterManager.Instance && CharacterManager.Instance.pCon) {

            if (enabled && pauseWait <= 0 && PauseController.Instance.pauseEnabled) {
                if (actionType != 1 && CharacterManager.Instance.pCon.RideEnabled()) {
                    CharacterManager.Instance.SetActionType(CharacterManager.ActionType.Sit, gameObject);
                    actionType = 1;
                }
            } else {
                if (actionType != 0) {
                    CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
                    actionType = 0;
                }
            }

            if (pauseWait <= 0 && PauseController.Instance.pauseEnabled && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                GameObject carObj = Instantiate(carPrefab, busParent.transform.position, busParent.transform.rotation, busParent.transform.parent);                
                for (int i = 0; i < inheritObjs.Length; i++) {
                    if (inheritObjs[i]) {
                        inheritObjs[i].transform.SetParent(carObj.transform);
                    }
                }
                RideBase_Driving driveComponent = carObj.GetComponent<RideBase_Driving>();
                if (driveComponent) {
                    driveComponent.WarpFriend(CharacterManager.Instance.pCon);
                    if (friendsListOnTrigger) {
                        driveComponent.SetFriendsRideTogether(friendsListOnTrigger.GetFriendsIDArray());
                    }
                    if (actionType != 2) {
                        CharacterManager.Instance.SetActionType(CharacterManager.ActionType.StandUp, carObj);
                        actionType = 2;
                    }
                }
                ActivateExceptHome[] toActs = carObj.GetComponents<ActivateExceptHome>();
                if (activateExceptHome && toActs != null && toActs.Length > 0) {
                    for (int i = 0; i < toActs.Length; i++) {
                        toActs[i].ActivateExternal(activateExceptHome.isExceptHome);
                    }
                }
                PauseController.Instance.SetFacilityObj(facilityID, carObj);
                Destroy(busParent);
                enabled = false;
            }
        } else {
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            enabled = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            enabled = false;
            if (actionType != 0) {
                CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
                actionType = 0;
            }
        }
    }
}
