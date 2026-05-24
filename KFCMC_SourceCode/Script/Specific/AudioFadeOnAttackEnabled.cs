using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeOnAttackEnabled : MonoBehaviour {

    public AttackDetection attackDetection;
    public float fadeTime;
    public float maxVolume = 1f;
    public bool timeRandomize;

    private AudioSource audioSource;
    private float volumeSave;
    private float timeSave;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = volumeSave = 0f;
    }

    void Update() {
        if (audioSource) {
            float volumeTemp = volumeSave;
            if (attackDetection && attackDetection.attackEnabled) {
                if (fadeTime <= 0f) {
                    timeSave = 1f;
                } else {
                    timeSave = Mathf.Clamp01(timeSave + Time.deltaTime / fadeTime);
                }
            } else {
                if (fadeTime <= 0f) {
                    timeSave = 0f;
                } else {
                    timeSave = Mathf.Clamp01(timeSave - Time.deltaTime / fadeTime);
                }
            }
            volumeTemp = timeSave * maxVolume;
            if (audioSource.volume != volumeTemp) {
                audioSource.volume = volumeTemp;
            }
            if (volumeSave <= 0f && volumeTemp > 0f) {
                if (timeRandomize) {
                    audioSource.time = Random.Range(0f, audioSource.clip.length);
                }
                audioSource.Play();
            } else if (volumeSave > 0f && volumeTemp <= 0f) {
                audioSource.Stop();
            }
            volumeSave = volumeTemp;
        }
    }
}
