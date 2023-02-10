using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSub_SummitSisin : MonoBehaviour
{

    public Event_Summit eventParent;
    public GameObject balloonPrefab;
    public GameObject emissionEffectPrefab;
    public GameObject normalSisin;
    public GameObject emissionSisin;
    public string title;

    const string targetTag = "ItemGetter";
    int state;
    int pauseWait;
    GameObject balloonInst;
    GameObject mapChip;

    private void Start() {
        balloonInst = Instantiate(balloonPrefab, transform.position, Quaternion.identity, transform);
        mapChip = Instantiate(MapDatabase.Instance.prefab[MapDatabase.other], transform);
    }

    protected virtual void Update() {
        if (PauseController.Instance) {
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
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.SetChoices(2, true, TextManager.Get(title), "CHOICE_AMULET", "CHOICE_CANCEL");
                            state = 2;
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
                                EventBody();
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
                        break;
                }
            }
        }
    }

    void EventBody() {
        CharacterManager.Instance.ClearSickAll();
        CharacterManager.Instance.Heal(0, 100, -1, true, true);
        if (balloonInst) {
            balloonInst.SetActive(false);
        }
        if (mapChip) {
            mapChip.SetActive(false);
        }
        normalSisin.SetActive(false);
        emissionSisin.SetActive(true);
        Instantiate(emissionEffectPrefab, transform.position, Quaternion.identity);
        eventParent.ActivateSisin();
        if (CharacterManager.Instance) {
            CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
        }
        enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (enabled && other.CompareTag(targetTag)) {
            state = state == 0 ? 1 : state;
            if (state == 1 && CharacterManager.Instance) {
                CharacterManager.Instance.SetActionType(CharacterManager.ActionType.Search, gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (enabled && other.CompareTag(targetTag)) {
            state = state == 1 ? 0 : state;
            if (state == 0 && CharacterManager.Instance) {
                CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
            }
        }
    }

}
