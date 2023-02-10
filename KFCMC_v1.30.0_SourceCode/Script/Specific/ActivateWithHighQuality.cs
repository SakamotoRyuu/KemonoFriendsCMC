using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateWithHighQuality : MonoBehaviour
{

    public int qualityCondition;
    public GameObject[] targetObjs;

    private void Update() {
        if (GameManager.Instance) {
            bool flag = GameManager.Instance.save.config[GameManager.Save.configID_QualityLevel] >= qualityCondition;
            for (int i = 0; i < targetObjs.Length; i++) {
                if (targetObjs[i] && targetObjs[i].activeSelf != flag) {
                    targetObjs[i].SetActive(flag);
                }
            }
        }
    }

}
