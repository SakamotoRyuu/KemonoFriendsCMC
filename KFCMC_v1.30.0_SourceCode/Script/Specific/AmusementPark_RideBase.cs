using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmusementPark_RideBase : RideBase {

    public GameObject obstacleObj;
    public GameObject[] respawnObjs;
    public ItemCharacter[] smileCharacters;
    
    protected float entireSpeed;

    protected override void FixedUpdate() {
        if (obstacleObj && obstacleObj.activeSelf != (entireSpeed > 0.2f)) {
            SetObstacleActive(entireSpeed > 0.2f);
        }
        if (rideCollider && rideCollider.enabled != (entireSpeed > 0f)) {
            SetRideActive(entireSpeed > 0f);
        }
        base.FixedUpdate();
        if (entireSpeed > 0.7f) {
            for (int i = 0; i < ridePoints.Length; i++) {
                if (ridePoints[i].usingFriends) {
                    ridePoints[i].usingFriends.SetFaceSmile();
                }
            }
            for (int i = 0; i < smileCharacters.Length; i++) {
                if (smileCharacters[i]) {
                    smileCharacters[i].SetSmileTimer();
                }
            }
        }
    }   
    
    protected void SetObstacleActive(bool flag) {
        if (obstacleObj) {
            obstacleObj.SetActive(flag);
        }
        for (int i = 0; i < respawnObjs.Length; i++) {
            respawnObjs[i].SetActive(!flag);
        }
    }
    
    public virtual void SetRunning(bool flag) {
        FixedUpdate();
        enabled = flag;
    }

    protected void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase && !fBase.isItem) {
                WarpFriend(fBase);
            }
        }
    }

    protected void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase && !fBase.isItem) {
                ReleaseFriend(fBase);
            }
        }
    }

}
