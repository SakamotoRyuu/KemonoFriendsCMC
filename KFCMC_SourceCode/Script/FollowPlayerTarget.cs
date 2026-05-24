using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerTarget : MonoBehaviour {

    public float yOffset;
    Transform trans;
    Vector3 faceOffset = Vector3.zero;
    GameObject targetSave;
    CharacterBase targetCBase;

    float[] speedArray = new float[21] { 0.0f, 1.8f, 2.88338f, 4.047038f, 5.296924f, 6.639427f, 8.081409f, 9.630242f, 11.293843f, 13.08071f, 15.0f, 17.0615f, 19.27575f, 21.65409f, 24.20866f, 26.95252f, 29.89971f, 33.06528f, 36.46542f, 40.11751f, 44.04022f };
    static readonly Vector3 vecForward = Vector3.forward;

    private void Awake() {
        trans = transform;
    }

    Vector3 PlayerForwardPosition() {
        return CharacterManager.Instance.playerTrans.position + CharacterManager.Instance.playerTrans.TransformDirection(vecForward) * 5f;
    }
    
    void Start() {
        trans.position = PlayerForwardPosition();
    }

    void Update() {
        if (GameManager.Instance && CharacterManager.Instance) {
            int speedIndex = GameManager.Instance.save.config[GameManager.Save.configID_CameraTurningSpeed];
            GameObject targetObj = CharacterManager.Instance.GetPlayerPotentialTarget();
            if (speedIndex > 0 && speedIndex < speedArray.Length && targetObj) {
                Vector3 toPos = trans.position;
                Vector3 targetPos = targetObj.transform.position;
                if (targetObj != targetSave) {
                    targetSave = targetObj;
                    if (targetObj) {
                        targetCBase = targetObj.GetComponentInParent<CharacterBase>();
                    }
                }
                if (targetCBase && targetCBase.faceHeight > 0f) {
                    faceOffset.y = targetCBase.faceHeight;
                    targetPos = targetCBase.transform.position + faceOffset;
                }
                toPos = Vector3.Lerp(trans.position, targetPos, speedArray[speedIndex] * Time.deltaTime);
                trans.position = toPos;
            } else {
                Vector3 targetPos = PlayerForwardPosition();
                targetPos.y += yOffset;
                trans.position = Vector3.Lerp(trans.position, targetPos, (speedIndex + 5) * Time.deltaTime);
            }
        }
    }
}
