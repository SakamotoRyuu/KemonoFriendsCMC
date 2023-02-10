using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCollisionLayer_ToInstantiate : StopCollisionLayer {

    public GameObject prefab;
    public bool prefabParenting;
    public bool inheritRotation;
    public bool toDownRayPosition;
    public LayerMask downRayLayer;
    public float releaseTime = -1f;

    float elapsedTime;

    private void Update() {
        if (releaseTime > 0f) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= releaseTime && controlRigidbody && controlRigidbody.isKinematic) {
                controlRigidbody.isKinematic = false;
                releaseTime = -1f;
            }
        }
    }

    protected override void HitLayer(Transform other) {
        base.HitLayer(other);
        Vector3 targetPos = transform.position;
        if (toDownRayPosition) {
            Ray ray = new Ray(targetPos + Vector3.up, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 3f, downRayLayer, QueryTriggerInteraction.Ignore)) {
                targetPos = hit.point;
            }
        }
        if (prefabParenting) {
            Instantiate(prefab, targetPos, inheritRotation ? transform.rotation : Quaternion.identity, transform);
        } else {
            Instantiate(prefab, targetPos, inheritRotation ? transform.rotation : Quaternion.identity);
        }
    }

}
