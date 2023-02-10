using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModBranchActivate : MonoBehaviour {

    public GameObject isModObj;
    public GameObject isNotModObj;

    void Start() {
        bool modFlag = (GameManager.Instance && GameManager.Instance.musicModFlag);
        if (isModObj) {
            isModObj.SetActive(modFlag);
        }
        if (isNotModObj) {
            isNotModObj.SetActive(!modFlag);
        }
    }

}
