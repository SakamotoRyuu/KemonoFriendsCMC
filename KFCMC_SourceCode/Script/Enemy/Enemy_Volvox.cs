using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Volvox : EnemyBase
{

    public GameObject[] throwPrefab;

    int attackSave = -1;
    Vector3 vecRand = Vector3.one;
    bool adjustThrowEnabled;

    const int dropItemIDHomo = 52;
    const int ThrowPrefabSetMax = 4;
    const int ThrowVelocitySetMax = 6;
    const int ThrowIndexJibaku = 6;
    const int ThrowIndexJibakuBig = 7;

    protected override void Awake()
    {
        base.Awake();
        attackedTimeRemainOnRestoreKnockDown = 0.5f;
    }

    protected override void SetLevelModifier()
    {
        actDistNum = Mathf.Clamp(level, 0, agentActionDistance.Length - 1);
        for (int i = 0; i < ThrowPrefabSetMax; i++)
        {
            throwing.throwSettings[i].prefab = throwPrefab[Mathf.Clamp(level, 0, throwPrefab.Length - 1)];
        }
        if (isHomoChild)
        {
            dropItem[0] = dropItemIDHomo;
        }
    }

    void SetThrowing(int index)
    {
        float speed = 10f;
        if (targetTrans)
        {
            float distance = GetTargetDistance(false, false, false);
            if (distance > 8f)
            {
                speed += (distance - 8f) * 0.8f;
            }
        }
        for (int i = 0; i < ThrowVelocitySetMax; i++)
        {
            if (i == index)
            {
                throwing.throwSettings[i].randomDirection = vecZero;
            }
            else
            {
                throwing.throwSettings[i].randomDirection = vecRand;
            }
            throwing.throwSettings[i].velocity = speed;
        }
    }

    void ThrowStartAdjust(int index)
    {
        if (adjustThrowEnabled)
        {
            SetThrowing(index);
            adjustThrowEnabled = false;
        }
        throwing.ThrowStart(index);
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (nowHP <= GetMaxHP() / 4)
        {
            attackedTimeRemain = -100;
        }
    }

    protected override void Attack()
    {
        base.Attack();
        adjustThrowEnabled = true;
        bool bigBombEnabled = level >= 5 && (nowHP < GetMaxHP() || isSuperman);
        if (nowHP > GetMaxHP() / 4)
        {
            int attackTemp = 0;
            if (level >= 2 && (nowHP < GetMaxHP() || isSuperman))
            {
                attackTemp = Random.Range(0, Mathf.Clamp(level, 1, 4));
                if (attackTemp == attackSave)
                {
                    attackTemp = Random.Range(0, Mathf.Clamp(level, 1, 4));
                }
            }
            attackSave = attackTemp;
            SetThrowing(-1);
            switch (attackTemp)
            {
                case 0:
                    if (targetTrans)
                    {
                        if (Vector3.Cross(trans.TransformDirection(vecForward), targetTrans.position - trans.position).y > 0)
                        {
                            //Right
                            AttackBase(bigBombEnabled ? 7 : 0, 1, 1.1f, 0, 60f / 60f, 60f / 60f + GetAttackInterval(2f));
                        }
                        else
                        {
                            //Left
                            AttackBase(bigBombEnabled ? 8 : 1, 1, 1.1f, 0, 60f / 60f, 60f / 60f + GetAttackInterval(2f));
                        }
                    }
                    break;
                case 1:
                    AttackBase(3, 1, 1.1f, 0, 60f / 60f, 60f / 60f + GetAttackInterval(2f, -1));
                    break;
                case 2:
                    AttackBase(4, 1, 1.1f, 0, 60f / 60f, 60f / 60f + GetAttackInterval(2f, -2));
                    break;
                case 3:
                    AttackBase(5, 1, 1.1f, 0, 60f / 60f, 60f / 60f + GetAttackInterval(2f, -3));
                    break;
            }
        }
        else
        {
            if (level >= 2)
            {
                fbStepTime = 0.5f;
                fbStepMaxDist = 2f;
                StepToTarget(0);
            }
            float knockTemp = (level >= 4 ? 5600 : level == 3 ? 4200 : level == 2 ? 2800 : 1400);
            knockRemain = knockTemp;
            knockEndurance = knockTemp;
            knockRemainLight = knockTemp;
            knockEnduranceLight = knockTemp;
            if (level >= 4)
            {
                AttackBase(6, 1.3f, 4f, 0, 2.5f, 2.5f, 1, 1, false);
            }
            else
            {
                AttackBase(2, 1.1f, 1.6f, 0, 2.5f, 2.5f, 1, 1, false);
            }
        }
    }

    private void Jibaku()
    {
        if (level <= 3)
        {
            throwing.ThrowStart(ThrowIndexJibaku);
        }
        else
        {
            throwing.ThrowStart(ThrowIndexJibakuBig);
        }
    }

    private void DeathEffectExtra()
    {
        BootDeathEffect(ed);
        deathEffectEnabled = false;
    }

    private void ForceDead()
    {
        nowHP = 0;
        SetState(State.Dead);
    }

}
