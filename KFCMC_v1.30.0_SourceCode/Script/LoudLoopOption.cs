using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoudLoopOption : MonoBehaviour {

    public CharacterManager.LoudLoopType loudLoopType;

    private void Awake() {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio && CharacterManager.Instance) {
            CharacterManager.Instance.AddLoudLoopInfo(loudLoopType, transform, audio);
        }
    }

}
