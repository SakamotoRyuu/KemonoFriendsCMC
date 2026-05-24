using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnTriggerEnter : MonoBehaviour {

    public GameObject targetObj;
    public string targetTag;
    public bool deactivateOnExit = false;

    protected bool activated = false;

    protected virtual void Activate(bool flag) {
        activated = flag;
        targetObj.SetActive(flag);
    }

    private void OnTriggerEnter(Collider other) {
        if (!activated && (string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag))) {
            Activate(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (deactivateOnExit && activated && (string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag))) {
            Activate(false);
        }
    }
}
