using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialExceptHome : MonoBehaviour {

    public Renderer targetRenderer;
    public int matIndex;
    public Material newMaterial;

    private void Awake() {
        if (StageManager.Instance && !StageManager.Instance.IsHomeStage && targetRenderer) {
            Material[] mats = targetRenderer.materials;
            if (mats.Length > matIndex) {
                mats[matIndex] = newMaterial;
            }
            targetRenderer.materials = mats;
        }
    }

}
