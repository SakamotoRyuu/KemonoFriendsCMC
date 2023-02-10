using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SwitchWallFade : MonoBehaviour {

    public Material defaultMaterial;
    public Material fadeMaterial;
    public float targetHeight;

    [System.NonSerialized]
    public bool nowFading;

    Renderer[] targetRenderer;
    bool rendererInited;

    void Start() {
        if (SwitchWallFadeManager.Instance) {
            SwitchWallFadeManager.Instance.SetWall(this);
        }
    }

    public void SetRenderer() {
        rendererInited = true;
        Renderer[] rendTemp = GetComponentsInChildren<Renderer>();
        targetRenderer = new Renderer[rendTemp.Length / 2];
        int j = 0;
        for (int i = 0; i < rendTemp.Length && j < targetRenderer.Length; i++) {
            if (rendTemp[i].shadowCastingMode != ShadowCastingMode.ShadowsOnly) {
                targetRenderer[j] = rendTemp[i];
                j++;
            }
        }
    }

    public void SetMaterial(bool toFade) {
        if (rendererInited) {
            if (toFade) {
                for (int i = 0; i < targetRenderer.Length; i++) {
                    if (targetRenderer[i]) {
                        targetRenderer[i].material = fadeMaterial;
                    }
                }
            } else {
                for (int i = 0; i < targetRenderer.Length; i++) {
                    if (targetRenderer[i]) {
                        targetRenderer[i].material = defaultMaterial;
                    }
                }
            }
            nowFading = toFade;
        }
    }

    public void SetColor(Color color) {
        if (rendererInited) {
            for (int i = 0; i < targetRenderer.Length; i++) {
                if (targetRenderer[i]) {
                    targetRenderer[i].material.color = color;
                }
            }
        }
    }

}
