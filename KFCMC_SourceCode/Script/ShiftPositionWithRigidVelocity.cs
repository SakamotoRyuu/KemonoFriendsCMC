using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftPositionWithRigidVelocity : MonoBehaviour {

    public Transform positionPivot;
    public Transform shiftTarget;
    public Rigidbody checkRigid;
    public float effectRate = 0.01f;

    void FixedUpdate() {
        if (positionPivot && shiftTarget && checkRigid && Time.timeScale > 0) {
            shiftTarget.position = positionPivot.position + checkRigid.velocity * effectRate;
        }
    }
}
