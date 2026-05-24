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
    public Image knockFillImage;
    public TMP_Text hpText;
    public TMP_Text stText;
    public TMP_ColorGradient[] colorGradient;
    public Image frameImage;
    public Image hpDiffImage;
    public Image knockDiffImage;
    public Image knockRecoveryImage;
    public bool frameFlashOnCaution = false;
    public bool frameColorOnDamage = false;
    public RectTransform[] vibrateRects;
    public float onDamageTime = 0.5f;
    public float vibrateSpeed = 60f;
    public float vibrateAmplitude = 4f;    
    public Direction vibrateDirection;
    public float fillSpaceBias;
    // Guts
    public Image gutsFillImage;
    public Image gutsEnabledBackImage;
    public Image gutsDisabledBackImage;
    // Chimera
    public Image chimeraFillImage;
    public Canvas knockCanvas;

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
    protected float hpDiffSave;
    protected float hpDiffTimeRemain;

    // Knockゲージ関連
    protected float nowKnockNum = -1;
    protected float maxKnockNum = -1;
    protected float nowKnockNumSave;
    protected float maxKnockNumSave;
    protected float knockDiffSave;
    protected float knockDiffTimeRemain;

    // Guts関連
    protected float gutsInvRateSave = -1;
    protected Color gutsNormalColor = new Color(0.4f, 0.4f, 0.4f, 1f);
    protected Color gutsZeroColor = Color.black;

    protected bool enableST = false;
    protected bool enableColor = false;
    protected double timeStamp;
    protected int frameColor = -1;
    protected float damageTimeRemain = 0;
    protected Vector2[] vibrateRectOriginPositions;
    protected const double flashInterval = 0.4;
    protected const float diffTimeMax = 0.6f;
    
    Color normalColorFrame = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    Color dangerColorFrame = new Color(1.0f, 0.5f, 0.5f, 1.0f);
    Color deadColorFrame = new Color(0.5f, 0.5f, 0.5f, 1.0f);

    void Awake()
    {
        vibrateRectOriginPositions = new Vector2[vibrateRects.Length];
        for (int i = 0; i < vibrateRects.Length; i++)
        {
            vibrateRectOriginPositions[i] = vibrateRects[i].anchoredPosition;
        }
    }

    void Start() {
        if (isSelf) {
            cBase = GetComponentInParent<CharacterBase>();
        } else if (isPlayer) {
            cBase = CharacterManager.Instance.pCon;
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
                    float amountTemp = Mathf.Clamp01((float)nowHPNum / maxHPNum);
                    if (fillSpaceBias > 0f && amountTemp > 0 && amountTemp < 1) {
                        amountTemp = fillSpaceBias + amountTemp * (1f - fillSpaceBias * 2f);
                    }
                    if (hpDiffImage) {
                        if (maxHPNum != maxHPNumSave) {
                            hpDiffTimeRemain = -1;
                            if (hpDiffImage.enabled) {
                                hpDiffImage.enabled = false;
                            }
                        } else {
                            if (damageTimeRemain <= diffTimeMax * 0.5f || hpDiffImage.fillAmount < hpFillImage.fillAmount) {
                                hpDiffImage.fillAmount = hpFillImage.fillAmount;
                            }
                            hpDiffSave = hpDiffImage.fillAmount - amountTemp;
                            if (hpDiffSave > 0f) {
                                hpDiffTimeRemain = diffTimeMax;
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

            // Knockゲージ
            if (knockFillImage)
            {
                nowKnockNumSave = nowKnockNum;
                maxKnockNumSave = maxKnockNum;
                nowKnockNum = cBase.GetKnockGaugeNow();
                maxKnockNum = cBase.GetKnockGaugeMax();
                bool knockRecoveryGaugeEnabled = cBase.KnockRecoveryGaugeEnabled();
                if (knockRecoveryGaugeEnabled)
                {
                    if (knockRecoveryImage)
                    {
                        knockRecoveryImage.fillAmount = cBase.GetKnockRecoveryGaugeRate();
                        if (!knockRecoveryImage.enabled)
                        {
                            knockRecoveryImage.enabled = true;
                        }
                        if (knockFillImage.enabled)
                        {
                            knockFillImage.enabled = false;
                        }
                    }
                }
                else
                {
                    if (knockRecoveryImage)
                    {
                        if (knockRecoveryImage.enabled)
                        {
                            knockRecoveryImage.enabled = false;
                        }
                        if (!knockFillImage.enabled)
                        {
                            knockFillImage.enabled = true;
                        }
                    }
                }
                if (nowKnockNumSave != nowKnockNum || maxKnockNumSave != maxKnockNum)
                {
                    if (knockFillImage && maxKnockNum > 0)
                    {
                        float amountTemp = Mathf.Clamp01(nowKnockNum / maxKnockNum);
                        if (fillSpaceBias > 0f && amountTemp > 0 && amountTemp < 1)
                        {
                            amountTemp = fillSpaceBias + amountTemp * (1f - fillSpaceBias * 2f);
                        }
                        if (knockDiffImage)
                        {
                            if (maxKnockNum != maxKnockNumSave)
                            {
                                knockDiffTimeRemain = -1;
                                if (knockDiffImage.enabled)
                                {
                                    knockDiffImage.enabled = false;
                                }
                            }
                            else
                            {
                                if (damageTimeRemain <= diffTimeMax * 0.5f || knockDiffImage.fillAmount < knockFillImage.fillAmount)
                                {
                                    knockDiffImage.fillAmount = knockFillImage.fillAmount;
                                }
                                knockDiffSave = knockDiffImage.fillAmount - amountTemp;
                                if (knockDiffSave > 0f)
                                {
                                    knockDiffTimeRemain = diffTimeMax;
                                    if (!knockDiffImage.enabled)
                                    {
                                        knockDiffImage.enabled = true;
                                    }
                                }
                            }
                        }
                        knockFillImage.fillAmount = amountTemp;
                    }
                }
            }

            if (nowHPNum < nowHPNumSave && nowHPNum < maxHPNum) {
                damageTimeRemain = onDamageTime;
            }

            // HP差分
            if (hpDiffTimeRemain > 0f) {
                hpDiffTimeRemain -= Time.deltaTime;
                if (hpDiffImage && hpDiffTimeRemain < diffTimeMax * 0.5f) {
                    float diffTemp = hpDiffTimeRemain > 0f ? Easing.SineIn(hpDiffTimeRemain, diffTimeMax * 0.5f, 0f, hpDiffSave) : -1f;
                    if (diffTemp >= 0.001f) {
                        hpDiffImage.fillAmount = hpFillImage.fillAmount + diffTemp;
                    } else {
                        hpDiffTimeRemain = -1f;
                        hpDiffImage.enabled = false;
                    }
                }
            }

            // Knock差分
            if (knockDiffTimeRemain > 0f)
            {
                knockDiffTimeRemain -= Time.deltaTime;
                if (knockDiffImage && knockDiffTimeRemain < diffTimeMax * 0.5f)
                {
                    float diffTemp = knockDiffTimeRemain > 0f ? Easing.SineIn(knockDiffTimeRemain, diffTimeMax * 0.5f, 0f, knockDiffSave) : -1f;
                    if (diffTemp >= 0.001f)
                    {
                        knockDiffImage.fillAmount = knockFillImage.fillAmount + diffTemp;
                    }
                    else
                    {
                        knockDiffTimeRemain = -1f;
                        knockDiffImage.enabled = false;
                    }
                }
            }

            if (damageTimeRemain > 0f) {
                damageTimeRemain -= Time.deltaTime;
                if (damageTimeRemain <= 0f) {
                    for (int i = 0; i < vibrateRects.Length; i++)
                    {
                        vibrateRects[i].anchoredPosition = vibrateRectOriginPositions[i];
                    }
                    if (frameColorOnDamage) {
                        frameImage.color = normalColorFrame;
                        frameColor = 1;
                    }
                } else {
                    if (vibrateRects.Length > 0) {
                        float posPlus = Mathf.Sin((onDamageTime - damageTimeRemain) * vibrateSpeed) * vibrateAmplitude;
                        Vector2 posShiftVector = new Vector2((int)vibrateDirection == 0 ? posPlus : 0, (int)vibrateDirection == 1 ? posPlus : 0);
                        for (int i = 0; i < vibrateRects.Length; i++)
                        {
                            vibrateRects[i].anchoredPosition = vibrateRectOriginPositions[i] + posShiftVector;
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
            if (frameFlashOnCaution && frameImage && (vibrateRects.Length == 0 || damageTimeRemain <= 0)) {
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

            // Guts
            if (gutsFillImage)
            {
                float gutsInvRate = 1 - cBase.GetGutsRate();
                if (gutsInvRate != gutsInvRateSave)
                {
                    gutsInvRateSave = gutsInvRate;
                    gutsFillImage.fillAmount = gutsInvRate;
                    if (gutsInvRate >= 1)
                    {
                        gutsFillImage.color = gutsZeroColor;
                    }
                    else
                    {
                        gutsFillImage.color = gutsNormalColor;
                    }
                    bool gutsInvEnabled = gutsInvRate > 0;
                    if (gutsFillImage.enabled != gutsInvEnabled)
                    {
                        gutsFillImage.enabled = gutsInvEnabled;
                    }
                    if (gutsEnabledBackImage && gutsDisabledBackImage)
                    {
                        if (gutsEnabledBackImage.enabled != gutsInvEnabled)
                        {
                            gutsEnabledBackImage.enabled = gutsInvEnabled;
                        }
                        if (gutsDisabledBackImage.enabled != !gutsInvEnabled)
                        {
                            gutsDisabledBackImage.enabled = !gutsInvEnabled;
                        }
                    }
                }
            }

            // Chimera
            if (chimeraFillImage && knockCanvas)
            {
                bool chimeraEnabled = cBase.ChimeraGaugeEnabled();
                if (chimeraFillImage.enabled != chimeraEnabled)
                {
                    chimeraFillImage.enabled = chimeraEnabled;
                }
                if (knockCanvas.enabled != !chimeraEnabled)
                {
                    knockCanvas.enabled = !chimeraEnabled;
                }
                if (chimeraEnabled)
                {
                    chimeraFillImage.fillAmount = cBase.GetChimeraGaugeRate();
                }
            }

        } else if (isPlayer && CharacterManager.Instance.pCon) {
            cBase = CharacterManager.Instance.pCon;
        }
    }
}