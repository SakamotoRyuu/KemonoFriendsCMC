using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Debris : EnemyBase
{
    public Transform[] quakePivot;
    public Transform[] movePivot;
    int throwNow;
    int throwMax;
    int attackMainSave = -1;
    int attackSubSave = -1;
    int movingIndex = -1;
    float movingSpeed = 1f;
    float movingBrake = 0.5f;
    float movingStop = 0.01f;
    bool cannotSupermanFlag;
    
    static readonly float[] intervalPlusArray = new float[] { 0.8f, 0.8f, 0.7f, 0.6f, 0.5f };
    static readonly Vector3 randomVecSmall = new Vector3(0.05f, 0.05f, 0.05f);
    static readonly Vector3 randomVecBig = new Vector3(0.5f, 0.5f, 0.5f);

    protected override void Awake() {
        base.Awake();
        attackWaitingLockonRotSpeed = 1.5f;
        stoppingDistanceBattle = 2.5f;
        mapChipSize = 1;
        normallyRequiredMaxMultiplier = 1.5f;
    }

    protected override void SetLevelModifier() {
        dropRate[0] = 250;
        dropRate[1] = 250;
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (cannotSupermanFlag) {
            supermanCheckInterval = 10000f;
            supermanEnabled = false;
        }
    }

    void MoveAttack(int index) {
        if (state == State.Attack) {
            switch (index) {
                case 0:
                    fbStepMaxDist = 8f;
                    fbStepTime = 25f / 60f;
                    fbStepEaseType = EasingType.SineInOut;
                    SeparateFromTarget(5f);
                    break;
                case 1:
                    movingSpeed = 1f;
                    movingIndex = 0;
                    movingBrake = 0.4f;
                    movingStop = 0.01f;
                    break;
                case 2:
                    movingSpeed = 1f;
                    movingIndex = 1;
                    movingBrake = 0.4f;
                    movingStop = 0.01f;
                    break;
                case 3:
                    movingSpeed = 1f;
                    movingIndex = 2;
                    movingBrake = 3.2f;
                    movingStop = 2.8f;
                    break;
                case 4:
                    fbStepMaxDist = 5f;
                    fbStepTime = 40f / 60f;
                    fbStepEaseType = EasingType.SineInOut;
                    SeparateFromTarget(7f);
                    break;
            }
        }
    }

    void MoveEnd() {
        movingIndex = -1;
        LockonEnd();
    }

    void QuakeKnock() {
        EmitEffect(9);
        if (state == State.Damage) {
            CameraManager.Instance.SetQuake(quakePivot[0].position, 5, 4, 0, 0, 1.5f, 3f, dissipationDistance_Large);
        }
    }

    void RushStart() {
        if (state == State.Attack) {
            LockonEnd();
            EmitEffect(0);
            SetSpecialMove(trans.TransformDirection(vecForward), Mathf.Clamp(GetTargetDistance(false, true, false) + 0.5f, 6f, 12f), 15f / 60f, EasingType.SineIn);
        }
    }

    void RushEnd(int amplitudePlus) {
        if (state == State.Attack) {
            specialMoveDuration = 0f;
            EmitEffect(1);
            CameraManager.Instance.SetQuake(quakePivot[0].position, 10 + amplitudePlus, 4, 0, 0, 1.5f, 3f, dissipationDistance_Large);
        }
    }

    void ArmHitGround() {
        if (state == State.Attack) {
            specialMoveDuration = 0f;
            EmitEffect(6);
            CameraManager.Instance.SetQuake(quakePivot[1].position, 10, 4, 0, 0, 1.5f, 3f, dissipationDistance_Large);
        }
    }

    void ThrowReadySpecial() {
        if (state == State.Attack) {
            ThrowReadyEffect();
            throwNow = 0;
            throwMax = Mathf.Clamp(level, 1, 4);
            for (int i = 0; i < throwMax; i++) {
                throwing.throwSettings[i].randomDirection = randomVecSmall * i;
                throwing.throwSettings[i].randomForceRate = 0.05f;
                throwing.ThrowReady(i);
            }
        }
    }

    void ThrowReadySuper() {
        if (state == State.Attack) {
            ThrowReadyEffect();
            throwNow = 0;
            throwMax = 10;
            for (int i = 0; i < throwMax; i++) {
                throwing.throwSettings[i].randomDirection = randomVecBig;
                throwing.throwSettings[i].randomForceRate = 0.2f;
                throwing.ThrowReady(i);
            }
        }
    }

    void ThrowStartSpecial() {
        if (state == State.Attack) {
            if (throwNow < throwMax) {
                throwing.ThrowStart(throwNow);
                throwNow++;
            }
        }
    }

    void ThrowReadyEffect() {
        if (state == State.Attack) {
            EmitEffect(8);
        }
    }

    protected override void DeadProcess() {
        base.DeadProcess();
        if (!fixKnockAmount && CharacterManager.Instance.GetFriendsExist(26, true) && attackerCB && attackerCB == CharacterManager.Instance.friends[26].fBase) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_GoldenMonkey, true);
        }
    }

    protected override void Attack() {
        base.Attack();
        int attackMainTemp = Random.Range(0, 3);
        int attackSubTemp = Random.Range(0, 2);
        float baseInterval = 0.4f;
        float maxHPTemp = GetMaxHP();
        if (maxHPTemp >= 1f) {
            baseInterval += Mathf.Clamp01(nowHP / maxHPTemp) * intervalPlusArray[Mathf.Clamp(level, 0, intervalPlusArray.Length - 1)];
        }
        if (attackMainTemp == attackMainSave) {
            attackMainTemp = Random.Range(0, 3);
        }
        if (attackSubTemp == attackSubSave) {
            attackSubTemp = Random.Range(0, 2);
        }
        movingIndex = -1;
        attackMainSave = attackMainTemp;
        attackSubSave = attackSubTemp;
        SuperarmorStart();
        float addStiff = (IsSuperLevel ? 0f : 0.6f);
        if (attackMainTemp == 0) {
            if (level <= 1 || attackSubTemp == 0) {
                float intervalTemp = GetAttackInterval(baseInterval);
                AttackBase(0, 1.1f, 1.7f, 0, 120f / 60f + Mathf.Min(intervalTemp, addStiff), 120f / 60f + intervalTemp, 0f);
            } else {
                float intervalTemp = GetAttackInterval(baseInterval, -1);
                AttackBase(1, 1.1f, 1.7f, 0, 170f / 60f + Mathf.Min(intervalTemp, addStiff), 170f / 60f + intervalTemp, 0f);
            }
        } else if (attackMainTemp == 1) {
            if (level <= 2 || attackSubTemp == 0) {
                int direction = Random.Range(0, 3);
                if (targetTrans) {
                    float crossAngle = Vector3.Cross(trans.TransformDirection(vecForward), targetTrans.position - trans.position).y;
                    if (crossAngle > 0.001f) {
                        direction = Random.Range(0, 100) < 60 ? 0 : 2;
                    } else if (crossAngle < -0.001f) {
                        direction = Random.Range(0, 100) < 60 ? 1 : 2;
                    }
                }
                float intervalTemp = GetAttackInterval(baseInterval);
                AttackBase(2 + direction, 1.1f, 1.4f, 0, 100f / 60f + Mathf.Min(intervalTemp, addStiff), 100f / 60f + intervalTemp, 0f);
            } else {
                float intervalTemp = GetAttackInterval(baseInterval, -2);
                AttackBase(5, 1.1f, 1.4f, 0, 180f / 60f + Mathf.Min(intervalTemp, addStiff), 180f / 60f + intervalTemp, 0f);
            }
        } else {
            if (level <= 3 || attackSubTemp == 0) {
                float intervalTemp = GetAttackInterval(baseInterval);
                AttackBase(6, 1f, 1.1f, 0, 90f / 60f + Mathf.Min(intervalTemp, addStiff), 90f / 60f + intervalTemp, 0f);
            } else {
                float intervalTemp = GetAttackInterval(baseInterval, -3);
                AttackBase(7, 1f, 1.1f, 0, 140f / 60f + Mathf.Min(intervalTemp, addStiff), 140f / 60f + intervalTemp, 0f);
            }
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (movingIndex >= 0 && movingIndex < movePivot.Length && movePivot[movingIndex] != null) {
            ApproachTransformPivot(movePivot[movingIndex], GetMinmiSpeed() * movingSpeed, movingBrake, movingStop);
        }
    }

    public void SetForEvent() {
        cannotSupermanFlag = true;
        supermanCheckInterval = 10000f;
        supermanEnabled = false;
    }

}
