using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ornithocercus : EnemyBase
{

    public Transform throwFrom;
    public Transform throwFromBig;

    int moveContinuous = 0;
    int attackSaveNear = -1;
    int attackSaveFar = -1;
    int throwIndexSave;
    bool bigThrowed;

    const int ThrowIndexBig = 6;

    protected override void Awake()
    {
        base.Awake();
        throwing = GetComponent<Throwing>();
        attackedTimeRemainOnRestoreKnockDown = 0.5f;
    }

    protected override void Attack()
    {
        moveContinuous = 0;
        throwIndexSave = level;
        if (targetTrans && (trans.position - targetTrans.position).sqrMagnitude > 2 * 2)
        {
            int attackRand = 0;
            if (level >= 2)
            {
                attackRand = Random.Range(0, level >= 4 ? 3 : 2);
                if (attackRand == attackSaveFar)
                {
                    attackRand = Random.Range(0, level >= 4 ? 3 : 2);
                }
            }
            attackSaveFar = attackRand;
            if (attackRand == 0)
            {
                if (level >= 5 && !bigThrowed)
                {
                    AttackBase(5, 1.2f, 1.6f, 0, 1f, GetAttackInterval(3.5f, -3) - 1f, 1, 1, true, 30);
                    bigThrowed = true;
                }
                else
                {
                    AttackBase(0, 1f, 0.6f, 0, 1f, GetAttackInterval(3.5f) - 1f, 1, 1, true, 30);
                    bigThrowed = false;
                }
            }
            else if (attackRand == 1)
            {
                AttackBase(2, 1f, 0.6f, 0, 80f / 60f + (level >= 4 || IsSuperLevel ? 0f : level == 3 ? 0.6f : 1.2f), GetAttackInterval(80f / 60f + 2f, -1), 1, 1, true, 30);
            }
            else
            {
                AttackBase(4, 1.2f, 1.6f, 0, 160f / 60f, 160f / 60f + GetAttackInterval(1.5f, -3), 0f, 1f, true, 15f);
            }
        }
        else
        {
            int attackRand = 0;
            if (level >= 3)
            {
                attackRand = Random.Range(0, 2);
                if (attackRand == attackSaveNear)
                {
                    attackRand = Random.Range(0, 2);
                }
            }
            attackSaveNear = attackRand;
            if (attackRand == 0)
            {
                AttackBase(1, 1.1f, 1.1f, 0, 1.5f, 1.5f + GetAttackInterval(1f));
            }
            else
            {
                AttackBase(3, 1.1f, 1.3f, 0, 1.5f, 1.5f + GetAttackInterval(1f, -2));
            }
        }
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        if (attackType == 4 && moveContinuous != 0)
        {
            cCon.Move(trans.TransformDirection(vecForward) * GetMaxSpeed() * 5 * deltaTimeMove);
        }
    }

    private void MoveContinuous(int flag)
    {
        moveContinuous = flag;
        if (moveContinuous != 0)
        {
            lockonRotSpeed = 2;
        }
    }

    private void ThrowReadySpecial()
    {
        throwIndexSave = level;
        if (throwIndexSave < throwing.throwSettings.Length)
        {
            throwing.ThrowReady(throwIndexSave);
        }
    }

    private void ThrowStartSpecial()
    {
        if (throwIndexSave < throwing.throwSettings.Length)
        {
            throwing.ThrowStart(throwIndexSave);
        }
    }

    private void ThrowReadyBig()
    {
        throwing.ThrowReady(ThrowIndexBig);
    }

    private void ThrowStartBig()
    {
        throwing.ThrowStart(ThrowIndexBig);
    }

}
