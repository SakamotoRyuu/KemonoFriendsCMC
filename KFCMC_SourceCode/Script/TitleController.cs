using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text;

public class TitleController : SingletonMonoBehaviour<TitleController> {
    

    public PauseController.Choices select; 
    public PauseController.Choices settings;
    public PauseController.Choices choices;
    public PauseController.Volume volume;
    public Image blackCurtain;
    public TMP_Text modErrorText;
    public AudioClip minmiClip;
    public Transform[] minmiPivot;
    public GameObject[] minmiPrefab;
    public GameObject[] normalCharacter;
    public GameObject[] anotherCharacter;
    public GameObject[] completeCharacter;
    public int normalLightingNumber;
    public int anotherLightingNumber;
    public int completeLightingNumber;
    public int normalMusicNumber;
    public int anotherMusicNumber;
    public ControlMapperWrapper controlMapperWrapper;
    public SetTitleChoiceImages setTitleChoiceImages;
    public int[] settingsItemID;
    public TMP_Text[] settingsInformationTexts;

    private State state;
    private float quitTime = 0;
    private int controlType = 0;
    private int saveSlot = 0;
    private int difficulty = 2;
    private float buttonDownTime = 0;
    private bool firstUpdated = false;
    private int leftMoveCount;
    private int rightMoveCount;
    private bool minmiCheckedFlag;
    private int minmiNum = 0;
    private bool isAnotherMode;
    private const int selectMax = 4;
    private const int settingsMax = 3;
    private int eventEnterNum = -1;
    private int eventClickNum = -1;
    private float eventClickedTimeRemain;
    private Color curtainColor;
    private StringBuilder sb = new StringBuilder();
    private const int eventChoiceBase = 900;
    int[] volumeAmount = new int[3];
    const int volumeMin = 0;
    const int volumeMax = 100;


    static readonly Vector2Int vec2IntZero = Vector2Int.zero;

    enum State
    {
        Title, Load, Quit, Delete, Difficulty, Settings = 10, Language, KeyBinds, Volume, End = 99
    }
    
    private void Start() {
        GameManager.Instance.state = GameManager.State.title;
        select.canvas.enabled = true;
        select.gRaycaster.enabled = true;
        SaveController.Instance.Deactivate();
        blackCurtain.enabled = false;
        GameManager.Instance.ChangeTimeScale(false, false);
        if (FogDatabase.Instance) {
            FogDatabase.Instance.SetDefault();
        }
        SetCharactersActivate(false);
        controlMapperWrapper.InitializeTexts();
        GameManager.Instance.GetModErrorText(modErrorText);
        select.cursorPos = GameManager.Instance.GetLastSaveSlot() >= 0 ? 1 : 0;
        select.cursorRect.anchoredPosition = select.origin + select.interval * select.cursorPos;
    }

    private void SetState(State newState) {
        state = newState;
        eventEnterNum = -1;
        eventClickNum = -1;
    }

    void CancelCommon(State newState = State.Title, bool sound = true) {
        if (sound) {
            UISE.Instance.Play(UISE.SoundName.cancel);
        }
        SaveController.Instance.DeactivateCaution();
        SaveController.Instance.Deactivate();
        switch (newState) {
            case State.Title:
                select.canvas.enabled = true;
                select.gRaycaster.enabled = true;
                settings.canvas.enabled = false;
                settings.gRaycaster.enabled = false;
                break;
            case State.Settings:
                settings.canvas.enabled = true;
                settings.gRaycaster.enabled = true;
                break;
        }
        SetState(newState);
    }

    void SetCharactersActivate(bool isAnother) {
        bool isNormal = !GameManager.Instance.afterClearFlag && !isAnother;
        bool isComplete = GameManager.Instance.afterClearFlag && !isAnother;
        for (int i = 0; i < normalCharacter.Length; i++) {
            if (normalCharacter[i].activeSelf != isNormal) {
                normalCharacter[i].SetActive(isNormal);
            }
        }
        for (int i = 0; i < anotherCharacter.Length; i++) {
            if (anotherCharacter[i].activeSelf != isAnother) {
                anotherCharacter[i].SetActive(isAnother);
            }
        }
        for (int i = 0; i < completeCharacter.Length; i++) {
            if (completeCharacter[i].activeSelf != isComplete) {
                completeCharacter[i].SetActive(isComplete);
            }
        }
        LightingDatabase.Instance.nowLightingNumber = -1;
        if (isAnother) {
            LightingDatabase.Instance.SetLighting(anotherLightingNumber);
        } else if (isComplete) {
            LightingDatabase.Instance.SetLighting(completeLightingNumber);
        } else {
            LightingDatabase.Instance.SetLighting(normalLightingNumber);
        }
        LightingDatabase.Instance.nowLightingNumber = -1;
        if (isAnother) {
            BGM.Instance.Play(anotherMusicNumber, 0f);
        } else {
            BGM.Instance.Play(normalMusicNumber, 0f);
        }
    }

