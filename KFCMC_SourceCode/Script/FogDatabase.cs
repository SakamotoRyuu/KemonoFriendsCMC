using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogDatabase : SingletonMonoBehaviour<FogDatabase> {

    [System.Serializable]
    public class FogSet {
        public FogMode mode;
        public Color color;
        public float density;
        public float start;
        public float end;
    }

    public FogSet[] fogSet;
    public int fogNumber;
    private bool dirtyFlag;

    protected override void Awake() {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() {
        SetDefault();
    }

    public void SetDefault() {
        fogNumber = -100;
        SetFog(-1);
    }

    public void SetFog(int fogNumber) {
        if (this.fogNumber != fogNumber || dirtyFlag) {
            dirtyFlag = false;
            this.fogNumber = fogNumber;
            if (fogNumber >= 0 && fogNumber < fogSet.Length) {
                RenderSettings.fog = false;
                RenderSettings.fogMode = fogSet[fogNumber].mode;
                RenderSettings.fogColor = fogSet[fogNumber].color;
                RenderSettings.fogDensity = fogSet[fogNumber].density;
                RenderSettings.fogStartDistance = fogSet[fogNumber].start;
                RenderSettings.fogEndDistance = fogSet[fogNumber].end;
                RenderSettings.fog = true;
            } else {
                RenderSettings.fog = false;
            }
        }
    }

    public void ResetFog()
    {
        dirtyFlag = true;
        SetFog(fogNumber);
    }

    public void SetDensity(float density)
    {
        RenderSettings.fogDensity = density;
        dirtyFlag = true;
    }

    public void SetColor(Color color)
    {
        RenderSettings.fogColor = color;
        dirtyFlag = true;
    }

}
