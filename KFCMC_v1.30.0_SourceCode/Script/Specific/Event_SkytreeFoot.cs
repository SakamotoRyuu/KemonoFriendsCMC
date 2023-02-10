using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_SkytreeFoot : MonoBehaviour {

    public GameObject balloonPrefab;
    public Transform balloonPivot;
    public GameObject emissionEffectPrefab;
    public GameObject destroyTarget;
    public GameObject destroyEffectPrefab;
    public GameObject activateTarget;
    public int newLightingNumber;
    public int newAmbientNumber;
    public string title;

    const string targetTag = "ItemGetter";
    const int pfAmuletID = 31;
    GameObject balloonInstance;
    int state;
    int pauseWait;
    float eventTimer;
    bool actionTextEnabled;

    private void Start() {
        if (balloonPivot && balloonPrefab && GameManager.Instance.save.weapon.Length > pfAmuletID && GameManager.Instance.save.weapon[pfAmuletID] != 0) {
            balloonInstance = Instantiate(balloonPrefab, balloonPivot);
        }
    }

    private void Update() {
        if (CharacterManager.Instance && PauseController.Instance) {
            if (PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
            if (pauseWait <= 0) {
                switch (state) {
                    case 1:
                        if (PauseController.Instance.returnToLibraryProcessing) {
                            state = 0;
                        }
                        if (pauseWait <= 0 && PauseController.Instance.GetPauseEnabled() && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            if (GameManager.Instance.save.weapon.Length > pfAmuletID && GameManager.Instance.save.weapon[pfAmuletID] != 0) {
                                UISE.Instance.Play(UISE.SoundName.submit);
                                PauseController.Instance.SetChoices(2, true, TextManager.Get(title), "CHOICE_AMULET", "CHOICE_CANCEL");
                                state = 2;
                            }
                        }
                        break;
                    case 2:
                        switch (PauseController.Instance.ChoicesControl()) {
                            case -2:
                                UISE.Instance.Play(UISE.SoundName.cancel);
                                PauseController.Instance.CancelChoices();
                                state = 1;
                                break;
                            case 0:
                                UISE.Instance.Play(UISE.SoundName.submit);
                                PauseController.Instance.CancelChoices();
                                if (CharacterManager.Instance.playerTrans) {
                                    Instantiate(emissionEffectPrefab, CharacterManager.Instance.playerTrans.position + Vector3.up * 0.8f + (transform.position - CharacterManager.Instance.playerTrans.position).normalized * 1f, Quaternion.identity);
                                }
                                if (balloonInstance) {
                                    Destroy(balloonInstance);
                                }
                                state = 3;
                                break;
                            case 1:
                                UISE.Instance.Play(UISE.SoundName.submit);
                                PauseController.Instance.CancelChoices();
                                state = 1;
                                break;
                        }
                        break;
                    case 3:
                        eventTimer += Time.deltaTime;
                        if (eventTimer >= 1.2f) {
                            EventBody();
                            state = 4;
                        }
                        break;
                    case 4:
                        break;
                }
            }
            bool actionTemp = (state == 1 && pauseWait <= 0 && PauseController.Instance.pauseEnabled && GameManager.Instance.save.weapon.Length > pfAmuletID && GameManager.Instance.save.weapon[pfAmuletID] != 0);
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    void EventBody() {
        Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        if (destroyTarget) {
            Destroy(destroyTarget);
        }
        if (activateTarget) {
            activateTarget.SetActive(true);
        }
        if (CameraManager.Instance) {
            CameraManager.Instance.SetQuake(transform.position, 10, 4, 0f, 0.5f, 2.5f, 75f, 200f);
        }
        if (LightingDatabase.Instance) {
            LightingDatabase.Instance.SetLighting(newLightingNumber);
        }
        if (Ambient.Instance) {
            Ambient.Instance.Play(newAmbientNumber, 2);
        }
        enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (enabled && other.CompareTag(targetTag)) {
            if (state == 0) {
                state = 1;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (enabled && other.CompareTag(targetTag)) {
            if (state == 1) {
                state = 0;
            }
        }
    }

}
