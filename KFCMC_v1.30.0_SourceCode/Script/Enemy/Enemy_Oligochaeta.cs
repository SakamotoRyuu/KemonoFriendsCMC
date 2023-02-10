using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Oligochaeta : EnemyBase {

    public GameObject trapPrefab;
    public Transform trapPivot;
    public Transform[] movePivot;
    public GameObject divingRayObj;
    public DamageDetection criticalDD;

    float stiffPlus = 0.2f;
    int attackSave = -1;
    int attackSubSave = -1;
    int movingIndex = -1;
    float movingSpeed;
    float brakeMul = 1f;
    bool divingFlag;

    static readonly float[] knockRestoreArray = new float[] { 1f, 1f, 1f, 1.2f, 1.44f };
    static readonly float[] stiffPlusArray = new float[] { 0.3f, 0.3f, 0.2f, 0.1f, 0f };
    static readonly float[] damageRateArray = new float[5] { 4f, 4f, 3.833333f, 3.666666f, 3.5f };

    protected override void Awake() {
        base.Awake();
        attackWaitingLockonRotSpeed = 2f;
    }

    protected override void SetLevelModifier() {
        knockRestoreSpeed = knockRestoreArray[Mathf.Clamp(level, 0, knockRestoreArray.Length - 1)];
        stiffPlus = stiffPlusArray[Mathf.Clamp(level, 0, stiffPlusArray.Length - 1)];
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        actDistNum = (level >= 2 ? 1 : 0);
        if (criticalDD) {
            criticalDD.damageRate = damageRateArray[Mathf.Clamp(level, 0, damageRateArray.Length - 1)];
        }
        attackSave = -1;
    }

    protected override void CheckSilhouette() {
        if (divingFlag && state == State.Attack) {
            silhouetteSqrDist = -1f;
        } else {
            silhouetteSqrDist = 64f;
        }
        base.CheckSilhouette();
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (divingFlag && state != State.Attack) {
            SetDivingFlag(0);
        }
    }

    void SetDivingFlag(int flag) {
        divingFlag = (flag != 0);
        if (divingRayObj) {
            divingRayObj.SetActive(divingFlag);
        }
    }

    int GetAttackType() {
        int answer = (level >= 3 ? Random.Range(0, 3) : level >= 2 ? Random.Range(0, 2) : 0);
        if (level >= 2 && answer == 0 && targetTrans && (targetTrans.position - trans.position).sqrMagnitude > 8f * 8f) {
            if (level >= 3) {
                answer = Random.Range(1, 3);
            } else {
                answer = 1;
            }
        }
        return answer;
    }

    void SetTrap() {
        if (level >= 3 && trapPrefab && trapPivot) {
            ray.origin = trapPivot.position;
            ray.direction = vecDown;
            if (Physics.Raycast(ray, out raycastHit, 0.5f, fieldLayerMask, QueryTriggerInteraction.Ignore)) {
                GameObject trapInstance = Instantiate(trapPrefab, raycastHit.point, quaIden);
                MissingObjectToDestroy trapChecker = trapInstance.GetComponent<MissingObjectToDestroy>();
                if (trapChecker) {
                    trapChecker.SetGameObject(gameObject);
                }
            }
        }
    }
    
    void MoveStart(int index) {
        switch (index) {
            case 0:
                movingSpeed = 1.1f;
                brakeMul = 1.1f;
                break;
            case 1:
                movingSpeed = 1.32f;
                brakeMul = 1.32f;
                break;
            case 2:
                movingSpeed = 3f;
                brakeMul = 10f;
                lockonRotSpeed = 20f;
                LockonStart();
                break;
        }
        movingIndex = index;
    }

    void MoveEnd() {
        LockonEnd();
        movingIndex = -1;
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (state == State.Damage && isDamageHeavy) {
            knockAmount = 0;
        }
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    protected override void Attack() {
        base.Attack();
        divingFlag = false;
        resetAgentRadiusOnChangeState = true;
        movingIndex = -1;
        int attackTemp = GetAttackType();
        if (attackTemp == attackSave) {
            attackTemp = GetAttackType();
        }
        attackSave = attackTemp;
        float stiffTemp = stiffPlus;
        if (IsSuperLevel) {
            stiffTemp = 0f;
        }
        switch (attackTemp) {
            case 0:
                if (targetTrans && targetTrans.position.y > trans.position.y + 1f) {
                    AttackBase(1, 1f, 1.1f, 0f, 32f / 30f, 32f / 30f, 0f, 1f, true, attackLockonDefaultSpeed * 1.2f);
                    MoveStart(1);
                } else {
                    int attackSubTemp = 0;
                    if (level >= 4) {
                        attackSubTemp = Random.Range(0, 2);
                        if (attackSubTemp == attackSubSave) {
                            attackSubTemp = Random.Range(0, 2);
                        }
                        attackSubSave = attackSubTemp;
                    }
                    if (attackSubTemp == 1) {
                        AttackBase(3, 1f, 1.1f, 0f, 53f / 30f + stiffTemp, 53f / 30f + stiffTemp + GetAttackInterval(1.5f), 0f);
                    } else {
                        AttackBase(0, 1f, 1.1f, 0f, 25f / 30f + stiffTemp, 25f / 30f + (level >= 4 ? 0.9f : level >= 3 ? 1.2f : 1.5f) + Random.Range(-0.1f, 0.1f), 0f);
                    }
                    MoveStart(0);
                }
                break;
            case 1:
                AttackBase(2, 1f, 1.4f, 0f, 50f / 30f / 1.5f + stiffTemp, 50f / 30f / 1.5f + stiffTemp + GetAttackInterval(1.6f, -1), 0, 1.5f);
                break;
            case 2:
                AttackBase(4, 1.1f, 1.4f, 0f, 90f / 30f, 90f / 30f + GetAttackInterval(1.5f), 0.5f, 1f, false);
                agent.radius = 0.2f;
                break;
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (movingIndex >= 0 && movingIndex < movePivot.Length && movePivot[movingIndex]) {
            ApproachTransformPivot(movePivot[movingIndex], Mathf.Clamp(GetMinmiSpeed() * movingSpeed, 0f, 22.8f), 0.25f * brakeMul, 0.02f * movingSpeed, true);
        }
    }

}
