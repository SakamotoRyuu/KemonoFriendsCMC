using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyCameraRotation : MonoBehaviour {

    Transform trans;
    Transform camT;
    Vector3 eulerTemp = Vector3.zero;
    
    private void Awake() {
        trans = transform;
    }

    private void Start() {
        if (CameraManager.Instance) {
            CameraManager.Instance.SetMainCameraTransform(ref camT);
        } else {
            Camera camTemp = Camera.main;
            if (camTemp) {
                camT = camTemp.transform;
            }
        }
    }
    
    void Update () {
		if (camT) {
            eulerTemp.y = camT.localEulerAngles.y;
            trans.eulerAngles = eulerTemp;
        }
	}
}
