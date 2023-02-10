using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothLookTarget : MonoBehaviour {

    public Transform target;
    public float minDistance = 0.1f;
    public float smoothSpeed = 5f;

    // Update is called once per frame
    void Update() {
        if (target && Time.timeScale > 0) {
            Vector3 forward = target.position - transform.position;
            if (forward.sqrMagnitude >= minDistance * minDistance) {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), Time.deltaTime * smoothSpeed);
            }
        }
    }

}
