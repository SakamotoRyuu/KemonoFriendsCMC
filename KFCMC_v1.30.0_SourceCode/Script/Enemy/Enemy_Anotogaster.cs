using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Anotogaster : EnemyBase {

    public Transform[] movePivot;
    public DamageDetection criticalDD;

    int attackSave = -1;
    int moveIndex = -1;
    static readonly float[] dodgePowerArray = new float[] { 5f, 5f, 6.3f, 7.938f, 10f };
    static readonly float[] lockonSpeedArray = new float[] { 5f, 5f, 6.3f, 7.938f, 10f};
    static readonly float[] damageRateArray = new float[5] { 4.0f, 4.0f, 3.833333f, 3.666666f, 3.5f };
    static readonly float[] quickAttackArray = new float[5] { 1f, 1f, 0.8f, 0.6f, 0.4f };
    static readonly float[] knockRestoreArray = new float[] { 1f, 1f, 1.1f, 1.21f, 1.331f };
    const int effSpecialDodge = 6;

    protected override void Awake() {
        base.Awake();
        dodgeStiffTime = 0.5f;
        attackLockonDefaultSpeed = 4f;
    }

    protected override void SetLevelModifier() {
        dodgeRemain = dodgePower = dodgePowerArray[Mathf.Clamp(level, 0, dodgePowerArray.Length - 1)];
        attackLockonDefaultSpeed = lockonSpeedArray[Mathf.Clamp(level, 0, lockonSpeedArray.Length - 1)];
        knockRestoreSpeed = knockRestoreArray[Mathf.Clamp(level, 0, knockRestoreArray.Length - 1)];
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        if (criticalDD) {
            criticalDD.damageRate = damageRateArray[Mathf.Clamp(level, 0, damageRateArray.Length - 1)];
        }
        if (level >= 4) {
            attackingDodgeEnabled = true;
        } else {
            attackingDodgeEnabled = false;
        }
        attackSave = -1;
    }

    public virtual void MoveAttack() {
        if (state == State.Attack) {
            float dist = 0;
            if (targetTrans) {
                dist = Vector3.Distance(trans.position, targetTrans.position);
            }
            if (dist < 3) {
                SetSpecialMove(trans.TransformDirection(Vector3.back), 3 - dist, 0.5f, EasingType.SineInOut);
            }
        }
    }

    public void MoveAttackNeedle() {
        SpecialStep(0.3f, 0.3f, 2.5f, 0f, 0f, true, false);
    }

    public void MoveStart(int index) {
        moveIndex = index;
        LockonStart();
    }

    public void MoveEnd() {
        moveIndex = -1;
        LockonEnd();
    }

    void MoveKnockEscape() {
        if (level >= 4 && state == State.Damage && targetTrans) {
            fbStepMaxDist = 4f;
            if (knockRestoreSpeed > 0f) {
                fbStepTime = 24f / 60f / knockRestoreSpeed;
            }
            fbStepEaseType = EasingType.SineInOut;
            SeparateFromTarget(7f);
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (targetTrans && level >= 2 && state != State.Attack && targetTrans.position.y - trans.position.y >= 1.6f && GetTargetDistance(true, true, false) < 6f * 6f) {
            /*
            attackedTimeRemain -= deltaTimeMove;
            float quickAttackTemp = quickAttackArray[Mathf.Clamp(level, 0, quickAttackArray.Length - 1)];
            if (attackedTimeRemain > quickAttackTemp) {
                attackedTimeRemain = quickAttackTemp;
            }
            */
            actDistNum = 1;
        } else {
            actDistNum = 0;
        }
    }

    protected override void SideStep_Move(Vector3 dir, float distance = 5) {
        if (attackingDodgeEnabled) {
            EmitEffect(effSpecialDodge);
        }
        if (attackedTimeRemain > 1f) {
            attackedTimeRemain = 1f;
        }
        base.SideStep_Move(dir, distance);
    }

    protected override void Attack() {
        base.Attack();
        resetAgentRadiusOnChangeState = true;
        moveIndex = -1;
        int attackMax = Mathf.Clamp(1 + level, 2, 5);
        int attackTemp = Random.Range(0, attackMax);
        if (attackTemp == attackSave) {
            attackTemp = Random.Range(0, attackMax);
        }
        attackSave = attackTemp;
        switch (attackTemp) {
            case 0:
                AttackBase(0, 1f, 1.1f, 0, 1.5f, 1.5f + GetAttackInterval(1.5f), 0f);
                break;
            case 1:
                AttackBase(1, 1f, 1.1f, 0, 2f, 2f + GetAttackInterval(1.5f), 0f);
                break;
            case 2:
                AttackBase(2, 1.1f, 1.5f, 0, 80f / 60f, 80f / 60f + GetAttackInterval(1.5f), 0f, 1f, true, attackLockonDefaultSpeed * 1.2f);
                break;
            case 3:
                AttackBase(3, 1.05f, 1.2f, 0, 150f / 60f, 150f / 60f + GetAttackInterval(1.5f), 0f, 1f, true, attackLockonDefaultSpeed * 1.2f);
                agent.radius = 0.05f;
                SuperarmorStart();
                break;
            case 4:
                AttackBase(4, 1.1f, 1.4f, 0, 150f / 60f, 150f / 60f + GetAttackInterval(1.5f), 0.5f);
                SuperarmorStart();
                SpecialStep(1f, 0.5f, 6f, 0f, 0f, true, false);
                break;
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (moveIndex >= 0 && moveIndex < movePivot.Length && movePivot[moveIndex]) {
            ApproachTransformPivot(movePivot[moveIndex], Mathf.Clamp(GetMaxSpeed() * 1.5f, 9f, 24f), 0.5f, 0.025f, true, 0.35f);
        }
    }

}
