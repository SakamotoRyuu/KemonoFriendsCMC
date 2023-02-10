using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Lehmannia : EnemyBase {

    public AttackDetectionParticle sickDetection;
    public AttackDetectionSick[] radulaDetection;
    public GameObject radulaPoisonObj;
    public GameObject cellienCore;
    public DamageDetection criticalDD;

    MissingObjectToDestroy deathChecker;
    float poisonTimeRemain = 0;
    int attackSave = -1;

    const int effectCoreBreak = 3;
    const int effectPoison = 0;
    const float poisonInterval = 8;
    static readonly int[] sickProbability = new int[] { 25, 33, 40, 50, 100 };
    static readonly int[] sickProbabilityRadula = new int[] { 0, 0, 0, 33, 50 };
    static readonly float[] damageRateArray = new float[5] { 3.5f, 3.5f, 3.333333f, 3.166666f, 3.0f };
    static readonly int[] dropRateArray = new int[5] { 120, 120, 120, 120, 200 };

    protected override void Awake() {
        base.Awake();
        belligerent = false;
    }

    protected override void SetLevelModifier() {
        sickDetection.probability = sickProbability[Mathf.Clamp(level, 0, sickProbability.Length - 1)];
        for (int i = 0; i < radulaDetection.Length; i++) {
            radulaDetection[i].probability = sickProbabilityRadula[Mathf.Clamp(level, 0, sickProbabilityRadula.Length - 1)];
        }
        radulaPoisonObj.SetActive(sickProbabilityRadula[Mathf.Clamp(level, 0, sickProbabilityRadula.Length - 1)] > 0);
        dropRate[0] = dropRateArray[Mathf.Clamp(level, 0, dropRateArray.Length - 1)];
        if (cellienCore) {
            cellienCore.SetActive(level <= 3);
        }
        if (criticalDD) {
            criticalDD.damageRate = damageRateArray[Mathf.Clamp(level, 0, damageRateArray.Length - 1)];
        }
        attackSave = -1;
        attackLockonDefaultSpeed = (level >= 2 ? 10f : 5f);
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (poisonTimeRemain > 0) {
            poisonTimeRemain -= deltaTimeMove;
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (nowHP > 0) {
            if (actDistNum == 0) {
                actDistNum = 1;
                belligerent = true;
            }
            if (level >= 4 && cellienCore && !cellienCore.activeSelf && (lightKnockCount >= 10 || heavyKnockCount >= 1 || nowHP <= GetMaxHP() * 0.75f)) {
                cellienCore.SetActive(true);
                EmitEffect(effectCoreBreak);
            }
        }
    }

    protected override void UpdateAC_AnimSpeed() {
        float temp = (GetSick(SickType.Mud) || GetSick(SickType.Slow) ? 0.5f : 1);
        if (state == State.Chase || state == State.Escape) {
            temp *= 2;
        }
        if (animParam.animSpeed != temp) {
            animParam.animSpeed = temp;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AnimSpeed], animParam.animSpeed);
        }
    }

    void MoveAttack() {
        if (state == State.Attack) {
            fbStepMaxDist = 2f;
            ApproachOrSeparate(4.5f);
        }
    }

    protected override void ParticlePlay(int index) {
        if (state == State.Attack) {
            base.ParticlePlay(index);
        }
    }

    protected override void Attack() {
        bool attacked = false;
        if (poisonTimeRemain <= 0) {
            AttackBase(1, 0, 0, 0, 0.5f, 0.5f + GetAttackInterval(0.5f), 1, 1, false, 0);
            poisonTimeRemain = poisonInterval;
            attacked = true;
        } else if (targetTrans) {
            float sqrDist = GetTargetDistance(true, false, false);
            if (level >= 2 && Random.Range(0, 2) == 1 && sqrDist < 8f * 8f) {
                AttackBase(sqrDist < 2f * 2f ? 3 : 2, 0.6f, 0.6f, 0, 60f / 60f, 60f / 60f + GetAttackInterval(1f, -1));
                attacked = true;
            } else if ((level >= 3 && sqrDist < 8f * 8f) || sqrDist < 2.5f * 2.5f) {
                int attackTemp = 0;
                if (level >= 3) {
                    int min = sqrDist < 2f * 2f ? 0 : 1;
                    attackTemp = Random.Range(min, level >= 4 ? 4 : 3);
                    if (attackTemp == attackSave) {
                        attackTemp = Random.Range(min, 3);
                    }
                }
                attackSave = attackTemp;
                if (attackTemp == 0) {
                    AttackBase(0, 1f, 0.8f, 0, 75f / 60f, 75f / 60f + GetAttackInterval(0.5f));
                    if (level >= 3) {
                        SpecialStep(0.4f);
                    }
                } else if (attackTemp == 1) {
                    AttackBase(4, 1.1f, 1f, 0, 75f / 60f, 75f / 60f + GetAttackInterval(0.75f));
                    MoveAttack();
                } else if (attackTemp == 2) {
                    AttackBase(5, 1.1f, 1f, 0, 80f / 60f, 80f / 60f + GetAttackInterval(1f));
                    MoveAttack();
                } else {
                    AttackBase(6, 1.1f, 1f, 0, 140f / 60f, 140f / 60f + GetAttackInterval(1.2f));
                    MoveAttack();
                }
                attacked = true;
            }
        }
        if (!attacked) {
            attackedTimeRemain = 0.5f;
            attackStiffTime = 0f;
        }
    }

    void EmitPoison() {
        Vector3 position = vecZero;
        if (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.trapSettings.heightOffset != 0f) {
            position.y += StageManager.Instance.dungeonController.trapSettings.heightOffset;
        }
        if (effect[effectPoison].pivot) {
            effect[effectPoison].pivot.localPosition = position;
        }
        EmitEffect(effectPoison);
        deathChecker = effect[effectPoison].instance.GetComponent<MissingObjectToDestroy>();
        if (deathChecker != null) {
            deathChecker.SetGameObject(gameObject);
        }
    }

    void LockonSlowdown() {
        if (state == State.Attack && lockonRotSpeed > 4f) {
            lockonRotSpeed = 4;
        }
    }

    void WeakenAttack() {
        if (state == State.Attack) {
            if (attackPowerMultiplier > 1f) {
                attackPowerMultiplier = 1f;
            }
            if (knockPowerMultiplier > 0.8f) {
                knockPowerMultiplier = 0.8f;
            }
        }
    }
    
}
