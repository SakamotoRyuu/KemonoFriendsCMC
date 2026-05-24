using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBGMOnEnable : MonoBehaviour
{

    public bool changeBgm;
    public int bgmNumber;
    public bool changeAmbient;
    public int ambientNumber;

    private void OnEnable() {
        if (changeBgm) {
            BGM.Instance.Play(bgmNumber);
        }
        if (changeAmbient) {
            Ambient.Instance.Play(ambientNumber);
        }
    }

}
