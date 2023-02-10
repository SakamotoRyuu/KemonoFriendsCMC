using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opening_ChangeMaterial : MonoBehaviour {

    public Renderer targetRend;
    public int matIndex;
    public Material newMat;

    private void OnEnable() {
        if (targetRend) {
            Material[] mats = targetRend.materials;
            mats[matIndex] = newMat;
            targetRend.materials = mats;
        }
    }

}
