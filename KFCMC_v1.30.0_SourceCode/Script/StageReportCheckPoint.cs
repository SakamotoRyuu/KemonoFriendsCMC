using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageReportCheckPoint : MonoBehaviour {

    public bool isEnd;

    void Start() {
        if (GameManager.Instance.save.stageReport.Length >= GameManager.stageReportMax) {
            if (isEnd) {
                if (CharacterManager.Instance && StageManager.Instance) {
                    CharacterManager.Instance.ShowStageResult(StageManager.Instance.stageNumber);
                }
            } else {
                for (int i = 0; i < GameManager.Instance.save.stageReport.Length; i++) {
                    GameManager.Instance.save.stageReport[i] = 0;
                }
                GameManager.Instance.save.stageReport[GameManager.stageReport_PlayTime] = GameManager.Instance.save.totalPlayTime;
                GameManager.Instance.save.stageReport[GameManager.stageReport_Minmi] += (GameManager.Instance.minmiBlue ? (1 << 0) : 0);
                GameManager.Instance.save.stageReport[GameManager.stageReport_Minmi] += (GameManager.Instance.minmiRed ? (1 << 1) : 0);
                GameManager.Instance.save.stageReport[GameManager.stageReport_Minmi] += (GameManager.Instance.minmiPurple ? (1 << 2) : 0);
                GameManager.Instance.save.stageReport[GameManager.stageReport_Minmi] += (GameManager.Instance.minmiBlack ? (1 << 3) : 0);
                GameManager.Instance.save.stageReport[GameManager.stageReport_Minmi] += (GameManager.Instance.minmiSilver ? (1 << 4) : 0);
                GameManager.Instance.save.stageReport[GameManager.stageReport_Minmi] += (GameManager.Instance.minmiGolden ? (1 << 5) : 0);
            }
        }
    }

}
