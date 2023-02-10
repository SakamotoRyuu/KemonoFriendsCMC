using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    StageManager.Instance.MoveStage(stageID, floorNumber, saveSlot);
                    SceneChange.Instance.EndEyeCatch();
                    PauseController.Instance.pauseEnabled = true;
                    state++;
                    Destroy(gameObject);
                }
                break;
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
