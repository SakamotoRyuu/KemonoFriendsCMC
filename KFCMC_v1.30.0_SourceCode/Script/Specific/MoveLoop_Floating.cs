using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLoop_Floating : MonoBehaviour {

    public float radius;
    public float hertz;

    float elapsedTime;
    Vector3 vecTemp = Vector3.zero;

    void Update() {
        if (Time.timeScale > 0f) {
            elapsedTime += Time.deltaTime;
            vecTemp.y = Mathf.Sin(Mathf.PI * 2f * hertz * elapsedTime) * radius;
            transform.localPosition = vecTemp;
        }
    }
}
