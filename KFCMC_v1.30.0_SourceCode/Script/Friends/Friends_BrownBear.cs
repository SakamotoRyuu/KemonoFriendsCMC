using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class Friends_BrownBear : FriendsBase {

    public ChangeMatSet weaponMat;
    public Transform quakePivot;
    public Transform movePivotAir;
    public Transform movePivotGround;
    public XWeaponTrail jumpTrail;
    public GameObject longBuffEffect;

    float skillAttackCount;
    int attackSave = -1;
    bool continuousMove = false;
    bool weaponMatIsSpecial = false;
    bool refresh = false;
    bool jumpTrailEnabled = false;
    const float skillAttackMax = 20f;
    float skilledTimeRemain;
    float attackEndTimer;
    const int penetrateAttackIndex = 2;
    float specialAttackReserved;
    private int faceIndex_Attack;
    private int faceIndex_Attack2;
    bool attackIsLong;
    static readonly Vector3 defaultScale = Vector3.one;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            // moveCost.attack = 12f;
            moveCost.attack = 27f * staminaCostRate;
            skillAttackCount = skillAttackMax;
            chatAttackCount = 6;
            attackLockonDefaultSpeed = 12f;
            skilledTimeRemain = -1f;
            specialMoveDirectionAdjustEnabled = true;
        }
    }

    protected override void Start() {
        base.Start();
        if (animatorForBattle) {
            jumpTrail.Init();
            jumpTrail.Deactivate();
            jumpTrailEnabled = false;
        }
    }

    protected override void SetFaceIndex() {
        base.SetFaceIndex();
        faceIndex_Attack = fCon.GetFaceIndex("Attack");
        faceIndex_Attack2 = fCon.GetFaceIndex("Attack2");
    }

    public override void SetForItem() {
        jumpTrail.gameObject.SetActive(false);
        base.SetForItem();
    }

    public override void ResetGuts() {
        base.ResetGuts();
        skillAttackCount = skillAttackMax;
    }

    void JumpStart() {
        jumpTrail.Activate();
        jumpTrailEnabled = true;
    }

    void JumpEnd() {
        if (jumpTrailEnabled) {
            jumpTrail.StopSmoothly(0.1f);
            jumpTrailEnabled = false;
        }
    }

    void ChangeWeaponMat(int param) {
        bool flag = (param != 0);
        if (weaponMatIsSpecial != flag) {
            SetForChangeMatSet(weaponMat, flag);
            weaponMatIsSpecial = flag;
        }
    }

    void MoveAttack(int index) {
        if (targetTrans) {
            float sqrDist = GetTargetDistance(true, true, true);
            float spRate = isSuperman ? 4f / 3f : 1f;
            fbStepTime = 0.25f / spRate;
            fbStepMaxDist = 4f;
            fbStepEaseType = EasingType.SineInOut;
            switch (index) {
                case 0:
                    if (sqrDist < 0.3f * 0.3f) {
                        BackStep(0.3f);
                    } else {
                        SpecialStep(0.45f, 0.25f / spRate, 5f);
                    }
                    break;
                case 1:
                    fbStepTime = 9f / 24f / spRate;
                    fbStepMaxDist = 4f;
                    fbStepEaseType = EasingType.SineOut;
                    SeparateFromTarget(3f);
                    break;
                case 2:
                    if (sqrDist < 0.6f * 0.6f) {
                        BackStep(0.6f);
                    } else {
                        SpecialStep(0.8f, 0.25f / spRate, 5f);
                    }
                    break;
                case 3:
                    fbStepTime = 9f / 30f / spRate;
                    fbStepEaseType = EasingType.SineOut;
                    if (sqrDist < 0.6f * 0.6f) {
                        BackStep(0.6f);
                    } else {
                        SpecialStep(0.8f, 0.25f / spRate, 5f);
                    }
                    break;
                case 4:
                    fbStepTime = 10f / 30f / spRate;
                    fbStepMaxDist = 4f;
                    fbStepEaseType = EasingType.SineOut;
                    if (sqrDist < 0.5f * 0.5f) {
                        BackStep(0.5f);
                    } else {
                        SpecialStep(0.7f, 0.25f / spRate, 5f);
                    }
                    break;
                case 5:
                    if (sqrDist < 0.3f * 0.3f) {
                        BackStep(0.3f);
                    } else {
                        SpecialStep(0.4f, 0.25f / spRate, 5f);
                    }
                    break;
                case 6:
                    fbStepTime = 8f / 30f / spRate;
                    fbStepMaxDist = 3f;
                    fbStepEaseType = EasingType.SineOut;
                    if (sqrDist < 0.4f * 0.4f) {
                        BackStep(0.4f);
                    } else {
                        SpecialStep(0.6f, 0.25f / spRate, 5f);
                    }
                    break;
                case 8:
                    if (targetTrans && targetTrans.position.y >= trans.position.y + 1f) {
                        agent.enabled = false;
                        isJumpingAttack = true;
                        gravityZeroTimeRemain = 0.25f;
                    }
                    continuousMove = true;
                    break;
                case 9:
                    continuousMove = false;
                    LockonEnd();
                    break;
            }
        }
    }

    void SetPowerForST() {
        if (state == State.Attack) {
            attackPowerMultiplier = knockPowerMultiplier = 7f + (nowST * 0.005f);
            SetNowST(0f);
            skillAttackCount = skillAttackMax;
            EmitEffect(1);
            knockRemain = GetHeavyKnockEndurance();
            knockRemainLight = GetLightKnockEndurance();
            skilledTimeRemain = 3f;
        }
    }

    void Impact() {
        if (!isItem && state == State.Attack) {
            if (jumpTrailEnabled) {
                JumpEnd();
            }
            EmitEffect(2);
            CameraManager.Instance.SetQuake(quakePivot.position, 12, 4, 0, 0, 1.5f, 1.5f, 20);
            if (attackDetection[penetrateAttackIndex].attackEnabled == false) {
                AttackStart(penetrateAttackIndex);
                attackEndTimer = 0.11f;
            }
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (state != State.Attack) {
            ChangeWeaponMat(0);
            isJumpingAttack = false;
            specialAttackReserved = 0f;
        }
        if (jumpTrailEnabled && state != State.Attack && state != State.Jump) {
            JumpEnd();
        }
        if ((state != State.Attack || attackType != 8) && skilledTimeRemain > 0f) {
            skilledTimeRemain -= deltaTimeCache;
        }
        if (attackEndTimer > 0f) {
            attackEndTimer -= deltaTimeCache;
            if (attackEndTimer <= 0f && attackDetection[penetrateAttackIndex].attackEnabled) {
                AttackEnd(penetrateAttackIndex);
            }
        }
        bool longBuffEnabled = CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Long);
        bool giraffeBeamEnabled = longBuffEnabled && GameManager.Instance.save.config[GameManager.Save.configID_ShowGiraffeBeam] >= 2;
        if (longBuffEffect && longBuffEffect.activeSelf != giraffeBeamEnabled) {
            longBuffEffect.SetActive(giraffeBeamEnabled);
        }
    }

    protected override void Update_Transition_Moves() {
        if (skillAttackCount <= 0) {
            if (refresh && nowST >= GetMaxST() * 0.95f) {
                refresh = false;
                actDistNum = 0;
            } else if (!refresh && nowST < GetMaxST() * 0.85f) {
                refresh = true;
                actDistNum = 1;
            }
        } else {
            if (refresh && nowST >= GetMaxST() * staminaBorder) {
                refresh = false;
                actDistNum = 0;
            } else if (!refresh && !JudgeStamina(GetCost(CostType.Attack))) {
                refresh = true;
                actDistNum = 1;
            }
        }
        if (searchArea.Length > 0 && searchArea[0]) {
            if (skillAttackCount <= 0 || (state == State.Attack && attackType == 8)) {
                searchArea[0].priorityEffectRate = 1f;
            } else {
                searchArea[0].priorityEffectRate = 0.4f;
            }
        }
        base.Update_Transition_Moves();
    }

    protected override void Attack() {
        float spRate = isSuperman ? 4f / 3f : 1f;
        int attackTemp;
        float sqrDist = GetTargetDistance(true, false, true);
        bool isEscapeAttack = false;
        isJumpingAttack = false;
        if (skillAttackCount <= 0f && nowST >= GetMaxST() * 0.75f) {
            attackTemp = 8;
        } else {
            if (skillAttackCount <= 0f || nowST < GetCost(CostType.Attack) * 2f) {
                attackTemp = 1;
                isEscapeAttack = true;
            } else {
                attackTemp = Random.Range(0, (sqrDist < 1.5f * 1.5f ? 8 : 7));
                if (attackTemp == attackSave) {
                    attackTemp = (attackTemp + 1) % 8;
                }
                if (sqrDist > 1f * 1f && attackTemp == 1) {
                    attackTemp = (attackSave == 4 ? 5 : 4);
                }
            }
        }
        attackSave = attackTemp;
        if (attackTemp != 8) {
            faceIndex[(int)FaceName.Attack] = faceIndex_Attack;
            base.Attack();
        } else {
            faceIndex[(int)FaceName.Attack] = faceIndex_Attack2;
            SetChat(chatKey_Attack[Random.Range(3, 6)], 35);
        }
        switch (attackTemp) {
            case 0:
                if (AttackBase(0, 1.5f, 5f / 3f, GetCost(CostType.Attack) * (40f / 27f), 40f / 30f / spRate, 40f / 30f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 40f / 30f);
                }
                break;
            case 1:
                if (AttackBase(1, 1f, 1f, GetCost(CostType.Attack) * (27f / 27f), 27f / 24f / spRate, 27f / 24f / spRate + (isEscapeAttack ? 0.5f : 0f), 0f, spRate)) {
                    skillAttackCount -= (1f + 27f / 24f);
                }
                break;
            case 2:
                if (AttackBase(2, 1.2f, 1.25f, GetCost(CostType.Attack) * (30f / 27f), 30f / 30f / spRate, 30f / 30f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 30f / 30f);
                }
                break;
            case 3:
                if (AttackBase(3, 1f, 1f, GetCost(CostType.Attack) * (44f / 27f), 44f / 30f / spRate, 44f / 30f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 44f / 30f);
                }
                break;
            case 4:
                if (AttackBase(4, 1.6f, 1.75f, GetCost(CostType.Attack) * (42f / 27f), 42f / 30f / spRate, 42f / 30f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 42f / 30f);
                }
                break;
            case 5:
                if (AttackBase(5, 1f, 1f, GetCost(CostType.Attack) * (24f / 27f), 24f / 30f / spRate, 24f / 30f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 24f / 30f);
                }
                break;
            case 6:
                if (AttackBase(6, 1f, 1f, GetCost(CostType.Attack) * (26f / 27f), 46f / 30f / spRate, 46f / 30f / spRate, 0.5f, spRate)) {
                    skillAttackCount -= (1f + 46f / 30f);
                }
                break;
            case 7:
                if (AttackBase(7, 1.25f, 4f / 3f, GetCost(CostType.Attack) * (32f / 27f), 32f / 30f / spRate, 32f / 30f / spRate, 0f, spRate)) {
                    skillAttackCount -= (1f + 32f / 30f);
                }
                break;
            case 8:
                if (AttackBase(8, 1f, 1f, 0f, 65f / 30f, 65f / 30f, 0.5f, 1f, true, 15f)) {
                    continuousMove = false;
                    skillAttackCount = skillAttackMax * 0.5f;
                    mutekiTimeRemain = 65f / 30f;
                    ChangeWeaponMat(1);
                    SuperarmorStart();
                    JumpStart();
                }
                break;
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (attackType == 8) {
            if (isSuperarmor) {
                SetMutekiTime(0.4f);
            }
            if (continuousMove) {
                if (isJumpingAttack) {
                    gravityZeroTimeRemain = 0.25f;
                    ApproachTransformPivot(movePivotAir, 11.25f, Mathf.Max(targetRadius, 0.2f), Mathf.Max(targetRadius - 0.4f, 0.05f), false);
                } else {
                    ApproachTransformPivot(movePivotGround, 11.25f, Mathf.Max(targetRadius, 0.2f), Mathf.Max(targetRadius - 0.4f, 0.05f), true);
                }
            }
            if (specialAttackReserved > 0f) {
                specialAttackReserved -= deltaTimeMove;
                if (specialAttackReserved <= 0f) {
                    AttackStart(penetrateAttackIndex);
                    attackEndTimer = 0.11f;
                }
            }
        }
    }

    protected override float GetSTHealRate() {
        return base.GetSTHealRate() * (skilledTimeRemain <= 0f ? 1f : 0.5f);
    }

    public override bool GetCanDodge() {
        if (state == State.Attack && attackType == 8 && isSuperarmor) {
            return false;
        }
        return base.GetCanDodge();
    }

    public bool CheckTrophy_IsSkillAttacking() {
        return (state == State.Attack && attackType == 8);
    }

    void AttackStartSpecial() {
        specialAttackReserved = 0.01f;
    }
    
}
