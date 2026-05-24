using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightActivateObject : MonoBehaviour {

    public GameObject activateTarget;
    public Renderer changeRenderer;
    public int matIndex;
    public Material changeMaterial;
    public bool homeOnly;

    bool activated = false;
	
	void Update () {
        if (!activated && LightingDatabase.Instance.IsNight && (!homeOnly || StageManager.Instance.IsHomeStage)) {
            if (activateTarget) {
                activateTarget.SetActive(true);
            }
            if (changeRenderer) {
                Material[] mats = changeRenderer.materials;
                if (matIndex >= 0 && matIndex < mats.Length) {
                    mats[matIndex] = changeMaterial;
                    changeRenderer.materials = mats;
                }
            }
            activated = true;
        }
	}
}
