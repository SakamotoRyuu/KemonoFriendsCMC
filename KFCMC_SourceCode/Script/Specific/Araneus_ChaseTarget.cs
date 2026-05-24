using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Araneus_ChaseTarget : MonoBehaviour {

    public SearchArea searchArea;

    Animator anim;
    NavMeshAgent agent;
    GameObject target;
    float destinationInterval;
    float destinationMax;
    int animHash_Move;
    bool moveSave;

    private void Awake() {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        animHash_Move = Animator.StringToHash("Move");
        float speedRate = Random.Range(0.8f, 1.2f);
        agent.speed = agent.speed * speedRate;
        agent.acceleration = agent.acceleration * speedRate;
        agent.angularSpeed = agent.angularSpeed * speedRate;
        destinationInterval = Random.Range(0.5f, 1.0f);
        destinationMax = Random.Range(0.3f, 0.5f);
    }

    void Update() {
        destinationInterval -= Time.deltaTime;
        if (destinationInterval <= 0f) {
            destinationInterval = destinationMax;
            if (searchArea && searchArea.enabled) {
                target = searchArea.GetNowTarget();
            } else {
                target = null;
            }
            if (agent.pathStatus != NavMeshPathStatus.PathInvalid) {
                if (target) {
                    agent.SetDestination(target.transform.position);
                } else {
                    agent.ResetPath();
                }
            }
        }
        bool moveTemp = agent.velocity.sqrMagnitude > 0.01f;
        if (moveTemp != moveSave) {
            moveSave = moveTemp;
            anim.SetBool(animHash_Move, moveSave);
        }
    }

}
