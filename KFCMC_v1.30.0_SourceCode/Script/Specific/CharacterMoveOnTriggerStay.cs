using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveOnTriggerStay : MonoBehaviour
{

    public float moveRate = 1f;
    public GameObject wall;

    Transform trans;
    Vector3 move = Vector3.zero;
    Vector3 prePos;
    Vector3 rot = Vector3.zero;
    float eulerY;
    float preEulerY;
    float onStayTime;

    static readonly Vector3 vecZero = Vector3.zero;

    private void Awake() {
        trans = transform;
        prePos = trans.position;
        preEulerY = trans.localEulerAngles.y;
    }

    private void Update() {
        if (onStayTime > 0f) {
            onStayTime -= Time.deltaTime;
        }
        if (wall.activeSelf != (onStayTime > 0f)) {
            wall.SetActive(onStayTime > 0f);
        }
    }

    private void LateUpdate() {
        move = trans.position - prePos;
        prePos = trans.position;
        float newEulerY = trans.localEulerAngles.y;
        if (preEulerY > newEulerY + 180f) {
            preEulerY -= 360f;
        } else if (preEulerY < newEulerY - 180f) {
            preEulerY += 360f;
        }
        eulerY = newEulerY - preEulerY;
        preEulerY = newEulerY;
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            if (move != vecZero) {
                CharacterController cCon = other.GetComponent<CharacterController>();
                if (cCon) {
                    Vector3 moveTemp = move;
                    moveTemp.x *= moveRate;
                    moveTemp.z *= moveRate;
                    cCon.Move(moveTemp);
                    onStayTime = 0.1f;
                }
            }
            if (eulerY != 0f) {
                rot.y = other.transform.localEulerAngles.y + eulerY;
                other.transform.localEulerAngles = rot;
            }

        }
    }

}
