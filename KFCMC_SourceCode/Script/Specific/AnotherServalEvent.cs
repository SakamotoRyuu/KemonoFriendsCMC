using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotherServalEvent : MonoBehaviour {

    public Transform cameraPivot1;
    public Vector3 adjustPosition;
    public Vector3 adjustRotation;
    public Transform playerPivot;
    public GameObject sandstarObj;
    public Light amuletLight;
    public Renderer emissionRend;
    public float[] maxBrights;
    public Transform cameraPivot2;
    public Transform[] friendsPivot;
    public Transform friendsLookAt;
    public GetDocumentSpecialImage referenceDocument;
    public int bgmNumber;
    public int documentID;
    public int itemID;
    public Vector3 overridePlayerPosition;
    public Vector3 overridePlayerRotation;
    public GameObject campoFlickerSpecialTalk;

    private float elapsedTime;
    private int progress;
    private Color emissionColor = Color.black;
    private Material[] emissionMaterials;
    private bool lightCompleted;
    int friendsCount;
    int answer;
    int saveSlot = -1;

    const int newProgress = 11;
    const int lodgeFriends = 23;

    private void Awake() {
        GameManager.Instance.save.SetClearStage(newProgress);
        GameManager.Instance.save.document[documentID] = 1;
        GameManager.Instance.save.AddItem(itemID);
        CharacterManager.Instance.SetPlayerUpdateEnabled(false);
        CharacterManager.Instance.ForceStopForEventAll(30);
        PauseController.Instance.OffPauseExternal(true);
        PauseController.Instance.pauseEnabled = false;
        emissionMaterials = emissionRend.materials;
        sandstarObj.SetActive(false);
    }

    void NextProgress() {
        progress++;
        elapsedTime = 0f;
    }

    void Update() {
        switch (progress) {
            case 0:
                if (elapsedTime >= 2.5f) {
                    CameraManager.Instance.SetEventCamera(cameraPivot1.position, cameraPivot1.eulerAngles, 10f, 1f, 1f);
                    NextProgress();
                }
                break;
            case 1:
                if (elapsedTime >= 3f) {
                    NextProgress();
                }
                break;
            case 2:
                PauseController.Instance.SetBlackCurtain(elapsedTime / 2f);
                if (elapsedTime >= 3f) {
                    if (PauseController.Instance.IsPhotoPausing) {
                        PauseController.Instance.PausePhoto(false);
                    }
                    StageManager.Instance.CleaningObjects();
                    transform.SetPositionAndRotation(adjustPosition, Quaternion.Euler(adjustRotation));
                    // CharacterManager.Instance.FriendsClearAll();
                    CharacterManager.Instance.ResetWR(false);
                    CharacterManager.Instance.ClearBuff();
                    CharacterManager.Instance.pCon.InitializeOnEnable();
                    CharacterManager.Instance.playerTrans.SetPositionAndRotation(playerPivot.position, playerPivot.rotation);
                    // CharacterManager.Instance.pCon.ForceStopForEvent(2f);
                    friendsCount = CharacterManager.Instance.GetFriendsCount(false);
                    if (friendsCount > 0) {
                        float randRadius = 0f;
                        Vector3 friendsPos = friendsPivot[0].position;
                        Vector2 randCircle = Vector3.zero;
                        int nowCount = 0;
                        if (friendsCount >= 3) {
                            randRadius = Mathf.Sqrt(friendsCount) * 0.5f;
                        }
                        for (int i = 1; i < GameManager.friendsMax; i++) {
                            if (CharacterManager.Instance.GetFriendsExist(i, false)) {
                                // CharacterManager.Instance.friends[i].fBase.ForceStopForEvent(10f);
                                randCircle = Random.insideUnitCircle * randRadius;
                                friendsPos = friendsPivot[nowCount % friendsPivot.Length].position;
                                friendsPos.x += randCircle.x;
                                friendsPos.z += randCircle.y;
                                CharacterManager.Instance.friends[i].fBase.Warp(friendsPos, 0f, 0f);
                                CharacterManager.Instance.friends[i].trans.SetPositionAndRotation(friendsPos, Quaternion.LookRotation(friendsLookAt.position - friendsPos));
                                nowCount++;
                            }
                        }
                    }
                    BGM.Instance.Play(bgmNumber);
                    referenceDocument.SetDocumentExternal();
                    NextProgress();
                }
                break;
            case 3:
                if (!PauseController.Instance.pauseGame) {
                    CameraManager.Instance.SetEventCamera(cameraPivot2.position, cameraPivot2.eulerAngles, 10f, 0f, 1.75f);
                    BGM.Instance.StopFade(2f);
                    CharacterManager.Instance.ForceCanvasEnabled(0, 0);
                    CharacterManager.Instance.SetPlayerUpdateEnabled(false);
                    for (int i = 0; i < emissionMaterials.Length; i++) {
                        emissionColor = Color.black;
                        emissionMaterials[i].SetColor("_EmissionColor", emissionColor);
                    }
                    amuletLight.intensity = 0f;
                    sandstarObj.SetActive(true);
                    NextProgress();
                }
                break;
            case 4:
                PauseController.Instance.SetBlackCurtain((2f - elapsedTime) / 2f);
                if (elapsedTime >= 2f) {
                    PauseController.Instance.SetBlackCurtain(0f);
                    NextProgress();
                }
                break;
            case 5:
                if (elapsedTime <= 2f) {
                    for (int i = 0; i < emissionMaterials.Length; i++) {
                        emissionColor.r = emissionColor.g = emissionColor.b = Mathf.Clamp01(elapsedTime / 2f) * maxBrights[i];
                        emissionMaterials[i].SetColor("_EmissionColor", emissionColor);
                    }
                    amuletLight.intensity = Mathf.Clamp01(elapsedTime / 2f) * 1.8f;
                } else if (!lightCompleted) {
                    lightCompleted = true;
                    for (int i = 0; i < emissionMaterials.Length; i++) {
                        emissionColor.r = emissionColor.g = emissionColor.b = maxBrights[i];
                        emissionMaterials[i].SetColor("_EmissionColor", emissionColor);
                    }
                    amuletLight.intensity = 1.8f;
                }
                if (elapsedTime >= 5f) {
                    CameraManager.Instance.SetEventTimer(0.01f);
                    PauseController.Instance.SetBlackCurtain(1f);
                    sandstarObj.SetActive(false);
                    NextProgress();
                }
                break;
            case 6:
                if (elapsedTime >= 0.5f) {
                    if (PauseController.Instance.IsPhotoPausing) {
                        PauseController.Instance.PausePhoto(false);
                    }
                    CharacterManager.Instance.ReleaseStopForEventAll();
                    SaveController.Instance.permitEmptySlot = true;
                    SaveController.Instance.Activate();
                    NextProgress();
                }
                break;
            case 7:
                saveSlot = -1;
                answer = SaveController.Instance.SaveControlExternal(true, false);
                if (answer >= 0 && answer < GameManager.saveSlotMax) {
                    saveSlot = answer;
                }
                if (answer != -1) {
                    if (!SceneChange.Instance.GetIsProcessing) {
                        SceneChange.Instance.StartEyeCatch(false);
                    }
                    NextProgress();
                }
                break;
            case 8:
                if (SceneChange.Instance.GetEyeCatch()) {
                    PauseController.Instance.SetBlackCurtain(0f);
                    CharacterManager.Instance.ForceCanvasEnabled(-1, 0);
                    CharacterManager.Instance.SetPlayerUpdateEnabled(true);
                    CharacterManager.Instance.ReleaseStopForEventAll();
                    StageManager.Instance.MoveStage(0, 0, saveSlot);
                    if (GameManager.Instance.save.config[GameManager.Save.configID_GenerateHomeObjects] != 0 && GameManager.Instance.save.friends[lodgeFriends] != 0) {
                        CharacterManager.Instance.playerTrans.position = overridePlayerPosition;
                        CharacterManager.Instance.playerTrans.eulerAngles = overridePlayerRotation;
                        CharacterManager.Instance.PlaceFriendsAroundPlayer();
                        CameraManager.Instance.ResetCameraFixPos();
                    }
                    SceneChange.Instance.EndEyeCatch();
                    MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_AMULET"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Important);
                    PauseController.Instance.pauseEnabled = true;
                    if (campoFlickerSpecialTalk) {
                        Instantiate(campoFlickerSpecialTalk);
                    }
                    elapsedTime = 0f;
                    progress = 100;
                    Destroy(gameObject);
                }
                break;
        }
        elapsedTime += Time.deltaTime;
    }
}
