using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChangeScale : MonoBehaviour {

    public Vector3 toScale;
    public float duration = 1f;
    Ease easeType = Ease.OutSine;
    
	void Start () {
        transform.DOScale(toScale, duration).SetEase(easeType);
    }

    private void OnDestroy() {
        if (gameObject) {
            gameObject.transform.DOKill();
        }
    }
}
