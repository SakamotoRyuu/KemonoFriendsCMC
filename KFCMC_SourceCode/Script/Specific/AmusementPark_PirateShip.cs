using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmusementPark_PirateShip : AmusementPark_RideBase {

    public Rigidbody shipRigid;
    public Transform rotationDummy;
    public float amplitudeCycleTime;
    public float amplitudeCycleMultiplier;
    public float amplitudeCycleOffset;
    public float maxAmplitude;
    public Vector2 pendulumCycleTime;
    
    Vector3 eulerTemp;
    float amplitudeDuration;
    float pendulumDuration;
    bool initialized;
    bool zeroSpeedReset;

    private void Start() {
        eulerTemp = rotationDummy.localEulerAngles;
        amplitudeDuration = amplitudeCycleTime * Random.value;
        initialized = true;
        FixedUpdate();
    }
    
    protected override void FixedUpdate() {
        if (initialized) {
            float delta = Time.fixedDeltaTime;
            amplitudeDuration += delta;
            if (amplitudeDuration >= amplitudeCycleTime) {
                amplitudeDuration -= amplitudeCycleTime;
            }
            entireSpeed = Mathf.Clamp01(Mathf.Sin(Mathf.PI * 2f * (amplitudeDuration / amplitudeCycleTime - 0.25f)) * amplitudeCycleMultiplier * (amplitudeDuration >= amplitudeCycleTime * 0.5f ? 2f : 1f) + amplitudeCycleOffset);
            float nowAmplitude = entireSpeed * maxAmplitude;
            if (nowAmplitude <= 0f) {
                eulerTemp.x = 0f;
            } else {
                pendulumDuration += delta / Mathf.Lerp(pendulumCycleTime.x, pendulumCycleTime.y, MyMath.Square(nowAmplitude / maxAmplitude));
                if (pendulumDuration >= 1f) {
                    pendulumDuration -= 1f;
                }
                eulerTemp.x = Mathf.Sin(Mathf.PI * 2f * pendulumDuration) * nowAmplitude;
            }
            if (entireSpeed > 0f || zeroSpeedReset) {
                zeroSpeedReset = entireSpeed > 0f;
                rotationDummy.localEulerAngles = eulerTemp;
                shipRigid.MoveRotation(rotationDummy.rotation);
            }
        }
        base.FixedUpdate();
    }

    public override void SetRunning(bool flag) {
        if (flag) {
            amplitudeDuration = amplitudeCycleTime * Random.value;
        } else {
            amplitudeDuration = amplitudeCycleTime * 0.9f;
        }
        pendulumDuration = 0f;
        base.SetRunning(flag);
    }

}
