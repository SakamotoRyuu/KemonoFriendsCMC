using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmitFade : MonoBehaviour {

    public ParticleSystem particle;
    public float startMultiplier;
    public float endMultiplier;
    public float delayTime;
    public float fadeTime;

    private ParticleSystem.EmissionModule emissionModule;
    private float elapsedTime;
    private bool completed;
    
    void Start() {
        if (particle) {
            emissionModule = particle.emission;
            emissionModule.rateOverTimeMultiplier = startMultiplier;
        }
    }
    
    void Update() {
        if (!completed && particle) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > delayTime) {
                if (elapsedTime < delayTime + fadeTime) {
                    emissionModule.rateOverTimeMultiplier = Mathf.Lerp(startMultiplier, endMultiplier, (elapsedTime - delayTime) / fadeTime);
                } else {
                    completed = true;
                    emissionModule.rateOverTimeMultiplier = endMultiplier;
                }
            }
        }
    }
}
