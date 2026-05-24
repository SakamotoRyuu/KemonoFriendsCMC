using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCollisionLayer : MonoBehaviour {

    public LayerMask targetLayerMask;
    public Rigidbody controlRigidbody;
    public bool parenting = false;

    protected int layerValue;
    protected bool stopped = false;

    protected virtual void Awake() {
        if (!controlRigidbody) {
            controlRigidbody = GetComponent<Rigidbody>();
        }
        layerValue = targetLayerMask.value;
    }

    protected virtual void HitLayer(Transform other) {
        if (controlRigidbody) {
            controlRigidbody.velocity = Vector3.zero;
            controlRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            controlRigidbody.isKinematic = true;
        }
        if (parenting) {
            controlRigidbody.transform.SetParent(other);
        }
        stopped = true;
    }

    protected void OnTriggerEnter(Collider other) {
        if (!stopped && ((1 << other.gameObject.layer) & layerValue) != 0) {
            HitLayer(other.transform);
        }
    }
}
