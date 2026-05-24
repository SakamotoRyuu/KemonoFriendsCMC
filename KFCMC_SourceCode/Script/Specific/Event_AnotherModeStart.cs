using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event_AnotherModeStart : MonoBehaviour {

    public GameObject blackCurtainPrefab;
    private Image curtainImage;
    private Color curtainColor = new Color(0f, 0f, 0f, 1f);
    private float elapsedTime;
    private int progress;
    private GameObject blackCurtainInstance;

    void NextProgress() {
        progress++;
        elapsedTime = 0f;
    }

    void Start() {
        if (GameManager.Instance.save.progress == 0 && GameManager.Instance.IsPlayerAnother) {
            if (PauseController.Instance) {
                PauseController.Instance.pauseEnabled = false;
                blackCurtainInstance = Instantiate(blackCurtainPrefab, PauseController.Instance.offPauseCanvas.transform);
                blackCurtainInstance.transform.SetAsFirstSibling();
                curtainImage = blackCurtainInstance.GetComponentInChildren<Image>();
                if (curtainImage) {
                    curtainImage.color = curtainColor;
                }
            }
            progress = 0;
        } else {
            progress = -1;
            enabled = false;
        }
    }

    private void Update() {
        elapsedTime += Time.unscaledDeltaTime;
        switch (progress) {
            case 0:
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.ForceStopForEventAll(10f);
                }
                if (elapsedTime > 0.4f) {
                    GameManager.Instance.ChangeTimeScale(true, true);
                    if (CharacterManager.Instance) {
                        CharacterManager.Instance.ForceStopForEventAll(10f);
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_SP_00", -1);
                    }
                    NextProgress();
                }
                break;
            case 1:
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.ForceStopForEventAll(10f);
                }
                if (elapsedTime > 2f) {
                    GameManager.Instance.ChangeTimeScale(true, true);
                    NextProgress();
                }
                break;
            case 2:
                if (elapsedTime > 3f) {
                    if (curtainImage) {
                        curtainColor.a = 0f;
                        curtainImage.color = curtainColor;
                    }
                    if (blackCurtainInstance) {
                        Destroy(blackCurtainInstance);
                    }
                    if (MessageUI.Instance) {
                        MessageUI.Instance.DestroyMessageSlotType(MessageUI.slotType_Speech);
                    }
                    if (CharacterManager.Instance) {
                        CharacterManager.Instance.ReleaseStopForEventAll();
                    }
                    if (PauseController.Instance) {
                        PauseController.Instance.pauseEnabled = true;
                    }
                    GameManager.Instance.save.config[GameManager.Save.configID_Gauge] = 1;
                    if (CanvasCulling.Instance) {
                        CanvasCulling.Instance.CheckConfig();
                    }
                    GameManager.Instance.ChangeTimeScale(false, true);
                    NextProgress();
                } else {
                    if (curtainImage) {
                        curtainColor.a = 1f - (elapsedTime / 3f);
                        curtainImage.color = curtainColor;
                    }
                }
                break;
            case 3:
                break;
        }
    }
}
