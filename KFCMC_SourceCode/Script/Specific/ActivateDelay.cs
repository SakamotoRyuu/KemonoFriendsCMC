using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateDelay : MonoBehaviour {

    public GameObject targetObj;
    public float delayTimer;
    public bool deactivate;

    void Update() {
        if (delayTimer >= 0f) {
            delayTimer -= Time.deltaTime;
            if (delayTimer < 0f) {
                if (targetObj) {
                    targetObj.SetActive(!deactivate);
                }
                enabled = false;
            }
        }
    }
}
