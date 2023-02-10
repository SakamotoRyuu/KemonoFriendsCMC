using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDurationByParentCondition : MonoBehaviour
{

    public ParticleSystem particle;
    public float baseMultiplier;
    public float idlingSpeed;
    public float walkingSpeed;
    public float runningSpeed;
    CharacterBase parentCBase;
    ParticleSystem.EmissionModule module;
    float timeCount;

    void Start() {
        parentCBase = GetComponentInParent<CharacterBase>();
        if (particle) {
            module = particle.emission;
        }
    }

    void Update() {
        if (particle && parentCBase && Time.timeScale > 0f) {
            if (parentCBase.IsAttacking() || !parentCBase.GetCanMove()) {
                module.rateOverTimeMultiplier = baseMultiplier;
                timeCount = 1.5f;
            } else {
                float speedTemp = walkingSpeed;
                float centerRate = 0.7f;
                if (parentCBase.GetRunning()) {
                    speedTemp = runningSpeed;
                    centerRate = 0.9f;
                } else if (parentCBase.GetIdling()) {
                    speedTemp = idlingSpeed;
                    centerRate = 0.5f;
                }
                timeCount += Time.deltaTime * speedTemp;
                if (timeCount > 2f) {
                    timeCount -= 2f;
                }
                module.rateOverTimeMultiplier = Mathf.Clamp01(centerRate + Mathf.Sin(Mathf.PI * timeCount)) * baseMultiplier;
            }
        }
    }
}
