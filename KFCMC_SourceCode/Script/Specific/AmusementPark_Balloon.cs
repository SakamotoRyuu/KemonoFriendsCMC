using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmusementPark_Balloon : AmusementPark_RideBase {

    public Transform shaftTransform;
    public Transform tiltTransform;
    public Transform[] neckTransform;
    public Rigidbody shaftRigid;
    public Rigidbody tiltRigid;
    public Rigidbody[] neckRigid;
    public float centripetalOffset;
    public MovingFloor[] movingFloors;
    public float entireCycleTime;

    float entireDuration;
    Vector3 shaftEuler = Vector3.zero;
    Vector3 shaftPos = Vector3.zero;
    Vector3 tiltEuler = Vector3.zero;
    Vector3 neckEuler = Vector3.zero;
    bool initialized;
    bool zeroSpeedReset;

    const float shaftSpeedMax = 70f;
    const float shaftHeightMax = 4f;
    const float tiltAngleMax = 10f;
    const float tiltCycle = 20f;
    const float neckAngleMax = -15f;

    private void Start() {
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
            float entirePoint = entireDuration / entireCycleTime;
            float shaftSpeedNow = 0f;
            if (entirePoint < 0.3f) {
                shaftSpeedNow = entirePoint / 0.3f * shaftSpeedMax;
            } else if (entirePoint < 0.7f) {
                shaftSpeedNow = shaftSpeedMax;
            } else if (entirePoint < 0.8666667f) {
                shaftSpeedNow = (0.8666667f - entirePoint) / 0.1666667f * shaftSpeedMax;
            }
            entireSpeed = Mathf.Clamp01(shaftSpeedNow / shaftSpeedMax);
            if (entireSpeed > 0f || zeroSpeedReset) {
                zeroSpeedReset = entireSpeed > 0f;
                tiltEuler.x = entireSpeed < 0.5f ? 0f : Mathf.Clamp(Mathf.Sin(Mathf.PI * 2f * ((entireDuration % tiltCycle) / tiltCycle)) * 2f, -1f, 1f) * tiltAngleMax * ((entireSpeed - 0.5f) * 2f);
                neckEuler.x = entireSpeed * neckAngleMax;
                shaftPos.y = entireSpeed * shaftHeightMax;
                shaftEuler.y += shaftSpeedNow * delta;

                shaftTransform.localPosition = shaftPos;
                shaftTransform.localEulerAngles = shaftEuler;
                tiltTransform.localEulerAngles = tiltEuler;
                for (int i = 0; i < neckTransform.Length; i++) {
                    neckTransform[i].localEulerAngles = neckEuler;
                }

                shaftRigid.MovePosition(shaftTransform.position);
                shaftRigid.MoveRotation(shaftTransform.rotation);
                tiltRigid.MovePosition(tiltTransform.position);
                tiltRigid.MoveRotation(tiltTransform.rotation);
                for (int i = 0; i < neckRigid.Length; i++) {
                    neckRigid[i].MovePosition(neckTransform[i].position);
                    neckRigid[i].MoveRotation(neckTransform[i].rotation);
                }

                float centripetalTemp = centripetalOffset * entireSpeed * entireSpeed;
                for (int i = 0; i < movingFloors.Length; i++) {
                    movingFloors[i].centripetalOffset = centripetalTemp;
                }
            }            
        }
        base.FixedUpdate();
    }

    public override void SetRunning(bool flag) {
        if (flag) {
            entireDuration = entireCycleTime * Random.value;
        } else {
            entireDuration = entireCycleTime * 0.9f;
        }
        base.SetRunning(flag);
    }

}
