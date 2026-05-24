using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalCondition : MonoBehaviour {
    
    public int conditionProgress;
    public Goal goal;
    public AutoRotation[] autoRotation;
    public GameObject[] activateObj;

    private void Update() {
        if (!goal.enabled && GameManager.Instance.save.progress >= conditionProgress) {
            goal.enabled = true;
            for (int i = 0; i < autoRotation.Length; i++) {
                autoRotation[i].enabled = true;
            }
            for (int i = 0; i < activateObj.Length; i++) {
                activateObj[i].SetActive(true);
            }
            enabled = false;
        }
    }

}
