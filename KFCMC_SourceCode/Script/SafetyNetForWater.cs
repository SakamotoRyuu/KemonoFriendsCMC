using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyNetForWater : MonoBehaviour {

    public Transform heightPivot;
    const float heightOffset = 1f;
   
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && heightPivot) {
            other.transform.position = new Vector3(other.transform.position.x, heightPivot.position.y + heightOffset, other.transform.position.z);
        }
    }

}
