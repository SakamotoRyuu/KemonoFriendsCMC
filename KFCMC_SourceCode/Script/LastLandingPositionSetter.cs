using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastLandingPositionSetter : MonoBehaviour {

    FriendsBase fBase;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            fBase = other.gameObject.GetComponent<FriendsBase>();
            if (fBase) {
                fBase.SetLastLandingPosition(transform.position);
            }
        }
    }

}
