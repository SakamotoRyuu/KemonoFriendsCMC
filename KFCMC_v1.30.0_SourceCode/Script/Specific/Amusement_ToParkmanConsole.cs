using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amusement_ToParkmanConsole : MonoBehaviour {

    public int stageNum;
    public int floorNum;
    public GameObject eventPrefab;

    int state;
    int pauseWait;
    bool actionTextEnabled;
    const string targetTag = "ItemGetter";

    private void Start() {
        Instantiate(MapDatabase.Instance.prefab[MapDatabase.goal], transform);
    }

    private void Update() {
        if (CharacterManager.Instance) {
            if (PauseController.Instance && PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
            if (pauseWait <= 0) {
                switch (state) {
                    case 0:
                        break;
                    case 1:
                        if (PauseController.Instance.returnToLibraryProcessing) {
                            state = 0;
                        }
                        if (pauseWait <= 0 && PauseController.Instance.GetPauseEnabled() && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.SetChoices(2, true, TextManager.Get("AMUSEMENT_VIDEOGAME"), "AMUSEMENT_PLAY", "CHOICE_CANCEL");
                            state = 2;
                        }
                        break;
                    case 2:
                        switch (PauseController.Instance.ChoicesControl()) {
                            case -2:
                                UISE.Instance.Play(UISE.SoundName.cancel);
                                PauseController.Instance.CancelChoices();
                                PauseController.Instance.HideCaution();
                                state = 1;
                                break;
                            case 0:
                                UISE.Instance.Play(UISE.SoundName.submit);
                                PauseController.Instance.CancelChoices(false);
                                PauseController.Instance.HideCaution();
                                SceneChange.Instance.StartEyeCatch();
                                state = 3;
                                break;
                            case 1:
                                UISE.Instance.Play(UISE.SoundName.submit);
                                PauseController.Instance.CancelChoices();
                                PauseController.Instance.HideCaution();
                                state = 1;
                                break;
                        }
                        break;
                    case 3:
                        if (SceneChange.Instance.GetEyeCatch()) {
                            PauseController.Instance.CancelChoices();
                            if (CharacterManager.Instance && StageManager.Instance && StageManager.Instance.dungeonMother) {
                                CharacterManager.Instance.SetNewPlayer(CharacterManager.playerIndexParkman);
                                if (StageManager.Instance.stageNumber == stageNum) {
                                    StageManager.Instance.dungeonMother.MoveFloor(floorNum, -1, false);
                                } else {
                                    StageManager.Instance.MoveStage(stageNum, floorNum, -1, false);
                                }
                                if (eventPrefab) {
                                    Instantiate(eventPrefab, StageManager.Instance.dungeonController.transform);
                                }
                            }
                            SceneChange.Instance.EndEyeCatch();
                        }
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

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            state = state == 0 ? 1 : state;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            state = state == 1 ? 0 : state;
        }
    }

}
