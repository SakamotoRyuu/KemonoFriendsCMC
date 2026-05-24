using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_SisinComplete : MonoBehaviour
{

    public Light pointLight;
    public LineRenderer[] lineRenderers;
    public ParticleSystem concentParticle;
    public GameObject burstObj;
    public Event_Summit eventParent;

    ParticleSystem.MainModule concentParMain;
    float elapsedTime;
    bool particleStopped;
    int state;

    private void Awake() {
        for (int i = 0; i < lineRenderers.Length; i++) {
            lineRenderers[i].widthMultiplier = 0;
        }
        pointLight.color = new Color(0f, 0.3333f, 1f);
        pointLight.intensity = 0f;
        concentParMain = concentParticle.main;
        concentParMain.simulationSpeed = 0.3f;
        concentParticle.Play();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        for (int i = 0; i < lineRenderers.Length; i++) {
            if (elapsedTime < 2f) {
                lineRenderers[i].widthMultiplier = Mathf.Clamp01(elapsedTime * 0.5f);
            } else if (elapsedTime < 5.1f) {
                lineRenderers[i].widthMultiplier = 1f;
            } else if (elapsedTime < 5.6f) {
                lineRenderers[i].widthMultiplier = Mathf.Clamp01(1f - (elapsedTime - 5.1f) * 2f);
            } else if (lineRenderers[i].enabled) {
                lineRenderers[i].enabled = false;
            }
        }
        if (elapsedTime < 5f) {
            concentParMain.simulationSpeed = 0.3f + Mathf.Clamp01(elapsedTime * 0.22f);
        }
        if (elapsedTime >= 5f && state == 0) {
            state = 1;
            concentParMain.simulationSpeed = 1f;
            concentParticle.Stop();
        }
        if (elapsedTime >= 6f && state == 1) {
            state = 2;
            pointLight.color = new Color(0f, 1f, 0f);
            burstObj.SetActive(true);
            if (eventParent) {
                eventParent.ClearUp();
            }
        }
        if (elapsedTime >= 12f && state == 2) {
            state = 3;
            pointLight.intensity = 0f;
            pointLight.enabled = false;
        }
        if (elapsedTime < 5.1f) {
            pointLight.intensity = Mathf.Clamp01(elapsedTime * 0.2f) * 4f;
        } else if (elapsedTime < 5.6f) {
            pointLight.intensity = Mathf.Clamp01(1f - (elapsedTime - 5.1f) * 2f) * 4f;
        } else if (elapsedTime < 6f) {
            pointLight.intensity = 0f;
        } else if (elapsedTime < 6.2f) {
            pointLight.intensity = Mathf.Clamp01((elapsedTime - 6f) * 5f) * 4f;
        } else if (elapsedTime < 9f) {
            pointLight.intensity = 4f;
        } else if (elapsedTime < 12f) {
            pointLight.intensity = Mathf.Clamp01(1f - (elapsedTime - 9f) * 0.333333f) * 4f;
        }
        if (elapsedTime >= 20f) {
            Destroy(gameObject);
        }
    }
}
