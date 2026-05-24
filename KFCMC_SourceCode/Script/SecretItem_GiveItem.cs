using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretItem_GiveItem : MonoBehaviour {

    public int itemID;
    public GameObject effectPrefab;

    const float dropItemVelocity = 5f;
    const float dropItemBalloonDelay = -1;
    const float dropItemGetDelay = -1;
    bool activated = false;
    GameObject mapChip;

    private void Start() {
        if (!activated) {
            mapChip = Instantiate(MapDatabase.Instance.prefab[MapDatabase.item], transform);
        }
    }    

    void Activate() {
        activated = true;
        bool isParenting = (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.itemSettings.container);
        ItemDatabase.Instance.GiveItem(itemID, transform.position, dropItemVelocity, dropItemBalloonDelay, dropItemGetDelay, 0f, isParenting ? StageManager.Instance.dungeonController.itemSettings.container : null);
        Instantiate(effectPrefab, transform.position, Quaternion.identity);
        if (mapChip != null) {
            Destroy(mapChip);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!activated && other.CompareTag("ItemGetter")) {
            Activate();
        }
    }

}
