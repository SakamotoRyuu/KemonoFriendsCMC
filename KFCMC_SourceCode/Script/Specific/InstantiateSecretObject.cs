using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateSecretObject : MonoBehaviour {

    public GameObject prefab;
    public GameObject effectPrefab;
    public bool rigidToKinematic;
    public bool useTransformRotation;

    bool activated = false;
    GameObject mapChip;

    void Activate() {
        activated = true;
        Quaternion rot = useTransformRotation ? transform.rotation : Quaternion.identity;
        GameObject instance = Instantiate(prefab, transform.position, rot);
        Instantiate(effectPrefab, transform.position, rot);
        if (mapChip != null) {
            Destroy(mapChip);
        }
        if (rigidToKinematic) {
            Rigidbody rigidTemp = instance.GetComponent<Rigidbody>();
            if (rigidTemp) {
                rigidTemp.useGravity = false;
                rigidTemp.isKinematic = true;
            }
        }
    }

    private void Start() {
        if (!activated) {
            mapChip = Instantiate(MapDatabase.Instance.prefab[MapDatabase.item], transform);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!activated && other.CompareTag("ItemGetter")) {
            Activate();
        }
    }
}
