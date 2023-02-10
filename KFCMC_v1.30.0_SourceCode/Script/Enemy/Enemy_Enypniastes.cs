using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Enypniastes : EnemyBase{

    public Renderer emitRend;
    public DamageDetection criticalDD;
    
    Material[] rendMats = new Material[2];
    int throwCount = 0;
    int throwMax = 0;
    float emitPower = 0;
    float emitTimeRemain = 0;
    int attackSave = -1;
    int attackSubSave = -1;
    bool isEmitting;
    int emissionPropertyID;
    static readonly Color emissionColor = new Color(1.5f, 1.5f, 1.5f);
    static readonly float[] damageRateArray = new float[5] { 4f, 4f, 3.833333f, 3.666666f, 3.5f };
    static readonly float[] lockonSpeedArray = new float[5] { 6f, 6f, 7.2f, 8.64f, 10.368f };
    const int throw_Big = 6;
    const int throw_Circle = 7;

    protected override void Awake() {
        base.Awake();
        attackedTimeRemainOnRestoreKnockDown = 0.5f;
        attackLockonDefaultSpeed = 6f;
        attackWaitingLockonRotSpeed = 1.5f;
        emissionPropertyID = Shader.PropertyToID("_EmissionColor");
    }

    void ThrowSpecial() {
        if (attackType < 2) {
            int emitBias = isEmitting ? 3 : 0;
            if (throwCount < throwMax) {
                throwing.ThrowStart(emitBias + (throwCount == 0 ? 0 : 1));
                throwCount++;
            }
        } else {
            if (throwCount < throwMax) {
                throwing.ThrowStart(throw_Big);
                throwCount = 100;
            }
        }
    }

    void ThrowSpecial2() {
        if (attackType < 2) {
            int emitBias = isEmitting ? 3 : 0;
            throwing.ThrowStart(emitBias + 2);
        } else {
            int subTemp = 0;
            if (level >= 4) {
                subTemp = Random.Range(0, 2);
                if (subTemp == attackSubSave) {
                    subTemp = Random.Range(0, 2);
                }
            }
            attackSubSave = subTemp;
            throwing.ThrowStart(throw_Circle + subTemp);
        }
    }

    protected override void Start() {
        base.Start();
        SetEmitMaterial(false);
    }

    void SetEmitMaterial(bool toEmit) {
        rendMats = emitRend.materials;
        if (toEmit) {
            rendMats[0].SetColor(emissionPropertyID, emissionColor);
        } else {
            rendMats[0].SetColor(emissionPropertyID, Color.black);
        }
        emitRend.materials = rendMats;
        isEmitting = toEmit;
    }

    protected override void SetLevelModifier() {
        emitPower = 0;
        emitTimeRemain = 0;
        SetEmitMaterial(false);
        if (criticalDD) {
            criticalDD.damageRate = damageRateArray[Mathf.Clamp(level, 0, damageRateArray.Length - 1)];
        }
        attackLockonDefaultSpeed = lockonSpeedArray[Mathf.Clamp(level, 0, lockonSpeedArray.Length - 1)];
        attackWaitingLockonRotSpeed = attackLockonDefaultSpeed * 0.25f;
    }

    public override float GetKnocked() {
        return base.GetKnocked() * (isEmitting ? 4f : 1f);
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        bool margayVoiceSave = !fixKnockAmount && colorType == damageColor_Critical && enemyCanvasLoaded && enemyCanvasChildObject[(int)EnemyCanvasChild.margayVoice].activeSelf && attacker != null && attacker == CharacterManager.Instance.pCon;
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        if (nowHP > 0 && colorType == damageColor_Enemy && !isEmitting) {
            emitPower += knockAmount * 0.1f * (attacker.isPlayer ? 1f : 0.5f);
            if (emitPower > 15f) {
                emitPower = 0f;
                SetEmitMaterial(true);
                emitTimeRemain = 4f;
            }
        }
        if (margayVoiceSave && nowHP <= 0) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_EnypniastesMargay, true);
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (isEmitting) {
            emitTimeRemain -= deltaTimeCache;
            if (emitTimeRemain <= 0 && state != State.Attack) {
                SetEmitMaterial(false);
            }
        }
        if (emitPower > 0f) {
            emitPower -= deltaTimeCache;
            if (emitPower < 0f) {
                emitPower = 0f;
            }
        }
    }

    int GetAttackType() {
        int answer = 0;
        if (level >= 3 && isEmitting) {
            answer = Random.Range(0, 4);
        } else if (level >= 2 && isEmitting) {
            answer = Random.Range(0, 3);
        } else {
            answer = Random.Range(0, 2);
        }
        return answer;
    }

    protected override void Attack() {
        base.Attack();
        float intervalPlus = (isEmitting ? 0.5f : 1.5f);
        int attackTemp = GetAttackType();
        if (attackTemp == attackSave) {
            attackTemp = GetAttackType();
        }
        attackSave = attackTemp;
        if (targetTrans && attackTemp == 1 && GetTargetDistance(true, true, false) >= 7.5f * 7.5f) {
            attackTemp = 0;
            attackSave = -1;
        }
        switch (attackTemp) {
            case 0:
                throwCount = 0;
                throwMax = 3;
                AttackBase(0, isEmitting ? 1.2f : 1f, isEmitting ? 1.2f : 0.6f, 0f, 60f / 60f, 60f / 60f + GetAttackInterval(intervalPlus));
                break;
            case 1:
                AttackBase(1, isEmitting ? 1.2f : 1f, isEmitting ? 1.2f : 0.6f, 0f, 90f / 60f, 90f / 60f + GetAttackInterval(intervalPlus), 1f, 1f, false);
                break;
            case 2:
                throwCount = 0;
                throwMax = 1;
                AttackBase(2, 1.2f, 1.2f, 0, 60f / 60f, 60f / 60f + GetAttackInterval(intervalPlus));
                break;
            case 3:
                AttackBase(3, 1.2f, 1.2f, 0, 90f / 60f, 90f / 60f + GetAttackInterval(intervalPlus), 1f, 1f, false);
                break;
        }
    }
}
