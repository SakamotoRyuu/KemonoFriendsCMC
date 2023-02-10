using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMaterialColor : MonoBehaviour {

    public Renderer targetRenderer;
    public int[] targetIndex;
    public float delayTime;
    public float fadeTime;
    public Color startColor;
    public Color endColor;
    public bool enablizeRendererOnStart;
    public bool disablizeRendererOnComplete;
    public Material specialMaterialOnFading;
    public bool emissionFade;
    [ColorUsage(false, true)]
    public Color emissionStartColor;
    [ColorUsage(false, true)]
    public Color emissionEndColor;
    public bool getMaterialsOnUpdate;

    Material[] mats;
    Material[] originalMaterials;
    Color colorTemp;
    float elapsedTime;
    int emissionPropertyID;
    bool enablizedFlag;

    void SetNewColor(float t) {
        if (getMaterialsOnUpdate) {
            mats = targetRenderer.materials;
        }
        colorTemp.r = Mathf.Lerp(startColor.r, endColor.r, t);
        colorTemp.g = Mathf.Lerp(startColor.g, endColor.g, t);
        colorTemp.b = Mathf.Lerp(startColor.b, endColor.b, t);
        colorTemp.a = Mathf.Lerp(startColor.a, endColor.a, t);
        for (int i = 0; i < targetIndex.Length; i++) {
            mats[targetIndex[i]].color = colorTemp;
        }
        if (emissionFade) {
            colorTemp.r = Mathf.Lerp(emissionStartColor.r, emissionEndColor.r, t);
            colorTemp.g = Mathf.Lerp(emissionStartColor.g, emissionEndColor.g, t);
            colorTemp.b = Mathf.Lerp(emissionStartColor.b, emissionEndColor.b, t);
            colorTemp.a = Mathf.Lerp(emissionStartColor.a, emissionEndColor.a, t);
            for (int i = 0; i < targetIndex.Length; i++) {
                mats[targetIndex[i]].SetColor(emissionPropertyID, colorTemp);
            }
        }
        targetRenderer.materials = mats;
    }

    void Start() {
        if (targetRenderer) {
            mats = targetRenderer.materials;
            if (specialMaterialOnFading != null) {
                originalMaterials = targetRenderer.sharedMaterials;
                for (int i = 0; i < mats.Length; i++) {
                    int renderQueue = mats[i].renderQueue;
                    mats[i] = specialMaterialOnFading;
                    mats[i].renderQueue = renderQueue;
                }
            }
            if (emissionFade) {
                emissionPropertyID = Shader.PropertyToID("_EmissionColor");
            }
            if (delayTime > 0) {
                SetNewColor(0);
            }
        }
    }

    void Update() {
        if (targetRenderer) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > delayTime) {
                if (elapsedTime < delayTime + fadeTime) {
                    SetNewColor((elapsedTime - delayTime) / fadeTime);
                    if (!enablizedFlag && enablizeRendererOnStart) {
                        targetRenderer.enabled = true;
                        enablizedFlag = true;
                    }
                } else {
                    SetNewColor(1);
                    if (disablizeRendererOnComplete) {
                        targetRenderer.enabled = false;
                    }
                    if (specialMaterialOnFading != null) {
                        targetRenderer.materials = originalMaterials;
                    }
                    enabled = false;
                }
            }
        }
    }
}
