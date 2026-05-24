using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerEmission : MonoBehaviour
{

    public string conditionTag = "ItemGetter";
    public int stayingMax = 2;
    public GameObject targetObj;
    public AudioSource sfx;
    public float duration = 1f;
    public Vector3 power;
    public EasingType easingType;
    public GameObject[] activateObjs;
    public GameObject[] deactivateObjs;

    Renderer rend;
    int matNameID;
    float enterTime;
    int entering;
    bool touchedOnThisFrame;
    Color colorTemp = new Color(0f, 0f, 0f);
    bool activeFlag;

    private void Awake() {
        if (targetObj) {
            matNameID = Shader.PropertyToID("_EmissionColor");
            rend = targetObj.GetComponent<Renderer>(); 
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor(matNameID, colorTemp);
        }
    }

    void Update() {
        if (Time.timeScale > 0f) {
            float timeSave = enterTime;
            if (entering > 0) {
                entering--;
            }
            if (entering > 0 && enterTime < duration) {
                enterTime = Mathf.Clamp(enterTime + Time.deltaTime, 0f, duration);
            } else if (entering <= 0 && enterTime > 0f) {
                enterTime = Mathf.Clamp(enterTime - Time.deltaTime, 0f, duration);
            }
            if (enterTime != timeSave) {
                float rate = Easing.GetEasing(easingType, enterTime, duration, 0f, 1f);
                colorTemp.r = rate * power.x;
                colorTemp.g = rate * power.y;
                colorTemp.b = rate * power.z;
                rend.material.SetColor(matNameID, colorTemp);
                bool flagTemp = (rate >= 0.5f);
                if (activeFlag != flagTemp) {
                    activeFlag = flagTemp;
                    if (activateObjs.Length > 0) {
                        for (int i = 0; i < activateObjs.Length; i++) {
                            if (activateObjs[i] && activateObjs[i].activeSelf != activeFlag) {
                                activateObjs[i].SetActive(activeFlag);
                            }
                        }
                    }
                    if (deactivateObjs.Length > 0) {
                        for (int i = 0; i < deactivateObjs.Length; i++) {
                            if (deactivateObjs[i] && deactivateObjs[i].activeSelf != !activeFlag) {
                                deactivateObjs[i].SetActive(!activeFlag);
                            }
                        }
                    }
                }
            }
            touchedOnThisFrame = false;
        }
    }

    /*
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            entering = true;
            if (enterTime <= 0f && sfx) {
                sfx.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            entering = false;
        }
    }
    */

    private void OnTriggerStay(Collider other) {
        if (!touchedOnThisFrame && other.CompareTag(conditionTag)) {
            touchedOnThisFrame = true;
            if (entering <= 0 && enterTime <= 0f && sfx) {
                sfx.Play();
            }
            entering = stayingMax;
        }
    }

    public void ForceTriggerStay() {
        if (!touchedOnThisFrame) {
            touchedOnThisFrame = true;
            if (entering <= 0 && enterTime <= 0f && sfx) {
                sfx.Play();
            }
            entering = stayingMax;
        }
    }

}