    void SetSettingsInformationText() {
        for (int i = 0; i < settingsItemID.Length; i++) {
            if (settingsItemID[i] == 402) {
                settingsInformationTexts[i].text = sb.Clear().Append(ItemDatabase.Instance.GetItemInfomation(settingsItemID[i])).AppendLine().Append(TextManager.Get("CONFIG_CURRENT")).Append(" ").Append(TextManager.Get("LANGUAGE_" + GameManager.Instance.save.language.ToString())).ToString();
            } else {
                settingsInformationTexts[i].text = ItemDatabase.Instance.GetItemInfomation(settingsItemID[i]);
            }
        }
    }

    void SubmitTitleSelect() {
        switch (select.cursorPos) {
            case 0:
                UISE.Instance.Play(UISE.SoundName.submit);
                select.canvas.enabled = false;
                select.gRaycaster.enabled = false;
                DifficultyController.Instance.SetTexts(2);
                DifficultyController.Instance.Activate(true);
                leftMoveCount = 0;
                rightMoveCount = 0;
                SetState(State.Difficulty);
                break;
            case 1:
                UISE.Instance.Play(UISE.SoundName.submit);
                select.canvas.enabled = false;
                select.gRaycaster.enabled = false;
                SaveController.Instance.permitEmptySlot = false;
                SaveController.Instance.Activate();
                SetState(State.Load);
                break;
            case 2:
                UISE.Instance.Play(UISE.SoundName.submit);
                SetSettingsInformationText();
                select.canvas.enabled = false;
                select.gRaycaster.enabled = false;
                settings.canvas.enabled = true;
                settings.gRaycaster.enabled = true;
                SetState(State.Settings);
                break;
            case 3:
                UISE.Instance.Play(UISE.SoundName.close);
                select.canvas.enabled = false;
                select.gRaycaster.enabled = false;
                curtainColor = new Color(0f, 0f, 0f, 0f);
                blackCurtain.color = curtainColor;
                blackCurtain.enabled = true;
                SetState(State.Quit);
                break;
        }
    }

    void SubmitSettings() {
        switch (settings.cursorPos) {
            case 0:
                UISE.Instance.Play(UISE.SoundName.submit);
                SetChoices(GameManager.languageMax + 1, false, ItemDatabase.Instance.GetItemName(settingsItemID[0]), "LANGUAGE_0", "LANGUAGE_1", "LANGUAGE_2", "LANGUAGE_3", "CHOICE_CANCEL");
                settings.gRaycaster.enabled = false;
                SetState(State.Language);
                break;
            case 1:
                UISE.Instance.Play(UISE.SoundName.submit);
                settings.canvas.enabled = false;
                settings.gRaycaster.enabled = false;
                controlMapperWrapper.Open();
                SetState(State.KeyBinds);
                break;
            case 2:
                UISE.Instance.Play(UISE.SoundName.submit);
                settings.gRaycaster.enabled = false;
                volume.cursorPos = 0;
                volume.cursorRect.anchoredPosition = volume.origin + volume.interval * volume.cursorPos;
                volume.choice[0].text = TextManager.Get("CONFIG_VOL_MUSIC");
                volume.choice[1].text = TextManager.Get("CONFIG_VOL_SOUND");
                volume.choice[2].text = TextManager.Get("CONFIG_VOL_AMBIENT");
                UpdateVolume(true);
                SetVolumeCanvasEnabled(true);
                SetState(State.Volume);
                break;
        }
    }

