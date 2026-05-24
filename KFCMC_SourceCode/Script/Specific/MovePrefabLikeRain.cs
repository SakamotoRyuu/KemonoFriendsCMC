using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePrefabLikeRain : MonoBehaviour
{

    public GameObject prefab;
    public int emitCount;
    public Vector2 appearHeight;
    public float dissapearHeight;
    public float horizontalRadius;
    public Vector3 forceVelocity;
    public bool destroyOnDisable;

    Vector3 pivotPos;
    Transform[] instanceTransform;
    Transform trans;
    bool initialized;
    float interval;

    private void Start() {
        if (!destroyOnDisable) {
            Ready();
        }
    }

    private void OnEnable() {
        if (destroyOnDisable) {
            Ready();
        }
    }

    private void OnDisable() {
        if (destroyOnDisable && initialized) {
            for (int i = 0; i < instanceTransform.Length; i++) {
                if (instanceTransform[i]) {
                    Destroy(instanceTransform[i].gameObject);
                }
            }
        }
    }

    private void Ready() {
        if (!initialized) {
            instanceTransform = new Transform[emitCount];
            initialized = true;
        }
        trans = transform;
        pivotPos = trans.position;
        for (int i = 0; i < emitCount; i++) {
            Vector3 randPos = pivotPos;
            Vector3 randCircle = Random.insideUnitCircle * horizontalRadius;
            randPos.x += randCircle.x;
            randPos.y += Random.Range(dissapearHeight, Random.Range(appearHeight.x, appearHeight.y));
            randPos.z += randCircle.y;
            instanceTransform[i] = Instantiate(prefab, randPos, Random.rotation, trans).transform;
            instanceTransform[i].GetComponent<Rigidbody>().AddForce(forceVelocity, ForceMode.VelocityChange);
        }
    }

    private void Update() {
        if (initialized) {
            interval -= Time.deltaTime;
            if (interval <= 0f) {
                interval = 0.175f;
                float dissapearCondition = trans.position.y + dissapearHeight;
                for (int i = 0; i < emitCount; i++) {
                    if (instanceTransform[i] && instanceTransform[i].position.y < dissapearCondition) {
                        Vector3 randPos = pivotPos;
                        Vector3 randCircle = Random.insideUnitCircle * horizontalRadius;
                        randPos.x += randCircle.x;
                        randPos.y += Random.Range(appearHeight.x, appearHeight.y);
                        randPos.z += randCircle.y;
                        instanceTransform[i].position = randPos;
                    }
                }
            }
        }
    }

}
