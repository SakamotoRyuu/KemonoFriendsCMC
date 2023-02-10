using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHomeTalkIndex : MonoBehaviour {

    public int friendsID;
    public int newHomeTalkIndex;
    public float delayTime;
    public bool completeAndDestroy;
    public bool presentEnabled;
    public int[] presentItemID;
    float elapsedTime;

    private void Update() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= delayTime) {
            ItemCharacter[] ifrs = GameObject.FindObjectsOfType<ItemCharacter>();
            for (int i = 0; i < ifrs.Length; i++) {
                if (ifrs[i] && ifrs[i].isFriends && ifrs[i].characterIndex == friendsID) {
                    ifrs[i].ForceHomeTalkIndex(newHomeTalkIndex);
                    if (presentEnabled) {
                        ifrs[i].SetPresentItem(presentItemID);
                    }
                    break;
                }
            }
            if (completeAndDestroy) {
                Destroy(gameObject);
            } else {
                this.enabled = false;
            }
        }
    }



}
