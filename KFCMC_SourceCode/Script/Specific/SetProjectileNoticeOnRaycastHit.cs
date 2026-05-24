using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetProjectileNoticeOnRaycastHit : MonoBehaviour {

    public Rigidbody checkRigidbody;
    public float delay = 0.5f;
    public float maxDistance = 10f;
    public LayerMask layerMask;
    public float sphereCastRadius;
    public float burstRadius;

    private float elapsedTime;
    private RaycastHit hit;
    private float interval;
    private const string targetTag = "PlayerDamageDetection";

    void LateUpdate() {
        if (elapsedTime >= delay) {
            interval -= Time.deltaTime;
            if (interval <= 0f) {
                interval = 0.01f;
                bool found = false;
                if (checkRigidbody.velocity.sqrMagnitude >= 0.01f) {
                    if (sphereCastRadius <= 0f) {
                        if (Physics.Raycast(checkRigidbody.position, checkRigidbody.velocity.normalized, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore)) {
                            found = true;
                        }
                    } else {
                        if (Physics.SphereCast(checkRigidbody.position, sphereCastRadius, checkRigidbody.velocity.normalized, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore)) {
                            found = true;
                        }
                    }
                }
                if (found) {
                    bool isPlayer = false;
                    if (hit.collider.CompareTag(targetTag)) {
                        PlayerController pCon = hit.collider.GetComponentInParent<PlayerController>();
                        if (pCon) {
                            pCon.SetProjectileNotice();
                            isPlayer = true;
                        }
                    }
                    if (burstRadius > 0f && !isPlayer && CharacterManager.Instance.pCon && CharacterManager.Instance.playerSearchTarget && (CharacterManager.Instance.playerSearchTarget.position - hit.point).sqrMagnitude <= burstRadius * burstRadius) {
                        CharacterManager.Instance.pCon.SetProjectileNotice();
                    }
                }
            }
        } else {
            elapsedTime += Time.deltaTime;
        }
    }
}