    // Copy from PauseController
    public void SetNumOfChoices(int num) {
        RectTransform canvasRect = choices.canvas.GetComponent<RectTransform>();
        if (canvasRect) {
            if (num <= 3) {
                canvasRect.anchoredPosition = new Vector2(0f, 0f);
            } else {
                canvasRect.anchoredPosition = new Vector2(0f, 15f * (num - 3));
            }
        }
        choices.panel.sizeDelta = new Vector2(choices.panel.sizeDelta.x, 140f + 60f * num);
        choices.panel.anchoredPosition = new Vector2(0f, 75f - 25f * num);
        for (int i = num; i < choices.choice.Length; i++) {
            choices.choice[i].text = string.Empty;
        }
        choices.max = num;
        choices.cursorPos = 0;
        choices.cursorRect.anchoredPosition = choices.origin;
    }
    void SetChoicesCanvasEnabled(bool flag) {
        if (choices.canvas && choices.canvas.enabled != flag) {
            choices.canvas.enabled = flag;
            for (int i = 0; i < choices.gridLayoutGroups.Length; i++) {
                choices.gridLayoutGroups[i].enabled = flag;
            }
            choices.gRaycaster.enabled = flag;
        }
    }

    public void SetChoices(int numOfChoices = 3, bool pause = true, string title = "", string choice1 = "", string choice2 = "", string choice3 = "", string choice4 = "", string choice5 = "", string choice6 = "", string choice7 = "") {
        SetNumOfChoices(numOfChoices);
        choices.title.text = sb.Clear().Append(TextManager.Get("QUOTE_START")).Append(title).Append(TextManager.Get("QUOTE_END")).ToString();
        if (numOfChoices > 0 && !string.IsNullOrEmpty(choice1)) {
            choices.choice[0].text = TextManager.Get(choice1);
        }
        if (numOfChoices > 1 && !string.IsNullOrEmpty(choice2)) {
            choices.choice[1].text = TextManager.Get(choice2);
        }
        if (numOfChoices > 2 && !string.IsNullOrEmpty(choice3)) {
            choices.choice[2].text = TextManager.Get(choice3);
        }
        if (numOfChoices > 3 && !string.IsNullOrEmpty(choice4)) {
            choices.choice[3].text = TextManager.Get(choice4);
        }
        if (numOfChoices > 4 && !string.IsNullOrEmpty(choice5)) {
            choices.choice[4].text = TextManager.Get(choice5);
        }
        if (numOfChoices > 5 && !string.IsNullOrEmpty(choice6)) {
            choices.choice[5].text = TextManager.Get(choice6);
        }
        if (numOfChoices > 6 && !string.IsNullOrEmpty(choice7)) {
            choices.choice[6].text = TextManager.Get(choice7);
        }
        SetChoicesCanvasEnabled(true);
        if (pause) {
            if (CharacterManager.Instance != null) {
                CharacterManager.Instance.TimeStop(true);
            }
        }
    }

