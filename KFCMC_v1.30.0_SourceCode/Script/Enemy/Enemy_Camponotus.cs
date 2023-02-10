using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Camponotus : EnemyBase {

    public AttackDetectionParticle acidDetection;
    public AttackDetectionSick poisonDetection;
    public GameObject[] weakEffectPrefab;
    public int weakEffectIndex;
    public int healEffectIndex;
    public GameObject[] weakObjs;
    public GameObject[] defaultObjs;
    public float showWeakHPRate = 0.5f;

    private bool weakFlag = false;
    float coreTimeRemain = 0f;
    float backAttackedTimeRemain;
    bool healEffectEmitted = false;
    int attackSave = -1;
    int attackSave2 = -1;
    int rockFlag;
    int noWeakAttackCount;

    static readonly int[] acidProbability = new int[] { 0, 0, 40, 50, 100 };
    static readonly int[] poisonProbability = new int[] { 25, 25, 33, 40, 50 };
    static readonly float[] lockonRotSpeedArray = new float[] { 2f, 2f, 2.4f, 3f, 4f };
    static readonly float[] knockRestoreArray = new float[] { 1f, 1f, 1.1f, 1.21f, 1.331f };
    const int effectRockImpact = 7;
    const int effectRockBreak = 8;
    const int effectNeedleDash = 9;

    protected override void Awake() {
        base.Awake();
        SetWeakFlag(false, true);
        attackWaitingLockonRotSpeed = 2f;
    }

    protected override void SetLevelModifier() {
        if (acidDetection) {
            acidDetection.probability = acidProbability[Mathf.Clamp(level, 0, acidProbability.Length - 1)];
        }
        if (poisonDetection) {
            poisonDetection.probability = poisonProbability[Mathf.Clamp(level, 0, poisonProbability.Length - 1)];
        }
        attackWaitingLockonRotSpeed = lockonRotSpeedArray[Mathf.Clamp(level, 0, lockonRotSpeedArray.Length - 1)];
        if (weakEffectIndex < effect.Length) {
            effect[weakEffectIndex].prefab = weakEffectPrefab[Mathf.Clamp(level, 0, weakEffectPrefab.Length - 1)];
        }
        knockRestoreSpeed = knockRestoreArray[Mathf.Clamp(level, 0, knockRestoreArray.Length - 1)];
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        SetWeakFlag(false);
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (!weakFlag && level <= 1 && nowHP > 0 && nowHP <= (int)(GetMaxHP() * showWeakHPRate)) {
            AppearWeakPoint();
        }
        if (weakFlag && level >= 4) {
            coreTimeRemain -= deltaTimeCache;
            if (!healEffectEmitted && coreTimeRemain <= 1f) {
                EmitEffect(healEffectIndex);
                healEffectEmitted = true;
            }
            if (coreTimeRemain <= 0f) {
                SetWeakFlag(false);
            }
        }
        if (backAttackedTimeRemain > 0f) {
            backAttackedTimeRemain -= deltaTimeMove;
        }
    }

    void SetRockFlag(int flag) {
        rockFlag = flag;
    }

    void EmitEffectImpact() {
        if (state == State.Attack) {
            EmitEffect(effectRockImpact);
            CameraManager.Instance.SetQuake(effect[effectRockImpact].pivot.position, 6, 4, 0, 0, 1.5f, 2f, dissipationDistance_Normal);
        }
    }

    void EmitEffectRock() {
        EmitEffect(effectRockBreak);
        rockFlag = 0;
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        AppearWeakPoint();
        if (rockFlag != 0) {
            EmitEffectRock();
        }
    }

    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        if (!isSuperarmor && rockFlag != 0) {
            EmitEffectRock();
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (!fixKnockAmount && lastDamagedColorType == damageColor_Effective && !TrophyManager.Instance.IsTrophyHad(TrophyManager.t_CamponotusJaguar) && CharacterManager.Instance.GetFriendsExist(4, true) && attackerCB == CharacterManager.Instance.friends[4].fBase) {
            Friends_Jaguar friendsTemp = CharacterManager.Instance.friends[4].fBase.GetComponent<Friends_Jaguar>();
            if (friendsTemp && friendsTemp.CheckTrophy_IsJumpingAttack()) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_CamponotusJaguar, true);
            }
        }
    }

    void MoveKnockEscape() {
        if (level >= 4 && state == State.Damage && targetTrans) {
            fbStepMaxDist = 4f;
            if (knockRestoreSpeed > 0f) {
                fbStepTime = 30f / 60f / knockRestoreSpeed;
            }
            fbStepEaseType = EasingType.SineInOut;
            SeparateFromTarget(7f);
        }
    }

    void SetWeakFlag(bool flag, bool initialize = false) {
        noWeakAttackCount = 0;
        if (weakFlag != flag || initialize) {
            weakFlag = flag;
            for (int i = 0; i < weakObjs.Length; i++) {
                if (weakObjs[i]) {
                    weakObjs[i].SetActive(flag);
                }
            }
            for (int i = 0; i < defaultObjs.Length; i++) {
                if (defaultObjs[i]) {
                    defaultObjs[i].SetActive(!flag);
                }
            }
        }
    }

    protected void AppearWeakPoint() {
        if (!weakFlag) {
            SetWeakFlag(true);
            EmitEffect(weakEffectIndex);
        }
        coreTimeRemain = 10f;
        healEffectEmitted = false;
        if (effect[healEffectIndex].instance) {
            Destroy(effect[healEffectIndex].instance);
        }
    }

    void MoveAttack_Ooago() {
        if (state == State.Attack && targetTrans) {
            float sqrDist = GetTargetDistance(true, true, false);
            if (sqrDist < 3f * 3f) {
                fbStepTime = 25f / 60f;
                fbStepMaxDist = 3f;
                SeparateFromTarget(3f);
            } else {
                SpecialStep(3.2f, 25f / 60f, 4f, 0f, 0f, true, false);
            }
        }
    }

    void MoveAttack_Rock() {
        if (state == State.Attack && targetTrans) {
            fbStepTime = 18f / 60f;
            fbStepMaxDist = 4f;
            ApproachOrSeparate(2.3f);
        }
    }

    void MoveAttack_Needle() {
        if (state == State.Attack) {
            fbStepTime = 20f / 60f;
            fbStepMaxDist = 4f;
            ApproachOrSeparate(3f);
        }
    }

    void MoveAttack_NeedleCombo() {
        if (state == State.Attack) {
            EmitEffect(effectNeedleDash);
            if (targetTrans && GetTargetDistance(true, true, false) >= 1f) {
                SetSpecialMove(GetTargetVector(true, true), 8f, 25f / 60f, EasingType.Linear);
            } else {
                SetSpecialMove(trans.TransformDirection(vecForward), 10f, 25f / 60f, EasingType.Linear);
            }
        }
    }

    protected override void Attack() {
        if (targetTrans) {
            float angle = Vector3.Angle((targetTrans.position - trans.position), trans.TransformDirection(vecForward));
            if (angle > 150f && backAttackedTimeRemain <= 0f) {
                AttackBase(2, 0.6f, 0.6f, 0, 1f, 1f + GetAttackInterval(0.3f), 1, 1, false);
                attackSave = 1;
                backAttackedTimeRemain = 4f;
            } else {
                int attackTemp = Random.Range(0, 2);
                if (attackTemp == attackSave) {
                    if (!weakFlag && noWeakAttackCount >= 2) {
                        attackTemp = 1;
                    } else {
                        attackTemp = Random.Range(0, 2);
                    }
                }
                attackSave = attackTemp;
                if (attackTemp == 0) {
                    noWeakAttackCount++;
                    int attackTemp2 = 0;
                    if (level >= 2) {
                        attackTemp2 = Random.Range(0, level >= 4 ? 4 : level >= 3 ? 3 : 2);
                        if (attackTemp2 == attackSave2) {
                            attackTemp2 = Random.Range(0, level >= 4 ? 4 : level >= 3 ? 3 : 2);
                        }
                        attackSave2 = attackTemp2;
                    }
                    if (attackTemp2 == 0) {
                        AttackBase(0, 1f, 1.1f, 0, 60f / 60f, 60f / 60f + (level >= 4 ? 1f : level >= 3 ? 1.2f : 1.5f) + Random.Range(-0.1f, 0.1f));
                    } else if (attackTemp2 == 1) {
                        if (level >= 4 && Random.Range(0, 100) < 50) {
                            AttackBase(6, 1f, 1.2f, 0, 150f / 60f, 150f / 60f + GetAttackInterval(1.5f, -1));
                        } else {
                            AttackBase(3, 1f, 1.2f, 0, 105f / 60f, 105f / 60f + GetAttackInterval(1.5f, -1));
                        }
                    } else if (attackTemp2 == 2) {
                        AttackBase(4, 1.4f, 2.4f, 0, 90f / 60f, 90f / 60f + GetAttackInterval(1.5f, -2));
                    } else {
                        AttackBase(5, 1.2f, 1.6f, 0, 160f / 60f, 160f / 60f + GetAttackInterval(1f, -3), 0, 1, false);
                    }
                } else {
                    noWeakAttackCount = 0;
                    AttackBase(1, 0.6f, 0.6f, 0, 95f / 60f, GetAttackInterval(2.75f));
                }
            }
        }
    }    
}