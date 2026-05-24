using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FixYAxisValue : MonoBehaviour {
    
    public CinemachineFreeLook cmvc_FreeLook;

    [System.NonSerialized]
    public float targetYAxisValue = 0.5f;
    [System.NonSerialized]
    public float yAxisTargetingTimeRemain;
    [System.NonSerialized]
    public int yAxisFixReserved;

    private void LateUpdate() {
        if (yAxisFixReserved > 0 && CameraManager.Instance) {
            cmvc_FreeLook.m_YAxis.Value = CameraManager.Instance.GetFreeLookYAxisDefaultValue();
            yAxisFixReserved--;
        }
        if (yAxisTargetingTimeRemain > 0) {
            float valueTemp = cmvc_FreeLook.m_YAxis.Value;
            if (valueTemp < targetYAxisValue) {
                valueTemp += Time.deltaTime;
                if (cmvc_FreeLook.m_YAxis.Value >= targetYAxisValue) {
                    yAxisTargetingTimeRemain = 0;
                }
            } else if (valueTemp > targetYAxisValue) {
                valueTemp -= Time.deltaTime;
                if (valueTemp <= targetYAxisValue) {
                    yAxisTargetingTimeRemain = 0;
                }
            } else {
                yAxisTargetingTimeRemain = 0f;
            }
            if (yAxisTargetingTimeRemain <= 0f) {
                valueTemp = targetYAxisValue;
            } else {
                yAxisTargetingTimeRemain -= Time.deltaTime;
            }
            cmvc_FreeLook.m_YAxis.Value = valueTemp;
        }
    }
    
}
