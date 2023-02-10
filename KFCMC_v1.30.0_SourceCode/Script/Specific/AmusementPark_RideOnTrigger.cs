using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class AmusementPark_RideOnTrigger : MonoBehaviour {

    public Transform[] ridePoints;
    public AmusementPark_RideBase parentRide;
    public bool alsoForFriends;

    int pauseWait;
    const float limitDistance = 10f;

    void SetRide(FriendsBase fBase) {
        Vector3 position = fBase.transform.position;
        int minIndex = -1;
        float minDist = float.MaxValue;
        if (ridePoints.Length == 1) {
            minIndex = 0;
        } else if (ridePoints.Length > 1) {
            for (int i = 0; i < ridePoints.Length; i++) {
                float distTemp = (position - ridePoints[i].position).sqrMagnitude;
                if (distTemp < minDist) {
                    minDist = distTemp;
                    minIndex = i;
                }
            }
        }
        if (minIndex >= 0) {
            for (int i = 0; i < ridePoints.Length; i++) {
                if (parentRide.WarpFriendSpecificRidePoint(fBase, ridePoints[(minIndex + i) % ridePoints.Length])) {
                    break;
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
            if (PauseController.Instance.returnToLibraryProcessing) {
                enabled = false;
            }
        }
        if (parentRide && CharacterManager.Instance && CharacterManager.Instance.pCon) {
            if (pauseWait <= 0 && PauseController.Instance.pauseEnabled && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                if ((CharacterManager.Instance.pCon.transform.position - transform.position).sqrMagnitude < limitDistance * limitDistance) {
                    if (CharacterManager.Instance.pCon.IsRiding) {
                        parentRide.ReleaseFriend(CharacterManager.Instance.pCon);
                    } else if (parentRide) {
                        SetRide(CharacterManager.Instance.pCon);
                    }
                } else {
                    enabled = false;
                }
            }
        } else {
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            enabled = true;
        } else if (alsoForFriends && other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase && !fBase.isItem && !fBase.isPlayer && !fBase.IsRiding) {
                SetRide(fBase);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            enabled = false;
        }
    }

}
