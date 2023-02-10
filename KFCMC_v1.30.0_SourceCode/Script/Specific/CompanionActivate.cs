using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionActivate : MonoBehaviour {

    public GameObject[] activateTarget;
    public GameObject[] deactivateTarget;

    private void OnEnable() {
        for (int i = 0; i < activateTarget.Length; i++) {
            if (activateTarget[i]) {
                activateTarget[i].SetActive(true);
            }
        }
        for (int i = 0; i < deactivateTarget.Length; i++) {
            if (deactivateTarget[i]) {
                deactivateTarget[i].SetActive(false);
            }
        }
    }

    private void OnDisable() {
        for (int i = 0; i < activateTarget.Length; i++) {
            if (activateTarget[i]) {
                activateTarget[i].SetActive(false);
            }
        }
        for (int i = 0; i < deactivateTarget.Length; i++) {
            if (deactivateTarget[i]) {
                deactivateTarget[i].SetActive(true);
            }
        }
    }

}
