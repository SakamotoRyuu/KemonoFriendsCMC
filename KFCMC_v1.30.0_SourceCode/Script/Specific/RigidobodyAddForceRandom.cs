using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidobodyAddForceRandom : MonoBehaviour {

    public Rigidbody[] targetRigidbody;
    public Vector2 power;
    public ForceMode forceMode;
    public Vector3 baseDirection;
    public Vector2 startHeightRandom;
    public float positionRandomizeRadius;
    public float forceRandomizeRate;
    private Vector3 circleToVec3;

    private void OnEnable() {
        for (int i = 0; i < targetRigidbody.Length; i++) {
            if (targetRigidbody[i]) {
                Vector2 circlePoint = Random.insideUnitCircle;
                if (positionRandomizeRadius > 0f) {
                    circleToVec3.x = circlePoint.x * positionRandomizeRadius;
                    circleToVec3.y = Random.Range(startHeightRandom.x, startHeightRandom.y);
                    circleToVec3.z = circlePoint.y * positionRandomizeRadius;
                    targetRigidbody[i].transform.localPosition = circleToVec3;
                }
                circleToVec3.x = circlePoint.x;
                circleToVec3.y = 0f;
                circleToVec3.z = circlePoint.y;
                targetRigidbody[i].AddForce((baseDirection + (Random.insideUnitSphere * 0.25f + circleToVec3 * 0.75f) * forceRandomizeRate).normalized * Random.Range(power.x, power.y), forceMode);
            }
        }
    }
}
