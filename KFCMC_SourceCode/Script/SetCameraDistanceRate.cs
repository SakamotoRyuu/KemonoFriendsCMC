using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraDistanceRate : MonoBehaviour {

    public float smallRate = 0;
    public float yAxisValue = 0.5f;

    bool changedFlag = true;

    private void OnTriggerEnter(Collider other) {
        if (CameraManager.Instance != null && GameManager.Instance.save.config[GameManager.Save.configID_CloseUpViewIndoors] != 0 && other.CompareTag("ItemGetter")) {
            CameraManager.Instance.smallRate = smallRate;
            CameraManager.Instance.SetYAxisRate(yAxisValue);
            changedFlag = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (CameraManager.Instance != null && changedFlag && other.CompareTag("ItemGetter")) {
            CameraManager.Instance.smallRate = 0;
            CameraManager.Instance.SetYAxisRate();
            changedFlag = false;
        }
    }

}
