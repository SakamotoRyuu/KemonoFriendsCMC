using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmusementPark_FerrisWheel : AmusementPark_RideBase {

    [System.Serializable]
    public class RideCombi {
        public int[] rideIndex;
        public ItemCharacter ifrCharacter;
        public float timeRemain;
        public bool flagSave;
    }

    public float rotationSpeed;
    public Rigidbody wheel;
    public AmusementPark_Door[] cabin;
    public float doorOpenHeight;
    public Transform[] exitPoint;
    public RideCombi[] rideCombi;


    Vector3 wheelEuler = Vector3.zero;
    Vector3 cabinEuler = Vector3.zero;
    float[] heightSave;
    bool initialized;

    protected void Start() {
        if (wheel) {
            heightSave = new float[cabin.Length];
            wheelEuler.z = Random.Range(-180f, 180f);
            wheel.MoveRotation(Quaternion.Euler(wheelEuler));
            for (int i = 0; i < cabin.Length; i++) {
                heightSave[i] = cabin[i].transform.position.y;
                cabin[i].ControlDoor(heightSave[i] <= doorOpenHeight);
            }
        }
        initialized = true;
    }

    protected override void FixedUpdate() {
        if (initialized) {
            if (CharacterManager.Instance) {
                for (int i = 0; i < ridePoints.Length; i++) {
                    if (ridePoints[i].usingFriends) {
                        if (!ridePoints[i].usingFriends.IsRiding) {
                            ridePoints[i].usingFriends = null;
                        } else if (ridePoints[i].precedingFriendsID > 0 && !CharacterManager.Instance.GetFriendsExist(ridePoints[i].precedingFriendsID)) {
                            ridePoints[i].usingFriends.RemoveRide();
                            ridePoints[i].usingFriends = null;
                        } 
                    }
                }
                if (Time.timeScale > 0) {
                    float deltaTimeCache = Time.fixedDeltaTime;
                    for (int i = 0; i < rideCombi.Length; i++) {
                        int combiCount = 0;
                        for (int j = 0; j < rideCombi[i].rideIndex.Length; j++) {
                            int index = rideCombi[i].rideIndex[j];
                            if (index >= 0 && index < ridePoints.Length && ridePoints[index].usingFriends) {
                                combiCount++;
                            }
                        }
                        if (rideCombi[i].ifrCharacter && rideCombi[i].ifrCharacter.GetCharacterActive()) {
                            combiCount++;
                        }
                        bool combiFlag = combiCount >= 2;
                        if (rideCombi[i].flagSave == false && combiFlag) {
                            rideCombi[i].timeRemain = Random.Range(20f, 25f);
                        }
                        rideCombi[i].flagSave = combiFlag;
                        if (combiFlag) {
                            rideCombi[i].timeRemain -= deltaTimeCache;
                            if (rideCombi[i].timeRemain < 0) {
                                float smileTime = Random.Range(4.5f, 5.5f);
                                rideCombi[i].timeRemain = Random.Range(20f, 25f);
                                for (int j = 0; j < rideCombi[i].rideIndex.Length; j++) {
                                    int index = rideCombi[i].rideIndex[j];
                                    if (index >= 0 && index < ridePoints.Length && ridePoints[index].usingFriends) {
                                        ridePoints[index].usingFriends.SetFaceSpecial(FriendsBase.FaceName.Smile, smileTime);
                                    }
                                }
                                if (rideCombi[i].ifrCharacter && rideCombi[i].ifrCharacter.GetCharacterActive()) {
                                    rideCombi[i].ifrCharacter.SetSmileTimer(smileTime);
                                }
                            }
                        }
                    }
                }

            }

            if (rotationSpeed != 0f && wheel) {
                float delta = Time.fixedDeltaTime;
                wheelEuler.z += rotationSpeed * delta;
                if (wheelEuler.z > 180f) {
                    wheelEuler.z -= 360f;
                }
                wheel.MoveRotation(Quaternion.Euler(wheelEuler));
                for (int i = 0; i < cabin.Length; i++) {
                    cabinEuler.z = (wheelEuler.z + 360f / cabin.Length * i) * -1f;
                    cabin[i].transform.localEulerAngles = cabinEuler;
                    float heightNow = cabin[i].transform.position.y;
                    if (heightSave[i] > doorOpenHeight && heightNow <= doorOpenHeight) {
                        cabin[i].ControlDoor(true);
                    } else if (heightSave[i] <= doorOpenHeight && heightNow > doorOpenHeight) {
                        cabin[i].ControlDoor(false);
                    }
                    heightSave[i] = heightNow;
                }
            }
        }
    }

    public override void SetRunning(bool flag) {
        if (flag) {
            wheelEuler.z = Random.Range(-180f, 180f);
            wheel.MoveRotation(Quaternion.Euler(wheelEuler));
            for (int i = 0; i < cabin.Length; i++) {
                heightSave[i] = cabin[i].transform.position.y;
                cabin[i].ControlDoor(heightSave[i] <= doorOpenHeight);
            }
        } else {
            int exitCount = 0;
            for (int i = 0; i < ridePoints.Length; i++) {
                if (ridePoints[i].usingFriends != null) {
                    ridePoints[i].usingFriends.RemoveRide();
                    ridePoints[i].usingFriends.transform.SetPositionAndRotation(exitPoint[exitCount].position, exitPoint[exitCount].rotation);
                    ridePoints[i].usingFriends = null;
                    exitCount = (exitCount + 1) % exitPoint.Length;
                }
            }
        }
        enabled = flag;
    }

}
