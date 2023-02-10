using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTransformPositionByVelocity : MonoBehaviour
{

    public Rigidbody referRigidbody;
    public Transform pivotTransform;
    public Transform targetTransform;
    public bool normalize;
    public float multiplier = 1;
    public bool targetActivateOnMove;
    public float activateBorderSpeed = 1;

    private void LateUpdate() {
        if (referRigidbody && pivotTransform && targetTransform) {
            Vector3 velocity = referRigidbody.velocity;
            float sqrMagnitude = velocity.sqrMagnitude;
            if (targetActivateOnMove) {
                bool toActive = sqrMagnitude >= activateBorderSpeed * activateBorderSpeed;
                if (targetTransform.gameObject.activeSelf != toActive) {
                    targetTransform.gameObject.SetActive(toActive);
                }
            }
            if (normalize && sqrMagnitude > 0f) {
                velocity.Normalize();
            }
            targetTransform.position = pivotTransform.position + velocity * multiplier;
        }
    }

}
