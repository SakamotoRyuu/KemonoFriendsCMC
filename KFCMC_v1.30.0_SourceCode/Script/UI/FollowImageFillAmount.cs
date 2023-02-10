using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowImageFillAmount : MonoBehaviour {

    public Image followImage;

    Image thisImage;
    float amountTo;
    float amountFrom;
    float timeRemain;

    private void Awake() {
        thisImage = GetComponent<Image>();
        thisImage.enabled = false;
        amountTo = thisImage.fillAmount = followImage.fillAmount;
    }

    private void LateUpdate() {
        if (Time.timeScale > 0) {
            if (followImage.fillAmount > amountTo) {
                timeRemain = 0f;
                amountTo = followImage.fillAmount;
                if (thisImage.fillAmount != amountTo) {
                    thisImage.fillAmount = amountTo;
                }
                if (thisImage.enabled) {
                    thisImage.enabled = false;
                }
            } else if (followImage.fillAmount < amountTo) {
                timeRemain = 2f;
                if (thisImage.fillAmount != amountTo) {
                    thisImage.fillAmount = amountTo;
                }
                if (!thisImage.enabled) {
                    thisImage.enabled = true;
                }
                amountFrom = amountTo;
                amountTo = followImage.fillAmount;
            } else if (timeRemain > 0f) {
                timeRemain -= Time.deltaTime;
                float diff = 0f;
                if (timeRemain > 0f) {
                    diff = Easing.SineIn(timeRemain, 1f, 0f, amountFrom - amountTo);
                }
                if (diff < 0.001f) {
                    thisImage.enabled = false;
                } else {
                    thisImage.fillAmount = amountTo + diff;
                }
            }
        }
    }

}
