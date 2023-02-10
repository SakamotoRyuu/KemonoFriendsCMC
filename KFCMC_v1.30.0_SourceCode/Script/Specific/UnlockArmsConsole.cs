using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System.Text;

public class UnlockArmsConsole : MonoBehaviour
{
    
    public GameObject climbBalloonPrefab;
    public GameObject effectPrefab;
    public Vector3 effectOffset;

    const string targetTag = "ItemGetter";
    int state;
    int pauseWait;
    GameObject climbBalloonInstance;
    GameObject mapChip;
    StringBuilder sb = new StringBuilder();
    int[] lockedTypes;
    string[] lockedNames;
    int choicesAnswer;
    bool entering;
    bool actionTextEnabled;

    private void Start() {
        if (climbBalloonPrefab) {
            climbBalloonInstance = Instantiate(climbBalloonPrefab, transform.position + new Vector3(0f, 4f, 0f), transform.rotation, transform);
            climbBalloonInstance.SetActive(false);
        }
        mapChip = Instantiate(MapDatabase.Instance.prefab[MapDatabase.other], transform);
        if (StageManager.Instance && StageManager.Instance.dungeonController) {
            StageManager.Instance.dungeonController.keyItemRemain += 1;
        }
    }

    protected virtual void Update() {
        if (CharacterManager.Instance && PauseController.Instance) {
            if (climbBalloonInstance) {
                if (climbBalloonInstance.activeSelf && !CharacterManager.Instance.isClimbing) {
                    climbBalloonInstance.SetActive(false);
                } else if (!climbBalloonInstance.activeSelf && CharacterManager.Instance.isClimbing) {
                    climbBalloonInstance.SetActive(true);
                }
            }
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
                        if (pauseWait <= 0 && PauseController.Instance.pauseEnabled && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            SetChoices();
                            UISE.Instance.Play(UISE.SoundName.submit);
                            state = 2;
                        }
                        break;
                    case 2:
                        choicesAnswer = PauseController.Instance.ChoicesControl();
                        if (choicesAnswer == -2) {
                            UISE.Instance.Play(UISE.SoundName.cancel);
                            CancelCommon();
                        } else if (choicesAnswer >= 0) {
                            if (lockedTypes[choicesAnswer] == 200) {
                                UISE.Instance.Play(UISE.SoundName.submit);
                                CancelCommon();
                            } else if (lockedTypes[choicesAnswer] == 100) {
                                LevelUp();
                            } else {
                                Unlock(lockedTypes[choicesAnswer]);
                            }
                        }
                        break;
                    case 3:
                        break;
                }
            }
            bool actionTemp = (state == 1 && pauseWait <= 0 && PauseController.Instance.pauseEnabled);
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    int GetRemainLockedCount() {
        int answer = 0;
        for (int i = 0; i < GameManager.Instance.save.armsLocked.Length; i++) {
            if (GameManager.Instance.save.armsLocked[i] > 0) {
                answer++;
            }
        }
        return answer;
    }

    void SetChoices() {
        lockedTypes = new int[GameManager.armsLockMax + 2];
        lockedNames = new string[GameManager.armsLockMax + 2];
        int countTemp = 0;
        string armsObject;
        string armsVerb = TextManager.Get("CHOICE_UNLOCK");
        for (int i = 0; i < GameManager.Instance.save.armsLocked.Length; i++) {
            if (GameManager.Instance.save.armsLocked[i] >= 1) {
                lockedTypes[countTemp] = i;
                armsObject = TextManager.Get(string.Format("ITEM_NAME_{0:000}", GameManager.armsIDBase + GameManager.Instance.GetLockedArmsID(i)));
                if (GameManager.Instance.save.language == GameManager.languageJapanese) {
                    lockedNames[countTemp] = sb.Clear().Append(armsObject).Append(armsVerb).ToString();
                } else {
                    lockedNames[countTemp] = sb.Clear().Append(armsVerb).Append(armsObject).ToString();
                }
                countTemp++;
            }
        }
        lockedTypes[countTemp] = 100;
        lockedNames[countTemp] = TextManager.Get("CHOICE_LEVELUP");
        countTemp++;
        lockedTypes[countTemp] = 200;
        lockedNames[countTemp] = TextManager.Get("CHOICE_CANCEL");
        countTemp++;
        PauseController.Instance.SetChoicesDirect(countTemp, true, TextManager.Get("WORD_UNLOCKCONSOLE"), lockedNames[0], lockedNames[1], lockedNames[2], lockedNames[3], lockedNames[4]);
    }

