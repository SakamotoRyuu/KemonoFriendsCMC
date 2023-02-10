using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastToAdjustCapsuleCollider : MonoBehaviour {

    [System.Serializable]
    public class ColliderSet {
        public CapsuleCollider targetCollider;
        public float heightBias;
    }

    [System.Serializable]
    public class TransformSet {
        public Transform targetTransform;
        public bool changeScaleX;
        public bool changeScaleY;
        public bool changeScaleZ;
        public float heightBias;
    }

    public float maxDistance = 20f;
    public LayerMask layerMask;
    public Transform pivot;
    public ColliderSet[] colliderSet;
    public TransformSet[] transformSet;
    public Transform[] hitEffect;
    public bool enableHitEffectOnlyLayer;
    public LayerMask hitEffectOnlyLayer;

    [System.NonSerialized]
    public bool hitEffectEnabled;

    Vector3[] defaultScale;
    bool isMax = false;
    Ray ray;
    RaycastHit hit;
    static readonly Vector3 vecForward = Vector3.forward;
    
    private void Awake() {
        defaultScale = new Vector3[transformSet.Length];
        for (int i = 0; i < defaultScale.Length; i++) {
            if (transformSet[i].targetTransform) {
                defaultScale[i] = transformSet[i].targetTransform.localScale;
            }
        }
        Deactivate();
    }

    void SetHitEffect(Vector3 position) {
        for (int i = 0; i < hitEffect.Length; i++) {
            if (hitEffect[i]) {
                hitEffect[i].position = position;
                if (hitEffectEnabled) {
                    if (!hitEffect[i].gameObject.activeSelf) {
                        hitEffect[i].gameObject.SetActive(true);
                    }
                } else {
                    if (hitEffect[i].gameObject.activeSelf) {
                        hitEffect[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    void Process() {
        if (pivot) {
            ray.origin = pivot.position;
            ray.direction = pivot.TransformDirection(vecForward);
            if (maxDistance > 0f && Physics.Raycast(ray, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore)) {
                isMax = false;
                float hitDistance = hit.distance;
                for (int i = 0; i < colliderSet.Length; i++) {
                    if (colliderSet[i].targetCollider) {
                        float height = Mathf.Clamp(hitDistance + colliderSet[i].heightBias, 0.1f, maxDistance);
                        int dir = colliderSet[i].targetCollider.direction;
                        colliderSet[i].targetCollider.height = height;
                        colliderSet[i].targetCollider.center = new Vector3(dir == 0 ? height / 2f : 0f, dir == 1 ? height / 2f : 0f, dir == 2 ? height / 2f : 0f);
                    }
                }
                for (int i = 0; i < transformSet.Length; i++) {
                    if (transformSet[i].targetTransform) {
                        Vector3 scaleTemp = defaultScale[i];
                        float scaleRate = Mathf.Clamp(hitDistance + transformSet[i].heightBias, 0.1f, maxDistance) / maxDistance;
                        if (transformSet[i].changeScaleX) {
                            scaleTemp.x *= scaleRate;
                        }
                        if (transformSet[i].changeScaleY) {
                            scaleTemp.y *= scaleRate;
                        }
                        if (transformSet[i].changeScaleZ) {
                            scaleTemp.z *= scaleRate;
                        }
                        transformSet[i].targetTransform.localScale = scaleTemp;
                    }
                }
                if (!enableHitEffectOnlyLayer) {
                    SetHitEffect(hit.point);
                }
            } else if (!isMax) {
                isMax = true;
                for (int i = 0; i < colliderSet.Length; i++) {
                    if (colliderSet[i].targetCollider) {
                        float height = maxDistance + colliderSet[i].heightBias;
                        int dir = colliderSet[i].targetCollider.direction;
                        colliderSet[i].targetCollider.height = height;
                        colliderSet[i].targetCollider.center = new Vector3(dir == 0 ? height / 2f : 0f, dir == 1 ? height / 2f : 0f, dir == 2 ? height / 2f : 0f);
                    }
                }
                for (int i = 0; i < transformSet.Length; i++) {
                    if (transformSet[i].targetTransform) {
                        transformSet[i].targetTransform.localScale = defaultScale[i];
                    }
                }
                for (int i = 0; i < hitEffect.Length; i++) {
                    if (hitEffect[i]) {
                        if (hitEffect[i].gameObject.activeSelf) {
                            hitEffect[i].gameObject.SetActive(false);
                        }
                    }
                }
            }
            if (enableHitEffectOnlyLayer) {
                if (maxDistance > 0f && Physics.Raycast(ray, out hit, maxDistance, hitEffectOnlyLayer, QueryTriggerInteraction.Ignore)) {
                    SetHitEffect(hit.point);
                }
            }
        }
    }
    
    void Update() {
        Process();
    }

    public void Activate() {
        enabled = true;
        isMax = false;
        Process();
    }

    public void Deactivate() {
        enabled = false;
        for (int i = 0; i < hitEffect.Length; i++) {
            if (hitEffect[i]) {
                hitEffect[i].gameObject.SetActive(false);
            }
        }
    }

}