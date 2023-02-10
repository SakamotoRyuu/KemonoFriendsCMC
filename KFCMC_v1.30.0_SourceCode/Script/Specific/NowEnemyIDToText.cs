using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class NowEnemyIDToText : MonoBehaviour {

    public TMP_Text infoText;
    public TMP_Text nameText;

    int pointerSave = -1;
    int idSave = -1;
    int levelSave = -1;
    StringBuilder sb = new StringBuilder();

    void LateUpdate() {
        if (StageManager.Instance.dungeonController) {
            bool changed = false;
            if (StageManager.Instance.dungeonController.enemySettings.pointer != pointerSave) {
                pointerSave = StageManager.Instance.dungeonController.enemySettings.pointer;
                idSave = StageManager.Instance.dungeonController.enemySettings.enemy[pointerSave].id;
                changed = true;
            }
            if (StageManager.Instance.dungeonController.enemySettings.enemy.Length > 1 && StageManager.Instance.dungeonController.enemySettings.enemy[0].level != levelSave) {
                levelSave = StageManager.Instance.dungeonController.enemySettings.enemy[1].level;
                changed = true;
            }
            if (changed) {
                infoText.text = sb.Clear().AppendFormat("ID {0:00}", idSave).AppendLine().AppendFormat("LEVEL {0}", levelSave).ToString();
                nameText.text = TextManager.Get("CELLIEN_NAME_" + idSave.ToString("00"));
            }
        }
    }
}
