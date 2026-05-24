using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigDogSimiShrink : MonoBehaviour {

    public Projector targetProjector;
    public float inflateTime;
    public float sustainTime;
    public float shrinkTime;
    public float startSize;
    public float maxSize;

    private float elapsedTime;
    private int state;

    void Update() {
        elapsedTime += Time.deltaTime;
        if (state == 0) {
            if (elapsedTime < inflateTime && inflateTime > 0f) {
                targetProjector.orthographicSize = Easing.SineOut(elapsedTime, inflateTime, startSize, maxSize);
            } else {
                targetProjector.orthographicSize = maxSize;
                elapsedTime = 0f;
                state = 1;
            }
        } else if (state == 1) {
            if (elapsedTime >= sustainTime) {
                elapsedTime = 0f;
                state = 2;
            }
        } else if (state == 2) {
            if (elapsedTime < shrinkTime && shrinkTime > 0f) {
                targetProjector.orthographicSize = Easing.SineIn(elapsedTime, shrinkTime, maxSize, 0f);
            } else {
                state = 3;
                Destroy(gameObject);
            }
        }
    }
}
