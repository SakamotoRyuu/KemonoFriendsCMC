using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_AnotherServal : FriendsBase {

    public GameObject pileAttackChecker;
    public Transform boltOrigin;
    public Transform boltThrowFrom;

    private bool isSkillAttacking;
    private bool isAnimEnd;
    private bool refresh;
    private float landingTime;
    private int attackIndexSave = -1;
    private int attackTypeSave = -1;
    private float addValue;
    private Collider[] _pileAttackCheckerCollider;
    private CheckTriggerStay _pileAttackCheckerTrigger;
    private bool _pileAttackCheckerEnabled = true;
    private float pileDuration;
    private float jumpAttackPosSave;
    private float spinMovingTime;
    private bool spinMoving;
    private int faceIndex_Attack;
    private int faceIndex_Attack2;
    private bool healOnDead = true;
    private bool healAll;
    private string[] chatKey_Dodge = new string[3];
    private float impactAttackDuration;
    private int speechType;
    private float speechTimer;
    private float heightDistConditionTime;
    private float heightAdjustTimer;
    private float waveAttackedTimeRemain;
    private bool[] attackConditions = new bool[5];

    private const int effectPile = 0;
    private const int effectSpin = 1;
    private const int effectJump = 2;
    private const int effectBolt = 3;
    private const int effectSlash = 4;
    private const int effectPileSuper = 5;
    private const int effectSpinSuper = 6;
    private const int effectSacrifice = 7;
    private const int effectDodge = 8;

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

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            deadTimer = 6;
            chatAttackCount = 6;
            moveCost.attack = 6f;
            moveCost.step = 0;
            moveCost.quick = 10;
            moveCost.jump = 10;
            moveCost.skill = 16;
            dodgeDistance = 4.5f;
            dodgeMutekiTime = 1f;
            lightMutekiTime = lightStiffTime + 0.3f;
            heavyMutekiTime = heavyStiffTime + 0.6f;
            superKnockRate = 1.75f;
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

    protected override void ChatKeyInit() {
        base.ChatKeyInit();
        chatKey_Dodge[0] = "TALK_ANOTHERSERVAL_DODGE_00";
        chatKey_Dodge[1] = "TALK_ANOTHERSERVAL_DODGE_01";
        chatKey_Dodge[2] = "TALK_ANOTHERSERVAL_DODGE_02";
    }

    public override void CounterDodge(int dodgeDir, bool changeState = true) {
        base.CounterDodge(dodgeDir, changeState);
        float maxTemp = GetMaxST();
        nowST += maxTemp * 0.1f;
        if (nowST > maxTemp) {
            nowST = maxTemp;
        }
    }

    protected override void SetFaceIndex() {
        base.SetFaceIndex();
        faceIndex_Attack = fCon.GetFaceIndex("Attack");
        faceIndex_Attack2 = fCon.GetFaceIndex("Attack2");
        faceIndex[(int)FaceName.Fear] = fCon.GetFaceIndex("Fear2");
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
        if (speechTimer > 0f) {
            speechTimer -= deltaTimeCache;
            if (speechTimer <= 0f) {
                switch (speechType) {
                    case 1:
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_QUEEN_0", friendsId, -1);
                        SetFaceString("Fear", 5f);
                        break;
                    case 2:
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_QUEEN_1", friendsId, -1);
                        SetFaceString("Determine", 5f);
                        break;
                    default:
                        break;
                }
                speechType = 0;
            }
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

    void SetAttackFaceForNormal() {
        faceIndex[(int)FaceName.Attack] = faceIndex_Attack;
        mesAtkMin = 0;
        mesAtkMax = 2;
    }

    void SetAttackFaceForSkill() {
        faceIndex[(int)FaceName.Attack] = faceIndex_Attack2;
        mesAtkMin = 3;
        mesAtkMax = 5;
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
                if (attackProcess == 0 || (attackProcess == 1 && attackTypeSave != 0)) {
                    if (sqrDist >= 8f * 8f || (sqrDist >= 5 * 5 && Random.Range(0, 100) < 75)) {
                        int randTemp = Random.Range(0, 2);
                        if (randTemp == 0) {
                            attackIndex = 2;
                        } else {
                            attackIndex = 4;
                        }
                    } else if (heightDistConditionTime >= 0.4f && Random.Range(0, 100) < 75 && canJump) {
                        attackIndex = 1;
                    } else if (searchArea[0].IsBesieged(isSuperman ? 4f : 3f)) {
                        attackIndex = (Random.Range(0, 100) < 50 ? 1 : 3);
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

                    for (int i = 0; i < attackConditions.Length; i++) {
                        attackConditions[i] = true;
                    }
                    if (!JudgeStamina(GetCost(CostType.Jump) + GetCost(CostType.Run) + GetCost(CostType.Attack))) {
                        attackConditions[1] = false;
                    }
                    if ((!JudgeStamina(GetPileCost()) || sqrDist <= 2f * 2f)) {
                        attackConditions[2] = false;
                    }
                    if (!JudgeStamina(GetSpinCost())) {
                        attackConditions[3] = false;
                    }
                    if (!JudgeStamina(GetWaveCost()) || (sqrDist < MyMath.Square(1.5f + waveAttackedTimeRemain * 0.5f) && Random.Range(0, 100) < 75)) {
                        attackConditions[4] = false;
                    }
                    for (int i = 0; i < 5 && attackConditions[attackIndex] == false; i++) {
                        attackIndex = Random.Range(0, 5);
                    }
                    if (attackConditions[attackIndex] == false) {
                        attackIndex = 0;
                    }

                    attackIndexSave = attackIndex;
                    if (attackIndex == 0 || attackIndex == 4) {
                        SetAttackFaceForNormal();
                    } else {
                        SetAttackFaceForSkill();
                    }
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
                    float spinInterval = (isSuperman ? 58f : 28f) / 30f / spinSp;
                    bool superSpinEnabled = (isSuperman && JudgeStamina(GetSpinCost()));
                    if (AttackBase(superSpinEnabled ? 9 : 6, 1f, 1f, GetSpinCost(), spinInterval, spinInterval, 0.5f, spinSp)) {
                        SpecialStep(0.4f, 0.25f, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        attackProcess = 0;
                        isSkillAttacking = true;
                        spinMoving = false;
                        SuperarmorStart();
                        if (attackType != 9) {
                            EmitEffect(effectSpin);
                        } else {
                            EmitEffect(effectSpinSuper);
                        }
                    }
                } else if (attackIndex == 4) {
                    if (AttackBase(7, 1f, 1f, GetWaveCost(), 30f / 30f / spRate, 30f / 30f / spRate, 0, spRate, true, 24f)) {
                        attackProcess = 0;
                        isSkillAttacking = true;
                        waveAttackedTimeRemain = 5f;
                    }
                } else if (attackProcess == 0) {
                    if (AttackBase(0, 1.2f, 1.2f, GetCost(CostType.Attack), 12f / 30f / spRate, 12f / 30f / spRate, 1f, spRate)) {
                        if (!refresh) {
                            SpecialStep(0.4f, 0.25f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        }
                        attackProcess = 1;
                        isSkillAttacking = false;
                    }
                } else if (attackProcess == 1) {
                    if (AttackBase(1, 1.2f, 1.2f, GetCost(CostType.Attack), 12f / 30f / spRate, 12f / 30f / spRate, 1f, spRate)) {
                        if (!refresh) {
                            SpecialStep(0.4f, 0.25f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        }
                        attackProcess = 2;
                        isSkillAttacking = false;
                    }
                } else if (attackProcess == 2) {
                    if (AttackBase(2, 1.2f, 0.9f, GetCost(CostType.Attack) * (6f / 5f), 14f / 30f / spRate,  14f / 30f / spRate, 1f, spRate)) {
                        if (!refresh) {
                            SpecialStep(0.4f, 0.25f / spRate, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        }
                        attackProcess = 3;
                        isSkillAttacking = false;
                    }
                } else if (attackProcess == 3) {
                    if (AttackBase(3, 1.92f, 3f, GetCost(CostType.Attack) * (6f / 5f), 16f / 30f / spRate, 16f / 30f / spRate + (refresh ? 0.25f : 0f), 1f, spRate)) {
                        if (!refresh) {
                            SpecialStep(0.4f, 0.25f / spRate, 6f, 0f, 0f, true, true, EasingType.SineInOut, true);
                        }
                        attackProcess = 0;
                        isSkillAttacking = false;
                    }
                }
            } else {
                if (JudgeStamina(GetBoltCost())) {
                    SetAttackFaceForSkill();
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
                    SetAttackFaceForNormal();
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

    public void SacrificeOnPlayerResurrection() {
        healOnDead = false;
        isSacrificeOnDead = true;
        if (CharacterManager.Instance.playerIndex == CharacterManager.playerIndexHyper) {
            chatKey_Dead = "TALK_ANOTHERSERVAL_CELLVAL";
        } else {
            chatKey_Dead = "TALK_ANOTHERSERVAL_SPECIAL";
        }
        nowHP = 0;
        SetState(State.Dead);
    }

    protected override void Start_Process_Dead() {
        if (healOnDead) {
            if (CharacterManager.Instance.GetAnyFriendsLiving()) {
                healAll = true;
            } else {
                healAll = false;
                if (CharacterManager.Instance.playerIndex == CharacterManager.playerIndexHyper) {
                    chatKey_Dead = "TALK_ANOTHERSERVAL_CELLVAL";
                } else {
                    chatKey_Dead = "TALK_ANOTHERSERVAL_SPECIAL";
                }
            }
        }
        base.Start_Process_Dead();
    }

    void HealEvent() {
        if (enabled) {
            EmitEffect(effectSacrifice);
            if (healOnDead) {
                if (healAll) {
                    CharacterManager.Instance.Heal(500, 50, (int)EffectDatabase.id.sacrifice, true, true, true, true, true);
                } else {
                    CharacterManager.Instance.Heal(9999, 100, (int)EffectDatabase.id.sacrifice, false, true, true);
                }
            }
        }
    }

    void SpinMoveStart() {
        if (enabled) {
            spinMovingTime = 0f;
            spinMoving = true;
        }
    }

    void SpinMoveEnd() {
        if (enabled) {
            spinMovingTime = 0f;
            spinMoving = false;
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
        if (throwing && isSuperman) {
            EmitEffect(effectSlash);
            throwing.ThrowStart(slashRightThrowIndex);
            throwing.ThrowStart(slashLeftThrowIndex);
        }
    }

    void ThrowWave() {
        if (throwing) {
            if (!isSuperman) {
                throwing.ThrowStart(waveThrowIndex);
            } else {
                throwing.ThrowStart(waveSuperThrowIndex);
            }
        }
    }

    void ThrowSpin() {
        if (throwing) {
            if (attackType != 9) {
                throwing.ThrowReady(spinThrowIndex);
            } else {
                throwing.ThrowReady(spinSuperThrowIndex);
            }
        }
    }

    void LightningBolt() {
        if (throwing) {
            SetTransformPositionToGround(boltOrigin, boltThrowFrom, 0.5f);
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
                float maxSpTemp = GetMaxSpeed(false, false, false, true);
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
                attackBiasValue = Mathf.Clamp01(pileDuration * 3f);
                attackPowerMultiplier = Mathf.Max(1.5f, 1.5f + (maxSpTemp - 2.25f) / 4.5f * attackBiasValue);
                knockPowerMultiplier = Mathf.Max(1.5f, 1.5f + (maxSpTemp - 3.375f) * 4f / 9f * attackBiasValue);
                if (!isAnimStopped) {
                    lockonRotSpeed = 20f * Mathf.Max(1f, pileMaxSpeed / 19.7f);
                } else if (!isAnimEnd) {
                    lockonRotSpeed = (20f - Mathf.Clamp(nowSpeed / pileMaxSpeed * 6f, 0f, 6f)) * Mathf.Max(1f, pileMaxSpeed / 19.7f);
                } else {
                    lockonRotSpeed = 0f;
                }
                if (isAnimStopped) {
                    pileDuration += deltaTimeMove;
                }
                anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AttackSpeed], isAnimEnd || !isAnimStopped ? 1f : 0f);
                break;
            case 6:
            case 9:
                if (spinMoving) {
                    spinMovingTime += deltaTimeCache;
                    if (targetTrans && GetTargetDistance(true, true, true) >= 0.4f) {
                        cCon.Move(GetTargetVector(true, true) * GetMaxSpeed(false, false, false, true) * 0.9f * Mathf.Clamp01(spinMovingTime * 3f) * deltaTimeMove);
                    }
                }
                break;
        }
    }

    protected override void Start_Process_Dodge() {
        base.Start_Process_Dodge();
        EmitEffect(effectDodge);
        SetChat(chatKey_Dodge[Random.Range(0, chatKey_Dodge.Length)], 15);
    }

    public override bool GetIdling() {
        return (base.GetIdling() && landingTime <= 0f);
    }
    public override bool GetWalking() {
        return (base.GetWalking() && landingTime <= 0f);
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

    public void QueenBattleStartSpeech() {
        speechTimer = 0.5f;
        speechType = 1;
    }

    public void QueenBattleEndSpeech() {
        speechTimer = 2f;
        speechType = 2;
    }

}
