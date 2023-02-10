using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioToLocalPosition : MonoBehaviour
{

    public float conditionRatio;
    public Transform[] target;
    public Vector3[] effectRate;

    void Awake() {
        float nowRatio = (float)Screen.width / Screen.height;
        if (nowRatio < conditionRatio) {
            float ratioDiff = conditionRatio - nowRatio;
            for (int i = 0; i < target.Length && i < effectRate.Length; i++) {
                if (target[i]) {
                    Vector3 posTemp = target[i].localPosition;
                    posTemp += ratioDiff * effectRate[i];
                    target[i].localPosition = posTemp;
                }
            }
        }
    }
}
