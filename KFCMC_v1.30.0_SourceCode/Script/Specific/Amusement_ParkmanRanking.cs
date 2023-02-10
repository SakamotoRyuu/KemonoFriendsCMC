using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Amusement_ParkmanRanking : MonoBehaviour {

    public TMP_Text[] textRank;
    public TMP_Text[] textScore;
    public TMP_Text[] textName;
    public int[] rankingFriendsID;
    public int[] rankingScore;
    public Image normalLogo;
    public Image hardLogo;

    void Awake() {
        bool purpleFlag = GameManager.Instance.minmiPurple;
        int playerScore = (purpleFlag ? GameManager.Instance.save.parkmanScoreHard : GameManager.Instance.save.parkmanScore);
        int nowRank = 0;
        bool hitFlag = false;
        for (int i = 0; i < rankingFriendsID.Length && nowRank < textRank.Length; i++) {
            if (GameManager.Instance.save.friends[rankingFriendsID[i]] != 0) {
                if (!hitFlag && playerScore >= rankingScore[i]) {
                    SetPlayerRank(nowRank, playerScore);
                    hitFlag = true;
                    nowRank++;
                }
                textRank[nowRank].text = TextManager.Get("AMUSEMENT_RANKING_" + (nowRank + 1).ToString("00"));
                textScore[nowRank].text = string.Format("{0:#,0}", rankingScore[i]);
                textName[nowRank].text = TextManager.Get("ITEM_NAME_1" + rankingFriendsID[i].ToString("00"));
                nowRank++;
            }
        }
        if (!hitFlag && playerScore > 0 && nowRank < textRank.Length) {
            SetPlayerRank(nowRank, playerScore);
        }
        normalLogo.enabled = !purpleFlag;
        hardLogo.enabled = purpleFlag;
    }

    void SetPlayerRank(int rank, int playerScore) {
        textRank[rank].colorGradientPreset = textScore[rank].colorGradientPreset = textName[rank].colorGradientPreset = (GameManager.Instance.minmiPurple ? CharacterManager.Instance.parkmanSettings.scoreHardColor : CharacterManager.Instance.parkmanSettings.scoreNormalColor);
        textRank[rank].text = TextManager.Get("AMUSEMENT_RANKING_" + (rank + 1).ToString("00"));
        textScore[rank].text = string.Format("{0:#,0}", playerScore);
        textName[rank].text = TextManager.Get("ITEM_NAME_132");
    }
}
