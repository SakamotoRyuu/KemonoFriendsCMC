using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTrigger : MonoBehaviour {

    [System.Serializable]
    public class AdditionalTrap {
        public bool enabled;
        public GameObject prefab;
        public int conditionLevel = 0;
        public bool applyGroundNormal;
    }

    public GameObject attackObject;
    public string targetTag = "";
    public float triggerDelay = 0.1f;
    public float startRadius = 0.01f;
    public float endRadius = 1f;
    public float inflationTime = 0.1f;
    public float endTime = 0.2f;
    public float forceBurstTime = 5f;
    public float forceBurstRandomRange = 0.1f;
    public float destroyTime = 10f;
    public float parentDestroyDelay = 0f;
    public bool adjustScaleOnStart = true;
    public bool attackStartOnBurst = false;
    public GameObject missingDestroyTarget;
    public AdditionalTrap additionalTrap;
    public bool restrictLayerEnabled;
    public LayerMask conditionLayer;
    public ParticleSystem[] remainParticles;

    protected AttackDetection attackDetection;
    protected CapsuleCollider attackCollider;
    protected CharacterBase parentCBase;
    protected bool parentFound;
    protected float duration;
    protected bool entered;
    protected bool inflated;
    protected bool ended;
    protected float forceBurstTimeReal;

    private void Awake() {
        if (attackObject) {
            attackDetection = attackObject.GetComponent<AttackDetection>();
            attackCollider = attackObject.GetComponent<CapsuleCollider>();
            if (forceBurstRandomRange != 0f) {
                forceBurstTimeReal = forceBurstTime + Random.Range(forceBurstRandomRange * -0.5f, forceBurstRandomRange * 0.5f);
            } else {
                forceBurstTimeReal = forceBurstTime;
            }
            parentCBase = GetComponentInParent<CharacterBase>();
            if (parentCBase) {
                parentFound = true;
            }
        } else {
            gameObject.SetActive(false);
        }
    }

    private void Start() {
        if (attackCollider && attackDetection) {
            attackCollider.enabled = false;
            attackCollider.radius = startRadius;
            attackDetection.enabled = false;
            attackDetection.DetectionEnd(false);
        }
        if (adjustScaleOnStart) {
            Vector3 lossy = transform.lossyScale;
            Vector3 newScale = Vector3.one;
            if (lossy != newScale) {
                if (lossy.x > 0f) {
                    newScale.x = 1f / lossy.x;
                }
                if (lossy.y > 0f) {
                    newScale.y = 1f / lossy.y;
                }
                if (lossy.z > 0f) {
                    newScale.z = 1f / lossy.z;
                }
                transform.localScale = newScale;
            }
        }
    }

    protected virtual void SetAdditionalTrap() {
        if (additionalTrap.enabled && parentCBase) {
            bool check = true;
            if (additionalTrap.conditionLevel >= 1) {
                if (parentCBase.Level < additionalTrap.conditionLevel) {
                    check = false;
                }
            }
            if (check) {
                LayerMask layerMask = LayerMask.GetMask("Field");
                RaycastHit hit;
                Ray ray = new Ray(transform.position + new Vector3(0f, 0.5f, 0f), Vector3.down);
                if (Physics.Raycast(ray, out hit, 2f, layerMask, QueryTriggerInteraction.Ignore)) {
                    Vector3 pointTemp = hit.point;
                    pointTemp.y += Random.Range(0.0025f, 0.005f);
                    GameObject trapInstance = Instantiate(additionalTrap.prefab, hit.point, additionalTrap.applyGroundNormal && hit.normal.y > 0f ? Quaternion.FromToRotation(Vector3.up, hit.normal) : Quaternion.identity);
                    if (trapInstance) {
                        MissingObjectToDestroy trapChecker = trapInstance.GetComponent<MissingObjectToDestroy>();
                        if (trapChecker) {
                            trapChecker.SetGameObject(parentCBase.gameObject);
                        }
                    }
                }
            }
        }
    }

    protected virtual void Update() {
        if (parentFound && missingDestroyTarget && parentCBase == null) {
            Destroy(missingDestroyTarget);
        } else {
            duration += Time.deltaTime;
            if (entered) {
                if (duration >= destroyTime && destroyTime < 10000 && ended) {
                    Destroy(gameObject);
                } else if (duration >= endTime && inflated) {
                    if (!ended) {
                        attackDetection.DetectionEnd(false);
                        ended = true;
                    }
                } else if (duration >= inflationTime) {
                    if (!inflated) {
                        if (attackCollider != null) {
                            attackCollider.radius = endRadius;
                        }
                        inflated = true;
                    }
                } else if (inflationTime > 0 && attackCollider != null) {
                    attackCollider.radius = Mathf.Lerp(startRadius, endRadius, duration / inflationTime);
                }
            } else {
                if (duration >= forceBurstTimeReal && forceBurstTimeReal < 10000) {
                    Burst();
                }
            }
        }
    }

    public void Burst() {
        if (!entered) {
            entered = true;
            attackCollider.enabled = true;
            attackCollider.radius = startRadius;
            attackDetection.enabled = true;
            if (attackStartOnBurst) {
                attackDetection.DetectionStart();
            }
            if (parentDestroyDelay >= 0) {
                Transform parentTransform = transform.parent;
                if (parentTransform) {
                    transform.SetParent(null);
                    if (remainParticles.Length > 0) {
                        for (int i = 0; i < remainParticles.Length; i++) {
                            if (remainParticles[i]) {
                                remainParticles[i].Stop();
                                remainParticles[i].transform.SetParent(null);
                            }
                        }
                    }
                    Destroy(parentTransform.gameObject, parentDestroyDelay);
                }
            }
            SetAdditionalTrap();
            duration = 0;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!entered && duration >= triggerDelay && (string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag)) && (!restrictLayerEnabled || ((1 << other.gameObject.layer) & conditionLayer) != 0)) {
            Burst();
        }
    }

}
