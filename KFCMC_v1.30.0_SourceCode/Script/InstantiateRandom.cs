using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateRandom : MonoBehaviour {

    public GameObject[] prefab;
    public bool parenting = false;

    private void Awake() {
        if (prefab.Length > 0) {
            int index = Random.Range(0, prefab.Length);
            if (prefab[index]) {
                if (parenting) {
                    Instantiate(prefab[index], transform);
                } else {
                    Instantiate(prefab[index], transform.position, transform.rotation);
                }
            }
        }
    }
}
