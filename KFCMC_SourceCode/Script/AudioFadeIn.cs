using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeIn : MonoBehaviour {

    public AudioSource audioSource;
    public float startVolume = 0f;
    public float endVolume = 1f;
    public bool changePitch;
    public float startPitch = 1f;
    public float endPitch = 1f;
    public float fadeTime = 1f;
    public float delayTime;

    private float elapsedTime;

    private void Awake() {
        if (audioSource) {
            audioSource.volume = startVolume;
            if (changePitch) {
                audioSource.pitch = startPitch;
            }
        }
    }
    
    void Update() {
        if (audioSource && fadeTime > 0f) {
            if (elapsedTime >= delayTime) {
                if (elapsedTime < delayTime + fadeTime) {
                    audioSource.volume = Mathf.Lerp(startVolume, endVolume, (elapsedTime - delayTime) / fadeTime);
                    if (changePitch) {
                        audioSource.pitch = Mathf.Lerp(startPitch, endPitch, (elapsedTime - delayTime) / fadeTime);
                    }
                } else {
                    audioSource.volume = endVolume;
                    if (changePitch) {
                        audioSource.pitch = endPitch;
                    }
                    enabled = false;
                }
            }
            elapsedTime += Time.deltaTime;
        }
    }

}
