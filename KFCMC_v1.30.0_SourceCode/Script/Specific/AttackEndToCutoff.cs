using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEndToCutoff : MonoBehaviour {

    public AttackDetection targetAD;
    public Renderer targetRenderer;
    public string propertyName = "_Cutoff";
    public float checkDelay = 1f;
    public float cutoffDelay = 1f;
    public float cutoffTime = 1f;
    public bool completeToDestroy = true;
    public ParticleSystem stopTargetParticle;
    private Material targetMaterial;
    private int propertyID;
    private int progress;
    private float elapsedTime;

    private void Awake() {
        propertyID = Shader.PropertyToID(propertyName);
        targetMaterial = targetRenderer.material;
    }

    void Update() {
        if (progress == 0) {
            if (elapsedTime >= checkDelay) {
                progress = 1;
                elapsedTime = 0f;
            }
        } else if (progress == 1) {
            if (targetAD && targetAD.attackEnabled) {
                elapsedTime = 0f;
            } else if (elapsedTime >= cutoffDelay) {
                progress = 2;
                elapsedTime = 0f;
                if (stopTargetParticle) {
                    stopTargetParticle.Stop();
                }
            }
        } else if (progress == 2) {
            if (elapsedTime < cutoffTime && cutoffTime > 0f) {
                targetMaterial.SetFloat(propertyID, elapsedTime / cutoffTime);
            } else {
                progress = 3;
                if (completeToDestroy) {
                    Destroy(gameObject);
                } else {
                    targetMaterial.SetFloat(propertyID, 1f);
                    enabled = false;
                }
            }
        }
        elapsedTime += Time.deltaTime;
    }
}
