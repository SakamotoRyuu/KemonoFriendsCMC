using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoDoor : MonoBehaviour {

    public string conditionTag = "ItemGetter";
    public GameObject target;
    public AudioSource aSrc;
    public Vector3 startPos;
    public Vector3 endPos;
    public float moveTime = 1f;
    public GameObject attensionMark;

    Vector3 origin;
    int touch;
    bool moving = false;
    bool opening = false;
    float remainTime = 0;

    private void OnDestroy() {
        if (target) {
            target.transform.DOKill();
        }
    }

    private void Update() {
        if (touch > 0) {
            touch--;
            remainTime = 2f;
            if (attensionMark && attensionMark.activeSelf) {
                attensionMark.SetActive(false);
            }
        }
        if (remainTime > 0 && !opening && !moving) {
            moving = true;
            opening = true;
            if (aSrc) {
                aSrc.Play();
            }
            target.transform.DOLocalMove(endPos, moveTime).SetEase(Ease.InOutSine).OnComplete(AnimationEnd);
        } else if (remainTime <= 0 && opening && !moving) {
            moving = true;
            opening = false;
            if (aSrc) {
                aSrc.Play();
            }
            target.transform.DOLocalMove(startPos, moveTime).SetEase(Ease.InOutSine).OnComplete(AnimationEnd);
        }
        if (remainTime > 0) {
            remainTime -= Time.deltaTime;
        }
    }

    void AnimationEnd() {
        moving = false;
    }    

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag(conditionTag)) {
            touch = 2;
        }
    }


}
