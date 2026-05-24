using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Lion : FriendsBase {    
    
    bool isAngry = false;
    bool refresh = false;
    float targetMissingTime = 0f;
    bool continuousMoveEnabled;
    float primeProjectileTimeRemain;
    int escapePoint;
    int dodgePrideCount;
    
    const float spBias = 1.2f;
    const float angryTimeLimit = 12f;
    const string chatKey_Special = "TALK_LION_SPECIAL";
    const int effectSlash = 2;
    const int slashThrowIndex = 1;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            chatAttackCount = 6;
            chatDamageCount = 8;
            damageReportEnabled = false;
            moveCost.attack = 14f * staminaCostRate;
            moveCost.skill = 30f;
        }
    }

    void MoveAttack1() {
        if (!refresh) {
            float spRate = isSuperman || isAngry ? 4f / 3f : 1;
            SpecialStep(0.4f, 9f / 30f / spRate, 4f, 0.1f, 0.4f, true, true, EasingType.SineInOut, true);
        }
    }

    void MoveAttack2() {
        continuousMoveEnabled = true;
    }

    void MoveEscape() {
        if ((!isAngry || escapePoint > 0) && state == State.Attack && target) {
            float spRate = isSuperman ? 4f / 3f : 1;
            fbStepMaxDist = isAngry ? 5f : 3f;
            fbStepTime = 9f / 30f / spRate;
            fbStepEaseType = EasingType.SineInOut;
            fbStepIgnoreY = false;
            SeparateFromTarget(6.5f);
            primeProjectileTimeRemain = 0.6f;
        }
    }

    void MoveEscape_Wave(int approach) {
        if (state == State.Attack && target) {
            float spRate = isSuperman || isAngry ? 4f / 3f : 1;
            fbStepMaxDist = isAngry ? 5f : 3f;
            fbStepTime = 12f / 30f / spRate;
            fbStepEaseType = EasingType.SineInOut;
            fbStepIgnoreY = false;
            if (isAngry && approach != 0 && escapePoint <= 0) {
                SpecialStep(2.25f, fbStepTime, fbStepMaxDist, 0f, 0f, false, true, fbStepEaseType, true);
            } else {
                SeparateFromTarget(6.5f);
            }
        }
    }

    void MoveEnd() {
        LockonEnd();
        continuousMoveEnabled = false;
    }

    void FinishAttackStart() {
        AttackStart(0);
        AttackStart(1);
    }
    
    void FinishAttackEnd() {
        AttackEnd(0);
        AttackEnd(1);
    }

    protected void LightEyeActivate(int param) {
        if (!isSuperman) {
            SupermanSetMaterial(param != 0);
        }
    }

    protected override void OnEnable() {
        base.OnEnable();
        SetAngry(false);
    }

    void SetAngry(bool flag) {
        if (flag) {
            isAngry = true;
            if (!effect[1].instance) {
                EmitEffect(1);
            }
            LightEyeActivate(1);
            if (GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] < 3) {
                SetChat(chatKey_Special, 35);
            }
            mesAtkMin = 3;
            mesAtkMax = 5;
            mesDmgLtMin = 4;
            mesDmgLtMax = 6;
            mesDmgHvMin = 5;
            mesDmgHvMax = 7;
            escapePoint = 0;
        } else {
            isAngry = false;
            if (effect[1].instance) {
                Destroy(effect[1].instance);
            }
            LightEyeActivate(0);
            mesAtkMin = 0;
            mesAtkMax = 2;
            mesDmgLtMin = 0;
            mesDmgLtMax = 2;
            mesDmgHvMin = 1;
            mesDmgHvMax = 3;
            escapePoint = 3;
        }
    }

    protected override void Update_Targeting() {
        base.Update_Targeting();
        if (target) {
            targetMissingTime = 0f;
        } else {
            targetMissingTime += deltaTimeCache;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        bool damagedFlag = (GameManager.Instance.time < CharacterManager.Instance.damageRecord.time + angryTimeLimit && CharacterManager.Instance.damageRecord.attacker != null && CharacterManager.Instance.damageRecord.receiver != this);
        bool dodgedFlag = (GameManager.Instance.time < CharacterManager.Instance.dodgeRecord.time + angryTimeLimit * 0.5f && CharacterManager.Instance.dodgeRecord.attacker != null && CharacterManager.Instance.dodgeRecord.receiver != this);
        if (target && state != State.Dead && !isAngry && (damagedFlag || dodgedFlag)) {
            attackerCB = (damagedFlag ? CharacterManager.Instance.damageRecord.attacker : CharacterManager.Instance.dodgeRecord.attacker);
            if (attackerCB) {
                attackerObj = attackerCB.gameObject;
                searchArea[0].SetLockTargetFromCharacter(attackerCB, angryTimeLimit * (damagedFlag ? 1f : 0.5f));
            }
            SetAngry(true);
            if (dodgedFlag) {
                dodgePrideCount++;
                if (dodgePrideCount >= 5) {
                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_Lion, true);
                    dodgePrideCount = -10000;
                }
            }
        }
        if (isAngry && !damagedFlag && !dodgedFlag) {
            SetAngry(false);
        }
        if (refresh && nowST >= GetMaxST() * staminaBorder) {
            refresh = false;
        } else if (!refresh && !JudgeStamina(GetCost(CostType.Skill))) {
            refresh = true;
        }
        if (refresh) {
            actDistNum = 2;
        } else {
            actDistNum = isAngry ? 1 : 0;
        }
        if (primeProjectileTimeRemain > 0f) {
            primeProjectileTimeRemain -= deltaTimeMove;
        }
    }

    protected override float GetSTHealRate() {
        return base.GetSTHealRate() * (isAngry ? 2f : 1f);
    }

    protected override void Start_Process_Dead() {
        base.Start_Process_Dead();
        SetAngry(false);
    }

    protected override void Attack() {
        base.Attack();
        float spRate = isSuperman || isAngry ? 4f / 3f : 1;
        float atkRate = isAngry ? 1.2f : 1f;
        float knockRate = isAngry ? 1.2f : 1f;
        float sqrDist = 0;
        continuousMoveEnabled = false;
        if (targetTrans) {
            Vector3 dirTemp = (trans.position - targetTrans.position).normalized;
            sqrDist = ((targetTrans.position + dirTemp * targetRadius) - trans.position).sqrMagnitude;
        }
        if ((sqrDist > 5f * 5f || (attackProcess == 0 && sqrDist > 3f * 3f && (Random.Range(0, 100) < 50 || primeProjectileTimeRemain > 0f))) && JudgeStamina(GetCost(CostType.Skill))) {
            if (AttackBase(4, 1.3f, 1.3f, GetCost(CostType.Skill), 57f / 30f / (spBias * spRate), 57f / 30f / (spBias * spRate), 0.5f, spBias * spRate, true, attackLockonDefaultSpeed * 2f)) {
                S_ParticlePlay(0);
                S_ParticlePlay(1);
                MoveEscape_Wave(0);
                escapePoint -= 2;
            }
        } else {
            switch (attackProcess) {
                case 0:
                    if (AttackBase(0, 1f * atkRate, 1f * knockRate, GetCost(CostType.Attack), 14f / 30f / spRate, 14f / 30f / spRate, 1f, spRate)) {
                        S_ParticlePlay(0);
                        MoveAttack1();
                        escapePoint++;
                    }
                    break;
                case 1:
                    if (AttackBase(1, 1f * atkRate, 1f * knockRate, GetCost(CostType.Attack), 14f / 30f / spRate, 14f / 30f / spRate, 1f, spRate)) {
                        S_ParticlePlay(1);
                        MoveAttack1();
                        escapePoint++;
                    }
                    break;
                case 2:
                    if (AttackBase(2, 1.35f * atkRate, 1.4f * knockRate, GetCost(CostType.Attack) * (20f / 14f), 20f / 30f / spRate, 20f / 30f / spRate, 1f, spRate)) {
                        S_ParticlePlay(0);
                        S_ParticlePlay(1);
                        escapePoint++;
                    }
                    break;
                case 3:
                    if (AttackBase(3, 2f * atkRate, 2.15f * knockRate, GetCost(CostType.Attack) * ((36f / spBias) / 14f), 36f / 30f / (spBias * spRate), 36f / 30f / (spBias * spRate) + (isAngry ? 0f : 5f / 30f / spRate), 0f, spBias * spRate)) {
                        S_ParticlePlay(0);
                        S_ParticlePlay(1);
                        escapePoint++;
                    }
                    break;
            }
            attackProcess = (attackProcess + 1) % 4;
        }
        if (isAngry) {
            SuperarmorStart();
        }
        escapePoint = Mathf.Clamp(escapePoint, -3, 3);
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        switch (attackType) {
            case 0:
            case 1:
            case 2:
                if (isAngry && isLockon && target) {
                    Continuous_Approach(6f, 0.4f, 0.1f, true, true, 0.25f);
                }
                break;
            case 3:
                if (continuousMoveEnabled && target) {
                    Continuous_Approach(13.5f * (isSuperman || isAngry ? 4f / 3f : 1f), 0.4f, 0.1f, true, true, 0.25f);
                }
                break;
        }
    }

    public override float GetMaxSpeed(bool isWalk = false, bool ignoreSuperman = false, bool ignoreSick = false, bool ignoreConfig = false) {
        return base.GetMaxSpeed(isWalk, ignoreSuperman, ignoreSick, ignoreConfig) * (isAngry ? 1.25f : 1f);
    }

    public override float GetKnocked() {
        return base.GetKnocked() * (isAngry ? 3f : 1f);
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (isAngry && damage > 0) {
            damage = Mathf.Max(damage * 4 / 5, 1);
        }
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    void ThrowStartSpecial(int index) {
        if (throwing && throwing.throwSettings.Length > index && throwing.throwSettings[index].from) {
            if (targetTrans) {
                Vector3 targetPos = targetTrans.position;
                float height = GetTargetHeight(true);
                if (height > -0.2f && height < 1.2f) {
                    targetPos.y = throwing.throwSettings[index].from.transform.position.y;
                }
                throwing.throwSettings[index].from.transform.LookAt(targetPos);
            } else {
                throwing.throwSettings[index].from.transform.localRotation = quaIden;
            }
            throwing.ThrowStart(index);
        }
    }

    void ThrowSlash() {
        if (throwing && isAngry && state == State.Attack) {
            EmitEffect(effectSlash);
            throwing.ThrowStart(slashThrowIndex);
        }
    }
}
