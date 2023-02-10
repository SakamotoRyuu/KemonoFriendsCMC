using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnProgress : MonoBehaviour {

    public enum GenerateCondition {
        Always, HomeObject, InverseHomeObject
    }

    public GameObject target;
    public int conditionProgress;
    public GenerateCondition generateCondition;
    bool activated;

    void Awake() {
        if (target) {
            target.SetActive(false);
        }
        activated = false;
    }

    void Update() {
        if (!activated && target && GameManager.Instance) {
            if (GameManager.Instance.save.progress >= conditionProgress) {
                activated = true;
                if (generateCondition == GenerateCondition.Always || (generateCondition == GenerateCondition.HomeObject && GameManager.Instance.save.config[GameManager.Save.configID_GenerateHomeObjects] != 0) || (generateCondition == GenerateCondition.InverseHomeObject && GameManager.Instance.save.config[GameManager.Save.configID_GenerateHomeObjects] == 0)) {
                    target.SetActive(true);
                }
            }
        }
        enabled = false;
    }
}
