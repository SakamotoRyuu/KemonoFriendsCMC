using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class SoundTestConsole : MonoBehaviour {

    public GameObject balloonPrefab;
    public Vector3 balloonOffset = new Vector3(0, 1.6f, 0);
    public Transform cameraPivot;
    public Transform cameraFocusPoint;
    public vintageRadioCassete radioCassete;

    Player playerInput;
    Transform trans;
    int state = 0;
    int pauseWait;
    bool entering;
    bool actionTextEnabled;
    const string targetTag = "ItemGetter";

    protected virtual void Awake() {
        trans = transform;
        Instantiate(balloonPrefab, trans.position + balloonOffset, trans.rotation, trans);
        if (radioCassete) {
            radioCassete.enabled = false;
        }
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
                            PauseController.Instance.SetChoices(2, true, TextManager.Get(GameManager.Instance.IsPlayerAnother ? "WORD_SOUNDTEST_ANOTHER" : "WORD_SOUNDTEST"), "CHOICE_SOUNDTEST", "CHOICE_CANCEL");
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
                            PauseController.Instance.PauseSoundTest();
                            if (radioCassete) {
                                radioCassete.enabled = true;
                            }
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
                        if (radioCassete) {
                            radioCassete.enabled = false;
                        }
                        CancelCommon();
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

}
