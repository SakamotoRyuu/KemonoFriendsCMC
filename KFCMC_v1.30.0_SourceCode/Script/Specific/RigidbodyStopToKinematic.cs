using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyStopToKinematic : MonoBehaviour {

    public Rigidbody rigid;
    public float delay = 0.8f;
    public float limitSpeed = 0.1f;
    public float duration = 0.1f;

    float elapsedTime = 0f;
    int progress = 0;

    void Update() {
        if (progress == 0) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= delay) {
                elapsedTime = 0f;
                progress = 1;
            }
        } else if (progress == 1 && rigid) {
            if ((rigid.velocity).sqrMagnitude <= limitSpeed * limitSpeed) {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= duration) {
                    rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    rigid.isKinematic = true;
                    rigid.useGravity = false;
                    progress = 2;
                }
            } else {
                elapsedTime = 0f;
            }
        }
    }
}
