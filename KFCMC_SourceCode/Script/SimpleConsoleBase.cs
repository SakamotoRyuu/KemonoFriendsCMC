using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleConsoleBase : MonoBehaviour {

    public GameObject balloonPrefab;
    public Vector3 balloonOffset;

    protected int state;
    protected int pauseWait;
    protected bool entering;
    protected bool actionTextEnabled;

    protected const string targetTag = "ItemGetter";

    protected virtual void Awake() {
        if (balloonPrefab) {
            Instantiate(balloonPrefab, transform.position + balloonOffset, transform.rotation, transform);
        }
    }

    protected virtual void SetChoice() { }

    protected virtual void UpdateChoice() { }

    protected virtual void Update() {
        if (CharacterManager.Instance) {
            switch (state) {
                case 1:
                    if (PauseController.Instance) {
                        if (PauseController.Instance.pauseGame) {
                            pauseWait = 2;
                        } else if (pauseWait > 0) {
                            pauseWait--;
                        }
                        if (PauseController.Instance.returnToLibraryProcessing) {
                            state = 0;
                        }
                        if (pauseWait <= 0 && PauseController.Instance.GetPauseEnabled() && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            SetChoice();
                            state = 2;
                        }
                    }
                    break;
                case 2:
                    UpdateChoice();
                    break;
            }
            bool actionTemp = (state == 1 && pauseWait <= 0 && PauseController.Instance.pauseEnabled);
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    protected virtual void PlayerEnter() {
        entering = true;
        state = state == 0 ? 1 : state;
    }

    protected virtual void PlayerExit() {
        entering = false;
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
