using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KuromeMoveLookPlayer : MonoBehaviour {

    public Transform shirome;
    public Transform kurome;
    public float motionRange;
    public float smoothTime;
    public float yOffset;
    public bool isConfuse;

    Vector3 smoothKuromeVelocity;
    bool foundFlag;
    Transform targetTrans;
    Ray ray;
    Vector3 targetConfusePos;
    float confuseResetTimer;
    static readonly Vector3 vecZero = Vector3.zero;

    private void Start() {
        if (CharacterManager.Instance && CharacterManager.Instance.playerSearchTarget) {
            targetTrans = CharacterManager.Instance.playerSearchTarget;
        }
    }

    void LateUpdate() {
        if (Time.timeScale > 0f) {
            Vector3 targetPos = vecZero;
            targetPos.z = kurome.localPosition.z;
            if (isConfuse) {
                confuseResetTimer -= Time.deltaTime;
                if (confuseResetTimer <= 0f) {
                    confuseResetTimer = Random.Range(smoothTime * (2f / 3f), smoothTime * (4f / 3f));
                    targetConfusePos = Random.insideUnitSphere.normalized;
                }
                Vector3 diff = targetConfusePos;
                targetPos.x = Mathf.Clamp(diff.x * 0.25f, -motionRange, motionRange);
                targetPos.y = Mathf.Clamp(diff.y * 0.25f, -motionRange, motionRange);
            } else {
                if (targetTrans && (targetTrans.position - shirome.position).sqrMagnitude <= (foundFlag ? 35f * 35f : 30f * 30f)) {
                    Vector3 playerPos = targetTrans.position;
                    playerPos.y += yOffset;
                    Vector3 diff = (shirome.InverseTransformPoint(playerPos) - shirome.localPosition).normalized;
                    targetPos.x = Mathf.Clamp(diff.x * 0.25f, -motionRange, motionRange);
                    targetPos.y = Mathf.Clamp(diff.y * 0.25f, -motionRange, motionRange);
                    foundFlag = true;
                } else {
                    foundFlag = false;
                }
            }
            kurome.localPosition = Vector3.SmoothDamp(kurome.localPosition, targetPos, ref smoothKuromeVelocity, smoothTime);
        }
    }
}
