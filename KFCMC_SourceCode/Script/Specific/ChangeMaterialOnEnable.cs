using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialOnEnable : MonoBehaviour {

    public Renderer targetRenderer;
    public int[] matIndex;
    public Material[] newMaterial;

    private void OnEnable() {
        if (targetRenderer) {
            Material[] mats = targetRenderer.materials;
            for (int i = 0; i < matIndex.Length; i++) {
                mats[matIndex[i]] = newMaterial[i];
            }
            targetRenderer.materials = mats;
        }
    }
}
