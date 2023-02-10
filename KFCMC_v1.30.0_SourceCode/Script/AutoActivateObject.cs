using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoActivateObject : MonoBehaviour {

    public GameObject targetObj;
    public float delay = 0.5f;
    public bool toActive = true;

    float elapsedTime = 0;
    bool activated = false;

	void Update () {
        if (!activated) {
            if (elapsedTime >= delay) {
                if (targetObj) {
                    targetObj.SetActive(toActive);
                }
                activated = true;
                enabled = false;
            } else {
                elapsedTime += Time.deltaTime;
            }
        }
	}
}
