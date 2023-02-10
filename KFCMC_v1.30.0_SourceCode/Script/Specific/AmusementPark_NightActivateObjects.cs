using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmusementPark_NightActivateObjects : MonoBehaviour {

    public bool isNight;
    public GameObject[] activateTargets;

    void Update() {
        if (isNight != LightingDatabase.Instance.IsNight) {
            isNight = LightingDatabase.Instance.IsNight;
            for (int i = 0; i < activateTargets.Length; i++) {
                if (activateTargets[i]) {
                    activateTargets[i].SetActive(isNight);
                }
            }
        }
    }

}
