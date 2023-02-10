using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfernoGameover : MonoBehaviour {

    int state;
    float elapsedTime;

    private void Update() {
        switch (state) {
            case 0:
                if (CharacterManager.Instance && CharacterManager.Instance.pCon && CharacterManager.Instance.pCon.GetNowHP() <= 0) {
                    state = 1;
                }
                break;
            case 1:
                PauseController.Instance.pauseEnabled = false;
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 1.5f) {
                    SceneChange.Instance.StartEyeCatch(false);
                    state = 2;
                    elapsedTime = 0f;
                }
                break;
            case 2:
                if (SceneChange.Instance.GetEyeCatch()) {
                    PauseController.Instance.CancelChoices();
                    CharacterManager.Instance.PlayerRevive();
                    if (StageManager.Instance && StageManager.Instance.dungeonMother) {
                        StageManager.Instance.dungeonMother.MoveFloor(StageManager.Instance.floorNumber, -1, false);
                    }
                    SceneChange.Instance.EndEyeCatch();
                    state = 3;
                    elapsedTime = 0f;
                }
                break;
            case 3:
                state = 0;
                break;
        }
    }
}
