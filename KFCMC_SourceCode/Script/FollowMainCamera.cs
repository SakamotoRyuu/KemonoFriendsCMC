using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMainCamera : MonoBehaviour {

    public bool fixY;
    public bool stopOnTimeScaleZero;
    
    Transform trans;
    Transform camT;
    
	void Start () {
        trans = transform;
        if (CameraManager.Instance) {
            CameraManager.Instance.SetMainCameraTransform(ref camT);
        } else {
            Camera mainCamera = Camera.main;
            if (mainCamera) {
                camT = mainCamera.transform;
                trans.position = camT.position;
            }
        }
        if (camT == null) {
            enabled = false;
        }
	}
	
	void Update () {
		if (camT && trans.position != camT.position && (!stopOnTimeScaleZero || Time.timeScale > 0.5f)) {
            if (fixY) {
                Vector3 posTemp = camT.position;
                posTemp.y = 0f;
                trans.position = posTemp;
            } else {
                trans.position = camT.position;
            }
        }
	}
}
