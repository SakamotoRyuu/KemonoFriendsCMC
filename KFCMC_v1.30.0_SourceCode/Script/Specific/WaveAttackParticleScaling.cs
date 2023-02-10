using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAttackParticleScaling : MonoBehaviour {

    public float timeMax = 1.8f;
    public ParticleSystem shock;
    public ParticleSystem arrow;
    public ParticleSystem dust;
    public ParticleSystem lick;
    public Vector2 shockSizeMin = new Vector2(2f, 2f);
    public Vector2 shockSizeMax = new Vector2(6.6f, 3f);
    public Vector3 arrowScaleMin = new Vector3(1f, 0.5f, 1f);
    public Vector3 arrowScaleMax = new Vector3(3.3f, 0.5f, 3.3f);
    public Vector3 dustScaleMin = new Vector3(1.8f, 0.4f, 0.8f);
    public Vector3 dustScaleMax = new Vector3(6f, 0.4f, 0.8f);
    public Vector3 lickScaleMin = new Vector3(1f, 0.5f, 1f);
    public Vector3 lickScaleMax = new Vector3(3.3f, 0.5f, 1f);

    float elapsedTime;
    ParticleSystem.MainModule shockMain;
    ParticleSystem.ShapeModule arrowShape;
    ParticleSystem.EmissionModule arrowEmission;
    ParticleSystem.ShapeModule dustShape;
    ParticleSystem.ShapeModule lickShape;
    ParticleSystem.EmissionModule lickEmission;

    private void Awake() {
        shockMain = shock.main;
        arrowShape = arrow.shape;
        arrowEmission = arrow.emission;
        dustShape = dust.shape;
        lickShape = lick.shape;
        lickEmission = lick.emission;
    }

    private void Update() {
        if (elapsedTime >= timeMax) {
            elapsedTime = timeMax;
            enabled = false;
        }
        float t = elapsedTime / timeMax;
        if (shock) {
            shockMain.startSizeXMultiplier = Mathf.Lerp(shockSizeMin.x, shockSizeMax.x, t);
            shockMain.startSizeYMultiplier = Mathf.Lerp(shockSizeMin.y, shockSizeMax.y, t);
        }
        if (arrow) {
            arrowShape.scale = Vector3.Lerp(arrowScaleMin, arrowScaleMax, t);
            arrowEmission.rateOverTimeMultiplier = Mathf.Lerp(100f, 200f, t);
        }
        if (dust) {
            dustShape.scale = Vector3.Lerp(dustScaleMin, dustScaleMax, t);
        }
        if (lick) {
            lickShape.scale = Vector3.Lerp(lickScaleMin, lickScaleMax, t);
            lickEmission.rateOverDistanceMultiplier = Mathf.Lerp(4f, 8f, t);
        }
        elapsedTime += Time.deltaTime;
    }

}
