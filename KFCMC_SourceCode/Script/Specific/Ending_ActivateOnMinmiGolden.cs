using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending_ActivateOnMinmiGolden : MonoBehaviour {

    public GameObject[] activateTarget;
    public GameObject[] deactivateTarget;

    void Start() {
        if (GameManager.Instance && GameManager.Instance.minmiGolden) {
            for (int i = 0; i < deactivateTarget.Length; i++) {
                if (deactivateTarget[i]) {
                    deactivateTarget[i].SetActive(false);
                }
            }
            for (int i = 0; i < activateTarget.Length; i++) {
                if (activateTarget[i]) {
                    activateTarget[i].SetActive(true);
                }
            }
        }
    }

}
