using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class Amusement_ToMogisenConsole : MonoBehaviour {

    public int mogisenRank;
    public string rankDicKey;
    public GameObject looksObj;
    public Image difficultyIcon;
    public Sprite[] difficultySprites;
    public TMP_Text rankText;
    public TMP_Text timeText;
    public GameObject mogisenEventPrefab;
    public int floorNum;
    public int conditionGameProgress;
    public bool conditionSkytreeCleared;
    public bool enableMultiplay;

    int state;
    int pauseWait;
    int diffSave;
    int langSave;
    StringBuilder sb = new StringBuilder();
    bool multiSave;
    bool actionTextEnabled;
    int saveIndex = -1;
    const string targetTag = "ItemGetter";


    private void Awake() {
        if (GameManager.Instance) {
            if (GameManager.Instance.save.zakoRushClearTime.Length < GameManager.zakoRushArrayMax) {
                GameManager.Instance.FixZakoRushTimeLength();
            }
            diffSave = Mathf.Clamp(GameManager.Instance.save.difficulty, 1, GameManager.difficultyMax);
            langSave = GameManager.Instance.save.language;
            SetRankText();
            CheckCondition();
        }
    }

    private void Start() {
        Instantiate(MapDatabase.Instance.prefab[MapDatabase.goal], looksObj.transform);
    }

    void SetRankText() {
        if (rankText) {
            rankText.text = sb.Clear().Append(TextManager.Get("AMUSEMENT_ZAKORUSH")).AppendLine().Append(TextManager.Get(rankDicKey)).ToString();
        }
    }

    void CheckCondition() {
        multiSave = CharacterManager.Instance.mogisenMultiPlay;
        if (looksObj) {
            bool found = false;
            if (mogisenRank < 1) {
                found = true;
            } else {
                if (GameManager.Instance.save.zakoRushClearTime.Length >= GameManager.zakoRushMax * GameManager.difficultyMax) {
                    for (int i = 0; i < GameManager.difficultyMax; i++) {
                        if (GameManager.Instance.save.zakoRushClearTime[(mogisenRank - 1) * GameManager.difficultyMax + i] > 0) {
                            found = true;
                            break;
                        }
                    }
                }
            }
            if (found && conditionGameProgress > 0 && GameManager.Instance.save.progress < conditionGameProgress) {
                found = false;
            }
            if (found && conditionSkytreeCleared && GameManager.Instance.GetSecret(GameManager.SecretType.SkytreeCleared) == false) {
                found = false;
            }
            if (looksObj.activeSelf != found) {
                looksObj.SetActive(found);
            }
            if (found) {
                if (difficultyIcon && diffSave - 1 < difficultySprites.Length) {
                    difficultyIcon.sprite = difficultySprites[diffSave - 1];
                }
                if (timeText) {
                    saveIndex = mogisenRank * GameManager.difficultyMax + diffSave - 1 + (multiSave ? GameManager.zakoRushArrayMax / 2 : 0);
                    int timeRaw = GameManager.Instance.save.zakoRushClearTime[saveIndex];
                    if (timeRaw > 0) {
                        int timeInteger = timeRaw / 100;
                        int timeDecimal = timeRaw % 100;
                        timeText.text = sb.Clear().Append(TextManager.Get("AMUSEMENT_RECORD")).AppendLine().Append((timeInteger / 60).ToString("00")).Append("\'").Append((timeInteger % 60).ToString("00")).Append("\"").Append(timeDecimal.ToString("00")).ToString();
                    } else {
                        timeText.text = "";
                    }
                }
            }
        }
    }

    private void Update() {
        if (CharacterManager.Instance) {
            if (diffSave != GameManager.Instance.save.difficulty || multiSave != CharacterManager.Instance.mogisenMultiPlay) {
                diffSave = Mathf.Clamp(GameManager.Instance.save.difficulty, 1, GameManager.difficultyMax);
                multiSave = CharacterManager.Instance.mogisenMultiPlay;
                CheckCondition();
            }
            if (langSave != GameManager.Instance.save.language) {
                langSave = GameManager.Instance.save.language;
                SetRankText();
                CheckCondition();
            }
            if (PauseController.Instance && PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
            if (pauseWait <= 0) {
                switch (state) {
                    case 0:
                        break;
                    case 1:
                        if (PauseController.Instance.returnToLibraryProcessing) {
                            state = 0;
                        }
                        if (pauseWait <= 0 && looksObj && looksObj.activeSelf && PauseController.Instance.GetPauseEnabled() && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.SetChoices(3, true, TextManager.Get("AMUSEMENT_ZAKORUSH") + " " + TextManager.Get(rankDicKey), "AMUSEMENT_TRY", "AMUSEMENT_DELETE", "CHOICE_CANCEL");
                            state = 2;
                        }
                        break;
                    case 2:
                        switch (PauseController.Instance.ChoicesControl()) {
                            case -2:
                                UISE.Instance.Play(UISE.SoundName.cancel);
                                PauseController.Instance.CancelChoices();
                                PauseController.Instance.HideCaution();
                                state = 1;
                                break;
                            case 0:
                                UISE.Instance.Play(UISE.SoundName.submit);
                                PauseController.Instance.CancelChoices(false);
                                PauseController.Instance.HideCaution();
                                SceneChange.Instance.StartEyeCatch();
                                state = 3;
                                break;
                            case 1:
                                UISE.Instance.Play(UISE.SoundName.submit);
                                PauseController.Instance.SetChoices(7, true, TextManager.Get("AMUSEMENT_DELETE"), "CHOICE_CANCEL", "CHOICE_CANCEL", "CHOICE_CANCEL", "CHOICE_EXECUTE", "CHOICE_CANCEL", "CHOICE_CANCEL", "CHOICE_CANCEL");
                                state = 4;
                                break;
                            case 2:
                                UISE.Instance.Play(UISE.SoundName.submit);
                                PauseController.Instance.CancelChoices();
                                PauseController.Instance.HideCaution();
                                state = 1;
                                break;
                        }
                        break;
                    case 3:
                        if (SceneChange.Instance.GetEyeCatch()) {
                            PauseController.Instance.CancelChoices();
                            if (StageManager.Instance && StageManager.Instance.dungeonMother) {
                                StageManager.Instance.dungeonMother.MoveFloor(floorNum, -1, false);
                                Instantiate(mogisenEventPrefab, StageManager.Instance.dungeonController.transform);
                            }
                            SceneChange.Instance.EndEyeCatch();
                        }
                        break;
                    case 4:
                        switch (PauseController.Instance.ChoicesControl()) {
                            case -2:
                                UISE.Instance.Play(UISE.SoundName.cancel);
                                PauseController.Instance.CancelChoices();
                                state = 1;
                                break;
                            case 3:
                                if (saveIndex >= 0 && saveIndex < GameManager.Instance.save.zakoRushClearTime.Length) {
                                    UISE.Instance.Play(UISE.SoundName.delete);
                                    GameManager.Instance.save.zakoRushClearTime[saveIndex] = 0;
                                    CheckCondition();
                                } else {
                                    UISE.Instance.Play(UISE.SoundName.error);
                                }
                                PauseController.Instance.CancelChoices();
                                state = 1;
                                break;
                            case 0:
                            case 1:
                            case 2:
                            case 4:
                            case 5:
                            case 6:
                                UISE.Instance.Play(UISE.SoundName.submit);
                                PauseController.Instance.CancelChoices();
                                state = 1;
                                break;
                        }
                        break;
                }
            }
            bool actionTemp = (state == 1 && pauseWait <= 0 && PauseController.Instance.pauseEnabled && looksObj && looksObj.activeSelf);
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            state = state == 0 ? 1 : state;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            state = state == 1 ? 0 : state;
        }
    }

}
