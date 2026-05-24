using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerInstantiate : MonoBehaviour {

    public GameObject prefab;
    public string targetTag;
    public bool yPosFix;
    public GameObject destroyTarget;

    private void OnTriggerEnter(Collider other) {
        if (string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag)) {
            Vector3 position = other.transform.position;
            if (yPosFix) {
                position.y = transform.position.y;
            }
            Instantiate(prefab, position, Quaternion.identity);
            if (destroyTarget) {
                Destroy(destroyTarget);
            }
        }
    }

}
