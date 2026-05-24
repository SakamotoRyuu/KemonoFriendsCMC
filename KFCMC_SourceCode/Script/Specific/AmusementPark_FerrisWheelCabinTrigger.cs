using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmusementPark_FerrisWheelCabinTrigger : MonoBehaviour {

    public AmusementPark_FerrisWheelEnter enterPoint;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("RidePoint")) {
            enterPoint.EnterFriend(other.transform);
        }
    }

}
