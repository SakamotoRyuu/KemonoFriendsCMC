using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CancelQuakeOnEnable : MonoBehaviour {

    private void OnEnable() {
        if (CameraManager.Instance) {
            CameraManager.Instance.CancelQuake();
        } else if (CinemachineImpulseManager.Instance != null) {
            CinemachineImpulseManager.Instance.Clear();
        }
    }

}
