using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerSmoothDamp : MonoBehaviour {

    Vector3 velocity;
    public float smoothTime;

    void Start() {
        if (CharacterManager.Instance && CharacterManager.Instance.playerTrans) {
            transform.position = CharacterManager.Instance.playerTrans.position;
        }
    }

    void Update() {
        if (CharacterManager.Instance && CharacterManager.Instance.playerTrans) {
            if (Time.timeScale == 0f) {
                transform.position = CharacterManager.Instance.playerTrans.position;
            } else {
                transform.position = Vector3.SmoothDamp(transform.position, CharacterManager.Instance.playerTrans.position, ref velocity, smoothTime);
            }
        }
    }
}
