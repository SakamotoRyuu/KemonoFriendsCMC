using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckArmsSound : MonoBehaviour {

    public AudioSource targetAudioSource;
    int configSave = -1;

    void Update() {
        int configTemp = GameManager.Instance.save.config[GameManager.Save.configID_ShowArms];
        if (configSave != configTemp) {
            configSave = configTemp;
            if (targetAudioSource) {
                targetAudioSource.enabled = (configSave == 1);
            }
        }
    }
}
