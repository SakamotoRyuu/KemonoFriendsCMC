using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_TimeTravel : MonoBehaviour
{

    public Transform playerPivot;
    public Transform cameraPivot1;
    public Transform cameraPivot2;
    public Transform cameraPivot3;
    public string faceName1;
    public string faceName2;
    public string dicKey1;
    public string dicKey2;
    public string dicKey3;
    public RuntimeAnimatorController eventAnimCon;
    public GameObject amuletObj;
    public Light amuletLight;
    public Transform amuletPivot1;
    public Transform amuletPivot2;
    public GameObject amuletEffect1;
    public GameObject amuletEffect2;
    public int destinationStage;
    public int destinationFloor;

    private int progress;
    private float elapsedTime;

    private void NextProgress() {
        progress++;
        elapsedTime = 0f;
    }

    private void Start() {
        if (amuletObj) {
            amuletObj.SetActive(false);
        }
    }

    private void Update() {
        if (progress > 0) {
            elapsedTime += Time.deltaTime;
        }
        switch (progress) {
            case 0:
                break;
            case 1:
                if (elapsedTime > 6f && MessageUI.Instance.GetMessageCount(1) == 0) {
                    CharacterManager.Instance.SetSpecialChat(dicKey2, -1, -1);
                    CameraManager.Instance.SetEventCamera(cameraPivot2.position, cameraPivot2.eulerAngles, 30, 0f, 2000f);
                    NextProgress();
                }
                break;
            case 2:
                if (elapsedTime > 6f && MessageUI.Instance.GetMessageCount(1) == 0) {
                    CharacterManager.Instance.SetSpecialChat(dicKey3, -1, 5f);
                    CameraManager.Instance.SetEventCamera(cameraPivot3.position, cameraPivot3.eulerAngles, 30, 0f, 0.5f);
                    CharacterManager.Instance.pCon.SetFaceString(faceName2, 30);
                    NextProgress();
                }
                break;
            case 3:
                if (elapsedTime > 2.5f) {
                    amuletLight.intensity = 0f;
                    amuletObj.transform.SetPositionAndRotation(amuletPivot1.position, amuletPivot1.rotation);
                    amuletObj.SetActive(true);
                    NextProgress();
                }
                break;
            case 4:
                amuletObj.transform.position = Vector3.Lerp(amuletPivot1.position, amuletPivot2.position, Easing.SineInOut(elapsedTime, 2.5f, 0f, 1f));
                if (elapsedTime > 2.5f) {
                    Instantiate(amuletEffect1, amuletObj.transform.position, amuletObj.transform.rotation);
                    NextProgress();
                }
                break;
            case 5:
                amuletLight.intensity = Mathf.Clamp(elapsedTime * 4f, 0f, 2.25f);
                if (elapsedTime > 2f) {
                    Instantiate(amuletEffect2, amuletObj.transform.position, amuletObj.transform.rotation, StageManager.Instance.transform);
                    NextProgress();
                }
                break;
            case 6:
                PauseController.Instance.SetBlackCurtain(Mathf.Clamp01(elapsedTime * 5f), true);
                if (elapsedTime > 2f) {
                    GameManager.Instance.save.progress = 0;
                    StageManager.Instance.MoveStage(destinationStage, destinationFloor);
                    NextProgress();
                }
                break;
            case 7:
                break;
            case 11:
                if (SceneChange.Instance.GetEyeCatch()) {
                    PauseController.Instance.CancelChoices();
                    StageManager.Instance.MoveStage(destinationStage, destinationFloor);
                    SceneChange.Instance.EndEyeCatch();
                    progress++;
                }
                break;
            case 12:
                break;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (progress == 0 && CharacterManager.Instance && CameraManager.Instance && StageManager.Instance && PauseController.Instance && MessageUI.Instance && CharacterManager.Instance.playerTrans && other.CompareTag("ItemGetter")) {
            if (GameManager.Instance.IsPlayerAnother) {
                PauseController.Instance.pauseEnabled = false;
                CharacterManager.Instance.ClearBuff();
                CharacterManager.Instance.ClearSickAll();
                CharacterManager.Instance.ResetST();
                CharacterManager.Instance.ForceStopForEventAll(30);
                CharacterManager.Instance.playerTrans.SetPositionAndRotation(playerPivot.position, playerPivot.rotation);
                CharacterManager.Instance.SetSpecialChat(dicKey1, -1, -1);
                CharacterManager.Instance.pCon.SetAnotherAnimatorController(true, eventAnimCon);
                CharacterManager.Instance.pCon.SetFaceString(faceName1, 30);
                CameraManager.Instance.SetEventCamera(cameraPivot1.position, cameraPivot1.eulerAngles, 30, 0f, 3f);
                if (CanvasCulling.Instance) {
                    CanvasCulling.Instance.CheckConfig(CanvasCulling.indexGauge, 0);
                }
                NextProgress();
            } else {
                /*
                PauseController.Instance.CancelChoices(false);
                PauseController.Instance.HideCaution();
                SceneChange.Instance.StartEyeCatch(false);
                progress = 11;
                */
            }
        }
    }

}
