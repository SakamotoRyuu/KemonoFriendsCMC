using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIImageFillController : MonoBehaviour {

    public enum Direction { Horizontal, Vertical };

    public bool isSelf = true;
    public bool isPlayer = false;
    public CharacterBase cBase;
    public Image hpFillImage;
    public Image stFillImage;
    public TMP_Text hpText;
    public TMP_Text stText;
    public TMP_ColorGradient[] colorGradient;
    public Image frameImage;
    public Image hpDiffImage;
    public bool frameFlashOnCaution = false;
    public bool frameColorOnDamage = false;
    public bool frameVibrateOnDamage = false;
    public bool hpFillVibrateOnDamage = false;
    public float onDamageTime = 0.5f;
    public float vibrateSpeed = 60f;
    public float vibrateAmplitude = 4f;    
    public Direction vibrateDirection;
    public float fillSpaceBias;

    protected int nowHPNum = -1;
    protected int maxHPNum = -1;
    protected int nowHPNumSave;
    protected int maxHPNumSave;
    protected int hpColor = -1;
    protected int hpColorSave;
    protected float nowSTNum = -1;
    protected float maxSTNum = -1;
    protected float nowSTNumSave;
    protected float maxSTNumSave;
    protected int stColor = -1;
    protected int stColorSave;
    protected float diffSave;
    protected float diffTimeRemain;

    protected bool enableST = false;
    protected bool enableColor = false;
    protected double timeStamp;
    protected int frameColor = -1;
    protected float damageTimeRemain = 0;
    protected RectTransform frameRect;
    protected Vector2 frameRectOriginPos;
    protected RectTransform hpFillRect;
    protected Vector2 hpFillRectOriginPos;
    protected const double flashInterval = 0.4;
    protected const float diffTimeMax = 0.6f;
    
    Color normalColorFrame = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    Color dangerColorFrame = new Color(1.0f, 0.5f, 0.5f, 1.0f);
    Color deadColorFrame = new Color(0.5f, 0.5f, 0.5f, 1.0f);

    void Start() {
        if (isSelf) {
            cBase = GetComponentInParent<CharacterBase>();
        } else if (isPlayer) {
            cBase = CharacterManager.Instance.pCon;
        }
        if (frameVibrateOnDamage && frameImage) {
            frameRect = frameImage.gameObject.GetComponent<RectTransform>();
            frameRectOriginPos = frameRect.anchoredPosition;
        }
        if (hpFillVibrateOnDamage && hpFillImage) {
            hpFillRect = hpFillImage.gameObject.GetComponent<RectTransform>();
            hpFillRectOriginPos = hpFillRect.anchoredPosition;
        }
        enableST = (stText || stFillImage);
        enableColor = (colorGradient.Length >= 3);
        timeStamp = GameManager.Instance.time;
    }

    float GetStaminaColorBorder() {
        if (!isPlayer) {
            return maxSTNum * 0.2f;
        } else if (CharacterManager.Instance) {
            return CharacterManager.Instance.staminaBorder;
        } else {
            return maxSTNum * 0.2f;
        }
    }

    void Update() {
        if (cBase) {
            nowHPNumSave = nowHPNum;
            maxHPNumSave = maxHPNum;
            nowHPNum = cBase.GetNowHP();
            maxHPNum = cBase.GetMaxHP();
            if (nowHPNumSave != nowHPNum || maxHPNumSave != maxHPNum) {
                if (hpText) {
                    hpText.text = nowHPNum.ToString();
                    if (enableColor) {
                        hpColorSave = hpColor;
                        hpColor = (nowHPNum >= maxHPNum ? 0 : nowHPNum >= (isPlayer ? CharacterManager.Instance.GetGutsBorder() : maxHPNum * 3 / 10) ? 1 : nowHPNum > 0 ? 2 : 3);
                        if (hpColorSave != hpColor) {
                            hpText.colorGradientPreset = colorGradient[hpColor];
                        }
                    }
                }
                if (hpFillImage && maxHPNum > 0) {
                    float amountTemp = (float)nowHPNum / maxHPNum;
                    if (fillSpaceBias > 0f) {
                        amountTemp = fillSpaceBias + amountTemp * (1f - fillSpaceBias * 2f);
                    }
                    if (hpDiffImage) {
                        if (maxHPNum != maxHPNumSave) {
                            diffTimeRemain = -1;
                            if (hpDiffImage.enabled) {
                                hpDiffImage.enabled = false;
                            }
                        } else {
                            if (damageTimeRemain <= diffTimeMax * 0.5f || hpDiffImage.fillAmount < hpFillImage.fillAmount) {
                                hpDiffImage.fillAmount = hpFillImage.fillAmount;
                            }
                            diffSave = hpDiffImage.fillAmount - amountTemp;
                            if (diffSave > 0f) {
                                diffTimeRemain = diffTimeMax;
                                if (!hpDiffImage.enabled) {
                                    hpDiffImage.enabled = true;
                                }
                            }
                        }
                    }
                    hpFillImage.fillAmount = amountTemp;
                }
            }
            if (enableST) {
                nowSTNumSave = nowSTNum;
                maxSTNumSave = maxSTNum;
                nowSTNum = cBase.GetNowST();
                maxSTNum = cBase.GetMaxST();
                if (stFillImage && maxSTNum > 0 && (nowSTNumSave != nowSTNum || maxSTNumSave != maxSTNum)) {
                    stFillImage.fillAmount = nowSTNum / maxSTNum;
                }
                if (stText && (int)nowSTNumSave != (int)nowSTNum) {
                    stText.text = ((int)nowSTNum).ToString();
                    if (enableColor) {
                        stColorSave = stColor;
                        stColor = (nowSTNum >= maxSTNum ? 0 : nowSTNum >= GetStaminaColorBorder() ? 1 : 2);
                        if (stColorSave != stColor) {
                            stText.colorGradientPreset = colorGradient[stColor];
                        }
                    }
                }
            }
            if (nowHPNum < nowHPNumSave && nowHPNum < maxHPNum) {
                damageTimeRemain = onDamageTime;
            }
            if (diffTimeRemain > 0f) {
                diffTimeRemain -= Time.deltaTime;
                if (hpDiffImage && diffTimeRemain < diffTimeMax * 0.5f) {
                    float diffTemp = diffTimeRemain > 0f ? Easing.SineIn(diffTimeRemain, diffTimeMax * 0.5f, 0f, diffSave) : -1f;
                    if (diffTemp >= 0.001f) {
                        hpDiffImage.fillAmount = hpFillImage.fillAmount + diffTemp;
                    } else {
                        diffTimeRemain = -1f;
                        hpDiffImage.enabled = false;
                    }
                }
            }
            if (damageTimeRemain > 0f) {
                damageTimeRemain -= Time.deltaTime;
                if (damageTimeRemain <= 0f) {
                    if (frameVibrateOnDamage) {
                        frameRect.anchoredPosition = frameRectOriginPos;
                    }
                    if (hpFillVibrateOnDamage) {
                        hpFillRect.anchoredPosition = hpFillRectOriginPos;
                    }
                    if (frameColorOnDamage) {
                        frameImage.color = normalColorFrame;
                        frameColor = 1;
                    }
                } else {
                    if (frameVibrateOnDamage || hpFillVibrateOnDamage) {
                        float posPlus = Mathf.Sin((onDamageTime - damageTimeRemain) * vibrateSpeed) * vibrateAmplitude;
                        if (frameVibrateOnDamage) {
                            frameRect.anchoredPosition = frameRectOriginPos + new Vector2((int)vibrateDirection == 0 ? posPlus : 0, (int)vibrateDirection == 1 ? posPlus : 0);
                        }
                        if (hpFillVibrateOnDamage) {
                            hpFillRect.anchoredPosition = hpFillRectOriginPos + new Vector2((int)vibrateDirection == 0 ? posPlus : 0, (int)vibrateDirection == 1 ? posPlus : 0);
                        }
                    }
                    if (frameColorOnDamage) {
                        if (nowHPNum <= 0) {
                            frameImage.color = deadColorFrame;
                            frameColor = 3;
                        } else {
                            frameImage.color = dangerColorFrame;
                            frameColor = 2;
                        }
                    }
                }
            }
            if (frameFlashOnCaution && frameImage && (!frameVibrateOnDamage || damageTimeRemain <= 0)) {
                if (nowHPNum <= 0) {
                    timeStamp = GameManager.Instance.time;
                    frameImage.color = deadColorFrame;
                    frameColor = 3;
                } else if (nowHPNum >= (isPlayer ? CharacterManager.Instance.GetGutsBorder() : (cBase && cBase.isBoss) ? 2 : maxHPNum * 3 / 10)) {
                    timeStamp = GameManager.Instance.time - flashInterval;
                    if (frameColor != 1) {
                        frameImage.color = normalColorFrame;
                        frameColor = 1;
                    }
                } else if (GameManager.Instance.time >= timeStamp + flashInterval) {
                    timeStamp = GameManager.Instance.time;
                    if (frameColor != 1) {
                        frameImage.color = normalColorFrame;
                        frameColor = 1;
                    } else {
                        frameImage.color = dangerColorFrame;
                        frameColor = 2;
                    }
                }
            }
        } else if (isPlayer && CharacterManager.Instance.pCon) {
            cBase = CharacterManager.Instance.pCon;
        }
    }
}