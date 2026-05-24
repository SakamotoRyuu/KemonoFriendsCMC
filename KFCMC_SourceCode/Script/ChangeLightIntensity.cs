using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChangeLightIntensity : MonoBehaviour {

    public Light pointLight;
    public float endIntensity;
    public float time;
    public Ease easeType = Ease.InOutSine;
    
    void Start () {
		if (!pointLight) {
            pointLight = GetComponent<Light>();
        }
        if (pointLight) {
            pointLight.DOIntensity(endIntensity, time).SetEase(easeType).OnComplete(EaseComplete);
        }
    }

    void EaseComplete() {
        if (pointLight && endIntensity <= 0) {
            pointLight.enabled = false;
        }
    }

    private void OnDestroy() {
        if (pointLight) {
            pointLight.DOKill();
        }
    }

}
