using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateExceptHome : MonoBehaviour {

    public GameObject[] activateTarget;
    public GameObject[] deactivateTarget;
    public int[] exceptHomeFloors;
    private bool processedFlag;
    [System.NonSerialized] public bool isExceptHome;

    private void Start() {
        if (!processedFlag && StageManager.Instance) {
            bool exceptHomeFlag = false;
            if (exceptHomeFloors != null && exceptHomeFloors.Length > 0 && StageManager.Instance.IsHomeStage) {
                for (int i = 0; i < exceptHomeFloors.Length; i++) {
                    if (StageManager.Instance.floorNumber == exceptHomeFloors[i]) {
                        exceptHomeFlag = true;
                        break;
                    }
                }
            }
            ActivateExternal(!StageManager.Instance.IsHomeStage || exceptHomeFlag);
        }
    }

    public void ActivateExternal(bool exceptHome) {
        if (activateTarget.Length > 0) {
            for (int i = 0; i < activateTarget.Length; i++) {
                if (activateTarget[i] && activateTarget[i].activeSelf != exceptHome) {
                    activateTarget[i].SetActive(exceptHome);
                }
            }
        }
        if (deactivateTarget.Length > 0) {
            for (int i = 0; i < deactivateTarget.Length; i++) {
                if (deactivateTarget[i] && deactivateTarget[i].activeSelf != !exceptHome) {
                    deactivateTarget[i].SetActive(!exceptHome);
                }
            }
        }
        processedFlag = true;
        isExceptHome = exceptHome;
    }

}
