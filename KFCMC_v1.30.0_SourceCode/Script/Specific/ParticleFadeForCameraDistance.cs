using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFadeForCameraDistance : MonoBehaviour
{

    public ParticleSystem particle;
    public Transform pivot;
    public float minAlpha = 0.5f;
    public float maxAlpha = 1f;
    public float minRadius = 3f;
    public float maxRadius = 6f;
    public bool ignoreY;
    
    ParticleSystem.MinMaxGradient parColor;
    ParticleSystem.MainModule main;
    Transform camTrans;
    Color colorSave;
    float alphaSave;

    private void Start() {
        if (particle) {
            main = particle.main;
            parColor = main.startColor;
            colorSave = parColor.color;
            alphaSave = colorSave.a;
            if (CameraManager.Instance) {
                camTrans = CameraManager.Instance.mainCamObj.transform;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (camTrans && particle) {
            bool alphaChanged = false;
            Vector3 particlePos = pivot.position;
            Vector3 camPos = camTrans.position;
            if (ignoreY) {
                camPos.y = particlePos.y;
            }
            float sqrDist = (camPos - particlePos).sqrMagnitude;
            if (sqrDist >= maxRadius * maxRadius) {
                if (alphaSave != maxAlpha) {
                    alphaSave = maxAlpha;
                    alphaChanged = true;
                }
            } else if (sqrDist <= minRadius * minRadius) {
                if (alphaSave != minAlpha) {
                    alphaSave = minAlpha;
                    alphaChanged = true;
                }
            } else if (maxRadius > minRadius){
                float distance = Vector3.Distance(camPos, particlePos);
                float alphaTemp = Mathf.Lerp(minAlpha, maxAlpha,(distance - minRadius) / (maxRadius - minRadius));
                if (alphaSave != alphaTemp) {
                    alphaSave = alphaTemp;
                    alphaChanged = true;
                }
            }
            if (alphaChanged) {
                colorSave.a = alphaSave;
                parColor.color = colorSave;
                main.startColor = parColor;
            }
        }
    }
}
