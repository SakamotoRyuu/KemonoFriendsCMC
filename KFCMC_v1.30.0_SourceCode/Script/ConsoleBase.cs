using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ConsoleBase : MonoBehaviour {
    
    public GameObject balloonPrefab;
    public Vector3 balloonOffset;
    public string titleKey;
    public string[] choiceKeys;
    public int choiceCancelNumber;

    protected Player playerInput;
    protected GameObject balloonInstance;
    protected int state;
    protected int pauseWait;
    protected bool entering;
    protected int choiceAnswer = -1;
    protected bool actionTextEnabled;
    protected const string targetTag = "ItemGetter";    

    protected void Start() {
        balloonInstance = Instantiate(balloonPrefab, transform.position + balloonOffset, transform.rotation, transform);
        playerInput = GameManager.Instance.playerInput;
    }

    protected void Update() {
        if (CharacterManager.Instance && PauseController.Instance) {
            if (state >= 2 && CharacterManager.Instance.pCon && CharacterManager.Instance.pCon.GetNowHP() <= 0) {
                PauseController.Instance.PauseDictionary(false);
                CancelCommon();
                return;
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
                        if (pauseWait <= 0 && PauseController.Instance.GetPauseEnabled() && playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            int choiceNum = choiceKeys.Length;
                            PauseController.Instance.SetChoices(choiceNum, true, TextManager.Get(titleKey), choiceKeys[0], choiceNum > 1 ? choiceKeys[1] : "", choiceNum >2  ? choiceKeys[2] : "", choiceNum > 3 ? choiceKeys[3] : "", choiceNum > 4 ? choiceKeys[4] : "");
                            choiceAnswer = -1;
                            state = 2;
                        }
                    }
                    break;
                case 2:
                    choiceAnswer = PauseController.Instance.ChoicesControl();
                    if (choiceAnswer != -1) {
                        if (choiceAnswer == -2) {
                            UISE.Instance.Play(UISE.SoundName.cancel);
                            CancelCommon();
                        } else if (choiceAnswer == choiceCancelNumber) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            CancelCommon();
                        } else {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.CancelChoices();
                            state = 3;
                            PauseController.Instance.PauseDictionary(true);
                            ConsoleStart();
                        }
                    }
                    break;
                case 3:
                    ConsoleUpdate();
                    if (!PauseController.Instance.pauseGame) {
                        state = 4;
                        ConsoleEnd();
                    }
                    break;
                case 4:
                    state = (entering ? 1 : 0);
                    break;
            }
            bool actionTemp = (state == 1 && pauseWait <= 0 && PauseController.Instance.pauseEnabled);
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    protected virtual void ConsoleStart() { }

    protected virtual void ConsoleUpdate() { }

    protected virtual void ConsoleEnd() { }

    protected void CancelCommon() {
        PauseController.Instance.CancelChoices();
        PauseController.Instance.HideCaution();
        CharacterManager.Instance.HideGold();
        state = (entering ? 1 : 0);
    }

    protected void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            entering = true;
            if (state < 2) {
                state = 1;
            }
        }
    }

    protected void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            entering = false;
            if (state < 2) {
                state = 0;
            }
        }
    }
}
