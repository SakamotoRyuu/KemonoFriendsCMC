using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_MoveStage : Goal {

    public int stageId;
    public int achievementId;

    protected override void Move() {
        if (StageManager.Instance != null) {
            if (stageId == StageManager.Instance.randomSettings.stageNumber && !StageManager.Instance.IsRandomStage && GameManager.Instance.save.isExpPreserved == 0) {
                GameManager.Instance.save.PreserveExp();
                CharacterManager.Instance.UpdateSandstarMax();
            } else if (stageId != StageManager.Instance.randomSettings.stageNumber && StageManager.Instance.IsRandomStage && GameManager.Instance.save.isExpPreserved != 0) {
                if (PauseController.Instance) {
                    PauseController.Instance.CopyItems_Reserve(true);
                }
                GameManager.Instance.save.RestoreExp();
                CharacterManager.Instance.UpdateSandstarMax();
            }
            GameManager.Instance.save.moveByBus = 0;
            StageManager.Instance.MoveStage(stageId, floorNum < 0 ? 0 : floorNum, saveSlot);
            if (PauseController.Instance) {
                PauseController.Instance.CopyItems_Done();
            }
        }
    }

    protected override void PlayerEnter() {
        base.PlayerEnter();
        if (state == 1 && achievementId >= 1 && CharacterManager.Instance && StageManager.Instance) {
            CharacterManager.Instance.AchievementShow(title, StageManager.Instance.GetAchievement(achievementId), achievementId);
        }
    }

    protected override void PlayerExit() {
        base.PlayerExit();
        if (state == 0) {
            CharacterManager.Instance.AchievementHide();
        }
    }

}
