using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeOutText : MonoBehaviour {
    
    public TMP_Text fadeOutText;
    public RectTransform rectTrans;
    
    private float delayTime;
    private float speed;
    private float alphaSpeed;
    private float alphaParam;
    private Color colorTemp = new Color(1f, 1f, 1f, 1f);
    private float deltaTimeCache;
    static readonly private Vector3 vecUp = Vector3.up;

    void Update() {
        if (Time.timeScale > 0f) {
            deltaTimeCache = Time.deltaTime;
            if (delayTime > 0) {
                delayTime -= deltaTimeCache;
            } else {
                if (rectTrans) {
                    rectTrans.Translate(speed * deltaTimeCache * vecUp);
                }
                if (fadeOutText) {
                    alphaParam -= deltaTimeCache * alphaSpeed;
                    if (alphaParam <= 0f) {
                        Destroy(gameObject);
                    } else {
                        colorTemp.a = alphaParam;
                        fadeOutText.color = colorTemp;
                    }
                }
            }
        }
    }

    public void SetText(string text, Vector2 position, float delayTime = 0f, float lifeTime = 1f, float speed = 100f) {
        this.delayTime = delayTime;
        this.speed = speed;
        if (lifeTime > 0f) {
            alphaSpeed = 1f / lifeTime;
        } else {
            alphaSpeed = 10000f;
        }
        alphaParam = 1f;
        if (fadeOutText) {
            fadeOutText.text = text;
        }
        if (rectTrans) {
            rectTrans.position = position;
        }
    }
}
