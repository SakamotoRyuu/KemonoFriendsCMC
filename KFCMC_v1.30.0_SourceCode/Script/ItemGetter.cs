using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Rewired;

public class ItemGetter : MonoBehaviour {

    public GameObject itemEffect;
    public GameObject itemImportantEffect;
    public GameObject moneyEffect;
    public GameObject moneyEffectNoAudio;
    GetItem getItem;
    GetMoney getMoney;
    Transform trans;
    string itemNameTemp;
    StringBuilder sb = new StringBuilder();
    bool sounded = false;
    float buttonTime;
    int state;
    float errorClearTimeRemain;
    bool enableUseAtFoot;
    GameObject getItemObj;

    const string itemTag = "Item";
    const string goldTag = "Money";
    static readonly Quaternion quaIden = Quaternion.identity;

    private void Awake() {
        trans = transform;
    }

    private void LateUpdate() {
        if (sounded) {
            sounded = false;
        }
    }

    private void Update() {
        if (state == 0) {
            if (Time.timeScale > 0 && getItem && getItem.itemType == GetItem.ItemType.Normal && getItem.transform && (getItem.transform.position - trans.position).sqrMagnitude < 3.6f) {
                if (!enableUseAtFoot) {
                    CharacterManager.Instance.SetActionType(CharacterManager.ActionType.UseHoldDown, getItemObj);
                    enableUseAtFoot = true;
                }
                if (GameManager.Instance.playerInput.GetButton(RewiredConsts.Action.Submit)) {
                    buttonTime += Time.deltaTime;
                    if (buttonTime >= 0.35f) {
                        buttonTime = 0f;
                        if (PauseController.Instance && PauseController.Instance.GetPauseEnabled()) {
                            PauseController.Instance.SetChoices(2, true, TextManager.Get("ITEM_NAME_" + getItem.id.ToString("000")), "CHOICE_USE", "CHOICE_CANCEL");
                            state = 1;
                        }
                    }
                } else {
                    buttonTime = 0f;
                }
            } else {
                if (enableUseAtFoot) {
                    CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, getItemObj);
                    enableUseAtFoot = false;
                }
            }
        } else if (state == 1) {
            switch (PauseController.Instance.ChoicesControl()) {
                case -2:
                    UISE.Instance.Play(UISE.SoundName.cancel);
                    PauseController.Instance.CancelChoices();
                    state = 2;
                    break;
                case 0:
                    UISE.Instance.Play(UISE.SoundName.submit);
                    if (getItem && PauseController.Instance.UseItem(getItem.id)) {
                        Destroy(getItem.gameObject);
                        getItem = null;
                    } else {
                        UISE.Instance.Play(UISE.SoundName.error);
                        errorClearTimeRemain = 3f;
                    }
                    PauseController.Instance.CancelChoices();
                    state = 2;
                    break;
                case 1:
                    UISE.Instance.Play(UISE.SoundName.submit);
                    PauseController.Instance.CancelChoices();
                    state = 2;
                    break;
            }
        } else if (state == 2) {
            if (!GameManager.Instance.playerInput.GetButton(RewiredConsts.Action.Submit)) {
                state = 0;
            }
        }
        if (errorClearTimeRemain > 0f) {
            errorClearTimeRemain -= Time.deltaTime;
            if ((errorClearTimeRemain <= 2.8f && GameManager.Instance.playerInput.GetButton(RewiredConsts.Action.Submit)) || errorClearTimeRemain <= 0f) {
                errorClearTimeRemain = 0f;
                if (PauseController.Instance) {
                    PauseController.Instance.HideCaution();
                }
            }
        }
    }

    void GetModify(GetItem getItemTemp) {
        itemNameTemp = ItemDatabase.Instance.GetItemName(getItemTemp.id);
        if (getItemTemp.itemType == GetItem.ItemType.Important) {
            MessageUI.Instance.SetMessage(sb.Clear().Append(TextManager.Get("QUOTE_START")).Append(itemNameTemp).Append(TextManager.Get("QUOTE_END")).Append(TextManager.Get("MESSAGE_GOTITEM")).ToString(), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Important);
            Instantiate(itemImportantEffect, trans.position, quaIden);
        } else {
            MessageUI.Instance.SetMessage(sb.Clear().Append(TextManager.Get("QUOTE_START")).Append(itemNameTemp).Append(TextManager.Get("QUOTE_END")).Append(TextManager.Get("MESSAGE_GOTITEM")).ToString());
            Instantiate(itemEffect, trans.position, quaIden);
        }
        if (getItemTemp == getItem) {
            getItem = null;
        }
        if (getItemTemp.id == GameManager.megatonCoinID) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_MegatonCoin, true);
        }
        Destroy(getItemTemp.gameObject);
    }

    public void GetItemProcess(GetItem getItemTemp) {
        bool getScceed = false;
        int id = getItemTemp.id;
        switch (getItemTemp.itemType) {
            case GetItem.ItemType.Normal:
            case GetItem.ItemType.Important:
                if (GameManager.Instance.save.AddItem(id)) {
                    getScceed = true;
                }
                if (GameManager.Instance.save.config[GameManager.Save.configID_JaparibunsAutoPacking] != 0 && PauseController.Instance) {
                    int japaribunType = GameManager.Instance.GetJaparibunType(id);
                    if (japaribunType != 0) {
                        if (getScceed) {
                            PauseController.Instance.JaparimanPacking();
                        } else if (PauseController.Instance.CheckJaparimanPackEnabled(japaribunType)) {
                            PauseController.Instance.JaparimanPacking(japaribunType);
                            getScceed = true;
                        }
                    }
                }
                if (getScceed) {
                    getItemTemp.GetProcess();
                    GetModify(getItemTemp);
                    GameManager.Instance.CheckMinmi();
                } else {
                    getItemTemp.TouchProcess();
                    MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_ITEMFULL"));
                }
                break;
            case GetItem.ItemType.HealHP:
                CharacterManager.Instance.Heal(80, 20, (int)EffectDatabase.id.itemHeal01, true, true, true, true, true);
                if (getItemTemp == getItem) {
                    getItem = null;
                }
                Destroy(getItemTemp.gameObject);
                break;
            case GetItem.ItemType.HealSandstar:
                CharacterManager.Instance.AddSandstar(1.25f, true, (int)EffectDatabase.id.itemSandstar, true, true);
                if (getItemTemp == getItem) {
                    getItem = null;
                }
                Destroy(getItemTemp.gameObject);
                break;
        }

    }    

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(itemTag)) {
            GetItem getItemTemp = other.gameObject.GetComponent<GetItem>();
            if (getItemTemp) {
                getItem = getItemTemp;
                getItemObj = getItem.gameObject;
                CharacterManager.Instance.SetActionType(CharacterManager.ActionType.UseHoldDown, getItemObj);
                enableUseAtFoot = true;
                if (getItem.getEnable) {
                    GetItemProcess(getItem);
                }
            }
        } else if (other.CompareTag(goldTag)) {
            getMoney = other.gameObject.GetComponent<GetMoney>();
            if (getMoney) {
                GameManager.Instance.save.money += getMoney.num;
                if (sounded) {
                    Instantiate(moneyEffectNoAudio, trans.position, quaIden);
                } else {
                    Instantiate(moneyEffect, trans.position, quaIden);
                    sounded = true;
                }
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.ShowGold();
                }
                Destroy(getMoney.gameObject);
                getMoney = null;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(itemTag)) {
            GetItem getItemTemp = other.gameObject.GetComponent<GetItem>();
            if (getItemTemp && getItem == getItemTemp) {
                getItem = null;
            }
        }
    }

}
