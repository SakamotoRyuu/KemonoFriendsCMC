using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideBase_OnTrigger : RideBase {

    [System.Serializable]
    public class CheckAnotherServalIFR {
        public int activatedIndex;
        public int targetRidePointIndex;
        public bool disablize;
    }

    public bool alsoForFriends;
    public CheckAnotherServalIFR[] checkAnotherServalIFR;
    int pauseWait;
    bool updateEnabled;
    const int anotherServalID = 31;

    private void Start() {
        if (checkAnotherServalIFR.Length > 0 && AnotherServalIFRBranch.Instance) {
            for (int i = 0; i < checkAnotherServalIFR.Length; i++) {
                if (AnotherServalIFRBranch.Instance.activatedIndex == checkAnotherServalIFR[i].activatedIndex && checkAnotherServalIFR[i].targetRidePointIndex < ridePoints.Length) {
                    if (checkAnotherServalIFR[i].disablize) {
                        ridePoints[checkAnotherServalIFR[i].targetRidePointIndex].point = null;
                    } else {
                        ridePoints[checkAnotherServalIFR[i].targetRidePointIndex].precedingFriendsID = anotherServalID;
                    }
                }
            }
        }
    }

    private void Update() {
        if (updateEnabled) {
            if (PauseController.Instance) {
                if (PauseController.Instance.pauseGame) {
                    pauseWait = 2;
                } else if (pauseWait > 0) {
                    pauseWait--;
                }
                if (PauseController.Instance.returnToLibraryProcessing) {
                    updateEnabled = false;
                }
            }
            if (CharacterManager.Instance && CharacterManager.Instance.pCon) {
                if (pauseWait <= 0 && PauseController.Instance.pauseEnabled && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                    if ((CharacterManager.Instance.pCon.transform.position - transform.position).sqrMagnitude < timeLimitDistance * timeLimitDistance) {
                        if (CharacterManager.Instance.pCon.IsRiding) {
                            ReleaseFriend(CharacterManager.Instance.pCon);
                        } else {
                            SetRide(CharacterManager.Instance.pCon);
                        }
                    } else {
                        updateEnabled = false;
                    }
                }
            } else {
                updateEnabled = false;
            }
        }
    }

    void SetRide(FriendsBase fBase) {
        Vector3 position = fBase.transform.position;
        int minIndex = -1;
        float minDist = float.MaxValue;
        if (ridePoints.Length == 1) {
            if (ridePoints[0].point) {
                minIndex = 0;
            }
        } else if (ridePoints.Length > 1) {
            for (int i = 0; i < ridePoints.Length; i++) {
                if (ridePoints[i].point && ridePoints[i].usingFriends == null) {
                    float distTemp = (position - ridePoints[i].point.position).sqrMagnitude;
                    if (distTemp < minDist) {
                        minDist = distTemp;
                        minIndex = i;
                    }
                }
            }
        }
        if (minIndex >= 0) {
            for (int i = 0; i < ridePoints.Length; i++) {
                if (WarpFriendSpecificRidePoint(fBase, ridePoints[(minIndex + i) % ridePoints.Length].point)) {
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            updateEnabled = true;
        } else if (alsoForFriends && other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase && !fBase.isItem && !fBase.isPlayer && !fBase.IsRiding) {
                SetRide(fBase);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            updateEnabled = false;
        }
    }

}
