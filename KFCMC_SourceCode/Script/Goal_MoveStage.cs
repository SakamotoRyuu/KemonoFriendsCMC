using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_MoveStage : Goal
{

    public int stageId;
    public int achievementId;

    protected override void Move()
    {
        if (StageManager.Instance != null)
        {
            if ((stageId == StageManager.Instance.randomSettings.stageNumber || stageId == StageManager.tribulationsStageNumber) && 
                !StageManager.Instance.IsWithoutBringItemStage && 
                GameManager.Instance.save.isExpPreserved == 0 &&
                !GameManager.Instance.CanBringInventory())
            {
                // ハイパー状態であれば通常に戻す
                if (GameManager.Instance.save.playerIndex == GameManager.playerIndex_HyperServal || GameManager.Instance.save.isPlayerHyper != 0)
                {
                    if (CharacterManager.Instance.IsPlayerServal)
                    {
                        CharacterManager.Instance.SetNewPlayer(GameManager.playerIndex_Serval, true, false);
                    }
                    else
                    {
                        CharacterManager.Instance.SetNewPlayer(CharacterManager.Instance.playerIndex, true, false);
                    }
                }
                GameManager.Instance.save.PreserveExp(stageId == StageManager.tribulationsStageNumber && GameManager.Instance.save.difficulty >= GameManager.difficultyVU);
                CharacterManager.Instance.UpdateSandstarMax();
            }
            else if (stageId != StageManager.Instance.randomSettings.stageNumber && 
                stageId != StageManager.tribulationsStageNumber && 
                StageManager.Instance.IsWithoutBringItemStage &&
                GameManager.Instance.save.isExpPreserved != 0)
            {
                if (PauseController.Instance)
                {
                    PauseController.Instance.CopyItems_Reserve(true);
                }
                GameManager.Instance.save.RestoreExp();
                CharacterManager.Instance.UpdateSandstarMax();
            }
            GameManager.Instance.save.moveByBus = 0;
            StageManager.Instance.MoveStage(stageId, floorNum < 0 ? 0 : floorNum, saveSlot);
            if (overridePlayerFixEnabled)
            {
                CharacterManager.Instance.playerTrans.position = overridePlayerPosition;
                CharacterManager.Instance.playerTrans.eulerAngles = overridePlayerRotation;
                CharacterManager.Instance.PlaceFriendsAroundPlayer();
                CameraManager.Instance.ResetCameraFixPos();
            }
            if (PauseController.Instance)
            {
                PauseController.Instance.CopyItems_Done();
            }
        }
    }

    protected override void PlayerEnter()
    {
        base.PlayerEnter();
        if (state == 1 && achievementId >= 1 && CharacterManager.Instance && StageManager.Instance)
        {
            CharacterManager.Instance.AchievementShow(title, StageManager.Instance.GetAchievement(achievementId), achievementId);
        }
    }

    protected override void PlayerExit()
    {
        base.PlayerExit();
        if (state == 0)
        {
            CharacterManager.Instance.AchievementHide();
        }
    }

}
