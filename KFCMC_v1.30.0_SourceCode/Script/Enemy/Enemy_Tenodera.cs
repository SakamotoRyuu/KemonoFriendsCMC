using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Tenodera : EnemyBase {

    public DamageDetection criticalDD;
    public GameObject[] scytheObj;
    public GameObject scytheRemainPrefab;
    public Transform[] scytheRemainPivot;
    public Transform headPivot;
    public GameObject headPrefab;

    int attackSave = -1;
    const int manyThrowIndex = 1;
    const int eff_scytheLost = 4;
    const int eff_scytheRestore = 5;
    bool scytheEnabled;
    float scytheRestoreTime;
    static readonly float[] damageRateArray = new float[5] { 4f, 4f, 3.833333f, 3.666666f, 3.5f };

    void SetScythe(bool flag) {
        for (int i = 0; i < scytheObj.Length; i++) {
            scytheObj[i].SetActive(flag);
        }
        scytheEnabled = flag;
    }

    protected override void SetLevelModifier() {
        if (level <= 1) {
            actDistNum = 0;
        } else {
            actDistNum = 1;
        }
        if (criticalDD) {
            criticalDD.damageRate = damageRateArray[Mathf.Clamp(level, 0, damageRateArray.Length - 1)];
        }
        SetScythe(level >= 3);
        attackSave = -1;
    }

    protected override void Update_AnimControl() {
        base.Update_AnimControl();
        UpdateAC_Run();
    }

    protected override void KnockLightProcess() {
        if (!isSuperarmor && knockRestoreSpeed != 1f) {
            knockRestoreSpeed = 1f;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        }
        base.KnockLightProcess();
    }

    protected override void KnockHeavyProcess() {
        if (knockRestoreSpeed != 0.5f) {
            knockRestoreSpeed = 0.5f;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        }
        if (scytheEnabled) {
            SetScythe(false);
            EmitEffect(eff_scytheLost);
            Vector3 velocity = (vecUp * Random.Range(9f, 11f) + transform.TransformDirection(vecBack) * Random.Range(2f, 3f));
            for (int i = 0; i < scytheRemainPivot.Length; i++) {
                Rigidbody rigid = Instantiate(scytheRemainPrefab, scytheRemainPivot[i].position, scytheRemainPivot[i].rotation).GetComponent<Rigidbody>();
                rigid.AddForce(velocity, ForceMode.VelocityChange);
                rigid.AddRelativeTorque(Random.Range(-800f, -640f), 0f, 0f, ForceMode.VelocityChange);
            }
        }
        base.KnockHeavyProcess();
    }

    void ThrowStartMany() {
        if (throwing && throwing.throwSettings.Length > manyThrowIndex) {
            for (int i = 0; i < 15; i++) {
                throwing.ThrowStart(manyThrowIndex);
            }
        }
    }

    protected override void DeadProcess() {
        if (level >= 4) {
            Instantiate(headPrefab, headPivot.position, headPivot.rotation);
        }
        base.DeadProcess();
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (level >= 3 && !scytheEnabled) {
            scytheRestoreTime += deltaTimeCache * CharacterManager.Instance.riskyIncrease;
            if (scytheRestoreTime >= 20f && GetCanControl()) {
                EmitEffect(eff_scytheRestore);
                SetScythe(true);
                scytheRestoreTime = 0f;
            }
        } else {
            scytheRestoreTime = 0f;
        }
    }

    protected override void Attack() {
        base.Attack();
        int attackTemp = Random.Range(0, 3);
        if (attackSave == attackTemp) {
            attackTemp = Random.Range(0, 3);
        }
        if (targetTrans && level >= 2) {
            if (GetTargetDistance(true, true, false) > 5f * 5f) {
                attackTemp = 3;
            }
        }
        attackSave = attackTemp;
        switch (attackTemp) {
            case 0:
                AttackBase(0, scytheEnabled ? 1.15f : 1f, scytheEnabled ? 1.3f : 1.1f, 0, 49f / 48f, 49f / 48f + GetAttackInterval(1.5f));
                break;
            case 1:
                AttackBase(1, scytheEnabled ? 1.15f : 1f, scytheEnabled ? 1.3f : 1.1f, 0, 60f / 50f, 60f / 50f + GetAttackInterval(1.5f));
                break;
            case 2:
                AttackBase(2, 0.9f, 0.6f, 0, 60f / 60f, 60f / 60f + GetAttackInterval(1.5f));
                break;
            case 3:
                AttackBase(3, 1f, 1.1f, 0, 100f / 60f, 100f / 60f + GetAttackInterval(1.5f, -1));
                break;
        }
    }

}
