using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Event_OpeningAnother : MonoBehaviour {

    public TMP_Text text;
    private const int pageMax = 13;
    private int pageNow;
    private float elapsedTime;
    private int progress;

    void NextProgress() {
        elapsedTime = 0f;
        progress++;
    }

    void Update() {
        elapsedTime += Time.unscaledDeltaTime;
        switch (progress) {
            case 0:
                if (elapsedTime > 1f) {
                    SetText();
                    NextProgress();
                }
                break;
            case 1:
                if (GameManager.Instance) {
                    if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
                        SetText();                        
                    }
                }
                break;
            case 2:
                if (elapsedTime > 1f) {
                    if (BGM.Instance) {
                        BGM.Instance.Stop();
                    }
                    GameManager.Instance.save.nowStage = 15;
                    GameManager.Instance.save.nowFloor = 0;
                    GameManager.Instance.LoadScene("Play");
                    NextProgress();
                }
                break;
            case 3:
                break;
        }
    }

    void SetText() {
        if (pageNow == pageMax) {
            text.text = "";
            NextProgress();
        } else if (pageNow < pageMax) {
            text.text = TextManager.Get(string.Format("OPENING_ANOTHER_{0:00}", pageNow));
        }
        pageNow++;
        if (pageNow == pageMax) {
            if (BGM.Instance) {
                BGM.Instance.StopFade(2f);
            }
        }
    }
}
