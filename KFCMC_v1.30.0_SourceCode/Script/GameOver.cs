using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour {

    public GameObject effect;
    public bool playBgm;
    public bool stopAmbient;
    public int bgmIndex;
    public float choicesStartTime = 4f;
    float time;
    int state;
    GameObject effectInstance;

    // Use this for initialization

    private void Awake() {
        PauseController.Instance.pauseEnabled = false;
    }

    void Start () {
        time = 0;
        state = 1;
        if (effect != null) {
            effectInstance = Instantiate(effect);
        }
        if (playBgm && BGM.Instance != null) {
            BGM.Instance.Play(bgmIndex);
        }
        if (stopAmbient && Ambient.Instance != null) {
            Ambient.Instance.Play(-1);
        }
    }

    void SaveCancelCommon() {
        SaveController.Instance.Deactivate();
        SetChoiceEnable(true);
        state = 2;
    }

    void SetChoiceEnable(bool flag) {
        PauseController.Instance.choices.canvas.enabled = flag;
        PauseController.Instance.choices.gRaycaster.enabled = flag ? GameManager.Instance.MouseEnabled : false;
    }
	
	// Update is called once per frame
	void Update () {
        switch (state) {
            case 1:
                if (time >= choicesStartTime && !PauseController.Instance.IsPhotoPausing) {
                    state = 2;
                    SetChoices();
                    PauseController.Instance.pauseEnabled = false;
                    CharacterManager.Instance.TimeStop(true);
                }
                time += Time.deltaTime;
                break;
            case 2:
                switch (PauseController.Instance.ChoicesControl()) {
                    case 0:
                        UISE.Instance.Play(UISE.SoundName.submit);
                        SetChoiceEnable(false);
                        SceneChange.Instance.StartEyeCatch();
                        state = 3;
                        break;
                    case 1:
                        UISE.Instance.Play(UISE.SoundName.submit);
                        SetChoiceEnable(false);
                        SaveController.Instance.Activate();
                        state = 4;
                        break;
                    case 2:
                        UISE.Instance.Play(UISE.SoundName.use);
                        CharacterManager.Instance.TimeStop(false);
                        GameManager.Instance.LoadScene("Title");
                        break;
                }
                break;
            case 3:
                if (SceneChange.Instance.GetEyeCatch()) {
                    CharacterManager.Instance.TimeStop(false);
                    if (CharacterManager.Instance.playerIndex != CharacterManager.Instance.GetOriginallyPlayerIndex) {
                        CharacterManager.Instance.SetNewPlayer(CharacterManager.Instance.GetOriginallyPlayerIndex);
                    }
                    if (GameManager.Instance.save.isExpPreserved != 0) {
                        PauseController.Instance.CopyItems_Reserve(true);
                        GameManager.Instance.save.RestoreExp();
                        CharacterManager.Instance.UpdateSandstarMax();
                    }
                    if (effectInstance) {
                        Destroy(effectInstance);
                    }
                    StageManager.Instance.MoveStage(0, StageManager.Instance.homeFloorNumber, -1);
                    PauseController.Instance.CopyItems_Done();
                    PauseController.Instance.SetBlackCurtain(0f, false);
                    SceneChange.Instance.EndEyeCatch();
                    PauseController.Instance.pauseEnabled = true;
                    Destroy(gameObject);
                }
                break;
            case 4:
                int controlType = SaveController.Instance.controlType;
                int answer = SaveController.Instance.SaveControl();
                if (controlType == 0) {
                    if (answer >= 0 && answer < GameManager.saveSlotMax) {
                        UISE.Instance.Play(UISE.SoundName.use);
                        LightingDatabase.Instance.nowLightingNumber = -1;
                        GameManager.Instance.DataLoad(answer);
                        GameManager.Instance.LoadScene("Play");
                        GameManager.Instance.ApplyVolume();
                        GameManager.Instance.ApplySpeakerMode();
                    } else {
                        switch (answer) {
                            case -2:
                                UISE.Instance.Play(UISE.SoundName.error);
                                break;
                            case -3:
                                UISE.Instance.Play(UISE.SoundName.cancel);
                                SaveCancelCommon();
                                break;
                        }
                    }
                } else {
                    if (answer >= 0 && answer < GameManager.saveSlotMax) {
                        UISE.Instance.Play(UISE.SoundName.submit);
                        SaveCancelCommon();
                    } else if (answer == -3) {
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        SaveCancelCommon();
                    }
                }
                break;
        }
	}
    
    protected virtual void SetChoices() {
        if (GameManager.Instance.IsPlayerAnother) {
            PauseController.Instance.SetChoices(3, true, TextManager.Get("WORD_GAMEOVER"), "CHOICE_QUIT_STAGE_ANOTHER", "CHOICE_RESTART", "CHOICE_QUIT_GAME");
        } else {
            PauseController.Instance.SetChoices(3, true, TextManager.Get("WORD_GAMEOVER"), "CHOICE_QUIT_STAGE", "CHOICE_RESTART", "CHOICE_QUIT_GAME");
        }
    }
    
}
