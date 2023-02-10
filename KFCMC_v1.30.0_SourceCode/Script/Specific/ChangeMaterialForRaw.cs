using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialForRaw : MonoBehaviour {

    public Renderer changeTargetRenderer;
    public Material normalMaterial;
    public Material rawMaterial;
    bool sinWR;

    private void Update() {
        if (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.sinWR != sinWR) {
            sinWR = !sinWR;
            if (sinWR) {
                changeTargetRenderer.material = rawMaterial;
            } else {
                changeTargetRenderer.material = normalMaterial;
            }
        }
    }

}
