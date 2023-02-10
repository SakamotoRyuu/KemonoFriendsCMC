using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {

    public enum FollowMode { Acceleration, Force, VelocityChange, Impulse, ConstantForward, Assignment, Disabled };

    public bool referenceParentCharacterTarget;
    public Rigidbody rb;
    public float delayTime = 1.0f;
    public float targetYOffset = 0.8f;
    public float speed = 5.0f;
    public string targetTag = "ItemDetection";
    public FollowMode followMode;
    public bool alwaysAddForce;
    public bool lookForward;
    public float lookSpeed = 5f;
    public bool disableOnKinematic = false;
    public GameObject targetingDeactivateObj;

    protected CharacterBase parentCBase;
    protected GameObject targetObj;
    protected float elapsedTime;
    protected Vector3 velocity;
    protected Transform trans;
    protected float fixedDelta;
    protected bool isTargeting;
    protected GameObject decoySave;
    protected static readonly Vector3 vecZero = Vector3.zero;
    protected static readonly Vector3 vecUp = Vector3.up;
    protected static readonly Vector3 vecForward = Vector3.forward;

    protected virtual void Awake() {
        trans = transform;
        if (rb == null) {
            rb = GetComponentInParent<Rigidbody>();
        }
        targetObj = null;
        if (referenceParentCharacterTarget) {
            parentCBase = GetComponentInParent<CharacterBase>();
            if (parentCBase && parentCBase.target) {
                targetObj = parentCBase.target;
            }
        }
        velocity = vecZero;
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (referenceParentCharacterTarget && decoySave) {
            targetObj = decoySave;
            return;
        }
        if (referenceParentCharacterTarget && parentCBase && parentCBase.target != null) {
            if (targetObj != parentCBase.target) {
                targetObj = parentCBase.target;
            }
        } else {
            if (!string.IsNullOrEmpty(targetTag) && other.CompareTag(targetTag)) {
                if (targetObj) {
                    if ((trans.position - other.transform.position).sqrMagnitude < (trans.position - targetObj.transform.position).sqrMagnitude) {
                        targetObj = other.gameObject;
                    }
                } else {
                    targetObj = other.gameObject;
                }
            }
        }
    }

    protected virtual void LateUpdate() {
        if (referenceParentCharacterTarget && parentCBase) {
            if (parentCBase.GetNowDecoy && parentCBase.GetNowDecoy == parentCBase.target) {
                targetObj = decoySave = parentCBase.GetNowDecoy;
            } else if (decoySave == null && parentCBase.target != null && targetObj != parentCBase.target) {
                targetObj = parentCBase.target;
            }
        }
    }

    protected virtual void FixedUpdate() {
        fixedDelta = Time.fixedDeltaTime;
        isTargeting = false;
        if (elapsedTime >= delayTime) {
            if (rb != null && (!disableOnKinematic || !rb.isKinematic)) {
                if (alwaysAddForce || targetObj) {
                    if (!targetObj) {
                        velocity = trans.TransformDirection(vecForward);
                    } else {
                        velocity = ((targetObj.transform.position + vecUp * targetYOffset) - trans.position).normalized;
                    }
                    if (lookForward && velocity != vecZero) {
                        rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(velocity), lookSpeed * fixedDelta));
                    }
                    isTargeting = true;
                    velocity *= speed;
                    switch (followMode) {
                        case FollowMode.Acceleration:
                            rb.AddForce(velocity, ForceMode.Acceleration);
                            break;
                        case FollowMode.Force:
                            rb.AddForce(velocity, ForceMode.Force);
                            break;
                        case FollowMode.VelocityChange:
                            rb.AddForce(velocity, ForceMode.VelocityChange);
                            break;
                        case FollowMode.Impulse:
                            rb.AddForce(velocity, ForceMode.Impulse);
                            break;
                        case FollowMode.ConstantForward:
                            rb.MovePosition(rb.transform.position + rb.transform.TransformDirection(vecForward) * speed * fixedDelta);
                            break;
                        case FollowMode.Disabled:
                            break;
                        case FollowMode.Assignment:
                            rb.velocity = velocity;
                            break;
                    }
                }
            }
        }
        if (targetingDeactivateObj) {
            if (targetingDeactivateObj.activeSelf == isTargeting) {
                targetingDeactivateObj.SetActive(!isTargeting);
            }
        }
        elapsedTime += fixedDelta;
    }
}
