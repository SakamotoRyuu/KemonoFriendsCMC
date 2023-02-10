using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterEvent : MonoBehaviour {
    public string targetTag = "";
    public UnityEvent onTriggerEnterEv = new UnityEvent();
    public UnityEvent onTriggerExitEv = new UnityEvent();

    void OnTriggerEnter(Collider other) {
        if (string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag)) {
            onTriggerEnterEv.Invoke();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag)) {
            onTriggerExitEv.Invoke();
        }
    }
}