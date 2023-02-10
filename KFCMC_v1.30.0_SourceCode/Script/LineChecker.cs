using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineChecker : MonoBehaviour {

    public LayerMask layerMask;
    public Transform point;
    public Vector3 offset;
    public bool considerClipping;
    public float clippingPlusParam;

    [System.NonSerialized]
    public bool reach;
    
    Transform camT;
    float timeRemain = 0f;
    const float interval = 0.125f;    
    
    private void Start() {
        if (point == null) {
            point = transform;
        }
        if (CameraManager.Instance) {
            CameraManager.Instance.SetMainCameraTransform(ref camT);
        } else {
            Camera camTemp = Camera.main;
            if (camTemp) {
                camT = camTemp.transform;
            }
        }
    }

    private void Update () {
        timeRemain -= Time.deltaTime;
        if (timeRemain <= 0f && camT) {
            if (considerClipping && CameraManager.Instance) {
                Vector3 targetPos = point.position + offset;
                Vector3 originPos = camT.position;
                float clippingNear = CameraManager.Instance.GetActualClippingNear() + clippingPlusParam;
                if ((targetPos - originPos).sqrMagnitude <= clippingNear * clippingNear) {
                    reach = true;
                } else {
                    originPos = originPos + (targetPos - originPos).normalized * clippingNear;
                    reach = !Physics.Linecast(targetPos, originPos, layerMask);
                }
            } else {
                reach = !Physics.Linecast(point.position + offset, camT.position, layerMask);
            }
            timeRemain = interval;
        }
	}
}
