using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyRandom : AutoDestroy {

    public float randomMin = -0.5f;
    public float randomMax = 0.5f;

    private void Awake() {
        life += Random.Range(randomMin, randomMax);
    }

}
