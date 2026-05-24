using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMessageForAttackDetection : MonoBehaviour {

    public AttackDetection sendTarget;
    public string targetTag = "PlayerDamageDetection";

    bool attackEnabled;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag) && sendTarget && sendTarget.attackEnabled) {
            sendTarget.WorkEnter(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag) && sendTarget && sendTarget.attackEnabled) {
            sendTarget.WorkExit(other.gameObject);
        }
    }
}
