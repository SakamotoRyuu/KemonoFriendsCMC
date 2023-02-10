using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCenterOfMass : MonoBehaviour {

    public Rigidbody targetRigidbody;
    public Vector3 centerOfMass;

    void Start() {
        if (targetRigidbody) {
            targetRigidbody.centerOfMass = centerOfMass;
        }
    }

}
