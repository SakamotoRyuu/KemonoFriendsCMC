using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class SelfDestructSwitch : MonoBehaviour {

    public GameObject destroyTarget;
    public GameObject effectPrefab;
    public Transform effectPivot;

    Player playerInput;
    int pauseWait;
    int state = 0;
    bool entering;
    bool actionTextEnabled;
    const string targetTag = "ItemGetter";

    void Start() {
        playerInput = GameManager.Instance.playerInput;
    }

    void Execute() {
        if (destroyTarget) {
            Destroy(destroyTarget);
            if (effectPrefab) {
                if (effectPivot) {
                    Instantiate(effectPrefab, effectPivot.position, effectPivot.rotation);
                } else {
                    Instantiate(effectPrefab, transform.position, transform.rotation);
                }
            }
        }
        enabled = false;
    }    

    private void Update() {
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
                        if (pauseWait <= 0 && PauseController.Instance.GetPauseEnabled() && playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.SetChoices(2, true, TextManager.Get("WORD_BUTTON"), "CHOICE_PUSH", "CHOICE_CANCEL");
                            state = 2;
                        }
                    }
                    break;
                case 2:
                    switch (PauseController.Instance.ChoicesControl()) {
                        case -2:
                            UISE.Instance.Play(UISE.SoundName.cancel);
                            CancelCommon();
                            break;
                        case 0:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            Execute();
                            CancelCommon();
                            break;
                        case 1:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            CancelCommon();
                            break;
                    }
                    break;
            }
            bool actionTemp = (state == 1 && pauseWait <= 0 && PauseController.Instance.pauseEnabled);
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    private void CancelCommon() {
        PauseController.Instance.CancelChoices();
        state = (entering ? 1 : 0);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            entering = true;
            state = 1;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            entering = false;
            state = 0;
        }
    }
}
