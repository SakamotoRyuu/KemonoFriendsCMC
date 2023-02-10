using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation_AwakeRandomize : AutoRotation {

    public Vector3 randomRange;

    private void Awake() {
        if (randomRange.x != 0f) {
            rotSpeed.x += Random.Range(randomRange.x * -0.5f, randomRange.x * 0.5f);
        }
        if (randomRange.y != 0f) {
            rotSpeed.y += Random.Range(randomRange.y * -0.5f, randomRange.y * 0.5f);
        }
        if (randomRange.z != 0f) {
            rotSpeed.z += Random.Range(randomRange.z * -0.5f, randomRange.z * 0.5f);
        }
    }

}
