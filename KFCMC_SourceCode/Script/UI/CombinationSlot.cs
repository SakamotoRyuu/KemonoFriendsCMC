using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class CombinationSlot : MonoBehaviour {

    public TMP_Text headerText;
    public GridLayoutGroup friendsFaceGrid;
    public Image[] friendsFaceImages;
    public TMP_Text friendsAndMoreText;
    public TMP_Text cautionText;
    public TMP_ColorGradient zeroColor;
    public TMP_ColorGradient normalColor;
    public TMP_ColorGradient overColor;
    
    Vector2 faceSpacing;
    StringBuilder sb = new StringBuilder();
    static readonly int[] gridSpacingArray = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -5, -8 };

    public void SetSlot(int index, int costLimit, int japaribunCount) {
        if (index >= 0 && index < GameManager.Instance.save.friendsCombination.Length) {
            int friendsCount = 0;
            int costSum = 0;
            int requiredCost = 0;
            int flag = GameManager.Instance.save.friendsCombination[index];
            for (int i = 1; i < GameManager.friendsMax; i++) {
                if ((flag & (1 << (i - 1))) != 0) {
                    if (friendsCount < friendsFaceImages.Length) {
                        friendsFaceImages[friendsCount].sprite = ItemDatabase.Instance.GetItemImage(ItemDatabase.friendsIndexBottom + i);
                        friendsFaceImages[friendsCount].enabled = true;
                    }
                    costSum += CharacterDatabase.Instance.friends[i].cost;
                    if (!CharacterManager.Instance.GetFriendsExist(i, true)) {
                        requiredCost += CharacterDatabase.Instance.friends[i].cost;
                    }
                    friendsCount++;
                }
            }
            if (friendsCount < friendsFaceImages.Length) {
                for (int i = friendsCount; i < friendsFaceImages.Length; i++) {
                    friendsFaceImages[i].enabled = false;
                }
            }
            headerText.text = sb.Clear().Append(TextManager.Get("WORD_COMBI_TOTALCOST")).Append(" ").Append(costSum).ToString();
            if (costSum <= 0) {
                headerText.colorGradientPreset = zeroColor;
            } else if (costSum > CharacterManager.riskyCost) {
                headerText.colorGradientPreset = overColor;
            } else {
                headerText.colorGradientPreset = normalColor;
            }
            if (costSum > costLimit) {
                cautionText.text = TextManager.Get("WORD_COMBI_COSTOVER");
            } else if (requiredCost > japaribunCount) {
                cautionText.text = TextManager.Get("WORD_COMBI_LACK");
            } else {
                cautionText.text = "";
            }
            if (friendsCount > friendsFaceImages.Length) {
                friendsAndMoreText.text = sb.Clear().Append(TextManager.Get("WORD_OTHERFRIENDS_START")).Append(friendsCount - friendsFaceImages.Length).Append(TextManager.Get("WORD_OTHERFRIENDS_END")).ToString();
            } else {
                friendsAndMoreText.text = "";
            }
            faceSpacing.x = gridSpacingArray[Mathf.Clamp(friendsCount, 0, gridSpacingArray.Length - 1)];
            faceSpacing.y = 0;
            friendsFaceGrid.spacing = faceSpacing; ;
        }
    }

}
