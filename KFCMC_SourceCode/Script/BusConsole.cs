using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class BusConsole : MonoBehaviour {

    public GameObject balloonPrefab;
    public Vector3 balloonOffset = new Vector3(0, 1.6f, 0);
    public Transform cameraPivot;
    public Transform cameraFocusPoint;

    Player playerInput;
    int state = 0;
    int answer;
    int saveSlot = -1;
    int stageId;
    int floorNum;
    int pauseWait;
    bool entering;
    bool actionTextEnabled;
    const string targetTag = "ItemGetter";    

    void Start() {
        Instantiate(balloonPrefab, transform.position + balloonOffset, transform.rotation, transform);
        playerInput = GameManager.Instance.playerInput;
    }

    void Update() {
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
                            PauseController.Instance.SetChoices(2, true, TextManager.Get("WORD_BUS"), "CHOICE_RIDE", "CHOICE_CANCEL");
                            state = 2;
                        }
                    }
                    break;
                case 2:
                    switch (PauseController.Instance.ChoicesControl()) {
                        case -2:
                            UISE.Instance.Play(UISE.SoundName.cancel);
                            PauseController.Instance.CancelChoices();
                            CancelCommon();
                            break;
                        case 0:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            if (cameraPivot && CameraManager.Instance) {
                                CameraManager.Instance.SetEventCamera(cameraPivot.position, cameraPivot.eulerAngles, float.MaxValue, 1f, Vector3.Distance(cameraPivot.position, cameraFocusPoint.position));
                            }
                            PauseController.Instance.CancelChoices();
                            PauseController.Instance.bus.busConsole = this;
                            PauseController.Instance.PauseBus();
                            state = 3;
                            break;
                        case 1:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.CancelChoices();
                            CancelCommon();
                            break;
                    }
                    break;
                case 3:
                    if (!PauseController.Instance.pauseGame) {
                        state = 4;
                    }
                    break;
                case 4:
                    CancelCommon();
                    break;
                case 11:
                    state = 12;
                    break;
                case 12:
                    switch (PauseController.Instance.ChoicesControl()) {
                        case -2:
                            UISE.Instance.Play(UISE.SoundName.cancel);
                            PauseController.Instance.CancelChoices();
                            PauseController.Instance.HideCaution();
                            CancelCommon();
                            break;
                        case 0:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.CancelChoices(false);
                            PauseController.Instance.HideCaution();
                            SceneChange.Instance.StartEyeCatch();
                            saveSlot = -1;
                            state = 14;
                            break;
                        case 1:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.CancelChoices(false);
                            PauseController.Instance.HideCaution();
                            SaveController.Instance.permitEmptySlot = true;
                            SaveController.Instance.Activate();
                            state = 13;
                            break;
                        case 2:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.CancelChoices();
                            PauseController.Instance.HideCaution();
                            CancelCommon();
                            break;
                    }
                    break;
                case 13:
                    saveSlot = -1;
                    answer = SaveController.Instance.SaveControlExternal();
                    if (answer >= 0 && answer < GameManager.saveSlotMax) {
                        saveSlot = answer;
                        state += 1;
                    } else if (answer < -1) {
                        PauseController.Instance.CancelChoices();
                        CancelCommon();
                    }
                    break;
                case 14:
                    if (SceneChange.Instance.GetEyeCatch()) {
                        PauseController.Instance.CancelChoices();
                        if (stageId != StageManager.homeStageId) {
                            GameManager.Instance.save.moveByBus = 1;
                        }
                        StageManager.Instance.MoveStage(stageId, floorNum < 0 ? 0 : floorNum, saveSlot, true);
                        SceneChange.Instance.EndEyeCatch();
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

    void CancelCommon() {
        state = (entering ? 1 : 0);
    }

    public void SetMoveChoices(int index) {
        stageId = PauseController.Instance.bus.destination[index].x;
        floorNum = PauseController.Instance.bus.destination[index].y;
        PauseController.Instance.SetChoices(3, true, TextManager.Get(PauseController.Instance.bus.dicKey[index]), "CHOICE_ENTER", "CHOICE_SAVEENTER", "CHOICE_CANCEL");
        state = 11;
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            entering = true;
            if (state < 2) {
                state = 1;
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            entering = false;
            if (state < 2) {
                state = 0;
            }
        }
    }

    private void OnDestroy() {
        if ((state == 2 || state == 12) && PauseController.Instance) {
            PauseController.Instance.CancelChoices();
        }
    }

}
