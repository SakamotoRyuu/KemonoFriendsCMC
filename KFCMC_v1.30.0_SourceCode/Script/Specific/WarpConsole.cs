using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class WarpConsole : MonoBehaviour {

    public Transform specifiedDestination;
    public bool resetCamera = false;
    public bool effectEnabled = true;
    public bool unnecessaryButton;

    Player playerInput;
    int pauseWait;
    int state = 0;
    bool actionTextEnabled;

    void Start() {
        playerInput = GameManager.Instance.playerInput;
    }

    protected virtual void Warp() {
        if (specifiedDestination) {
            CharacterManager.Instance.SuperWarp(specifiedDestination.position, resetCamera, effectEnabled);
        } else {
            CharacterManager.Instance.SuperWarp(resetCamera, effectEnabled);
        }
    }

    void Update() {
        if (CharacterManager.Instance && PauseController.Instance) {
            if (PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
            if (state == 1 && pauseWait <= 0 && PauseController.Instance.pauseEnabled && (unnecessaryButton || playerInput.GetButtonDown(RewiredConsts.Action.Submit))) {
                state = 0;
                Warp();
            }
            bool actionTemp = (state == 1 && pauseWait <= 0 && PauseController.Instance.pauseEnabled);
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (state == 0 && other.CompareTag("ItemGetter")) {
            state = 1;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (state != 0 && other.CompareTag("ItemGetter")) {
            state = 0;
        }
    }

}
