using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Lion : PlayerController
{

    bool isAngry = false;
    bool continuousMoveEnabled;
    int escapePoint;

    const float spBias = 1.2f;
    const float angryTimeLimit = 12f;
    const string chatKey_Special = "TALK_LION_SPECIAL";
    const int effectSlash = 2;
    const int slashThrowIndex = 1;

    protected override void Awake()
    {
        base.Awake();
        chatAttackCount = 6;
        chatDamageCount = 8;
        damageReportEnabled = false;
        moveCost.attack = 14f * staminaCostRate;
        moveCost.skill = 30f;
        justDodgeCounterAttackProcess = 2;
    }

    void MoveAttack1()
    {
        if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman || isAngry ? 4f / 3f : 1;
            SpecialStep(0.4f, 9f / 30f / spRate, 4f, 0.1f, 0.4f, true, true, EasingType.SineInOut, true);
        }
    }

    void MoveAttack2()
    {
        continuousMoveEnabled = true;
    }

    void MoveEscape()
    {
        if ((!isAngry || escapePoint > 0) && state == State.Attack && target && groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman ? 4f / 3f : 1;
            fbStepMaxDist = isAngry ? 5f : 3f;
            fbStepTime = 9f / 30f / spRate;
            fbStepEaseType = EasingType.SineInOut;
            fbStepIgnoreY = false;
            SeparateFromTarget(6.5f);
        }
    }

    void MoveEscape_Wave(int approach)
    {
        if (state == State.Attack && target && groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
        {
            float spRate = isSuperman || isAngry ? 4f / 3f : 1;
            fbStepMaxDist = isAngry ? 5f : 3f;
            fbStepTime = 12f / 30f / spRate;
            fbStepEaseType = EasingType.SineInOut;
            fbStepIgnoreY = false;
            if (isAngry && approach != 0 && escapePoint <= 0)
            {
                SpecialStep(2.25f, fbStepTime, fbStepMaxDist, 0f, 0f, false, true, fbStepEaseType, true);
            }
            else
            {
                SeparateFromTarget(6.5f);
            }
        }
    }

    void MoveEnd()
    {
        LockonEnd();
        continuousMoveEnabled = false;
    }

    void FinishAttackStart()
    {
        AttackStart(0);
        AttackStart(1);
    }

    void FinishAttackEnd()
    {
        AttackEnd(0);
        AttackEnd(1);
    }

    protected void LightEyeActivate(int param)
    {
        if (!isSuperman)
        {
            SupermanSetMaterial(param != 0);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetAngry(false);
    }

    void SetAngry(bool flag)
    {
        if (flag)
        {
            isAngry = true;
            if (!effect[1].instance)
            {
                EmitEffect(1);
            }
            LightEyeActivate(1);
            if (GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] < 3)
            {
                SetChat(chatKey_Special, 35);
            }
            mesAtkMin = 3;
            mesAtkMax = 5;
            mesDmgLtMin = 4;
            mesDmgLtMax = 6;
            mesDmgHvMin = 5;
            mesDmgHvMax = 7;
            escapePoint = 0;
            sandstarHealMultiplier = justDodgeAmountMultiplier = 1.91f / (4f / 3f);
            if (attackPower > 0)
            {
                sandstarHealMultiplier *= 10f / attackPower;
            }
        }
        else
        {
            isAngry = false;
            if (effect[1].instance)
            {
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
            sandstarHealMultiplier = justDodgeAmountMultiplier = 1.91f;
            if (attackPower > 0)
            {
                sandstarHealMultiplier *= 10f / attackPower;
            }
        }
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        bool damagedFlag = (GameManager.Instance.time < CharacterManager.Instance.damageRecord.time + angryTimeLimit && CharacterManager.Instance.damageRecord.attacker != null && CharacterManager.Instance.damageRecord.receiver != this);
        if (target && state != State.Dead && !isAngry && damagedFlag)
        {
            attackerCB = (damagedFlag ? CharacterManager.Instance.damageRecord.attacker : CharacterManager.Instance.dodgeRecord.attacker);
            if (attackerCB)
            {
                attackerObj = attackerCB.gameObject;
                searchArea[0].SetLockTargetFromCharacter(attackerCB, angryTimeLimit * (damagedFlag ? 1f : 0.5f));
            }
            SetAngry(true);
        }
        if (isAngry && !damagedFlag)
        {
            SetAngry(false);
        }
    }

    protected override float GetSTHealRate()
    {
        return base.GetSTHealRate() * (isAngry ? 2f : 1f);
    }

    protected override void Start_Process_Dead()
    {
        base.Start_Process_Dead();
        SetAngry(false);
    }

    protected override void AttackBody()
    {
        float spRate = isSuperman || isAngry ? 4f / 3f : 1;
        float atkRate = isAngry ? 1.2f : 1f;
        float knockRate = isAngry ? 1.2f : 1f;
        float sqrDist = 0;
        continuousMoveEnabled = false;
        if (targetTrans)
        {
            Vector3 dirTemp = (trans.position - targetTrans.position).normalized;
            sqrDist = ((targetTrans.position + dirTemp * targetRadius) - trans.position).sqrMagnitude;
        }
        if (playerInput.GetButton(RewiredConsts.Action.Special) && JudgeStamina(GetCost(CostType.Skill)))
        {
            if (AttackBase(4, 1.3f, 1.3f, GetCost(CostType.Skill), 57f / 30f / (spBias * spRate), 57f / 30f / (spBias * spRate), 0.5f, spBias * spRate, true, attackLockonDefaultSpeed * 2f))
            {
                S_ParticlePlay(0);
                S_ParticlePlay(1);
                MoveEscape_Wave(0);
                escapePoint -= 2;
                isSkillAttacking = true;
                AttackStartAir();
                attackingMoveReservedTimer = 49f / 30f / (spBias * spRate);
                attackingDodgeReservedTimer = 20f / 30f / (spBias * spRate);
            }
        }
        else
        {
            switch (attackProcess)
            {
                case 0:
                    if (AttackBase(0, 1f * atkRate, 1f * knockRate, GetCost(CostType.Attack), 14f / 30f / spRate, 14f / 30f / spRate, 1f, spRate))
                    {
                        S_ParticlePlay(0);
                        MoveAttack1();
                        escapePoint++;
                        isSkillAttacking = false;
                        AttackStartAir();
                        attackingMoveReservedTimer = 13f / 30f / spRate;
                        attackingDodgeReservedTimer = 13f / 30f / spRate;
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                    break;
                case 1:
                    if (AttackBase(1, 1f * atkRate, 1f * knockRate, GetCost(CostType.Attack), 14f / 30f / spRate, 14f / 30f / spRate, 1f, spRate))
                    {
                        S_ParticlePlay(1);
                        MoveAttack1();
                        escapePoint++;
                        isSkillAttacking = false;
                        AttackStartAir();
                        attackingMoveReservedTimer = 13f / 30f / spRate;
                        attackingDodgeReservedTimer = 13f / 30f / spRate;
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                    break;
                case 2:
                    if (AttackBase(2, 1.35f * atkRate, 1.4f * knockRate, GetCost(CostType.Attack) * (20f / 14f), 20f / 30f / spRate, 20f / 30f / spRate, 1f, spRate))
                    {
                        S_ParticlePlay(0);
                        S_ParticlePlay(1);
                        escapePoint++;
                        isSkillAttacking = false;
                        AttackStartAir();
                        attackingMoveReservedTimer = 18f / 30f / spRate;
                        attackingDodgeReservedTimer = 17f / 30f / spRate;
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                        nowSpeed = Mathf.Min(nowSpeed, 9f);
                    }
                    break;
                case 3:
                    if (AttackBase(3, 2f * atkRate, 2.15f * knockRate, GetCost(CostType.Attack) * ((36f / spBias) / 14f), 36f / 30f / (spBias * spRate), 36f / 30f / (spBias * spRate) + (isAngry ? 0f : 5f / 30f / spRate), 0f, spBias * spRate))
                    {
                        S_ParticlePlay(0);
                        S_ParticlePlay(1);
                        escapePoint++;
                        isSkillAttacking = false;
                        AttackStartAir();
                        attackingMoveReservedTimer = 34f / 30f / (spBias * spRate);
                        attackingDodgeReservedTimer = 31f / 30f / (spBias * spRate);
                    }
                    break;
            }
            attackProcess = (attackProcess + 1) % 4;
        }
        if (isAngry)
        {
            SuperarmorStart();
        }
        escapePoint = Mathf.Clamp(escapePoint, -3, 3);
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        switch (attackType)
        {
            case 0:
            case 1:
            case 2:
                if (isAngry && isLockon && target && groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    Continuous_Approach(6f, 0.4f, 0.1f, true, true, 0.25f);
                }
                break;
            case 3:
                if (continuousMoveEnabled && target && groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    Continuous_Approach(13.5f * (isSuperman || isAngry ? 4f / 3f : 1f), 0.4f, 0.1f, true, true, 0.25f);
                }
                break;
        }
        AttackContinuousAir();
    }

    public override float GetMaxSpeed(bool isWalk = false, bool ignoreSuperman = false, bool ignoreSick = false, bool ignoreConfig = false)
    {
        return base.GetMaxSpeed(isWalk, ignoreSuperman, ignoreSick, ignoreConfig) * (isAngry ? 1.25f : 1f);
    }

    public override float GetKnocked()
    {
        return base.GetKnocked() * (isAngry ? 3f : 1f);
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false)
    {
        if (isAngry && damage > 0)
        {
            damage = Mathf.Max(damage * 4 / 5, 1);
        }
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    void ThrowStartSpecial(int index)
    {
        if (throwing && throwing.throwSettings.Length > index && throwing.throwSettings[index].from)
        {
            if (targetTrans)
            {
                Vector3 targetPos = targetTrans.position;
                float height = GetTargetHeight(true);
                if (height > -0.2f && height < 1.2f)
                {
                    targetPos.y = throwing.throwSettings[index].from.transform.position.y;
                }
                throwing.throwSettings[index].from.transform.LookAt(targetPos);
            }
            else
            {
                throwing.throwSettings[index].from.transform.localRotation = quaIden;
            }
            throwing.ThrowStart(index);
        }
    }

    protected override void ThrowSlash()
    {
        if (throwing && isAngry && state == State.Attack)
        {
            EmitEffect(effectSlash);
            throwing.ThrowStart(slashThrowIndex);
        }
    }
}
