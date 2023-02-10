using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlot : MonoBehaviour {

    public int slotIndex;
    public TMP_Text headerText;
    public Image difficultyImage;
    public TMP_Text levelText;
    public TMP_Text friendsText;
    public TMP_Text[] progressText;
    public Canvas dataExistCanvas;
    public Canvas[] normalCanvas;
    public Canvas[] anotherCanvas;
    public Canvas[] canvasByFormat;
    public TMP_Text anotherClearText;
    public GridLayoutGroup friendsFaceGrid;
    public GridLayoutGroup minmiGrid;
    public Image[] friendsFaceImages;
    public TMP_Text friendsAndMoreText;
    public Image[] minmiImages;
    public TMP_ColorGradient whiteGradient;
    public TMP_ColorGradient rainbowGradient;
    public Sprite[] difficultyIcon;
    public Sprite[] minmiIcon;
    public TMP_Text timeText;
    public TMP_Text trophyText;
    public TMP_Text dateText;
    
    GameManager.Save saveTemp;
    StringBuilder sb = new StringBuilder();
    Vector2 faceSpacing;
    static readonly int[] gridSpacingArray = new int[] { 0, 0, 0, 0, -4, -10};
    const int formatFriends = 0;
    const int formatTime = 1;
    const int formatDate = 2;

    public void SetSlot(int fileIndex, int displayFormat) {
        saveTemp = GameManager.Instance.TempLoad(fileIndex);
        sb.Clear();
        sb.Append(TextManager.Get("WORD_FILE"));
        sb.Append((fileIndex + 1).ToString("00"));
        if (saveTemp != null && saveTemp.dataExist != 0) {
            dataExistCanvas.enabled = true;

            sb.Append(" ");
            if (SaveController.Instance && SaveController.Instance.IsSpecialFloor(saveTemp.nowStage, saveTemp.nowFloor)) {
                sb.Append(TextManager.Get(string.Format("SAVEFLOOR_{0:00}_{1:00}", saveTemp.nowStage, saveTemp.nowFloor)));
            } else {
                sb.Append(TextManager.Get(string.Format("STAGE_{0:00}", saveTemp.nowStage)));
                if (saveTemp.nowFloor + 1 > GameManager.Instance.GetDenotativeMaxFloor(saveTemp.nowStage)) {
                    sb.Append(" ?");
                } else {
                    sb.Append(" ");
                    sb.Append((saveTemp.nowFloor + 1).ToString());
                }
                sb.Append(TextManager.Get("WORD_FLOOR"));
            }
            headerText.text = sb.ToString();
            headerText.colorGradientPreset = (saveTemp.playerType == GameManager.playerType_Hyper || saveTemp.playerType == GameManager.playerType_AnotherEscape ? rainbowGradient : whiteGradient);

            difficultyImage.sprite = difficultyIcon[Mathf.Clamp(saveTemp.difficulty - 1, 0, difficultyIcon.Length - 1)];

            int level = saveTemp.Level;
            levelText.text = level.ToString();
            levelText.colorGradientPreset = level >= 100 ? rainbowGradient : whiteGradient;

            bool playerIsDefault = (saveTemp.playerType != GameManager.playerType_Another && saveTemp.playerType != GameManager.playerType_AnotherEscape);
            if (playerIsDefault) {
                if (displayFormat == formatFriends) {
                    int rescueRate = saveTemp.GetRescueNow() * 100 / GameManager.rescueMax;
                    friendsText.text = string.Format("{0:0}%", rescueRate);
                    friendsText.colorGradientPreset = rescueRate >= 100 ? rainbowGradient : whiteGradient;
                    int livingFriendsCount = 0;
                    for (int i = 0; i < saveTemp.friendsLiving.Length; i++) {
                        if (saveTemp.friendsLiving[i] != 0) {
                            if (livingFriendsCount < friendsFaceImages.Length) {
                                friendsFaceImages[livingFriendsCount].sprite = ItemDatabase.Instance.GetItemImage(ItemDatabase.friendsIndexBottom + i);
                                friendsFaceImages[livingFriendsCount].enabled = true;
                            }
                            livingFriendsCount++;
                        }
                    }
                    if (livingFriendsCount < friendsFaceImages.Length) {
                        for (int i = livingFriendsCount; i < friendsFaceImages.Length; i++) {
                            friendsFaceImages[i].enabled = false;
                        }
                    }
                    if (livingFriendsCount > friendsFaceImages.Length) {
                        friendsAndMoreText.text = sb.Clear().Append(TextManager.Get("WORD_OTHERFRIENDS_START")).Append(livingFriendsCount - friendsFaceImages.Length).Append(TextManager.Get("WORD_OTHERFRIENDS_END")).ToString();
                    } else {
                        friendsAndMoreText.text = "";
                    }
                    faceSpacing.x = gridSpacingArray[Mathf.Clamp(livingFriendsCount, 0, gridSpacingArray.Length - 1)];
                    faceSpacing.y = 0;
                    friendsFaceGrid.spacing = faceSpacing;
                }
                if (displayFormat == formatFriends || displayFormat == formatTime) {
                    sb.Clear().Append(saveTemp.progress);
                    if ((saveTemp.secret & (1 << (int)GameManager.SecretType.SkytreeCleared)) != 0) {
                        sb.Append("+");
                    }
                    for (int i = 0; i < progressText.Length; i++) {
                        progressText[i].text = sb.ToString();
                        progressText[i].colorGradientPreset = saveTemp.progress >= GameManager.gameClearedProgress ? rainbowGradient : whiteGradient;
                    }
                }
                if (displayFormat == formatTime) {
                    int trophyCount = 0;
                    for (int i = 0; i < GameManager.trophyMax; i++) {
                        int arrayNum = i / 32;
                        int bitNum = 1 << (i % 32);
                        if ((saveTemp.trophy[arrayNum] & bitNum) != 0) {
                            trophyCount++;
                        }
                    }
                    trophyText.text = trophyCount.ToString();
                    trophyText.colorGradientPreset = trophyCount >= GameManager.trophyMax ? rainbowGradient : whiteGradient;
                }
            }

            if (displayFormat == formatTime) {
                timeText.text = sb.Clear().Append((saveTemp.totalPlayTime / 3600).ToString("0")).Append(":").Append((saveTemp.totalPlayTime % 3600 / 60).ToString("00")).Append("\'").Append((saveTemp.totalPlayTime % 60).ToString("00")).Append("\"").ToString();
            }
            if (displayFormat == formatDate) {
                if (saveTemp.date == null || saveTemp.date.Length < 6) {
                    saveTemp.date = new int[6];
                }
                dateText.text = string.Format("{0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}", saveTemp.date[0], saveTemp.date[1], saveTemp.date[2], saveTemp.date[3], saveTemp.date[4], saveTemp.date[5]);
            }

            int minmiCount = 0;
            for (int i = 0; i < minmiImages.Length; i++) {
                if (saveTemp.HaveSpecificItem(GameManager.minmiGoldenID - i)) {
                    minmiImages[minmiCount].sprite = minmiIcon[minmiIcon.Length - 1 - i];
                    minmiImages[minmiCount].enabled = true;
                    minmiCount++;
                }
            }
            if (minmiCount < minmiImages.Length) {
                for (int i = minmiCount; i < minmiImages.Length; i++) {
                    minmiImages[i].enabled = false;
                }
            }
            if (anotherClearText) {
                anotherClearText.enabled = !playerIsDefault && saveTemp.progress >= GameManager.gameClearedProgress;
            }
            for (int i = 0; i < normalCanvas.Length; i++) {
                normalCanvas[i].enabled = playerIsDefault;
            }
            for (int i = 0; i < anotherCanvas.Length; i++) {
                anotherCanvas[i].enabled = !playerIsDefault;
            }
            for (int i = 0; i < canvasByFormat.Length; i++) {
                canvasByFormat[i].enabled = i == displayFormat;
            }
        } else {
            dataExistCanvas.enabled = false;
            for (int i = 0; i < minmiImages.Length; i++) {
                minmiImages[i].enabled = false;
            }
            headerText.text = sb.ToString();
        }
    }

    public void EventEnter() {
        if (SaveController.Instance) {
            SaveController.Instance.EventEnter(slotIndex);
        }
    }

    public void EventClick() {
        if (SaveController.Instance) {
            SaveController.Instance.EventClick(slotIndex);
        }
    }

}
