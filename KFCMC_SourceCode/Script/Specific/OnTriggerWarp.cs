using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerWarp : MonoBehaviour {

    public bool resetCameraEnabled;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            if (CharacterManager.Instance) {
                CharacterManager.Instance.SuperWarp(resetCameraEnabled);
            }
        }
    }
}
