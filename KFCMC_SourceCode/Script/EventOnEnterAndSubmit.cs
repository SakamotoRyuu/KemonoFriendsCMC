using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Rewired.UI.ControlMapper.Window;

public class EventOnEnterAndSubmit : MonoBehaviour
{

    protected bool entered = false;
    protected int pauseWait;
    protected bool actionTextEnabled;
    protected int state;
    protected bool isCalledFrame;
    const string targetTag = "ItemGetter";

    protected virtual void CallAction() { }

    protected virtual void Update()
    {
        isCalledFrame = false;
        if (PauseController.Instance && PauseController.Instance.pauseGame)
        {
            pauseWait = 2;
        }
        else if (pauseWait > 0)
        {
            pauseWait--;
        }
        if (PauseController.Instance && PauseController.Instance.returnToLibraryProcessing)
        {
            entered = false;
        }
        if (CharacterManager.Instance)
        {
            if (state == 0)
            {
                if (entered && Time.timeScale > 0f && pauseWait <= 0)
                {
                    if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit))
                    {
                        CallAction();
                        isCalledFrame = true;
                    }
                }
            }
            bool actionTemp = (state == 0 && entered && pauseWait <= 0 && PauseController.Instance.pauseEnabled);
            if (actionTextEnabled != actionTemp)
            {
                CharacterManager.Instance.SetActionType(actionTemp ? CharacterManager.ActionType.Search : CharacterManager.ActionType.None, gameObject);
                actionTextEnabled = actionTemp;
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            entered = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            entered = false;
        }
    }

}
