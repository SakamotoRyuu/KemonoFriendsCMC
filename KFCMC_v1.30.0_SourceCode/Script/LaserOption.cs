using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserOption : MonoBehaviour {

    public GameObject eyeLight;
    public GameObject lineObj;
    public float lightChargeTimeMax = 1.5f;
    public Vector2 lightStartIntensity = new Vector2(0.2f, 0.8f);
    public Vector2 lightEndIntensity = new Vector2(1.8f, 4.4f);

    [System.NonSerialized]
    public bool isLightCharging = false;
    [System.NonSerialized]
    public bool isBlasting = false;

    private FlickeringLight flickLight;
    private float lightChargeTime;
    
    void Awake () {
        if (eyeLight) {
            flickLight = eyeLight.GetComponent<FlickeringLight>();
        }
    }

    public void LightFlickeringChargeStart() {
        lightChargeTime = 0;
        if (flickLight) {
            flickLight.intensity.x = lightStartIntensity.x;
            flickLight.intensity.y = lightStartIntensity.y;
        }
        if (eyeLight) {
            eyeLight.SetActive(true);
        }
        if (lineObj) {
            lineObj.SetActive(true);
        }
        isLightCharging = true;
        isBlasting = true;
    }
    
    public void LightFlickeringChargeEnd() {
        if (flickLight) {
            flickLight.intensity.x = lightEndIntensity.x;
            flickLight.intensity.y = lightEndIntensity.y;
        }
        if (lineObj && lineObj.activeSelf) {
            lineObj.SetActive(false);
        }
        isLightCharging = false;
    }

    public void LightFlickeringBlastEnd() {
        if (eyeLight && eyeLight.activeSelf) {
            eyeLight.SetActive(false);
        }
        isBlasting = false;
    }

    public void CancelLaser() {
        LightFlickeringChargeEnd();
        LightFlickeringBlastEnd();
    }

    private void Update() {
        if (flickLight) {
            if (isLightCharging) {
                lightChargeTime += Time.deltaTime;
                if (lightChargeTime > lightChargeTimeMax) {
                    lightChargeTime = lightChargeTimeMax;
                }
                float rate = (lightChargeTime / lightChargeTimeMax);
                flickLight.intensity.x = rate * lightEndIntensity.x * 0.25f + lightStartIntensity.x;
                flickLight.intensity.y = rate * lightEndIntensity.y * 0.25f + lightStartIntensity.y;
            }
        }
    }
}
