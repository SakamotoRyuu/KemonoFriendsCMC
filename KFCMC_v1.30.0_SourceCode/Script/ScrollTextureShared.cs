using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTextureShared : MonoBehaviour {
    
    [System.Serializable]
    public class ScrollTextureSetting {
        public int materialNum;
        public string propertyName = "_MainTex";
        public Vector2 speed;
        public Vector2 circularCycle;
        public Vector2 circularAmplitude;
        public float time;
    }

    public ScrollTextureSetting[] scrollTextureSettings;
    public Renderer rend;
    protected  float time;
    protected Vector2 offset;
    private int[] propertyID;
    private float deltaTime;

    protected virtual void Awake() {
        if (rend == null) {
            rend = GetComponent<Renderer>();
        }
    }

    protected virtual void Start() {
        propertyID = new int[scrollTextureSettings.Length];
        for (int i = 0; i < scrollTextureSettings.Length; i++) {
            propertyID[i] = Shader.PropertyToID(scrollTextureSettings[i].propertyName);
        }
    }

    public virtual void Scroll() {
        deltaTime = Time.deltaTime;
        for (int i = 0; i < scrollTextureSettings.Length; i++) {
            if (scrollTextureSettings[i].materialNum < rend.sharedMaterials.Length) {
                scrollTextureSettings[i].time += deltaTime;
                offset = scrollTextureSettings[i].speed * scrollTextureSettings[i].time;
                if (scrollTextureSettings[i].circularCycle.x > 0 && scrollTextureSettings[i].circularAmplitude.x != 0) {
                    offset.x += (float)System.Math.Cos(System.Math.PI * 2 * (scrollTextureSettings[i].time / scrollTextureSettings[i].circularCycle.x)) * scrollTextureSettings[i].circularAmplitude.x;
                }
                if (scrollTextureSettings[i].circularCycle.y > 0 && scrollTextureSettings[i].circularAmplitude.y != 0) {
                    offset.y += (float)System.Math.Sin(System.Math.PI * 2 * (scrollTextureSettings[i].time / scrollTextureSettings[i].circularCycle.y)) * scrollTextureSettings[i].circularAmplitude.y;
                }
                rend.sharedMaterials[scrollTextureSettings[i].materialNum].SetTextureOffset(propertyID[i], offset);
            }
        }
    }
}

