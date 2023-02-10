using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Rewired;

public class Goal : MonoBehaviour {

    public Renderer goalMark;
    public Material triggerExitMat;
    public Material triggerEnterMat;
    public string title = "WORD_GOALPOINT";
    public string choice1 = "CHOICE_ENTER";
    public string choice2 = "CHOICE_SAVEENTER";
    public string choice3 = "CHOICE_CANCEL";
    public int floorNum = 1;
    public bool additive = true;
    public GameObject destroyTarget;
    public GameObject climbBalloonPrefab;
    public GameObject stageNamePrefab;
    public Vector3 stageNameOffset;
    public bool overridePlayerFixEnabled;
    public Vector3 overridePlayerPosition;
    public Vector3 overridePlayerRotation;
    public bool checkDefeatEnemyCondition;
    public Material disabledMat;
    public GameObject showConditionObj;
    public TMP_Text showConditionText;
    public bool saveDisabled;

    protected int state = 0;
    protected int pauseWait;
    protected int saveSlot = -1;
    protected GameObject climbBalloonInstance;
    protected int language = -1;
    protected UIOperationCanvas_Scaling_TwoTone operation;
    protected bool defeatEnemyConditionSave;
    protected int defeatEnemyCountSave;
    protected int stageSave;
    protected int floorSave;
    protected int answer;
    protected float buttonDownTime;
    protected bool actionTextEnabled;

    const string targetTag = "ItemGetter";

    protected virtual void Move() {
        if (StageManager.Instance && StageManager.Instance.dungeonMother) {
            StageManager.Instance.dungeonMother.MoveFloor(floorNum, saveSlot, additive);
            if (overridePlayerFixEnabled) {
                CharacterManager.Instance.playerTrans.position = overridePlayerPosition;
                CharacterManager.Instance.playerTrans.eulerAngles = overridePlayerRotation;
                CharacterManager.Instance.PlaceFriendsAroundPlayer();
                CameraManager.Instance.ResetCameraFixPos();
            }
        }
    }

    protected virtual void Start() {
        if (StageManager.Instance) {
            int index = StageManager.Instance.GetShortcutEndIndex();
            if (index >= 0) {
                floorNum = StageManager.Instance.dungeonMother.shortcutSettings[index].endFloor;
                additive = false;
            }
            SetMapChip();
        }
        if (climbBalloonPrefab) {
            climbBalloonInstance = Instantiate(climbBalloonPrefab, transform.position + new Vector3(0f, 4f, 0f), transform.rotation, transform);
            climbBalloonInstance.SetActive(false);
        }
        if (stageNamePrefab) {
            operation = Instantiate(stageNamePrefab, transform.position + stageNameOffset, transform.rotation, transform).GetComponent<UIOperationCanvas_Scaling_TwoTone>();
            if (operation) {
                MessageBackColor mesBackColor = GetComponent<MessageBackColor>();
                if (mesBackColor) {
                    operation.SetFrameColor(mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType);
                }
                SetOperationText();
            }
        }
        defeatEnemyConditionSave = GetDefeatEnemyCondition();
        if (!defeatEnemyConditionSave && disabledMat) {
            goalMark.material = disabledMat;
            if (showConditionObj) {
                showConditionText.text = StageManager.Instance.dungeonController.GetDefeatMissionConditionText();
                defeatEnemyCountSave = CharacterManager.Instance.defeatEnemyCount;
                showConditionObj.SetActive(true);
            }
        }
    }

    protected virtual bool GetDefeatEnemyCondition() {
        if (checkDefeatEnemyCondition && StageManager.Instance && StageManager.Instance.dungeonController) {
            return StageManager.Instance.dungeonController.GetDefeatMissionCompleted();
        }
        return true;
    }

    protected virtual void SetMapChip() {
        Instantiate(MapDatabase.Instance.prefab[MapDatabase.goal], transform);
    }

    private void OnEnable() {
        defeatEnemyConditionSave = GetDefeatEnemyCondition();
        PlayerExit();
    }

    void SetOperationText() {
        if (!string.IsNullOrEmpty(title)) {
            language = GameManager.Instance.save.language;
            operation.SetText(TextManager.Get(title), 100000, true);
        }
    }

    protected virtual void ActionSubmitButton() {
        UISE.Instance.Play(UISE.SoundName.submit);
        if (saveDisabled) {
            PauseController.Instance.SetChoices(2, true, TextManager.Get(title), choice1, choice3);
        } else {
            PauseController.Instance.SetChoices(3, true, TextManager.Get(title), choice1, choice2, choice3);
        }
        if (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.keyItemRemain > 0) {
            if (StageManager.Instance.IsRandomStage) {
                PauseController.Instance.ShowCaution(TextManager.Get("CAUTION_UNLOCK"));
            } else if (!CharacterManager.Instance.inertIFR) {
                if (StageManager.Instance.dungeonController.keyItemIsLB) {
                    PauseController.Instance.ShowCaution(TextManager.Get("CAUTION_HELP_LB"));
                } else {
                    PauseController.Instance.ShowCaution(TextManager.Get("CAUTION_HELP"));
                }
            }
        }
    }

