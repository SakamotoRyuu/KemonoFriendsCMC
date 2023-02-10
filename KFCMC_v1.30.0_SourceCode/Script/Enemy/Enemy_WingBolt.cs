using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_WingBolt : EnemyBase {

    public Transform quakePivot;
    public DamageDetection criticalDD;

    int attackSave = -1;
    int moveContinuous = 0;
    bool isSecret;
    float secretCollapseTimer;

    static readonly float[] damageRateArray = new float[5] { 3.5f, 3.5f, 3.333333f, 3.166666f, 3f };
    static readonly float[] moveSpeedMulArray = new float[5] { 1f, 1f, 0.8f, 0.6f, 0.4f };
    static readonly float[] lockonSpeedArray = new float[5] { 10f, 10f, 17f, 24f, 31f };
    const int effGiganticAttack = 3;


    public void SetForSecret() {
        SetLevel(1, false, false);
        exp = 0;
        actDistNum = 2;
        dropRate[0] = dropRate[1] = 0;
        dropExpDisabled = true;
        agent.speed = maxSpeed = 3f;
        if (enemyCanvas) {
            enemyCanvas.gameObject.SetActive(false);
        }
        isSecret = true;
    }

    public override void SetLevel(int newLevel, bool effectFlag = false, bool isLevelUp = true, int variableLevel = 0) {
        if (!isSecret) {
            base.SetLevel(newLevel, effectFlag, isLevelUp, variableLevel);
        }
    }

    protected override void SetLevelModifier() {
        if (level >= 2) {
            actDistNum = 1;
            fbStepMaxDist = 10;
        } else {
            actDistNum = 0;
            fbStepMaxDist = 5;
        }
        if (criticalDD) {
            criticalDD.damageRate = damageRateArray[Mathf.Clamp(level, 0, damageRateArray.Length - 1)];
        }
        attackSave = -1;
    }

    void QuakeAttack() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePivot.position, 10, 4, 0, 0, 1.5f, 2f, dissipationDistance_Normal);
        }
    }

    void ForwardStepLockonEnd(float distanceToTarget) {
        LockonEnd();
        ForwardStep(distanceToTarget);
    }

    private void MoveContinuous(int flag) {
        moveContinuous = flag;
    }

    void ChangeLockonRotSpeed() {
        lockonRotSpeed = attackLockonDefaultSpeed * 0.8f;
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (isSecret) {
            supermanCheckInterval = 10000f;
            supermanEnabled = false;
            secretCollapseTimer += deltaTimeCache;
            if (secretCollapseTimer >= 0.5f) {
                secretCollapseTimer -= 0.5f;
                if (nowHP > 0) {
                    nowHP--;
                    if (nowHP <= 0) {
                        SetState(State.Dead);
                    }
                }
            }
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (!fixKnockAmount && nowHP <= 0 && lastDamagedColorType == damageColor_Critical && attackerCB == CharacterManager.Instance.pCon) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_WingBolt, true);
        }
    }

    protected override void Attack() {
        int attackRand = 0;
        resetAgentRadiusOnChangeState = true;
        moveContinuous = 0;
        if (level >= 3) {
            attackRand = Random.Range(0, level);
            if (attackSave == attackRand) {
                attackRand = Random.Range(0, level);
            }
        }
        attackSave = attackRand;
        switch (attackRand) {
            case 0:
                fbStepTime = 15f / 60f;
                fbStepEaseType = EasingType.SineInOut;
                AttackBase(0, 1, 0.8f, 0, 1, 2.2f + Random.Range(-0.1f, 0.1f) - (level >= 2 ? (level - 1) * 0.3f : 0), moveSpeedMulArray[Mathf.Clamp(level, 0, 4)], 1f, true, lockonSpeedArray[Mathf.Clamp(level, 0, 4)]);
                agent.radius = 0.2f;
                break;
            case 1:
                fbStepTime = 15f / 60f;
                fbStepEaseType = EasingType.SineInOut;
                SeparateFromTarget(3.5f);
                fbStepTime = 25f / 60f;
                fbStepEaseType = EasingType.SineIn;
                SuperarmorStart();
                AttackBase(2, 1.15f, 1.2f, 0, 85f / 60f, 85f / 60f + GetAttackInterval(1.5f, -2), 0, 1, true, 30f);
                agent.radius = 0.05f;
                break;
            case 2:
                fbStepTime = 15f / 60f;
                fbStepEaseType = EasingType.SineInOut;
                AttackBase(1, 1.15f, 1.2f, 0, 75f / 60f, 75f / 60f + GetAttackInterval(1.5f, -2), 0);
                agent.radius = 0.05f;
                break;
            case 3:
                EmitEffect(effGiganticAttack);
                fbStepTime = 20f / 60f;
                fbStepEaseType = EasingType.SineInOut;
                SeparateFromTarget(4f);
                fbStepTime = 20f / 60f;
                fbStepEaseType = EasingType.SineIn;
                SuperarmorStart();
                AttackBase(3, 1.25f, 2.4f, 0, 110f / 60f, 110f / 60f + GetAttackInterval(1.5f, -3), 0, 1f, true, 15f);
                break;
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (attackType == 1 && moveContinuous != 0) {
            Continuous_Approach(Mathf.Max(level >= 4 ? 12.6f : 10.5f, GetMaxSpeed(false, false, true)), 0.5f, 0.01f, true, false, 0.35f);
        }
    }

}
