using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIOperationCanvas_Scaling_TwoTone : UIOperationCanvas_Scaling {
    
    public RectTransform frame;
    public RectTransform backSimple;
    public RectTransform backLeft;
    public RectTransform backRight;
    public Sprite[] leftSprite;
    public Sprite[] rightSprite;
    public TMP_Text uiText;

    protected Image frameImage;
    protected Image backSimpleImage;
    protected Image backLeftImage;
    protected Image backRightImage;
    protected Vector2 frameSizeOffset = new Vector2(40f, 30f);
    protected Vector2 frameSizeRate = new Vector2(1f, 1.2f);
    protected Vector2 frameSizeMax = new Vector2(340f, 140f);
    protected const float sizeRate = 0.6f / 140f;

    public void SetFrameColor(Color color1, Color color2, int twoToneType, float frameColorAlpha = 0.7f, float backColorAlpha = 0.5f) {
        frameImage = frame.GetComponent<Image>();
        backSimpleImage = backSimple.GetComponent<Image>();
        backLeftImage = backLeft.GetComponent<Image>();
        backRightImage = backRight.GetComponent<Image>();
        if (twoToneType >= 0 && twoToneType < leftSprite.Length && twoToneType < rightSprite.Length) {
            backLeftImage.sprite = leftSprite[twoToneType];
            backRightImage.sprite = rightSprite[twoToneType];
        }
        Color colorFrameTemp = color1;
        Color color1Temp = color1;
        Color color2Temp = color2;
        colorFrameTemp.a = frameColorAlpha;
        color1Temp.a = color2Temp.a = backColorAlpha;
        frameImage.color = colorFrameTemp;
        backSimpleImage.color = backLeftImage.color = color1Temp;
        backRightImage.color = color2Temp;
    }

    public void SetText(string text, float displayTime = 3, bool twoTone = true, bool sfx = true) {
        uiText.text = text;
        Vector2 size = new Vector2(Mathf.Min(uiText.preferredWidth * frameSizeRate.x + frameSizeOffset.x, frameSizeMax.x), Mathf.Min(uiText.preferredHeight * frameSizeRate.y + frameSizeOffset.y, frameSizeMax.y)) * sizeRate;
        frame.sizeDelta = size;
        backSimple.sizeDelta = size;
        backLeft.sizeDelta = size;
        backRight.sizeDelta = size;
        backSimpleImage.enabled = !twoTone;
        backLeftImage.enabled = twoTone;
        backRightImage.enabled = twoTone;
    }

}
