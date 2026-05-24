using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateOnTriggerEnter : MonoBehaviour {

    public GameObject prefab;
    public Vector3 offset;
    public bool parenting;
    public bool inheritRotation;
    public string targetTag;
    public float delayTime;

    protected bool instantiated = false;
    protected float elapsedTime = 0;
    
    private void OnTriggerEnter(Collider other) {
        if (elapsedTime >= delayTime && !instantiated && (string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag))) {
            instantiated = true;
            if (parenting) {
                Instantiate(prefab, transform.position + offset, inheritRotation ? transform.rotation : Quaternion.identity, transform);
            } else {
                Instantiate(prefab, transform.position + offset, inheritRotation ? transform.rotation: Quaternion.identity);
            }
        }
    }

    private void Update() {
        elapsedTime += Time.deltaTime;
    }

}
