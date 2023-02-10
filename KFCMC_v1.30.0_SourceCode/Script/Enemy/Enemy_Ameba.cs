using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ameba : EnemyBase {

    public bool splitEnabled;
    const int splitNum = 2;
    const float splitRadius = 0.2f;
    const int id = 4;

    int attackSave = -1;
    Amusement_Mogisen amusementSave;
    bool itemDamaged;
    const int effRushReady = 4;

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (level >= 3) {
            actDistNum = 1;
        } else {
            actDistNum = 0;
        }
    }

    void MoveAttack2() {
        if (state == State.Attack) {
            EmitEffect(3);
            SetSpecialMove(trans.TransformDirection(Vector3.forward), 15f, 45f / 60f, EasingType.SineOut);
        }
    }

    protected override void Attack() {
        int attackTemp = 0;
        fbStepTime = 15f / 60f;
        fbStepMaxDist = 3f;
        if (level >= 3) {
            attackTemp = Random.Range(0, level == 3 ? 2 : 3);
            if (attackTemp == attackSave) {
                attackTemp = Random.Range(0, level == 3 ? 2 : 3);
            }
        }
        if (attackTemp == 0) {
            if (level >= 3) {
                ApproachOrSeparate(1.7f);
            }
            AttackBase(0, 1, 0.8f, 0, 1, GetAttackInterval(2.1f), 0);
        } else if (attackTemp == 1){
            ApproachOrSeparate(3f);
            AttackBase(1, 1.1f, 1.2f, 0, 1f, 1f + GetAttackInterval(1f, -2), 0);
        } else {
            SeparateFromTarget(6f);
            AttackBase(2, 1.25f, 1.7f, 0, 106f / 60f, 106f / 60f + GetAttackInterval(1f, -3), 0.5f, 1, true, 15);
            EmitEffect(effRushReady);
        }
        attackSave = attackTemp;
    }

    public override void ForceDeath() {
        splitEnabled = false;
        base.ForceDeath();
    }

    public override void SetForAmusement(Amusement_Mogisen amusement) {
        base.SetForAmusement(amusement);
        amusementSave = amusement;
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        itemDamaged = (colorType == damageColor_Critical);
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    protected override void BootDeathEffect(EnemyDeath enemyDeath) {
        base.BootDeathEffect(enemyDeath);
        if (level >= 2 && splitEnabled && !itemDamaged) {
            float angStart = transform.eulerAngles.y + 360f / splitNum / 2f;
            float angDiff = 360f / splitNum;
            GameObject prefab = CharacterDatabase.Instance.GetEnemy(id);
            if (prefab != null) {
                for (int i = 0; i < splitNum; i++) {
                    Vector3 childPos = trans.position;
                    float angle = (angStart + angDiff * i) * Mathf.Deg2Rad;
                    childPos.x += Mathf.Sin(angle) * splitRadius;
                    childPos.z += Mathf.Cos(angle) * splitRadius;
                    EnemyBase eBase = Instantiate(prefab, childPos, trans.rotation, trans.parent).GetComponent<EnemyBase>();
                    if (eBase != null) {
                        eBase.enemyID = enemyID;
                        eBase.SetLevel(level - 1);
                        eBase.SetDropRate(0);
                        eBase.defeatCountEnabled = false;
                        eBase.excludeActionEnemy = true;
                        eBase.ed.numOfPieces = ed.numOfPieces / 2;
                        eBase.ed.scale = ed.scale * 1.2f;
                        if (isForAmusement && amusementSave) {
                            eBase.SetForAmusement(amusementSave);
                            amusementSave.AddEnemy(eBase);
                        }
                    }
                }
            }
        }
    }
}
