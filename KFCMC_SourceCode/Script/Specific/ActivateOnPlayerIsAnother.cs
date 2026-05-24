using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnPlayerIsAnother : MonoBehaviour {

    public GameObject[] activateTarget;
    public GameObject[] deactivateTarget;

    void Start() {
        if (GameManager.Instance) {
            bool isAnother = GameManager.Instance.IsPlayerAnother;
            for (int i = 0; i < activateTarget.Length; i++) {
                if (activateTarget[i] && activateTarget[i].activeSelf != isAnother) {
                    activateTarget[i].SetActive(isAnother);
                }
            }
            for (int i = 0; i < deactivateTarget.Length; i++) {
                if (deactivateTarget[i] && deactivateTarget[i].activeSelf != !isAnother) {
                    deactivateTarget[i].SetActive(!isAnother);
                }
            }
        }
    }

}
