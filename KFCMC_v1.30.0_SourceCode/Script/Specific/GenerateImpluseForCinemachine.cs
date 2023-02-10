using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GenerateImpluseForCinemachine : MonoBehaviour {

    CinemachineImpulseSource source;
    float interval;

    void Start() {
        source = GetComponent<CinemachineImpulseSource>();
    }

    private void Update() {
        interval -= Time.deltaTime;
        if (source && interval <= 0f) {
            interval = 0.1f;
            source.GenerateImpulseAt(transform.position, Vector3.one);
        }
    }

}
