using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Another : PlayerController
{

    public GameObject cervalBall;
    protected const int effectPileSuper = 8;
    protected int faceIndex_Attack;
    protected int faceIndex_Attack2;

    float speechTimer;
    int speechType;

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (speechTimer > 0f) {
            speechTimer -= deltaTimeCache;
            if (speechTimer <= 0f) {
                switch (speechType) {
                    case 1:
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_AVENGE_0", -1, -1);
                        SetFaceString("Fear", 5f);
                        break;
                    case 2:
                        CharacterManager.Instance.SetSpecialChat("EVENT_ANOTHERSERVAL_AVENGE_1", -1, -1);
                        SetFaceString("Smile2", 5f);
                        break;
                    default:
                        break;
                }
                speechType = 0;
            }
        }
    }

    public override void ResetGuts() {
        base.ResetGuts();
        speechTimer = 0f;
        speechType = 0;
    }

    protected override void SetFaceIndex() {
        base.SetFaceIndex();
        faceIndex_Attack = fCon.GetFaceIndex("Attack");
        faceIndex_Attack2 = fCon.GetFaceIndex("Attack2");
        faceIndex[(int)FaceName.Fear] = fCon.GetFaceIndex("Fear2");
        faceIndex[(int)FaceName.Other] = fCon.GetFaceIndex("Sad");
    }

    void SetAttackFaceForNormal() {
        faceIndex[(int)FaceName.Attack] = faceIndex_Attack;
    }

    void SetAttackFaceForSkill() {
        faceIndex[(int)FaceName.Attack] = faceIndex_Attack2;
    }    

    protected override void AttackBody() {
        float spRate = isSuperman ? 4f / 3f : 1;
        bool lockonEnabled = (CharacterManager.Instance.autoAim != 0 || searchArea[0].isLocking);
        if (throwing.throwSettings[spinNormalThrowIndex].instance) {
            Destroy(throwing.throwSettings[spinNormalThrowIndex].instance);
        }
        if (effect[effectSpin].instance) {
            ParticleStopOnAttackEnd particleStopTemp = effect[effectSpin].instance.GetComponent<ParticleStopOnAttackEnd>();
            if (particleStopTemp) {
                particleStopTemp.StopExternal();
            }
        }
        if (throwing.throwSettings[spinPlasmaThrowIndex].instance) {
            Destroy(throwing.throwSettings[spinPlasmaThrowIndex].instance);
        }
        if (effect[effectSpinPlasma].instance) {
            ParticleStopOnAttackEnd particleStopTemp = effect[effectSpinPlasma].instance.GetComponent<ParticleStopOnAttackEnd>();
            if (particleStopTemp) {
                particleStopTemp.StopExternal();
            }
            AudioReplayOnAttacking audioReplayTemp = effect[effectSpinPlasma].instance.GetComponent<AudioReplayOnAttacking>();
            if (audioReplayTemp) {
                audioReplayTemp.maxCount = 0;
            }
        }
        if (!groundedFlag) {
            if ((CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Bolt) != 0 || (isSuperman && CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Judgement) != 0)) && (playerInput.GetButton(RewiredConsts.Action.Special) || forceBolt) && JudgeStamina(GetBoltCost())) {
                SetAttackFaceForSkill();
                if (AttackBase(8, 1, 1, GetBoltCost(), 5f, 15f / 30f / spRate, 0.2f, 1, lockonEnabled, 20f)) {
                    S_ParticlePlay(0);
                    attackProcess = 1;
                    attackBiasValue = 0;
                    isSkillAttacking = true;
                    obstacleDisableTimeRemain = 1f;
                    SuperarmorStart();
                    EmitEffect(effectBolt);
                    if (move.y > -0.25f) {
                        move.y = -0.25f;
                    }
                }
            } else {
                SetAttackFaceForNormal();
                if (AttackBase(4, 1, 2, GetCost(CostType.Attack), 5f, 15f / 30f / spRate, 0.2f, 1, lockonEnabled, 20f)) {
                    S_ParticlePlay(0);
                    attackProcess = 1;
                    attackBiasValue = 0;
                    isSkillAttacking = false;
                    obstacleDisableTimeRemain = 1f;
                    if (move.y > -0.25f) {
                        move.y = -0.25f;
                    }
                }
            }
        } else {
            if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Pile) != 0 && ((playerInput.GetButton(RewiredConsts.Action.Special) && axisInput.y > 0.5f) || forcePile) && JudgeStamina(GetPileCost())) {
                SetAttackFaceForSkill();
                if (AttackBase(5, 1.5f, 1.5f, GetPileCost(), 15f / 30f / spRate, 15f / 30f / spRate, 0, spRate, lockonEnabled, 20f)) {
                    PerfectLockon();
                    S_ParticlePlay(0);
                    attackBiasValue = 0;
                    attackProcess = 1;
                    PileAttackCheckerActivate(true);
                    pileDuration = 0f;
                    isSkillAttacking = true;
                    EmitEffect(effectPile);
                    if (isSuperman) {
                        EmitEffect(effectPileSuper);
                    }
                }
            } else if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Wave) != 0 && (playerInput.GetButton(RewiredConsts.Action.Dodge) || (playerInput.GetButton(RewiredConsts.Action.Special) && axisInput.y < -0.5f) || forceWave) && JudgeStamina(GetWaveCost())) {
                spRate = isSuperman ? 4f / 3f : 10f / 9f;
                SetAttackFaceForNormal();
                if (AttackBase(7, 1f, 1f, GetWaveCost(), 24f / 30f / spRate, 24f / 30f / spRate, 0, spRate, lockonEnabled, 24f)) {
                    S_ParticlePlay(0);
                    attackProcess = 1;
                    isSkillAttacking = true;
                    attackingMoveReservedTimer = 20f / 30f / spRate;
                }
            } else if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Spin) != 0 && (playerInput.GetButton(RewiredConsts.Action.Special) || forceSpin) && JudgeStamina(GetSpinCost())) {
                attackBiasValue = 0;
                float spinSp = 3f / 4f;
                SetAttackFaceForSkill();
                bool superSpinEnabled = (isSuperman && nowST >= GetSpinCost());
                float spinInterval = (superSpinEnabled ? 50f : 20f) / 30f / spinSp;
                if (AttackBase(superSpinEnabled ? 9 : 6, 1f, 1f, GetSpinCost(), spinInterval, spinInterval, 0.5f, spinSp, false)) {
                    S_ParticlePlay(0);
                    S_ParticlePlay(1);
                    SpecialStep(0.4f, 0.25f, 4f, 0f, 0f, true, true, EasingType.SineInOut, true);
                    attackProcess = 0;
                    isSkillAttacking = true;
                    SuperarmorStart();
                    if (attackType != 9) {
                        attackingMoveReservedTimer = 16f / 30f / spinSp;
                        EmitEffect(effectSpin);
                    } else {
                        attackingMoveReservedTimer = 0.25f;
                        EmitEffect(effectSpinPlasma);
                    }
                }
            } else if (attackProcess == 0) {
                SetAttackFaceForNormal();
                if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 10f / 30f / spRate, 10f / 30f / spRate, 1f, spRate, lockonEnabled)) {
                    S_ParticlePlay(0);
                    attackProcess = 1;
                    isSkillAttacking = false;
                    attackingMoveReservedTimer = 7f / 30f / spRate;
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge)) {
                        SpecialStep(0.4f, 0.25f / spRate, 1f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                    }
                }
            } else if (attackProcess == 1) {
                SetAttackFaceForNormal();
                if (AttackBase(1, 1f, 1f, GetCost(CostType.Attack), 10f / 30f / spRate, 10f / 30f / spRate, 1f, spRate, lockonEnabled)) {
                    S_ParticlePlay(1);
                    attackProcess = (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Combo) != 0 ? 2 : 0);
                    isSkillAttacking = false;
                    attackingMoveReservedTimer = 7f / 30f / spRate;
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge)) {
                        SpecialStep(0.4f, 0.25f / spRate, 1f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                    }
                }
            } else if (attackProcess == 2) {
                SetAttackFaceForNormal();
                if (AttackBase(2, 1f, 0.75f, GetCost(CostType.Attack) * (7f / 6f), 12f / 30f / spRate, 12f / 30f / spRate, 1f, spRate, lockonEnabled)) {
                    S_ParticlePlay(0);
                    S_ParticlePlay(1);
                    attackProcess = 3;
                    isSkillAttacking = false;
                    attackingMoveReservedTimer = 7f / 30f / spRate;
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge)) {
                        SpecialStep(0.4f, 0.25f / spRate, 1f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                    }
                }
            } else if (attackProcess == 3) {
                SetAttackFaceForNormal();
                if (AttackBase(3, 1f, 0.75f, GetCost(CostType.Attack) * (7f / 6f), 14f / 30f / spRate, 14f / 30f / spRate, 1f, spRate, lockonEnabled)) {
                    S_ParticlePlay(0);
                    S_ParticlePlay(1);
                    attackProcess = 0;
                    isSkillAttacking = false;
                    attackingMoveReservedTimer = 7f / 30f / spRate;
                    if (!playerInput.GetButton(RewiredConsts.Action.Dodge)) {
                        SpecialStep(0.4f, 0.25f / spRate, 1f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                    }
                }
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
    }
    
    protected override void ThrowSlash() {
        if (isSuperman) {
            EmitEffect(effectSlash);
            throwing.ThrowStart(slashRightThrowIndex);
            throwing.ThrowStart(slashLeftThrowIndex);
        }
    }

    protected override void ThrowSpin() {
        if (attackType == 9) {
            throwing.ThrowReady(spinPlasmaThrowIndex);
        } else {
            throwing.ThrowReady(spinNormalThrowIndex);
        }
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
