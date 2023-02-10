using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnHomeStage : MonoBehaviour {

    public GameObject activateTarget;

    private void Start() {
        if (activateTarget && StageManager.Instance && StageManager.Instance.IsHomeStage) {
            activateTarget.SetActive(true);
        }
    }

}
