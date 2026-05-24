using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ItemCharacterLB : ItemCharacter {

    public int[] gold;
    public GameObject shopBalloon;
    public Vector3 shopBalloonOffset = new Vector3(0, 0.5f, 0);
    public GameObject completeEffectPrefab;

    protected int shopLevel = 0;
    protected int state = 0;
    protected int pauseWait;
    protected bool completeActionReserved;
    protected const int kabanID = 1;
    protected const string kabanOption = "_KABAN";

    protected override void Start() {
        base.Start();
        shopLevel = ShopDatabase.Instance.GetShopLevel();
        if (chTrans && isHome && shopLevel > 0 && shopBalloon) {
            Instantiate(shopBalloon, chTrans.position + shopBalloonOffset, chTrans.rotation, chTrans);
        }
        submitButtonTalkEnabled = false;
    }

    protected override void KeyItemPlus() {
        base.KeyItemPlus();
        StageManager.Instance.dungeonController.keyItemIsLB = true;
    }

    protected override void HomeUpdate() {
        base.HomeUpdate();
        if (CharacterManager.Instance && !CharacterManager.Instance.inertIFR) {
            switch (state) {
                case 1:
                    if (PauseController.Instance) {
                        if (PauseController.Instance.returnToLibraryProcessing) {
                            state = 0;
                        }
                        if (PauseController.Instance.pauseGame) {
                            pauseWait = 2;
                        } else if (pauseWait > 0) {
                            pauseWait--;
                        }
                        if (shopLevel > 0 && pauseWait <= 0 && PauseController.Instance.GetPauseEnabled() && playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.SetChoices(3, true, TextManager.Get("WORD_SHOP"), "CHOICE_BUY", "CHOICE_SELL", "CHOICE_CANCEL");
                            state = 2;
                        }
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
                            PauseController.Instance.CancelChoices();
                            PauseController.Instance.PauseBuy();
                            state = 1;
                            break;
                        case 1:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.CancelChoices();
                            PauseController.Instance.PauseSell();
                            state = 1;
                            break;
                        case 2:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.CancelChoices();
                            state = 1;
                            break;
                    }
                    break;
            }
        }
    }

    protected override string GetTalkMargedName() {
        if (CharacterManager.Instance && CharacterManager.Instance.GetFriendsExist(kabanID, true)) {
            return base.GetTalkMargedName() + kabanOption;
        } else {
            return base.GetTalkMargedName();
        }
    }

    protected override void DungeonUpdate() {
        if (completeActionReserved && smile && (destroyTimeConstant > 5f || destroyTimeSpeech > 5f) && completeEffectPrefab) {
            string speechContent = TextManager.Get("TALK_LB_COMPLETE");
            smileDestroyTimer = Mathf.Max(smileDestroyTimer, MessageUI.Instance.GetSpeechAppropriateTime(speechContent.Length));
            MessageUI.Instance.SetMessageOptional(speechContent, mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType, dungeonFaceIndex, -1, 1, 1, true);
            MessageUI.Instance.SetMessageLog(speechContent, dungeonFaceIndex);
            Instantiate(completeEffectPrefab, chTrans.position, chTrans.rotation, chTrans);
            if (characterObj) {
                ChangeRendererMaterial changer = characterObj.GetComponent<ChangeRendererMaterial>();
                if (changer) {
                    changer.ChangeMaterial(1);
                }
            }
            MessageUI.Instance.SetMessage(TextManager.Get("MESSAGE_SECURITY_4"), MessageUI.time_Important, MessageUI.panelType_Information, MessageUI.slotType_Lucky);
            completeActionReserved = false;
            destroyTimeConstant = 0f;
            destroyTimeSpeech = 0f;
        }
        base.DungeonUpdate();
    }

    protected override void DungeonAction() {
        base.DungeonAction();
        if (CharacterManager.Instance && !CharacterManager.Instance.inertIFR && stageNumber < gold.Length) {
            ItemDatabase.Instance.GiveGold(gold[stageNumber], trans);
        }
        if (IsLBComplete()) {
            smileDestroyTimer = 15f;
            completeActionReserved = true;
        }
        TrophyManager.Instance.CheckTrophy(TrophyManager.t_RescueAllLB);
    }

    protected override void SetHomeTalkOption() {
        homeTalkOption = sb.Clear().AppendFormat("_{0:00}", shopLevel).ToString();
    }

    protected override void SetHomeTalkIndex() { }

    protected override void HomeAction() {
        base.HomeAction();
        state = 1;
    }

    protected bool IsLBComplete() {
        if (ShopDatabase.Instance) {
            return (ShopDatabase.Instance.GetShopLevel() >= 4);
        } else {
            return false;
        }
    }

    protected override void OnTriggerExit(Collider other) {
        base.OnTriggerExit(other);
        if (isHome && other.CompareTag(targetTag)) {
            state = 0;
        }
    }

    protected override void SetActionText() {
        if (CharacterManager.Instance) {
            if (!InertIFR && !PauseController.Instance.pauseGame && state == 1) {
                if (!actionEnabled) {
                    CharacterManager.Instance.SetActionType(CharacterManager.ActionType.Talk, gameObject);
                    actionEnabled = true;
                }
            } else {
                if (actionEnabled) {
                    CharacterManager.Instance.SetActionType(CharacterManager.ActionType.None, gameObject);
                    actionEnabled = false;
                }
            }
        }
    }
}
