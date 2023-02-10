using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Periplaneta : EnemyBase {

    public Renderer emitRend;
    public Transform[] movePivot;

    Material[] rendMats = new Material[2];
    float refreshTime = 0f;
    bool escapeMode = false;
    int nextAttack = 0;
    int attackSave = -1;
    int movingIndex = -1;
    float movingSpeed;
    int emissionPropertyID;
    int levelUpCount;
    const int levelUpMax = 3;
    static readonly Color emissionColor = new Color(1.5f, 1.5f, 1.5f);
    static readonly float[] knockRestoreArray = new float[] { 1f, 1f, 1.25f, 1.5f, 1.75f };
    static readonly float[] attackedTimeRemainArray = new float[] { 1f, 1f, 1f / 1.1f, 1f / 1.21f, 1f / 1.331f };
    static readonly float[] attackIntervalArray = new float[] { 1.4f, 1.4f, 1.4f / 1.1f, 1.4f / 1.21f, 1.4f / 1.331f };
    const float levelUpCondition = 10f;

    protected override void Awake() {
        base.Awake();
        attackLockonDefaultSpeed = 16f;
        emissionPropertyID = Shader.PropertyToID("_EmissionColor");
    }

    void SetEmitMaterial(bool toEmit) {
        if (emitRend) {
            rendMats = emitRend.materials;
            if (rendMats.Length > 0) {
                if (toEmit) {
                    rendMats[0].SetColor(emissionPropertyID, emissionColor);
                } else {
                    rendMats[0].SetColor(emissionPropertyID, Color.black);
                }
            }
            emitRend.materials = rendMats;
        }
    }

    void SetEscapeMode(bool flag) {
        escapeMode = flag;
        if (escapeMode) {
            actDistNum = 3;
            roveInterval = -1;
        } else {
            actDistNum = (nextAttack == 0 ? 0 : 1);
            roveInterval = 8f;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (!escapeMode && levelUpCount < levelUpMax) {
            if (nowHP <= GetMaxHP() / 3 && targetExistTime >= 0.25f) {
                SetEscapeMode(true);
                SetEmitMaterial(true);
            } else {
                if (attackedTimeRemain > GetAttackIntervalSpecial() / 2) {
                    actDistNum = 2;
                } else {
                    actDistNum = (nextAttack == 0 ? 0 : 1);
                }
            }
        }
        if (escapeMode) {
            refreshTime += deltaTimeCache;
            if (refreshTime > levelUpCondition) {
                LevelUp();
                levelUpCount++;
                nowHP = GetMaxHP();
            }
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        refreshTime = 0f;
    }

    protected override void Update_AnimControl() {
        base.Update_AnimControl();
        UpdateAC_Run();
    }

    void MoveAttack0() {
        if (state == State.Attack) {
            // SpecialStep(2f, 5f / 30f, 2f, 0f, 0f, true, false);
            movingIndex = 0;
            movingSpeed = 0.9f;
        }
    }

    void MoveAttack1() {
        if (state == State.Attack) {
            SetSpecialMove(trans.TransformDirection(vecForward), 14f, 20f / 60f, EasingType.Linear);
        }
    }

    void MovingEnd() {
        movingIndex = -1;
    }

    void MoveKnockEscape() {
        if (level >= 3 && state == State.Damage && targetTrans) {
            fbStepMaxDist = 3f;
            if (knockRestoreSpeed > 0f) {
                fbStepTime = 18f / 30f / knockRestoreSpeed;
            }
            fbStepEaseType = EasingType.SineOut;
            SeparateFromTarget(7f);
        }
    }

    protected override void SetLevelModifier() {
        SetEmitMaterial(false);
        nextAttack = (level >= 4 ? Random.Range(0, 3) : level >= 2 ? Random.Range(0, 2) : 0);
        actDistNum = (nextAttack == 0 ? 0 : 1);
        refreshTime = 0f;
        roveInterval = 8f;
        escapeMode = false;
        knockRestoreSpeed = knockRestoreArray[Mathf.Clamp(level, 0, knockRestoreArray.Length - 1)];
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        attackedTimeRemainOnDamage = attackedTimeRemainArray[Mathf.Clamp(level, 0, attackedTimeRemainArray.Length - 1)];
    }

    float GetAttackIntervalSpecial() {
        return attackIntervalArray[Mathf.Clamp(level, 0, attackIntervalArray.Length - 1)];
    }
    
    void SpecialLockonStart() {
        specialMoveDuration = 0f;
        lockonRotSpeed = 40f;
        LockonStart();
    }

    protected override void DeadProcess() {
        base.DeadProcess();
        if (!fixKnockAmount) {
            CharacterManager.Instance.CheckTrophy_Giraffe(attackerCB);
        }
    }

    protected override void Attack() {
        base.Attack();
        movingIndex = -1;
        float stiffPlus = (IsSuperLevel ? 0f : 0.1f);
        if (nextAttack == 0 || level <= 1) {
            AttackBase(0, 1f, 0.8f, 0, 30f / 30f, 30f / 30f + GetAttackIntervalSpecial() + Random.Range(-0.1f, 0.1f), 0f);
            MoveAttack0();
        } else if (nextAttack == 1 || level <= 3) {
            AttackBase(1, 1f, 0.8f, 0, 60f / 60f + stiffPlus, 60f / 60f + stiffPlus + GetAttackIntervalSpecial() + Random.Range(-0.1f, 0.1f), 0f);
        } else {
            AttackBase(2, 1f, 0.8f, 0f, 90f / 60f + stiffPlus, 90f / 60f + stiffPlus + GetAttackIntervalSpecial() + Random.Range(-0.1f, 0.1f), 0f);
        }
        if (level >= 2) {
            nextAttack = Random.Range(0, level >= 4 ? 3 : 2);
            if (nextAttack == attackSave) {
                nextAttack = Random.Range(0, 2);
            }
        } else {
            nextAttack = 0;
        }
        attackSave = nextAttack;
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (movingIndex >= 0 && movingIndex < movePivot.Length) {
            ApproachTransformPivot(movePivot[movingIndex], GetMinmiSpeed() * movingSpeed, 0.25f, 0.1f);
        }
    }

}
