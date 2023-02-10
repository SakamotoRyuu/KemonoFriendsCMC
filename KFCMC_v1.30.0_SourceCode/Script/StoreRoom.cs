using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class StoreRoom : MonoBehaviour {
    
    public GameObject balloonPrefab;
    public Vector3 balloonOffset = new Vector3(0, 1.6f, 0);    

    Player playerInput;
    Transform trans;
    int state = 0;
    int pauseWait;
    bool actionTextEnabled;
    const string targetTag = "ItemGetter";

    protected virtual void Awake() {
        trans = transform;
        Instantiate(balloonPrefab, trans.position + balloonOffset, trans.rotation, trans);
    }

    protected virtual void Start() {
        playerInput = GameManager.Instance.playerInput;
    }
    
    protected virtual void Update() {
        if (CharacterManager.Instance) {
            switch (state) {
                case 1:
                    if (PauseController.Instance) {
                        if (PauseController.Instance.returnToLibraryProcessing) {
                            state = 0;
                        }
                        if (PauseController.Instance.pauseGame) {
                            pauseWait = 2;
                        } else if (pauseWait > 0) {
                            pauseWait--;
                        }
                        if (pauseWait <= 0 && PauseController.Instance.GetPauseEnabled() && playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.SetChoices(3, true, TextManager.Get(GameManager.Instance.IsPlayerAnother ? "WORD_STORAGE_ANOTHER" : "WORD_STORAGE"), "CHOICE_STORE", "CHOICE_TAKEOUT", "CHOICE_CANCEL");
                            state = 2;
                        }
                    }
                    break;
                case 2:
                    switch (PauseController.Instance.ChoicesControl()) {
                        case -2:
                            UISE.Instance.Play(UISE.SoundName.cancel);
                            PauseController.Instance.CancelChoices();
                            state = 1;
                            break;
                        case 0:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.CancelChoices();
                            PauseController.Instance.PauseStore();
                            state = 1;
                            break;
                        case 1:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.CancelChoices();
                            PauseController.Instance.PauseTakeOut();
                            state = 1;
                            break;
                        case 2:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.CancelChoices();
                            state = 1;
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

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            state = 1;
        }
    }

    protected virtual void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            state = 0;
        }
    }
}
