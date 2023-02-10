using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleController : SingletonMonoBehaviour<TitleController> {
    
    [System.Serializable]
    public class Choices {
        public Canvas canvas;
        public GraphicRaycaster gRaycaster;
        public RectTransform cursorRect;
        public int cursorPos;
        public Vector2 origin;
        public Vector2 interval;
    }

    public Choices select;
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

    private int state = 0;
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
    private int eventEnterNum = -1;
    private int eventClickNum = -1;
    private float eventClickedTimeRemain;
    private Color curtainColor;
    static readonly Vector2Int vec2IntZero = Vector2Int.zero;
    
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
        // GameManager.Instance.InitLanguage();
        controlMapperWrapper.InitializeTexts();
        GameManager.Instance.GetModErrorText(modErrorText);
        select.cursorPos = GameManager.Instance.GetLastSaveSlot() >= 0 ? 1 : 0;
        select.cursorRect.anchoredPosition = select.origin + select.interval * select.cursorPos;
    }

    private void SetState(int newState) {
        state = newState;
        eventEnterNum = -1;
        eventClickNum = -1;
    }

    void CancelCommon(bool sound = true) {
        if (sound) {
            UISE.Instance.Play(UISE.SoundName.cancel);
        }
        SaveController.Instance.DeactivateCaution();
        SaveController.Instance.Deactivate();
        select.canvas.enabled = true;
        select.gRaycaster.enabled = true;
        SetState(0);
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
                SetState(4);
                break;
            case 1:
                UISE.Instance.Play(UISE.SoundName.submit);
                select.canvas.enabled = false;
                select.gRaycaster.enabled = false;
                SaveController.Instance.permitEmptySlot = false;
                SaveController.Instance.Activate();
                SetState(1);
                break;
            case 2:
                UISE.Instance.Play(UISE.SoundName.submit);
                select.canvas.enabled = false;
                select.gRaycaster.enabled = false;
                controlMapperWrapper.Open();
                SetState(5);
                break;
            case 3:
                UISE.Instance.Play(UISE.SoundName.close);
                select.canvas.enabled = false;
                select.gRaycaster.enabled = false;
                curtainColor = new Color(0f, 0f, 0f, 0f);
                blackCurtain.color = curtainColor;
                blackCurtain.enabled = true;
                SetState(2);
                break;
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
                case 0:
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
                case 1:
                    controlType = SaveController.Instance.controlType;
                    saveSlot = SaveController.Instance.SaveControl();
                    if (controlType == 0) {
                        if (saveSlot >= 0 && saveSlot < GameManager.saveSlotMax) {
                            UISE.Instance.Play(UISE.SoundName.use);
                            GameManager.Instance.DataLoad(saveSlot);
                            GameManager.Instance.LoadScene("Play");
                            GameManager.Instance.ApplyVolume();
                            SetState(6);
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
                                CancelCommon(false);
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
                            SetState(3);
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
                case 2:
                    quitTime += Time.deltaTime;
                    curtainColor.a = Mathf.Clamp01(quitTime);
                    blackCurtain.color = curtainColor;
                    if (quitTime > 1) {
                        SetState(6);
                        Application.Quit();
                    }
                    break;
                case 3:
                    if (GameManager.Instance.playerInput.GetButton(RewiredConsts.Action.Submit)) {
                        buttonDownTime += Time.deltaTime;
                        if (buttonDownTime >= 1f) {
                            GameManager.Instance.DataDelete(saveSlot);
                            UISE.Instance.Play(UISE.SoundName.delete);
                            CancelCommon(false);
                        }
                    } else {
                        buttonDownTime = 0;
                    }
                    SaveController.Instance.SetCautionFill(buttonDownTime);
                    if (GameManager.Instance.GetCancelButtonDown || GameManager.Instance.MoveCursor(true) != Vector2Int.zero) {
                        CancelCommon();
                    }
                    break;
                case 4:
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
                        SetState(6);
                    }
                    break;
                case 5:
                    if (!controlMapperWrapper.IsControlMapperActive) {
                        CancelCommon(false);
                        UISE.Instance.Play(UISE.SoundName.submit);
                    }
                    break;
                case 6:
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
