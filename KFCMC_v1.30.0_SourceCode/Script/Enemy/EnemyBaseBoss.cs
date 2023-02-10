using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseBoss : EnemyBase {
    
    [System.Serializable]
    public class ChimeraHeal {
        public int conditionDifficulty;
        public int conditionBuffCount;
        public bool isSandstarRawOnly;
        public float healRate;
        public GameObject effectPrefab;
    }

    public enum WeakPointType { StandingLight, StandingHeavy, KnockingDown, CoreShowing, Other };

    [System.Serializable]
    public class WeakPoint {
        public Transform pivot;
        public WeakPointType type;
        public int prefabIndex;
        public bool used;
        public GameObject instance;
    }

    public int bgmNumberBattle;
    public ChimeraHeal chimeraHeal;
    public WeakPoint[] weakPoints;

    protected bool winActionGravityZero = false;
    protected float winActionForceGroundedTimePlus = 0f;
    protected float targetIsNotPlayerTime = 0f;
    protected bool retargetingToPlayer = true;
    protected float retargetingConditionTime = 8f;
    protected float retargetingDecayMultiplier = 1.6f;
    protected bool sandstarRawEnabled = false;
    protected float sandstarRawKnockEndurance = 5000;
    protected float sandstarRawKnockEnduranceLight = 5000;
    protected float sandstarRawFriendsKnockRate = 0.08333333f;
    protected float sandstarRawLockonSpeed = 15f;
    protected float sandstarRawMaxSpeed = 13.5f;
    protected float sandstarRawAcceleration = 27f;
    protected int weakProgress;
    protected bool isCoreShowed;
    protected int coreShowHP;
    protected int coreHideConditionDamage;
    protected float coreHideDenomi = 5f;
    protected float coreTimeRemain;
    protected float coreTimeMax;
    protected bool supermanEffectActivated;
    protected bool isLastOne = true;
    protected bool bgmReplayOnEnd = true;
    protected bool winActionEnabled = true;
    protected bool changeMusicEnabled = true;
    protected float chimeraHealReserveTimeRemain;
    protected float chimeraHealProcessTimeRemain;
    protected float chimeraHealMultiplier = 1f;
    protected bool battleStarted;
    protected int blackMinmiIDRank;
    protected bool blackMinmiChecked;

    protected override void Awake() {
        base.Awake();
        defStats.knockRecovery = knockRecovery = 0f;
        expForce = true;
        isBoss = true;
        killByPlayerOnly = true;
        watchoutTime = float.MaxValue;
        destinationUpdateInterval = 0.2f;
        roveInterval = -1;
        angryFixTime = 0.6f;
        stealedMax = 4;
        sickHealSpeed = 2;
        attackedTimeRemainReduceOnAngry = 1.5f;
        retargetingToPlayer = true;
        mapChipSize = 2;
        supermanEffectNumber = (int)EffectDatabase.id.enemyYK_Boss;
        supermanAuraNumber = (int)EffectDatabase.id.enemyYK_AuraBig;
    }

    public void SetAgentTypeHumanoid() {
        if (agent) {
            GameObject cellienObj = Instantiate(CharacterDatabase.Instance.GetEnemy(0));
            if (cellienObj) {
                UnityEngine.AI.NavMeshAgent agentTemp = cellienObj.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agentTemp) {
                    agent.agentTypeID = agentTemp.agentTypeID;
                    agent.radius = agentTemp.radius;
                    agent.height = agentTemp.height;
                }
                Destroy(cellienObj);
            }
        }
    }

    protected int GetCoreHideBorder() {
        if (coreTimeMax > 1f) {
            return coreShowHP - (int)Mathf.Lerp(coreHideConditionDamage * 0.6666667f, coreHideConditionDamage * 1.333333f, Mathf.Clamp01(coreTimeRemain * 1.2f / coreTimeMax));
        }
        return coreShowHP;
    }

    protected int GetCoreHideConditionDamage() {
        if (coreHideDenomi > 1f) {
            return coreHideConditionDamage = (int)(GetMaxHP() / (coreHideDenomi * CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.CoreHideRate)));
        }
        return 0;
    }

    protected override void LoadEnemyCanvas() {
        base.LoadEnemyCanvas();
        enemyCanvas.transform.GetChild((int)EnemyCanvasChild.back).gameObject.SetActive(false);
        enemyCanvas.transform.GetChild((int)EnemyCanvasChild.hpDiff).gameObject.SetActive(false);
        enemyCanvas.transform.GetChild((int)EnemyCanvasChild.hpFill).gameObject.SetActive(false);
    }

    public virtual void SetSandstarRaw() {
        if (!sandstarRawEnabled && state != State.Dead) {
            level = isForAmusement ? CharacterDatabase.amusementBossLevel : CharacterDatabase.sandstarRawLevel;
            if (level <= CharacterDatabase.Instance.enemy[enemyID].maxLevel) {
                maxHP = CharacterDatabase.Instance.enemy[enemyID].status[level].hp;
                attackPower = CharacterDatabase.Instance.enemy[enemyID].status[level].attack;
                defensePower = CharacterDatabase.Instance.enemy[enemyID].status[level].defense;
                exp = CharacterDatabase.Instance.enemy[enemyID].status[level].exp;
            }
            nowHP = GetMaxHP();
            knockRemain = knockEndurance = sandstarRawKnockEndurance;
            knockRemainLight = knockEnduranceLight = sandstarRawKnockEnduranceLight;
            attackLockonDefaultSpeed = sandstarRawLockonSpeed;
            friendsKnockRate = sandstarRawFriendsKnockRate;
            costKnockedBase = 12;
            if (!isForAmusement) {
                maxSpeed = sandstarRawMaxSpeed;
                acceleration = sandstarRawAcceleration;
                if (agent) {
                    agent.speed = maxSpeed;
                    agent.acceleration = acceleration;
                }
            }
            if (!isForAmusement) {
                SetSupermanEffect(true);
                SupermanSetObj(true);
                SupermanSetMaterial(true);
            }
            sandstarRawEnabled = true;
            if (coreTimeRemain > 0.01f) {
                coreTimeRemain = 0.01f;
            }
            weakProgress = 0;
            coreShowHP = GetMaxHP();
            coreHideConditionDamage = 0;
            for (int i = 0; i < weakPoints.Length; i++) {
                if (weakPoints[i].instance) {
                    Destroy(weakPoints[i].instance);
                }
            }
        }
    }

    public override void SetForAmusement(Amusement_Mogisen amusement) {
        base.SetForAmusement(amusement);
        SetSandstarRaw();
        sandstarRawEnabled = false;
        coreHideDenomi *= 0.25f;
    }

    protected override void OnEnable() {
        base.OnEnable();
        if (sandstarRawEnabled) {
            SupermanSetObj(true);
            SupermanSetMaterial(true);
        }
    }

    protected override void Start() {
        base.Start();
        if (!isItem) {
            if (CharacterManager.Instance) {
                CharacterManager.Instance.multiBossCount++;
                if (!isForAmusement) {
                    CharacterManager.Instance.BossTimeInit();
                }
            }
            if (StageManager.Instance) {
                if (StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.bossMusicNumber >= 0) {
                    bgmNumberBattle = StageManager.Instance.dungeonController.bossMusicNumber;
                }
                if (agent && agent.agentTypeID != 0) {
                    StageManager.Instance.SetAdditiveNavMesh(agent.agentTypeID);
                }
            }
        }
    }

    protected override void Start_Process_Dead() {
        CharacterManager.Instance.multiBossCount--;
        isLastOne = (CharacterManager.Instance.multiBossCount <= 0 && !isForAmusement);
        base.Start_Process_Dead();
        if (!isForAmusement) {
            CharacterManager.Instance.BossTimeEnd();
        }
    }

    protected virtual void BattleStart() {
        if (changeMusicEnabled && !isForAmusement) {
            BGM.Instance.Play(bgmNumberBattle);
            Ambient.Instance.Play(-1);
        }
        roveInterval = 8f;
        CharacterManager.Instance.SetBossHP(this);
        if (!isForAmusement) {
            CharacterManager.Instance.BossTimeStart();
            CharacterManager.Instance.InitBossResult();
        }
        battleStarted = true;
    }

    protected virtual void BattleEnd() {
        CharacterManager.Instance.SetBossHP(null);
        if (isLastOne) {
            if (bgmReplayOnEnd) {
                StageManager.Instance.PlayBGM();
            }
            if (winActionEnabled) {
                CharacterManager.Instance.SetAllFriendsWinAction(winActionGravityZero, winActionForceGroundedTimePlus);
            }
            if (sandstarRawEnabled) {
                CharacterManager.Instance.sandstarRawActivated -= 1;
                if (CharacterManager.Instance.sandstarRawActivated <= 0) {
                    StageManager.Instance.DestroyReaperEffect();
                }
            }
            StageManager.Instance.defeatBossSave = StageManager.Instance.stageNumber;
            EndMessage();
        } else {
            GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemyObjects.Length > 0) {
                for (int i = 0; i < enemyObjects.Length; i++) {
                    EnemyBaseBoss bossBase = enemyObjects[i].GetComponent<EnemyBaseBoss>();
                    if (bossBase != null && bossBase != this) {
                        bossBase.SetBossHPExternal();
                        break;
                    }
                }
            }
        }
    }

    public void SetBossHPExternal() {
        if (CharacterManager.Instance) {
            CharacterManager.Instance.SetBossHP(this);
        }
    }

    protected void EndMessage() {
        CharacterManager.Instance.SetEndBossMessage(sandstarRawEnabled);
        CharacterManager.Instance.ShowBossResult(enemyID, sandstarRawEnabled);
    }

    protected override void SetSupermanEffect(bool flag = true) {
        if (flag) {
            if (!supermanEffectActivated) {
                base.SetSupermanEffect(flag);
                supermanEffectActivated = true;
            }
        } else {
            base.SetSupermanEffect(flag);
        }
    }

    public override void SetForDictionary(bool toSuperman, int layer) {
        base.SetForDictionary(toSuperman, layer);
        supermanEffectActivated = toSuperman;
    }

    protected override void DeadProcess() {
        if (!isLastOne) {
            dropItem[0] = -1;
            SetDropRate(0);
        }
        BattleEnd();
        base.DeadProcess();
        if (sandstarRawEnabled) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_DefeatSinWR, true);
            if (GameManager.Instance.GetDefeatSinWRComplete()) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_DefeatAllSinWR, true);
            }
        }
        if (isLastOne && StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.bossMinmiBlackFlag) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_BlackMinmiBoss, true);
        }
    }

    protected void ResetTargetToPlayer() {
        if (target && searchArea.Length > 0) {
            if (targetTrans == CharacterManager.Instance.playerSearchTarget) {
                targetIsNotPlayerTime -= deltaTimeCache * retargetingDecayMultiplier;
            } else {
                targetIsNotPlayerTime += deltaTimeCache;
            }
            targetIsNotPlayerTime = Mathf.Clamp(targetIsNotPlayerTime, 0f, retargetingConditionTime + 0.25f);
            if (GetCanControl() && targetIsNotPlayerTime >= retargetingConditionTime) {
                targetIsNotPlayerTime = Mathf.Max(retargetingConditionTime - 0.5f, 0f);
                if (targetHateEnabled) {
                    float paramMax = 0f;
                    int playerID = CharacterManager.Instance.GetPlayerID();
                    bool find = false;
                    for (int i = 0; i < targetHates.Length; i++) {
                        if (targetHates[i].characterID >= 0 && targetHates[i].param > paramMax) {
                            paramMax = targetHates[i].param;
                        }
                    }
                    paramMax += CharacterManager.Instance.GetNormalKnockAmount() * (12f + CharacterManager.Instance.costSumSave);
                    if (paramMax >= 1000000) {
                        paramMax /= 8;
                        for (int i = 0; i < targetHates.Length; i++) {
                            targetHates[i].param /= 8;
                        }
                    }
                    for (int i = 0; i < targetHates.Length; i++) {
                        if (targetHates[i].characterID == playerID) {
                            find = true;
                            if (paramMax > targetHates[i].param) {
                                targetHates[i].param = paramMax;
                            }
                            break;
                        }
                    }
                    if (!find) {
                        RegisterTargetHate(CharacterManager.Instance.pCon, paramMax);
                    }
                }
                searchArea[0].SetLockTarget(CharacterManager.Instance.playerSearchTarget.gameObject);
            }
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (!blackMinmiChecked) {
            SetBlackMinmiIDRank();
        }
        if (retargetingToPlayer) {
            ResetTargetToPlayer();
        }
        if (nowHP > 0 && chimeraHeal.healRate > 0f && GameManager.Instance.save.difficulty >= chimeraHeal.conditionDifficulty && (sandstarRawEnabled || !chimeraHeal.isSandstarRawOnly)) {
            if (chimeraHealReserveTimeRemain > 0f) {
                chimeraHealReserveTimeRemain -= deltaTimeCache;
            }
            if (Time.timeScale > 0f && chimeraHealReserveTimeRemain <= 0f && nowHP <= Mathf.RoundToInt(GetMaxHP() * (1f - chimeraHeal.healRate))) {
                int buffCount = CharacterManager.Instance.GetBuffCount(true);
                if (chimeraHeal.conditionBuffCount <= 0 || buffCount >= chimeraHeal.conditionBuffCount || (CharacterManager.Instance.costSumSave > CharacterManager.riskyCost && buffCount >= chimeraHeal.conditionBuffCount - 1)) {
                    if (chimeraHeal.effectPrefab && centerPivot) {
                        Instantiate(chimeraHeal.effectPrefab, centerPivot);
                    }
                    chimeraHealReserveTimeRemain = 10f;
                    chimeraHealProcessTimeRemain = 1.5f;
                    if (chimeraHeal.conditionBuffCount > 0) {
                        chimeraHealMultiplier = buffCount * CharacterManager.Instance.riskyIncrease;
                    } else {
                        chimeraHealMultiplier = 1f;
                    }
                }
            }
            if (chimeraHealProcessTimeRemain > 0f) {
                chimeraHealProcessTimeRemain -= deltaTimeCache;
                if (chimeraHealProcessTimeRemain <= 0f) {
                    AddNowHP(Mathf.RoundToInt(GetMaxHP() * chimeraHealMultiplier * chimeraHeal.healRate), GetCenterPosition(), true, damageColor_Heal);
                }
            }
        }
        if (battleStarted && GameManager.Instance.save.difficulty <= 2 && !sandstarRawEnabled) {
            for (int i = 0; i < weakPoints.Length; i++) {
                if (weakPoints[i].instance) {
                    bool answer = false;
                    switch (weakPoints[i].type) {
                        case WeakPointType.StandingLight:
                            answer = state == State.Damage || state == State.Dead;
                            break;
                        case WeakPointType.StandingHeavy:
                            answer = (state == State.Damage && isDamageHeavy) || state == State.Dead;
                            break;
                        case WeakPointType.KnockingDown:
                            answer = state != State.Damage || state == State.Dead;
                            break;
                        case WeakPointType.CoreShowing:
                            answer = !isCoreShowed;
                            break;
                    }
                    if (answer) {
                        Destroy(weakPoints[i].instance);
                    }
                } else if (!weakPoints[i].used && weakPoints[i].pivot && weakPoints[i].pivot.gameObject.activeInHierarchy) {
                    bool answer = false;
                    switch (weakPoints[i].type) {
                        case WeakPointType.StandingLight:
                        case WeakPointType.StandingHeavy:
                            answer = GetCanControl() && stateTime > 0.02f;
                            break;
                        case WeakPointType.KnockingDown:
                            answer = state == State.Damage && isDamageHeavy;
                            break;
                        case WeakPointType.CoreShowing:
                            answer = isCoreShowed;
                            break;
                    }
                    if (answer) {
                        weakPoints[i].instance = Instantiate(CharacterManager.Instance.weakPointPrefab[weakPoints[i].prefabIndex], weakPoints[i].pivot);
                        weakPoints[i].used = true;
                    }
                }
            }
        }
    }

    public override int GetStealItemID() {
        if (stealedCount < stealedMax) {
            stealedCount++;
            return ContainerDatabase.Instance.GetIDSingle(StageManager.Instance.GetContainerRankForRaccoon(sandstarRawEnabled));
        }
        return healStarID;
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (state == State.Damage && isDamageHeavy) {
            knockAmount = 0;
        }
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
    }

    protected void SetBlackMinmiIDRank() {
        if (!blackMinmiChecked) {
            blackMinmiChecked = true;
            if (StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.bossMinmiBlackFlag) {
                blackMinmiIDRank = 0;
                GameObject[] enemyObj = GameObject.FindGameObjectsWithTag("Enemy");
                for (int i = 0; i < enemyObj.Length; i++) {
                    EnemyBaseBoss bossBaseTemp = enemyObj[i].GetComponent<EnemyBaseBoss>();
                    if (bossBaseTemp && bossBaseTemp.enemyID == enemyID && bossBaseTemp.characterId < characterId) {
                        blackMinmiIDRank++;
                    }
                }
            }
        }
    }

}
