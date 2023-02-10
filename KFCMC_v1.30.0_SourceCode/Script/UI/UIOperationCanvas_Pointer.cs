using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOperationCanvas_Pointer : MonoBehaviour {

    public Canvas canvas;
    public RectTransform panel;

    [System.NonSerialized]
    public bool active;
    [System.NonSerialized]
    public Vector3 targetPosition;

    public float scaleMinDist = 6f;
    public float scaleMaxDist = 40f;
    public float distRateMin = 0.25f;

    float distRateSave = 0;
    Camera mainCamera;
    Transform camT;

    private void Awake() {
        canvas.enabled = false;
        active = false;
    }

    private void Start() {
        if (CameraManager.Instance) {
            CameraManager.Instance.SetMainCameraTransform(ref camT);
            CameraManager.Instance.SetMainCamera(ref mainCamera);
        } else {
            Camera camTemp = Camera.main;
            if (camTemp) {
                mainCamera = camTemp;
                camT = camTemp.transform;
            }
        }
    }

    private void LateUpdate() {
        if (active) { 
            if (!canvas.enabled) {
                canvas.enabled = true;
            }
            panel.position = RectTransformUtility.WorldToScreenPoint(mainCamera, targetPosition);
            float distRate = GetDistanceRate();
            if (distRateSave != distRate) {
                panel.localScale = Vector3.one * distRate;
                distRateSave = distRate;
            }
        } else {
            if (canvas.enabled) {
                canvas.enabled = false;
            }
        }
    }

    private float GetDistanceRate() {
        float distTemp = (camT ? Vector3.Distance(camT.position, targetPosition) : 0f);
        if (distTemp < scaleMinDist) {
            return 1f;
        } else if (distTemp > scaleMaxDist) {
            return distRateMin;
        } else {
            return Mathf.Lerp(1f, distRateMin, (distTemp - scaleMinDist) / (scaleMaxDist - scaleMinDist));
        }
    }

}