    protected virtual void ChoicesControl() {
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
                saveSlot = -1;
                state = 4;
                break;
            case 1:
                if (saveDisabled) {
                    UISE.Instance.Play(UISE.SoundName.submit);
                    PauseController.Instance.CancelChoices();
                    PauseController.Instance.HideCaution();
                    state = 1;
                } else {
                    UISE.Instance.Play(UISE.SoundName.submit);
                    PauseController.Instance.CancelChoices(false);
                    PauseController.Instance.HideCaution();
                    SaveController.Instance.permitEmptySlot = true;
                    SaveController.Instance.Activate();
                    state = 3;
                }
                break;
            case 2:
                UISE.Instance.Play(UISE.SoundName.submit);
                PauseController.Instance.CancelChoices();
                PauseController.Instance.HideCaution();
                state = 1;
                break;
        }
    }
    
    protected virtual void Update () {
        if (CharacterManager.Instance) {
            if (climbBalloonInstance) {
                if (climbBalloonInstance.activeSelf && !CharacterManager.Instance.isClimbing) {
                    climbBalloonInstance.SetActive(false);
                } else if (!climbBalloonInstance.activeSelf && CharacterManager.Instance.isClimbing) {
                    climbBalloonInstance.SetActive(true);
                }
            }
            if (!CharacterManager.Instance.GetPlayerLive()) {
                PlayerExit();
            }
        }
        if (language >= 0 && operation && language != GameManager.Instance.save.language) {
            SetOperationText();
        }
        if (!defeatEnemyConditionSave) {
            defeatEnemyConditionSave = GetDefeatEnemyCondition();
            if (defeatEnemyConditionSave) {
                goalMark.material = (state <= 0 ? triggerExitMat : triggerEnterMat);
                if (showConditionObj) {
                    showConditionObj.SetActive(false);
                }
                Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.defeatMissionCompleteVFX], transform.position, Quaternion.identity);
            } else {
                if (showConditionObj && defeatEnemyCountSave != CharacterManager.Instance.defeatEnemyCount) {
                    showConditionText.text = StageManager.Instance.dungeonController.GetDefeatMissionConditionText();
                    defeatEnemyCountSave = CharacterManager.Instance.defeatEnemyCount;
                    if (!showConditionObj.activeSelf) {
                        showConditionObj.SetActive(true);
                    }
                }
            }
        }
        if (PauseController.Instance && CharacterManager.Instance) {
            if (PauseController.Instance.pauseGame) {
                pauseWait = 2;
            } else if (pauseWait > 0) {
                pauseWait--;
            }
            if (pauseWait <= 0) {
                switch (state) {
                    case 1:
                        if (PauseController.Instance.returnToLibraryProcessing) {
                            state = 0;
                        }
                        if (stageSave != StageManager.Instance.stageNumber || floorSave != StageManager.Instance.floorNumber) {
                            PlayerExit();
                        }
                        if (pauseWait <= 0 && PauseController.Instance.GetPauseEnabled() && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            if (defeatEnemyConditionSave) {
                                ActionSubmitButton();
                                state = 2;
                            } else {
                                UISE.Instance.Play(UISE.SoundName.error);
                                if (MessageUI.Instance && StageManager.Instance && StageManager.Instance.dungeonController) {
                                    MessageUI.Instance.SetMessage(TextManager.Get("WORD_DEFEATCONDITION") + " " + StageManager.Instance.dungeonController.GetDefeatMissionConditionText());
                                }
                            }
                        }
                        break;
                    case 2:
                        ChoicesControl();
                        break;
                    case 3:
                        saveSlot = -1;
                        answer = SaveController.Instance.SaveControlExternal();
                        if (answer >= 0 && answer < GameManager.saveSlotMax) {
                            saveSlot = answer;
                            state += 1;
                        } else if (answer < -1) {
                            PauseController.Instance.CancelChoices();
                            state -= 2;
                        }
                        break;
                    case 4:
                        if (SceneChange.Instance.GetEyeCatch()) {
                            PauseController.Instance.CancelChoices();
                            Move();
                            SceneChange.Instance.EndEyeCatch();
                            if (destroyTarget) {
                                Destroy(destroyTarget);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            bool actionTemp = (state == 1 && pauseWait <= 0 && PauseController.Instance.pauseEnabled && !SceneChange.Instance.GetIsProcessing);
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
	}
    
    protected virtual void PlayerEnter() {
        state = state == 0 ? 1 : state;
        if (defeatEnemyConditionSave) {
            if (goalMark != null && triggerEnterMat != null) {
                goalMark.material = triggerEnterMat;
            }
        }
        if (state == 1 && StageManager.Instance) {
            stageSave = StageManager.Instance.stageNumber;
            floorSave = StageManager.Instance.floorNumber;
        }
    }

    protected virtual void PlayerExit() {
        state = state == 1 ? 0 : state;
        if (defeatEnemyConditionSave && goalMark != null && triggerExitMat != null) {
            goalMark.material = triggerExitMat;
        }
        if (state == 0) {
            stageSave = -1;
            floorSave = -1;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (enabled && other.CompareTag(targetTag)) {
            PlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (enabled && other.CompareTag(targetTag)) {
            PlayerExit();
        }
    }

}
