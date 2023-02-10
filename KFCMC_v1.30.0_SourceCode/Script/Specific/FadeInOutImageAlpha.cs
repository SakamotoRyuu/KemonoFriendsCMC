using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOutImageAlpha : MonoBehaviour {

    public Image image;
    public float startAlpha;
    public float endAlpha;
    public float fadeTime;
    public float delayTime;

    private float elapsedTime;

    void Update() {
        elapsedTime += Time.deltaTime;
        Color colorTemp = image.color;
        if (elapsedTime > delayTime) {
            if (fadeTime > 0) {
                colorTemp.a = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01((elapsedTime - delayTime) / fadeTime));
            } else {
                colorTemp.a = endAlpha;
            }
            image.color = colorTemp;
            if (elapsedTime >= delayTime + fadeTime) {
                enabled = false;
            }
        }
    }
}
