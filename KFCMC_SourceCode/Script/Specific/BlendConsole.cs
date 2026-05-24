using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class BlendConsole : MonoBehaviour {

    public GameObject effectPrefab;
    public Transform itemPivot;
    public Transform effectPivot;
    public GameObject balloonPrefab;
    public Vector3 balloonOffset = new Vector3(0, 1.6f, 0);
    public Transform cameraPivot;
    public Transform cameraFocusPoint;
    public bool requireGold;
    public string titleDicKey;
    public float clippingDistance;

    Player playerInput;
    int state = 0;
    int blendIndex = -1;
    int pauseWait;
    bool entering;
    bool actionTextEnabled;
    const string targetTag = "ItemGetter";
    
    protected virtual void Start() {
        Instantiate(balloonPrefab, transform.position + balloonOffset, transform.rotation, transform);
        playerInput = GameManager.Instance.playerInput;
        if (GameManager.Instance.IsPlayerAnother) {
            requireGold = false;
        }
    }

    protected virtual void Update() {
        if (state >= 2 && CharacterManager.Instance.pCon && CharacterManager.Instance.pCon.GetNowHP() <= 0) {
            PauseController.Instance.PauseBlend(false);
            CancelCommon();
            return;
        }
        if (CharacterManager.Instance) {
            switch (state) {
                case 1:
                    if (PauseController.Instance) {
                        if (PauseController.Instance.pauseGame) {
                            pauseWait = 2;
                        } else if (pauseWait > 0) {
                            pauseWait--;
                        }
                        if (PauseController.Instance.returnToLibraryProcessing) {
                            state = 0;
                        }
                        if (pauseWait <= 0 && PauseController.Instance.GetPauseEnabled() && playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            PauseController.Instance.SetChoices(2, true, TextManager.Get(titleDicKey), "CHOICE_USE", "CHOICE_CANCEL");
                            state = 2;
                        }
                    }
                    break;
                case 2:
                    switch (PauseController.Instance.ChoicesControl()) {
                        case -2:
                            UISE.Instance.Play(UISE.SoundName.cancel);
                            CancelCommon();
                            break;
                        case 0:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            if (cameraPivot && CameraManager.Instance) {
                                CameraManager.Instance.SetEventCamera(cameraPivot.position, cameraPivot.eulerAngles, float.MaxValue, 1f, Vector3.Distance(cameraPivot.position, cameraFocusPoint.position), clippingDistance);
                            }
                            PauseController.Instance.CancelChoices();
                            PauseController.Instance.blend.blendConsole = this;
                            PauseController.Instance.PauseBlend(true, requireGold);
                            if (requireGold) {
                                CharacterManager.Instance.showGoldContinuous = true;
                                CharacterManager.Instance.ShowGold();
                            }
                            state = 3;
                            break;
                        case 1:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            CancelCommon();
                            break;
                    }
                    break;
                case 3:
                    if (!PauseController.Instance.pauseGame) {
                        state = 4;
                    }
                    break;
                case 4:
                    state = (entering ? 1 : 0);
                    break;
                case 11:
                    state = 12;
                    break;
                case 12:
                    switch (PauseController.Instance.ChoicesControl()) {
                        case -2:
                            UISE.Instance.Play(UISE.SoundName.cancel);
                            CancelCommon();
                            break;
                        case 0:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            if (requireGold) {
                                int costTemp = PauseController.Instance.blendChildList[blendIndex].overrideCost;
                                if (costTemp <= 0) {
                                    costTemp = ItemDatabase.Instance.GetItemPrice(PauseController.Instance.blendChildList[blendIndex].afterItemID);
                                }
                                GameManager.Instance.save.money -= costTemp;
                            }
                            for (int i = 0; i < PauseController.Instance.blendChildList[blendIndex].beforeItemID.Length; i++) {
                                for (int j = 0; j < PauseController.Instance.blendChildList[blendIndex].beforeItemCount[i]; j++) {
                                    GameManager.Instance.save.RemoveItem(PauseController.Instance.blendChildList[blendIndex].beforeItemID[i]);
                                }
                            }
                            ItemDatabase.Instance.GiveItem(PauseController.Instance.blendChildList[blendIndex].afterItemID, itemPivot.position, 2f, -1, -1, -1, StageManager.Instance.dungeonController.transform);
                            Instantiate(effectPrefab, effectPivot.position, Quaternion.identity);
                            CancelCommon();
                            break;
                        case 1:
                            UISE.Instance.Play(UISE.SoundName.submit);
                            CancelCommon();
                            break;
                    }
                    break;
            }
            bool actionTemp = (state == 1 && pauseWait <= 0 && PauseController.Instance.pauseEnabled);
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    public void SetChoices(int index) {
        blendIndex = index;
        if (blendIndex >= 0) {
            PauseController.Instance.SetChoices(2, true, TextManager.Get(System.String.Format("ITEM_NAME_{0:000}", PauseController.Instance.blendChildList[blendIndex].afterItemID)), "CHOICE_BLEND", "CHOICE_CANCEL");
            state = 11;
        }
    }

    private void CancelCommon() {
        PauseController.Instance.CancelChoices();
        PauseController.Instance.HideCaution();
        CharacterManager.Instance.HideGold();
        blendIndex = -1;
        state = (entering ? 1 : 0);
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            entering = true;
            if (state < 2) {
                state = 1;
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            entering = false;
            if (state < 2) {
                state = 0;
            }
        }
    }
}
