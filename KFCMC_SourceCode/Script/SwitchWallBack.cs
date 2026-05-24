using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWallBack : MonoBehaviour {

    public GameObject switchTarget;
    public Material transparentMaterial;
    public float sqrDistOffset;

    [System.NonSerialized]
    public bool nowFading;
    [System.NonSerialized]
    public bool nowRendererEnabled;

    Renderer targetRenderer;
    Material defaultMaterial;

    void Start() {
        targetRenderer = switchTarget.GetComponent<Renderer>();
        defaultMaterial = targetRenderer.material;
        nowRendererEnabled = true;
        if (SwitchWallBackManager.Instance) {
            SwitchWallBackManager.Instance.SetWall(this);
        }
    }

    public void SetMaterial(bool toFade) {
        if (targetRenderer) {
            if (toFade) {
                targetRenderer.material = transparentMaterial;
            } else {
                targetRenderer.material = defaultMaterial;
            }
        }
        nowFading = toFade;
    }

    public void SetRendererEnabled(bool toEnable) {
        if (targetRenderer && targetRenderer.enabled != toEnable) {
            targetRenderer.enabled = toEnable;
        }
        nowRendererEnabled = toEnable;
    }

    public void SetColor(Color color) {
        if (targetRenderer) {
            targetRenderer.material.color = color;
        }
    }

}
