using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget_Money : MonoBehaviour {
    
    public Rigidbody rb;
    public float delayTime;
    public float targetYOffset;
    public float speed;
    public GameObject targetingDeactivateObj;
    
    protected GameObject targetObj;
    protected float elapsedTime;
    protected float fixedDelta;
    protected bool isTargeting;
    protected const string targetTag = "ItemGetter";
    protected static readonly Vector3 vecUp = Vector3.up;

    protected void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            targetObj = other.gameObject;
            enabled = true;
        }
    }

    protected virtual void FixedUpdate() {
        if (elapsedTime < delayTime) {
            isTargeting = false;
            elapsedTime += Time.fixedDeltaTime;
        } else { 
            if (rb && targetObj) {
                isTargeting = true;
                rb.velocity = ((targetObj.transform.position + vecUp * targetYOffset) - transform.position).normalized * speed;
            } else {
                isTargeting = false;
                enabled = false;
            }
        }
        if (targetingDeactivateObj) {
            if (targetingDeactivateObj.activeSelf == isTargeting) {
                targetingDeactivateObj.SetActive(!isTargeting);
            }
        }
    }
}
