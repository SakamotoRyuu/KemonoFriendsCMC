using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretZoneEnter : MonoBehaviour
{

    public int conditionCount;
    public OnTriggerEmission emitter;
    public int stageId;
    public int floorNum;
    int submitCount;
    int state;
    float elapsedTime;

    void Update()
    {
        if (GameManager.Instance && PauseController.Instance && SceneChange.Instance) {
            switch (state) {
                case 0:
                    if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) && !PauseController.Instance.returnToLibraryProcessing) {
                        submitCount++;
                        if (submitCount >= conditionCount) {
                            state++;
                            elapsedTime = 0f;
                            if (emitter) {
                                emitter.enabled = true;
                                emitter.gameObject.SetActive(true);
                                emitter.ForceTriggerStay();
                            }
                            CharacterManager.Instance.ForceStopForEventAll(5f);
                        }
                    }
                    break;
                case 1:
                    if (!PauseController.Instance.returnToLibraryProcessing) {
                        if (emitter) {
                            emitter.ForceTriggerStay();
                        }
                        elapsedTime += Time.deltaTime;
                        if (elapsedTime >= 2f) {
                            PauseController.Instance.CancelChoices(false);
                            PauseController.Instance.HideCaution();
                            SceneChange.Instance.StartEyeCatch(false);
                            state++;
                            elapsedTime = 0f;
                        }
                    }
                    break;
                case 2:
                    if (SceneChange.Instance.GetEyeCatch()) {
                        PauseController.Instance.CancelChoices();
                        Move();
                        SceneChange.Instance.EndEyeCatch();
                        state++;
                        elapsedTime = 0f;
                    }
                    break;
                case 3:
                    break;
            }
        }
    }

    private void Move() {
        if (StageManager.Instance) {
            GameManager.Instance.save.moveByBus = 0;
            StageManager.Instance.MoveStage(stageId, floorNum < 0 ? 0 : floorNum);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (state == 0 && other.CompareTag("ItemGetter") && GameManager.Instance && GameManager.Instance.GetPerfectCompleted()) {
            enabled = true;
            submitCount = 0;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (state == 0 && other.CompareTag("ItemGetter")) {
            enabled = false;
            submitCount = 0;
        }
    }
}
