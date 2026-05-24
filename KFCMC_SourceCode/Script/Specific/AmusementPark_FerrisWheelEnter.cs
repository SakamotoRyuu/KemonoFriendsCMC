using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmusementPark_FerrisWheelEnter : MonoBehaviour {

    public AmusementPark_FerrisWheel parentWheel;

    FriendsBase[] fBaseArray = new FriendsBase[GameManager.friendsMax];

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase && !fBase.isItem && !fBase.IsRiding) {
                fBaseArray[Mathf.Clamp(fBase.friendsId, 0, GameManager.friendsMax - 1)] = fBase;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase && !fBase.isItem) {
                fBaseArray[Mathf.Clamp(fBase.friendsId, 0, GameManager.friendsMax - 1)] = null;
            }
        }
    }

    public void EnterFriend(Transform ridePoint) {
        float minDist = 100f;
        int minIndex = -1;
        for (int i = 0; i < fBaseArray.Length; i++) {
            if (fBaseArray[i]) {
                float distTemp = (fBaseArray[i].transform.position - ridePoint.position).sqrMagnitude;
                if (distTemp < minDist) {
                    minDist = distTemp;
                    minIndex = i;
                }
            }
        }
        if (minIndex >= 0) {
            parentWheel.WarpFriendSpecificRidePoint(fBaseArray[minIndex], ridePoint);
            fBaseArray[minIndex] = null;
        }
    }

}
