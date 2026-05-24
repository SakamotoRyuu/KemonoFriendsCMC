using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmusementPark_AttractionConsole : SimpleConsoleBase {

    public AmusementPark_FerrisWheel attractionFerrisWheel;
    public AmusementPark_Balloon attractionBalloon;
    public AmusementPark_PirateShip attractionPirateShip;
    public AmusementPark_TeaCup attractionTeaCup;
    public AmusementPark_MerryGoRound attractionMerryGoRound;
    public MeshRenderer consoleMachine;
    public int matIndex;
    public Material[] lightMaterial;

    int choiceAnswer;
    bool isRunning = true;

    protected override void SetChoice() {
        PauseController.Instance.SetChoices(2, true, TextManager.Get("AP_RIDE_TITLE"), isRunning ? "AP_RIDE_1" : "AP_RIDE_0", "CHOICE_CANCEL");
    }

    protected override void UpdateChoice() {
        choiceAnswer = PauseController.Instance.ChoicesControl();
        switch (choiceAnswer) {
            case -1:
                break;
            case -2:
                UISE.Instance.Play(UISE.SoundName.cancel);
                PauseController.Instance.CancelChoices();
                state = (entering ? 1 : 0);
                break;
            case 0:
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices();
                ChangeRunning();
                state = (entering ? 1 : 0);
                break;
            case 1:
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices();
                state = (entering ? 1 : 0);
                break;
        }
    }

    void ChangeRunning() {
        UISE.Instance.Play(UISE.SoundName.use);
        isRunning = !isRunning;
        MessageUI.Instance.SetMessage(TextManager.Get("QUOTE_START") + TextManager.Get(isRunning ? "AP_RIDE_0" : "AP_RIDE_1") + TextManager.Get("QUOTE_END"));
        if (attractionFerrisWheel) {
            attractionFerrisWheel.SetRunning(isRunning);
        }
        if (attractionBalloon) {
            attractionBalloon.SetRunning(isRunning);
        }
        if (attractionPirateShip) {
            attractionPirateShip.SetRunning(isRunning);
        }
        if (attractionTeaCup) {
            attractionTeaCup.SetRunning(isRunning);
        }
        if (attractionMerryGoRound) {
            attractionMerryGoRound.SetRunning(isRunning);
        }
        if (consoleMachine && lightMaterial.Length >= 2) {
            Material[] matTemp = consoleMachine.materials;
            matTemp[matIndex] = lightMaterial[isRunning ? 0 : 1];
            consoleMachine.materials = matTemp;
        }
    }
}
