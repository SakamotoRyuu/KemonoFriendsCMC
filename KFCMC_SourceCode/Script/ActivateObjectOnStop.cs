using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActivateObjectOnStop : MonoBehaviour {

    [System.Serializable]
    public class BeaverWeapon {
        public bool enabled;
        public GameObject switchingObj;
    }

    public GameObject targetObject;
    public Rigidbody rigid;
    public float limitSpeed = 1f;
    public float limitConditionTime = 0.25f;
    public float delay = 1f;
    public bool checkSleep;
    public bool setKinematicOnStop;
    public BeaverWeapon beaverWeapon;    

    private float elapsedTime = 0;
    private float limittingTime = 0;
    private bool stopped = false;
    // private float beaverTime;
    // const float carvingTime = 0.5f;
    // const float noCarvingTime = 1.0f;

    void FixedUpdate() {
        if (!stopped && Time.timeScale > 0 && rigid) {
            float deltaTimeCache = Time.fixedDeltaTime;
            elapsedTime += deltaTimeCache;
            if (elapsedTime >= delay) {
                if (rigid.velocity.sqrMagnitude <= limitSpeed * limitSpeed && (!checkSleep || rigid.IsSleeping())) {
                    limittingTime += deltaTimeCache;
                } else {
                    limittingTime = 0f;
                }
                if (limittingTime >= limitConditionTime) {
                    if (targetObject) {
                        targetObject.SetActive(true);
                    }
                    if (setKinematicOnStop) {
                        rigid.isKinematic = true;
                    }
                    stopped = true;
                }
            }
        }
        if (beaverWeapon.enabled && stopped && Time.timeScale > 0 && beaverWeapon.switchingObj) {
            bool switchOn = (GameManager.Instance.time % 0.75 < 0.25);
            if (beaverWeapon.switchingObj.activeSelf != switchOn) {
                beaverWeapon.switchingObj.SetActive(switchOn);
            }
        }
    }
}
