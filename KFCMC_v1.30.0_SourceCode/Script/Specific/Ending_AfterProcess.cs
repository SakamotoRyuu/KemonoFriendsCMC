using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ending_AfterProcess : MonoBehaviour
{

    public Canvas endmarkCanvas;
    public TMP_Text endmarkText;
    public bool isAnother;
    public bool isLonesome;
    public Canvas messageCanvas;

    private int progress;
    private float elapsedTime;
    private int answer;
    private int saveSlot = -1;
    private bool achievementChecked;

    private const int lastStageProgress = 13;

    private void Start() {
        if (endmarkText && TextManager.IsInitialized) {
            endmarkText.text = TextManager.Get("CREDIT_END");
        }
        if (endmarkCanvas) {
            endmarkCanvas.enabled = true;
        }
        if (GameManager.Instance) {
            isAnother = GameManager.Instance.IsPlayerAnother;
        }
        if (messageCanvas && TrophyManager.Instance && MessageUI.Instance && GameManager.Instance.minmiGolden && !TrophyManager.Instance.IsTrophyHad(TrophyManager.t_GoldenMinmiED)) {
            messageCanvas.enabled = true;
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_GoldenMinmiED, true);
        }
    }

    private void NextProgress() {
        progress++;
        elapsedTime = 0f;
    }

    private void Update() {
        if (GameManager.Instance) {
            elapsedTime += Time.deltaTime;
            switch (progress) {
                case 0:
                    if (elapsedTime >= 1f && !achievementChecked) {
                        achievementChecked = true;
                        if (isLonesome) {
                            GameManager.Instance.SetSteamAchievement("LONESOMESERVAL");
                        } else {
                            GameManager.Instance.SetSteamAchievement("GAMECLEAR");
                        }
                    }
                    if (elapsedTime >= 5f) {
                        NextProgress();
                    }
                    break;
                case 1:
                    if (GameManager.Instance && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                        if (endmarkCanvas) {
                            endmarkCanvas.enabled = false;
                        }
                        if (messageCanvas && messageCanvas.enabled) {
                            messageCanvas.enabled = false;
                        }
                        NextProgress();
                    }
                    break;
                case 2:
                    if (elapsedTime >= 1f) {
                        if (SaveController.Instance) {
                            if (PauseController.Instance && PauseController.Instance.IsPhotoPausing) {
                                PauseController.Instance.PausePhoto(false);
                            }
                            GameManager.Instance.save.nowStage = 0;
                            GameManager.Instance.save.nowFloor = isAnother ? StageManager.anotherHomeFloor : StageManager.amusementParkFloorNumber;
                            GameManager.Instance.save.progress = lastStageProgress;
                            GameManager.Instance.save.sandstar = 0f;
                            for (int i = 0; i < GameManager.Instance.save.friendsLiving.Length; i++) {
                                GameManager.Instance.save.friendsLiving[i] = 0;
                            }
                            SaveController.Instance.permitEmptySlot = true;
                            SaveController.Instance.Activate();
                            NextProgress();
                        }
                    }
                    break;
                case 3:
                    if (GameManager.Instance && SaveController.Instance) {
                        saveSlot = -1;
                        answer = SaveController.Instance.SaveControlExternal(false, false);
                        if (answer >= 0 && answer < GameManager.saveSlotMax) {
                            saveSlot = answer;
                            GameManager.Instance.DataSave(saveSlot);
                        }
                        if (answer != -1) {
                            NextProgress();
                        }
                    }
                    break;
                case 4:
                    if (elapsedTime >= 1f) {
                        if (GameManager.Instance) {
                            GameManager.Instance.LoadScene("Title");
                            GameManager.Instance.ChangeTimeScale(false, false);
                        }
                        NextProgress();
                    }
                    break;
                case 5:
                    break;
            }
        }
    }

}
