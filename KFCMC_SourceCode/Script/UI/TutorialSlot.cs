using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class TutorialSlot : MonoBehaviour {

    public int tutorialNumber;
    public TMP_Text headText;
    public TMP_Text bodyText;
    public TMP_Text[] subTexts;
    protected StringBuilder sb = new StringBuilder();

    protected virtual void Awake() {
        string header = sb.Clear().AppendFormat("TUTORIAL_{0:00}", tutorialNumber).ToString();
        if (headText) {
            headText.text = TextManager.Get(sb.Clear().Append(header).Append("_HEAD").ToString());
        }
        if (bodyText) {
            bodyText.text = TextManager.Get(sb.Clear().Append(header).Append("_BODY").ToString());
        }
        for (int i = 0; i < subTexts.Length; i++) {
            if (subTexts[i]) {
                subTexts[i].text = TextManager.Get(sb.Clear().Append(header).AppendFormat("_SUB_{0:00}", i).ToString());
            }
        }
    }

}
