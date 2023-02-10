using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateWithGreatComplete : MonoBehaviour {

    public GameObject[] activateTargets;

    private void Awake() {
        if (GameManager.Instance && GameManager.Instance.GetPerfectCompleted()) {
            for (int i = 0; i < activateTargets.Length; i++) {
                if (activateTargets[i] && !activateTargets[i].activeSelf) {
                    activateTargets[i].SetActive(true);
                }
            }
        }
    }
}
