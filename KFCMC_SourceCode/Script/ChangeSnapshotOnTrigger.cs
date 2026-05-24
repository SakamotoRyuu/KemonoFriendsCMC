using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSnapshotOnTrigger : MonoBehaviour {

    public string targetSnapshot = "Snapshot";
    public float transitionTime = 0.5f;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            GameManager.Instance.ChangeSnapshot(targetSnapshot, transitionTime);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            if (StageManager.Instance && StageManager.Instance.dungeonController) {
                StageManager.Instance.dungeonController.SetDefaultSnapshot(transitionTime);
            }
        }
    }

}
