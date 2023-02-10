using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideChildTrigger_ChangeWithEuler : RideChildTrigger {

    public Transform[] candidates;

    protected override void SetRideBody(FriendsBase fBase) {
        if (candidates.Length > 0 && rideTargetIndex.Length > 0) {
            parentRide.WarpFriendWithEuler(fBase, rideTargetIndex[0], candidates);
        } else {
            base.SetRideBody(fBase);
        }
    }

}
