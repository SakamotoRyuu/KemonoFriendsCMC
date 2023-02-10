using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Eomaia : EnemyBase
{

    public Transform[] movePivot;
    public Transform quakePivot;
    int movingIndex = -1;
    float movingSpeedMul;
    float movingBrakeMul = 1f;
    float movingBrakeRate = 0.5f;
    int throwMax;
    MissingObjectToDestroy deathChecker;
    int attackSave = -1;

    const int effectCoreBreak = 3;
    const int effectPoison = 7;
    const int effectTortureEnd = 10;

    static readonly float[] speedMulArray = new float[] { 1f, 1.1f, 1.2f, 0.5f, 1f };
    static readonly float[] brakeMulArray = new float[] { 1.4f, 1.4f, 1.4f, 3f, 1.4f };
    static readonly float[] brakeRateArray = new float[] { 0.4f, 0.4f, 0.4f, 0.2f, 0.4f };
    static readonly Vector3 vecRand = new Vector3(2f, 2f, 2f);
    const int effectThrow = 6;

    protected override void Awake() {
        base.Awake();
        stoppingDistanceBattle = 3f;
        attackedTimeRemainOnDamage = 0.5f;
    }

    void MoveStart(int index) {
        if (state == State.Attack) {
            movingIndex = index;
            if (movingIndex >= 0 && movingIndex < speedMulArray.Length) {
                movingSpeedMul = speedMulArray[movingIndex];
                movingBrakeMul = brakeMulArray[movingIndex];
                movingBrakeRate = brakeRateArray[movingIndex];
            } else {
                movingSpeedMul = 1f;
                movingBrakeMul = 1f;
                movingBrakeRate = 0.4f;
            }
        } else {
            movingIndex = -1;
        }
    }

    void MoveEnd() {
        movingIndex = -1;
        LockonEnd();
    }

    void MoveSeparate() {
        if (state == State.Attack) {
            fbStepMaxDist = 5f;
            fbStepTime = 0.5f;
            SeparateFromTarget(5f);
        }
    }

    /*
    void MoveJumpApproach() {
        if (state == State.Attack && targetTrans && movePivot[2] != null) {
            Vector3 targetTemp = targetTrans.position;
            targetTemp.y = movePivot[2].position.y;
            float dist = Mathf.Clamp((targetTemp - movePivot[2].position).magnitude, 0f, 8f);
            SetSpecialMove((targetTemp - movePivot[2].position).normalized, dist, Mathf.Clamp(dist * 0.1f, 6f / 20f, 13f / 20f), EasingType.SineOut);
        }
    }
    */

    void MoveJump() {
        if (state == State.Attack) {
            SetSpecialMove(trans.TransformDirection(Vector3.forward), 10f * (20f / 30f), 20f / 30f, EasingType.Linear);
        }
    }

    void MoveAttack2() {
        if (state == State.Attack) {
            SpecialStep(1.5f, 8f / 20f, 6f, 0f, 0f, true, false, EasingType.SineOut);
            LockonEnd();
        }
    }

    void ThrowPoison() {
        if (state == State.Attack) {
            EmitEffect(effectPoison);
        }
    }

    void ThrowReadySpecial() {
        throwMax = Mathf.Clamp(level, 1, 4);
        throwing.throwSettings[0].randomDirection = vecZero;
        throwing.throwSettings[0].randomForceRate = 0.05f;
        if (state == State.Attack) {
            throwing.ThrowReady(0);
        }
    }

    void ThrowStartSpecial() {
        if (state == State.Attack) {
            EmitEffect(effectThrow);
            throwing.ThrowStart(0);
            if (throwMax > 1) {
                throwing.throwSettings[0].randomDirection = vecRand;
                throwing.throwSettings[0].randomForceRate = 0.4f;
                for (int i = 1; i < throwMax; i++) {
                    throwing.ThrowStart(0);
                }
            }
        } else {
            throwing.ThrowCancelAll();
        }
    }

    void QuakeTortureEnd() {
        if (state == State.Attack) {
            EmitEffect(effectTortureEnd);
            CameraManager.Instance.SetQuake(quakePivot.position, 8, 4, 0, 0, 1.5f, 4f, dissipationDistance_Normal);
        }
    }

    protected override void Update_AnimControl() {
        base.Update_AnimControl();
        UpdateAC_Run();
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        resetAgentRadiusOnChangeState = true;
        agent.radius = 0.1f;
    }

    public override void EmitEffect(int index) {
        base.EmitEffect(index);
        if (index == effectPoison) {
            deathChecker = effect[index].instance.GetComponent<MissingObjectToDestroy>();
            if (deathChecker != null) {
                deathChecker.SetGameObject(gameObject);
            }
        }
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (state == State.Damage && isDamageHeavy) {
            knockAmount = 0;
        }
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    protected override void DeadProcess() {
        base.DeadProcess();
        if (!fixKnockAmount && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.buildUsed && CharacterManager.Instance.GetFriendsExist(30, true) && attackerCB == CharacterManager.Instance.friends[30].fBase) {
            CharacterManager.Instance.CheckTrophy_Fennec();
        }
    }

    protected override void Attack() {
        base.Attack();
        resetAgentRadiusOnChangeState = true;
        movingIndex = -1;
        int typeMax = (level >= 4 ? 7 : level == 3 ? 6 : level == 2 ? 5 : 4);
        int attackTemp = Random.Range(0, typeMax);
        float baseInterval = (nowHP <= GetMaxHP() / 2 ? 0.4f : 1f);
        if (attackTemp == attackSave) {
            attackTemp = Random.Range(0, typeMax);
        }
        attackSave = attackTemp;
        if (attackTemp == 0) {
            AttackBase(0, 1f, 1.1f, 0, 60f / 40f, 60f / 40f + GetAttackInterval(baseInterval), 0f);
        } else if (attackTemp == 1) {
            AttackBase(1, 1f, 0.8f, 0, 46f / 30f, 46f / 30f + GetAttackInterval(baseInterval), 0f);
        } else if (attackTemp == 2) {
            agent.radius = 0.1f;
            AttackBase(2, 1.05f, 1.1f, 0, 36f / 20f, 36f / 20f + GetAttackInterval(baseInterval), 0f);
        } else if (attackTemp == 3) {
            agent.radius = 0.1f;
            AttackBase(3, 0, 0, 0, 65f / 30f, 65f / 30f + GetAttackInterval(baseInterval), 0f);
        } else if (attackTemp == 4) {
            AttackBase(4, 0.6f, 0.6f, 0, 43f / 30f, 43f / 30f + GetAttackInterval(baseInterval), 0f);
        } else if (attackTemp == 5) {
            AttackBase(5, 1f, 0.8f, 0, 90f / 30f, 90f / 30f + GetAttackInterval(baseInterval, -2), 0f);
        } else if (attackTemp == 6) {
            AttackBase(6, 1.25f, 2.2f, 0, 79f / 30f, 79f / 30f + GetAttackInterval(baseInterval, -3), 0f);
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (movingIndex >= 0 && movingIndex < movePivot.Length && movePivot[movingIndex] != null) {
            ApproachTransformPivot(movePivot[movingIndex], GetMinmiSpeed() * movingSpeedMul, 0.5f * movingBrakeMul, 0.02f * movingBrakeMul, true, movingBrakeRate);
        }
    }

}
