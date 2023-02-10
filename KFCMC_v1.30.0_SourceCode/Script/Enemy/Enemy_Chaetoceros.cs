using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Chaetoceros : EnemyBase {

    public LayerMask startCheckMask;
    int attackSave = -1;
    bool adjustDirectionReserved;

    protected override void Awake() {
        base.Awake();
        attackedTimeRemainOnRestoreKnockDown = 0.5f;
        roveInterval = -1;
        throwing = GetComponent<Throwing>();
        attackedTimeRemainOnDamage = 0.5f;
    }

    protected override void Start() {
        if (!targetHateInitialized) {
            adjustDirectionReserved = true;
        }
        base.Start();
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (adjustDirectionReserved) {
            adjustDirectionReserved = false;
            LookNoWallDirection(10f, 1f, startCheckMask);
        }
    }

    void MoveAttack() {
        SpecialStep(2.5f, 0.5f, 4f, 0f, 0f);
    }

    int GetAttackRandom() {
        if (level == 2) {
            return Random.Range(0, 2);
        } else if (level == 3) {
            return Random.Range(0, 3);
        } else if (level >= 4) {
            if (targetTrans) {
                float sqrDist = (targetTrans.position - trans.position).sqrMagnitude;
                if (sqrDist < 4f * 4f) {
                    return Random.Range(0, 4);
                } else {
                    return Random.Range(0, 3);
                }
            } else {
                return Random.Range(0, 3);
            }
        }
        return 0;
    }

    void EmitEffectQuake(int index) {
        EmitEffect(index);
        if (effect[index].pivot && state == State.Attack) {
            CameraManager.Instance.SetQuake(effect[index].pivot.position, 5, 4, 0, 0, 1.5f, 2f, dissipationDistance_Normal);
        }
    }

    protected override void Attack() {
        int attackTemp = GetAttackRandom();
        if (attackTemp == attackSave) {
            attackTemp = GetAttackRandom();
        }
        attackSave = attackTemp;
        if (attackTemp == 0) {
            float widthMagnitude = (Vector3.Scale(transform.position, new Vector3(1, 0, 1)) - Vector3.Scale(target.transform.position, new Vector3(1, 0, 1))).sqrMagnitude;
            float heightMagnitude = (Vector3.Scale(transform.position, new Vector3(0, 1, 0)) - Vector3.Scale(target.transform.position, new Vector3(0, 1, 0))).sqrMagnitude;
            float targetYVel = 0.0f;
            CharacterController targetCCon = target.GetComponent<CharacterController>();
            if (targetCCon != null) {
                targetYVel = targetCCon.velocity.y;
            }
            if (heightMagnitude > 1 && targetYVel > -2) {
                if (widthMagnitude < 1) {
                    AttackBase(2, 1, 1, 0, 1.25f, 1.6f);
                } else {
                    AttackBase(Random.Range(0, 4) / 3, 1f, 0.6f, 0, 75f / 60f, 75f / 60f + GetAttackInterval(1.1f));
                }
            } else {
                AttackBase(Random.Range(0, 2), 1f, 0.6f, 0, 75f / 60f, 75f / 60f + GetAttackInterval(1.1f));
            }
        } else if (attackTemp == 1) {
            AttackBase(3, 1.15f, 1.4f, 0, 75f / 60f, 75f / 60f + GetAttackInterval(1.1f, -1));
        } else if (attackTemp == 2) {
            AttackBase(4, 1.15f, 1.4f, 0, 75f / 60f, 75f / 60f + GetAttackInterval(1.1f, -2));
        } else if (attackTemp == 3) {
            AttackBase(5, 1f, 1f, 0, 180f / 60f, 180f / 60f + GetAttackInterval(1.1f, -3));
        }
    }

    void ChangeForPressAttack() {
        attackPowerMultiplier = 1.2f;
        knockPowerMultiplier = 1.6f;
    }
    
}
