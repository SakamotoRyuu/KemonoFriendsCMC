using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCirclePositionUpdate : MonoBehaviour {

    public float radius = 1f;
    public bool isLocal = false;
    Vector3 origin;
    Transform trans;

    private void Start() {
        trans = transform;
        if (isLocal) {
            origin = trans.localPosition;
        } else {
            origin = trans.position;
        }
    }

    // Update is called once per frame
    void Update() {
        Vector2 circlePos = Random.insideUnitCircle * radius;
        if (isLocal) {
            trans.localPosition = origin + new Vector3(circlePos.x, 0f, circlePos.y);
        } else {
            trans.position = origin + new Vector3(circlePos.x, 0f, circlePos.y);
        }
    }
}
