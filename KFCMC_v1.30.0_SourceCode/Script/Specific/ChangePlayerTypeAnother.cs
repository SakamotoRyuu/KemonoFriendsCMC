using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ChangePlayerTypeAnother : MonoBehaviour {

    public GameObject effectPrefabTWR;

    Player playerInput;
    int state;
    int pauseWait;
    bool entering;
    bool actionTextEnabled;
    bool isAnother;
    float coolTime;

    const string targetTag = "ItemGetter";

    private void Start() {
        playerInput = GameManager.Instance.playerInput;
    }

    private void Update() {
        if (CharacterManager.Instance) {
            if (coolTime > 0f) {
                coolTime -= Time.deltaTime;
            }
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
                        if (pauseWait <= 0 && coolTime <= 0f && PauseController.Instance.GetPauseEnabled() && playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            isAnother = GameManager.Instance.IsPlayerAnother;
                            PauseController.Instance.SetChoices(2, true, TextManager.Get(isAnother ? "ITEM_NAME_132" : "ITEM_NAME_131"), "CHOICE_TRANSFORM", "CHOICE_CANCEL");
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
                            GameManager.Instance.save.playerType = (isAnother ? GameManager.playerType_Normal : GameManager.playerType_Another);
                            CharacterManager.Instance.SetNewPlayer(CharacterManager.Instance.GetOriginallyPlayerIndex);
                            Instantiate(effectPrefabTWR, CharacterManager.Instance.playerTrans.position, Quaternion.identity);
                            coolTime = 2f;
                            CharacterManager.Instance.AddSandstar(100, true);
                            CharacterManager.Instance.pCon.ForceStopForEvent(1f);
                            CancelCommon();
                            break;
                        case 1:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            CancelCommon();
                            break;
                    }
                    break;
            }
            bool actionTemp = (state == 1 && pauseWait <= 0 && coolTime <= 0f && PauseController.Instance.pauseEnabled);
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
