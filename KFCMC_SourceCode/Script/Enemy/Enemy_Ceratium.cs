using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ceratium : EnemyBase {

    int attackSaveNear = -1;
    int attackSaveFar = -1;

    // UpperAim
    public Transform upperAimAnglePivot;
    public Transform upperAimMultiplier;
    public UpperAimBoneClass[] upperAimBones;
    private const int upperAimLevel = 5;
    private bool upperAimEnabled;
    private float upperAimAngleSave;
    private bool upperAimAngleUpdateEnabled;

    protected override void SetLevelModifier() {
        if (level >= 3) {
            actDistNum = 1;
        } else {
            actDistNum = 0;
        }
    }

    protected override void Attack()
    {
        upperAimEnabled = false;
        upperAimAngleUpdateEnabled = false;
        if (targetTrans) {
            float sqrDist = GetTargetDistance(true, true, false);
            if (sqrDist > 2f) {
                int attackRand = Random.Range(0, 3);
                if (attackRand == attackSaveFar) {
                    attackRand = Random.Range(0, 3);
                }
                attackSaveFar = attackRand;
                if (level >= 4 && attackRand == 2) {
                    fbStepTime = 20f / 60f;
                    fbStepMaxDist = 4f;
                    //ApproachOrSeparate(4.2f);
                    ApproachOrSeparateBetween(4.2f, 2.1f);
                    AttackBase(3, 1.2f, 1.6f, 0, 110f / 60f, 110f / 60f + GetAttackInterval(1f, -3), 0);
                } else {
                    int subType = (level >= 3 && sqrDist > 4f ? 4 : 0);
                    AttackBase(subType, 1.1f, 1.2f, 0, 100f / 60f, 100f / 60f + GetAttackInterval(1f, subType == 4 ? -2 : 0), 0);
                    if (level >= upperAimLevel)
                    {
                        upperAimEnabled = true;
                        upperAimAngleUpdateEnabled = true;
                    }
                }
            } else {
                int attackRand = Random.Range(0, 2);
                if (attackRand == attackSaveNear) {
                    attackRand = Random.Range(0, 2);
                }
                attackSaveNear = attackRand;
                if (level >= 2 && attackRand == 1) {
                    AttackBase(2, 1, 0.8f, 0, 170f / 60f, 170f / 60f + GetAttackInterval(1f, -1), 1, 1, false);
                } else {
                    AttackBase(1, 1, 0.8f, 0, 1.5f, 1.5f + GetAttackInterval(1f), 1, 1, false);
                }
            }
        }
    }

    protected override void ResetTriggerOnDamage() {
        base.ResetTriggerOnDamage();
        if (attackedTimeRemain > 0.5f) {
            attackedTimeRemain = 0.5f;
        }
    }

    private void EmitEffectOnAttack(int index)
    {
        if (state == State.Attack)
        {
            EmitEffect(index);
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
                upperAimAngleSave = Mathf.Clamp(GetTargetUpperAngle(upperAimAnglePivot), 0, 90);
            }
            float mul = upperAimMultiplier.localPosition.y;
            if (mul != 0)
            {
                for (int i = 0; i < upperAimBones.Length; i++)
                {
                    Vector3 euler = upperAimBones[i].boneTransform.localEulerAngles;
                    euler.x = upperAimAngleSave * mul * upperAimBones[i].angleMultiplier + upperAimBones[i].angleOffset;
                    upperAimBones[i].boneTransform.localEulerAngles = euler;

                    Vector3 position = upperAimBones[i].boneTransform.localPosition;
                    position.z = upperAimAngleSave * mul * upperAimBones[i].positionMultiplier + upperAimBones[i].positionOffset;
                    upperAimBones[i].boneTransform.localPosition = position;
                }
            }
        }
    }
}
