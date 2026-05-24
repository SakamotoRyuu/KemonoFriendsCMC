using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour {

    [System.Serializable]
    public class ScrollTextureSetting {
        public int materialNum;
        public string propertyName = "_MainTex";
        public bool forceTiling;
        public Vector2 tiling;
        public Vector2 offset;
        public Vector2 speed;
        public Vector2 circularCycle;
        public Vector2 circularAmplitude;
        public float timeRandomize;
        public float time;
    }

    public ScrollTextureSetting[] scrollTextureSettings;
    private Material[] materials;
    private Vector2 offsetTemp;
    private int[] propertyID;
    private float deltaTime;

    private void Start() {
        ResetMaterial();
    }

    public void ResetMaterial() {
        materials = GetComponent<Renderer>().materials;
        propertyID = new int[scrollTextureSettings.Length];
        for (int i = 0; i < scrollTextureSettings.Length; i++) {
            if (scrollTextureSettings[i].timeRandomize > 0) {
                scrollTextureSettings[i].time += Random.Range(0.0f, scrollTextureSettings[i].timeRandomize);
            }
            propertyID[i] = Shader.PropertyToID(scrollTextureSettings[i].propertyName);
            if (scrollTextureSettings[i].materialNum < materials.Length) {
                if (scrollTextureSettings[i].forceTiling) {
                    materials[scrollTextureSettings[i].materialNum].SetTextureScale(propertyID[i], scrollTextureSettings[i].tiling);
                }
            }
        }
    }

    void Update() {
        //_MainTex
        //_DetailAlbedoMap
        deltaTime = Time.deltaTime;
        for (int i = 0; i < scrollTextureSettings.Length; i++) {
            if (scrollTextureSettings[i].materialNum < materials.Length) {
                scrollTextureSettings[i].time += deltaTime;
                offsetTemp = scrollTextureSettings[i].offset + scrollTextureSettings[i].speed * scrollTextureSettings[i].time;
                if (scrollTextureSettings[i].circularCycle.x > 0 && scrollTextureSettings[i].circularAmplitude.x != 0) {
                    offsetTemp.x += Mathf.Cos(Mathf.PI * 2 * (scrollTextureSettings[i].time / scrollTextureSettings[i].circularCycle.x)) * scrollTextureSettings[i].circularAmplitude.x;
                }
                if (scrollTextureSettings[i].circularCycle.y > 0 && scrollTextureSettings[i].circularAmplitude.y != 0) {
                    offsetTemp.y += Mathf.Sin(Mathf.PI * 2 * (scrollTextureSettings[i].time / scrollTextureSettings[i].circularCycle.y)) * scrollTextureSettings[i].circularAmplitude.y;
                }
                materials[scrollTextureSettings[i].materialNum].SetTextureOffset(propertyID[i], offsetTemp);
            }
        }
    }
}

