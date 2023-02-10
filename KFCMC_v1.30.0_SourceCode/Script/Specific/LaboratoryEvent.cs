using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaboratoryEvent : MonoBehaviour {

    public GameObject destroyCheckTarget_1;
    public GameObject destroyCheckTarget_2;
    public GameObject[] normalTube;
    public GameObject[] breakedTube;
    public GameObject balloonPrefab;
    public GameObject bootSFX1;
    public GameObject bootSFX2;
    public GameObject startSFX;
    public GameObject loopSFX;
    public GameObject[] computer;
    public GameObject[] goalObject;
    public string messageKey;

    int state = 0;
    float duration = 0f;
    bool tatched = false;
    int pauseWait;
    GameObject balloonObj;
    GameObject mapChip;

    const int progress = 10;
    const string targetTag = "ItemGetter";

    void Update () {
        switch (state) {
            case 0:
                if (destroyCheckTarget_1 == null) {
                    state = 1;
                    duration = 0f;
                }
                break;
            case 1:
                duration += Time.deltaTime;
                if (duration >= 0.25f) {
                    for (int i = 0; i < normalTube.Length; i++) {
                        normalTube[i].SetActive(false);
                    }
                    for (int i = 0; i < breakedTube.Length; i++) {
                        breakedTube[i].SetActive(true);
                    }
                    if (StageManager.Instance) {
                        StageManager.Instance.SetActiveEnemies(true);
                    }
                    state = 2;
                    duration = 0f;
                }
                break;
            case 2:
                if (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.EnemyCount() <= 0) {
                    state = 3;
                    duration = 0f;
                }
                break;
            case 3:
                if (destroyCheckTarget_2 == null) {
                    state = 4;
                    if (balloonPrefab) {
                        balloonObj = Instantiate(balloonPrefab, transform.position, Quaternion.identity, transform);
                    }
                    mapChip = Instantiate(MapDatabase.Instance.prefab[MapDatabase.other], transform);
                }
                break;
            case 4:
                if (PauseController.Instance) {
                    if (PauseController.Instance.pauseGame) {
                        pauseWait = 2;
                    } else if (pauseWait > 0) {
                        pauseWait--;
                    }
                    if (tatched && pauseWait <= 0 && PauseController.Instance.pauseEnabled && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                        if (balloonObj) {
                            Destroy(balloonObj);
                        }
                        if (mapChip) {
                            mapChip.SetActive(false);
                        }
                        if (CharacterManager.Instance) {
                            CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
                        }
                        bootSFX1.SetActive(true);
                        state = 5;
                        duration = 0f;
                    }
                }
                break;
            case 5:
                duration += Time.deltaTime;
                if (duration >= 0.8f) {
                    for (int i = 0; i < computer.Length; i++) {
                        computer[i].SetActive(true);
                    }
                    bootSFX2.SetActive(true);
                    startSFX.SetActive(true);
                    loopSFX.SetActive(true);
                    AudioSource aSrc = loopSFX.GetComponent<AudioSource>();
                    aSrc.PlayDelayed(4f);
                    for (int i = 0; i < goalObject.Length; i++) {
                        goalObject[i].SetActive(true);
                    }
                    GameManager.Instance.save.SetClearStage(progress);
                    if (!string.IsNullOrEmpty(messageKey)) {
                        MessageUI.Instance.SetMessage(TextManager.Get(messageKey), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Lucky);
                    }
                    state = 6;
                    duration = 0f;
                }
                break;
            case 6:
                break;
        }
	}

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            tatched = true;
            if (state == 4 && CharacterManager.Instance) {
                CharacterManager.Instance.SetActionType(CharacterManager.ActionType.Search, gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            tatched = false;
            if (CharacterManager.Instance) {
                CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
            }
        }
    }

}
