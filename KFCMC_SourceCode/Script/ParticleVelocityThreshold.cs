using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleVelocityThreshold : MonoBehaviour {

    public float startDelay = 0.5f;
    public float thresholdMagnitude = 1.0f;
    public int enduranceCount = 2;

    ParticleSystem[] particles;
    Transform trans;
    Vector3 prev;
    float delayTimer = 0.0f;
    bool stopped = false;
    int nowCount = 0;

    // Use this for initialization
    void Start() {
        particles = GetComponentsInChildren<ParticleSystem>(false);
        trans = transform;
        prev = trans.position;
    }

    // Update is called once per frame
    void Update() {
        if (!stopped) {
            float deltaTime = Time.deltaTime;
            if (deltaTime > 0) {
                delayTimer += deltaTime;
                if (delayTimer > startDelay && ((trans.position - prev) / deltaTime).sqrMagnitude < thresholdMagnitude * thresholdMagnitude) {
                    nowCount++;
                    if (nowCount > enduranceCount) {
                        for (int i = 0; i < particles.Length; i++) {
                            particles[i].Stop();
                        }
                        stopped = true;
                    }
                } else {
                    nowCount = 0;
                }
                prev = trans.position;
            }
        }
    }
}
