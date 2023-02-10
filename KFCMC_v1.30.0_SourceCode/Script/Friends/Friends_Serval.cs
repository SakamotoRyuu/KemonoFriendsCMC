using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Serval : FriendsBase
{

    public GameObject pileAttackChecker;
    public bool isHyper;

    private bool isSkillAttacking;
    private bool isAnimEnd;
    private bool refresh;
    private float heightDistConditionTime;
    private float landingTime;
    private int attackIndexSave = -1;
    private int attackTypeSave = -1;
    private float addValue;
    private Collider[] _pileAttackCheckerCollider;
    private CheckTriggerStay _pileAttackCheckerTrigger;
    private bool _pileAttackCheckerEnabled = true;
    private float pileDuration;
    private float jumpAttackPosSave;
    private float waveAttackedTimeRemain;
    private float impactAttackDuration;
    private float heightAdjustTimer;
    private bool bothHandAttacking;

    private const int effectPile = 0;
    private const int effectSpin = 1;
    private const int effectJump = 2;
    private const int effectBolt = 3;
    private const int effectSlash = 4;
    private const int effectPileSuper = 5;
    private const int effectSpinSuper = 6;
    private const int effectSacrifice = 7;
    private const int effectDodge = 8;

    protected const int rightDet = 0;
    protected const int leftDet = 1;
    private const int impactDet = 2;
    private const int screwDet = 3;
    private const int boosterDet = 4;
    private const int bootsLeftDet = 5;
    private const int bootsRightDet = 6;

    private const int waveThrowIndex = 0;
    private const int spinThrowIndex = 1;
    private const int boltThrowIndex = 2;
    private const int waveSuperThrowIndex = 3;
    private const int spinSuperThrowIndex = 4;
    private const int boltSuperThrowIndex = 5;
    private const int slashRightThrowIndex = 6;
    private const int slashLeftThrowIndex = 7;
    private const float impactAttackConditionSpeed = 16.875f;
    protected Vector3 attackOffsetNormal = new Vector3(0f, 0f, 1.2f);
    protected Vector3 attackOffsetBothHand = new Vector3(-0.22f, 0f, 1.2f);
    protected float bothHandAttackMul = 1.6f;
    protected float bothHandKnockMul = 2.5f;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            deadTimer = 6;
            chatAttackCount = 6;
            moveCost.attack = 12f * staminaCostRate;
            moveCost.step = 0;
            moveCost.quick = 10;
            moveCost.jump = 10;
            moveCost.skill = 16;
            dodgeDistance = 4.5f;
            dodgeMutekiTime = 1f;
            lightMutekiTime = lightStiffTime + 0.3f;
            heavyMutekiTime = heavyStiffTime + 0.6f;
            if (isHyper) {
                superAttackRate = 3f;
                superKnockRate = 3.5f;
            } else {
                superAttackRate = 1.5f;
                superKnockRate = 1.75f;
            }
            if (pileAttackChecker) {
                _pileAttackCheckerCollider = pileAttackChecker.GetComponents<Collider>();
                _pileAttackCheckerTrigger = pileAttackChecker.GetComponent<CheckTriggerStay>();
                _pileAttackCheckerEnabled = true;
                PileAttackCheckerActivate(false);
            }
        }
    }

    protected override void OnEnable() {
        base.OnEnable();
        if (transform.parent == null) {
            CheckWeapon();
        }
    }

    protected override void Update() {
        base.Update();
        if ((PauseController.Instance && PauseController.Instance.pauseGame) || checkWeaponPreservedFlag) {
            if (CheckWeapon()) {
                SetMultiHitBuff(CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Multi));
            }
        }
    }    

    public override void CounterDodge(int dodgeDir, bool changeState = true) {
        base.CounterDodge(dodgeDir, changeState);
        float maxTemp = GetMaxST();
        nowST += maxTemp * 0.1f;
        if (nowST > maxTemp) {
            nowST = maxTemp;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (state != State.Attack) {
            isSkillAttacking = false;
        }
        if (attackDetection.Length > impactDet && attackDetection[impactDet]) {
            if (GetCanControl() && groundedFlag && state == State.Wait && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Impact) != 0) {
                if (nowSpeed >= impactAttackConditionSpeed - 0.01f) {
                    impactAttackDuration = Mathf.Clamp(impactAttackDuration + deltaTimeCache, 0f, 0.1f);
                } else if (nowSpeed < impactAttackConditionSpeed - 0.5f && nowSpeed > walkSpeed) {
                    impactAttackDuration = Mathf.Clamp(impactAttackDuration - deltaTimeCache, 0f, 0.1f);
                } else if (nowSpeed <= walkSpeed) {
                    impactAttackDuration = 0f;
                }
            } else {
                impactAttackDuration = 0f;
            }
            if (impactAttackDuration > 0f) {
                float plusMul = Mathf.Clamp(nowSpeed / impactAttackConditionSpeed, 1f, 10f);
                attackDetection[impactDet].attackRate = 1.2f * plusMul;
                attackDetection[impactDet].knockRate = 2.4f * plusMul;
                if (!attackDetection[impactDet].attackEnabled) {
                    AttackStart(impactDet);
                }
            } else {
                if (attackDetection[impactDet].attackEnabled) {
                    AttackEnd(impactDet);
                }
            }
        }
        if (attackDetection.Length > screwDet && attackDetection[screwDet] && attackDetection[screwDet].attackEnabled) {
            if (attackedTimeRemain < 0) {
                attackedTimeRemain = 0;
            }
            if (state != State.Jump) {
                AttackEnd(screwDet);
            }
        }

        if (attackDetection[boosterDet]) {
            if (GameManager.Instance.save.config[GameManager.Save.configID_ShowArms] > 0 && groundedFlag && GetCanControl() && nowSpeed >= costRunBaseSpeed * Mathf.Clamp01(1f + GameManager.Instance.save.config[GameManager.Save.configID_FriendsRunningSpeed] * 0.01f)) {
                if (!attackDetection[boosterDet].attackEnabled) {
                    AttackStart(boosterDet);
                }
            } else {
                if (attackDetection[boosterDet].attackEnabled) {
                    AttackEnd(boosterDet);
                }
            }
        }

        if (state != State.Jump && landingTime > 0) {
            landingTime -= deltaTimeMove;
        }
        if (state != State.Jump) {
            heightAdjustTimer += deltaTimeCache;
            if (heightAdjustTimer > 10f) {
                heightAdjustTimer = 10f;
            }
        }
        if (targetTrans) {
            float heightDist = targetTrans.position.y - targetRadius - trans.position.y;
            if (heightDist >= (1.2f - heightAdjustTimer * 0.04f)) {
                heightDistConditionTime += deltaTimeCache;
            } else {
                heightDistConditionTime -= deltaTimeCache;
            }
            heightDistConditionTime = Mathf.Clamp01(heightDistConditionTime);
        } else {
            heightDistConditionTime = 0f;
        }
        if (waveAttackedTimeRemain > 0f) {
            waveAttackedTimeRemain = Mathf.Clamp(waveAttackedTimeRemain - deltaTimeCache, 0f, 10f);
        }
    }

    protected override void Update_Transition_Moves() {
        if (refresh && nowST >= GetMaxST() * 0.9f) {
            refresh = false;
            actDistNum = 0;
        } else if (!refresh && !JudgeStamina(GetCost(CostType.Skill) * 1.5625f)) {
            refresh = true;
            actDistNum = 1;
        }
        base.Update_Transition_Moves();
    }

    void PileAttackCheckerActivate(bool flag) {
        if (_pileAttackCheckerEnabled != flag) {
            if (_pileAttackCheckerCollider[0]) {
                for (int i = 0; i < _pileAttackCheckerCollider.Length; i++) {
                    _pileAttackCheckerCollider[i].enabled = flag;
                }
            }
            if (_pileAttackCheckerTrigger) {
                _pileAttackCheckerTrigger.enabled = flag;
            }
            _pileAttackCheckerEnabled = flag;
        }
    }

    protected override void ScrewStart() {
        AttackStart(screwDet);
    }

    protected override void ScrewEnd() {
        AttackEnd(screwDet);
    }

    float GetPileCost() {
        return GetCost(CostType.Skill) * (1f + CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.SpeedRank) * 0.125f);
    }

    float GetSpinCost() {
        return GetCost(CostType.Skill) * (isSuperman ? 2.5f : 1.5625f);
    }

    float GetWaveCost() {
        return GetCost(CostType.Skill) * (isSuperman ? 1.5625f : 0.9375f);
    }

    float GetBoltCost() {
        return GetCost(CostType.Skill) * 1.875f;
    }

    float GetScrewCost() {
        return GetCost(CostType.Skill) * 0.625f;
    }

    protected override void Attack() {
        if (targetTrans) {
            float spRate = isSuperman ? 4f / 3f : 1f;
            float sqrDist = GetTargetDistance(true, true, true);
            int attackIndex = 0;
            bool canJump = !GetSick(SickType.Mud);
            isAnimStopped = false;
            isAnimEnd = false;
            if (groundedFlag) {
                if (attackProcess == 0 || (attackProcess == 1 || attackTypeSave != 0)) {
                    if (sqrDist >= 8f * 8f || (sqrDist >= 5 * 5 && Random.Range(0, 100) < 75)) {
                        int randTemp = Random.Range(0, 2);
                        if (randTemp == 0) {
                            attackIndex = 2;
                        } else {
                            attackIndex = 4;
                        }
                    } else if (heightDistConditionTime >= 0.4f && Random.Range(0, 100) < 75 && canJump) {
                        attackIndex = 1;
                    } else {
                        attackIndex = Random.Range(0, 5);
                        if (attackIndex == attackIndexSave) {
                            attackIndex = Random.Range(0, 5);
                        }
                        if (attackIndex == 1 && !canJump) {
                            attackIndex = Random.Range(0, 4);
                            if (attackIndex >= 1) {
                                attackIndex += 1;
                            }
                        }
                    }
                    if (attackIndex == 1 && !JudgeStamina(GetCost(CostType.Jump) + GetCost(CostType.Run) + GetCost(CostType.Attack))) {
                        attackIndex = 0;
                    } else if (attackIndex == 2 && (!JudgeStamina(GetPileCost()) || sqrDist <= 2f * 2f)) {
                        attackIndex = 0;
                    } else if (attackIndex == 3 && !JudgeStamina(GetSpinCost())) {
                        attackIndex = 0;
                    } else if (attackIndex == 4 && (!JudgeStamina(GetWaveCost()) || (sqrDist < MyMath.Square(1.5f + waveAttackedTimeRemain * 0.5f) && Random.Range(0, 100) < 75))) {
                        attackIndex = 0;
                    }
                    attackIndexSave = attackIndex;
                }
                if (attackIndex == 0 && attackProcess >= 2 && !JudgeStamina(GetCost(CostType.Attack) * (14f / 12f))) {
                    attackProcess = 0;
                }
                if (attackIndex == 1) {
                    Jump(9);
                    attackedTimeRemain = 0.6f;
                    attackProcess = 0;
                    heightAdjustTimer = 0f;
                } else if (attackIndex == 2) {
                    attackBiasValue = 0;
                    if (AttackBase(5, 1.5f, 1.5f, GetPileCost(), 15f / 30f / spRate, 15f / 30f / spRate, 0f, spRate, true, 20f)) {
                        PerfectLockon();
                        S_ParticlePlay(0);
                        attackBiasValue = 0;
                        PileAttackCheckerActivate(true);
                        pileDuration = 0f;
                        attackProcess = 1;
                        isSkillAttacking = true;
                        EmitEffect(effectPile);
                        if (isSuperman) {
                            EmitEffect(effectPileSuper);
                        }
                    }
                } else if (attackIndex == 3) {
                    attackBiasValue = 0;
                    float spinSp = 3f / 4f;
                    float spinInterval = 28f / 30f / spinSp;
                    if (AttackBase(6, 1f, 1f, GetSpinCost(), spinInterval, spinInterval, 0.5f, spinSp)) {
                        SpecialStep(0.4f, 0.25f, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        attackProcess = 0;
                        isSkillAttacking = true;
                        SuperarmorStart();
                        EmitEffect(effectSpin);
                    }
                } else if (attackIndex == 4) {
                    if (AttackBase(7, 1f, 1f, GetWaveCost(), 30f / 30f / spRate, 30f / 30f / spRate, 0, spRate, true, 24f)) {
                        attackProcess = 0;
                        isSkillAttacking = true;
                        waveAttackedTimeRemain = 5f;
                    }
                } else if (attackProcess == 0) {
                    if (AttackBase(0, 1.15f, 1.2f, GetCost(CostType.Attack), 10f / 30f / spRate, 10f / 30f / spRate, 1f, spRate)) {
                        if (!refresh) {
                            SpecialStep(0.4f, 0.25f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        }
                        attackProcess = 1;
                        isSkillAttacking = false;
                    }
                } else if (attackProcess == 1) {
                    if (AttackBase(1, 1.15f, 1.2f, GetCost(CostType.Attack), 10f / 30f / spRate, 10f / 30f / spRate, 1f, spRate)) {
                        if (!refresh) {
                            SpecialStep(0.4f, 0.25f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        }
                        attackProcess = 2;
                        isSkillAttacking = false;
                    }
                } else if (attackProcess == 2) {
                    if (AttackBase(2, 1.15f, 0.875f, GetCost(CostType.Attack) * (14f / 12f), 12f / 30f / spRate, 12f / 30f / spRate, 1f, spRate)) {
                        if (!refresh) {
                            SpecialStep(0.4f, 0.25f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        }
                        attackProcess = 3;
                        isSkillAttacking = false;
                    }
                } else if (attackProcess == 3) {
                    if (AttackBase(3, 1.75f, 2.875f, GetCost(CostType.Attack) * (14f / 12f), 14f / 30f / spRate, 14f / 30f / spRate + (refresh ? 0.25f : 0f), 1f, spRate)) {
                        if (!refresh) {
                            SpecialStep(0.4f, 0.25f / spRate, 6f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        }
                        attackProcess = 0;
                        isSkillAttacking = false;
                    }
                }
            } else {
                if (JudgeStamina(GetBoltCost())) {
                    if (AttackBase(8, 1f, 1f, GetBoltCost(), 5f, 15f / 30f / spRate, 0f, 1, true, 20f)) {
                        S_ParticlePlay(0);
                        attackProcess = 0;
                        attackBiasValue = 0;
                        isSkillAttacking = true;
                        SuperarmorStart();
                        EmitEffect(effectBolt);
                        if (move.y > -0.25f) {
                            move.y = -0.25f;
                        }
                    }
                } else {
                    if (AttackBase(4, 1f, 2f, GetCost(CostType.Attack), 5f, 15f / 30f / spRate, 0f, 1, true, 20f)) {
                        S_ParticlePlay(0);
                        attackProcess = 0;
                        attackBiasValue = 0;
                        isSkillAttacking = false;
                        if (move.y > -0.25f) {
                            move.y = -0.25f;
                        }
                    }
                }
                jumpAttackPosSave = trans.position.y;
            }
        }
        if (attackType == 3) {
            if (attackDetection[rightDet] && attackDetection[leftDet]) {
                attackDetection[rightDet].relationIndex = leftDet;
                attackDetection[leftDet].relationIndex = rightDet;
                bothHandAttacking = true;
                attackDetection[rightDet].offset = attackOffsetBothHand;
                attackDetection[leftDet].offset = attackOffsetNormal;
            }
        } else {
            if (attackDetection[rightDet] && attackDetection[leftDet]) {
                attackDetection[rightDet].relationIndex = -1;
                attackDetection[leftDet].relationIndex = -1;
                attackDetection[rightDet].offset = attackOffsetNormal;
                attackDetection[leftDet].offset = attackOffsetNormal;
            }
            bothHandAttacking = false;
        }
        attackTypeSave = attackType;
        base.Attack();
    }

    protected override void Jump(float power) {
        if (JudgeStamina(GetCost(CostType.Jump))) {
            nowST -= GetCost(CostType.Jump);
            if (JudgeStamina(GetScrewCost()) && (isSuperman || (targetTrans && GetTargetDistance(true, true, true) <= 1.5f * 1.5f) || Random.Range(0, 100) < 40)) {
                nowST -= GetScrewCost();
                attackPowerMultiplier = 1f;
                knockPowerMultiplier = 1f;
                anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], 2);
            } else {
                anim.SetInteger(AnimHash.Instance.ID[(int)AnimHash.ParamName.JumpType], 0);
            }
            base.Jump(power * (GetSick(SickType.Mud) ? 0.25f : 1f));
            mutekiTimeRemain = 11f / 60f;
            landingTime = 9f / 60f;
            EmitEffect(effectJump);
        }
    }

    void PileAttackEnd() {
        isAnimEnd = true;
        isAnimStopped = false;
        PileAttackCheckerActivate(false);
    }

    protected override void Update_Transition_Jump() {
        base.Update_Transition_Jump();
        if (targetTrans && stateTime >= 0.5f && GetCanControl() && (command == Command.Default || command == Command.Free)) {
            float sqrDist = GetTargetDistance(true, true, true);
            float condDist = 0.7f;
            if (JudgeStamina(GetBoltCost())) {
                condDist = isSuperman ? 1.1f : 0.9f;
            }
            if (state != State.Attack && sqrDist <= condDist * condDist && attackedTimeRemain <= 0f && JudgeStamina(GetCost(CostType.Attack))) {
                SetState(State.Attack);
            }
        }
    }

    protected override void Update_Process_Jump() {
        base.Update_Process_Jump();
        if (targetTrans && (command == Command.Default || command == Command.Free)) {
            lockonRotSpeed = attackLockonDefaultSpeed;
            float sqrDist = GetTargetDistance(true, true, false);
            if (JudgeStamina(GetCost(CostType.Attack))) {
                if (sqrDist > 0.1f * 0.1f) {
                    CommonLockon();
                    cCon.Move(GetTargetVector(true, true, false) * GetMaxSpeed(false, false, false, true) * Mathf.Clamp01(0.5f + stateTime * 2f) * deltaTimeMove);
                }
            } else {
                if (sqrDist > 0.1f * 0.1f) {
                    CommonLockon();
                }
                cCon.Move(GetTargetVector(true, true, true) * GetMaxSpeed(false, false, false, true) * Mathf.Clamp01(0.5f + stateTime * 2f) * deltaTimeMove);
            }
        }
    }

    void ThrowSlash() {
        if (throwing && isHyper && isSuperman) {
            EmitEffect(effectSlash);
            throwing.ThrowStart(slashRightThrowIndex);
            throwing.ThrowStart(slashLeftThrowIndex);
        }
    }

    void ThrowWave() {
        if (throwing) {
            if (isSuperman) {
                throwing.ThrowStart(waveSuperThrowIndex);
            } else {
                throwing.ThrowStart(waveThrowIndex);
            }
        }
    }

    void ThrowSpin() {
        if (throwing) {
            if (isSuperman) {
                throwing.ThrowReady(spinSuperThrowIndex);
            } else {
                throwing.ThrowReady(spinThrowIndex);
            }
        }
    }

    void LightningBolt() {
        if (throwing) {
            if (isSuperman) {
                throwing.ThrowStart(boltSuperThrowIndex);
            } else {
                throwing.ThrowStart(boltThrowIndex);
            }
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        switch (attackType) {
            case 4:
            case 8:
                gravityMultiplier = 3.8f;
                attackBiasValue = Mathf.Min(attackBiasValue, move.y);
                if (attackType == 4) {
                    addValue = Mathf.Max(0f, (attackBiasValue + 5f) * (-0.1f));
                    attackPowerMultiplier = 1.25f + addValue * (isSuperman ? 1.5f : 1f);
                    knockPowerMultiplier = 1.25f + addValue * 2f;
                } else {
                    addValue = Mathf.Max(0f, (attackBiasValue + 8f) * (-0.1f));
                    attackPowerMultiplier = 1.25f + addValue * (isSuperman ? 1.5f : 1f);
                    knockPowerMultiplier = 1f + addValue * 2f;
                }
                lockonRotSpeed = Mathf.Clamp(20f - stateTime * 50f, 8f, 20f);
                if (groundedFlag || stateTime > 3f || (stateTime > 0.6f && Time.timeScale > 0f && trans.position.y >= jumpAttackPosSave)) {
                    if (attackType == 8) {
                        LightningBolt();
                    }
                    float plusTime = (attackType == 4 ? 4f / 30f : 16f / 30f);
                    lockonRotSpeed = 8f;
                    attackType = -1;
                    attackProcess = 1;
                    attackStiffTime = stateTime + plusTime;
                    attackedTimeRemain = plusTime + 0.1f;
                    nowSpeed = 0;
                    gravityMultiplier = 1;
                } else {
                    attackStiffTime = stateTime + 1.0f;
                    if (attackedTimeRemain < 1.5f) {
                        attackedTimeRemain = 1.5f;
                    }
                }
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], attackType != 4 || !isAnimStopped ? 1 : 0);
                if (Time.timeScale > 0f) {
                    jumpAttackPosSave = trans.position.y;
                }
                break;
            case 5:
                float hyperRate = (isHyper ? 1.5f : 1f);
                float maxSpTemp = GetMaxSpeed(false, false, false, true) * hyperRate;
                float pileMaxSpeed = maxSpTemp * 1.3f + 8f;
                float speedTemp = pileMaxSpeed * (stateTime < 0.3f ? stateTime / 0.3f : 1);
                if (!isAnimEnd) {
                    if (stateTime > 0.9f) {
                        PileAttackEnd();
                    } else if (_pileAttackCheckerTrigger && _pileAttackCheckerEnabled && _pileAttackCheckerTrigger.stayFlag) {
                        PileAttackEnd();
                    } else if (nowSpeed > 0f && targetTrans && GetTargetDistance(true, true, false) < Mathf.Clamp(nowSpeed * nowSpeed * 0.0004f, 0.01f, 0.36f)) {
                        float targetHeight = GetTargetHeight(true);
                        if (targetHeight > 1.3f || targetHeight <= 0f) {
                            PileAttackEnd();
                        }
                    }
                }
                if (!isAnimEnd) {
                    attackStiffTime = stateTime + 0.6f;
                    cCon.Move(trans.TransformDirection(vecForward) * speedTemp * deltaTimeMove);
                } else {
                    agent.velocity = vecZero;
                }
                attackBiasValue = Mathf.Clamp01(pileDuration * 3f * hyperRate);
                attackPowerMultiplier = Mathf.Max(1.5f, 1.5f + (maxSpTemp - 2.25f) / 4.5f * attackBiasValue);
                knockPowerMultiplier = Mathf.Max(1.5f, 1.5f + (maxSpTemp - 3.375f) * 4f / 9f * attackBiasValue);
                if (!isAnimStopped) {
                    lockonRotSpeed = 20f * Mathf.Max(1f, pileMaxSpeed / 19.7f) * hyperRate * hyperRate;
                } else if (!isAnimEnd) {
                    lockonRotSpeed = (20f - Mathf.Clamp(nowSpeed / pileMaxSpeed * 6f, 0f, 6f)) * Mathf.Max(1f, pileMaxSpeed / 19.7f) * hyperRate * hyperRate;
                } else {
                    lockonRotSpeed = 0f;
                }
                if (isAnimStopped) {
                    pileDuration += deltaTimeMove;
                }
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], isAnimEnd || !isAnimStopped ? 1f : 0f);
                break;
        }
    }

    protected override void Start_Process_Dodge() {
        base.Start_Process_Dodge();
        EmitEffect(effectDodge);
    }

    public override bool GetIdling() {
        return (base.GetIdling() && landingTime <= 0f);
    }
    public override bool GetWalking() {
        return (base.GetWalking() && landingTime <= 0f);
    }

    protected override void Start_Process_Jump() {
        base.Start_Process_Jump();
        bothHandAttacking = false;
    }

    public override void AttackStart(int index) {
        if (bothHandAttacking && index == leftDet) {
            attackEffectEnabled = false;
        } else {
            attackEffectEnabled = true;
        }
        if (attackType == 3 && bothHandAttacking) {
            attackPowerMultiplier = bothHandAttackMul;
            knockPowerMultiplier = bothHandKnockMul;
        }
        if (state != State.Dodge && (state != State.Jump || (index != rightDet && index != leftDet))) {
            base.AttackStart(index);
        } else {
            AttackEnd(index);
        }
    }

    protected override float GetSTHealRateChild_Normal() {
        float rate = 1f;
        if (GetIdling() || state == State.Damage) {
            rate = 2f;
        } else if (GetWalking()) {
            rate = 1.5f;
        }
        return base.GetSTHealRateChild_Normal() * rate;
    }
    protected override float GetSTHealRateChild_Attack() {
        return base.GetSTHealRateChild_Attack() + (isSkillAttacking || isAnimStopped ? 0f : 0.03f);
    }
    protected override float GetSTHealRateChild_Jump() {
        float rate = 0.02f;
        float speedTemp = Mathf.Abs(nowSpeed);
        float walkTemp = GetMaxSpeed(true, false, false, true);
        if (speedTemp >= walkTemp || (attackDetection.Length > screwDet && attackDetection[screwDet] && attackDetection[screwDet].attackEnabled)) {
            return 0f;
        } else if (speedTemp > 0f && walkTemp > 0f) {
            rate *= (walkTemp - speedTemp) / walkTemp;
        }
        return rate;
    }

    public override void ResetGuts() {
        base.ResetGuts();
        gutsRemain = gutsMax = 5f;
    }

}
