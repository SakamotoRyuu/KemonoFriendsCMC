using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class SaveController : SingletonMonoBehaviour<SaveController> {

    [System.Serializable]
    public class Choices {
        public Canvas canvas;
        public GridLayoutGroup gridLayoutGroup;
        public GraphicRaycaster gRaycaster;
        public RectTransform cursorRect;
        public int cursorPos;
        public Image cursorImage;
        public Vector2 origin;
        public Vector2 interval;
    }

    [System.Serializable]
    public class Caution {
        public Canvas cautionCanvas;
        public RectTransform panelRect;
        public Vector2 origin;
        public Vector2 interval;
        public TMP_Text cautionText;
        public Image fillImage;
    }

    public bool permitEmptySlot = false;
    public Choices save;
    public Caution caution;
    public SaveSlot[] texts;
    public TMP_Text controlTypeText;
    public Image[] backImage;
    public Image formatFrame;
    public TMP_Text formatText;

    [System.NonSerialized]
    public int controlType;

    // private GameManager.Save[] saveTemp;
    private int saveSlot = 0;
    private int sumOrig = 0;
    //0-Load 1-Cancel 2-Copy 3-Delete
    private int[] saveDifficulty;
    private StringBuilder sb = new StringBuilder();
    private Vector2Int move;
    private float buttonDownTime;
    private bool isOverwriting;
    // private static readonly int[] specialStage = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 13, 13 };
    // private static readonly int[] specialFloor = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 33, 38 };
    private const int specialStageMax = 31;
    private static readonly int[,] specialStageFloor = new int[specialStageMax, 2] {
        {0, 0 }, {0, 1 }, {0, 2 }, {0, 3 }, {0, 4 }, {0, 5 }, {0, 6 }, {0, 7 }, {0, 8 }, {0, 9 }, {0, 10 }, {0, 11 }, {0, 12 }, {0, 13 },
        {12, 0 }, {12, 1 }, {12, 2 }, {12, 3 }, {12, 4 }, {12, 5 }, {12, 6 }, {12, 7 }, {12, 8 }, {12, 9 }, {12, 10 }, {12, 11 }, {12, 12 }, {12, 13 }, {12, 14 },
        {13, 33 }, {13, 38 }
    };
    private static readonly Color[] formatColors = new Color[formatMax] {
        new Color(1f, 1f, 0.6f), 
        new Color(0.6f, 1f, 0.6f),
        new Color(0.8f, 0.6f, 1f)
    };

    private int eventEnterNum = -1;
    private int eventClickReserved = -1;
    private int eventClickTypeReserved = -1;
    private int eventClickOrigReserved = -1;
    private float mouseScroll;
    private int displayFormat;
    private const int formatMax = 3;
    private GameManager.Save saveTemp;

    private void SetTexts() {
        for (int i = 0; i < texts.Length; i++) {
            int fileIndex = (i + sumOrig) % GameManager.saveSlotMax;
            texts[i].slotIndex = i;
            texts[i].SetSlot(fileIndex, displayFormat);
        }
    }

    public void Activate(int saveSlot = -1) {
        displayFormat = GameManager.Instance.GetDisplayFormat();
        if (!(saveSlot >= 0 && saveSlot < GameManager.saveSlotMax)) {
            saveSlot = GameManager.Instance.currentSaveSlot;
            if (saveSlot < 0) {
                for (int i = 0; i < GameManager.saveSlotMax; i++) {
                    saveTemp = GameManager.Instance.TempLoad(i);
                    if (saveTemp != null && saveTemp.dataExist == 0) {
                        saveSlot = i;
                        break;
                    }
                }
            }
            if (saveSlot < 0) {
                saveSlot = 0;
            }
        }
        sumOrig = this.saveSlot = saveSlot;
        save.cursorPos = (saveSlot - sumOrig + GameManager.saveSlotMax) % GameManager.saveSlotMax;
        save.cursorRect.anchoredPosition = save.origin + save.interval * save.cursorPos;
        SetTexts();
        SetControlType(0);
        SetFormatColor();
        SetGridLayoutEnabled(true);
        save.canvas.enabled = true;
        if (save.gRaycaster && GameManager.Instance.MouseEnabled) {
            save.gRaycaster.enabled = true;
        }
        if (GameManager.Instance.state == GameManager.State.play && PauseController.Instance) {
            PauseController.Instance.pauseEnabled = false;
        }
    }

    public void Deactivate() {
        save.canvas.enabled = false;
        SetGridLayoutEnabled(false);
        if (GameManager.Instance.state == GameManager.State.play && PauseController.Instance) {
            PauseController.Instance.pauseEnabled = true;
        }
        if (save.gRaycaster) {
            save.gRaycaster.enabled = false;
        }
        DeactivateCaution();
    }

    void SetGridLayoutEnabled(bool flag) {
        save.gridLayoutGroup.enabled = flag;
        for (int i = 0; i < texts.Length; i++) {
            texts[i].friendsFaceGrid.enabled = flag;
            texts[i].minmiGrid.enabled = flag;
        }
    }

    public bool IsSpecialFloor(int stage, int floor) {
        for (int i = 0; i < specialStageMax; i++) {
            if (stage == specialStageFloor[i, 0] && floor == specialStageFloor[i, 1]) {
                return true;
            }
        }
        return false;
    }

    public void SetControlType(int num) {
        backImage[controlType].enabled = false;
        controlType = num;
        backImage[controlType].enabled = true;
        if (controlTypeText) {
            if (permitEmptySlot) {
                controlTypeText.text = TextManager.Get(StringUtils.Format("SAVETYPE_{0}", controlType));
            } else {
                controlTypeText.text = TextManager.Get(StringUtils.Format("LOADTYPE_{0}", controlType));
            }
        }
        save.cursorImage.enabled = true;
    }

    public bool SaveControl_Move() {
        int controlTypeMax = (GameManager.Instance.state == GameManager.State.title ? 3 : 1);
        move = GameManager.Instance.MoveCursor(true);
        int length = GameManager.saveSlotMax;
        if (move.y != 0 || eventEnterNum >= 0 || eventClickOrigReserved >= 0 || mouseScroll != 0) {
            if (UISE.Instance) {
                UISE.Instance.Play(UISE.SoundName.move);
            }
            if (move.y != 0) {
                saveSlot = (saveSlot + move.y + length) % length;
                if (move.y < 0 && save.cursorPos <= 0) {
                    sumOrig = saveSlot;
                } else if (move.y > 0 && save.cursorPos >= texts.Length - 1) {
                    sumOrig = (saveSlot - (texts.Length - 1) + length) % length;
                } else {
                    save.cursorPos = (saveSlot - sumOrig + length) % length;
                }
            }
            if (eventEnterNum >= 0) {
                saveSlot = (sumOrig + eventEnterNum) % length;
                save.cursorPos = eventEnterNum;
            } else if (eventClickOrigReserved >= 0) {
                int yTemp = (eventClickOrigReserved == 0 ? -1 : 1);
                saveSlot = (saveSlot + yTemp + length) % length;
                sumOrig = (sumOrig + yTemp + length) % length;
            }
            if (mouseScroll != 0) {
                int yTemp = mouseScroll > 0 ? -1 : 1;
                saveSlot = (saveSlot + yTemp + length) % length;
                sumOrig = (sumOrig + yTemp + length) % length;
            }
            save.cursorRect.anchoredPosition = save.origin + save.interval * save.cursorPos;
            SetTexts();
            return true;
        }
        if (controlTypeMax > 1 && (move.x != 0 || eventClickTypeReserved >= 0)) {
            if (UISE.Instance) {
                UISE.Instance.Play(UISE.SoundName.move);
            }
            if (move.x == 0) {
                move.x = (eventClickTypeReserved == 0 ? -1 : 1);
            }
            SetControlType((controlType + move.x + controlTypeMax) % controlTypeMax);
            return true;
        }
        return false;
    }


    public int SaveControl() {
        int answer = -1;
        mouseScroll = Input.mouseScrollDelta.y;
        if (GameManager.Instance.MouseCancelling) {
            eventEnterNum = -1;
            eventClickReserved = -1;
            eventClickTypeReserved = -1;
            eventClickOrigReserved = -1;
            mouseScroll = 0;
        }
        bool movedFlag = SaveControl_Move();
        if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || eventClickReserved >= 0) {
            if (eventClickReserved >= 0) {
                saveSlot = (sumOrig + eventClickReserved) % GameManager.saveSlotMax;
            }

            if (permitEmptySlot) {
                answer = saveSlot;
            } else {
                saveTemp = GameManager.Instance.TempLoad(saveSlot);
                if (saveTemp != null && saveTemp.dataExist != 0) {
                    answer = saveSlot;
                } else {
                    answer = -2;
                }
            }
        } else if (GameManager.Instance.GetCancelButtonDown) {
            answer = -3;
        } else if (movedFlag == false && 
            (  GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Special)
            || GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Dodge)
            || GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Aim)
            )) {
            ChangeDisplayFormat();
        }
        eventEnterNum = -1;
        eventClickReserved = -1;
        eventClickTypeReserved = -1;
        eventClickOrigReserved = -1;
        mouseScroll = 0;
        return answer;
    }

    void SetFormatColor() {
        formatFrame.color = formatColors[displayFormat];
        formatText.text = TextManager.Get(string.Format("SAVE_FORMAT_{0}", displayFormat));
    }

    public void ChangeDisplayFormat() {
        displayFormat = (displayFormat + 1) % formatMax;
        GameManager.Instance.SetDisplayFormat(displayFormat);
        SetTexts();
        SetFormatColor();
        if (UISE.Instance) {
            UISE.Instance.Play(UISE.SoundName.move);
        }
    }

    public bool CheckOverwriteDanger {
        get {
            if (saveSlot >= 0 && saveSlot < GameManager.saveSlotMax && saveSlot != GameManager.Instance.currentSaveSlot) {
                saveTemp = GameManager.Instance.TempLoad(saveSlot);
                if (saveTemp != null && saveTemp.dataExist != 0) {
                    return true;
                }
            }
            return false;
        }
    }

    public void ActivateCaution() {
        if (UISE.Instance) {
            UISE.Instance.Play(UISE.SoundName.caution);
        }
        caution.panelRect.anchoredPosition = caution.origin + caution.interval * save.cursorPos;
        caution.cautionText.text = permitEmptySlot ? TextManager.Get("CAUTION_OVERWRITE") : TextManager.Get("CAUTION_DELETE");
        caution.fillImage.fillAmount = 0;
        caution.cautionCanvas.enabled = true;
    }

    public void DeactivateCaution() {
        if (caution.cautionCanvas.enabled) {
            caution.cautionCanvas.enabled = false;
        }
        isOverwriting = false;
        buttonDownTime = 0f;
    }

    public void SetCautionFill(float fillAmount) {
        caution.fillImage.fillAmount = Mathf.Clamp01(fillAmount);
    }

    public int SaveControlExternal(bool eyeCatchEnabled = true, bool eyeCatchSfx = true) {
        int answer = -1;
        if (!isOverwriting) {
            answer = SaveControl();
            if (answer >= 0 && answer < GameManager.saveSlotMax) {
                saveSlot = answer;
                if (CheckOverwriteDanger) {
                    ActivateCaution();
                    buttonDownTime = 0;
                    isOverwriting = true;
                    answer = -1;
                } else {
                    Deactivate();
                    if (eyeCatchEnabled && SceneChange.Instance) {
                        SceneChange.Instance.StartEyeCatch(eyeCatchSfx);
                    }
                    if (UISE.Instance && (!eyeCatchEnabled || !eyeCatchSfx)) {
                        UISE.Instance.Play(UISE.SoundName.submit);
                    }
                }
            } else if (answer < -1) {
                Deactivate();
                if (UISE.Instance) {
                    UISE.Instance.Play(UISE.SoundName.cancel);
                }
            }
        } else {
            if (SaveControl_Move()) {
                DeactivateCaution();
            } else if (GameManager.Instance.GetCancelButtonDown) {
                DeactivateCaution();
                if (UISE.Instance) {
                    UISE.Instance.Play(UISE.SoundName.cancel);
                }
            } else if (GameManager.Instance.playerInput.GetButton(RewiredConsts.Action.Submit)) {
                buttonDownTime += Time.unscaledDeltaTime;
                SetCautionFill(buttonDownTime);
                if (buttonDownTime >= 1f) {
                    Deactivate();
                    if (eyeCatchEnabled && SceneChange.Instance) {
                        SceneChange.Instance.StartEyeCatch(eyeCatchSfx);
                    }
                    if (UISE.Instance && (!eyeCatchEnabled || !eyeCatchSfx)) {
                        UISE.Instance.Play(UISE.SoundName.submit);
                    }
                    answer = saveSlot;
                }
            } else {
                buttonDownTime = 0f;
                SetCautionFill(buttonDownTime);
            }
        }
        return answer;
    }

    public void EventEnter(int param) {
        eventEnterNum = param;
    }

    public void EventClick(int param) {
        if (!Input.GetMouseButtonUp(1)) {
            eventClickReserved = param;
        }
    }

    public void EventClickType(int param) {
        if (!Input.GetMouseButtonUp(1)) {
            eventClickTypeReserved = param;
        }
    }

    public void EventClickOrig(int param) {
        if (!Input.GetMouseButtonUp(1)) {
            eventClickOrigReserved = param;
        }
    }

}
