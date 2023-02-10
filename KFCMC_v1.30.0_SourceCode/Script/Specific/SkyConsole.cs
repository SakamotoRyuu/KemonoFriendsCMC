using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyConsole : SimpleConsoleBase {
    
    public int[] skyNumbers;
    
    int choiceAnswer;

    protected override void SetChoice() {
        PauseController.Instance.SetChoices(4, true, TextManager.Get("AP_SKY_TITLE"), "AP_SKY_0", "AP_SKY_1", "AP_SKY_2", "CHOICE_CANCEL");
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
            case 3:
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices();
                state = (entering ? 1 : 0);
                break;
            default:
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices();
                SetSky(choiceAnswer);
                state = (entering ? 1 : 0);
                break;
        }
    }

    void SetSky(int index) {
        UISE.Instance.Play(UISE.SoundName.use);
        if (index >= 0 && index < skyNumbers.Length) {
            LightingDatabase.Instance.SetLighting(skyNumbers[index]);
            MessageUI.Instance.SetMessage(TextManager.Get("QUOTE_START") + TextManager.Get("AP_SKY_" + index.ToString()) + TextManager.Get("QUOTE_END"));
        }
    }    

}
