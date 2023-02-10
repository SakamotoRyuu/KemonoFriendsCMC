using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_SunrisePort : MonoBehaviour {

    public GameObject checkTarget;
    public GameObject blackCurtainPrefab;

    const int kabanID = 1;
    const int anotherServalID = 31;
    AudioSource audioSource;
    float elapsedTime;
    float conditionTime;
    int progress;
    int answer;
    int saveSlot = -1;
    GameObject blackCurtainInstance;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    void NextProgress() {
        progress++;
        elapsedTime = 0f;
    }

    void Update() {
        elapsedTime += Time.deltaTime;
        switch (progress) {
            case 0:
                if (checkTarget == null) {
                    PauseController.Instance.pauseEnabled = false;
                    NextProgress();
                    conditionTime = (CharacterManager.Instance.GetFriendsExist(kabanID, true) || CharacterManager.Instance.GetFriendsExist(anotherServalID, true)) ? 4f : 1f;
                }
                break;
            case 1:
                if (elapsedTime >= conditionTime && MessageUI.Instance && MessageUI.Instance.GetMessageCount(1) == 0) {
                    CharacterManager.Instance.ForceStopForEventAll(20);
                    CharacterManager.Instance.StopFriends();
                    CameraManager.Instance.SetQuake(Vector3.zero, 2, 8, 2f, 5f, 2f, 100f, 200f, true);
                    Ambient.Instance.StopFade(1f);
                    if (audioSource) {
                        audioSource.Play();
                    }
                    NextProgress();
                }
                break;
            case 2:
                if (elapsedTime >= 0.5f) {
                    CharacterManager.Instance.EveryFriendsFear();
                    if (CharacterManager.Instance.GetFriendsExist(1)) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_KABAN_12_8", 1, 3);
                    }
                    elapsedTime -= 0.5f;
                    progress++;
                }
                break;
            case 3:
                if (elapsedTime >= 2.5f && MessageUI.Instance && MessageUI.Instance.GetMessageCount(1) == 0) {
                    blackCurtainInstance = Instantiate(blackCurtainPrefab, PauseController.Instance.offPauseCanvas.transform);
                    blackCurtainInstance.transform.SetAsFirstSibling();
                    NextProgress();
                }
                break;
            case 4:
                if (elapsedTime >= 1f) {
                    CameraManager.Instance.CancelQuake();
                    if (audioSource) {
                        audioSource.Stop();
                    }
                    if (CharacterManager.Instance.GetFriendsExist(1)) {
                        CharacterManager.Instance.SetSpecialChat("EVENT_KABAN_12_9", 1, MessageUI.Instance.GetSpeechAppropriateTime(TextManager.Get("EVENT_KABAN_12_9").Length));
                    }
                    NextProgress();
                }
                break;
            case 5:
                if (MessageUI.Instance && MessageUI.Instance.GetMessageCount(1) == 0) {
                    PauseController.Instance.SetBlackCurtain(1f);
                    if (blackCurtainInstance) {
                        Destroy(blackCurtainInstance);
                    }
                    NextProgress();
                }
                break;
            case 6:
                if (elapsedTime >= 0.5f) {
                    CharacterManager.Instance.TimeStop(true);
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
                    StageManager.Instance.MoveStage(0, 0, saveSlot);
                    SceneChange.Instance.EndEyeCatch();
                    PauseController.Instance.pauseEnabled = true;
                    NextProgress();
                    Destroy(gameObject);
                }
                break;
            case 9:
                break;
        }
    }
}
