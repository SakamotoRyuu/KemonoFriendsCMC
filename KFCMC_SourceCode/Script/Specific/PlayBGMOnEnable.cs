using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBGMOnEnable : MonoBehaviour
{

    public int musicIndex;
    public float fadeInTime;
    public bool stopOnDisable;
    public float fadeOutTime;

    private void OnEnable() {
        if (BGM.Instance) {
            BGM.Instance.Play(musicIndex, fadeInTime);
        }
    }

    private void OnDisable() {
        if (stopOnDisable && BGM.Instance && BGM.Instance.GetPlayingIndex() == musicIndex) {
            if (fadeOutTime > 0f) {
                BGM.Instance.StopFade(fadeOutTime);
            } else {
                BGM.Instance.Stop();
            }
        }
    }

}
