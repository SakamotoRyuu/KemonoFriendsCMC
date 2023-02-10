using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amusement_ParkmanLight : MonoBehaviour {

    public GameObject enterEffectPrefab;
    public GameObject exitEffectPrefab;
    public Vector3 effectOffset;
    public GameObject mapChipPrefab;
    public Amusement_Parkman eventParent;

    GameObject mapChipInstance;

    private void Start() {
        mapChipInstance = Instantiate(mapChipPrefab, transform);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            Instantiate(enterEffectPrefab, transform.position + effectOffset, Quaternion.identity);
            if (eventParent) {
                eventParent.SetLightActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            Instantiate(exitEffectPrefab, transform.position + effectOffset, Quaternion.identity);
            if (eventParent) {
                eventParent.SetLightActive(false);
            }
        }
    }

}
