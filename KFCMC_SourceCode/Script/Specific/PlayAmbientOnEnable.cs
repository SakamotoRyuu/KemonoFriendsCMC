using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAmbientOnEnable : MonoBehaviour {

    public int ambientIndex;
    public float fadeInTime;
    public bool stopOnDisable;
    public float fadeOutTime;

    private void OnEnable() {
        if (Ambient.Instance) {
            Ambient.Instance.Play(ambientIndex, fadeInTime);
        }
    }

    private void OnDisable() {
        if (stopOnDisable && Ambient.Instance && Ambient.Instance.GetPlayingIndex() == ambientIndex) {
            if (fadeOutTime > 0f) {
                Ambient.Instance.StopFade(fadeOutTime);
            } else {
                Ambient.Instance.Stop();
            }
        }
    }
}
