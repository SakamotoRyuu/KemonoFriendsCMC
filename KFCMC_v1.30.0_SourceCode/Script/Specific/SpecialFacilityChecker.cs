using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialFacilityChecker : MonoBehaviour {

    public int facilityID;
    public GameObject[] disablizeTargets;
    public GameObject[] inverseTargets;
    public InstantiateWithFriends.RepositionTarget[] repositionTargets;
    public bool setFacilityForFDKaban;

    void Awake() {
        if (!CharacterDatabase.Instance.CheckFacilityEnabled(facilityID)) {
            for (int i = 0; i < disablizeTargets.Length; i++) {
                if (disablizeTargets[i]) {
                    disablizeTargets[i].SetActive(false);
                }
            }
            for (int i = 0; i < inverseTargets.Length; i++) {
                if (inverseTargets[i]) {
                    inverseTargets[i].SetActive(true);
                }
            }
            for (int i = 0; i < repositionTargets.Length; i++) {
                if (repositionTargets[i].targetTransform) {
                    Vector3 posTemp = repositionTargets[i].targetTransform.position;
                    posTemp.y = repositionTargets[i].yPosition;
                    repositionTargets[i].targetTransform.position = posTemp;
                }
            }
        } else if (setFacilityForFDKaban && PauseController.Instance) {
            PauseController.Instance.SetFacilityObj(facilityID, gameObject);
        }
    }

}
