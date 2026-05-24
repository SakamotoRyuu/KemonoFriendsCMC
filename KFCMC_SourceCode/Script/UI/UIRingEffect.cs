using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRingEffect : MonoBehaviour
{
    public Image effectImage;
    public Color effectColor = Color.white;
    public float effectDuration = 1;
    public float effectAlphaStart = 1.5f;
    public float effectAlphaEnd = 0;
    public float effectSizeStart = 20;
    public float effectSizeEnd = 120;

    float elapsedTime;

    void Update()
    {
        if (elapsedTime >= effectDuration)
        {
            effectImage.enabled = false;
            enabled = false;
        }
        else
        {
            effectColor.a = Mathf.Clamp01(Easing.SineIn(elapsedTime, effectDuration, effectAlphaStart, effectAlphaEnd));
            effectImage.color = effectColor;
            float sizeTemp = Easing.CubicOut(elapsedTime, effectDuration, effectSizeStart, effectSizeEnd);
            Vector2 effectSize;
            effectSize.x = effectSize.y = sizeTemp;
            effectImage.rectTransform.sizeDelta = effectSize;
            elapsedTime += Time.deltaTime;
        }
    }

    public void StartEffect()
    {
        elapsedTime = 0;
        effectColor.a = effectAlphaStart;
        effectImage.color = effectColor;
        float sizeTemp = effectSizeStart;
        Vector2 effectSize;
        effectSize.x = effectSize.y = sizeTemp;
        effectImage.rectTransform.sizeDelta = effectSize;
        effectImage.enabled = true;
        enabled = true;
    }

    public void EndEffect()
    {
        effectImage.enabled = false;
        enabled = false;
    }

}
