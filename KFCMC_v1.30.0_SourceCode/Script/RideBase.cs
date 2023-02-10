using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideBase : MonoBehaviour {

    [System.Serializable]
    public class RidePoint {
        public Transform point;
        public int precedingFriendsID;
        public FriendsBase usingFriends;
        public Transform releasePoint;
    }
    
    public RidePoint[] ridePoints;
    public Collider rideCollider;
    public float forceStopTime;
    public bool timeLimitEnabled;
    public float timeLimitDistance = 10f;
    public bool continueOnDamage;
    public int motionType = 2;
    public bool enablizeOnRide;

    protected bool anyoneRiding;

    protected virtual void FixedUpdate() {
        if (CharacterManager.Instance) {
            anyoneRiding = false;
            for (int i = 0; i < ridePoints.Length; i++) {
                if (ridePoints[i].usingFriends) {
                    anyoneRiding = true;
                    if (!ridePoints[i].usingFriends.IsRiding) {
                        ridePoints[i].usingFriends = null;
                    } else if (ridePoints[i].precedingFriendsID > 0 && !CharacterManager.Instance.GetFriendsExist(ridePoints[i].precedingFriendsID)) {
                        ridePoints[i].usingFriends.RemoveRide(true);
                        ridePoints[i].usingFriends = null;
                    }
                }
            }
            if (enablizeOnRide && !anyoneRiding) {
                enabled = false;
            }
        }
    }

    public void WarpFriend(FriendsBase fBase) {
        if (fBase && !fBase.isItem && fBase.RideEnabled()) {
            bool already = false;
            bool vacancy = false;
            for (int i = 0; i < ridePoints.Length; i++) {
                if (ridePoints[i].usingFriends == null) {
                    vacancy = true;
                } else if (ridePoints[i].usingFriends == fBase) {
                    already = true;
                    break;
                }
            }
            if (vacancy && !already) {
                Vector3 friendsPos = fBase.transform.position;
                int minIndex = -1;
                float minDist = float.MaxValue;
                for (int i = 0; i < ridePoints.Length; i++) {
                    if (ridePoints[i].point && ridePoints[i].usingFriends == null && !(ridePoints[i].precedingFriendsID > 0 && !CharacterManager.Instance.GetFriendsExist(ridePoints[i].precedingFriendsID))) {
                        float sqrDist = (ridePoints[i].point.position - friendsPos).sqrMagnitude;
                        if (sqrDist < minDist) {
                            minDist = sqrDist;
                            minIndex = i;
                        }
                    }
                }
                if (minIndex >= 0) {
                    fBase.SetRide(ridePoints[minIndex].point, forceStopTime, motionType, timeLimitEnabled, timeLimitDistance, ridePoints[minIndex].releasePoint, continueOnDamage);
                    ridePoints[minIndex].usingFriends = fBase;
                    if (enablizeOnRide && !enabled) {
                        enabled = true;
                    }
                }
            }
        }
    }

    public bool WarpFriendSpecificRidePoint(FriendsBase fBase, Transform ridePoint) {
        bool answer = false;
        if (fBase && !fBase.isItem && fBase.RideEnabled()) {
            for (int i = 0; i < ridePoints.Length; i++) {
                if (ridePoints[i].point && ridePoints[i].point == ridePoint) {
                    if (ridePoints[i].usingFriends == null && !(ridePoints[i].precedingFriendsID > 0 && !CharacterManager.Instance.GetFriendsExist(ridePoints[i].precedingFriendsID))) {
                        fBase.SetRide(ridePoints[i].point, forceStopTime, motionType, timeLimitEnabled, timeLimitDistance, ridePoints[i].releasePoint, continueOnDamage);
                        ridePoints[i].usingFriends = fBase;
                    }
                    if (enablizeOnRide && !enabled) {
                        enabled = true;
                    }
                    answer = true;
                    break;
                }
            }
        }
        return answer;
    }

    public bool WarpFriendWithEuler(FriendsBase fBase, int index, Transform[] candidates) {
        bool answer = false;
        if (fBase && !fBase.isItem && fBase.RideEnabled()) {
            if (ridePoints[index].point) {
                if (ridePoints[index].usingFriends == null && !(ridePoints[index].precedingFriendsID > 0 && !CharacterManager.Instance.GetFriendsExist(ridePoints[index].precedingFriendsID))) {
                    Vector3 friendEuler = fBase.transform.forward;
                    float minAngle = float.MaxValue;
                    int minIndex = -1;
                    for (int i = 0; i < candidates.Length; i++) {
                        if (candidates[i]) {
                            float angleTemp = Vector3.Angle(friendEuler, candidates[i].forward);
                            if (angleTemp < minAngle) {
                                minAngle = angleTemp;
                                minIndex = i;
                            }
                        }
                    }
                    if (minIndex >= 0) {
                        ridePoints[index].point.SetPositionAndRotation(candidates[minIndex].position, candidates[minIndex].rotation);
                    }
                    fBase.SetRide(ridePoints[index].point, forceStopTime, motionType, timeLimitEnabled, timeLimitDistance, ridePoints[index].releasePoint, continueOnDamage);
                    ridePoints[index].usingFriends = fBase;
                    if (enablizeOnRide && !enabled) {
                        enabled = true;
                    }
                }
                answer = true;
            }
        }
        return answer;
    }

    public virtual int WarpFriendRideIndexArray(FriendsBase fBase, float forceStopTime, int motionType, bool timeLimitEnabled, float timeLimitDistance, int[] indexArray) {
        if (fBase && !fBase.isItem && fBase.RideEnabled() && indexArray.Length > 0) {
            int minIndex = -1;
            float minDist = float.MaxValue;
            Vector3 friendsPos = fBase.transform.position;
            for (int i = 0; i < indexArray.Length; i++) {
                if (indexArray[i] >= 0 && indexArray[i] < ridePoints.Length) {
                    int index = indexArray[i];
                    if (ridePoints[index].point && ridePoints[index].usingFriends == null && !(ridePoints[index].precedingFriendsID > 0 && !CharacterManager.Instance.GetFriendsExist(ridePoints[index].precedingFriendsID))) {
                        float distTemp = (ridePoints[index].point.position - friendsPos).sqrMagnitude;
                        if (distTemp < minDist) {
                            minDist = distTemp;
                            minIndex = index;
                        }
                    }
                }
            }
            if (minIndex >= 0) {
                fBase.SetRide(ridePoints[minIndex].point, forceStopTime, motionType, timeLimitEnabled, timeLimitDistance, ridePoints[minIndex].releasePoint, continueOnDamage);
                ridePoints[minIndex].usingFriends = fBase;
                if (enablizeOnRide && !enabled) {
                    enabled = true;
                }
                return minIndex;
            }
        }
        return -1;
    }

    public void ReleaseAllFriends() {
        for (int i = 0; i < ridePoints.Length; i++) {
            if (ridePoints[i].usingFriends != null) {
                ridePoints[i].usingFriends.RemoveRide(true);
                ridePoints[i].usingFriends = null;
            }
        }
    }

    public void ReleaseFriend(FriendsBase fBase) {
        if (fBase && !fBase.isItem) {
            for (int i = 0; i < ridePoints.Length; i++) {
                if (ridePoints[i].usingFriends == fBase) {
                    ridePoints[i].usingFriends.RemoveRide(true);
                    ridePoints[i].usingFriends = null;
                    break;
                }
            }
        }
    }    

    protected void SetRideActive(bool flag) {
        if (rideCollider) {
            rideCollider.enabled = flag;
        }
        if (flag == false) {
            ReleaseAllFriends();
        }
    }

    protected virtual void OnDisable() {
        ReleaseAllFriends();
    }
}
