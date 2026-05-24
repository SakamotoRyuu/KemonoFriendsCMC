using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOperationCanvas_InverseScaling : UIOperationCanvas
{

    public Transform scalingPivot;
    public RectTransform scalingTarget;
    public float farDistance = 100;
    public float nearDistance = 1;
    public float farScalingEffectRate = 1;
    public float nearScalingEffectRate = 1;
    public float scaleMultiplier = 1;

    private float scaleSave = -1;

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (scalingPivot && camT && canvas.enabled)
        {
            Vector3 pivotPos = scalingPivot.position;
            Vector3 camPos = camT.position;
            float sqrDist = (pivotPos - camPos).sqrMagnitude;
            float scaleTemp = scaleMultiplier;
            if (sqrDist > farDistance * farDistance)
            {
                float distance = Vector3.Distance(pivotPos, camPos);
                scaleTemp *= Mathf.Lerp(1, distance / farDistance, farScalingEffectRate);
            }
            else if (sqrDist < nearDistance * nearDistance)
            {
                float distance = Vector3.Distance(pivotPos, camPos);
                scaleTemp *= Mathf.Lerp(1, distance / nearDistance, nearScalingEffectRate);
            }
            if (scaleTemp != scaleSave)
            {
                scaleSave = scaleTemp;
                scalingTarget.localScale = Vector3.one * scaleTemp;
            }
        }
    }

}
