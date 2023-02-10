using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepDetection : MonoBehaviour {

    public JudgeFootMaterial judgeFootMaterial;

    AudioSource audioSource;
    AudioClip audioClip;
    double preEnterTime = -1.0f;
    int materialType = -1;
    GameObject colliderObj;
    int preMatType = -2;
    float exitTimeRemain;
    
    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate() {
        if (exitTimeRemain > 0f) {
            exitTimeRemain -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (FootstepManager.Instance && !other.isTrigger) {
            if (judgeFootMaterial && judgeFootMaterial.materialType >= 0) {
                materialType = judgeFootMaterial.materialType;
            } else {
                materialType = FootstepManager.Instance.JudgeType(other.gameObject);
            }
            if (materialType >= 0) {
                if (GameManager.Instance.time >= preEnterTime + 0.125 && (materialType != preMatType || (colliderObj == null && exitTimeRemain <= 0f) || (colliderObj != null && colliderObj.activeInHierarchy == false))) {
                    float pitchTemp = FootstepManager.Instance.GetMaterialPitch(materialType);
                    if (audioSource.pitch != pitchTemp) {
                        audioSource.pitch = pitchTemp;
                    }
                    audioClip = FootstepManager.Instance.GetPlayClip(materialType);
                    preEnterTime = GameManager.Instance.time;
                    if (audioClip) {
                        audioSource.PlayOneShot(audioClip);
                    }
                }
                colliderObj = other.gameObject;
                preMatType = materialType;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (FootstepManager.Instance && colliderObj != null && other.gameObject == colliderObj) {
            colliderObj = null;
            exitTimeRemain = 0.03f;
        }

    }

}
