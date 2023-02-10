using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Euglena : EnemyBase {

    public NavMeshAgent eventAgent;
    bool isRemain = false;
    int attackSave = -1;
    bool contMove;

    public void SetForRemain(bool isReaper) {
        enemyID = 10;
        SetLevel(isReaper ? 4 : 1, false, false);
        nowHP = maxHP = 1;
        attackPower = 10;
        defensePower = 0;
        exp = 0;
        actDistNum = 1;
        dropRate[0] = dropRate[1] = 0;
        agent.speed = maxSpeed = 3f;
        for (int i = 0; i < searchTarget.Length; i++) {
            searchTarget[i].SetActive(false);
        }
        if (enemyCanvas) {
            enemyCanvas.gameObject.SetActive(false);
        }
        if (agent && eventAgent && !isReaper) {
            agent.agentTypeID = eventAgent.agentTypeID;
        }
        isRemain = true;
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (contMove && state != State.Attack) {
            contMove = false;
        }
    }

    protected override void Update_TimeCount() {
        if (isRemain) {
            supermanStartTimeRemain = 1000f;
        }
        base.Update_TimeCount();
    }

    void MoveAttack4() {
        fbStepTime = 20f / 60f;
        fbStepMaxDist = 3f;
        ApproachOrSeparate(3.5f);
    }

    void MoveAttack5After() {
        fbStepTime = 20f / 60f;
        fbStepMaxDist = 3f;
        SeparateFromTarget(3.5f);
    }

    void StopContinuousMove() {
        contMove = false;
    }

    protected override void Attack() {
        base.Attack();
        contMove = false;
        if (targetTrans) {
            int attackRand = 0;
            if (level >= 2) {
                int randMax = (level >= 4 ? 4 : level == 3 ? 3 : 2);
                attackRand = Random.Range(0, randMax);
                if (attackRand == attackSave) {
                    attackRand = Random.Range(0, randMax);
                }
            }
            attackSave = attackRand;
            if (attackRand == 0 || attackRand == 1) {
                fbStepTime = 10f / 30f;
                fbStepMaxDist = Mathf.Clamp(level, 1f, 4f);
                ApproachOrSeparate(4f);
                if (Vector3.Cross(trans.TransformDirection(vecForward), targetTrans.position - trans.position).y > 0) {
                    //Right
                    if (attackRand != 0) {
                        AttackBase(2, 1, 0.8f, 0, 50f / 30f, 50f / 30f + GetAttackInterval(1f, -1), 0.5f);
                    } else {
                        AttackBase(0, 1, 0.8f, 0, 40f / 30f, 40f / 30f + GetAttackInterval(1f), 0.5f);
                    }
                } else {
                    //Left
                    if (attackRand != 0) {
                        AttackBase(3, 1, 0.8f, 0, 50f / 30f, 50f / 30f + GetAttackInterval(1f, -1), 0.5f);
                    } else {
                        AttackBase(1, 1, 0.8f, 0, 40f / 30f, 40f / 30f + GetAttackInterval(1f), 0.5f);
                    }
                }
            } else if (attackRand == 2) {
                MoveAttack4();
                AttackBase(4, 1f, 0.8f, 0, 90f / 60f, 90f / 60f + GetAttackInterval(1f, -2), 0.5f);
            } else if (attackRand == 3) {
                AttackBase(5, 1.2f, 1.2f, 0, 65f / 60f, 65f / 60f + GetAttackInterval(0.6f, -3), 0f, 1f, true, 20);
                contMove = true;
            }
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (contMove) {
            Continuous_Approach(Mathf.Max(12f, GetMaxSpeed(false, false, true)), 1f, 0.5f);
        }
    }

}
