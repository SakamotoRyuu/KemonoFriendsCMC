using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using Rewired;

public class TutorialSlot_ControllerMapping : TutorialSlot {
    
    public TMP_Text[] actionTexts;
    public Image wildReleaseButton;
    const int wildReleaseIndex = 10;
    
    static readonly string[] actionNames = new string[] {
        "MOVE",
        "SUBMIT",
        "CANCEL",
        "ATTACK",
        "JUMP",
        "DODGE",
        "SPECIAL",
        "AIM",
        "CHANGETARGET",
        "PAUSE",
        "WILDRELEASE"
    };

    protected override void Awake() {
        base.Awake();
        if (GameManager.Instance && GameManager.Instance.playerInput != null) {
            for (int i = 0; i < actionNames.Length && i < actionTexts.Length; i++) {
                actionTexts[i].text = TextManager.Get(sb.Clear().Append("CONTROL_").Append(actionNames[i]).ToString());
            }
            bool wildReleaseEnabled = GameManager.Instance.save.equip[GameManager.sandstarCondition] != 0;
            actionTexts[wildReleaseIndex].enabled = wildReleaseEnabled;
            wildReleaseButton.enabled = wildReleaseEnabled;
        }
    }

}
