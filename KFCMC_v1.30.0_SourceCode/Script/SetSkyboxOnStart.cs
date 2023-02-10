using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSkyboxOnStart : MonoBehaviour {

    public Material skybox;
    public double lastUsedTime;
    public int pathHash;

    void Awake() {
        SetSkybox();
    }

    public void SetSkybox() {
        RenderSettings.skybox = skybox;
        if (GameManager.Instance) {
            lastUsedTime = GameManager.Instance.unscaledTime + 1.0;
        }
    }

}
