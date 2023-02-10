using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Amusement_MogisenSetting : MonoBehaviour {

    public GameObject balloonPrefab;
    public Vector3 balloonOffset;
    public GameObject multiMark;

    int state;
    int pauseWait;
    Transform trans;
    bool entering;
    bool actionTextEnabled;

    const string targetTag = "ItemGetter";
    Player playerInput;

    protected virtual void Awake() {
        trans = transform;
        if ((GameManager.Instance.save.minmi & 2) != 0) {
            Instantiate(balloonPrefab, trans.position + balloonOffset, trans.rotation, trans);
        } else {
            enabled = false;
        }
    }

    protected virtual void Start() {
        playerInput = GameManager.Instance.playerInput;
        if (multiMark) {
            multiMark.SetActive(CharacterManager.Instance && CharacterManager.Instance.mogisenMultiPlay);
        }
    }

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
                        if (pauseWait <= 0 && PauseController.Instance.GetPauseEnabled() && playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.SetChoices(3, true, TextManager.Get("AMUSEMENT_SETTING"), "AMUSEMENT_SINGLE", "AMUSEMENT_MULTI", "CHOICE_CANCEL");
                            state = 2;
                        }
                    }
                    break;
                case 2:
                    switch (PauseController.Instance.ChoicesControl()) {
                        case -2:
                            UISE.Instance.Play(UISE.SoundName.cancel);
                            PauseController.Instance.CancelChoices();
                            state = (entering ? 1 : 0);
                            break;
                        case 0:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.CancelChoices();
                            SetMultiPlay(false);
                            state = (entering ? 1 : 0);
                            break;
                        case 1:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.CancelChoices();
                            SetMultiPlay(true);
                            state = (entering ? 1 : 0);
                            break;
                        case 2:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.CancelChoices();
                            state = (entering ? 1 : 0);
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

    void SetMultiPlay(bool flag) {
        UISE.Instance.Play(UISE.SoundName.use);
        CharacterManager.Instance.mogisenMultiPlay = flag;
        MessageUI.Instance.SetMessage(TextManager.Get("QUOTE_START") + (flag ? TextManager.Get("AMUSEMENT_MULTI") : TextManager.Get("AMUSEMENT_SINGLE")) + TextManager.Get("QUOTE_END"));
        if (multiMark) {
            multiMark.SetActive(CharacterManager.Instance.mogisenMultiPlay);
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
