using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmusementPark_FerrisWheelConsole : SimpleConsoleBase {

    public AmusementPark_FerrisWheel attractionFerrisWheel;
    public AmusementPark_RainbowEmission rainbowEmission;
    public float[] speedArray;
    public float[] emissionStoppingArray;
    public float[] emissionTransitionArray;

    int choiceAnswer;

    protected override void SetChoice() {
        PauseController.Instance.SetChoices(7, true, TextManager.Get("AP_WHEEL_TITLE"), "AP_WHEEL_0", "AP_WHEEL_1", "AP_WHEEL_2", "AP_WHEEL_3", "AP_WHEEL_4", "AP_WHEEL_5", "CHOICE_CANCEL");
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
            case 6:
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices();
                state = (entering ? 1 : 0);
                break;
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices();
                ChangeSpeed(choiceAnswer);
                state = (entering ? 1 : 0);
                break;
        }
    }

    void ChangeSpeed(int speedIndex) {
        UISE.Instance.Play(UISE.SoundName.use);
        MessageUI.Instance.SetMessage(TextManager.Get("QUOTE_START") + TextManager.Get("AP_WHEEL_" + speedIndex.ToString()) + TextManager.Get("QUOTE_END"));
        if (attractionFerrisWheel) {
            attractionFerrisWheel.rotationSpeed = speedArray[speedIndex];
        }
        if (rainbowEmission) {
            rainbowEmission.timeMultiplierStopping = emissionStoppingArray[speedIndex];
            rainbowEmission.timeMultiplierTransition = emissionTransitionArray[speedIndex];
        }
    }
}
