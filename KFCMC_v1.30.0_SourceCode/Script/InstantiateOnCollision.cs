using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateOnCollision : MonoBehaviour {

    public GameObject prefab;
    public Rigidbody targetRigidbody;
    public float velocityThreshold = 4f;
    public float timeThreshold = 0.1f;
    public bool adjustAudioVolume;
    public float volumeMaxVelocity = 24f;
    public bool superBounce;
    public float bouncePlus;
    public float bounceSpeedMin = 25f;
    public float bounceSpeedMax = 250f;
    public float destroyOnBounceTimer = 20f;

    int bounceCount;
    float elapsedTime;
    float timeCount;
    float bounceRateRoot;
    CharacterBase cBase;
    Transform trans;
    static readonly Quaternion quaIden = Quaternion.identity;

    private void Awake() {
        trans = transform;
        cBase = GetComponentInParent<CharacterBase>();
    }

    private void Update() {
        float deltaTimeCache = Time.deltaTime;
        timeCount += deltaTimeCache;
        elapsedTime += deltaTimeCache;
    }
    
    private void OnCollisionEnter(Collision collision) {
        if (elapsedTime >= destroyOnBounceTimer) {
            Destroy(gameObject);
        } else {
            Vector3 bouncedVelocity = targetRigidbody.velocity;
            Vector3 relativeVelocity = collision.relativeVelocity;
            if (timeCount >= timeThreshold && relativeVelocity.sqrMagnitude >= velocityThreshold * velocityThreshold) {
                timeCount = 0f;
                if (adjustAudioVolume) {
                    Instantiate(prefab, collision.contacts[0].point, quaIden).GetComponent<AudioSource>().volume = Mathf.Min(relativeVelocity.magnitude, volumeMaxVelocity) / volumeMaxVelocity;
                } else {
                    Instantiate(prefab, collision.contacts[0].point, quaIden);
                }
                if (superBounce && cBase) {
                    bounceCount++;
                    if (bounceCount >= 3 && cBase.targetTrans) {
                        Vector3 targetDir = cBase.targetTrans.position - trans.position;
                        if (Vector3.Angle(targetDir, bouncedVelocity) < Mathf.Min(30f + bounceCount * 10f, 180f)) {
                            bounceCount = 0;
                            float newSpeed = Mathf.Clamp(bouncedVelocity.magnitude + bouncePlus, bounceSpeedMin, bounceSpeedMax);
                            float distance = Vector3.Distance(cBase.targetTrans.position, trans.position);
                            float reachTime = distance / newSpeed;
                            float fallDist = Mathf.Clamp(-0.5f * -9.81f * reachTime * reachTime, 0f, distance);
                            Vector3 targetPos = cBase.targetTrans.position;
                            targetPos.y += fallDist;
                            targetDir = targetPos - trans.position;
                            targetRigidbody.velocity = targetDir.normalized * newSpeed;
                        }
                    }
                }
            }
        }
    }

}
