using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOperationCanvasOverlay : MonoBehaviour {

    public Canvas canvas;
    public VisibleChecker visibleChecker;
    public LineChecker lineChecker;
    public RectTransform panel;
    public Transform followTarget;
    public Vector3 offset;
    public float scaleMinDist = 6f;
    public float scaleMaxDist = 40f;
    public float disablizeDist = 100f;
    public float distRateMin = 0.25f;
    public bool disablizePivotIsPlayer = false;
    public bool checkCanvasCulling;

    float distRateSave = 0;
    Camera mainCamera;
    Transform camT;
    Vector3 camPos;
    static readonly Vector3 vecOne = Vector3.one;

    private void Awake() {
        canvas.enabled = false;
    }

    private void Start() {
        if (CameraManager.Instance) {
            CameraManager.Instance.SetMainCamera(ref mainCamera);
        } else {
            mainCamera = Camera.main;
        }
        if (mainCamera != null) {
            camT = mainCamera.transform;
        }
    }

    void LateUpdate() {
        if (camT) {
            if (disablizePivotIsPlayer && CharacterManager.Instance.playerTrans) {
                camPos = CharacterManager.Instance.playerTrans.position;
                camPos.y = followTarget.position.y;
                camPos.y += offset.y;
            } else {
                camPos = camT.position;
            }
            if ((!visibleChecker || visibleChecker.isVisible) && (!lineChecker || lineChecker.reach) && (camPos - (followTarget.position + offset)).sqrMagnitude < disablizeDist * disablizeDist && (!checkCanvasCulling || !CanvasCulling.Instance || CanvasCulling.Instance.canvas[0].enabled)) {
                if (!canvas.enabled) {
                    canvas.enabled = true;
                }
                panel.position = RectTransformUtility.WorldToScreenPoint(mainCamera, followTarget.position + offset);
            } else {
                if (canvas.enabled) {
                    canvas.enabled = false;
                }
            }
            float distRate = GetDistanceRate();
            if (distRateSave != distRate) {
                panel.localScale = vecOne * distRate;
                distRateSave = distRate;
            }
        }
    }

    float GetDistanceRate() {
        float sqrDistTemp = (camT.position - (followTarget.position + offset)).sqrMagnitude;
        if (sqrDistTemp <= scaleMinDist * scaleMinDist) {
            return 1f;
        } else if (sqrDistTemp >= scaleMaxDist * scaleMaxDist) {
            return distRateMin;
        } else {
            float distTemp = Vector3.Distance(camT.position, followTarget.position + offset);
            return Mathf.Lerp(1f, distRateMin, (distTemp - scaleMinDist) / (scaleMaxDist - scaleMinDist));
        }
    }
}
