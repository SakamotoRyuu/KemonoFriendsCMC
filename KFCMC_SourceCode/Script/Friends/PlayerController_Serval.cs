using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Serval : PlayerController
{

    public GameObject pileAttackChecker;
    public JumpEffectSetting hyperJumpEffect;


    protected Collider[] _pileAttackCheckerCollider;
    protected CheckTriggerStay _pileAttackCheckerTrigger;
    protected bool _pileAttackCheckerEnabled = true;
    protected float[] costArray = new float[5];

    // Attack Detection
    protected const int rightDet = 0;
    protected const int leftDet = 1;
    protected const int impactDet = 2;
    protected const int screwDet = 3;
    protected const int boosterDet = 4;
    protected const int bootsLeftDet = 5;
    protected const int bootsRightDet = 6;
    // Effect
    protected const int effectPile = 0;
    protected const int effectSpin = 1;
    protected const int effectSpinPlasma = 2;
    protected const int effectBolt = 3;
    protected const int effectPileEnd = 4;
    protected const int effectSlash = 5;
    // Attack Index
    protected const int jumpAttackIndex = 4;
    protected const int pileAttackIndex = 5;
    protected const int spinAttackIndex = 6;
    protected const int waveAttackIndex = 7;
    protected const int boltAttackIndex = 8;
    // Throw Index
    protected const int waveNormalThrowIndex = 0;
    protected const int wavePlasmaThrowIndex = 1;
    protected const int spinNormalThrowIndex = 2;
    protected const int spinPlasmaThrowIndex = 3;
    protected const int boltThrowIndex = 4;
    protected const int judgementThrowIndex = 5;
    protected const int slashRightThrowIndex = 6;
    protected const int slashLeftThrowIndex = 7;
    // Battle Assist
    protected bool forcePile;
    protected bool forceSpin;
    protected bool forceWave;
    protected bool forceBolt;
    protected bool forceScrew;
    protected float waveAttackedTimeRemain;
    protected bool[] attackConditions = new bool[5];

    // Attack Animation
    public static readonly float animStopNormalizedTimeJump = 0.325f;
    public static readonly float animStopNormalizedTimeBolt = 0.3921568f;
    public static readonly float animStopNormalizedTimePile = 0.2933333f;
    public static readonly int animStateHashJump = Animator.StringToHash("Attack 4");
    public static readonly int animStateHashBolt = Animator.StringToHash("Attack 8");
    public static readonly int animStateHashPile = Animator.StringToHash("Attack 5");

    // Combo Rank
    protected const int comboPointNormal1 = 5;
    protected const int comboPointNormal2 = 5;
    protected const int comboPointNormal3 = 6;
    protected const int comboPointNormal4 = 7;
    protected const int comboPointJump = 7;
    protected const int comboPointPile = 8;
    protected const int comboPointSpin = 15;
    protected const int comboPointWave = 10;
    protected const int comboPointBolt = 15;
    protected const int comboPointScrew = 5;

    protected override void Awake()
    {
        base.Awake();
        brakeOnAttacking = false;
        skipHeavyKnockAnimationEnabled = false;
        justDodgeCounterAttackProcess = 2;
        if (pileAttackChecker)
        {
            _pileAttackCheckerCollider = pileAttackChecker.GetComponents<Collider>();
            _pileAttackCheckerTrigger = pileAttackChecker.GetComponent<CheckTriggerStay>();
            _pileAttackCheckerEnabled = true;
            PileAttackCheckerActivate(false);
        }
        if (isHyper)
        {
            superAttackRate = 3f;
            superKnockRate = 3.5f;
        }
    }

    protected float GetPileCost()
    {
        return GetCost(CostType.Skill) * (1f + CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.SpeedRank) * 0.125f); // 16, 18, 20
    }

    protected virtual float GetSpinCost()
    {
        return GetCost(CostType.Skill) * (isPlasma ? 1.5625f : 1.25f); // 20, 25
    }

    protected virtual float GetWaveCost()
    {
        return GetCost(CostType.Skill) * (isPlasma ? 1.25f : 1.0f); // 16, 20
    }

    protected float GetBoltCost()
    {
        return GetCost(CostType.Skill) * 1.875f; // 30
    }

    protected float GetScrewCost()
    {
        return GetCost(CostType.Skill) * 0.625f; // 10
    }

    public override float GetStaminaBorder()
    {
        costArray[0] = GetCost(CostType.Attack) + (state == State.Jump ? 0f : GetCost(CostType.Jump));
        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Pile) != 0)
        {
            costArray[1] = GetPileCost();
        }
        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Spin) != 0)
        {
            costArray[2] = GetSpinCost();
        }
        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Wave) != 0)
        {
            costArray[3] = GetWaveCost();
        }
        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Bolt) != 0 || CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Judgement) != 0)
        {
            costArray[4] = GetBoltCost() + (state == State.Jump ? 0f : GetCost(CostType.Jump));
        }
        float tempMax = costArray[0];
        for (int i = 1; i < costArray.Length; i++)
        {
            if (costArray[i] > tempMax)
            {
                tempMax = costArray[i];
            }
        }
        return tempMax;
    }

    protected void PileAttackCheckerActivate(bool flag)
    {
        if (_pileAttackCheckerEnabled != flag)
        {
            if (_pileAttackCheckerCollider[0])
            {
                for (int i = 0; i < _pileAttackCheckerCollider.Length; i++)
                {
                    _pileAttackCheckerCollider[i].enabled = flag;
                }
            }
            if (_pileAttackCheckerTrigger)
            {
                _pileAttackCheckerTrigger.enabled = flag;
            }
            _pileAttackCheckerEnabled = flag;
        }
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (_pileAttackCheckerEnabled && state != State.Attack)
        {
            PileAttackCheckerActivate(false);
        }
        if (hyperJumpEffect.enabled && state != State.Jump && groundedFlag)
        {
            SetAnimationWing(0);
        }
    }

    protected override void CheckBooster()
    {
        if (GameManager.Instance.save.config[GameManager.Save.configID_ShowArms] > 0 && attackDetection.Length > boosterDet && attackDetection[boosterDet] && nowSpeed >= costRunBaseSpeed * Mathf.Clamp01(1f + GameManager.Instance.save.config[GameManager.Save.configID_FriendsRunningSpeed] * 0.01f) && disableControlTimeRemain <= 0f && !rideTarget)
        {
            boosterFlag = true;
            if (!attackDetection[boosterDet].attackEnabled)
            {
                AttackStart(boosterDet);
            }
        }
    }

    protected override float GetSTHealRateChild_Attack()
    {
        return base.GetSTHealRateChild_Attack() + (isSkillAttacking || isAnimStopped ? 0f : 0.03f);
    }
    protected override float GetSTHealRateChild_Jump()
    {
        if (attackDetection.Length > screwDet && attackDetection[screwDet] && attackDetection[screwDet].attackEnabled)
        {
            return 0f;
        }
        return base.GetSTHealRateChild_Jump();
    }

    public override void AnimationStopTiming()
    {
        if (state == State.Attack && !isAnimEnd && !isAnimStopped)
        {
            switch (attackType)
            {
                case jumpAttackIndex:
                case boltAttackIndex:
                case pileAttackIndex:
                    anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], 0);
                    break;
            }
            switch (attackType)
            {
                case jumpAttackIndex:
                    anim.Play(animStateHashJump, 0, animStopNormalizedTimeJump);
                    break;
                case boltAttackIndex:
                    anim.Play(animStateHashBolt, 0, animStopNormalizedTimeBolt);
                    break;
                case pileAttackIndex:
                    anim.Play(animStateHashPile, 0, animStopNormalizedTimePile);
                    break;
            }
        }
        base.AnimationStopTiming();
    }

    protected void PileAttackEnd()
    {
        float spRate = isSuperman ? 4f / 3f : 1;
        attackStiffTime = Mathf.Max(stateTime, 0.1f) + (5f / 30f) / spRate;
        if (attackedTimeRemain < (7f / 30f) / spRate)
        {
            attackedTimeRemain = (7f / 30f) / spRate;
        }
        attackingMoveReservedTimer = (9f / 60f) / spRate;
        isAnimEnd = true;
        isAnimStopped = false;
        EmitEffect(effectPileEnd);
        PileAttackCheckerActivate(false);
    }

    protected override float GetDefCCStepOffset()
    {
        if (state == State.Attack && attackType == pileAttackIndex)
        {
            return 0.6f;
        }
        return defCCStepOffset;
    }

    protected override float GetDefCCSlopeLimit()
    {
        if (state == State.Attack && attackType == pileAttackIndex)
        {
            return 60f;
        }
        return defCCSlopeLimit;
    }

    void SetAnimationWing(int index)
    {
        for (int i = 0; i < hyperJumpEffect.animationWings.Length; i++)
        {
            if (hyperJumpEffect.animationWings[i])
            {
                hyperJumpEffect.animationWings[i].ChangeRotation(index);
            }
        }
    }

    protected override void PlayHyperEffect()
    {
        if (isHyper && isSuperman && hyperJumpEffect.enabled)
        {
            if (hyperJumpEffect.audioSource)
            {
                if (hyperJumpEffect.audioSource.isPlaying)
                {
                    hyperJumpEffect.audioSource.Stop();
                }
                hyperJumpEffect.audioSource.Play();
            }
            for (int i = 0; i < hyperJumpEffect.particles.Length; i++)
            {
                if (hyperJumpEffect.particles[i])
                {
                    hyperJumpEffect.particles[i].Play();
                }
            }
        }
    }

    protected override bool IsHyperJumpEnabled()
    {
        return (isHyper && isSuperman && hyperJumpEffect.enabled && hyperJumpedTimeRemain <= 0f);
    }

    public override void SupermanStart(bool effectEnable = true)
        {
        base.SupermanStart(effectEnable);
        if (effectEnable)
        {
            PlayHyperEffect();
        }
    }

    protected override void SetJumpAnim()
    {
        if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Screw) != 0 && (playerInput.GetButton(RewiredConsts.Action.Special) || forceScrew) && JudgeStamina(GetScrewCost()))
        {
            nowST -= GetScrewCost();
            SetComboRankAttackPoint(10, comboPointScrew);
            anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 2);
            attackPowerMultiplier = 1f;
            knockPowerMultiplier = 1f;
            isScrewJumping = true;
            if (hyperJumpEffect.enabled)
            {
                SetAnimationWing(1);
            }
        }
        else
        {
            if (attackDetection.Length > screwDet && attackDetection[screwDet] && attackDetection[screwDet].attackEnabled)
            {
                AttackEnd(screwDet);
            }
            if (nowSpeed > GetMaxSpeed(true))
            {
                anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 1);
            }
            else
            {
                anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], jumpType = 0);
            }
            isScrewJumping = false;
            if (hyperJumpEffect.enabled)
            {
                SetAnimationWing(0);
            }
        }
    }

    protected override void SetMultiHitBuff(bool flag)
    {
        if (flag)
        {
            for (int i = 0; i < 2; i++)
            {
                if (attackDetection[i] && (attackDetection[i].multiHitInterval == 0 || attackDetection[i].multiHitInterval > multiHitBuffInterval))
                {
                    attackDetection[i].multiHitInterval = multiHitBuffInterval;
                }
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                if (attackDetection[i] && attackDetection[i].multiHitInterval != defaultMultiHitInterval[i])
                {
                    attackDetection[i].multiHitInterval = defaultMultiHitInterval[i];
                }
            }
        }
        isMultiBuff = flag;
    }

    protected override void PlayerJump()
    {
        base.PlayerJump();
        hyperJumpedTimeRemain = 0.4f;
        PlayHyperEffect();
    }

    protected override void QuickTurn()
    {
        base.QuickTurn();
        hyperJumpedTimeRemain = 0.4f;
        if (hyperJumpEffect.enabled)
        {
            SetAnimationWing(0);
        }
    }

    protected override void QuickJump()
    {
        base.QuickJump();
        if (hyperJumpEffect.enabled)
        {
            SetAnimationWing(0);
            PlayHyperEffect();
        }
    }

    protected override void ScrewStart()
    {
        AttackStart(screwDet);
    }

    protected override void ScrewEnd()
    {
        AttackEnd(screwDet);
    }

    protected override void ImpactAttack()
    {
        if (Time.timeScale > 0f && attackDetection.Length > impactDet && attackDetection[impactDet])
        {
            float conditionSpeed = impactAttackConditionSpeed * GameManager.Instance.megatonCoinSpeedMul;
            if (GetCanControl() && groundedFlag && (state == State.Wait || state == State.Chase || state == State.Escape))
            {
                if (nowSpeed >= conditionSpeed - 0.01f)
                {
                    impactAttackDuration = Mathf.Clamp(impactAttackDuration + deltaTimeCache, 0f, 0.1f);
                }
                else if (nowSpeed < conditionSpeed - 0.5f && nowSpeed > walkSpeed)
                {
                    impactAttackDuration = Mathf.Clamp(impactAttackDuration - deltaTimeCache, 0f, 0.1f);
                }
                else if (nowSpeed <= walkSpeed)
                {
                    impactAttackDuration = 0f;
                }
            }
            else
            {
                impactAttackDuration = 0f;
            }
            if (impactAttackDuration > 0f)
            {
                float plusMul = Mathf.Clamp(nowSpeed / conditionSpeed, 1f, 10f);
                attackDetection[impactDet].attackRate = 1.5f * plusMul;
                attackDetection[impactDet].knockRate = 2.4f * plusMul;
                if (!attackDetection[impactDet].attackEnabled)
                {
                    AttackStart(impactDet);
                }
            }
            else
            {
                if (attackDetection[impactDet].attackEnabled)
                {
                    AttackEnd(impactDet);
                }
            }
        }
    }

    protected override void PlayerControl_BattleAssist()
    {
        if (state != State.Jump)
        {
            heightAdjustTimer += deltaTimeCache;
            if (heightAdjustTimer > 10f)
            {
                heightAdjustTimer = 10f;
            }
        }
        if (targetTrans)
        {
            float heightDist = targetTrans.position.y - targetRadius - trans.position.y;
            if (heightDist >= (1.2f - heightAdjustTimer * 0.04f))
            {
                heightDistConditionTime += deltaTimeCache;
            }
            else
            {
                heightDistConditionTime -= deltaTimeCache;
            }
            heightDistConditionTime = Mathf.Clamp01(heightDistConditionTime);
        }
        else
        {
            heightDistConditionTime = 0f;
        }
        if (waveAttackedTimeRemain > 0f)
        {
            waveAttackedTimeRemain = Mathf.Clamp(waveAttackedTimeRemain - deltaTimeCache, 0f, 10f);
        }
        if (state != State.Dead && !checkPaused && GameManager.Instance.save.difficulty <= GameManager.difficultyNT && GameManager.Instance.save.config[GameManager.Save.configID_BattleAssist] != 0 && playerInput.GetButtonDown(RewiredConsts.Action.Attack) && !playerInput.GetButton(RewiredConsts.Action.Special))
        {
            assistTimeRemain = 0.2f;
            if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Dodge) != 0)
            {
                assistDodgeTimeRemain = 0.6f;
            }
            if (!isSuperman && CharacterManager.Instance.GetSandstarIsMax() && targetTrans && (CharacterManager.Instance.isBossBattle || CharacterManager.Instance.GetActionEnemyCount() >= 4))
            {
                forceWildReleaseOn = true;
            }
            else if (isSuperman && !targetTrans && !CharacterManager.Instance.isBossBattle && CharacterManager.Instance.GetActionEnemyCount() <= 0)
            {
                forceWildReleaseOff = true;
            }
        }
    }

    protected override void PlayerControl_CheckScrew()
    {
        if (attackDetection.Length > screwDet && attackDetection[screwDet] && attackDetection[screwDet].attackEnabled)
        {
            if (attackedTimeRemain < 0)
            {
                attackedTimeRemain = 0;
            }
            if (state != State.Jump)
            {
                AttackEnd(screwDet);
            }
        }
    }

    protected override void PlayerControl_CheckBooster()
    {
        if (attackDetection.Length > boosterDet && attackDetection[boosterDet] && !boosterFlag && attackDetection[boosterDet].attackEnabled)
        {
            AttackEnd(boosterDet);
        }
    }

    protected override void SpaceJumpBoots()
    {
        if (GameManager.Instance.save.config[GameManager.Save.configID_ShowArms] > 0 && attackDetection.Length > bootsLeftDet && attackDetection.Length > bootsRightDet && attackDetection[bootsLeftDet] && attackDetection[bootsRightDet])
        {
            AttackEnd(bootsLeftDet);
            AttackEnd(bootsRightDet);
            AttackStart(bootsLeftDet);
            AttackStart(bootsRightDet);
        }
    }

    protected override bool LightningBolt()
    {
        if (isSuperman && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Judgement) != 0 && playerInput.GetButton(RewiredConsts.Action.Special) && (playerInput.GetButton(RewiredConsts.Action.Attack) || GameManager.Instance.save.config[GameManager.Save.configID_SimplifySkillCommand] != 0))
        {
            throwing.ThrowStart(judgementThrowIndex);
            CharacterManager.Instance.AddSandstar(judgementAttackSandstarCost, true);
            t_JudgementDefeatCount = 0;
            if (comboRankAttackPoint > comboPointBolt)
            {
                comboRankAttackPoint = comboPointBolt;
            }
            return true;
        }
        else
        {
            throwing.ThrowStart(boltThrowIndex);
            return false;
        }
    }

    protected override void ThrowWave()
    {
        if (!isItem)
        {
            if (isPlasma)
            {
                throwing.ThrowStart(wavePlasmaThrowIndex);
            }
            else
            {
                throwing.ThrowStart(waveNormalThrowIndex);
            }
        }
    }

    protected override void ThrowSpin()
    {
        if (!isItem)
        {
            if (isPlasma)
            {
                throwing.ThrowReady(spinPlasmaThrowIndex);
            }
            else
            {
                throwing.ThrowReady(spinNormalThrowIndex);
            }
        }
    }

    protected override void ThrowSlash()
    {
        if (isHyper && isSuperman)
        {
            EmitEffect(effectSlash);
            throwing.ThrowStart(slashRightThrowIndex);
            throwing.ThrowStart(slashLeftThrowIndex);
        }
    }

    protected override bool IsAttackStartEnabled(int index)
    {
        return state != State.Dodge && (state != State.Jump || (index != rightDet && index != leftDet));
    }

    public override void AttackStart(int index)
    {
        if (bothHandAttacking && index == leftDet)
        {
            attackEffectEnabled = false;
        }
        else
        {
            attackEffectEnabled = true;
        }
        if (attackType == 3 && bothHandAttacking)
        {
            attackPowerMultiplier = bothHandAttackMul;
            knockPowerMultiplier = bothHandKnockMul;
        }
        base.AttackStart(index);
    }

    protected override void BattleAssist()
    {
        forcePile = false;
        forceSpin = false;
        forceWave = false;
        forceBolt = false;
        forceScrew = false;
        if (assistTimeRemain > 0f && targetTrans)
        {
            int assistIndex = -1;
            float sqrDist = GetTargetDistance(true, true, true);
            bool canJump = !GetSick(SickType.Mud);
            if (groundedFlag)
            {
                if (attackProcess == 0 || (attackProcess == 1 && attackTypeSave != 0))
                {
                    int assistMin = playerInput.GetButton(RewiredConsts.Action.Special) ? 1 : 0;
                    if (sqrDist >= 6f * 6f || (sqrDist >= 4f * 4f && Random.Range(0, 100) < 75))
                    {
                        int randTemp = Random.Range(0, 2);
                        if (randTemp == 0)
                        {
                            assistIndex = 2;
                        }
                        else
                        {
                            assistIndex = 4;
                        }
                    }
                    else if (heightDistConditionTime >= 0.4f && Random.Range(0, 100) < 75 && canJump)
                    {
                        assistIndex = 1;
                    }
                    else if (searchArea[0].IsBesieged(isSuperman ? 4f : 3f))
                    {
                        assistIndex = (Random.Range(0, 100) < 50 ? 1 : 3);
                    }
                    else
                    {
                        assistIndex = Random.Range(assistMin, 5);
                        if (assistIndex == assistIndexSave)
                        {
                            assistIndex = Random.Range(assistMin, 5);
                        }
                        if (assistIndex == 1 && !canJump)
                        {
                            assistIndex = Random.Range(assistMin, 4);
                            if (assistIndex >= 1)
                            {
                                assistIndex += 1;
                            }
                        }
                    }

                    for (int i = 0; i < attackConditions.Length; i++)
                    {
                        attackConditions[i] = true;
                    }
                    if (!JudgeStamina(GetCost(CostType.Jump) + GetCost(CostType.Run) + GetCost(CostType.Attack)))
                    {
                        attackConditions[1] = false;
                    }
                    if (!JudgeStamina(GetPileCost()) || sqrDist <= 2f * 2f || CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Pile) == 0)
                    {
                        attackConditions[2] = false;
                    }
                    if (!JudgeStamina(GetSpinCost()) || CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Spin) == 0)
                    {
                        attackConditions[3] = false;
                    }
                    if (!JudgeStamina(GetWaveCost()) || (sqrDist < MyMath.Square(1.5f + waveAttackedTimeRemain * 0.5f) && Random.Range(0, 100) < 75) || CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Wave) == 0)
                    {
                        attackConditions[4] = false;
                    }
                    for (int i = 0; i < 5 && attackConditions[assistIndex] == false; i++)
                    {
                        assistIndex = Random.Range(assistMin, 5);
                    }
                    if (attackConditions[assistIndex] == false)
                    {
                        assistIndex = 0;
                    }
                }

                assistIndexSave = assistIndex;

                if (assistIndex == 1)
                {
                    lastJumpButtonTime = 0f;
                    heightAdjustTimer = 0f;
                    if (inputMagMoment < 0.1f)
                    {
                        assistJumpingMoveTimeRemain = 0.9f;
                    }
                    if (sqrDist < 1.2f * 1.2f && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Screw) != 0)
                    {
                        forceScrew = true;
                    }
                }
                else if (assistIndex == 2)
                {
                    forcePile = true;
                }
                else if (assistIndex == 3)
                {
                    forceSpin = true;
                }
                else if (assistIndex == 4)
                {
                    forceWave = true;
                }
            }
            else
            {
                if (JudgeStamina(GetBoltCost()))
                {
                    forceBolt = true;
                }
            }
        }
    }

    protected override void AttackBody()
    {
        float spRate = isSuperman ? 4f / 3f : 1;
        bool lockonEnabled = (CharacterManager.Instance.autoAim != 0 || searchArea[0].isLocking);
        if (!groundedFlag)
        {
            if ((CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Bolt) != 0 || (isSuperman && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Judgement) != 0)) && (playerInput.GetButton(RewiredConsts.Action.Special) || forceBolt) && JudgeStamina(GetBoltCost()))
            {
                if (AttackBase(8, 1, 1, GetBoltCost(), 5f, 15f / 30f / spRate, 0.2f, 1, lockonEnabled, 20f))
                {
                    S_ParticlePlay(0);
                    attackProcess = 1;
                    attackBiasValue = 0;
                    isSkillAttacking = true;
                    assistJumpingMoveTimeRemain = 0f;
                    obstacleDisableTimeRemain = 1f;
                    SuperarmorStart();
                    EmitEffect(effectBolt);
                    if (move.y > -0.25f)
                    {
                        move.y = -0.25f;
                    }
                    SetComboRankAttackPoint(8, comboPointBolt);
                }
            }
            else
            {
                if (AttackBase(4, 1, 2, GetCost(CostType.Attack), 5f, 15f / 30f / spRate, 0.2f, 1, lockonEnabled, 20f))
                {
                    S_ParticlePlay(0);
                    attackProcess = 1;
                    attackBiasValue = 0;
                    isSkillAttacking = false;
                    assistJumpingMoveTimeRemain = 0f;
                    obstacleDisableTimeRemain = 1f;
                    if (move.y > -0.25f)
                    {
                        move.y = -0.25f;
                    }
                    SetComboRankAttackPoint(4, comboPointJump);
                }
            }
        }
        else
        {
            if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Pile) != 0 && ((playerInput.GetButton(RewiredConsts.Action.Special) && axisInput.y > 0.5f) || forcePile) && JudgeStamina(GetPileCost()))
            {
                if (AttackBase(pileAttackIndex, 1.5f, 1.5f, GetPileCost(), 15f / 30f / spRate, 15f / 30f / spRate, 0, spRate, lockonEnabled, 20f))
                {
                    PerfectLockon();
                    S_ParticlePlay(0);
                    attackBiasValue = 0;
                    attackProcess = 1;
                    PileAttackCheckerActivate(true);
                    pileDuration = 0f;
                    isSkillAttacking = true;
                    EmitEffect(effectPile);
                    SetComboRankAttackPoint(pileAttackIndex, comboPointPile);
                }
            }
            else if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Wave) != 0 && (playerInput.GetButton(RewiredConsts.Action.Dodge) || (playerInput.GetButton(RewiredConsts.Action.Special) && axisInput.y < -0.5f) || forceWave) && JudgeStamina(GetWaveCost()))
            {
                spRate = isSuperman ? 4f / 3f : 10f / 9f;
                if (AttackBase(waveAttackIndex, 1f, 1f, GetWaveCost(), 24f / 30f / spRate, 24f / 30f / spRate, 0, spRate, lockonEnabled, 24f))
                {
                    S_ParticlePlay(0);
                    attackProcess = 1;
                    isSkillAttacking = true;
                    attackingMoveReservedTimer = 18f / 30f / spRate;
                    SetComboRankAttackPoint(waveAttackIndex, comboPointWave);
                }
            }
            else if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Spin) != 0 && (playerInput.GetButton(RewiredConsts.Action.Special) || forceSpin) && JudgeStamina(GetSpinCost()))
            {
                attackBiasValue = 0;
                const float spinSp = 3f / 4f;
                if (AttackBase(spinAttackIndex, 1f, 1f, GetSpinCost(), 20f / 30f / spinSp / spRate, 20f / 30f / spinSp / spRate, 0.5f, spinSp * spRate, false))
                {
                    S_ParticlePlay(0);
                    S_ParticlePlay(1);
                    attackProcess = 0;
                    isSkillAttacking = true;
                    SuperarmorStart();
                    EmitEffect(isPlasma ? effectSpinPlasma : effectSpin);
                    attackingMoveReservedTimer = 18f / 30f / spRate;
                    SetComboRankAttackPoint(spinAttackIndex, comboPointSpin);
                }
            }
            else if (attackProcess == 0)
            {
                if (AttackBase(0, 1, 1, GetCost(CostType.Attack), 10f / 30f / spRate, 10f / 30f / spRate, 1f, spRate, lockonEnabled))
                {
                    S_ParticlePlay(0);
                    attackProcess = 1;
                    isSkillAttacking = false;
                    attackingMoveReservedTimer = 7f / 30f / spRate;
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge))
                    {
                        SpecialStep(0.4f, 0.25f / spRate, dodgeCancelCounterAttack ? 4f : 1f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                    }
                    SetComboRankAttackPoint(0, comboPointNormal1, 1);
                }
            }
            else if (attackProcess == 1)
            {
                if (AttackBase(1, 1, 1, GetCost(CostType.Attack), 10f / 30f / spRate, 10f / 30f / spRate, 1f, spRate, lockonEnabled))
                {
                    S_ParticlePlay(1);
                    attackProcess = (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Combo) != 0 ? 2 : 0);
                    isSkillAttacking = false;
                    attackingMoveReservedTimer = 7f / 30f / spRate;
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge))
                    {
                        SpecialStep(0.4f, 0.25f / spRate, dodgeCancelCounterAttack ? 4f : 1f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                    }
                    SetComboRankAttackPoint(1, comboPointNormal2, 1);
                }
            }
            else if (attackProcess == 2)
            {
                if (AttackBase(2, 1, 0.75f, GetCost(CostType.Attack) * (7f / 6f), 12f / 30f / spRate, 12f / 30f / spRate, 1f, spRate, lockonEnabled))
                {
                    S_ParticlePlay(0);
                    S_ParticlePlay(1);
                    attackProcess = 3;
                    isSkillAttacking = false;
                    attackingMoveReservedTimer = 7f / 30f / spRate;
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge))
                    {
                        SpecialStep(0.4f, 0.25f / spRate, dodgeCancelCounterAttack ? 4f : 1f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                    }
                    SetComboRankAttackPoint(2, comboPointNormal3, 1);
                }
            }
            else if (attackProcess == 3)
            {
                if (AttackBase(3, 1f, 0.75f, GetCost(CostType.Attack) * (7f / 6f), 14f / 30f / spRate, 14f / 30f / spRate, 1f, spRate, lockonEnabled))
                {
                    S_ParticlePlay(0);
                    S_ParticlePlay(1);
                    attackProcess = 0;
                    isSkillAttacking = false;
                    attackingMoveReservedTimer = 7f / 30f / spRate;
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge))
                    {
                        SpecialStep(0.4f, 0.25f / spRate, dodgeCancelCounterAttack ? 4f : 1f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                    }
                    SetComboRankAttackPoint(3, comboPointNormal4, 1);
                }
            }
        }
        if (attackType == 3)
        {
            if (attackDetection[rightDet] && attackDetection[leftDet])
            {
                attackDetection[rightDet].relationIndex = leftDet;
                attackDetection[leftDet].relationIndex = rightDet;
                bothHandAttacking = true;
                attackDetection[rightDet].offset = attackOffsetBothHand;
                attackDetection[leftDet].offset = attackOffsetNormal;
            }
        }
        else
        {
            if (attackDetection[rightDet] && attackDetection[leftDet])
            {
                attackDetection[rightDet].relationIndex = -1;
                attackDetection[leftDet].relationIndex = -1;
                attackDetection[rightDet].offset = attackOffsetNormal;
                attackDetection[leftDet].offset = attackOffsetNormal;
            }
            bothHandAttacking = false;
        }
        attackTypeSave = attackType;
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        float addValue = 0f;
        float spRate = isSuperman ? 4f / 3f : 1;
        switch (attackType)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                gravityMultiplier = 1f;
                if (nowSpeed > GetMaxSpeed(true, false, false, true))
                {
                    nowSpeed -= GetAcceleration() * deltaTimeMove;
                }
                break;
            case jumpAttackIndex:
            case boltAttackIndex:
                gravityMultiplier = 3.8f;
                attackBiasValue = Mathf.Min(attackBiasValue, move.y);
                if (attackType == jumpAttackIndex)
                {
                    addValue = Mathf.Max(0f, (attackBiasValue + 5f) * (-0.1f));
                    attackPowerMultiplier = 1.25f + addValue * (isSuperman ? 1.5f : 1f);
                    knockPowerMultiplier = 1.25f + addValue * 2f;
                    if (FootstepManager.Instance)
                    {
                        FootstepManager.Instance.SetActionType(0);
                        footstepJumpTimeRemain = 0.4f;
                    }
                    if (comboRankAttackPoint > 0)
                    {
                        comboRankAttackPoint = comboPointJump + Mathf.Clamp(Mathf.RoundToInt(addValue * 10), 0, comboPointJump * 4);
                    }
                }
                else
                {
                    addValue = Mathf.Max(0f, (attackBiasValue + 8f) * (-0.1f));
                    attackPowerMultiplier = 1.25f + addValue * (isSuperman ? 1.5f : 1f);
                    knockPowerMultiplier = 1f + addValue * 2f;
                    if (FootstepManager.Instance)
                    {
                        FootstepManager.Instance.SetActionType(0);
                        footstepJumpTimeRemain = 0.7f;
                    }
                    if (comboRankAttackPoint > 0)
                    {
                        comboRankAttackPoint = comboPointBolt + Mathf.Clamp(Mathf.RoundToInt(addValue * 5), 0, comboPointBolt);
                    }
                }
                lockonRotSpeed = Mathf.Clamp(20f - stateTime * 50f, 8f, 20f);
                if ((stateTime >= 0.2f && groundedFlag) || stateTime > 3f || (stateTime >= 0.6f && Time.timeScale > 0f && trans.position.y >= jumpAttackPosSave))
                {
                    float plusStiff = 4f / 30f;
                    float plusInterval = 4f / 30f;
                    if (attackType == boltAttackIndex)
                    {
                        LightningBolt();
                        plusStiff = 12f / 30f;
                        plusInterval = 16f / 30f;
                    }
                    lockonRotSpeed = 8f;
                    attackType *= -1;
                    attackProcess = 1;
                    attackingMoveReservedTimer = plusStiff;
                    attackStiffTime = stateTime + plusInterval;
                    obstacleDisableTimeRemain = plusStiff * 0.5f;
                    attackedTimeRemain = plusInterval;
                    nowSpeed = 0;
                    gravityMultiplier = 1;
                }
                else
                {
                    attackStiffTime = stateTime + 1;
                    obstacleDisableTimeRemain = 1f;
                }
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], (attackType != 4 && attackType != 8) || !isAnimStopped ? spRate : 0);
                if (Time.timeScale > 0f)
                {
                    jumpAttackPosSave = trans.position.y;
                }
                break;
            case pileAttackIndex:
                {
                    gravityMultiplier = 1f;
                    float hyperRate = (isHyper ? 1.5f : 1f);
                    float maxSpTemp = GetMaxSpeed(false, false, false, true) * hyperRate;
                    float pileMaxSpeed = maxSpTemp * 1.3f + 8f;
                    // float pileAttackAddValueMax = 1.6f + CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.SpeedRank) * 0.4f + (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Speed) ? 1f : 0f) + (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Speed) != 0f ? 0.1f : 0f);

                    if (!isAnimEnd)
                    {
                        if (stateTime > 0.9f)
                        {
                            PileAttackEnd();
                        }
                        else if (_pileAttackCheckerTrigger && _pileAttackCheckerEnabled && _pileAttackCheckerTrigger.stayFlag)
                        {
                            PileAttackEnd();
                        }
                        else if (nowSpeed > 0f && targetTrans && GetTargetDistance(true, true, false) < Mathf.Clamp(MyMath.Square(nowSpeed * 0.02f), 0.01f, 0.36f))
                        {
                            float targetHeight = GetTargetHeight(true);
                            if (targetHeight > 1.2f || targetHeight <= 0f)
                            {
                                PileAttackEnd();
                            }
                        }
                    }
                    if (!isAnimEnd)
                    {
                        if (nowSpeed < pileMaxSpeed * 0.4f)
                        {
                            nowSpeed = pileMaxSpeed * 0.4f;
                        }
                        if (nowSpeed < pileMaxSpeed)
                        {
                            nowSpeed += pileMaxSpeed * hyperRate * deltaTimeMove;
                            if (nowSpeed > pileMaxSpeed)
                            {
                                nowSpeed = pileMaxSpeed;
                            }
                        }
                        // attackStiffTime = Mathf.Max(stateTime, 0.1f) + (5f / 30f) / spRate;
                        attackStiffTime = Mathf.Max(stateTime, 0.1f, deltaTimeMove) + (7f / 30f) / spRate;
                        if (attackedTimeRemain < (7f / 30f) / spRate)
                        {
                            attackedTimeRemain = (7f / 30f) / spRate;
                        }
                        attackProcess = 1;
                    }
                    else
                    {
                        nowSpeed = 0f;
                    }
                    ignoreMegatonCoin = true;
                    float maxSpPower = GetMaxSpeed(false, false, false, true) * hyperRate;
                    ignoreMegatonCoin = false;
                    attackBiasValue = Mathf.Clamp01(Mathf.Max(0f, pileDuration - (isHyper && isSuperman ? 0.1f : 0.2f)) * 2.5f);
                    attackPowerMultiplier = Mathf.Max(1.5f, 1.5f + (maxSpPower - 2.25f) / 4.5f * attackBiasValue);
                    knockPowerMultiplier = Mathf.Max(1.5f, 1.5f + (maxSpPower - 3.375f) * 4f / 9f * attackBiasValue);
                    if (!isAnimStopped)
                    {
                        lockonRotSpeed = 20f * Mathf.Max(1f, pileMaxSpeed / 19.7f) * hyperRate * hyperRate;
                    }
                    else if (!isAnimEnd)
                    {
                        lockonRotSpeed = (20f - Mathf.Clamp(nowSpeed / pileMaxSpeed * 6f, 0f, 6f)) * Mathf.Max(1f, pileMaxSpeed / 19.7f) * hyperRate * hyperRate;
                    }
                    else
                    {
                        lockonRotSpeed = 0f;
                    }
                    if (!isAnimEnd)
                    {
                        pileDuration += deltaTimeMove * hyperRate;
                    }
                    anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], isAnimEnd || !isAnimStopped ? spRate : 0f);
                    if (comboRankAttackPoint > 0)
                    {
                        comboRankAttackPoint = comboPointPile + Mathf.RoundToInt(attackBiasValue * comboPointPile * 2);
                    }
                }
                break;
        }
    }

    public override void Event_PlayerJumping(float height, float moveVelocity)
    {
        if (attackDetection.Length > screwDet && attackDetection[screwDet] && attackDetection[screwDet].attackEnabled)
        {
            AttackEnd(screwDet);
        }
        base.Event_PlayerJumping(height, moveVelocity);
    }

    public override void Event_SetHyperJumpEffectVolume(float volume)
    {
        if (hyperJumpEffect.audioSource)
        {
            hyperJumpEffect.audioSource.volume = volume;
        }
    }

    public override void Event_SetHyperJumpEffectEnabled(bool enabled)
    {
        hyperJumpEffect.enabled = enabled;
    }

    public override void SetHyper(bool activate = true) { }

    public bool IsWaveAttacking
    {
        get
        {
            return state == State.Attack && attackType == waveAttackIndex;
        }
    }

}

