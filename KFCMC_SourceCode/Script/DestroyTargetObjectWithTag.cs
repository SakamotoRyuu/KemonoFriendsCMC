using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTargetObjectWithTag : MonoBehaviour {

    public string targetTag;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            Destroy(other.gameObject);
        }
    }
}
