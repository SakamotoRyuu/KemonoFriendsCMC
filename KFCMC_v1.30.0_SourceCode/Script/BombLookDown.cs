using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombLookDown : MonoBehaviour {

    Rigidbody rb;
    Transform trans;
    static readonly Vector3 vecZero = Vector3.zero;
    static readonly Vector3 vecRight = Vector3.right;
    
	void Start () {
        rb = GetComponent<Rigidbody>();
        trans = transform;
	}
	
	void Update () {
        if (rb) {
            if (trans.rotation.eulerAngles.x < 85f) {
                rb.AddTorque(trans.TransformDirection(vecRight) * 2 * Time.deltaTime, ForceMode.VelocityChange);
            } else {
                rb.AddTorque(vecZero, ForceMode.VelocityChange);
            }
        }
	}
}
