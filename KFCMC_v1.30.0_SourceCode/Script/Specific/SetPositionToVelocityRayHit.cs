using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPositionToVelocityRayHit : MonoBehaviour {

    public Rigidbody checkRigidbody;
    public Transform setTarget;
    public float delay = 0.5f;
    public float maxDistance = 10f;
    public LayerMask layerMask;
    public bool fixWorldRotation;
    public bool collideTriggerEnabled;

    private float elapsedTime;
    private RaycastHit hit;
    static readonly Quaternion quaIden = Quaternion.identity;

    void Update() {
        if (elapsedTime >= delay) {
            bool found = false;
            if (checkRigidbody.velocity.sqrMagnitude >= 0.01f) {
                if (Physics.Raycast(checkRigidbody.position, checkRigidbody.velocity.normalized, out hit, maxDistance, layerMask, collideTriggerEnabled ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore)) {
                    setTarget.position = hit.point;
                    found = true;
                }
                if (fixWorldRotation && found) {
                    setTarget.rotation = quaIden;
                }
            }
            if (found) {
                if (!setTarget.gameObject.activeSelf) {
                    setTarget.gameObject.SetActive(true);
                }
            } else {
                if (setTarget.gameObject.activeSelf) {
                    setTarget.gameObject.SetActive(false);
                }
            }
        }
        elapsedTime += Time.deltaTime;
    }
}
