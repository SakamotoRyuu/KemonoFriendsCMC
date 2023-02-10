using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Polistes : EnemyBase {

    public AttackDetectionSick attackDetectionSick;
    public GameObject coreObj;
    public Material[] materialForChild;

    int attackSave = -1;
    float summonInterval;
    bool isChild;
    bool summoned;
    GameObject parentObj;
    static readonly int[] sickProbability = new int[] { 25, 25, 33, 40, 50 };
    const float attackSpeed = 0.75f;
    const int attackParticleIndex = 5;
    const float splitRadius = 0.2f;
    const int eff_summon = 3;

    protected override void SetLevelModifier() {
        if (isChild) {
            maxHP /= 3;
            SetDropRate(0);
            defeatCountEnabled = false;
            exp = 0;
            defensePower = 0;
            dampNormalDamage = false;
        }
        if (attackDetectionSick) {
            attackDetectionSick.probability = sickProbability[Mathf.Clamp(level, 0, sickProbability.Length - 1)];
        }
        if (level >= 2) {
            actDistNum = Random.Range(0, 2);
        } else {
            actDistNum = 0;
        }
        if (level <= 2) {
            summonInterval = 0f;
        }
    }

    public void SetForChild(GameObject parentObj) {
        this.parentObj = parentObj;
        for (int i = 0; i < levelStatus.Length && i < materialForChild.Length; i++) {
            levelStatus[i].changeMaterial[0] = materialForChild[i];
        }
        if (coreObj) {
            coreObj.SetActive(false);
        }
        dampNormalDamage = false;
        defensePower = 0;
        ed.numOfPieces /= 2;
        isChild = true;
        supermanCheckInterval = 10000f;
        attackedTimeRemain = Random.Range(1.0f, 2.0f);
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (level >= 3 && summonInterval > 0f) {
            summonInterval -= deltaTimeCache;
            if (summonInterval <= 0f) {
                EmitEffect(eff_summon);
                int splitNum = (level >= 4 ? 4 : 2);
                float angStart = transform.eulerAngles.y + 360f / splitNum / 2f;
                float angDiff = 360f / splitNum;
                GameObject prefab = CharacterDatabase.Instance.GetEnemy(enemyID);
                if (prefab != null) {
                    for (int i = 0; i < splitNum; i++) {
                        Vector3 childPos = trans.position;
                        float angle = (angStart + angDiff * i) * Mathf.Deg2Rad;
                        childPos.x += Mathf.Sin(angle) * splitRadius;
                        childPos.z += Mathf.Cos(angle) * splitRadius;
                        Enemy_Polistes eBase = Instantiate(prefab, childPos, trans.rotation, trans.parent).GetComponent<Enemy_Polistes>();
                        if (eBase != null) {
                            eBase.enemyID = enemyID;
                            eBase.SetForChild(gameObject);
                            eBase.SetLevel(level);
                        }
                    }
                }
            }
        }
        if (isChild){
            supermanCheckInterval = 10000f;
            if (parentObj == null) {
                ForceDeath();
            }
        }
    }

    protected override void Update_Transition_Moves() {
        base.Update_Transition_Moves();
        if (targetTrans) {
            float sqrDist = GetTargetDistance(true, true, false);
            if (sqrDist < 2f * 2f && targetTrans.position.y > trans.position.y + 2f) {
                SideStep_ConsiderWall(0, 4, 0f);
            }
        }
    }

    void ParticleAttackStart() {
        attackPowerMultiplier = 0.1f;
        knockPowerMultiplier = 0.6f;
        AttackStart(attackParticleIndex);
        if (!summoned && !isChild) {
            summoned = true;
            summonInterval = 1f;
        }
    }

    protected override void Attack() {
        base.Attack();
        int attackTemp = Random.Range(0, 3);
        if (attackTemp == attackSave) {
            attackTemp = Random.Range(0, 3);
        }
        if (level >= 2 && actDistNum == 1 && targetTrans && (targetTrans.position - trans.position).sqrMagnitude > 3f * 3f) {
            attackTemp = 3;
        }
        if (attackTemp <= 2) {
            if (attackTemp == 2 && level >= 3 && nowHP < GetMaxHP() && !summoned && !isChild) {
                AttackBase(4, 1f, 0.8f, 0, 24f / 24f / attackSpeed, 24f / 24f / attackSpeed + GetAttackInterval(1f, -2), 0, attackSpeed);
            } else {
                AttackBase(attackTemp, 1f, 0.8f, 0, 15f / 24f / attackSpeed, 15f / 24f / attackSpeed + GetAttackInterval(1f), 0, attackSpeed);
            }
            if (attackTemp == 0 || attackTemp == 1) {
                // ForwardOrBackStep(1.5f);
                fbStepTime = 0.25f;
                fbStepMaxDist = 2f;
                ApproachOrSeparate(1.5f);
            } else {
                SpecialStep(0.4f, 0.25f, 2f, 0f, 0f);
            }
        } else {
            AttackBase(3, 1f, 1.1f, 0, 80f / 60f, 80f / 60f + GetAttackInterval(1f, -1), 0);
        }
        attackSave = attackTemp;
        if (level >= 2) {
            actDistNum = Random.Range(0, 2);
        } else {
            actDistNum = 0;
        }
    }


}
