using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShrink : MonoBehaviour {

    public float delay = 0;
    public float shrinkTime = 1;
    public bool scaleZeroDestroy = true;
    private Vector3 startScale;
    private float duration = 0f;
    bool startScaleGot = false;
	
	void Update () {
        if (duration >= delay) {
            if (shrinkTime > 0 && (duration - delay) < shrinkTime) {
                if (!startScaleGot) {
                    startScale = transform.localScale;
                    startScaleGot = true;
                }
                transform.localScale = startScale * (1f - (duration - delay) / shrinkTime);
            } else {
                if (scaleZeroDestroy && gameObject) {
                    Destroy(gameObject);
                } else {
                    transform.localScale = Vector3.zero;
                }
            }
        }
        duration += Time.deltaTime;        
	}
}
