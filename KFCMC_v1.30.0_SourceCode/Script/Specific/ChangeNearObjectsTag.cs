using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeNearObjectsTag : MonoBehaviour {

    public Transform pivotTransform;
    public string beforeTag;
    public string afterTag;
    public float nearDistance;

    void Start() {
        float sqrDist = nearDistance * nearDistance;
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(beforeTag);
        for (int i = 0; i < taggedObjects.Length; i++) {
            if ((taggedObjects[i].transform.position - pivotTransform.position).sqrMagnitude < sqrDist) {
                taggedObjects[i].tag = afterTag;
            }
        }
    }

}
