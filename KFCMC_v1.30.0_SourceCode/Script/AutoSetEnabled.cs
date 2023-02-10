using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSetEnabled : MonoBehaviour {

    public GameObject[] targetObjects;
    public Collider[] targetColliders;
    public AudioSource[] targetAudioSources;
    public MonoBehaviour[] targetBehaviours;
    public float delay = 1f;
    public bool toDisable = false;
    float elapsedTime = 0;

    void Activate() {
        for (int i = 0; i < targetObjects.Length; i++) {
            if (targetObjects[i]) {
                targetObjects[i].SetActive(!toDisable);
            }
        }
        for (int i = 0; i < targetColliders.Length; i++) {
            if (targetColliders[i]) {
                targetColliders[i].enabled = !toDisable;
            }
        }
        for (int i = 0; i < targetAudioSources.Length; i++) {
            if (targetAudioSources[i]) {
                targetAudioSources[i].enabled = !toDisable;
            }
        }
        for (int i = 0; i < targetBehaviours.Length; i++) {
            if (targetBehaviours[i]) {
                targetBehaviours[i].enabled = !toDisable;
            }
        }
    }

    private void Start() {
        if (delay <= 0f) {
            Activate();
        }
    }

    void Update() {
        if (elapsedTime < delay) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= delay) {
                Activate();
            }
        }
    }
}
