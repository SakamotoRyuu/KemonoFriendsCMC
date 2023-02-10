using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmusementPark_MerryGoRound : AmusementPark_RideBase {

    public Rigidbody floorRigid;
    public Rigidbody[] horseRigid;
    public Transform floorTrans;
    public Transform[] horseTrans;
    public float centripetalOffset;
    public MovingFloor[] movingFloors;
    public float entireCycleTime;

    float entireDuration;
    float[] horseDuration = new float[horseMax];

    Vector3 floorEuler;
    bool initialized;
    bool zeroSpeedReset;
    const int horseMax = 4;
    const float floorRotationSpeed = 50f;
    const float horseCycleTime = 4f;
    static readonly Vector3 vecZero;

    private void Start() {
        entireDuration = entireCycleTime * Random.value;
        for (int i = 0; i < horseMax; i++) {
            horseDuration[i] = horseCycleTime * i / horseMax;
        }
        initialized = true;
        FixedUpdate();
    }

    protected override void FixedUpdate() {
        if (initialized) {
            float delta = Time.fixedDeltaTime;
            entireDuration += delta;
            if (entireDuration >= entireCycleTime) {
                entireDuration -= entireCycleTime;
            }
            entireSpeed = Mathf.Clamp01(Mathf.Sin(Mathf.PI * 2f * entireDuration / entireCycleTime) * (entireDuration < entireCycleTime * 0.75f ? 1.25f : 1f) + 0.96f);

            if (entireSpeed > 0f || zeroSpeedReset) {
                zeroSpeedReset = entireSpeed > 0f;
                floorEuler.y += floorRotationSpeed * entireSpeed * delta;
                floorTrans.localEulerAngles = floorEuler;
                for (int i = 0; i < horseMax; i++) {
                    horseDuration[i] += delta * entireSpeed;
                    if (horseDuration[i] >= horseCycleTime) {
                        horseDuration[i] -= horseCycleTime;
                    }
                    float cyclePoint = Mathf.Clamp01((Mathf.Sin(Mathf.PI * 2f * (horseDuration[i] / horseCycleTime)) + 1f) * 0.5f);
                    Vector3 horsePos = vecZero;
                    horsePos.y += Mathf.Lerp(0f, 0.25f * Mathf.Clamp01(entireSpeed * 2f), cyclePoint);
                    horseTrans[i].localPosition = horsePos;
                }

                floorRigid.MoveRotation(floorTrans.rotation);
                for (int i = 0; i < horseMax; i++) {
                    horseRigid[i].MovePosition(horseTrans[i].position);
                    horseRigid[i].MoveRotation(horseTrans[i].rotation);
                }
                for (int i = 0; i < movingFloors.Length; i++) {
                    movingFloors[i].centripetalOffset = centripetalOffset * entireSpeed * entireSpeed;
                }
            }
        }
        base.FixedUpdate();
    }

    public override void SetRunning(bool flag) {
        if (flag) {
            entireDuration = entireCycleTime * Random.value;
        } else {
            entireDuration = entireCycleTime * 0.7f;
        }
        base.SetRunning(flag);
    }

}
