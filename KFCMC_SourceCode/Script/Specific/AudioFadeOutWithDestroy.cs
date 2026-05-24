using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeOutWithDestroy : MonoBehaviour {

    public AudioSource audioSource;
    public GameObject checkTarget;
    public float fadeTime = 1f;
    public bool completeToDestroy;

    private float startVolume;
    private float elapsedTime;

    private void Awake() {
        if (!audioSource) {
            audioSource = GetComponent<AudioSource>();
        }
        if (audioSource) {
            startVolume = audioSource.volume;
        }
    }

    void Update() {
        if (audioSource && checkTarget == null) {
            elapsedTime += Time.deltaTime;
            if (fadeTime > 0f) {
                audioSource.volume = startVolume * ((fadeTime - elapsedTime) / fadeTime);
            }
            if (elapsedTime >= fadeTime) {
                if (completeToDestroy) {
                    Destroy(gameObject);
                } else {
                    audioSource.volume = 0f;
                    enabled = false;
                }
            }
        }
    }
}
