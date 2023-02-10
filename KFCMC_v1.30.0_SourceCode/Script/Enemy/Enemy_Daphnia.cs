using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Daphnia : EnemyBase {

    public Transform needlePivot;
    public Transform[] movePivot;
    public Transform distantNeedlePivot;

    int attackSave = -1;
    int attackSubSave = -1;
    bool topNeedleAttack = false;
    int moveIndex = -1;
    const int needleAttackIndex = 2;
    const int eff_charge = 2;

    void MoveStart(int index) {
        moveIndex = index;
        LockonStart();
    }

    void MoveEnd() {
        moveIndex = -1;
        LockonEnd();
    }

    void SetDistantNeedlePosition() {
        Vector3 answerPos = trans.position;
        bool foundFlag = false;
        if (targetTrans) {
            ray.origin = targetTrans.position + vecUp;
            ray.direction = vecDown;
            if (Physics.Raycast(ray, out raycastHit, 5f, fieldLayerMask, QueryTriggerInteraction.Ignore)) {
                answerPos = raycastHit.point;
                foundFlag = true;
            }
        }
        if (!foundFlag) {
            ray.origin = trans.position + trans.TransformDirection(vecForward) + vecUp;
            ray.direction = vecDown;
            if (Physics.Raycast(ray, out raycastHit, 5f, fieldLayerMask, QueryTriggerInteraction.Ignore)) {
                answerPos = raycastHit.point;
                foundFlag = true;
            }
        }
        if (!foundFlag) {
            answerPos = trans.position + trans.TransformDirection(vecForward);
        }
        distantNeedlePivot.position = answerPos;
        attackPowerMultiplier = 1.2f;
        knockPowerMultiplier = 1.7f;
        distantNeedlePivot.gameObject.SetActive(true);
    }

    protected override void SetLevelModifier() {
        attackLockonDefaultSpeed = (level >= 3 ? 20f : 10f);
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        topNeedleAttack = false;
        if (GetCanControl() && targetTrans && targetTrans.position.y > needlePivot.position.y) {
            Vector3 pivotPos = needlePivot.position;
            pivotPos.y = targetTrans.position.y;
            if ((pivotPos - targetTrans.position).sqrMagnitude < 0.8f * 0.8f) {
                topNeedleAttack = true;
                attackedTimeRemain = -100;
            }
        }
        if (state != State.Attack && distantNeedlePivot.gameObject.activeSelf) {
            distantNeedlePivot.gameObject.SetActive(false);
        }
    }

    int GetAttackRandom() {
        if (level >= 4) {
            return Random.Range(0, 4);
        } else if (level >= 2) {
            return Random.Range(0, 3);
        } else {
            return Random.Range(0, 2);
        }
    }

    void ChargeBack() {
        if (state == State.Attack) {
            lockonRotSpeed *= 1.5f;
            fbStepTime = 15f / 60f;
            BackStep(4f);
        }
    }

    void ChargeAttack() {
        if (state == State.Attack) {
            CommonLockon();
            LockonEnd();
            AttackStart(needleAttackIndex);
            EmitEffect(eff_charge);
            SetSpecialMove(trans.TransformDirection(Vector3.forward), 20f * 30f / 60f, 30f / 60f, EasingType.SineOut);
        }
    }

    protected override void Attack() {
        base.Attack();
        resetAgentRadiusOnChangeState = true;
        moveIndex = -1;
        if (topNeedleAttack) {
            AttackBase(2, 1.1f, 1.7f, 0, 1.1f, 1.1f, 1, 1, false);
            agent.radius = 0.1f;
            moveIndex = 1;
        } else {
            int attackTemp = GetAttackRandom();
            if (attackTemp == attackSave) {
                attackTemp = GetAttackRandom();
            }
            attackSave = attackTemp;
            switch (attackTemp) {
                case 0:
                    fbStepTime = 15f / 60f;
                    AttackBase(0, 1, 1.1f, 0, 75f / 60f, 75f / 60f + GetAttackInterval(1.5f));
                    break;
                case 1:
                    fbStepTime = 15f / 60f;
                    int attackSubTemp = Random.Range(0, 2);
                    if (attackSubTemp == attackSubSave) {
                        attackSubTemp = Random.Range(0, 2);
                    }
                    attackSubSave = attackSubTemp;
                    if (level >= 3 && attackSubTemp == 1) {
                        AttackBase(4, 1.05f, 1.2f, 0, 135f / 60f, 135f / 60f + GetAttackInterval(1.5f));
                    } else {
                        AttackBase(1, 1.05f, 1.2f, 0, 105f / 60f, 105f / 60f + GetAttackInterval(1.5f));
                    }
                    break;
                case 2:
                    fbStepTime = 30f / 60f;
                    AttackBase(3, 0.9f, 1.2f, 0, 155f / 60f, 155f / 60f + GetAttackInterval(1.5f, -1));
                    break;
                case 3:
                    AttackBase(5, 1.1f, 1.4f, 0, 180f / 60f, 180f / 60f + GetAttackInterval(1.5f), 0.5f);
                    agent.radius = 0.2f;
                    break;
            }
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (moveIndex >= 0 && moveIndex < movePivot.Length && movePivot[moveIndex]) {
            ApproachTransformPivot(movePivot[moveIndex], 10f, 0.05f, 0.02f, true);
        }
    }

}
