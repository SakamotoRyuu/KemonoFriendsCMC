using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ending_ClearResult : MonoBehaviour {

    public TMP_Text clearTimeCaption;
    public TMP_Text clearTimeBody;
    public TMP_Text rescueRateCaption;
    public TMP_Text rescueRateBody;
    public TMP_Text difficultyCaption;
    public Image difficultyIcon;
    public TMP_Text difficultyPlus;
    public Sprite[] difficultySprites;
    public TMP_Text defeatCountCaption;
    public TMP_Text defeatCountBody;
    public TMP_Text secretBody;
    public TMP_ColorGradient normalColor;
    public TMP_ColorGradient completeColor;
    public TMP_ColorGradient[] difficultyColor;
    public bool isAnother;

    private void Start() {
        if (TextManager.IsInitialized && GameManager.Instance) {
            int minDifficulty = GameManager.Instance.GetGameClearDifficulty();
            if (difficultyCaption && difficultyIcon && difficultyPlus) {
                if (minDifficulty >= 1) {
                    difficultyCaption.text = TextManager.Get("WORD_OVERALLDIFFICULTY");
                    difficultyIcon.sprite = difficultySprites[Mathf.Clamp(minDifficulty - 1, 0, difficultySprites.Length - 1)];
                    difficultyIcon.enabled = true;
                    if (GameManager.Instance.GetSecret(GameManager.SecretType.SkytreeCleared)) {
                        difficultyPlus.text = "+";
                        difficultyPlus.colorGradientPreset = difficultyColor[Mathf.Clamp(GameManager.Instance.save.clearDifficulty[14] - 1, 0, difficultyColor.Length - 1)];
                    } else {
                        difficultyPlus.text = "";
                    }
                } else {
                    difficultyCaption.text = "";
                    difficultyIcon.enabled = false;
                    difficultyPlus.text = "";
                }
            }

            if (clearTimeCaption && clearTimeBody) {
                clearTimeCaption.text = TextManager.Get("WORD_PLAYTIME");
                int playTime = GameManager.Instance.save.totalPlayTime;
                clearTimeBody.text = string.Format("{0:0}:{1:00}\'{2:00}\"", playTime / 3600, playTime % 3600 / 60, playTime % 60);
                int conditionTime = 0;
                if (GameManager.Instance.IsPlayerAnother) {
                    if (minDifficulty <= 2) {
                        conditionTime = 7200;
                    } else {
                        conditionTime = 10800;
                    }
                } else {
                    if (minDifficulty <= 2) {
                        conditionTime = 14400;
                    } else {
                        conditionTime = 18000;
                    }
                }
                if (playTime <= conditionTime) {
                    clearTimeBody.colorGradientPreset = completeColor;
                } else {
                    clearTimeBody.colorGradientPreset = normalColor;
                }
            }

            if (rescueRateCaption && rescueRateBody) {
                rescueRateCaption.text = TextManager.Get("WORD_RESCUERATE");
                int rescueNow = GameManager.Instance.save.GetRescueNow();
                int rescueMax = GameManager.rescueMax;
                rescueRateBody.text = string.Format("{0:0}%", rescueNow * 100 / rescueMax);
                if (rescueNow >= rescueMax) {
                    rescueRateBody.colorGradientPreset = completeColor;
                } else {
                    rescueRateBody.colorGradientPreset = normalColor;
                }
            }

            if (defeatCountCaption && defeatCountBody && CharacterDatabase.Instance) {
                defeatCountCaption.text = TextManager.Get("WORD_DEFEATCOUNT");
                defeatCountBody.text = GameManager.Instance.GetDefeatSum().ToString();
                if (GameManager.Instance.GetDefeatSinWRComplete()) {
                    defeatCountBody.colorGradientPreset = completeColor;
                } else {
                    defeatCountBody.colorGradientPreset = normalColor;
                }
            }

            if (secretBody) {
                secretBody.text = TextManager.Get(isAnother ? "SECRET_ENDING_ANOTHER" : "SECRET_ENDING");
            }

        }
    }

}
