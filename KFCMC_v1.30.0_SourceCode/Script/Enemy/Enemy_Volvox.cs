using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Volvox : EnemyBase {

    public GameObject[] throwPrefab;

    int attackSave = -1;
    Vector3 vecRand = Vector3.one;
    const int throwPrefabSetMax = 4;
    const int dropItemIDHomo = 52;

    protected override void Awake() {
        base.Awake();
        attackedTimeRemainOnRestoreKnockDown = 0.5f;
    }

    protected override void SetLevelModifier() {
        actDistNum = level;
        for (int i = 0; i < throwPrefabSetMax; i++) {
            throwing.throwSettings[i].prefab = throwPrefab[Mathf.Clamp(level, 0, throwPrefab.Length - 1)];
        }
        if (isHomoChild) {
            dropItem[0] = dropItemIDHomo;
        }
    }

    void SetThrowing(int index) {
        float speed = 10f;
        if (targetTrans) {
            float distance = GetTargetDistance(false, false, false);
            if (distance > 8f) {
                speed += (distance - 8f) * 0.8f;
            }
        }
        if (index == 10) {
            index = Random.Range(0, 2);
        }
        for (int i = 0; i < throwPrefabSetMax; i++) {
            if (i == index) {
                throwing.throwSettings[i].randomDirection = vecZero;
            } else {
                throwing.throwSettings[i].randomDirection = vecRand;
            }
            throwing.throwSettings[i].velocity = speed;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (nowHP <= GetMaxHP() / 4) {
            attackedTimeRemain = -100;
        }
    }

    protected override void Attack() {
        base.Attack();
        if (nowHP > GetMaxHP() / 4) {
            int attackTemp = 0;
            if (level >= 2 && (nowHP < GetMaxHP() || isSuperman)) {
                attackTemp = Random.Range(0, level);
                if (attackTemp == attackSave) {
                    attackTemp = Random.Range(0, level);
                }
            }
            attackSave = attackTemp;
            if (targetTrans) {
                for (int i = 0; i < 4; i++) {
                    throwing.throwSettings[i].velocity = 2f + Vector3.Distance(targetTrans.position, GetCenterPosition());
                }
            }
            switch (attackTemp) {
                case 0:
                    if (targetTrans) {
                        if (Vector3.Cross(trans.TransformDirection(vecForward), targetTrans.position - trans.position).y > 0) {
                            //Right
                            AttackBase(0, 1, 1.1f, 0, 60f / 60f, 60f / 60f + GetAttackInterval(2f));
                        } else {
                            //Left
                            AttackBase(1, 1, 1.1f, 0, 60f / 60f, 60f / 60f + GetAttackInterval(2f));
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
        } else {
            if (level >= 2) {
                fbStepTime = 0.5f;
                fbStepMaxDist = 2f;
                StepToTarget(0);
            }
            float knockTemp = (level >= 4 ? 5600 : level == 3 ? 4200 : level == 2 ? 2800 : 1400);
            knockRemain = knockTemp;
            knockEndurance = knockTemp;
            knockRemainLight = knockTemp;
            knockEnduranceLight = knockTemp;
            if (level >= 4) {
                AttackBase(6, 1.3f, 4f, 0, 2.5f, 2.5f, 1, 1, false);
            } else {
                AttackBase(2, 1.1f, 1.6f, 0, 2.5f, 2.5f, 1, 1, false);
            }
        }
    }

    private void Jibaku() {
        if (level <= 3) {
            throwing.ThrowStart(4);
        } else {
            throwing.ThrowStart(5);
        }
    }

    private void DeathEffectExtra() {
        BootDeathEffect(ed);
        deathEffectEnabled = false;
    }

    private void ForceDead() {
        nowHP = 0;
        SetState(State.Dead);
    }

}
