using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideBase_MoveChair : RideBase
{

    public Transform[] chair;
    public Vector3 ridingLocalPos;
    public Vector3 nobodyLocalPos;
    public bool changeEuler;
    public Vector3 ridingLocalEuler;
    public Vector3 nobodyLocalEuler;

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (CharacterManager.Instance) {
            for (int i = 0; i < ridePoints.Length; i++) {
                if (ridePoints[i].usingFriends == null && chair[i] && chair[i].localPosition != nobodyLocalPos) {
                    chair[i].localPosition = nobodyLocalPos;
                    if (changeEuler) {
                        chair[i].localEulerAngles = nobodyLocalEuler;
                    }
                }
            }
        }
    }    

    public override int WarpFriendRideIndexArray(FriendsBase fBase, float forceStopTime, int motionType, bool timeLimitEnabled, float timeLimitDistance, int[] indexArray) {
        int id = base.WarpFriendRideIndexArray(fBase, forceStopTime, motionType, timeLimitEnabled, timeLimitDistance, indexArray);
        if (id >= 0 && id < chair.Length && chair[id]) {
            chair[id].localPosition = ridingLocalPos;
            if (changeEuler) {
                chair[id].localEulerAngles = ridingLocalEuler;
            }
            if (enablizeOnRide && !enabled) {
                enabled = true;
            }
        }
        return id;
    }

}
