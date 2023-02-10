using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ChangeExtraCharacter : MonoBehaviour
{

    public int extraIndex;

    Player playerInput;
    int state;
    int pauseWait;
    bool entering;
    bool actionTextEnabled;
    bool isAnother;
    float coolTime;

    const string targetTag = "ItemGetter";

    private void Start() {
        playerInput = GameManager.Instance.playerInput;
    }

    private void Update() {
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
                        if (pauseWait <= 0 && coolTime <= 0f && PauseController.Instance.pauseEnabled && playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                            UISE.Instance.Play(UISE.SoundName.submit);
                            CharacterManager.Instance.extraFriendsIndex = extraIndex;
                            CharacterManager.Instance.ChangeFriends(CharacterManager.extraFriendsID[0], true, false, true);
                        }
                    }
                    break;
            }
            bool actionTemp = (state == 1 && pauseWait <= 0 && coolTime <= 0f && PauseController.Instance.pauseEnabled);
            if (actionTextEnabled != actionTemp) {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            entering = true;
            state = 1;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            entering = false;
            state = 0;
        }
    }

}
