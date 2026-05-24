using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldPrefab : MonoBehaviour {

    public GameObject prefab;
    public Vector3 offset;

    public void InstantiatePrefab() {
        if (prefab) {
            Instantiate(prefab, transform.position + offset, transform.rotation);
        }
    }

}
