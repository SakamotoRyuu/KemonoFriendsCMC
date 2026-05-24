using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlaySpecifyTime : MonoBehaviour {

    public float startTime;

	// Use this for initialization
	void Start () {
        AudioSource aSrc = GetComponent<AudioSource>();
        aSrc.time = startTime;
        aSrc.Play();
	}
}
