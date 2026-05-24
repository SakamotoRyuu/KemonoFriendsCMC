using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAndPositionToRaycastHit : MonoBehaviour {

    public GameObject targetObj;
    public Transform rayOrigin;
    public Vector3 rayOffset;
    public Vector3 direction;
    public float distance = 10f;
    public LayerMask layerMask;
    public bool activation = true;
    public bool positioning = true;
    public Vector3 offset;

    Transform targetTrans;
    Ray ray = new Ray();
    RaycastHit raycastHit = new RaycastHit();
    static readonly Vector3 vecZero = Vector3.zero;
    static readonly Vector3 vecForward = Vector3.forward;
    
	void Start () {
        targetTrans = targetObj.transform;
		if (activation) {
            targetObj.SetActive(false);
        }
	}

    public void RaycastHitActivate(int param = 1) {
        enabled = (param != 0);
        if (activation && !enabled && targetObj.activeSelf) {
            targetObj.SetActive(false);
        }
    }
	
	void LateUpdate () {
        ray.origin = rayOrigin.position + rayOffset;
        ray.direction = (direction == vecZero ? rayOrigin.TransformDirection(vecForward) : direction);
        if (Physics.Raycast(ray, out raycastHit, distance, layerMask, QueryTriggerInteraction.Ignore)) {
            if (positioning) {
                targetTrans.position = raycastHit.point + offset;
            }
            if (activation && !targetObj.activeSelf) {
                targetObj.SetActive(true);
            }
        } else {
            if (activation && targetObj.activeSelf) {
                targetObj.SetActive(false);
            }
        }
	}
}
