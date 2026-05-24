using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmusementPark_FerrisWheelExit : MonoBehaviour {

    public AmusementPark_FerrisWheel parentWheel;
    public Transform exitPoint;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase && !fBase.isItem && fBase.IsRiding) {
                parentWheel.ReleaseFriend(fBase);
                fBase.transform.SetPositionAndRotation(exitPoint.position, exitPoint.rotation);
            }
        }
    }

}
