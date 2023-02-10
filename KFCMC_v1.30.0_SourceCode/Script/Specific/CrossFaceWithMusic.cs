using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossFaceWithMusic : MonoBehaviour {

    public AudioSource audioSource;
    public float inTime = 1f;
    public float outTime = 1f;
    public float minVolume = 0f;
    public float maxVolume = 1f;

    private bool silentSave = false;
    private int fading = 0;
    private double timeStamp;
	
	// Update is called once per frame
	void Update () {
        bool silentTemp = true;
        if (BGM.Instance) {
            silentTemp = (BGM.Instance.GetPlayingIndex() < 0);
        }
        if (audioSource) {
            if (silentTemp && !silentSave) {
                fading = 1;
                timeStamp = GameManager.Instance.time;
                audioSource.volume = minVolume;
                audioSource.Play();
                silentSave = silentTemp;
            } else if (!silentTemp && silentSave) {
                fading = 2;
                timeStamp = GameManager.Instance.time;
                audioSource.volume = maxVolume;
                silentSave = silentTemp;
            }

            if (fading == 1) {
                if (GameManager.Instance.time - timeStamp < inTime) {
                    audioSource.volume = Mathf.Lerp(minVolume, maxVolume, (float)(GameManager.Instance.time - timeStamp) / inTime);
                } else {
                    audioSource.volume = maxVolume;
                    fading = 0;
                }
            } else if (fading == 2) {
                if (GameManager.Instance.time - timeStamp < outTime) {
                    audioSource.volume = Mathf.Lerp(maxVolume, minVolume, (float)(GameManager.Instance.time - timeStamp) / outTime);
                } else {
                    audioSource.volume = minVolume;
                    fading = 0;
                    if (minVolume <= 0) {
                        audioSource.Stop();
                    }
                }
            }
        }
    }
}
