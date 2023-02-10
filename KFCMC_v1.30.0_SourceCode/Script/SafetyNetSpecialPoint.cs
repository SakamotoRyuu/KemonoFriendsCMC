using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyNetSpecialPoint : MonoBehaviour {

    public Transform teleportPoint;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && teleportPoint) {
            other.transform.position = teleportPoint.position;
        }
    }
}
