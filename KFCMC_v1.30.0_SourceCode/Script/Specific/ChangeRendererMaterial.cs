using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRendererMaterial : MonoBehaviour {

    public Renderer targetRenderer;
    public int targetIndex;
    public Material[] materials;

    public void ChangeMaterial(int matIndex) {
        if (matIndex >= 0 && matIndex < materials.Length) {
            Material[] matsTemp = targetRenderer.materials;
            matsTemp[targetIndex] = materials[matIndex];
            targetRenderer.materials = matsTemp;
        }
    }

}