    private void CancelCommon() {
        PauseController.Instance.CancelChoices();
        state = (entering ? 1 : 0);
    }

    void HPUp() {
        if (GameManager.Instance.save.isExpPreserved != 0 && Mathf.Min(GameManager.Instance.save.GotHpUpOriginal, GameManager.Instance.save.equipedHpUp) > GameManager.Instance.save.unlockCount + 10) {
            MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_HPUP"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_HP);
        }
        if (GameManager.Instance.save.isExpPreserved != 0 && Mathf.Min(GameManager.Instance.save.GotStUpOriginal, GameManager.Instance.save.equipedStUp) > GameManager.Instance.save.unlockCount + 10) {
            MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_STUP"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_ST);
        }
        GameManager.Instance.save.unlockCount++;
    }

    void LevelUp() {
        CharacterManager.Instance.AddExp(GameManager.Instance.save.NeedExpToNextLevel, true);
        HPUp();
        DestroyCommon();
    }

    void Unlock(int type) {
        int lockedArmsID = GameManager.Instance.GetLockedArmsID(type);
        if (MessageUI.Instance) {
            string armsObject = TextManager.Get(string.Format("ITEM_NAME_{0:000}", GameManager.armsIDBase + lockedArmsID));
            MessageUI.Instance.SetMessage(sb.Clear().Append(armsObject).Append(TextManager.Get("MESSAGE_UNLOCKED")).ToString(), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Lucky);
        }
        GameManager.Instance.save.armsLocked[type] -= 1;
        bool setWeaponFlag = false;
        if (GameManager.Instance.save.equip[lockedArmsID] == 0) {
            setWeaponFlag = true;
            switch (lockedArmsID) {
                case 3:
                    if (GameManager.Instance.save.equip[5] != 0 || GameManager.Instance.save.equip[4] != 0) {
                        setWeaponFlag = false;
                    }
                    break;
                case 4:
                    if (GameManager.Instance.save.equip[5] != 0) {
                        setWeaponFlag = false;
                    }
                    break;
                case 6:
                    if (GameManager.Instance.save.equip[8] != 0 || GameManager.Instance.save.equip[7] != 0) {
                        setWeaponFlag = false;
                    }
                    break;
                case 7:
                    if (GameManager.Instance.save.equip[8] != 0) {
                        setWeaponFlag = false;
                    }
                    break;
                case 9:
                    if (GameManager.Instance.save.equip[10] != 0) {
                        setWeaponFlag = false;
                    }
                    break;
            }
        }
        if (setWeaponFlag) {
            CharacterManager.Instance.SetWeapon(lockedArmsID);
        }
        CharacterManager.Instance.CheckWeaponAll();
        CharacterManager.Instance.UpdateSandstarMax();
        HPUp();
        DestroyCommon();
    }

    void DestroyCommon() {
        if (StageManager.Instance && StageManager.Instance.dungeonController) {
            StageManager.Instance.dungeonController.keyItemRemain -= 1;
        }
        CancelCommon();
        state = 3;
        Instantiate(effectPrefab, transform.position + effectOffset, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (enabled && other.CompareTag(targetTag)) {
            state = state == 0 ? 1 : state;
            entering = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (enabled && other.CompareTag(targetTag)) {
            state = state == 1 ? 0 : state;
            entering = false;
        }
    }

}
