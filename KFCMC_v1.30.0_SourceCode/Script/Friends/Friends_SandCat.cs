using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_SandCat : FriendsBase {
    
    private float modeTimeRemain;
    private int nowMode;
    private string[] chatKey_Special = new string[2];
    private const float modeTimeMin = 6;
    private const float modeTimeMax = 12;
    private const int findMax = 3;
    private const float findRate = 60f;
    private const int effFindSucceed = 0;
    private const int effFindFailed = 1;
    private float findPlusTime;
    private float foundSerifTimeRemain;

    const float hateConditionDistance = 10f;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            SetMode(0, Random.Range(modeTimeMin, modeTimeMax));
            // moveCost.attack = 6;
            moveCost.attack = 10f / 1.2f * staminaCostRate;
            moveCost.skill = 80;
            chatAttackCount = 6;
        }
    }

    protected override void ChatKeyInit() {
        base.ChatKeyInit();
        chatKey_Special[0] = StringUtils.Format("{0}{1}_SPECIAL_00", talkHeader, talkName);
        chatKey_Special[1] = StringUtils.Format("{0}{1}_SPECIAL_01", talkHeader, talkName);
    }

    protected void SetMode(int newMode, float modeTimeRemain) {
        if (newMode == 1 && CharacterManager.Instance.sandcatFindNum >= findMax) {
            if (nowMode == 2) {
                newMode = 0;
            } else {
                newMode = Random.Range(0, 2) * 2;
            }
            if (foundSerifTimeRemain <= 0f && targetTrans && newMode == 0) {
                SetChat("TALK_SANDCAT_FOUND_01", 15, 3);
                foundSerifTimeRemain = 12f;
            }
        }
        nowMode = newMode;
        this.modeTimeRemain = modeTimeRemain;
        belligerent = (nowMode == 2);
        actDistNum = nowMode;
        if (nowMode == 2) {
            mesAtkMin = 0;
            mesAtkMax = 2;
            moveCost.attack = 8;
        } else {
            mesAtkMin = 3;
            mesAtkMax = 5;
            moveCost.attack = moveCost.skill;
        }
    }
    
    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (nowMode == 2 && !JudgeStamina(GetCost(CostType.Attack))) {
            SetMode(0, Random.Range(modeTimeMin, modeTimeMax) * 0.5f);
        }
        if (nowMode == 0 && GetCanControl() && targetTrans && GetTargetDistance(true, false, true) < 15f * 15f) {
            findPlusTime = Mathf.Clamp(findPlusTime + deltaTimeCache, 0f, 100f);
        }
        if (foundSerifTimeRemain > 0f) {
            foundSerifTimeRemain -= deltaTimeCache;
        }
    }

    public override void ResetGuts() {
        base.ResetGuts();
        findPlusTime = 0f;
    }

    public override void ChangeActionDistance(bool isBossBattle) {
        if (!isBossBattle) {
            agentActionDistance[1].attack.y = 15f;
        } else {
            agentActionDistance[1].attack.y = 30f;
        }
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        modeTimeRemain -= deltaTimeCache;
        if (modeTimeRemain <= 0) {
            SetMode(Random.Range(0, nowMode != 2 ? 3 : 2), Random.Range(modeTimeMin, modeTimeMax));
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        if (nowMode != 2) {
            SetMode(2, Random.Range(modeTimeMin, modeTimeMax) * 1.5f);
        }
    }

    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        if (nowMode != 2 && Random.Range(0, 100) < 50) {
            SetMode(2, Random.Range(modeTimeMin, modeTimeMax) * 0.75f);
        }
    }

    void FindItem() {
        if (!isItem && StageManager.Instance && CharacterManager.Instance.sandcatFindNum < findMax && Random.Range(0f, 100f) < findRate + findPlusTime) {
            bool succeed = false;
            bool isBossFloor = (StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.isBossFloor);
            int rand = Random.Range(isBossFloor ? 8 : 0, 100);
            if (StageManager.Instance.IsHomeStage) {
                rand = 0;
            }
            if (rand < 8) {
                if (StageManager.Instance.dungeonController) {
                    if (StageManager.Instance.IsHomeStage) {
                        EnemyBase eTemp = StageManager.Instance.dungeonController.SummonSpecificEnemy(0, 1, trans.position);
                        if (eTemp) {
                            Enemy_WingBolt wingbolt = eTemp.GetComponent<Enemy_WingBolt>();
                            if (wingbolt) {
                                wingbolt.SetForSecret();
                            }
                        }
                    } else {
                        StageManager.Instance.dungeonController.SpawnEnemyPos(trans.position, 0f, 1);
                    }
                    EmitEffect(effFindFailed);
                    SetChat(chatKey_Special[Random.Range(0, 2)], 14);
                }
            } else if (rand < 38) {
                if (isBossFloor || StageManager.Instance.IsRandomStage) {
                    GameObject itemPrefab = ItemDatabase.Instance.GetItemPrefab(Random.Range(0, 2) == 0 ? healStarID : sandstarBlockID);
                    if (itemPrefab != null) {
                        succeed = true;
                        Instantiate(itemPrefab, trans.position, quaIden);
                    }
                } else {
                    succeed = true;
                    int goldCount = Random.Range(2, 5);
                    Vector2 randCircle;
                    for (int i = 0; i < goldCount; i++) {
                        randCircle = Random.insideUnitCircle;
                        Instantiate(ItemDatabase.Instance.GetGoldPrefab(StageManager.Instance.GetGoldRank()), trans.position + new Vector3(randCircle.x, 0, randCircle.y), quaIden);
                    }
                }
            } else {
                int containerRank = StageManager.Instance.GetContainerRankForSandCat();
                if (containerRank >= 0) {
                    int itemID = ContainerDatabase.Instance.GetIDSingle(containerRank);
                    if (itemID >= 0) {
                        succeed = true;
                        bool isParenting = (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.itemSettings.container);
                        int replaceLevel = (StageManager.Instance && StageManager.Instance.dungeonController ? StageManager.Instance.dungeonController.itemReplaceLevel : 0);
                        ItemDatabase.Instance.GiveItem(itemID, trans.position, 5f, -1f, -1f, 0f, isParenting ? StageManager.Instance.dungeonController.itemSettings.container : null, replaceLevel);      
                    }
                }
            }
            if (succeed) {
                findPlusTime = 0f;
                CharacterManager.Instance.sandcatFindNum += 1;
                EmitEffect(effFindSucceed);
                if (CharacterManager.Instance.sandcatFindNum >= findMax) {
                    SetChat("TALK_SANDCAT_FOUND_00", 35, 3);
                    SetMode(0, Random.Range(modeTimeMin, modeTimeMax));
                    foundSerifTimeRemain = 12f;
                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_SandCat, true);
                }

                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                EnemyBase enemyBaseTemp;
                float amount = CharacterManager.Instance.GetNormalKnockAmount();
                for (int i = 0; i < enemies.Length; i++) {
                    enemyBaseTemp = enemies[i].GetComponent<EnemyBase>();
                    if (enemyBaseTemp) {
                        if (enemyBaseTemp.isBoss) {
                            enemyBaseTemp.RegisterTargetHate(this, amount * 4f);
                        } else if ((enemies[i].transform.position - trans.position).sqrMagnitude <= hateConditionDistance * hateConditionDistance) {
                            enemyBaseTemp.RegisterTargetHate(this, amount);
                        }
                    }
                }

            }
        }
    }

    protected override void Attack() {
        base.Attack();
        float spRate = (isSuperman ? 4f / 3f : 1f) * 1.2f;
        if (nowMode == 1 && JudgeStamina(GetCost(CostType.Skill))) {
            if (AttackBase(2, 1f, 1f, GetCost(CostType.Skill), 80f / 30f / spRate, (80f / 30f + 1f) / spRate, 0, spRate, false)) {
                attackProcess = 0;
            }
        } else if (nowMode == 2) {
            if (AttackBase(attackProcess, 1f, 1f, GetCost(CostType.Attack), 10f / 30f / spRate, 10f / 30f / spRate, 1f, spRate)) {
                S_ParticlePlay(attackProcess);
                SpecialStep(0.4f, 0.25f / spRate, 4f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                attackProcess = (attackProcess + 1) % 2;
            }
        }
    }

}
