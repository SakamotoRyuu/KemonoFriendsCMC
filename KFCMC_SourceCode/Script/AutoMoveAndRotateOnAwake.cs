using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoveAndRotateOnAwake : MonoBehaviour {

    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    private void Awake() {
        transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
        transform.localScale = scale;
    }
}
