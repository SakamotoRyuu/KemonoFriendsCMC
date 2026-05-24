using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_AnotherSavanna : MonoBehaviour
{

    public Transform playerPivot;
    public Transform cameraPivot1;
    public Transform cameraPivot2;
    public string dicKey1;
    public string dicKey2;
    public string faceName1;
    public string faceName2;
    public GameObject amuletObj;
    public Transform amuletPivot1;
    public Transform amuletPivot2;
    public int condition;

    private int progress;
    private float elapsedTime;

    private void NextProgress() {
        progress++;
        elapsedTime = 0f;
    }

    private void Update() {
        if (progress > 0) {
            elapsedTime += Time.deltaTime;
        }
        switch (progress) {
            case 0:
                break;
            case 1:
                if (elapsedTime > 1f) {
                    CameraManager.Instance.SetEventCamera(cameraPivot1.position, cameraPivot1.eulerAngles, 30, 0f, 0.5f);
                    NextProgress();
                }
                break;
            case 2:
                PauseController.Instance.SetBlackCurtain(Mathf.Clamp01(1f - elapsedTime * 0.5f), true);
                if (elapsedTime > 2f) {
                    CharacterManager.Instance.SetSpecialChat(dicKey1, -1, 7f);
                    CharacterManager.Instance.pCon.SetFaceString(faceName2, 7f);
                    NextProgress();
                }
                break;
            case 3:
                amuletObj.transform.position = Vector3.Lerp(amuletPivot1.position, amuletPivot2.position, Easing.SineIn(elapsedTime, 2f, 0f, 1f));
                if (elapsedTime > 3.5f) {
                    amuletObj.SetActive(false);
                    CameraManager.Instance.SetEventCamera(cameraPivot2.position, cameraPivot2.eulerAngles, 5f, 1.5f, 1000f);
                    NextProgress();
                }
                break;
            case 4:
                if (elapsedTime > 4.5f) {
                    CharacterManager.Instance.SetSpecialChat(dicKey2, -1, -1);
                    NextProgress();
                }
                break;
            case 5:
                if (elapsedTime > 2f) {
                    CameraManager.Instance.SetEventTimer(0.005f);
                    PauseController.Instance.pauseEnabled = true;
                    CharacterManager.Instance.pCon.SetAnotherAnimatorController(false, null);
                    CharacterManager.Instance.ReleaseStopForEventAll();
                    GameManager.Instance.save.progress = condition;
                    GameManager.Instance.save.config[GameManager.Save.configID_RestingMotion] = 0;
                    if (CanvasCulling.Instance) {
                        CanvasCulling.Instance.CheckConfig();
                    }
                    if (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.goalSettings.goalInstance) {
                        StageManager.Instance.dungeonController.goalSettings.goalInstance.SetActive(true);
                    }
                    progress = 0;
                }
                break;
        }
    }

    private void Start() {
        if (GameManager.Instance && CharacterManager.Instance && CameraManager.Instance && GameManager.Instance.IsPlayerAnother && GameManager.Instance.save.progress < condition) {
            PauseController.Instance.pauseEnabled = false;
            PauseController.Instance.SetBlackCurtain(1f, true);
            CharacterManager.Instance.ForceStopForEventAll(15);
            CharacterManager.Instance.playerTrans.SetPositionAndRotation(playerPivot.position, playerPivot.rotation);
            CharacterManager.Instance.pCon.SetFaceString(faceName1, 30);
            CameraManager.Instance.SetEventCamera(cameraPivot1.position, cameraPivot1.eulerAngles, 30, 0f, 0.5f);
            amuletObj.transform.SetPositionAndRotation(amuletPivot1.position, amuletPivot2.rotation);
            amuletObj.SetActive(true);
            if (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.goalSettings.goalInstance) {
                StageManager.Instance.dungeonController.goalSettings.goalInstance.SetActive(false);
            }
            NextProgress();
        }
    }

}
