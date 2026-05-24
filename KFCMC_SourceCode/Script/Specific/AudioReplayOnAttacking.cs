using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioReplayOnAttacking : MonoBehaviour {

    public AudioSource audioSource;
    public float interval = 1f;
    public int maxCount = 3;

    float timer;
    int nowCount;
    CharacterBase parentCBase;

    private void Awake() {
        if (audioSource == null) {
            audioSource = GetComponent<AudioSource>();
        }
        parentCBase = GetComponentInParent<CharacterBase>();
    }

    void Update() {
        if (nowCount < maxCount && audioSource && parentCBase && parentCBase.IsAttacking()) {
            timer += Time.deltaTime;
            if (timer >= interval) {
                timer -= interval;
                audioSource.time = 0f;
                audioSource.Play();
                nowCount++;
            }
        }
    }
}
