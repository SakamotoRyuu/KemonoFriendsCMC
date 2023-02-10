using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Aurelia : EnemyBase {

    public AttackDetectionParticle sickDetection;

    int attackSave = -1;
    int moveContinuous = 0;
    static readonly int[] sickProbability = new int[] { 25, 33, 40, 50, 100 };
    static readonly float[] dodgePowerArray = new float[] { 5f, 5f, 6.3f, 7.938f, 10f };

    private void MoveContinuous(int flag) {
        moveContinuous = flag;
    }

    protected override void Awake() {
        base.Awake();
        dodgeStiffTime = 0.45f;
        dodgeDistance = 5f;
    }

    protected override void SetLevelModifier() {
        if (sickDetection) {
            sickDetection.probability = sickProbability[Mathf.Clamp(level, 0, sickProbability.Length - 1)];
        }
        dodgeRemain = dodgePower = dodgePowerArray[Mathf.Clamp(level, 0, dodgePowerArray.Length - 1)];
        if (level >= 4) {
            attackingDodgeEnabled = true;
        } else {
            attackingDodgeEnabled = false;
        }
    }

    protected override void Attack() {
        int attackTemp = Random.Range(0, Mathf.Clamp(level + 1, 2, 4));
        resetAgentRadiusOnChangeState = true;
        if (attackTemp == attackSave) {
            attackTemp = Random.Range(0, Mathf.Clamp(level + 1, 2, 4));
        }
        float moveDecay = 1f;
        float maxSpeed = GetMaxSpeed();
        float lockonMul = 1f;
        if (maxSpeed > 5) {
            moveDecay = 5 / maxSpeed;
            lockonMul = Mathf.Clamp(maxSpeed / 5, 1f, 2f);
        }
        switch (attackTemp) {
            case 0:
                AttackBase(0, 1f, 0.8f, 0f, 105f / 60f, 105f / 60f + GetAttackInterval(1.5f), moveDecay);
                break;
            case 1:
                AttackBase(1, 1f, 1f, 0f, 130f / 60f, 130f / 60f + GetAttackInterval(1.5f), moveDecay, 1f, true, attackLockonDefaultSpeed * lockonMul);
                break;
            case 2:
                AttackBase(2, 0.6f, 0.6f, 0, 100f / 60f, 100f / 60f + GetAttackInterval(1.5f), moveDecay);
                agent.radius = 0.05f;
                break;
            case 3:
                AttackBase(3, 1.1f, 0.8f, 0, 160f / 60f, 160f / 60f + GetAttackInterval(1.5f), moveDecay);
                agent.radius = 0.05f;
                break;
        }
        attackSave = attackTemp;
    }

    protected override void SideStep_Move(Vector3 dir, float distance = 5) {
        if (level >= 4) {
            EmitEffect(1);
        }
        SetSpecialMove(dir, distance, dodgeStiffTime, EasingType.CubicOut);
    }
    
    public void MoveAttack(int attackIndex) {
        if (state == State.Attack) {
            float dist = 0;
            if (target) {
                dist = Vector3.Distance(trans.position, targetTrans.position);
            }
            if (attackIndex == 0) {
                SetSpecialMove(transform.TransformDirection(vecForward), dist > 4f ? 4f : dist, 8f / 12f, EasingType.SineInOut);
            } else {
                dist += 4f;
                SetSpecialMove(transform.TransformDirection(vecForward), dist > 6f ? 6f : dist, 10f / 12f, EasingType.SineInOut);
            }
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (attackType == 2 && moveContinuous != 0) {
            Continuous_Approach(Mathf.Max(10f, GetMaxSpeed(false, false, true)));
        }
    }

    public void CheckTrophy_JustDodge(AttackDetection attacker) {
        if (attacker == attackDetection[2]) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_AureliaJumpDodge, true);
        }
    }

}
