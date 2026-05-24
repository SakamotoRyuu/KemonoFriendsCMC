using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetThunderPole : MonoBehaviour {

    public GameObject thunderPrefab;
    public int audioPriority = 224;
    public float audioVolume = 0.4f;
    public float audioMinDistance = 6f;
    public float delayTime = 0.5f;

    void Awake() {
        if (thunderPrefab) {
            GameObject inst = Instantiate(thunderPrefab, transform);
            AudioSource aSrc = inst.GetComponentInChildren<AudioSource>();
            if (aSrc) {
                if (audioVolume >= 0f) {
                    aSrc.priority = audioPriority;
                    aSrc.volume = audioVolume;
                    aSrc.minDistance = audioMinDistance;
                } else {
                    aSrc.enabled = false;
                }
            }
            AudioPlayRandomTime aPlay = inst.GetComponentInChildren<AudioPlayRandomTime>();
            if (aPlay) {
                if (audioVolume >= 0f) {
                    aPlay.delayTime = delayTime;
                } else {
                    aPlay.enabled = false;
                }
            }
            AutoSetEnabled autoSet = inst.GetComponentInChildren<AutoSetEnabled>();
            if (autoSet) {
                autoSet.delay = delayTime;
            }
        }
    }
}
