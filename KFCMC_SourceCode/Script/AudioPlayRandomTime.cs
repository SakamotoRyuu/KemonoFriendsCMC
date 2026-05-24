using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayRandomTime : MonoBehaviour {

    public AudioSource audioSource;
    public float minTime = 0f;
    public float maxTime = 2f;
    public float delayTime = 0f;

    bool played = false;
    float elapsedTime = 0f;
    
	private void Awake () {
		if (audioSource) {
            if (minTime > 0f || maxTime > 0f) {
                audioSource.time = Random.Range(minTime, maxTime);
            }
            if (delayTime <= 0f) {
                audioSource.Play();
                played = true;
            }
        }
	}

    private void Update() {
        if (!played) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= delayTime) {
                audioSource.Play();
                played = true;
            }
        }
    }

}
