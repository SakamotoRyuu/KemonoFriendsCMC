using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCollisionLayer_IceEffect : StopCollisionLayer
{

    public float forceDestroyTimer;
    public float stoppedDestroyTimer;
    public GameObject effectPrefab;

    float timeRemain;

    protected override void Awake() {
        base.Awake();
        timeRemain = forceDestroyTimer;
    }

    private void Update() {
        timeRemain -= Time.deltaTime;
        if (timeRemain < 0f) {
            if (effectPrefab) {
                Instantiate(effectPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    protected override void HitLayer(Transform other) {
        base.HitLayer(other);
        timeRemain = stoppedDestroyTimer;
    }

}
