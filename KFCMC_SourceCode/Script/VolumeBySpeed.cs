using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeBySpeed : MonoBehaviour {

    public float minVolume = 0f;
    public float maxVolume = 1f;
    public float deltaTimeMultiplier = 5f;
    CharacterBase parentCBase;
    AudioSource aSrc;
    float nowVolume = 0f;
    float nowSpeed = 0f;
    bool isMoving = false;
    float volumeSave = 0f;
    
	void Awake () {
        parentCBase = GetComponentInParent<CharacterBase>();
        aSrc = GetComponent<AudioSource>();
        if (aSrc) {
            aSrc.volume = nowVolume = volumeSave = 0f;
        }
	}

    void Update() {
        if (parentCBase && aSrc) {
            nowSpeed = parentCBase.GetNowSpeed();
            isMoving = (nowSpeed > 0.1f || nowSpeed < -0.1f);
            if (isMoving && nowVolume < maxVolume) {
                nowVolume = Mathf.Clamp(nowVolume += Time.deltaTime * deltaTimeMultiplier, minVolume, maxVolume);
            } else if (!isMoving && nowVolume > minVolume) {
                nowVolume = Mathf.Clamp(nowVolume -= Time.deltaTime * deltaTimeMultiplier, minVolume, maxVolume);
            }
            if (nowVolume != volumeSave) {
                aSrc.volume = nowVolume;
                volumeSave = nowVolume;
            }
        }
    }
}
