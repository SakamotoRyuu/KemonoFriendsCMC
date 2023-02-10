using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorFarClipToRaycastHit : MonoBehaviour {

    public Projector projector;
    public Transform pivot;
    public LayerMask layerMask;
    public float minDistance;
    public float maxDistance;
    public float distanceToAdd;

    Ray ray = new Ray();
    RaycastHit hitInfo;
    float distanceSave;

    void Update() {
        if (projector && pivot) {
            ray.origin = pivot.position;
            ray.direction = pivot.forward;
            if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore)) {
                float distanceTemp = Mathf.Clamp(Vector3.Distance(hitInfo.point, pivot.position) + distanceToAdd, minDistance, maxDistance);
                if (distanceSave != distanceTemp) {
                    distanceSave = distanceTemp;
                    projector.farClipPlane = distanceSave;
                }
            } else {
                if (distanceSave != maxDistance) {
                    distanceSave = maxDistance;
                    projector.farClipPlane = distanceSave;
                }
            }
        }
    }
}
