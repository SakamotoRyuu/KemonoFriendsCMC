using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingDatabase : SingletonMonoBehaviour<LightingDatabase> {

    [System.Serializable]
    public class SkyLightSet
    {
        public GameObject lightPrefab;
        public string skyBoxResourcePath;
        public int pathHash;
        public bool ambientFlat;
        [ColorUsage(false, true)]
        public Color ambientColor;
        public float reflectionIntensity = 1f;
        public int fogNumber = -1;
        public bool playerLight;
        public int lightType;
        public bool isNight;
    }

    public SkyLightSet[] skyLightSet;
    public GameObject[] playerLightPrefab;
    public int defaultLightingNumber;
    public int nowLightingNumber;

    GameObject nowLightInstance;
    Light nowLightComponent;
    int cachePointer;
    bool dirtyFlag;
    const int cacheMax = 8;
    SetSkyboxOnStart[] skyboxCache = new SetSkyboxOnStart[cacheMax];

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
            for (int i = 0; i < skyLightSet.Length; i++) {
                skyLightSet[i].pathHash = skyLightSet[i].skyBoxResourcePath.GetHashCode();
            }
        }
    }

    private void Start() {
        nowLightingNumber = -1;
        SetLighting(defaultLightingNumber);
    }

    public bool PlayerLight {
        get {
            return skyLightSet[nowLightingNumber].playerLight;
        }
    }

    public int LightType {
        get {
            return skyLightSet[nowLightingNumber].lightType;
        }
    }

    public bool IsNight {
        get {
            return skyLightSet[nowLightingNumber].isNight;
        }
    }

    public void LoadSkybox(int lightingNumber) {
        if (!(nowLightingNumber >= 0 && skyLightSet[lightingNumber].pathHash == skyLightSet[nowLightingNumber].pathHash)) {
            bool cacheFound = false;
            for (int i = 0; i < skyboxCache.Length; i++) {
                if (skyboxCache[i] != null) {
                    if (skyLightSet[lightingNumber].pathHash == skyboxCache[i].pathHash) {
                        cacheFound = true;
                        skyboxCache[i].SetSkybox();
                        break;
                    }
                }
            }
            if (!cacheFound) {
                cachePointer = 0;
                double lastUseTime = double.MaxValue;
                for (int i = 0; i < skyboxCache.Length; i++) {
                    if (skyboxCache[i] == null) {
                        cachePointer = i;
                        break;
                    } else {
                        if (lastUseTime > skyboxCache[i].lastUsedTime) {
                            lastUseTime = skyboxCache[i].lastUsedTime;
                            cachePointer = i;
                        }
                    }
                }
                if (skyboxCache[cachePointer] != null) {
                    Destroy(skyboxCache[cachePointer].gameObject);
                    skyboxCache[cachePointer] = null;
                }
                skyboxCache[cachePointer] = Instantiate(Resources.Load(skyLightSet[lightingNumber].skyBoxResourcePath, typeof(GameObject)) as GameObject, transform).GetComponent<SetSkyboxOnStart>();
                skyboxCache[cachePointer].pathHash = skyLightSet[lightingNumber].pathHash;
            }
        }
    }

    public void SetLighting(int lightingNumber) {
        if (nowLightingNumber != lightingNumber || dirtyFlag) {
            dirtyFlag = false;
            if (nowLightInstance) {
                Destroy(nowLightInstance);
                nowLightInstance = null;
            }
            if (lightingNumber >= 0 && lightingNumber < skyLightSet.Length) {
                if (skyLightSet[lightingNumber].lightPrefab) {
                    nowLightInstance = Instantiate(skyLightSet[lightingNumber].lightPrefab);
                    nowLightComponent = nowLightInstance.GetComponent<Light>();
                    if (nowLightComponent) {
                        RenderSettings.sun = nowLightComponent;
                    }
                }

                LoadSkybox(lightingNumber);
                
                if (skyLightSet[lightingNumber].ambientFlat) {
                    RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                    RenderSettings.ambientLight = skyLightSet[lightingNumber].ambientColor;
                } else {
                    RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
                }
                if (skyLightSet[lightingNumber].reflectionIntensity != RenderSettings.reflectionIntensity) {
                    RenderSettings.reflectionIntensity = skyLightSet[lightingNumber].reflectionIntensity;
                }
            } else {
                RenderSettings.skybox = null;
                RenderSettings.sun = null;
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                RenderSettings.ambientLight = Color.black;
            }
            nowLightingNumber = lightingNumber;
        }
        if (FogDatabase.Instance) {
            FogDatabase.Instance.SetFog(skyLightSet[lightingNumber].fogNumber);
        }
    }

    public void SetDirty()
    {
        dirtyFlag = true;
    }

}
