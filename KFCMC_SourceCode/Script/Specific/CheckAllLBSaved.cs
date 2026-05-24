using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAllLBSaved : MonoBehaviour {

    public GameObject activateTarget;
    public GameObject deactivateTarget;

    private void Awake() {
        if (GameManager.Instance && GameManager.Instance.save.GotLuckyBeastCount >= GameManager.luckyBeastMax) {
            if (activateTarget) {
                activateTarget.SetActive(true);
            }
            if (deactivateTarget) {
                deactivateTarget.SetActive(false);
            }
        }
    }

}
