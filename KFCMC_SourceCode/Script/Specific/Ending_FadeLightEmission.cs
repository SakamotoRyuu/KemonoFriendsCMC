using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending_FadeLightEmission : MonoBehaviour {
    
    public Light emissionLight;
    public Renderer emissionRenderer;
    public float delayTime;
    public float fadeTime;
    public float lightStartIntensity;
    public float lightEndIntensity;
    public float[] rendererStartBrights;
    public float[] rendererEndBrights;

    private float elapsedTime;
    private Color emissionColor = Color.black;
    private Material[] emissionMaterials;
    private bool fadeCompleted;
    private int propertyID;

    private void Start() {
        if (emissionRenderer) {
            emissionMaterials = emissionRenderer.materials;
            propertyID = Shader.PropertyToID("_EmissionColor");
            for (int i = 0; i < emissionMaterials.Length; i++) {
                emissionColor.r = emissionColor.g = emissionColor.b = rendererStartBrights[i];
                emissionMaterials[i].SetColor(propertyID, emissionColor);
            }
        }
        if (emissionLight) {
            emissionLight.intensity = lightStartIntensity;
        }
    }

    void Update() {
        if (!fadeCompleted) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > delayTime) {
                if (elapsedTime < delayTime + fadeTime) {
                    if (emissionRenderer) {
                        for (int i = 0; i < emissionMaterials.Length; i++) {
                            emissionColor.r = emissionColor.g = emissionColor.b = Mathf.Lerp(rendererStartBrights[i], rendererEndBrights[i], (elapsedTime - delayTime) / fadeTime);
                            emissionMaterials[i].SetColor(propertyID, emissionColor);
                        }
                    }
                    if (emissionLight) {
                        emissionLight.intensity = Mathf.Lerp(lightStartIntensity, lightEndIntensity, (elapsedTime - delayTime) / fadeTime);
                    }
                } else {
                    fadeCompleted = true;
                    if (emissionRenderer) {
                        for (int i = 0; i < emissionMaterials.Length; i++) {
                            emissionColor.r = emissionColor.g = emissionColor.b = rendererEndBrights[i];
                            emissionMaterials[i].SetColor(propertyID, emissionColor);
                        }
                    }
                    if (emissionLight) {
                        emissionLight.intensity = lightEndIntensity;
                    }
                }
            }
        }
    }
}
