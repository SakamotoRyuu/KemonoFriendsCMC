using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOperationCanvasOverlay : MonoBehaviour
{

    public Canvas canvas;
    public VisibleChecker visibleChecker;
    public LineCheckerBase lineChecker;
    public RectTransform panel;
    public Transform followTarget;
    public Vector3 offset;
    public float scaleMinDist = 6f;
    public float scaleMaxDist = 40f;
    public float disablizeDist = 100f;
    public float distRateMin = 0.25f;
    public float scaleMultiplier = 1f;
    public bool disablizePivotIsPlayer = false;
    public bool checkCanvasCulling;
    public ScalingType scalingType;
    public float nearScalingEffectRate = 0; // Natural Only

    public enum ScalingType
    {
        Linear, Natural
    }

    float distRateSave = 0;
    Camera mainCamera;
    Transform camT;
    Vector3 camPos;
    static readonly Vector3 vecOne = Vector3.one;

    private void Awake()
    {
        canvas.enabled = false;
    }

    private void Start()
    {
        if (CameraManager.Instance)
        {
            CameraManager.Instance.SetMainCamera(ref mainCamera);
        }
        else
        {
            mainCamera = Camera.main;
        }
        if (mainCamera != null)
        {
            camT = mainCamera.transform;
        }
    }

    protected virtual void LateUpdate()
    {
        if (camT)
        {
            if (disablizePivotIsPlayer && CharacterManager.Instance.playerTrans)
            {
                camPos = CharacterManager.Instance.playerTrans.position;
                camPos.y = followTarget.position.y;
                camPos.y += offset.y;
            }
            else
            {
                camPos = camT.position;
            }
            if ((!visibleChecker || visibleChecker.isVisible) &&
                (!lineChecker || lineChecker.Reaching) &&
                (camPos - (followTarget.position + offset)).sqrMagnitude < disablizeDist * disablizeDist &&
                (!checkCanvasCulling || !CanvasCulling.Instance || CanvasCulling.Instance.canvas[0].enabled))
            {
                panel.position = RectTransformUtility.WorldToScreenPoint(mainCamera, followTarget.position + offset);
                float distRate = GetDistanceRate();
                if (distRateSave != distRate)
                {
                    panel.localScale = vecOne * distRate * scaleMultiplier;
                    distRateSave = distRate;
                }
                if (!canvas.enabled)
                {
                    canvas.enabled = true;
                }
            }
            else
            {
                if (canvas.enabled)
                {
                    canvas.enabled = false;
                }
            }
        }
    }

    float GetDistanceRate()
    {
        Vector3 cameraPos = camT.position;
        Vector3 targetPos = followTarget.position + offset;
        float sqrDistTemp = (cameraPos - targetPos).sqrMagnitude;
        if (scalingType == ScalingType.Linear)
        {
            if (sqrDistTemp <= scaleMinDist * scaleMinDist)
            {
                return 1f;
            }
            else if (sqrDistTemp >= scaleMaxDist * scaleMaxDist)
            {
                return distRateMin;
            }
            else
            {
                float distTemp = Vector3.Distance(cameraPos, targetPos);
                return Mathf.Lerp(1f, distRateMin, (distTemp - scaleMinDist) / (scaleMaxDist - scaleMinDist));
            }
        }
        else if (scalingType == ScalingType.Natural)
        {
            if (sqrDistTemp <= scaleMinDist * scaleMinDist && nearScalingEffectRate <= 0)
            {
                return 1f;
            }
            else
            {
                float distTemp = Vector3.Distance(cameraPos, targetPos);
                if (distTemp < 0.01f)
                {
                    distTemp = 0.01f;
                }
                if (distTemp > scaleMinDist)
                {
                    return Mathf.Clamp(scaleMinDist / distTemp, distRateMin, 1f);
                }
                else
                {
                    return Mathf.Lerp(1, scaleMinDist / distTemp, nearScalingEffectRate);
                }
            }
        }
        else
        {
            return 1f;
        }
    }
}
