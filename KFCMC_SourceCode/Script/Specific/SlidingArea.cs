using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingArea : MonoBehaviour {

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase) {
                fBase.slidingFlag = true;
            }
        }
    }

}
