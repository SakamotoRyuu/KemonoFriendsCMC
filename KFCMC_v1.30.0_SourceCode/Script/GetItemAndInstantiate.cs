using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItemAndInstantiate : GetItem {

    public GameObject onDestroyObj;
    public bool yReset;
    public int setProgress = -1;

    public override void GetProcess() {
        base.GetProcess();
        if (setProgress >= 0) {
            GameManager.Instance.save.SetClearStage(setProgress);
        }
        if (onDestroyObj) {
            Vector3 pos = transform.position;
            if (yReset) {
                pos.y = 0;
            }
            if (StageManager.Instance.dungeonController) {
                Instantiate(onDestroyObj, pos, Quaternion.identity, StageManager.Instance.dungeonController.transform);
            } else {
                Instantiate(onDestroyObj, pos, Quaternion.identity);
            }
        }
    }
}
