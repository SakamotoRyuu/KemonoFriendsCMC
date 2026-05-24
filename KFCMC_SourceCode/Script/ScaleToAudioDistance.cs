using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToAudioDistance : MonoBehaviour {

    public AudioSource audioSource;
    public float rate = 1;
    public float offset = 1f;

    Transform trans;

    private void Awake() {
        trans = transform;
    }

    // Update is called once per frame
    void Update () {
        if (audioSource) {
            Vector3 scaleTemp = trans.lossyScale;
            audioSource.minDistance = (scaleTemp.x + scaleTemp.y + scaleTemp.z) * 0.333333f * rate + offset;
        }
	}
}
