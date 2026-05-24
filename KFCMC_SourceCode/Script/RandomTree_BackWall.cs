using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTree_BackWall : RandomTree {
    
    int wallDir = -2;
    const string targetTag = "WallLooks";

    protected override void Start() {
        LayerMask layerMask = LayerMask.GetMask("Field");
        Vector3 origin = transform.position + Vector3.up;
        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.back, out hit, 3f, layerMask, QueryTriggerInteraction.Ignore) && hit.collider.CompareTag(targetTag)) {
            wallDir = 0;
        } else if (Physics.Raycast(origin, Vector3.left, out hit, 3f, layerMask, QueryTriggerInteraction.Ignore) && hit.collider.CompareTag(targetTag)) {
            wallDir = 1;
        } else if (Physics.Raycast(origin, Vector3.forward, out hit, 3f, layerMask, QueryTriggerInteraction.Ignore) && hit.collider.CompareTag(targetTag)) {
            wallDir = 2;
        } else if (Physics.Raycast(origin, Vector3.back, out hit, 3f, layerMask, QueryTriggerInteraction.Ignore) && hit.collider.CompareTag(targetTag)) {
            wallDir = -1;
        } else {
            wallDir = Random.Range(-1, 3);
        }
        base.Start();
    }

    /*
    protected override bool GetInstantiateCondition() {
        return (wallDir > -2);
    }
    */

    protected override Quaternion GetRandomRotate() {
        return Quaternion.Euler(new Vector3(0f, wallDir * 90f, 0f));
    }

}
