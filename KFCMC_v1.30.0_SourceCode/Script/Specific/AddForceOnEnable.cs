using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceOnEnable : MonoBehaviour {

    public Rigidbody targetRigidbody;
    public float power;
    public ForceMode forceMode;
    public bool useGravity;
    public bool isKinematic;

    private void OnEnable() {
        if (targetRigidbody) {
            targetRigidbody.useGravity = useGravity;
            targetRigidbody.isKinematic = isKinematic;
            targetRigidbody.AddForce(transform.TransformDirection(Vector3.forward) * power, forceMode);
        }
    }

}
