using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrairieHoleRemover : MonoBehaviour {

    public GameObject removeEffect;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PrairieHole") && other.transform.parent != null) {
            Instantiate(removeEffect, other.transform.parent.position, Quaternion.identity);
            Destroy(other.transform.parent.gameObject);
        }
    }

}
