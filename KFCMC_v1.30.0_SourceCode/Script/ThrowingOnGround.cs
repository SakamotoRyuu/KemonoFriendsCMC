using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingOnGround : ThrowingRandom {

    public Transform rayPivot;
    public LayerMask groundLayer;
    public float maxDistance;
    public bool applyGroundNormal;
    public float groundMaxAngle;

    static readonly Vector3 vecDown = Vector3.down;
    static readonly Vector3 vecUp = Vector3.up;

    public override void ThrowStart(int index) {
        Ray ray = new Ray(rayPivot.position, vecDown);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance, groundLayer, QueryTriggerInteraction.Ignore)) {
            throwSettings[index].from.transform.position = hit.point;
            base.ThrowStart(index);
            if (applyGroundNormal && throwSettings[index].instance) {
                Vector3 groundNormal = hit.normal;
                if (groundNormal.y > 0f && groundMaxAngle >= Vector3.Angle(groundNormal, vecUp)) {
                    throwSettings[index].instance.transform.rotation = Quaternion.FromToRotation(vecUp, groundNormal);
                }
            }
        } else {
            ThrowCancelAll();
        }
    }

}
