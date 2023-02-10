using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsListOnTrigger : MonoBehaviour {

    public bool includePlayer;
    protected bool[] stayArray = new bool[GameManager.friendsMax];

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase && (includePlayer || !fBase.isPlayer)) {
                stayArray[Mathf.Clamp(fBase.friendsId, 0, stayArray.Length - 1)] = true;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase && (includePlayer || !fBase.isPlayer)) {
                stayArray[Mathf.Clamp(fBase.friendsId, 0, stayArray.Length - 1)] = false;
            }
        }
    }

    public int[] GetFriendsIDArray() {
        int friendsCount = 0;
        if (CharacterManager.Instance) {
            for (int i = 0; i < stayArray.Length; i++) {
                if (stayArray[i]) {
                    if (CharacterManager.Instance.GetFriendsExist(i, false)) {
                        friendsCount++;
                    } else {
                        stayArray[i] = false;
                    }
                }
            }
        }
        int[] idArray = new int[friendsCount];
        if (friendsCount > 0) {
            int idIndex = 0;
            for (int i = 0; i < stayArray.Length && idIndex < idArray.Length; i++) {
                if (stayArray[i]) {
                    idArray[idIndex] = i;
                    idIndex++;
                }
            }
        }
        return idArray;
    }

}
