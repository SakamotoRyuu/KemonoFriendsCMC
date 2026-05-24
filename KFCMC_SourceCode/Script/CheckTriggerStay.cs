using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTriggerStay : MonoBehaviour {

    public string targetTag = "EnemyDamageDetection";
    public string targetLayer = "";
    public bool stayFlag;
    public GameObject stayObj;
    int stayCount;
    float stayTimer;

    int targetLayerIndex = -1;

    private void Awake() {
        if (!string.IsNullOrEmpty(targetLayer)) {
            targetLayerIndex = LayerMask.NameToLayer(targetLayer);
        }
    }

    private void OnTriggerStay(Collider other) {
        if ((string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag)) && (targetLayerIndex < 0 || other.gameObject.layer == targetLayerIndex)) {
            stayCount = 2;
            stayTimer = 1.5f / 60f;
            stayFlag = true;
            stayObj = other.gameObject;
        }
    }

    private void LateUpdate() {
        if (stayFlag) {
            stayCount--;
            stayTimer -= Time.deltaTime;
            if (stayCount <= 0 && stayTimer <= 0f) {
                stayFlag = false;
                stayObj = null;
            }
        }
    }

    private void OnDisable() {
        stayFlag = false;
        stayCount = 0;
    }

}
