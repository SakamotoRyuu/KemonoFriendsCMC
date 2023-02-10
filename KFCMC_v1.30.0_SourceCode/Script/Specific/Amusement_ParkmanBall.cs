using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amusement_ParkmanBall : MonoBehaviour {

    public GameObject getEffectPrefab;
    public Vector3 effectOffset;
    public Vector3 textOffset;
    public Amusement_Parkman.ItemType itemType;
    public GameObject mapChipPrefab;
    public Amusement_Parkman eventParent;

    GameObject mapChipInstance;

    private void Start() {
        mapChipInstance = Instantiate(mapChipPrefab, transform);
        if (itemType == Amusement_Parkman.ItemType.Ball && eventParent && !eventParent.IsBallVisible) {
            mapChipInstance.SetActive(false);
        }
    }

    private void Update() {
        if (itemType == Amusement_Parkman.ItemType.Ball && eventParent) {
            bool flag = eventParent.IsBallVisible;
            if (mapChipInstance && mapChipInstance.activeSelf != flag) {
                mapChipInstance.SetActive(flag);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            Instantiate(getEffectPrefab, transform.position + effectOffset, Quaternion.identity);
            if (eventParent) {
                eventParent.ReceiveItemGet(itemType, transform.position + textOffset);
            }
            Destroy(gameObject);
        }
    }

}
