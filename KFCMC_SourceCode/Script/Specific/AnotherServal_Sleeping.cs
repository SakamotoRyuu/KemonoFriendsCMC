using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Mebiustos.MMD4MecanimFaciem;

public class AnotherServal_Sleeping : MonoBehaviour {

    public GameObject balloonPrefab;
    public GameObject campoFlickerSpecialTalk;

    private MessageBackColor mesBackColor;
    private Animator anim;
    private FaciemController fCon;
    private int state = 0;
    private GameObject balloonInstance;
    private bool condition;
    private bool pauseSave;
    private float destroyTime;
    private StringBuilder sb = new StringBuilder();
    private int faceIndex;
    private int pauseWait;

    private const int itemConditionID = 81;
    private const int lodgeConditionID = 23;
    private const int characterIndex = 31;
    private const int friendsIndexBottom = 100;
    private const string title = "ITEM_NAME_081";
    private const string choice1 = "CHOICE_USE";
    private string choice2 = "CHOICE_CANCEL";
    private const string targetTag = "ItemGetter";

    private void Awake() {
        anim = GetComponent<Animator>();
        fCon = GetComponent<FaciemController>();
        mesBackColor = GetComponent<MessageBackColor>();
    }

    private void Start() {
        balloonInstance = Instantiate(balloonPrefab, transform.position + new Vector3(0f, 0.15f, 0f), transform.rotation, transform);
        condition = GameManager.Instance.save.NumOfSpecificItems(itemConditionID) > 0;
        if (balloonInstance && balloonInstance.activeSelf != condition) {
            balloonInstance.SetActive(condition);
        }
        Instantiate(MapDatabase.Instance.prefab[MapDatabase.itemCharacter], transform);
    }

    protected virtual void Update() {
        if (PauseController.Instance) {
            if (!PauseController.Instance.pauseGame) {
                if (pauseSave) {
                    condition = GameManager.Instance.save.NumOfSpecificItems(itemConditionID) > 0;
                    if (balloonInstance && balloonInstance.activeSelf != condition) {
                        balloonInstance.SetActive(condition);
                    }
                    pauseSave = false;
                }
            } else {
                pauseSave = true;
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
                        } else if (PauseController.Instance.GetPauseEnabled() && GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) && GameManager.Instance.save.NumOfSpecificItems(itemConditionID) > 0) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.SetChoices(2, true, TextManager.Get(title), choice1, choice2);
                            state = 2;
                        }
                        break;
                    case 2:
                        switch (PauseController.Instance.ChoicesControl()) {
                            case -2:
                                UISE.Instance.Play(UISE.SoundName.cancel);
                                PauseController.Instance.CancelChoices();
                                state = 1;
                                break;
                            case 0:
                                UISE.Instance.Play(UISE.SoundName.submit);
                                PauseController.Instance.CancelChoices(true);
                                GameManager.Instance.save.RemoveItem(itemConditionID);
                                Bond();
                                state = 3;
                                break;
                            case 1:
                                UISE.Instance.Play(UISE.SoundName.submit);
                                PauseController.Instance.CancelChoices();
                                state = 1;
                                break;
                        }
                        break;
                    case 3:
                        destroyTime += Time.deltaTime;
                        if (destroyTime > 5f) {
                            if (GameManager.Instance.save.config[GameManager.Save.configID_GenerateHomeObjects] != 0 && GameManager.Instance.save.friends[lodgeConditionID] != 0) {
                                AnotherServalIFRBranch ifrBranch = GetComponentInParent<AnotherServalIFRBranch>();
                                if (ifrBranch && ifrBranch.targetIFR.Length > ifrBranch.awakeIndex) {
                                    ifrBranch.targetIFR[ifrBranch.awakeIndex].SetActive(true);
                                }
                            }
                            Destroy(gameObject);
                            state = 100;
                        }
                        break;
                }
            }
        }
    }

    void Bond() {
        string margedName = "TALK_ANOTHERSERVAL_GET";
        faceIndex = friendsIndexBottom + characterIndex;
        MessageUI.Instance.SetMessageOptional(TextManager.Get(margedName), mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType, faceIndex, -1, 1, 1, true);
        MessageUI.Instance.SetMessageLog(TextManager.Get(margedName), faceIndex);
        Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.getFriends], transform.position, Quaternion.identity);
        if (anim) {
            anim.SetTrigger("Smile");
        }
        if (fCon) {
            fCon.SetFace("Awake");
        }
        GameManager.Instance.save.friends[0] = 1;
        if (GameManager.Instance.save.friends[characterIndex] == 0) {
            GameManager.Instance.save.friends[characterIndex] = 1;
            if (MessageUI.Instance != null && ItemDatabase.Instance != null) {
                MessageUI.Instance.SetMessage(sb.Clear().Append(TextManager.Get("QUOTE_START")).Append(ItemDatabase.Instance.GetItemName(faceIndex)).Append(TextManager.Get("QUOTE_END")).Append(TextManager.Get("MESSAGE_FRIENDSBOND")).ToString(), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Friends);
            }
        }
        if (balloonInstance) {
            Destroy(balloonInstance);
        }
        if (campoFlickerSpecialTalk) {
            Instantiate(campoFlickerSpecialTalk);
        }
        TrophyManager.Instance.CheckTrophy(TrophyManager.t_RescueAllFriends);
    }

    private void OnTriggerEnter(Collider other) {
        if (state == 0 && other.CompareTag(targetTag)) {
            state = 1;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (state == 1 && other.CompareTag(targetTag)) {
            state = 0;
        }
    }

}
