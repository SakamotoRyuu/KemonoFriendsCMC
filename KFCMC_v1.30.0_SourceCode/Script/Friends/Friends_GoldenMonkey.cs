using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class Friends_GoldenMonkey : FriendsBase {
    
    public Transform[] checkScaleTarget;
    public LookatTarget lookatTarget;
    public Transform nullTarget;

    bool refresh = false;
    bool lookatFlag;
    Transform lookatSave;
    Vector3[] checkScaleTargetDefaultPosition;
    Quaternion[] checkScaleTargetDefaultRotation;
    Vector3[] checkScaleTargetDefaultScale;
    float skillAttackCount;
    int attackSave;
    Vector2 lookatRotationRange = new Vector2(60f, 0f);
    const float skillAttackMax = 20f;
    const int skillAttackIndex = 5;
    const int effSkillReady = 0;
    const int effLengthenNormalPar = 2;
    const int effLengthenNormalSE = 3;
    const int effLengthenSuperPar = 4;
    const int effLengthenSuperSE = 5;
    const float ats1 = 34f / 29f;
    const float ats2 = 1f;
    const float ats3 = 0.8f;


    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            checkScaleTargetDefaultPosition = new Vector3[checkScaleTarget.Length];
            checkScaleTargetDefaultRotation = new Quaternion[checkScaleTarget.Length];
            checkScaleTargetDefaultScale = new Vector3[checkScaleTarget.Length];
            for (int i = 0; i < checkScaleTarget.Length; i++) {
                if (checkScaleTarget[i]) {
                    checkScaleTargetDefaultPosition[i] = checkScaleTarget[i].localPosition;
                    checkScaleTargetDefaultRotation[i] = checkScaleTarget[i].localRotation;
                    checkScaleTargetDefaultScale[i] = checkScaleTarget[i].localScale;
                }
            }
            if (lookatTarget) {
                lookatTarget.enabled = true;
            }
            moveCost.attack = 28f * staminaCostRate;
            moveCost.skill = 50f * staminaCostRate;
            notResetAttackProcessOnDamage = true;
            skillAttackCount = skillAttackMax;
            chatAttackCount = 6;
        }
    }

    public override void ResetGuts() {
        base.ResetGuts();
        skillAttackCount = skillAttackMax;
    }

    protected override void Update_Transition_Moves() {
        if (refresh && nowST >= GetMaxST() * staminaBorder) {
            refresh = false;
            actDistNum = 0;
        } else if (!refresh && !JudgeStamina(GetCost(CostType.Skill))) {
            refresh = true;
            actDistNum = 1;
        }
        if (searchArea.Length > 0 && searchArea[0]) {
            if (skillAttackCount <= 0 || (state == State.Attack && attackType == skillAttackIndex)) {
                searchArea[0].priorityEffectRate = 1f;
            } else {
                searchArea[0].priorityEffectRate = 0.8f;
            }
        }
        base.Update_Transition_Moves();
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (lookatFlag && (state != State.Attack || lookatSave == null || lookatSave != targetTrans)) {
            LookatEnd();
        }
    }

    void LookatStart() {
        if (targetTrans) {
            LockonStart();
            lookatFlag = true;
            lookatSave = targetTrans;
            lookatTarget.SetTarget(targetTrans);
            lookatTarget.SetFollowSpeed(isSuperman || attackType == skillAttackIndex ? 0.03f : 0.04f);
            lookatRotationRange.x = (attackType == skillAttackIndex ? 180f : 90f);
            lookatTarget.SetRotationRange(lookatRotationRange);
        }
    }

    void LookatEnd() {
        LockonEnd();
        lookatFlag = false;
        lookatTarget.SetTarget(nullTarget);
        lookatTarget.ResetFollowVelocity();
        lookatTarget.SetFollowSpeed(0.5f);
        lookatRotationRange.x = 90f;
        lookatTarget.SetRotationRange(lookatRotationRange);
    }

    private void OnDisable() {
        if (animatorForBattle) {
            for (int i = 0; i < checkScaleTarget.Length; i++) {
                if (checkScaleTarget[i]) {
                    checkScaleTarget[i].localPosition = checkScaleTargetDefaultPosition[i];
                    checkScaleTarget[i].localRotation = checkScaleTargetDefaultRotation[i];
                    checkScaleTarget[i].localScale = checkScaleTargetDefaultScale[i];
                }
            }
        }
    }

    void AfterAttackMove() {
        if (targetTrans) {
            float spRate = isSuperman ? 4f / 3f : 1f;
            switch (attackType) {
                case 0:
                    fbStepTime = 7f / 30f / spRate;
                    fbStepMaxDist = 13.5f * 7f / 30f;
                    break;
                case 1:
                case 5:
                    fbStepTime = 12f / 30f / spRate;
                    fbStepMaxDist = 13.5f * 12f / 30f;
                    break;
                case 4:
                    fbStepTime = 8f / 30f / spRate;
                    fbStepMaxDist = 13.5f * 8f / 30f;
                    break;
            }
            EscapeFromTarget(Random.Range(3.4f, 4.4f));
        }
    }

    void EscapeFromTarget(float distanceToTarget) {
        if (targetTrans) {
            float dist = GetTargetDistance(false, fbStepIgnoreY, fbStepConsiderRadius);
            if (dist < distanceToTarget) {
                Vector3 escapeDestination = GetEscapeDestination(searchArea[0].GetTargetsAveragePosition(), distanceToTarget);
                escapeDestination.y = trans.position.y;
                SetSpecialMove((escapeDestination - trans.position).normalized, Mathf.Clamp(distanceToTarget - dist, 0f, fbStepMaxDist), fbStepTime, fbStepEaseType);
            }
        }
    }

    void EmitEffectLengthen(int isSuper) {
        if (state == State.Attack) {
            if (isSuper == 0) {
                EmitEffect(effLengthenNormalPar);
                EmitEffect(effLengthenNormalSE);
            } else {
                EmitEffect(effLengthenSuperPar);
                EmitEffect(effLengthenSuperSE);
            }
        }
    }

    protected override void Attack() {
        float sqrDist = GetTargetDistance(true, true, true);
        float spRate = isSuperman ? 4f / 3f : 1f;
        float heightOffset = 0f;
        if (targetTrans) {
            heightOffset = Mathf.Clamp01((GetTargetHeight(false) - 1.4f) * 0.5f);
        }
        float isLion = CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Multi) ? 1f : heightOffset;
        bool skillFlag = (skillAttackCount <= 0f && JudgeStamina(GetCost(CostType.Skill) * 2.4f));
        fbStepTime = 0.25f / spRate;
        fbStepMaxDist = 4f;
        fbStepConsiderRadius = true;
        if (skillFlag) {
            SetChat(chatKey_Attack[Random.Range(3, 6)], 35);
        } else {
            base.Attack();
        }
        if (skillFlag) {
            AttackBase(skillAttackIndex, 3.0f, 40f / 11f, GetCost(CostType.Skill) * 2.4f, 118f / 60f, 118f / 60f, 0, 1f, true, attackLockonDefaultSpeed * 4f);
            SuperarmorStart();
            knockRemain = GetHeavyKnockEndurance();
            knockRemainLight = GetLightKnockEndurance();
            skillAttackCount = skillAttackMax;
            EmitEffect(effSkillReady);
            SetMutekiTime(1f);
            ApproachOrSeparate(5f);
        } else if (sqrDist >= 4f * 4f && (sqrDist >= 5.5f * 5.5f || (attackSave != 4 && Random.Range(0, 100) < 60) || (attackSave == 4 && Random.Range(0, 100) < 40)) && attackProcess != 3 && JudgeStamina(GetCost(CostType.Attack) * (20f / 14f))) {
            AttackBase(4, 1.2f, 1.4f, GetCost(CostType.Attack) * (20f / 14f), 40f / 30f / spRate, (40f / 30f + 0.3f) / spRate, 0.5f, spRate);
            skillAttackCount -= (1f + 40f / 30f);
            if (!refresh) {
                SpecialStep(6f - isLion, 8f / 30f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
            }
        } else {
            if ((attackProcess == 2 && !JudgeStamina(GetCost(CostType.Attack) * (19f / 14f))) || (attackProcess == 3 && !JudgeStamina(GetCost(CostType.Skill)))) {
                attackProcess = 0;
            }
            if (attackProcess == 0) {
                AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 34f / 30f / ats1 / spRate, 34f / 30f / ats1 / spRate, 0, ats1 * spRate);
                skillAttackCount -= (1f + 34f / 30f / ats1);
                if (sqrDist < MyMath.Square(2.7f - isLion)) {
                    SeparateFromTarget(2.7f - isLion);
                } else if (!refresh) {
                    SpecialStep(2.5f - isLion, 8f / 30f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                }
                attackProcess = 1;
            } else if (attackProcess == 1) {
                AttackBase(1, 1f, 1f, GetCost(CostType.Attack), 34f / 30f / ats1 / spRate, 34f / 30f / ats1 / spRate, 0, ats1 * spRate);
                skillAttackCount -= (1f + 34f / 30f / ats1);
                if (sqrDist < MyMath.Square(1.7f - isLion)) {
                    SeparateFromTarget(1.7f - isLion);
                } else if (!refresh) {
                    SpecialStep(1.5f - isLion, 8f / 30f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                }
                attackProcess = (JudgeStamina(GetCost(CostType.Attack) * (19f / 14f)) ? 2 : 0);
            } else if (attackProcess == 2) {
                AttackBase(2, 0.9f, 0.5f, GetCost(CostType.Attack) * (19f / 14f), 38f / 30f / ats2 / spRate, 38f / 30f / ats2 / spRate, 0, ats2 * spRate);
                skillAttackCount -= (1f + 38f / 30f / ats2);
                if (sqrDist < MyMath.Square(2.2f - isLion)) {
                    SeparateFromTarget(2.2f - isLion);
                } else if (!refresh) {
                    SpecialStep(2f - isLion, 8f / 30f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                }
                attackProcess = (JudgeStamina(GetCost(CostType.Skill)) ? 3 : 0);
            } else if (attackProcess == 3) {
                AttackBase(3, 2.3f, 2.5f, GetCost(CostType.Skill), (40f / 30f / ats3 + 0.05f) / spRate, (40f / 30f / ats3 + 0.2f) / spRate, 0, ats3 * spRate);
                skillAttackCount -= (1f + 40f / 30f / ats3);
                if (sqrDist < MyMath.Square(4.5f - isLion * 1.6f)) {
                    SeparateFromTarget(4.5f - isLion * 1.6f);
                } else if (!refresh) {
                    SpecialStep(5f - isLion * 1.6f, 8f / 30f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                }
                attackProcess = 0;
            }
        }
        attackSave = attackType;
        S_ParticlePlay(0);
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (attackType == skillAttackIndex && isSuperarmor) {
            SetMutekiTime(0.4f);
        }
    }

    public override bool GetCanDodge() {
        if (state == State.Attack && attackType == skillAttackIndex && isSuperarmor) {
            return false;
        }
        return base.GetCanDodge();
    }

}