    public int ChoicesControl() {
        Vector2Int move = GameManager.Instance.MoveCursor(true);
        if (choices.max > 0 && (move.y != 0 || (eventEnterNum >= eventChoiceBase && eventEnterNum < eventChoiceBase + choices.max))) {
            choices.cursorPos = (eventEnterNum >= eventChoiceBase && eventEnterNum < eventChoiceBase + choices.max ? eventEnterNum - eventChoiceBase : (choices.cursorPos + choices.max + move.y) % choices.max);
            choices.cursorRect.anchoredPosition = choices.origin + choices.cursorPos * choices.interval;
            UISE.Instance.Play(UISE.SoundName.move);
            // HideCaution();
        }
        bool considerSubmit = false;
        if (eventClickNum >= eventChoiceBase && eventClickNum < eventChoiceBase + choices.max) {
            choices.cursorPos = eventClickNum - eventChoiceBase;
            choices.cursorRect.anchoredPosition = choices.origin + choices.cursorPos * choices.interval;
            considerSubmit = true;
        }
        if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || considerSubmit) {
            return choices.cursorPos;
        } else if (GameManager.Instance.GetCancelButtonDown) {
            return -2;
        } else {
            return -1;
        }
    }

    void SetVolumeText(int index, int param) {
        if (param > volumeMin) {
            sb.Clear();
            volume.amount[index].text = sb.Append(param).Append("%").ToString();
        } else {
            volume.amount[index].text = TextManager.Get("CONFIG_VOL_MUTE");
        }
    }

    void UpdateVolume(bool init = false) {
        volumeAmount[0] = GameManager.Instance.save.musicVolume;
        volumeAmount[1] = GameManager.Instance.save.seVolume;
        volumeAmount[2] = GameManager.Instance.save.ambientVolume;
        for (int i = 0; i < volumeAmount.Length && i < volume.slider.Length; i++) {
            if (init || volume.slider[i].value != volumeAmount[i]) {
                if (volume.slider[i].maxValue != 100) {
                    volume.slider[i].maxValue = 100;
                }
                volume.slider[i].value = volumeAmount[i];
                SetVolumeText(i, volumeAmount[i]);
            }
        }
    }

    public void VolumeSliderChanged(int type) {
        int newNum = Mathf.RoundToInt(volume.slider[type].value);
        bool changed = false;
        switch (type) {
            case 0:
                if (GameManager.Instance.save.musicVolume != newNum) {
                    GameManager.Instance.save.musicVolume = newNum;
                    changed = true;
                }
                break;
            case 1:
                if (GameManager.Instance.save.seVolume != newNum) {
                    GameManager.Instance.save.seVolume = newNum;
                    changed = true;
                }
                break;
            case 2:
                if (GameManager.Instance.save.ambientVolume != newNum) {
                    GameManager.Instance.save.ambientVolume = newNum;
                    changed = true;
                }
                break;
        }
        if (changed) {
            UISE.Instance.Play(UISE.SoundName.move);
            SetVolumeText(type, newNum);
            GameManager.Instance.ApplyVolume();
        }
    }

    void VolumeControl() {
        Vector2Int move = GameManager.Instance.MoveCursor(true, 0.0125f, 0.1f);
        float mouseScroll = Input.mouseScrollDelta.y;
        if (move.x != 0) {
            if (volume.cursorPos == 0) {
                GameManager.Instance.save.musicVolume += move.x;
                if (GameManager.Instance.save.musicVolume < volumeMin) {
                    GameManager.Instance.save.musicVolume = volumeMin;
                } else if (GameManager.Instance.save.musicVolume > volumeMax) {
                    GameManager.Instance.save.musicVolume = volumeMax;
                }
            } else if (volume.cursorPos == 1) {
                GameManager.Instance.save.seVolume += move.x;
                if (GameManager.Instance.save.seVolume < volumeMin) {
                    GameManager.Instance.save.seVolume = volumeMin;
                } else if (GameManager.Instance.save.seVolume > volumeMax) {
                    GameManager.Instance.save.seVolume = volumeMax;
                }
            } else if (volume.cursorPos == 2) {
                GameManager.Instance.save.ambientVolume += move.x;
                if (GameManager.Instance.save.ambientVolume < volumeMin) {
                    GameManager.Instance.save.ambientVolume = volumeMin;
                } else if (GameManager.Instance.save.ambientVolume > volumeMax) {
                    GameManager.Instance.save.ambientVolume = volumeMax;
                }
            }
            UpdateVolume();
            GameManager.Instance.ApplyVolume();
        }
        if (move.y == 0 && mouseScroll != 0) {
            move.y = (mouseScroll > 0 ? -1 : 1);
        }
        if (move.y != 0) {
            volume.cursorPos = (volume.cursorPos + move.y + volume.max) % volume.max;
            volume.cursorRect.anchoredPosition = volume.origin + volume.interval * volume.cursorPos;
        }
        if (move != Vector2Int.zero) {
            UISE.Instance.Play(UISE.SoundName.move);
        }
        if (GameManager.Instance.GetCancelButtonDown) {
            SetVolumeCanvasEnabled(false);
            GameManager.Instance.DataOverwriteVolume(GameManager.Instance.save.seVolume, GameManager.Instance.save.musicVolume, GameManager.Instance.save.ambientVolume);
            CancelCommon(State.Settings);
        }
    }
    void SetVolumeCanvasEnabled(bool flag) {
        if (volume.canvas && volume.canvas.enabled != flag) {
            volume.canvas.enabled = flag;
            volume.gRaycaster.enabled = flag;
        }
    }

    void Update () {
        if (GameManager.Instance.MouseCancelling) {
            eventEnterNum = -1;
            eventClickNum = -1;
        }
        if (firstUpdated) {
            Vector2Int move = vec2IntZero;
            switch (state) {
                case State.Title:
                    move = GameManager.Instance.MoveCursor(false);
                    if (move.y != 0 || eventEnterNum >= 0) {
                        UISE.Instance.Play(UISE.SoundName.move);
                        select.cursorPos = (eventEnterNum >= 0 ? eventEnterNum : (select.cursorPos + move.y + selectMax) % selectMax);
                        select.cursorRect.anchoredPosition = select.origin + select.interval * select.cursorPos;
                    }
                    if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || eventClickNum >= 0) {
                        if (eventClickNum >= 0) {
                            select.cursorPos = eventClickNum;
                            select.cursorRect.anchoredPosition = select.origin + select.interval * select.cursorPos;
                        }
                        SubmitTitleSelect();
                    }
                    break;
                case State.Load:
                    controlType = SaveController.Instance.controlType;
                    saveSlot = SaveController.Instance.SaveControl();
                    if (controlType == 0) {
                        if (saveSlot >= 0 && saveSlot < GameManager.saveSlotMax) {
                            UISE.Instance.Play(UISE.SoundName.use);
                            GameManager.Instance.DataLoad(saveSlot);
                            GameManager.Instance.LoadScene("Play");
                            GameManager.Instance.ApplyVolume();
                            SetState(State.End);
                        } else {
                            switch (saveSlot) {
                                case -2:
                                    UISE.Instance.Play(UISE.SoundName.error);
                                    break;
                                case -3:
                                    CancelCommon();
                                    break;
                            }
                        }
                    } else if (controlType == 1) {
                        if (saveSlot >= 0 && saveSlot < GameManager.saveSlotMax) {
                            if (GameManager.Instance.DataCopy(saveSlot)) {
                                UISE.Instance.Play(UISE.SoundName.bridge);
                                CancelCommon(State.Title, false);
                            } else {
                                UISE.Instance.Play(UISE.SoundName.error);
                            }
                        } else {
                            switch (saveSlot) {
                                case -2:
                                    UISE.Instance.Play(UISE.SoundName.error);
                                    break;
                                case -3:
                                    CancelCommon();
                                    break;
                            }
                        }
                    } else if (controlType == 2) {
                        if (saveSlot >= 0 && saveSlot < GameManager.saveSlotMax) {
                            SaveController.Instance.ActivateCaution();
                            buttonDownTime = 0;
                            SetState(State.Delete);
                        } else {
                            switch (saveSlot) {
                                case -2:
                                    UISE.Instance.Play(UISE.SoundName.error);
                                    break;
                                case -3:
                                    CancelCommon();
                                    break;
                            }
                        }
                    }
                    break;
                case State.Quit:
                    quitTime += Time.deltaTime;
                    curtainColor.a = Mathf.Clamp01(quitTime);
                    blackCurtain.color = curtainColor;
                    if (quitTime > 1) {
                        SetState(State.End);
                        Application.Quit();
                    }
                    break;
                case State.Delete:
                    if (GameManager.Instance.playerInput.GetButton(RewiredConsts.Action.Submit)) {
                        buttonDownTime += Time.deltaTime;
                        if (buttonDownTime >= 1f) {
                            GameManager.Instance.DataDelete(saveSlot);
                            UISE.Instance.Play(UISE.SoundName.delete);
                            CancelCommon(State.Title, false);
                        }
                    } else {
                        buttonDownTime = 0;
                    }
                    SaveController.Instance.SetCautionFill(buttonDownTime);
                    if (GameManager.Instance.GetCancelButtonDown || GameManager.Instance.MoveCursor(true) != Vector2Int.zero) {
                        CancelCommon();
                    }
                    break;
                case State.Difficulty:
                    move = GameManager.Instance.MoveCursor(false);
                    if (move.y != 0) {
                        leftMoveCount = 0;
                        rightMoveCount = 0;
                    }
                    if (move.x < 0) {
                        leftMoveCount++;
                        rightMoveCount = 0;
                        if (isAnotherMode) {
                            isAnotherMode = false;
                            SetCharactersActivate(false);
                        }
                        if (leftMoveCount >= 10 && !minmiCheckedFlag) {
                            minmiCheckedFlag = true;
                            minmiNum = 0;
                            for (int i = 0; i < GameManager.saveSlotMax; i++) {
                                GameManager.Save saveTemp = GameManager.Instance.TempLoad(i);
                                minmiNum |= saveTemp.minmi;
                            }
                            if (minmiNum != 0) {
                                if (UISE.Instance && minmiClip != null) {
                                    UISE.Instance.audioSource.PlayOneShot(minmiClip);
                                }
                                for (int i = 0; i < minmiPivot.Length; i++) {
                                    if ((minmiNum & (1 << i)) != 0) {
                                        Instantiate(minmiPrefab[i], minmiPivot[i].position, minmiPivot[i].rotation, minmiPivot[i]);
                                    }
                                }
                            }
                        }
                    } else if (move.x > 0) {
                        leftMoveCount = 0;
                        rightMoveCount++;
                        if (!isAnotherMode && rightMoveCount == 10 && (GameManager.Instance.GetClearFlag() & 1) != 0) {
                            isAnotherMode = true;
                            SetCharactersActivate(true);
                        }
                    }
                    difficulty = DifficultyController.Instance.DifficultyControl(move);                    
                    if (difficulty <= -2) {
                        DifficultyController.Instance.Activate(false);
                        CancelCommon();
                    } else if (difficulty >= 1) {
                        NewGameStart();
                        SetState(State.End);
                    }
                    break;
                case State.Settings:
                    move = GameManager.Instance.MoveCursor(false);
                    if (move.y != 0 || eventEnterNum >= 0) {
                        UISE.Instance.Play(UISE.SoundName.move);
                        settings.cursorPos = (eventEnterNum >= 0 ? eventEnterNum : (settings.cursorPos + move.y + settingsMax) % settingsMax);
                        settings.cursorRect.anchoredPosition = settings.origin + settings.interval * settings.cursorPos;
                    }
                    if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || eventClickNum >= 0) {
                        if (eventClickNum >= 0) {
                            settings.cursorPos = eventClickNum;
                            settings.cursorRect.anchoredPosition = settings.origin + settings.interval * settings.cursorPos;
                        }
                        SubmitSettings();
                    } else if (GameManager.Instance.GetCancelButtonDown) {
                        CancelCommon();
                    }
                    break;
                case State.Language:
                    int answer = ChoicesControl();
                    if (answer == GameManager.languageMax) {
                        UISE.Instance.Play(UISE.SoundName.submit);
                        SetChoicesCanvasEnabled(false);
                        CancelCommon(State.Settings, false);
                        break;
                    } else if (answer >= 0 && answer < GameManager.languageMax) {
                        UISE.Instance.Play(UISE.SoundName.use);
                        GameManager.Instance.save.language = answer;
                        GameManager.Instance.InitLanguage();
                        controlMapperWrapper.InitializeTexts();
                        SetSettingsInformationText();
                        setTitleChoiceImages.SetSprite();
                        GameManager.Instance.DataOverwriteLanguage(answer); 
                        SetChoicesCanvasEnabled(false);
                        CancelCommon(State.Settings, false);
                    } else if (answer == -2) {
                        UISE.Instance.Play(UISE.SoundName.cancel);
                        SetChoicesCanvasEnabled(false);
                        CancelCommon(State.Settings, false);
                    }
                    break;
                case State.KeyBinds:
                    if (!controlMapperWrapper.IsControlMapperActive) {
                        UISE.Instance.Play(UISE.SoundName.submit);
                        CancelCommon(State.Settings, false);
                    }
                    break;
                case State.Volume:
                    VolumeControl();
                    break;
                case State.End:
                    break;

            }
        } else {
            if (BGM.Instance) {
                BGM.Instance.Play(0, 0);
            }
            if (Ambient.Instance) {
                Ambient.Instance.Play(-1, 0);
            }
            firstUpdated = true;
        }
        eventEnterNum = -1;
        eventClickNum = -1;
    }

    void NewGameStart() {
        UISE.Instance.Play(UISE.SoundName.use);
        GameManager.Instance.currentSaveSlot = -1;
        GameManager.Instance.InitSave(isAnotherMode ? GameManager.playerType_Another : GameManager.playerType_Normal);
        GameManager.Instance.save.difficulty = difficulty;
        if (minmiNum != 0) {
            GameManager.Instance.save.minmi = minmiNum;
        }
        if (isAnotherMode) {
            GameManager.Instance.save.config[GameManager.Save.configID_RestingMotion] = 1;
            GameManager.Instance.save.config[GameManager.Save.configID_Gauge] = 0;
            SceneManager.LoadScene("OpeningAnother");
        } else {
            SceneManager.LoadScene("Opening");
            if (BGM.Instance) {
                BGM.Instance.Play(-1, 0f);
            }
        }
    }

    public void SE_Submit() {
        if (UISE.Instance) {
            UISE.Instance.Play(UISE.SoundName.submit);
        }
    }

    public void EventEnter(int param) {
        eventEnterNum = param;
    }

    public void EventClick(int param) {
        if (!Input.GetMouseButtonUp(1)) {
            eventClickNum = param;
        }
    }
    
}
