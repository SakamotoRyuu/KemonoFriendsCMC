using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLineRendererBetween : MonoBehaviour
{

    public LineRenderer lineRenderer;
    public Transform objA;
    public Transform objB;
    public bool checkObjActive;

    private void Update() {
        if (objA == null || objB == null || lineRenderer == null) {
            return;
        }
        if (checkObjActive) {
            bool activeFlag = (objA.gameObject.activeInHierarchy && objB.gameObject.activeInHierarchy);
            if (lineRenderer.enabled != activeFlag) {
                lineRenderer.enabled = activeFlag;
            }
        }
        if (lineRenderer.enabled) {
            lineRenderer.SetPosition(0, objA.position);
            lineRenderer.SetPosition(1, objB.position);
        }
    }

}
