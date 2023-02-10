using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFadeBySpeed : MonoBehaviour {

    public Rigidbody rb;
    public AudioSource aSrc;
    public Vector2 speedRange = new Vector2(5, 20);
    public float smoothTime = 0.2f;

    float nowVolume = 0;
    
	void Start () {
        aSrc.volume = nowVolume = 0f;
	}

    private void FixedUpdate() {
        float speed = rb.velocity.magnitude;
        float targetVolume = 0f;
        float currentVelocity = 0f;
        if (speedRange.y > speedRange.x) {
            targetVolume = Mathf.Clamp01((speed - speedRange.x) / (speedRange.y - speedRange.x));
        }
        nowVolume = Mathf.SmoothDamp(nowVolume, targetVolume, ref currentVelocity, smoothTime);
        aSrc.volume = nowVolume;
        if (aSrc.isPlaying && nowVolume <= 0.001f) {
            aSrc.Stop();
        } else if (!aSrc.isPlaying && nowVolume > 0.02f) {
            aSrc.Play();
        }
    }

}
