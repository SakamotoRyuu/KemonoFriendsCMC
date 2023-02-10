using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideChildTrigger : MonoBehaviour {

    public RideBase parentRide;
    public float forceStopTime = 30;
    public int motionType = 2;
    public bool timeLimitEnabled = true;
    public float timeLimitDistance = 10;
    public bool alsoForPlayer = true;
    public bool alsoForFriends = true;
    public bool compareWithEuler;
    public int[] rideTargetIndex;
    int pauseWait;
    int actionType;

    private void Update() {
        if (PauseController.Instance) {
            if (PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
            if (PauseController.Instance.returnToLibraryProcessing) {
                Disablize();
            }
        }
        if (parentRide && alsoForPlayer && CharacterManager.Instance && CharacterManager.Instance.pCon) {
            if (pauseWait <= 0 && PauseController.Instance.pauseEnabled && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                if ((CharacterManager.Instance.pCon.transform.position - transform.position).sqrMagnitude < timeLimitDistance * timeLimitDistance) {
                    if (CharacterManager.Instance.pCon.IsRiding) {
                        parentRide.ReleaseFriend(CharacterManager.Instance.pCon);
                    } else {
                        SetRideBody(CharacterManager.Instance.pCon);
                    }
                } else {
                    Disablize();
                }
            }
            if (enabled && pauseWait <= 0 && PauseController.Instance.pauseEnabled) {
                if (CharacterManager.Instance.pCon.IsRiding) {
                    if (actionType != 2) {
                        CharacterManager.Instance.SetActionType(CharacterManager.ActionType.StandUp, gameObject);
                        actionType = 2;
                    }
                } else {
                    if (actionType != 1 && CharacterManager.Instance.pCon.RideEnabled()) {
                        CharacterManager.Instance.SetActionType(CharacterManager.ActionType.Sit, gameObject);
                        actionType = 1;
                    }
                }
            } else {
                if (actionType != 0) {
                    CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
                    actionType = 0;
                }
            }
        } else {
            Disablize();
        }
    }

    void Disablize() {
        if (actionType != 0 && CharacterManager.Instance && gameObject) {
            CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
            actionType = 0;
        }
        enabled = false;
    }

    protected virtual void SetRideBody(FriendsBase fBase) {
        parentRide.WarpFriendRideIndexArray(fBase, forceStopTime, motionType, timeLimitEnabled, timeLimitDistance, rideTargetIndex);
    }

    private void OnTriggerEnter(Collider other) {
        if (alsoForPlayer && other.CompareTag("ItemGetter")) {
            enabled = true;
        } else if (alsoForFriends && other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase && !fBase.isItem && !fBase.isPlayer && !fBase.IsRiding && parentRide) {
                SetRideBody(fBase);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (alsoForPlayer && other.CompareTag("ItemGetter")) {
            Disablize();
        }
    }
}
