using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeActiveWithGraphicQuality : MonoBehaviour {

    public enum Condition { VeryLowQuality, DungeonObject, HomeObject, ThreeStages, LowQuality };
    public Condition condition;
    public GameObject hqObj;
    public GameObject mqObj;
    public GameObject lqObj;

    private int GetCondition() {
        int answer = 2;
        if (GameManager.Instance) {
            int qualityLevel = QualitySettings.GetQualityLevel();
            switch (condition) {
                case Condition.VeryLowQuality:
                    if (qualityLevel <= 0) {
                        answer = 0;
                    }
                    break;
                case Condition.DungeonObject:
                    if (qualityLevel <= 0 && GameManager.Instance.save.config[GameManager.Save.configID_GenerateDungeonObjects] == 0) {
                        answer = 0;
                    }
                    break;
                case Condition.HomeObject:
                    if (qualityLevel <= 0 && GameManager.Instance.save.config[GameManager.Save.configID_GenerateHomeObjects] == 0) {
                        answer = 0;
                    }
                    break;
                case Condition.ThreeStages:
                    if (qualityLevel <= 1 || GameManager.Instance.save.config[GameManager.Save.configID_GenerateDungeonObjects] == 0) {
                        answer = 0;
                    } else if (qualityLevel <= 3) {
                        answer = 1;
                    }
                    break;
                case Condition.LowQuality:
                    if (qualityLevel <= 1) {
                        answer = 0;
                    }
                    break;
            }
        }
        return answer;
    }

    private void UpdateObject() {
        int flag = GetCondition();        
        if (hqObj && hqObj.activeSelf != (flag == 2)) {
            hqObj.SetActive(flag == 2);
        }
        if (mqObj && mqObj.activeSelf != (flag == 1)) {
            mqObj.SetActive(flag == 1);
        }
        if (lqObj && lqObj.activeSelf != (flag == 0)) {
            lqObj.SetActive(flag == 0);
        }
    }

    private void Awake() {
        UpdateObject();
    }

    private void Update() {
        UpdateObject();
    }

}
