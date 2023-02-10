using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandbyPrefab : MonoBehaviour {

    public GameObject[] prefab;

    void Start() {
        for (int i = 0; i < prefab.Length; i++) {
            if (prefab[i]) {
                Destroy(Instantiate(prefab[i]));
            }
        }
    }

}
