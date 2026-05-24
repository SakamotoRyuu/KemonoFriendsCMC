using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Rewired;

public class MovieSkipOnButton : MonoBehaviour
{

    public PlayableDirector playableDirector;
    public float resetTime;

    Player playerInput;
    float buttonTime;
    float conditionTime = 1f;

    void Start() {
        if (GameManager.Instance) {
            playerInput = GameManager.Instance.playerInput;
        }
    }

    void Update() {
        if (GameManager.Instance) {
            if (playerInput.GetButton(RewiredConsts.Action.Cancel)) {
                buttonTime += Time.unscaledDeltaTime;
                if (buttonTime >= conditionTime) {
                    if (playableDirector) {
                        playableDirector.time = resetTime;
                    }
                    buttonTime = -1000f;
                    enabled = false;
                }
            } else {
                buttonTime = 0f;
            }
        }
    }
}
