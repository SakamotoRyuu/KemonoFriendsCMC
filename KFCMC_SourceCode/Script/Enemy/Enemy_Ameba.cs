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
    int weakenCount;

    const int effRushReady = 4;

    // UpperAim
    public Transform upperAimAnglePivot;
    public Transform upperAimMultiplier;
    public UpperAimBoneClass[] upperAimBones;
    private const int upperAimLevel = 5;
    private bool upperAimEnabled;
    private float upperAimAngleSave;
    private bool upperAimAngleUpdateEnabled;

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

    void EndUpperAimAngleUpdate()
    {
        upperAimAngleUpdateEnabled = false;
    }

    private void LateUpdate()
    {
        if (upperAimEnabled && state == State.Attack)
        {
            if (upperAimAngleUpdateEnabled && targetTrans)
            {
                upperAimAngleSave = Mathf.Clamp(GetTargetUpperAngle(upperAimAnglePivot), 0, 90) * -1 + 90;
            }
            float mul = upperAimMultiplier.localPosition.y;
            if (mul != 0) {
                for (int i = 0; i < upperAimBones.Length; i++)
                {
                    Vector3 euler = upperAimBones[i].boneTransform.localEulerAngles;
                    euler.x = upperAimAngleSave * mul * upperAimBones[i].angleMultiplier + upperAimBones[i].angleOffset;
                    upperAimBones[i].boneTransform.localEulerAngles = euler;
                }
            }
        }
    }

    private int GetAttackRandom()
    {
        return Random.Range(level >= 5 ? 1 : 0, level == 3 ? 2 : 3);
    }

    protected override void Attack() {
        int attackTemp = 0;
        fbStepTime = 15f / 60f;
        fbStepMaxDist = 3f;
        upperAimEnabled = false;
        upperAimAngleUpdateEnabled = false;
        upperAimAngleSave = 0;
        if (level >= 3) {
            attackTemp = GetAttackRandom();
            if (attackTemp == attackSave) {
                attackTemp = GetAttackRandom();
            }
        }
        if (attackTemp == 0) {
            //if (level >= 3) {
            //    ApproachOrSeparate(1.7f);
            //}
            ApproachOrSeparateBetween(1.7f, 1f);
            AttackBase(0, 1, 0.8f, 0, 1, GetAttackInterval(2.1f), 0);
        } else if (attackTemp == 1){
            //ApproachOrSeparate(3f);
            ApproachOrSeparateBetween(3f, 1f);
            AttackBase(1, 1.1f, 1.2f, 0, 1f, 1f + GetAttackInterval(1f, -2), 0);
            if (level >= upperAimLevel)
            {
                upperAimEnabled = true;
                upperAimAngleUpdateEnabled = true;
            }
        } else {
            //SeparateFromTarget(6f);
            SeparateFromTarget(1f);
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
                    Enemy_Ameba eBase = Instantiate(prefab, childPos, trans.rotation, trans.parent).GetComponent<Enemy_Ameba>();
                    if (eBase != null) {
                        eBase.enemyID = enemyID;
                        eBase.SetLevel(level - 1, false, true, 
                            variableLevelSettings.nowVariableLevel <= 0 ? 0 : Mathf.Max(1, variableLevelSettings.nowVariableLevel - 20));
                        if (variableLevelSettings.nowVariableLevel == 1)
                        {
                            eBase.SetWeakening(weakenCount + 1);
                        }
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

    public void SetWeakening(int value)
    {
        weakenCount = value;
        float multiplier = 1f / (weakenCount + 1);
        nowHP = maxHP = Mathf.Max(Mathf.RoundToInt(maxHP * multiplier), 1);
        attackPower = Mathf.Max(Mathf.Round(attackPower * multiplier), 1);
        defensePower = Mathf.Max(Mathf.Round(defensePower * multiplier), 1);
        exp = Mathf.Max(Mathf.RoundToInt(exp * multiplier * multiplier), 1);
    }

}
