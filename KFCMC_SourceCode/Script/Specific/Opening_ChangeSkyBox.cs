using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opening_ChangeSkyBox : MonoBehaviour {    

    public Material skyboxMat;
    public bool setAmbientColorEnabled;
    [ColorUsage(false, true)]
    public Color ambientColor;
    public bool setIntensityEnabled;
    public float ambientIntensity = 1f;
    public float reflectionIntensity = 1f;

    private void OnEnable() {
        RenderSettings.skybox = skyboxMat;
        if (setAmbientColorEnabled) {
            RenderSettings.ambientSkyColor = ambientColor;
        }
        if (setIntensityEnabled) {
            RenderSettings.ambientIntensity = ambientIntensity;
            RenderSettings.reflectionIntensity = reflectionIntensity;
        }
    }

}
