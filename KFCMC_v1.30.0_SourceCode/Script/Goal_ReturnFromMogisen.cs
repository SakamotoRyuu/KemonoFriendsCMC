using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_ReturnFromMogisen : Goal {

    public string choice4;
    Amusement_Mogisen mogisenInstance;
    string dicKey;

    protected override void Start() {
        base.Start();
        GameObject[] eventObj = GameObject.FindGameObjectsWithTag("GameController");
        if (eventObj.Length > 0) {
            for (int i = 0; i < eventObj.Length; i++) {
                mogisenInstance = eventObj[i].GetComponent<Amusement_Mogisen>();
                if (mogisenInstance != null) {
                    dicKey = mogisenInstance.dicKey;
                    break;
                }
            }
        }
    }

    protected override void Update() {
        if (mogisenInstance && mogisenInstance.Cleared) {
            pauseWait = 5;
        }
        base.Update();
    }

    protected override void ActionSubmitButton() {
        UISE.Instance.Play(UISE.SoundName.submit);
        string margedTitle;
        if (!string.IsNullOrEmpty(dicKey)) {
            margedTitle = TextManager.Get(title) + " " + TextManager.Get(dicKey);
        } else {
            margedTitle = TextManager.Get(title);
        }
        PauseController.Instance.SetChoices(4, true, margedTitle, choice1, choice2, choice3, choice4);
    }

    protected override void ChoicesControl() {
        switch (PauseController.Instance.ChoicesControl()) {
            case -2:
                UISE.Instance.Play(UISE.SoundName.cancel);
                PauseController.Instance.CancelChoices();
                PauseController.Instance.HideCaution();
                state = 1;
                break;
            case 0:
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices();
                PauseController.Instance.HideCaution();
                state = 1;
                break;
            case 1:
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices(false);
                PauseController.Instance.HideCaution();
                SceneChange.Instance.StartEyeCatch();
                saveSlot = -1;
                state = 4;
                break;
            case 2:
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices(false);
                PauseController.Instance.HideCaution();
                SaveController.Instance.permitEmptySlot = true;
                SaveController.Instance.Activate();
                state = 3;
                break;
            case 3:
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices();
                PauseController.Instance.HideCaution();
                SceneChange.Instance.StartEyeCatch();
                saveSlot = -1;
                state = 0;
                if (mogisenInstance) {
                    mogisenInstance.Retry();
                }
                break;
        }
    }

    protected override void Move() {
        base.Move();
        if (mogisenInstance) {
            mogisenInstance.ReadyForRestoreFriends();
            CharacterManager.Instance.RestoreFriends();
        }
    }

}
