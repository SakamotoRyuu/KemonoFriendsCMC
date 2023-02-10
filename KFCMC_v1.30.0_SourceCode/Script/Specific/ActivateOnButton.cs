using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ActivateOnButton : MonoBehaviour {

    public string buttonName = "Cancel";
    public GameObject targetObject;
    public GameObject deactivateTarget;
    public bool onlyOnce;
    public float conditionButtonTime;

    Player playerInput;
    float buttonTime;
    
	void Start () {
        if (GameManager.Instance) {
            playerInput = GameManager.Instance.playerInput;
        }
	}

    void Activate() {
        if (deactivateTarget) {
            deactivateTarget.SetActive(false);
        }
        if (targetObject) {
            targetObject.SetActive(true);
        }
        if (onlyOnce) {
            enabled = false;
        }
    }
	
	void Update () {
		if (GameManager.Instance) {
            if (conditionButtonTime <= 0f) {
                if (playerInput.GetButtonDown(buttonName)) {
                    Activate();
                }
            } else {
                if (playerInput.GetButton(buttonName)) {
                    buttonTime += Time.unscaledDeltaTime;
                    if (buttonTime >= conditionButtonTime) {
                        Activate();
                    }
                } else {
                    buttonTime = 0f;
                }
            }
        }
	}
}
