using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwing : MonoBehaviour {

    [System.Serializable]
    public class ThrowSettings {
        public GameObject prefab;
        public GameObject from;
        public GameObject effect;
        public GameObject cancelEffect;
        public float velocity;
        public Vector3 angularVelocity;
        public Vector3 randomDirection;
        public Vector3 randomDirectionOffset;
        [Range(0.0f, 1.0f)]
        public float randomForceRate;
        public bool lookTarget;
        public float lookGravity = -9.81f;
        public bool setChildRigidbody;
        public bool changeCollisionDetectionMode;
        public bool detectionStartOnThrowStart;
        public GameObject instance;
        [System.NonSerialized]
        public bool isReady = false;
    }

    public ThrowSettings[] throwSettings;

    [System.NonSerialized]
    public Transform target;

    protected CharacterBase cBase;
    protected static readonly Vector3 vecZero = Vector3.zero;
    protected static readonly Vector3 vecForward = Vector3.forward;

    protected virtual void Start() {
        cBase = GetComponent<CharacterBase>();
    }

    public virtual Vector3 LookAtTarget(int index, Transform target, float velocity) {
        if (throwSettings[index].instance && target) {
            Vector3 fromPosition = throwSettings[index].instance.transform.position;
            Vector3 targetPos = target.position;
            Vector3 targetXZ = targetPos;
            targetXZ.y = fromPosition.y;
            float distance = Vector3.Distance(targetPos, fromPosition);
            float reachTime = distance / velocity;
            float fallDist = -0.5f * throwSettings[index].lookGravity * reachTime * reachTime;
            targetPos.y += Mathf.Clamp(fallDist, 0, Vector3.Distance(targetXZ, fromPosition));
            return (targetPos - fromPosition).normalized;
        }
        return transform.TransformDirection(vecForward);
    }

    public virtual void ThrowReady(int index) {
        if (cBase && cBase.IsAttacking() && throwSettings[index].prefab != null && cBase.enabled) {
            throwSettings[index].instance = Instantiate(throwSettings[index].prefab, throwSettings[index].from.transform.position, throwSettings[index].from.transform.rotation, throwSettings[index].from.transform);
            if (throwSettings[index].instance != null) {
                throwSettings[index].isReady = true;
                Rigidbody rb = throwSettings[index].instance.GetComponent<Rigidbody>();
                if (rb != null) {
                    if (throwSettings[index].changeCollisionDetectionMode) {
                        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    }
                    rb.isKinematic = true;
                }
                if (throwSettings[index].setChildRigidbody) {
                    Rigidbody[] rbs = throwSettings[index].instance.GetComponentsInChildren<Rigidbody>();
                    for (int i = 0; i < rbs.Length; i++) {
                        if (throwSettings[index].changeCollisionDetectionMode) {
                            rbs[i].collisionDetectionMode = CollisionDetectionMode.Discrete;
                        }
                        rbs[i].isKinematic = true;
                    }
                }
            }
        }
    }

    protected void ThrowRigid(Rigidbody rb, Vector3 force, Vector3 angularVelocity, bool changeMode) {
        rb.isKinematic = false;
        if (changeMode) {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        rb.AddForce(force, ForceMode.VelocityChange);
        if (angularVelocity != vecZero) {
            rb.AddRelativeTorque(angularVelocity, ForceMode.VelocityChange);
        }
    }

    public virtual void ThrowStart(int index) {
        if (!throwSettings[index].isReady && throwSettings[index].prefab != null && cBase && cBase.enabled) {
            throwSettings[index].instance = Instantiate(throwSettings[index].prefab, throwSettings[index].from.transform.position, throwSettings[index].from.transform.rotation, throwSettings[index].from.transform);
        }
        if (throwSettings[index].instance != null) {
            float velocity = throwSettings[index].velocity;
            if (throwSettings[index].randomForceRate != 0) {
                velocity *= Random.Range(1.0f - throwSettings[index].randomForceRate, 1.0f + throwSettings[index].randomForceRate);
            }
            throwSettings[index].isReady = false;
            if (throwSettings[index].effect != null) {
                Instantiate(throwSettings[index].effect, throwSettings[index].instance.transform.position, throwSettings[index].from.transform.rotation);
            }
            throwSettings[index].instance.transform.SetParent(null);
            Vector3 dir = throwSettings[index].from.transform.TransformDirection(vecForward);
            if (cBase && cBase.target) {
                target = cBase.target.transform;
            }
            if (throwSettings[index].lookTarget && target) {
                dir = LookAtTarget(index, target, velocity);
            }
            if (throwSettings[index].randomDirection != vecZero) {
                dir += Vector3.Scale(Random.insideUnitSphere * 0.5f, throwSettings[index].randomDirection);
            }
            if (throwSettings[index].randomDirectionOffset != vecZero) {
                dir += throwSettings[index].randomDirectionOffset;
            }
            dir.Normalize();
            Rigidbody rb = throwSettings[index].instance.GetComponent<Rigidbody>();
            if (rb != null) {
                ThrowRigid(rb, dir * velocity, throwSettings[index].angularVelocity, throwSettings[index].changeCollisionDetectionMode);
            }
            if (throwSettings[index].setChildRigidbody) {
                Rigidbody[] rbs = throwSettings[index].instance.GetComponentsInChildren<Rigidbody>();
                for (int i = 0; i < rbs.Length; i++) {
                    ThrowRigid(rbs[i], dir * velocity, throwSettings[index].angularVelocity, throwSettings[index].changeCollisionDetectionMode);
                }
            }
            if (throwSettings[index].detectionStartOnThrowStart) {
                AttackDetectionProjectile adTemp = throwSettings[index].instance.GetComponentInChildren<AttackDetectionProjectile>();
                if (adTemp) {
                    adTemp.DetectionStart();
                }
            }
        }
    }

    public virtual void ThrowCancel(int index, bool onlyReady = false) {
        if (throwSettings[index].instance != null && (!onlyReady || throwSettings[index].instance.transform.parent != null)) {
            if (throwSettings[index].cancelEffect) {
                Instantiate(throwSettings[index].cancelEffect, throwSettings[index].instance.transform.position, Quaternion.identity);
            }
            Destroy(throwSettings[index].instance);
            throwSettings[index].instance = null;
            throwSettings[index].isReady = false;
        }
    }

    public virtual void ThrowCancelAll(bool onlyReady = false) {
        for (int i = 0; i < throwSettings.Length; i++) {
            if (throwSettings[i].instance != null && (!onlyReady || throwSettings[i].instance.transform.parent != null)) {
                if (throwSettings[i].cancelEffect) {
                    Instantiate(throwSettings[i].cancelEffect, throwSettings[i].instance.transform.position, Quaternion.identity);
                }
                Destroy(throwSettings[i].instance);
                throwSettings[i].instance = null;
                throwSettings[i].isReady = false;
            }
        }
    }

    public bool GetAnyReady() {
        for (int i = 0; i < throwSettings.Length; i++) {
            if (throwSettings[i].instance != null && throwSettings[i].isReady) {
                return true;
            }
        }
        return false;
    }

    public bool GetIsReady(int index) {
        if (throwSettings[index].instance != null && throwSettings[index].isReady) {
            return true;
        }
        return false;
    }


}
