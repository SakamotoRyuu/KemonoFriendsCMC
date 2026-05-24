using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRotationOnGroundNormal : MonoBehaviour {

    public Transform target;
    public LayerMask rayLayer;
    public Vector3 offset;
    public float distance;
    public float maxAngle;
    public bool activateTarget;

    private void Awake() {
        if (target) {
            Ray ray = new Ray(target.position + offset, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, rayLayer, QueryTriggerInteraction.Ignore)) {
                Vector3 groundNormal = hitInfo.normal;
                if (groundNormal.y > 0f && maxAngle >= Vector3.Angle(groundNormal, Vector3.up)) {
                    target.SetPositionAndRotation(hitInfo.point, Quaternion.FromToRotation(Vector3.up, groundNormal));
                }
            }
            if (activateTarget) {
                target.gameObject.SetActive(true);
            }
        }
    }

}
