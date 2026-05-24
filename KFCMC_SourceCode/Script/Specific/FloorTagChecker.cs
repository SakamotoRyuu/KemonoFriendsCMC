using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTagChecker : MonoBehaviour {

    public GameObject colliderObj;
    DungeonController dcSave;

    void Update() {
        if (colliderObj && StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController != dcSave) {
            dcSave = StageManager.Instance.dungeonController;
            if (colliderObj.activeSelf != dcSave.floorCollider) {
                colliderObj.SetActive(dcSave.floorCollider);
            }
            if (!string.IsNullOrEmpty(dcSave.footstepType) && !colliderObj.CompareTag(dcSave.footstepType)) {
                colliderObj.tag = dcSave.footstepType;
            }
        }
    }
}
