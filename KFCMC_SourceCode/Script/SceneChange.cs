using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChange : SingletonMonoBehaviour<SceneChange> {
    
    public GameObject eyeCatch;
    
    private GameObject eyeCatchInstance;
    private Image image;
    private bool isProcessing = false;
    private bool incrementFlag;
    private bool changeOK;
    private float amount;
    int waitFrames;
    bool endedFlag;
    bool reserveCheckTrophy;

    [System.NonSerialized]
    public bool reserveShortcut;

    protected override void Awake() {
        base.Awake();
        EndEyeCatch();
    }

    public void StartEyeCatch(bool sfx = true) {
        if (eyeCatchInstance != null) {
            Destroy(eyeCatchInstance);
            eyeCatchInstance = null;
        }
        eyeCatchInstance = Instantiate(eyeCatch);
        image = eyeCatchInstance.GetComponentInChildren<Image>();
        amount = 0;
        incrementFlag = true;
        changeOK = false;
        GameManager.Instance.ChangeTimeScale(true);
        image.fillClockwise = true;
        image.fillAmount = 0;
        if (sfx) {
            UISE.Instance.Play(UISE.SoundName.bridge);
        }
        waitFrames = 1;
        isProcessing = true;
        if (PauseController.Instance) {
            PauseController.Instance.pauseEnabled = false;
        }
    }

    public bool GetEyeCatch() {
        return changeOK || !isProcessing;
    }

    public bool GetIsProcessing {
        get {
            return isProcessing;
        }
    }

    public bool GetIsEnding {
        get {
            return isProcessing && !incrementFlag;
        }
    }

    public void EndEyeCatch() {
        if (eyeCatchInstance == null) {
            eyeCatchInstance = Instantiate(eyeCatch);
            image = eyeCatchInstance.GetComponentInChildren<Image>();
        }
        amount = 1;
        incrementFlag = false;
        changeOK = false;
        GameManager.Instance.ChangeTimeScale(false);
        image.fillClockwise = false;
        image.fillAmount = 1;
        waitFrames = 2;
        isProcessing = true;
        if (PauseController.Instance) {
            PauseController.Instance.pauseEnabled = true;
        }
    }

    private void Update() {
        if (PauseController.Instance && !PauseController.Instance.pauseGame && PauseController.Instance.pauseEnabled) {
            if (endedFlag) {
                GameManager.Instance.CheckTutorial();
                endedFlag = GameManager.Instance.tutorialContinuousFlag;
                reserveCheckTrophy = true;
            } else if (!isProcessing) {
                if (reserveCheckTrophy) {
                    reserveCheckTrophy = false;
                    TrophyManager.Instance.CheckTrophy_Clear(!GameManager.Instance.dontShowTrophyOnStart);
                    GameManager.Instance.dontShowTrophyOnStart = false;
                }
                if (reserveShortcut) {
                    reserveShortcut = false;
                    MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_FINDSHORTCUT"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Friends);
                    Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.notificationShortcut]);
                }
            }  
        }
        if (isProcessing) {
            if (waitFrames <= 0) {
                if (incrementFlag) {
                    if (amount >= 1.0f) {
                        changeOK = true;
                    } else {
                        amount += Time.unscaledDeltaTime;
                    }
                    if (amount > 1.0f) {
                        amount = 1.0f;
                    }
                } else {
                    if (amount <= 0.0f) {
                        Destroy(eyeCatchInstance);
                        eyeCatchInstance = null;
                        isProcessing = false;
                        endedFlag = true;
                    } else {
                        amount -= Time.unscaledDeltaTime;
                    }
                    if (amount < 0.0f) {
                        amount = 0.0f;
                    }
                }
                if (eyeCatchInstance != null && image != null) {
                    image.fillAmount = amount;
                }
            } else {
                waitFrames--;
            }
        } 
    }

}
