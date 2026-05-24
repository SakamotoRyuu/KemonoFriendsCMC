using UnityEngine;

public class RideBase_FaceOnTimer : RideBase_CheckAnotherServalIFR {

    public float conditionTime;
    public FriendsBase.FaceName changedFace;
    public float faceFixingTime;

    protected float[] ridingTimer;

    protected void Awake() {
        ridingTimer = new float[ridePoints.Length];
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (CharacterManager.Instance && ridingTimer.Length >= ridePoints.Length) {
            float deltaTimeCache = Time.deltaTime;
            for (int i = 0; i < ridePoints.Length; i++) {
                if (ridePoints[i].usingFriends) {
                    ridingTimer[i] += deltaTimeCache;
                    if (ridingTimer[i] >= conditionTime) {
                        ridePoints[i].usingFriends.SetFaceSpecial(changedFace, faceFixingTime);
                    }
                } else {
                    ridingTimer[i] = 0f;
                }
            }
        }
    }

    public override int WarpFriendRideIndexArray(FriendsBase fBase, float forceStopTime, int motionType, bool timeLimitEnabled, float timeLimitDistance, int[] indexArray) {
        int id = base.WarpFriendRideIndexArray(fBase, forceStopTime, motionType, timeLimitEnabled, timeLimitDistance, indexArray);
        if (id >= 0 && id < ridingTimer.Length) {
            ridingTimer[id] = 0f;
        }
        return id;
    }

}
