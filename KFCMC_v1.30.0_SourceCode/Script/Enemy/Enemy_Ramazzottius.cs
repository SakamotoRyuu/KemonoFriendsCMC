using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ramazzottius : EnemyBase {

    public GameObject[] criticalPoints;
    public GameObject[] normalDD;
    public GameObject[] anotherDD;
    public Transform quakePivot;

    int attackSave = -1;
    bool isNormal = false;
    public DamageDetection[] criticalDD;
    float healReserveTimeRemain;
    float healProcessTimeRemain;
    float trapAttackTimeRemain;
    const int effHeal = 5;
    const int effMolt = 6;
    const int effThrowReady = 7;
    static readonly Vector3 moltDirection = new Vector3(0, 1, -1);
    static readonly int[] criticalNum = new int[] { 2, 2, 3, 4, 5 };
    static readonly int[] damageZero = new int[] { 0, 0, 0, 6, 12 };
    static readonly float[] damageRateArray = new float[5] { 3.5f, 3.5f, 3.333333f, 3.166666f, 3.0f };
    static readonly float[] knockRestoreArray = new float[] { 1f, 1f, 1.1f, 1.21f, 1.331f };

    protected override void Awake() {
        base.Awake();
        attackWaitingLockonRotSpeed = 0.8f;
    }

    int RemainCriticalPoints() {
        int answer = 0;
        for (int i = 0; i< criticalPoints.Length; i++) {
            if (criticalPoints[i].activeSelf) {
                answer++;
            }
        }
        return answer;
    }

    void SetNormalDD(bool flag) {
        if (isNormal != flag) {
            isNormal = flag;
            dampNormalDamage = flag;
            for (int i = 0; i < normalDD.Length; i++) {
                normalDD[i].SetActive(flag);
            }
            for (int i = 0; i < anotherDD.Length; i++) {
                anotherDD[i].SetActive(!flag);
            }
        }
    }

    public void GetCritical(GameObject obj) {
        for (int i = 0; i < criticalPoints.Length; i++) {
            if (criticalPoints[i] == obj) {
                criticalPoints[i].SetActive(false);
                break;
            }
        }
        SetNormalDD(RemainCriticalPoints() > 0);
    }

    void ThrowStartSpecial() {
        if (state == State.Attack) {
            throwing.ThrowStart(0);
            trapAttackTimeRemain += 4f;
        } else {
            throwing.ThrowCancel(0);
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (level >= 3 && nowHP > 0) {
            if (healReserveTimeRemain > 0f) {
                healReserveTimeRemain -= deltaTimeCache * CharacterManager.Instance.riskyIncrease;
            }
            if (healReserveTimeRemain <= 0f && GetCanControl() && nowHP <= GetMaxHP() * 4 / 5) {
                healReserveTimeRemain = 12f;
                if (RemainCriticalPoints() > 0) {
                    EmitEffect(effHeal);
                    EmitEffect(effMolt);
                    healProcessTimeRemain = 0.5f;
                }
            }
            if (healProcessTimeRemain > 0f) {
                healProcessTimeRemain -= deltaTimeCache;
                if (healProcessTimeRemain <= 0f) {
                    if (effect[effMolt].instance) {
                        effect[effMolt].instance.transform.SetParent(null);
                        Rigidbody moltRigid = effect[effMolt].instance.GetComponent<Rigidbody>();
                        if (moltRigid) {
                            moltRigid.AddForce(transform.TransformDirection(moltDirection.normalized) * 10f, ForceMode.VelocityChange);
                        }
                    }
                    AddNowHP(GetMaxHP() / 5, GetCenterPosition(), true, damageColor_Heal);
                }
            }
        }
        if (trapAttackTimeRemain > 0f) {
            trapAttackTimeRemain -= deltaTimeMove;
        }
        if (actDistNum == 1 && (level <= 3 || trapAttackTimeRemain > 0f)) {
            actDistNum = 0;
        }
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (dampNormalDamage && colorType != damageColor_Critical) {
            knockRemain = knockEndurance;
        }
        if (damageZero[level] > 0 && RemainCriticalPoints() > 0) {
            if (nowHP <= damageZero[level]) {
                damage = 1;
            } else if (damage > nowHP - damageZero[level]) {
                damage = nowHP - damageZero[level];
            }
        }
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    public override float GetDefense(bool ignoreMultiplier = false) {
        return base.GetDefense(ignoreMultiplier) * (RemainCriticalPoints() > 0 ? 1 : 0);
    }

    protected void SetCriticalPoint() {
        int[] indexes = new int[criticalPoints.Length];
        for (int i = 0; i < indexes.Length; i++) {
            indexes[i] = i;
        }
        for (int i = indexes.Length - 1; i > 0; i--){
            int j = Random.Range(0, i + 1);
            int tmp = indexes[i];
            indexes[i] = indexes[j];
            indexes[j] = tmp;
        }
        for (int i = 0; i < indexes.Length; i++) {
            if (i < criticalNum[level]) {
                criticalPoints[indexes[i]].SetActive(true);
            } else {
                criticalPoints[indexes[i]].SetActive(false);
            }
        }
        isNormal = false;
        SetNormalDD(true);
    }

    protected override void SetLevelModifier() {
        SetCriticalPoint();
        for (int i = 0; i < criticalDD.Length; i++) {
            if (criticalDD[i]) {
                criticalDD[i].damageRate = damageRateArray[Mathf.Clamp(level, 0, damageRateArray.Length - 1)];
            }
        }
        knockRestoreSpeed = knockRestoreArray[Mathf.Clamp(level, 0, knockRestoreArray.Length - 1)];
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        attackSave = -1;
        healReserveTimeRemain = 0f;
        actDistNum = 0;
    }

    void MoveAttack0() {
        if (state == State.Attack) {
            SpecialStep(0.8f, 25f / 60f, 5f, 0, 0, true, false);
        }
    }

    void MoveAttack1_0() {
        if (state == State.Attack) {
            SpecialStep(1.4f, 25f / 60f, 5f, 0, 0, true, false);
        }
    }

    void MoveAttack1_1() {
        if (state == State.Attack) {
            SpecialStep(1.4f, 20f / 60f, 2f, 0, 0, true, false);
        }
    }

    void MoveAttack2() {
        if (state == State.Attack) {
            SpecialStep(0f, 40f / 60f, 8f, 0f, 0f, true, false);
        }
    }

    void QuakeAttack() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePivot.position, 8, 4, 0, 0, 1.5f, 2f, dissipationDistance_Normal);
        }
    }

    int GetAttackType() {
        if (level >= 4 && trapAttackTimeRemain <= 0f) {
            if (targetTrans && GetTargetDistance(true, false, false) > 6f * 6f) {
                return 3;
            }
            if (attackSave != 3) {
                return Random.Range(0, 4);
            }
        }
        return Random.Range(0, 3);
    }

    protected override void Attack() {
        base.Attack();
        resetAgentRadiusOnChangeState = true;
        int attackTemp = GetAttackType();
        if (attackTemp == attackSave) {
            attackTemp = GetAttackType();
        }
        attackSave = attackTemp;
        switch (attackTemp) {
            case 0:
                agent.radius = 0.1f;
                AttackBase(0, 1f, 1.1f, 0f, 90f / 60f + (IsSuperLevel ? 0f : 0.5f), 90f / 60f + GetAttackInterval(1.5f, -1));
                break;
            case 1:
                AttackBase(1, 1f, 1.2f, 0f, 120f / 60f + (IsSuperLevel ? 0f : 0.8f), 120f / 60f + GetAttackInterval(1.7f, -1));
                break;
            case 2:
                agent.radius = 0.1f;
                AttackBase(2, 1.1f, 1.7f, 0f, 120f / 60f + (IsSuperLevel ? 0f : 0.8f), 120f / 60f + GetAttackInterval(1.7f, -1));
                break;
            case 3:
                AttackBase(3, 0f, 1f, 0, 120f / 60f, 120f / 60f + GetAttackInterval(1.5f, -3), 0);
                fbStepMaxDist = 4f;
                fbStepTime = 40f / 60f;
                SeparateFromTarget(4f);
                EmitEffect(effThrowReady);
                throwing.ThrowReady(0);
                trapAttackTimeRemain = Random.Range(10f, 12f);
                break;
        }
        if (level >= 4 && trapAttackTimeRemain <= 0f && Random.Range(0, 100) < 50) {
            actDistNum = 1;
        } else {
            actDistNum = 0;
        }
    }

}
