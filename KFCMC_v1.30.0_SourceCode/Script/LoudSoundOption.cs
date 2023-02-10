using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoudSoundOption : MonoBehaviour {

    public CharacterManager.LoudSoundType loudSoundType;
    public float decreaseAmount = 0.25f;

    private void Awake() {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio && CharacterManager.Instance) {
            float rate = CharacterManager.Instance.GetLoudSoundVolumeRate(loudSoundType, decreaseAmount);
            audio.volume *= rate;
            if (rate <= 0f) {
                audio.enabled = false;
            }
        }
    }

}
