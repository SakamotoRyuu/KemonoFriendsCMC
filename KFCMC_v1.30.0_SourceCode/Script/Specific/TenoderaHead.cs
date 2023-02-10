using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TenoderaHead : TriggerReceiver {

    public SearchArea searchArea;
    public Transform rayPivot;
    public LayerMask rayLayer;
    public GameObject enemyCol;
    public GameObject ddObj;
    public GameObject wrEffectPrefab;
    public BombReceiverForEnemy bombReceiver;

    Animator anim;
    NavMeshAgent agent;
    CharacterController cCon;
    Rigidbody rigid;
    CapsuleCollider capsuleCollider;
    GameObject target;
    float destroyTimer;
    float destinationInterval;
    float ddActivateDelay;
    int animHash_Move;
    bool moveSave;
    bool agentStarted;
    Ray ray;
    int rayCount;
    float wrTimer = -1f;
    bool stopped;
    EnemyBase wrTargetEnemy;
    const float destinationMax = 0.4f;
    const float rayMax = 0.2f;
    static readonly Vector3 vecDown = Vector3.down;

    private void Awake() {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        cCon = GetComponent<CharacterController>();
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animHash_Move = Animator.StringToHash("Move");
        destroyTimer = 20f;
        destinationInterval = 0.8f;
        ddActivateDelay = 0.5f;
        agent.enabled = false;
        cCon.enabled = false;
        ray = new Ray(rayPivot.position, vecDown);
    }

    void Update() {
        float deltaTimeCache = Time.deltaTime;
        if (ddActivateDelay > 0f) {
            ddActivateDelay -= deltaTimeCache;
            if (ddActivateDelay <= 0f) {
                ddObj.SetActive(true);
            }
        }
        if (!stopped) {
            destroyTimer -= deltaTimeCache;
            if (destroyTimer <= 0f) {
                if (bombReceiver) {
                    bombReceiver.BootDeathEffect();
                }
                Destroy(gameObject);
            } else {
                destinationInterval -= deltaTimeCache;
                if (destinationInterval <= 0f) {
                    if (agentStarted) {
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
                        bool moveTemp = agent.velocity.sqrMagnitude > 0.01f;
                        if (moveTemp != moveSave) {
                            moveSave = moveTemp;
                            anim.SetBool(animHash_Move, moveSave);
                        }
                    } else {
                        destinationInterval = rayMax;
                        ray.origin = rayPivot.position;
                        if (Physics.Raycast(ray, 0.35f, rayLayer, QueryTriggerInteraction.Ignore)) {
                            rayCount++;
                        } else {
                            rayCount = 0;
                        }
                        if (rayCount >= 3) {
                            rigid.velocity = Vector3.zero;
                            rigid.useGravity = false;
                            rigid.isKinematic = true;
                            capsuleCollider.enabled = false;
                            cCon.enabled = true;
                            agent.enabled = true;
                            agentStarted = true;
                            enemyCol.SetActive(true);
                        }
                    }
                }
            }
        } else {
            wrTimer -= deltaTimeCache;
            if (wrTimer <= 0f) {
                if (wrTargetEnemy) {
                    if (wrTargetEnemy.Level < 4) {
                        wrTargetEnemy.LevelUp();
                    } else {
                        wrTargetEnemy.SupermanStart();
                    }
                }
                Destroy(gameObject);
            }
        }
    }

    public override void Receive(int index, Collider other) {
        if (index == 0) { // TouchPlayerAttack
            if (other.CompareTag("PlayerAttackDetection")) {
                if (bombReceiver) {
                    bombReceiver.BootDeathEffect();
                }
                Destroy(gameObject);
            }
        } else if (index == 1) { // TouchEnemy
            if (!stopped && other.CompareTag("EnemyDamageDetection")) {
                wrTargetEnemy = other.GetComponentInParent<EnemyBase>();
                if (wrTargetEnemy && !wrTargetEnemy.isBoss) {
                    if (agent.pathStatus != NavMeshPathStatus.PathInvalid) {
                        agent.ResetPath();
                    }
                    agent.velocity = Vector3.zero;
                    agent.speed = 0f;
                    wrTimer = 0.5f;
                    stopped = true;
                    SmoothLookTarget smooth = Instantiate(wrEffectPrefab, rayPivot.position, Quaternion.LookRotation(other.transform.position - rayPivot.position)).GetComponent<SmoothLookTarget>();
                    if (smooth) {
                        smooth.target = other.transform;
                    }
                }
            }
        }
    }

}
