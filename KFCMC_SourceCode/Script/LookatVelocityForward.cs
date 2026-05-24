using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookatVelocityForward : MonoBehaviour {

    public Rigidbody targetRigidbody;
    public bool enabledAlsoKinematic = false;
    public float thresholdSpeed = 1f;
    public float smoothSpeed = 180f;    

    void FixedUpdate() {
        if ((enabledAlsoKinematic || !targetRigidbody.isKinematic) && targetRigidbody.velocity.sqrMagnitude >= thresholdSpeed * thresholdSpeed) {
            targetRigidbody.MoveRotation(Quaternion.RotateTowards(targetRigidbody.rotation, Quaternion.LookRotation(targetRigidbody.velocity), smoothSpeed * Time.fixedDeltaTime));
        }
    }
}
