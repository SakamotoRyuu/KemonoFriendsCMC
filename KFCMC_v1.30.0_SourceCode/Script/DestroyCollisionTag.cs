using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCollisionTag : MonoBehaviour {

    public string targetTag;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            Destroy(gameObject);
        }
    }

}
