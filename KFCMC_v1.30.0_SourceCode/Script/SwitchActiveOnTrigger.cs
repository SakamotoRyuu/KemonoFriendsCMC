using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchActiveOnTrigger : MonoBehaviour {

    public GameObject targetObj;
    public string targetTag;    

    private void OnTriggerEnter(Collider other) {
        if ((string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag))) {
            targetObj.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if ((string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag))) {
            targetObj.SetActive(false);
        }
    }

}
