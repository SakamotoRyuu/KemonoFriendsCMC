using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmusementPark_TeaCup : AmusementPark_RideBase {

    public Rigidbody floorRigid;
    public Rigidbody[] cupRigid;
    public Transform floorTrans;
    public Transform[] cupTrans;
    public float floorRotSpeed;
    public float maxRotSpeed;
    public float cupCycleTime;
    public float cupSineMultiplier;
    public float entireCycleTime;
    public MovingFloor movingFloor;
    public MovingFloor[] movingFloors_Cup;
    public float centripetalOffset_Floor;
    public float centripetalOffset_Cup;

    Vector3 floorEuler;
    float[] cupDuration = new float[cupMax];
    Vector3[] cupEuler = new Vector3[cupMax];
    bool initialized;
    bool zeroSpeedReset;
    float entireDuration;

    const int cupMax = 5;

    private void Start() {
        entireDuration = entireCycleTime * Random.value;
        for (int i = 0; i < cupDuration.Length; i++) {
            cupDuration[i] = cupCycleTime * i / cupMax;
            cupEuler[i] = new Vector3(0f, 360f / cupMax * i, 0f);
        }
        floorEuler = Vector3.zero;
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
                floorEuler.y += floorRotSpeed * entireSpeed * delta;
                floorTrans.localEulerAngles = floorEuler;
                for (int i = 0; i < cupMax; i++) {
                    cupDuration[i] += delta;
                    if (cupDuration[i] >= cupCycleTime) {
                        cupDuration[i] -= cupCycleTime;
                    }
                    float cupRotSpeedRate = Mathf.Clamp(Mathf.Sin(cupDuration[i] / cupCycleTime * Mathf.PI * 2f) * cupSineMultiplier, -1f, 1f) * entireSpeed;
                    cupEuler[i].y += maxRotSpeed * cupRotSpeedRate * delta;
                    if (cupEuler[i].y > 180f) {
                        cupEuler[i].y -= 360f;
                    } else if (cupEuler[i].y < -180f) {
                        cupEuler[i].y += 360f;
                    }
                    cupTrans[i].localEulerAngles = cupEuler[i];
                    movingFloors_Cup[i].centripetalOffset = centripetalOffset_Cup * cupRotSpeedRate * cupRotSpeedRate;
                }
                movingFloor.centripetalOffset = centripetalOffset_Floor * entireSpeed * entireSpeed;

                floorRigid.MoveRotation(floorTrans.rotation);
                for (int i = 0; i < cupMax; i++) {
                    cupRigid[i].MovePosition(cupTrans[i].position);
                    cupRigid[i].MoveRotation(cupTrans[i].rotation);
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
