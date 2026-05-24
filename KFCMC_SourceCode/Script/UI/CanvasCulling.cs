using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCulling : SingletonMonoBehaviour<CanvasCulling> {

    public Canvas[] canvas;
    public Camera mapCamera;
    private const int culledMax = 3;
    private int[] culled = new int[culledMax];
    private int[] configValue = new int[culledMax];
    private string[] layerName = { "UI", "Arms", "Grass" };
    Camera mainCamera;
    public const int indexGauge = 0;
    public const int indexShowArms = 1;
    public const int indexShowGrass = 2;
    public const int indexAll = 100;

    bool GetMainCamera() {
        if (mainCamera == null) {
            if (CameraManager.Instance) {
                CameraManager.Instance.SetMainCamera(ref mainCamera);
            } else {
                mainCamera = Camera.main;
            }
        }
        return mainCamera != null;
    }

    public void CheckConfig(int forceIndex = -1, int forceValue = 0) {
        if (GameManager.Instance) {
            configValue[0] = GameManager.Instance.save.config[GameManager.Save.configID_Gauge];
            configValue[1] = GameManager.Instance.save.config[GameManager.Save.configID_ShowArms];
            configValue[2] = GameManager.Instance.save.config[GameManager.Save.configID_ShowGrass];
            if (forceIndex >= indexAll) {
                for (int i = 0; i < configValue.Length; i++) {
                    configValue[i] = forceValue;
                }
            } else if (forceIndex >= 0 && forceIndex < configValue.Length) {
                configValue[forceIndex] = forceValue;
            }
            if (culled[0] != configValue[0]) {
                bool flag = (configValue[0] > 0);
                for (int i = 0; i < canvas.Length; i++) {
                    canvas[i].enabled = flag;
                }
            }
            if (mainCamera || GetMainCamera()) {
                for (int i = 0; i < culledMax; i++) {
                    if (culled[i] != configValue[i]) {
                        culled[i] = configValue[i];
                        int bitNum = 1 << LayerMask.NameToLayer(layerName[i]);
                        if (culled[i] > 0) {
                            if ((mainCamera.cullingMask & bitNum) == 0) {
                                mainCamera.cullingMask |= (1 << LayerMask.NameToLayer(layerName[i]));
                            }
                        } else {
                            if ((mainCamera.cullingMask & bitNum) != 0) {
                                mainCamera.cullingMask ^= (1 << LayerMask.NameToLayer(layerName[i]));
                            }
                        }
                    }
                }
            }
        }
    }

    public void SetMapCameraEnabled(bool flag) {
        mapCamera.enabled = flag;
    }

    protected override void Awake() {
        base.Awake();
        for (int i = 0; i < culledMax; i++) {
            culled[i] = -1;
        }
    }

    private void Start() {
        GetMainCamera();
        CheckConfig();
    }    
}
