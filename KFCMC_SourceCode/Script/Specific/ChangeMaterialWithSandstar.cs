using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialWithSandstar : MonoBehaviour {

    public Renderer targetRenderer;
    public Material defaultMaterial;
    public Material changedMaterial;
    public int materialIndex;
    public bool changeWithWildRelease;
    public bool changeWithSandstarMax;

    private bool saveFlag;
    private bool initialized;
    private Material[] materialArray;

    void Update() {
        if (targetRenderer && CharacterManager.Instance) {
            bool newFlag = (changeWithWildRelease && CharacterManager.Instance.IsPlayerWildReleasing) || (changeWithSandstarMax && CharacterManager.Instance.GetSandstarIsMax());
            if (newFlag != saveFlag) {
                saveFlag = newFlag;
                if (!initialized) {
                    initialized = true;
                    materialArray = targetRenderer.materials;
                }
                materialArray[materialIndex] = newFlag ? changedMaterial : defaultMaterial;
                targetRenderer.materials = materialArray;
            }
        }
    }
}
