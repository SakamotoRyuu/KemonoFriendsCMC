using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AdjustOpeningTextByAspectRatio : MonoBehaviour {

    public RectTransform[] rect;
    public TMP_Text[] tmpText;
    public float limitAspectRatio = 1.333333f;

    void Awake() {
        float nowAspectRatio = (float)Screen.width / Screen.height;
        if (nowAspectRatio < limitAspectRatio) {
            float rate = nowAspectRatio / limitAspectRatio;
            Vector2 size;
            for (int i = 0; i < rect.Length; i++) {
                if (rect[i]) {
                    size = rect[i].sizeDelta;
                    size.x *= rate;
                    rect[i].sizeDelta = size;
                }
            }
            for (int i = 0; i < tmpText.Length; i++) {
                if (tmpText[i]) {
                    tmpText[i].fontSize *= rate;
                }
            }
        }
    }

}
