using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Aedes : EnemyBase
{

    public AttackDetectionParticle sickDetection;
    public GameObject[] defaultObj;
    public GameObject[] weakObj;
    public Transform poisonParticlePivot;

    bool weakFlag = true;
    int weakEffectIndex = 0;
    float needleAttackedTimeRemain = 0f;
    float katoriAttackedTimeRemain = 0f;
    int attackSave = -1;
    bool absorbEnabled;
    bool poisonParticlePrepared;
    const int PoisonParticleIndex = 1;

    // UpperAim
    public Transform upperAimAnglePivot;
    public Transform upperAimMultiplier;
    public UpperAimBoneClass[] upperAimBones;
    private const int upperAimLevel = 5;
    private bool upperAimEnabled;
    private float upperAimAngleSave;
    private bool upperAimAngleUpdateEnabled;

    static readonly int[] sickProbability = new int[] { 25, 33, 40, 50, 100 };
    static readonly float[] absorbRate = new float[] { 5f, 5f, 10f, 20f, 40f, 80f };

    protected override void SetLevelModifier()
    {
        if (sickDetection)
        {
            sickDetection.probability = sickProbability[Mathf.Clamp(level, 0, sickProbability.Length - 1)];
        }
        if (level >= 3)
        {
            weakFlag = true;
            HideWeakPoint();
        }
        else
        {
            weakFlag = false;
            AppearWeakPoint(false);
        }
        weakEffectIndex = (level <= 3 ? 0 : 1);
    }

    bool GetNeedleAttackCondition()
    {
        if (level >= 4 && needleAttackedTimeRemain <= 0f && targetTrans)
        {
            if (GetTargetDistance(true, true, false) <= 1.2f * 1.2f && targetTrans.position.y <= trans.position.y + 1.2f)
            {
                return true;
            }
        }
        return false;
    }

    bool GetKatoriAttackCondition()
    {
        if (level >= 3 && katoriAttackedTimeRemain <= 0f && targetTrans)
        {
            if (GetTargetDistance(true, true, false) <= 1.2f * 1.2f && targetTrans.position.y >= trans.position.y + 3f)
            {
                return true;
            }
        }
        return false;
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (GetNeedleAttackCondition() || GetKatoriAttackCondition())
        {
            attackedTimeRemain -= deltaTimeCache * 100f;
        }
        if (needleAttackedTimeRemain > 0f)
        {
            needleAttackedTimeRemain -= deltaTimeMove;
        }
        if (katoriAttackedTimeRemain > 0f)
        {
            katoriAttackedTimeRemain -= deltaTimeMove;
        }
        if (absorbEnabled && state != State.Attack)
        {
            absorbEnabled = false;
        }
    }

    protected override void KnockHeavyProcess()
    {
        base.KnockHeavyProcess();
        if (level >= 3)
        {
            AppearWeakPoint(true);
        }
    }

    void SetWeak(bool flag)
    {
        for (int i = 0; i < weakObj.Length; i++)
        {
            if (weakObj[i])
            {
                weakObj[i].SetActive(flag);
            }
        }
        for (int i = 0; i < defaultObj.Length; i++)
        {
            if (defaultObj[i])
            {
                defaultObj[i].SetActive(!flag);
            }
        }
    }

    protected void AppearWeakPoint(bool effectEnable = false)
    {
        if (!weakFlag)
        {
            weakFlag = true;
            SetWeak(true);
            if (effectEnable)
            {
                EmitEffect(weakEffectIndex);
            }
        }
    }

    void HideWeakPoint()
    {
        if (weakFlag)
        {
            weakFlag = false;
            SetWeak(false);
        }
    }

    protected override void Attack()
    {
        base.Attack();
        absorbEnabled = false;
        upperAimEnabled = false;
        upperAimAngleUpdateEnabled = false;
        poisonParticlePrepared = false;
        int attackTemp = Random.Range(0, 2);
        if (attackTemp == attackSave)
        {
            attackTemp = Random.Range(0, 2);
        }
        if (GetNeedleAttackCondition())
        {
            attackTemp = 2;
        }
        else if (GetKatoriAttackCondition())
        {
            attackTemp = 3;
        }
        if (attackTemp == 0)
        {
            if (level >= 2 && Random.Range(0, 2) == 1)
            {
                AttackBase(2, 1f, 1.1f, 0, 105f / 60f, 105f / 60f + GetAttackInterval(1.5f), 0);
                absorbEnabled = true;
            }
            else
            {
                AttackBase(0, 1f, 1.1f, 0, 80f / 60f, 80f / 60f + GetAttackInterval(1.5f), 0);
                absorbEnabled = true;
            }
            if (level >= upperAimLevel)
            {
                upperAimEnabled = true;
                upperAimAngleUpdateEnabled = true;
            }
        }
        else if (attackTemp == 1)
        {
            float stiffTemp = (IsSuperLevel || level >= 3 ? 60f : level == 2 ? 70f : 80f) / 60f;
            AttackBase(1, 0.95f, 0.6f, 0, stiffTemp, stiffTemp + GetAttackInterval(1.5f));
        }
        else if (attackTemp == 2)
        {
            AttackBase(3, 1.2f, 1.7f, 0, 75f / 60f, 75f / 60f, 0, 1, true, 20);
            SpecialStep(0.2f, 15f / 60f, 1.5f, 0f, 0f, true, false);
            needleAttackedTimeRemain = 3f;
        }
        else if (attackTemp == 3)
        {
            AttackBase(4, 0.2f, 0.6f, 0, 60f / 60f, 60f / 60f + GetAttackInterval(1f, -2), 0f);
            katoriAttackedTimeRemain = 4f;
        }
        attackSave = attackTemp;
    }

    void ThrowAllReady()
    {
        for (int i = 0; i < 5; i++)
        {
            throwing.ThrowReady(i);
        }
    }

    void ThrowAllStart()
    {
        for (int i = 0; i < 5; i++)
        {
            throwing.ThrowStart(i);
        }
    }

    void DisableAbsorb()
    {
        absorbEnabled = false;
    }

    public override void CallBackGiveDamage(CharacterBase damageTaker, int damageNum)
    {
        base.CallBackGiveDamage(damageTaker, damageNum);
        if (state == State.Attack && absorbEnabled && damageNum > 0)
        {
            AddNowHP((int)(damageNum * absorbRate[Mathf.Clamp(level, 0, absorbRate.Length - 1)]), GetCenterPosition(), true, damageColor_Heal);
        }
    }

    private void EndUpperAimAngleUpdate()
    {
        upperAimAngleUpdateEnabled = false;
    }

    private void LateUpdate()
    {
        if (upperAimEnabled && state == State.Attack)
        {
            if (upperAimAngleUpdateEnabled && targetTrans)
            {
                upperAimAngleSave = Mathf.Clamp(GetTargetUpperAngle(upperAimAnglePivot, true), -90, 90) * -1;
            }
            float mul = upperAimMultiplier.localPosition.y;
            if (mul != 0)
            {
                for (int i = 0; i < upperAimBones.Length; i++)
                {
                    Vector3 euler = upperAimBones[i].boneTransform.localEulerAngles;
                    euler.x = upperAimAngleSave * mul * upperAimBones[i].angleMultiplier + upperAimBones[i].angleOffset;
                    upperAimBones[i].boneTransform.localEulerAngles = euler;
                }
            }
        }
        if (state == State.Attack && poisonParticlePrepared)
        {
            attackDetection[PoisonParticleIndex].transform.SetPositionAndRotation(poisonParticlePivot.position, poisonParticlePivot.rotation);
            AttackStart(PoisonParticleIndex);
            poisonParticlePrepared = false;
        }
    }

    private void ReservePoisonParticle()
    {
        poisonParticlePrepared = true;
    }

}
