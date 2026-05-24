using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoRotation_iTween : MonoBehaviour {

    public Vector3 toRotation;
    public float delay = 0f;
    public float time = 1f;
    public Ease ease;

    Transform trans;

    void Start() {
        trans = transform;
        if (trans) {
            if (delay > 0f) {
                trans.DOLocalRotate(toRotation, time).SetRelative().SetEase(ease).SetDelay(delay);
            } else {
                trans.DOLocalRotate(toRotation, time).SetRelative().SetEase(ease);
            }
        }
    }

    private void OnDestroy() {
        if (trans) {
            trans.DOKill();
        }
    }
}
