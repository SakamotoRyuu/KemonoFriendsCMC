using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget_Item : FollowTarget {

    public float inertDelay;
    public int japaribunCount;

    bool fixedUpdateEnabled;
    bool exceptionalGetProcessed;
    int preFrameFollowedSave;
    GetItem parentGetItem;

    protected override void FixedUpdate() {
        fixedUpdateEnabled = false;
        if (Time.timeScale > 0f && GameManager.Instance.save.config[GameManager.Save.configID_PullItems] != 0) {
            if (inertDelay > 0f) {
                inertDelay -= Time.fixedDeltaTime;
            }
            if (PauseController.Instance && PauseController.Instance.IsItemGetable(japaribunCount)) {
                base.FixedUpdate();
                fixedUpdateEnabled = true;
                preFrameFollowedSave = 3;
            } else if (preFrameFollowedSave > 0) {
                preFrameFollowedSave--;
                if (rb != null) {
                    rb.velocity = vecZero;
                }
            }
        }
        if (!fixedUpdateEnabled && targetingDeactivateObj && targetingDeactivateObj.activeSelf == false) {
            targetingDeactivateObj.SetActive(true);
        }
        if (fixedUpdateEnabled && !exceptionalGetProcessed && targetObj && (targetObj.transform.position - transform.position).sqrMagnitude < 0.1f) {
            if (parentGetItem == null) {
                parentGetItem = GetComponentInParent<GetItem>();
            }
            if (parentGetItem != null && parentGetItem.getEnable) {
                exceptionalGetProcessed = true;
                ItemGetter itemGetter = targetObj.GetComponent<ItemGetter>();
                if (itemGetter) {
                    itemGetter.GetItemProcess(parentGetItem);
                }
            }
        }
    }

    protected override void OnTriggerEnter(Collider other) {
        if (GameManager.Instance.save.config[GameManager.Save.configID_PullItems] != 0 && inertDelay <= 0f) {
            base.OnTriggerEnter(other);
        }
    }

    protected void OnTriggerExit(Collider other) {
        if (targetObj != null && other.gameObject == targetObj) {
            targetObj = null;
        }
    }

}
