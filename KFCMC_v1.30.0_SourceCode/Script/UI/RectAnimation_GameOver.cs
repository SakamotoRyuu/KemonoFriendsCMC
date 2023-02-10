using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RectAnimation_GameOver : MonoBehaviour {
    
    RectTransform rect;
    Image image;
    float randomValue;
    Vector2 origin;
    float tweenValue1 = 0f;
    float tweenValue2 = 0f;
    
    void Start() {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        randomValue = Random.Range(-1f, 1f);
        AnimationStart1();
        origin = rect.anchoredPosition;
    }

    void AnimationStart1() {
        DOTween.To(() => tweenValue1, x => tweenValue1 = x, 1f, 0.8f).SetEase(Ease.InSine).OnUpdate(AnimationUpdate1).OnComplete(AnimationStart2);
    }

    void AnimationUpdate1() {
        if (image && rect) {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f + tweenValue1 * 0.5f);
            rect.sizeDelta = new Vector2(300 - tweenValue1 * 200, 300 - tweenValue1 * 200);
        }
    }

    void AnimationStart2() {
        DOTween.To(() => tweenValue2, x => tweenValue2 = x, 1f, 2.5f).SetEase(Ease.Linear).SetDelay(0.8f).OnUpdate(AnimationUpdate2).OnComplete(AnimationComplete);
    }

    private void AnimationComplete() {
        DOTween.KillAll();
    }

    private void OnDestroy() {
        DOTween.KillAll();
    }

    void AnimationUpdate2() {
        rect.eulerAngles = new Vector3(0, 0, tweenValue2 * 400 * randomValue);
        rect.anchoredPosition = new Vector2(origin.x, origin.y - tweenValue2 * 400);
    }

}
