using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerController pCon = other.gameObject.GetComponent<PlayerController>();
            if (pCon != null) {
                pCon.climbTarget = transform;
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerController pCon = other.gameObject.GetComponent<PlayerController>();
            if (pCon != null && pCon.climbTarget == transform) {
                pCon.climbTarget = null;
            }
        }
    }
}
