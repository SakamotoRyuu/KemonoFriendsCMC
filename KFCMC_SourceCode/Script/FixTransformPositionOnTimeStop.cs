using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixTransformPositionOnTimeStop : MonoBehaviour {

    public Transform[] targetTransform;

    float preTimeScale = 1f;
    Vector3[] memPos;
    
	void Start () {
        memPos = new Vector3[targetTransform.Length];
	}
    
    void LateUpdate() {
        if (preTimeScale <= 0f && Time.timeScale <= 0f && PauseController.Instance && !PauseController.Instance.IsPhotoPausing) {
            for (int i = 0; i < targetTransform.Length; i++) {
                if (targetTransform[i]) {
                    targetTransform[i].position = memPos[i];
                }
            }
        } else { 
            for (int i = 0; i < targetTransform.Length; i++) {
                if (targetTransform[i]) {
                    memPos[i] = targetTransform[i].position;
                }
            }
        }
        preTimeScale = Time.timeScale;
    }
}
