using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCheckerWithCamera : LineCheckerBase
{

    public bool considerClipping;
    public float clippingPlusParam;
    public float surelyReachDistance = 0f; // 必ずReach=Trueになる距離
    public float unreachableDistance = 1000f; // 必ずReach=Falseになる距離
    public float checkInterval = 0.125f;

    Transform camT;
    float timeRemain = 0f;

    protected void Start()
    {
        if (point == null)
        {
            point = transform;
        }
        if (CameraManager.Instance)
        {
            CameraManager.Instance.SetMainCameraTransform(ref camT);
        }
        else
        {
            Camera camTemp = Camera.main;
            if (camTemp)
            {
                camT = camTemp.transform;
            }
        }
    }

    protected void Update()
    {
        timeRemain -= Time.deltaTime;
        if (timeRemain <= 0f && camT)
        {
            CheckReaching();
            timeRemain = checkInterval;
        }
    }

    protected virtual void CheckReaching()
    {
        Vector3 targetPos = point.position + offset;
        Vector3 cameraPos = camT.position;
        float sqrDist = (targetPos - cameraPos).sqrMagnitude;
        if (sqrDist <= surelyReachDistance * surelyReachDistance)
        {
            reaching = true;
        }
        else if (sqrDist >= unreachableDistance * unreachableDistance)
        {
            reaching = false;
        }
        else
        {
            if (considerClipping && CameraManager.Instance)
            {
                float clippingNear = CameraManager.Instance.GetActualClippingNear() + clippingPlusParam;
                if ((targetPos - cameraPos).sqrMagnitude <= clippingNear * clippingNear)
                {
                    reaching = true;
                }
                else
                {
                    cameraPos += (targetPos - cameraPos).normalized * clippingNear;
                    reaching = !Physics.Linecast(targetPos, cameraPos, layerMask);
                }
            }
            else
            {
                reaching = !Physics.Linecast(targetPos, cameraPos, layerMask);
            }
        }
    }

    public virtual void AddDistance(float amount)
    {
        surelyReachDistance += amount;
        unreachableDistance += amount;
    }
}
