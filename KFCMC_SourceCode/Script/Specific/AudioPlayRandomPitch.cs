using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayRandomPitch : MonoBehaviour {

    public AudioSource aSrc;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    private void Awake() {
        aSrc.pitch = Random.Range(minPitch, maxPitch);
        aSrc.Play();
    }

}
