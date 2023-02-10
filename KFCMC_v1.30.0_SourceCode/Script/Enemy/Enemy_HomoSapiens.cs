using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HomoSapiens : EnemyBaseBoss {

    public bool isReaper;
    public GameObject[] normalObj;
    public GameObject[] weakObj;
    
    bool isStarted;
    bool healEffectEmitted;
    bool isWalking;
    int summonCount;
    static readonly int[] summonID = new int[] { 39, 39, 39, 39, 11};
    static readonly int[] summonLevel = new int[] { 1, 2, 3, 4, 4 };
    float jibakuTimer;
    float knockItemTimeInterval;
    const int stealedItemID = 56;
    const int knockDownItemID = 59;
    const int dropKeyID = 337;
    const int effectShowCore = 0;
    const int effectHideCore = 1;
    const int effectDeadSave = 2;
    const int effectFindItem = 3;

    protected override void Awake() {
        base.Awake();
        deadTimer = 6f;
        isAnimParamDetail = true;
        HideCore();
        isCoreShowed = false;
        healEffectEmitted = false;
        coreTimeRemain = 0f;
        coreTimeMax = 30f;
        retargetingConditionTime = 4f;
        spawnStiffTime = 1f;
        attackedTimeRemainOnDamage = 10f;
        roveInterval = 8f;
        attractionTime = 1.5f;
        confuseTime = 1.5f;
        angryFixTime = 2f;
        coreHideDenomi = 10f;
        stealedMax = 10000000;
        cannotDoubleKnockDown = true;
        catchupExpDisabled = true;
        supermanKnockPlus = 0f;
        supermanEffectNumber = (int)EffectDatabase.id.enemyYK_Man;
        supermanAuraNumber = (int)EffectDatabase.id.enemyYK_Aura;
        SetSupermanEffect();
    }

    void HideCore() {
        for (int i = 0; i < normalObj.Length; i++) {
            if (normalObj[i] && !normalObj[i].activeSelf) {
                normalObj[i].SetActive(true);
            }
        }
        for (int i = 0; i < weakObj.Length; i++) {
            if (weakObj[i] && weakObj[i].activeSelf) {
                weakObj[i].SetActive(false);
            }
        }
        isCoreShowed = false;
    }

    void ShowCore() {
        for (int i = 0; i < weakObj.Length; i++) {
            if (weakObj[i] && !weakObj[i].activeSelf) {
                weakObj[i].SetActive(true);
            }
        }
        for (int i = 0; i < normalObj.Length; i++) {
            if (normalObj[i] && normalObj[i].activeSelf) {
                normalObj[i].SetActive(false);
            }
        }
        isCoreShowed = true;
        coreTimeRemain = coreTimeMax;
        coreShowHP = nowHP;
        coreHideConditionDamage = GetCoreHideConditionDamage();
        EmitEffect(effectShowCore);
    }
    
    protected override void BattleEnd() {
        base.BattleEnd();
        if (isReaper && StageManager.Instance) {
            StageManager.Instance.DefeatReaper();
        }
    }

    protected override void SetLevelModifier() {
        dropItem[0] = dropKeyID;
        SetDropRate(10000);
    }

    public override int GetStealItemID() {
        return stealedItemID;
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        knockRemain = knockEndurance;
        if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU) {
            weakProgress = (nowHP <= GetMaxHP() / 3 ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 0);
        } else {
            weakProgress = (nowHP <= GetMaxHP() / 2 ? 1 : 0);
        }
        if (state == State.Chase) {
            maxSpeed = 25f;
            acceleration = 100f;
        } else if (weakProgress >= 2) {
            maxSpeed = 9f;
            acceleration = 18f;
        } else {
            maxSpeed = walkSpeed;
            acceleration = 18f;
        }
        if (!isStarted && target) {
            isStarted = true;
            BattleStart();
            attackedTimeRemain = 5f;
        }
        if (isCoreShowed && state != State.Dead) {
            coreTimeRemain -= deltaTimeCache * (coreTimeRemain >= 1f ? CharacterManager.Instance.riskyIncSqrt : 1f);
            if (!healEffectEmitted && coreTimeRemain < 1) {
                EmitEffect(effectHideCore);
                healEffectEmitted = true;
            }
            if (coreTimeRemain < 0 && state != State.Dead) {
                HideCore();
            }
        }
        if (state != State.Attack) {
            if (isWalking && targetTrans) {
                float sqrDist = GetTargetDistance(true, true, false);
                if (sqrDist >= 15f * 15f) {
                    isWalking = false;
                }
            }
        }
        if (knockItemTimeInterval > 0f) {
            knockItemTimeInterval -= deltaTimeCache;
        }
        if (state == State.Dead) {
            if (jibakuTimer < 4.0f) {
                jibakuTimer += deltaTimeCache;
                if (jibakuTimer >= 4.0f) {
                    attackPowerMultiplier = 10f;
                    knockPowerMultiplier = 10f;
                    throwing.ThrowStart(1);
                    BootDeathEffect(ed);
                    for (int i = 0; i < silhouetteRenderer.Length; i++) {
                        if (silhouetteRenderer[i]) {
                            silhouetteRenderer[i].enabled = false;
                        }
                    }
                    coreTimeRemain = -1f;
                    HideCore();
                    for (int i = 0; i < searchTarget.Length; i++) {
                        if (searchTarget[i] && searchTarget[i].activeSelf) {
                            searchTarget[i].SetActive(false);
                        }
                    }
                    if (centerPivot) {
                        centerPivot.gameObject.SetActive(false);
                    }
                    deathEffectEnabled = false;
                    if (effect[effectDeadSave].instance) {
                        Destroy(effect[effectDeadSave].instance);
                    }
                }
            }
        }
    }

    protected override void Update_MoveControl_ChildAgentSpeed() {
        if (isWalking) {
            float speedTemp = GetMaxSpeed(true);
            if (agent.speed != speedTemp) {
                agent.speed = speedTemp;
            }
        } else {
            base.Update_MoveControl_ChildAgentSpeed();
        }
    }

    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        if (!isCoreShowed && state != State.Dead) {
            ShowCore();
            EmitEffect(effectFindItem);
            GiveItem(stealedItemID);
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        if (isCoreShowed && coreTimeRemain > 1.25f) {
            coreTimeRemain = 1.25f;
        }
        if (knockItemTimeInterval <= 0f) {
            EmitEffect(effectFindItem);
            GiveItem(knockDownItemID);
            knockItemTimeInterval = 1.5f;
        }
    }
    
    public override float GetKnocked() {
        return base.GetKnocked() * (!(state == State.Damage && isDamageHeavy) && isCoreShowed && nowHP <= GetCoreHideBorder() ? 0 : 1);
    }

    public override void TakeDamageFixKnock(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (damage > 0) {
            damage = Mathf.Max(damage / 2, 1);
        }
        base.TakeDamageFixKnock(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    protected override void Attack() {
        base.Attack();
        float spRate = 50f / 75f;
        float intervalTemp = (weakProgress >= 2 ? 8f : weakProgress == 1 ? 10f : 12f);
        attackedTimeRemainOnDamage = intervalTemp * CharacterManager.Instance.riskyDecrease;
        AttackBase(0, 0f, 0f, 0f, 75f / 30f, 75f / 30f + GetAttackInterval(intervalTemp), 0f, spRate, false);
        if (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.EnemyCount() < 100) {
            SummonSpecificEnemy(StageManager.Instance.dungeonController.GetRespawnPosClosest(trans.position));
            if (weakProgress >= 1) {
                SummonSpecificEnemy(StageManager.Instance.dungeonController.GetRespawnPosTag());
            }
            if (weakProgress >= 2) {
                SummonSpecificEnemy(StageManager.Instance.dungeonController.GetRespawnPosTag());
            }
            summonCount++;
        }
        throwing.ThrowReady(0);
    }

    void ChangeFaceCry() { }

    void SummonSpecificEnemy(Vector3 position) {
        EnemyBase eBaseTemp = StageManager.Instance.dungeonController.SummonSpecificEnemy(summonID[summonCount % summonID.Length], summonLevel[summonCount % summonLevel.Length], position);
        if (eBaseTemp) {
            eBaseTemp.SetHomoParent(this);
        }
    }

    protected override void Start_Process_Dead() {
        base.Start_Process_Dead();
        EmitEffect(effectDeadSave);
        if (BGM.Instance) {
            BGM.Instance.StopFade(2f);
        }
    }

    protected override void DeadProcess() {
        TrophyManager.Instance.CheckTrophy(TrophyManager.t_DefeatHomoSapiens, true);
        base.DeadProcess();
    }

}
