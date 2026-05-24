using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateRandomForce : MonoBehaviour {

    public GameObject prefab;
    public int emitNum;
    public float radius;
    public Vector2 forceRange;
    public bool isHemisphere;
    public float upperBias;
    public Transform parentTransform;
    public bool destroyOnDisable;

    private void OnEnable() {
        Vector3 pivot = transform.position;
        Vector3 direction;
        Vector3 forceRandom;
        for (int i = 0; i < emitNum; i++) {
            float forceBias = Random.Range(forceRange.x, forceRange.y);
            direction = Random.insideUnitSphere;
            forceRandom = Random.insideUnitSphere;
            if (isHemisphere) {
                if (direction.y < 0f) {
                    direction.y *= -1;
                }
            }
            if (upperBias != 0f) {
                forceRandom.y += upperBias;
            }
            Instantiate(prefab, pivot + direction * radius, Random.rotation, parentTransform).GetComponent<Rigidbody>().AddForceAtPosition((direction * 0.9f + forceRandom * 0.1f) * forceBias, pivot, ForceMode.VelocityChange);
        }
    }
}
