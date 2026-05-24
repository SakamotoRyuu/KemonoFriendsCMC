using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialConsole : MonoBehaviour {

    public GameObject balloonPrefab;
    public Vector3 balloonOffset;
    protected int state = 0;
    int pauseWait;
    bool actionTextEnabled;
    const string targetTag = "ItemGetter";

    void Start() {
        if (balloonPrefab) {
            Instantiate(balloonPrefab, transform.position + balloonOffset, transform.rotation, transform);
        }
    }

    protected virtual void Update() {
        if (PauseController.Instance && DifficultyController.Instance && CharacterManager.Instance) {
            if (PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
            if (pauseWait <= 0) {
                switch (state) {
                    case 1:
                        if (pauseWait <= 0 && PauseController.Instance.GetPauseEnabled() && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.SetTutorial(0);
                            state = 2;
                        }
                        break;
                    case 2:
                        state = 0;
                        break;
                }
            }
            bool actionTemp = (state == 1 && pauseWait <= 0 && PauseController.Instance.pauseEnabled);
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    protected virtual void PlayerEnter() {
        state = state == 0 ? 1 : state;
    }

    protected virtual void PlayerExit() {
        state = state == 1 ? 0 : state;
    }

    private void OnTriggerEnter(Collider other) {
        if (enabled && other.CompareTag(targetTag)) {
            PlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (enabled && other.CompareTag(targetTag)) {
            PlayerExit();
        }
    }
}
