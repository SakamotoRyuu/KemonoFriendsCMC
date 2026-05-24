using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnTriggerStay : MonoBehaviour {

    public GameObject targetObj;
    public string conditionTag;
    public float duration = 0.1f;

    protected float stayTime;
    protected bool touchedOnThisFrame;

    private void Update() {
        if (Time.timeScale > 0) {
            if (targetObj.activeSelf != (stayTime > 0)) {
                targetObj.SetActive(stayTime > 0);
            }
            touchedOnThisFrame = false;
            if (stayTime > 0) {
                stayTime -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (!touchedOnThisFrame && other.CompareTag(conditionTag)) {
            stayTime = duration;
            touchedOnThisFrame = true;
        }
    }

}
