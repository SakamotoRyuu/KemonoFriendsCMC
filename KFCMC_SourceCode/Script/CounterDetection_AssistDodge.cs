using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterDetection_AssistDodge : MonoBehaviour {

    public string targetTag = "EnemyAttackDetection";

    protected CharacterBase parentCBase;
    protected static readonly Vector3 vecForward = Vector3.forward;

    protected virtual void Awake() {
        parentCBase = GetComponentInParent<CharacterBase>();
    }

    void DodgeBody(Vector3 closestPoint) {
        int dodgeDir = Random.Range(-1, 2);
        if (transform.position != closestPoint) {
            closestPoint.y = transform.position.y;
            float axis = Vector3.Cross(transform.TransformDirection(vecForward), closestPoint - transform.position).y;
            if (axis > 0.001f) {
                dodgeDir = -1;
            } else if (axis < -0.001f) {
                dodgeDir = 1;
            }
        }
        parentCBase.CounterDodge(dodgeDir);
    }

    private void OnTriggerStay(Collider other) {
        if (parentCBase != null && other.CompareTag(targetTag) && parentCBase.enabled && parentCBase.GetCanTakeDamage(false)) {
            DodgeBody(other.ClosestPoint(transform.position));
        }
    }

}
