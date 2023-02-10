using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetTextAndFadeInOut : MonoBehaviour
{

    public TMP_Text text;
    public string key;
    public float fadeInStartTime;
    public float fadeInDuration;
    public float fadeOutStartTime;
    public float fadeOutDuration;

    float elapsedTime;
    Color colorSave;

    private void Start() {
        if (text) {
            if (!string.IsNullOrEmpty(key)) {
                text.text = TextManager.Get(key);
            }
            colorSave = text.color;
            if (fadeInStartTime <= 0f && fadeInDuration <= 0f) {
                colorSave.a = 1f;
            } else {
                colorSave.a = 0f;
            }
            text.color = colorSave;
            text.enabled = true;
        }
    }

    void Update()
    {
        if (text) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > fadeOutStartTime) {
                if (fadeOutDuration > 0f && (elapsedTime - fadeOutStartTime) < fadeOutDuration) {
                    colorSave.a = 1f - (elapsedTime - fadeOutStartTime) / fadeOutDuration;
                } else {
                    colorSave.a = 0f;
                    text.enabled = false;
                    enabled = false;
                }
                text.color = colorSave;
            } else if (elapsedTime > fadeInStartTime) {
                if (fadeInDuration > 0f && (elapsedTime - fadeInStartTime) < fadeInDuration) {
                    colorSave.a = (elapsedTime - fadeOutStartTime) / fadeOutDuration;
                } else {
                    colorSave.a = 1f;
                }
                text.color = colorSave;
            }
        }
    }
}
