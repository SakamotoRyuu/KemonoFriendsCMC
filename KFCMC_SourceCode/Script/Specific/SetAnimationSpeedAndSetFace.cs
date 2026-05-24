using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mebiustos.MMD4MecanimFaciem;

public class SetAnimationSpeedAndSetFace : MonoBehaviour {

    public float animationSpeed = 1f;
    public string faceName;

    void Start() {
        Animator animator = GetComponent<Animator>();
        if (animator) {
            animator.speed = animationSpeed;
        }
        if (!string.IsNullOrEmpty(faceName)) {
            FaciemController fCon = GetComponent<FaciemController>();
            if (fCon) {
                fCon.SetFace(faceName);
            }
        }
    }

}
