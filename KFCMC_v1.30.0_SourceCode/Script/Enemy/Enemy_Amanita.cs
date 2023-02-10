using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Amanita : EnemyBase {

    int attackSave = -1;
    float particleAttackedTimeRemain;
    static readonly float[] knockRestoreArray = new float[] { 1f, 1f, 1f, 1.15f, 1.3f };
    static readonly int[] dropRateArray = new int[5] { 120, 120, 120, 120, 200 };
    const int particleIndex = 2;

    protected override void SetLevelModifier() {
        dropRate[0] = dropRateArray[Mathf.Clamp(level, 0, dropRateArray.Length - 1)];
        knockRestoreSpeed = knockRestoreArray[Mathf.Clamp(level, 0, knockRestoreArray.Length - 1)];
        if (anim) {
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        }
    }

    void MoveAttack(int index) {
        if (state == State.Attack) {
            switch (index) {
                case 0:
                    SpecialStep(0.5f, 20f / 60f, 4f, 0f, 0f);
                    break;
                case 1:
                    fbStepTime = 20f / 60f;
                    fbStepMaxDist = 4f;
                    fbStepEaseType = EasingType.SineInOut;
                    BackStep(5f);
                    break;
                case 2:
                    SetSpecialMove(trans.TransformDirection(vecForward), 10f, 50f / 60f, EasingType.SineOut);
                    break;
            }
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (particleAttackedTimeRemain > 0f) {
            particleAttackedTimeRemain -= deltaTimeCache;
        }
    }

    protected override void Update_Process_Damage() {
        base.Update_Process_Damage();
        if (isDamageHeavy) {
            knockRemain = knockEndurance;
        }
        knockRemainLight = knockEnduranceLight;
    }
    
    void ParticleAttackStart() {
        if (state == State.Attack) {
            AttackStart(particleIndex);
            particleAttackedTimeRemain = 12f;
        }
    }

    int GetAttackType() {
        return Random.Range(0, level >= 4 && particleAttackedTimeRemain <= 0f ? 5 : level >= 3 ? 4 : level >= 2 ? 3 : 2);
    }

    protected override void Attack() {
        base.Attack();
        int attackTemp = GetAttackType();
        if (attackSave == attackTemp) {
            attackTemp = GetAttackType();
        }
        attackSave = attackTemp;
        switch (attackTemp) {
            case 0:
                AttackBase(0, 1f, 0.8f, 0f, 60f / 60f, 60f / 60f + GetAttackInterval(1.15f));
                break;
            case 1:
                AttackBase(1, 1.1f, 1.2f, 0f, 120f / 60f, 120f / 60f + GetAttackInterval(1.5f));
                break;
            case 2:
                AttackBase(2, 0.6f, 0.6f, 0f, 70f / 60f, 70f / 60f + GetAttackInterval(1.5f), 0f);
                break;
            case 3:
                AttackBase(3, 0.2f, 0.6f, 0f, 70f / 60f, 70f / 60f + GetAttackInterval(1.5f), 0f);
                break;
            case 4:
                AttackBase(4, 0.1f, 0.6f, 0, 90f / 60f, 90f / 60f + GetAttackInterval(1.5f), 0.5f, 1, false);
                SuperarmorStart();
                particleAttackedTimeRemain = 8f;
                break;
        }
    }

}
