using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ChangePlayerTypeOnSubmit : MonoBehaviour {

    public GameObject effectPrefabTWR;

    Player playerInput;
    int state = 0;
    int pauseWait;
    bool entering;
    bool toHyper;
    float coolTime;
    bool actionTextEnabled;
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
                            if (GameManager.Instance.IsPlayerAnother) {
                                UISE.Instance.Play(UISE.SoundName.use);
                                GameManager.Instance.save.playerType = (GameManager.Instance.save.playerType == GameManager.playerType_Another ? GameManager.playerType_AnotherEscape : GameManager.playerType_Another);
                                CharacterManager.Instance.SetNewPlayer(CharacterManager.Instance.GetOriginallyPlayerIndex);
                            } else {
                                UISE.Instance.Play(UISE.SoundName.submit);
                                toHyper = GameManager.Instance.save.playerType != GameManager.playerType_Hyper;
                                PauseController.Instance.SetChoices(2, true, TextManager.Get(toHyper ? "WORD_TRUEWILDRELEASE" : "WORD_USUALSERVAL"), "CHOICE_TRANSFORM", "CHOICE_CANCEL");
                                state = 2;
                            }
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
                            GameManager.Instance.save.playerType = (toHyper ? GameManager.playerType_Hyper : GameManager.playerType_Normal);
                            CharacterManager.Instance.SetNewPlayer(CharacterManager.Instance.GetOriginallyPlayerIndex);
                            if (toHyper) {
                                Instantiate(effectPrefabTWR, CharacterManager.Instance.playerTrans.position, Quaternion.identity);
                                coolTime = 2f;
                                CharacterManager.Instance.AddSandstar(100, true);
                                CharacterManager.Instance.pCon.SupermanStart(false);
                                CharacterManager.Instance.pCon.ForceStopForEvent(1f);
                            } else {
                                CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.friendsYKEnd, -1, true, true, false);
                                CharacterManager.Instance.pCon.SupermanEnd(false);
                                coolTime = 1f;
                                CharacterManager.Instance.pCon.ForceStopForEvent(0.5f);
                            }
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
