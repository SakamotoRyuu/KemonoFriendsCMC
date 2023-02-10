using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideBase_Slider : RideBase
{

    public GameObject ridePrefab;
    public Transform[] relayPoints;
    public float acceleration;
    public float maxSpeed;
    public float smileTime;
    public float smileConditionSpeed;

    protected float[] nowSpeeds;
    protected float[] nowPoints;
    protected float[] distances;
    protected bool initialized;

    protected void InitArray() {
        for (int i = 0; i < ridePoints.Length; i++) {
            ridePoints[i].point = Instantiate(ridePrefab, transform).transform;
        }
        nowSpeeds = new float[ridePoints.Length];
        nowPoints = new float[ridePoints.Length];
        distances = new float[relayPoints.Length];
        for (int i = 1; i < distances.Length; i++) {
            distances[i] = distances[i - 1] + Vector3.Distance(relayPoints[i - 1].position, relayPoints[i].position);
        }
        initialized = true;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (initialized && CharacterManager.Instance) {
            float deltaTimeCache = Time.fixedDeltaTime;
            for (int i = 0; i < ridePoints.Length; i++) {
                if (ridePoints[i].usingFriends) {
                    nowPoints[i] += nowSpeeds[i] * deltaTimeCache;
                    int nowRelay = -1;
                    for (int j = 1; j < distances.Length; j++) {
                        if (distances[j] > nowPoints[i]) {
                            nowRelay = j;
                            break;
                        }
                    }
                    if (nowRelay <= 0) {
                        ridePoints[i].usingFriends.RemoveRide();
                        ridePoints[i].usingFriends = null;
                    } else {
                        float lerpTime = (nowPoints[i] - distances[nowRelay - 1]) / (distances[nowRelay] - distances[nowRelay - 1]);
                        Vector3 posTemp = Vector3.Lerp(relayPoints[nowRelay - 1].position, relayPoints[nowRelay].position, lerpTime);
                        Quaternion rotTemp = Quaternion.Lerp(relayPoints[nowRelay - 1].rotation, relayPoints[nowRelay].rotation, lerpTime);
                        ridePoints[i].point.SetPositionAndRotation(posTemp, rotTemp);
                    }
                    if (nowSpeeds[i] < maxSpeed) {
                        nowSpeeds[i] = Mathf.Clamp(nowSpeeds[i] + acceleration * deltaTimeCache, 0, maxSpeed);
                    }
                    if (smileTime > 0f && ridePoints[i].usingFriends && nowSpeeds[i] >= smileConditionSpeed) {
                        ridePoints[i].usingFriends.SetFaceSpecial(FriendsBase.FaceName.Smile, smileTime);
                    }
                }
            }
        }
    }

    public override int WarpFriendRideIndexArray(FriendsBase fBase, float forceStopTime, int motionType, bool timeLimitEnabled, float timeLimitDistance, int[] indexArray) {
        if (fBase && !fBase.isItem && fBase.RideEnabled()) {
            if (!initialized) {
                InitArray();
            }
            int id = Mathf.Clamp(fBase.friendsId, 0, ridePoints.Length - 1);
            ridePoints[id].point.SetPositionAndRotation(relayPoints[0].position, relayPoints[0].rotation);
            fBase.SetRide(ridePoints[id].point, forceStopTime, motionType, timeLimitEnabled, timeLimitDistance, ridePoints[id].releasePoint);
            ridePoints[id].usingFriends = fBase;
            nowSpeeds[id] = 0f;
            nowPoints[id] = 0f;
            if (enablizeOnRide && !enabled) {
                enabled = true;
            }
            return id;
        }
        return -1;
    }

}
