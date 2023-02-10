using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Araneus : EnemyBase {

    public Transform quakePivot;
    public GameObject eggPrefab;
    public Transform eggPivot;

    int attackSave = -1;
    float eggInterval = 4f;
    static readonly float[] knockRestoreArray = new float[] { 1f, 1f, 1.1f, 1.21f, 1.331f };
    const int eff_egg = 5;

    void ThrowSpecial() {
        throwing.ThrowStart(0);
        if (level >= 2) {
            int throwTypeTemp = Random.Range(0, 2) * 2;
            throwing.ThrowStart(1 + throwTypeTemp);
            throwing.ThrowStart(2 + throwTypeTemp);
        }
    }

    void MoveAttack0() {
        if (state == State.Attack && targetTrans) {
            fbStepTime = 8f / 30f;
            fbStepMaxDist = 4f;
            ApproachOrSeparate(0.6f);
        }
    }

    void MoveAttack1() {
        if (state == State.Attack && targetTrans) {
            fbStepTime = 8f / 30f;
            fbStepMaxDist = 4f;
            ApproachOrSeparate(2f);
        }
    }

    void MoveAttack2() {
        if (state == State.Attack && targetTrans) {
            fbStepTime = 16f / 30f;
            fbStepMaxDist = 4f;
            SeparateFromTarget(4f);
        }
    }

    void MoveAttack4() {
        if (state == State.Attack && targetTrans) {
            Vector3 targetTemp = targetTrans.position;
            targetTemp.y = quakePivot.position.y;
            float distance = Vector3.Distance(targetTemp, quakePivot.position);
            SetSpecialMove((targetTemp - quakePivot.position).normalized, distance, 50f / 60f, EasingType.SineOut);
        }
    }

    void QuakeAttack4() {
        if (state == State.Attack) {
            CameraManager.Instance.SetQuake(quakePivot.position, 8, 4, 0, 0, 1.5f, 2f, dissipationDistance_Normal);
        }
    }

    int GetAttackIndex() {
        return Random.Range(0, level >= 3 ? 4 : 3);
    }

    protected override void SetLevelModifier() {
        knockRestoreSpeed = knockRestoreArray[Mathf.Clamp(level, 0, knockRestoreArray.Length - 1)];
        anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (level >= 4) {
            if (target) {
                eggInterval -= deltaTimeCache * (isSuperman ? 4f / 3f : 1f) * CharacterManager.Instance.riskyIncrease;
                if (eggInterval <= 0f && GetCanControl()) {
                    eggInterval = Random.Range(10f, 12f);
                    GameObject eggInstance = Instantiate(eggPrefab, eggPivot.position, eggPivot.rotation);
                    AraneusEgg araneusEgg = eggInstance.GetComponent<AraneusEgg>();
                    if (araneusEgg) {
                        araneusEgg.parentCBase = this;
                    }
                    Rigidbody eggRigid = eggInstance.GetComponent<Rigidbody>();
                    if (eggRigid) {
                        eggRigid.AddForce(0f, 5f, 0f, ForceMode.VelocityChange);
                    }
                    EmitEffect(eff_egg);
                }
            }
        } else {
            eggInterval = 4f;
        }
    }

    protected override void Attack() {
        base.Attack();
        resetAgentRadiusOnChangeState = true;
        int attackTemp = GetAttackIndex();
        if (attackTemp == attackSave) {
            attackTemp = GetAttackIndex();
        }
        attackSave = attackTemp;
        switch (attackTemp) {
            case 0:
                AttackBase(0, 1, 1.2f, 0, 42f / 30f, 42f / 30f + GetAttackInterval(1.5f), 0f);
                break;
            case 1:
                AttackBase(1, 1, 1.2f, 0, 48f / 30f, 48f / 30f + GetAttackInterval(1.5f), 0f);
                break;
            case 2:
                float angle = Vector3.Angle((targetTrans.position - trans.position), trans.TransformDirection(vecForward));
                if (angle > 160) {
                    AttackBase(3, 0.1f, 0.6f, 0, 20f / 30f, 20f / 30f + GetAttackInterval(1.5f), 1, 1, false);
                } else {
                    AttackBase(2, 0.1f, 0.6f, 0, 60f / 30f, 60f / 30f + GetAttackInterval(1.5f));
                }
                break;
            case 3:
                AttackBase(4, 1.2f, 1.7f, 0, 105f / 60f + (IsSuperLevel ? 0f : 0.3f), 105f / 60f + GetAttackInterval(1.5f, -2), 0f);
                agent.radius = 0.05f;
                break;
        }
    }

}
