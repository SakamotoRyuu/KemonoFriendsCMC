using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_PaintedWolf : FriendsBase {

    public ParticleSystem rushParticle;
    public ParticleSystem upperParticle;
    public Transform movePivot;

    bool continuousMove = false;
    float skillAttackCount;
    int attackSave = -1;
    bool refresh = false;
    float targetLostTime = 0f;
    string[] chatKey_Sp = new string[3];
    float upperedTime;
    const float skillAttackMax = 16f;
    const int skillAttackIndex = 8;
    const int upperAttackIndex = 9;
    const int upperEffectIndex = 2;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            // moveCost.attack = 12f;
            moveCost.attack = 30f * staminaCostRate;
            moveCost.skill = 80f;
            skillAttackCount = skillAttackMax;
            chatAttackCount = 6;
        }
    }

    protected override void ChatKeyInit() {
        base.ChatKeyInit();
        chatKey_Sp[0] = StringUtils.Format("{0}{1}_SP_00", talkHeader, talkName);
        chatKey_Sp[1] = StringUtils.Format("{0}{1}_SP_01", talkHeader, talkName);
        chatKey_Sp[2] = StringUtils.Format("{0}{1}_SP_02", talkHeader, talkName);
    }

    public override void ResetGuts() {
        base.ResetGuts();
        skillAttackCount = skillAttackMax;
    }

    protected override void Update_Transition_Moves() {
        if (skillAttackCount <= 0) {
            if (refresh && JudgeStamina(GetCost(CostType.Skill) + GetCost(CostType.Attack) + GetCost(CostType.Run) * 2f)) {
                refresh = false;
                actDistNum = 0;
            } else if (!refresh && !JudgeStamina(GetCost(CostType.Skill) + GetCost(CostType.Attack))) {
                refresh = true;
                actDistNum = 1;
            }
        } else {
            if (refresh && nowST >= GetMaxST() * staminaBorder) {
                refresh = false;
                actDistNum = 0;
            } else if (!refresh && !JudgeStamina(GetCost(CostType.Attack) * (98f / 60f))) {
                refresh = true;
                actDistNum = 1;
            }
        }
        if (searchArea.Length > 0 && searchArea[0]) {
            if (skillAttackCount <= 0 || (state == State.Attack && attackType == skillAttackIndex)) {
                searchArea[0].priorityEffectRate = 1f;
            } else {
                searchArea[0].priorityEffectRate = 0.3f;
            }
        }
        base.Update_Transition_Moves();
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (state != State.Attack) {
            ActivateRushParticle(false);
            ActivateUpperParticle(false);
        }
        if (upperedTime > 0f) {
            upperedTime -= deltaTimeMove;
        }
    }

    void MoveAttack(int index) {
        float spRate = isSuperman ? 4f / 3f : 1f;
        switch (index) {
            case 0:
                if (!refresh) {
                    SpecialStep(0.35f, 0.2f / spRate, 3.5f, 0f, 0f, true, true, EasingType.SineInOut, true);
                    continuousMove = true;
                }
                break;
            case 1:
                if (!refresh) {
                    SpecialStep(0.45f, 0.2f / spRate, 3.5f, 0f, 0f, true, true, EasingType.SineInOut, true);
                    continuousMove = true;
                }
                break;
            case 2:
                SeparateFromTarget(2.5f);
                break;
            case 3:
                continuousMove = true;
                targetLostTime = 0f;
                ActivateRushParticle(true);
                break;
            case 4:
                continuousMove = false;
                ActivateRushParticle(false);
                break;
            case 5:
                continuousMove = false;
                break;
            case 6:
                ActivateUpperParticle(true);
                break;
            case 7:
                ActivateUpperParticle(false);
                break;
        }
    }

    void ActivateRushParticle(bool flag) {
        if (enabled && rushParticle) {
            if (flag) {
                rushParticle.Play();
            } else if (!flag && rushParticle.isPlaying) {
                rushParticle.Stop();
            }
        }
    }

    void ActivateUpperParticle(bool flag) {
        if (enabled && upperParticle) {
            if (flag) {
                upperParticle.Play();
            } else if (!flag && upperParticle.isPlaying) {
                upperParticle.Stop();
            }
        }
    }

    void ChangeAttack(int index) {
        switch (index) {
            case 0:
                attackPowerMultiplier = 1.3f;
                knockPowerMultiplier = 1.4f;
                break;
            case 1:
                attackPowerMultiplier = 1.1f;
                knockPowerMultiplier = 1.14f;
                break;
            case 3:
                attackPowerMultiplier = 1.1f;
                knockPowerMultiplier = 1.14f;
                break;
            case 4:
                attackPowerMultiplier = 1.25f;
                knockPowerMultiplier = 1.32f;
                break;
            case 5:
                attackPowerMultiplier = 1.1f;
                knockPowerMultiplier = 1.14f;
                break;
            case 6:
                attackPowerMultiplier = 1.2f;
                knockPowerMultiplier = 1.25f;
                break;
            case 7:
                attackPowerMultiplier = 1.1f;
                knockPowerMultiplier = 1.14f;
                break;
            case 8:
                attackPowerMultiplier = 1.1f;
                knockPowerMultiplier = 1.14f;
                break;
            case 9:
                attackPowerMultiplier = 3.5f;
                knockPowerMultiplier = 4.5f;
                break;
        }
    }
    
    protected override void Attack() {
        float spRate = (isSuperman ? 4f / 3f : 1f);
        int attackTemp;
        resetAgentRadiusOnChangeState = true;
        continuousMove = false;
        if (skillAttackCount <= 0f && JudgeStamina(GetCost(CostType.Skill) + GetCost(CostType.Attack)) && GetTargetHeight(true) < 1.6f) {
            attackTemp = skillAttackIndex;
        } else if (JudgeStamina(GetCost(CostType.Attack) * (4f / 3f)) && target && GetTargetHeight(true) >= 1.3f + Mathf.Clamp(upperedTime * 0.07f, 0f, 0.6f)) {
            attackTemp = 9;
        } else { 
            attackTemp = Random.Range(0, 8);
            if (attackTemp == attackSave) {
                attackTemp = (attackTemp + 1) % 8;
            }
        }
        attackSave = attackTemp;
        if (attackTemp != skillAttackIndex) {
            base.Attack();
        } else {
            SetChat(chatKey_Attack[Random.Range(3, 6)], 35);
        }
        switch (attackTemp) {
            case 0:
                // 75f / 60f
                if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack) * (68f / 60f), 68f / 60f / spRate, 68f / 60f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 68f / 60f);
                }
                break;
            case 1:
                if (AttackBase(1, 1f, 1f, GetCost(CostType.Attack) * (66f / 60f), 66f / 60f / spRate, 66f / 60f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 66f / 60f);
                }
                break;
            case 2:
                // 98f / 60f
                if (AttackBase(2, 1f, 1f, GetCost(CostType.Attack) * (94f / 60f), 94f / 60f / spRate, 94f / 60f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 94f / 60f);
                }
                break;
            case 3:
                if (AttackBase(3, 1f, 1f, GetCost(CostType.Attack) * (68f / 60f), 68f / 60f / spRate, 68f / 60f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 68f / 60f);
                }
                break;
            case 4:
                if (AttackBase(4, 1f, 1f, GetCost(CostType.Attack), 60f / 60f / spRate, 60f / 60f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 60f / 60f);
                }
                break;
            case 5:
                if (AttackBase(5, 0.92f, 0.9f, GetCost(CostType.Attack), 60f / 60f / spRate, 60f / 60f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 60f / 60f);
                }
                break;
            case 6:
                if (AttackBase(6, 1.1f, 1.14f, GetCost(CostType.Attack) * (62f / 60f), 62f / 60f / spRate, 62f / 60f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 62f / 60f);
                }
                break;
            case 7:
                if (AttackBase(7, 1f, 1f, GetCost(CostType.Attack) * (66f / 60f), 66f / 60f / spRate, 66f / 60f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 66f / 60f);
                }
                break;
            case 8:
                if (AttackBase(8, 1.6f, 1.8f, GetCost(CostType.Attack), 265f / 60f, 280f / 60f, 0.5f, 1f)) {
                    continuousMove = false;
                    SuperarmorStart();
                    knockRemain = GetHeavyKnockEndurance();
                    knockRemainLight = GetLightKnockEndurance();
                    skillAttackCount = skillAttackMax * 0.25f;
                }
                break;
            case 9:
                if (AttackBase(9, 2.4f, 3f, GetCost(CostType.Attack) * (4f / 3f), 31f / 30f / 0.8f / spRate, 31f / 30f / 0.8f / spRate, 0f, 0.8f * spRate, true, attackLockonDefaultSpeed * 20f)) {
                    skillAttackCount -= (1f + 31f / 30f / 0.8f);
                    continuousMove = true;
                    upperedTime = 10f;
                    EmitEffect(upperEffectIndex);
                    SetMutekiTime(0.4f);
                    agent.radius = 0.1f;
                }
                break;
        }
    }

    void NoCostDodge() {
        attackStiffTime = 0f;
        attackedTimeRemain = 0f;
        SetState(State.Wait);
        float costSave = dodgeCost;
        dodgeCost = 0f;
        SideStep(0, dodgeDistance, dodgeMutekiTime, true);
        dodgeCost = costSave;
    }

    void SkillTrigger() {
        if (enabled) {
            if (target) {
                EmitEffect(0);
                skillAttackCount = skillAttackMax;
                nowST -= GetCost(CostType.Skill);
                targetLostTime = 0f;
                if (nowST < 0f) {
                    nowST = 0f;
                }
            } else {
                NoCostDodge();
            }
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (attackType == skillAttackIndex) {
            if (isSuperarmor) {
                SetMutekiTime(0.4f);
            }
            if (continuousMove) {
                if (target) {
                    targetLostTime = 0f;
                    Continuous_Approach(Mathf.Max(GetMaxSpeed(false, false, false, true), 16.875f), 0.35f, 0.25f, true, true);
                } else {
                    targetLostTime += deltaTimeCache;
                    if (targetLostTime >= 0.25f) {
                        NoCostDodge();
                    }
                }
            }
        } else if (attackType == upperAttackIndex) {
            if (continuousMove && target) {
                ApproachTransformPivot(movePivot, 16.875f * (isSuperman ? 4f / 3f : 1f), 0.3f, 0.03f, true, 0.4f);
            }
        } else { 
            if (continuousMove && target) {
                Continuous_Approach(6f, 0.5f, 0.3f, true, true);
            }
        }
    }

    public override bool GetCanDodge() {
        if (state == State.Attack && attackType == skillAttackIndex && isSuperarmor) {
            return false;
        }
        return base.GetCanDodge();
    }

    public void SneezeSerif() {
        SetChat(chatKey_Sp[Random.Range(0, 3)], 15);
    }

}
