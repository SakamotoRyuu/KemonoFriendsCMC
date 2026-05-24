using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.CinemachineCore;

public class OnTriggerMoveFloor : MonoBehaviour {

    public int stageID;
    public int floorNumber;
    public bool blackOut;
    public float delay;
    public AudioSource[] stopAudio;
    public bool stopBGM;
    int state;
    float elapsedTime;
    int answer;
    int saveSlot = -1;

    private void Update() {
        switch (state) {
            case 0:
                break;
            case 1:
                elapsedTime += Time.unscaledDeltaTime;
                if (elapsedTime >= delay) {
                    if (PauseController.Instance.IsPhotoPausing) {
                        PauseController.Instance.PausePhoto(false);
                    }
                    GameManager.Instance.save.moveByBus = 0;
                    SaveController.Instance.permitEmptySlot = true;
                    SaveController.Instance.Activate();
                    state++;
                }
                break;
            case 2:
                saveSlot = -1;
                answer = SaveController.Instance.SaveControlExternal(true, false);
                if (answer >= 0 && answer < GameManager.saveSlotMax) {
                    saveSlot = answer;
                }
                if (answer != -1) {
                    if (!SceneChange.Instance.GetIsProcessing) {
                        SceneChange.Instance.StartEyeCatch(false);
                    }
                    state++;
                }
                break;
            case 3:
                if (SceneChange.Instance.GetEyeCatch()) {
                    PauseController.Instance.SetBlackCurtain(0f);
                    CharacterManager.Instance.SetPlayerUpdateEnabled(true);
                    MoveStage();
                    SceneChange.Instance.EndEyeCatch();
                    PauseController.Instance.pauseEnabled = true;
                    state++;
                    Destroy(gameObject);
                }
                break;
        }
    }
    private void MoveStage()
    {
        if (StageManager.Instance != null)
        {
            if ((stageID == StageManager.Instance.randomSettings.stageNumber || stageID == StageManager.tribulationsStageNumber) &&
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
                GameManager.Instance.save.PreserveExp(stageID == StageManager.tribulationsStageNumber && GameManager.Instance.save.difficulty >= GameManager.difficultyVU);
                CharacterManager.Instance.UpdateSandstarMax();
            }
            else if (stageID != StageManager.Instance.randomSettings.stageNumber &&
                stageID != StageManager.tribulationsStageNumber &&
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
            StageManager.Instance.MoveStage(stageID, floorNumber < 0 ? 0 : floorNumber, saveSlot);
            if (PauseController.Instance)
            {
                PauseController.Instance.CopyItems_Done();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            state = 1;
            PauseController.Instance.pauseEnabled = false;
            CharacterManager.Instance.SetPlayerUpdateEnabled(false);
            if (blackOut) {
                PauseController.Instance.SetBlackCurtain(1f);
                Time.timeScale = 0;
            }
            if (stopAudio.Length > 0) {
                for (int i = 0; i < stopAudio.Length; i++) {
                    if (stopAudio[i]) {
                        stopAudio[i].Stop();
                    }
                }
            }
            if (stopBGM) {
                BGM.Instance.Stop();
                Ambient.Instance.Stop();
            }
        }
    }

}
