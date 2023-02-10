using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretItem : MonoBehaviour
{

    public GameObject activateTarget;
    public GameObject effectPrefab;

    bool activated = false;
    GameObject mapChip;

    void Activate() {
        if (activateTarget && !activateTarget.activeSelf) {
            activated = true;
            activateTarget.SetActive(true);
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
            if (mapChip != null) {
                Destroy(mapChip);
            }
        }
    }

    private void Awake() {
        if (StageManager.Instance.IsHomeStage) {
            activated = true;
            if (activateTarget && !activateTarget.activeSelf) {
                activateTarget.SetActive(true);
            }
        } else if (activateTarget && activateTarget.activeSelf) {
            activateTarget.SetActive(false);
        }
    }

    private void Start() {
        if (!activated) {
            mapChip = Instantiate(MapDatabase.Instance.prefab[MapDatabase.other], transform);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!activated && other.CompareTag("ItemGetter")) {
            Activate();
        }
    }

}
