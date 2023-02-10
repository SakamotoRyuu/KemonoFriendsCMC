using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class Amusement_ResultText : MonoBehaviour {

    public TMP_Text titleText;
    public TMP_Text[] rankText;
    public TMP_Text timeHeaderText;
    public TMP_Text timeBodyText;
    public TMP_Text newRecordText;
    public TMP_Text minmiRecordText;
    public Image difficultyIcon;
    public Sprite[] difficultySprites;
    public Image[] minmiIcons;
    public Sprite[] minmiSprites;
    public TMP_Text statusNameText;
    public TMP_Text statusParamText;

    StringBuilder sb = new StringBuilder();
    const int infernalRank = 4;

    public void SetText(string rankDicKey, int rank, int timeRaw, int progressNow, int progressMax, int newRecordFlag, int hpSave, float stSave, float sandstarSave) {
        int minmiIndex = 0;
        titleText.text = TextManager.Get("AMUSEMENT_ZAKORUSH");
        if (rank >= 0 && rank < rankText.Length) {
            if (CharacterManager.Instance.mogisenMultiPlay) {
                rankText[rank].fontSize = 36;
                rankText[rank].text = sb.Clear().Append(TextManager.Get(rankDicKey)).AppendLine().Append("<size=50%>").Append(TextManager.Get("AMUSEMENT_MULTI")).Append("</size>").ToString();
            } else {
                rankText[rank].fontSize = 48;
                rankText[rank].text = TextManager.Get(rankDicKey);
            }
            rankText[rank].gameObject.SetActive(true);
        }
        if (timeRaw > 0) {
            int timeInteger = timeRaw / 100;
            int timeDecimal = timeRaw % 100;
            timeHeaderText.text = TextManager.Get("AMUSEMENT_RECORD");
            timeBodyText.text = sb.Clear().Append((timeInteger / 60).ToString("00")).Append("\'").Append((timeInteger % 60).ToString("00")).Append("\"").Append(timeDecimal.ToString("00")).ToString();
            if (newRecordFlag == 1) {
                newRecordText.text = TextManager.Get("AMUSEMENT_NEWRECORD");
                newRecordText.gameObject.SetActive(true);
            }
        } else {
            timeHeaderText.text = TextManager.Get("AMUSEMENT_PROGRESS");
            timeBodyText.text = sb.Clear().AppendFormat("{0} / {1}", progressNow, progressMax).ToString();
        }
        if (newRecordFlag == 2) {
            minmiRecordText.text = TextManager.Get("AMUSEMENT_MINMIRECORD");
            minmiRecordText.gameObject.SetActive(true);
        }
        difficultyIcon.sprite = difficultySprites[Mathf.Clamp(GameManager.Instance.save.difficulty - 1, 0, difficultySprites.Length)];
        if (GameManager.Instance.minmiBlue) {
            minmiIcons[minmiIndex].sprite = minmiSprites[0];
            minmiIcons[minmiIndex].gameObject.SetActive(true);
            minmiIndex++;
        }
        if (GameManager.Instance.minmiRed) {
            minmiIcons[minmiIndex].sprite = minmiSprites[1];
            minmiIcons[minmiIndex].gameObject.SetActive(true);
            minmiIndex++;
        }
        if (GameManager.Instance.minmiPurple) {
            minmiIcons[minmiIndex].sprite = minmiSprites[2];
            minmiIcons[minmiIndex].gameObject.SetActive(true);
            minmiIndex++;
        }
        if (GameManager.Instance.minmiBlack) {
            minmiIcons[minmiIndex].sprite = minmiSprites[3];
            minmiIcons[minmiIndex].gameObject.SetActive(true);
            minmiIndex++;
        }
        if (GameManager.Instance.minmiSilver) {
            minmiIcons[minmiIndex].sprite = minmiSprites[4];
            minmiIcons[minmiIndex].gameObject.SetActive(true);
            minmiIndex++;
        }
        if (GameManager.Instance.minmiGolden) {
            minmiIcons[minmiIndex].sprite = minmiSprites[5];
            minmiIcons[minmiIndex].gameObject.SetActive(true);
            minmiIndex++;
        }
        statusNameText.text = sb.Clear().
            Append(TextManager.Get("AMUSEMENT_RECEIVEDDAMAGE")).AppendLine().
            Append(TextManager.Get("STATUS_LEVEL")).AppendLine().
            Append(TextManager.Get("STATUS_HP")).AppendLine().
            Append(TextManager.Get("STATUS_ST")).AppendLine().
            Append(TextManager.Get("STATUS_SANDSTAR")).AppendLine().
            Append(TextManager.Get("STATUS_ATTACK")).AppendLine().
            Append(TextManager.Get("STATUS_DEFENSE")).AppendLine().
            Append(TextManager.Get("AMUSEMENT_ARMS")).ToString();
        PlayerController pCon = CharacterManager.Instance.pCon;
        if (pCon) {
            statusParamText.text = sb.Clear().
                AppendFormat("{0} ({1} {2})", CharacterManager.Instance.playerDamageSum, CharacterManager.Instance.playerDamageCount, TextManager.Get("AMUSEMENT_TIMES")).AppendLine().
                Append(GameManager.Instance.GetLevelNow.ToString()).AppendLine().
                AppendFormat("{0} / {1}", hpSave, pCon.GetMaxHP()).AppendLine().
                AppendFormat("{0} / {1}", (int)stSave, (int)pCon.GetMaxST()).AppendLine().
                AppendFormat("{0:0.00} / {1}", sandstarSave, GameManager.Instance.save.SandstarMax).AppendLine().
                AppendFormat("{0:0.00}", pCon.GetAttackNoEffected()).AppendLine().
                AppendFormat("{0:0.00}", pCon.GetDefenseNoEffected()).AppendLine().
                Append(TextManager.Get("AMUSEMENT_ARMSRANK_" + CharacterManager.Instance.GetEquipedWeaponRank().ToString())).ToString();
        }
    }

}
