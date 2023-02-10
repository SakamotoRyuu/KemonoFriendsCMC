using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideBase_CheckAnotherServalIFR : RideBase {

    [System.Serializable]
    public class CheckAnotherServalIFR {
        public int activatedIndex;
        public int rideIndex;
        public bool isDisablize;
    }

    public CheckAnotherServalIFR[] checkAnotherServalIFR;
    const int anotherServalID = 31;

    protected void Start() {
        if (checkAnotherServalIFR.Length > 0 && AnotherServalIFRBranch.Instance) {
            for (int i = 0; i < checkAnotherServalIFR.Length; i++) {
                if (AnotherServalIFRBranch.Instance.activatedIndex == checkAnotherServalIFR[i].activatedIndex) {
                    int index = checkAnotherServalIFR[i].rideIndex;
                    if (index < ridePoints.Length) {
                        if (checkAnotherServalIFR[i].isDisablize) {
                            ridePoints[index].point = null;
                        } else {
                            ridePoints[index].precedingFriendsID = anotherServalID;
                        }
                    }
                }
            }
        }
        if (enablizeOnRide && !anyoneRiding) {
            enabled = false;
        }
    }
}
