using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FixPositionZero : MonoBehaviour {

    NavMeshAgent agent;
    Transform trans;
    float interval;
    static readonly Vector3 vecZero = Vector3.zero;
    static readonly Quaternion quaIden = Quaternion.identity;

    private void Awake() {
        trans = transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        trans.localPosition = vecZero;
        trans.localRotation = quaIden;
        interval -= Time.deltaTime;
        if (interval <= 0f && agent && agent.enabled) {
            agent.enabled = false;
            agent.enabled = true;
            interval = 0.2f;
        }
    }

    private void LateUpdate() {
        trans.localPosition = vecZero;
        trans.localRotation = quaIden;
    }

}
