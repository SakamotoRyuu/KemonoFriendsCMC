using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_SandCat : PlayerController {
    
    private const int findMax = 3;
    private const float findRate = 60f;
    private const int effFindSucceed = 0;
    private const int effFindFailed = 1;
    private float findPlusTime;
    private float skillPercentage = 0f;

    const float hateConditionDistance = 10f;

    protected override void Awake() {
        base.Awake();
        moveCost.attack = 10f / 1.2f * staminaCostRate;
        moveCost.skill = 80;
        sandstarHealMultiplier = justDodgeAmountMultiplier = 1.02f;
        if (attackPower > 0)
        {
            sandstarHealMultiplier *= 10f / attackPower;
        }
    }
    
    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (GetCanControl() && targetTrans && GetTargetDistance(true, false, true) < 15f * 15f) {
            findPlusTime = Mathf.Clamp(findPlusTime + deltaTimeCache, 0f, 100f);
            skillPercentage = Mathf.Clamp(skillPercentage + deltaTimeMove * 100f, 0f, 100f);
        }
        if (CharacterManager.Instance)
        {
            CharacterManager.Instance.SetSkillPercentage((int)skillPercentage);
        }
    }

    public override void ResetStatus() {
        base.ResetStatus();
        findPlusTime = 0f;
        skillPercentage = 0f;
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
                }
            } else if (rand < 38) {
                if (isBossFloor || StageManager.Instance.IsWithoutBringItemStage) {
                    GameObject itemPrefab = ItemDatabase.Instance.GetItemPrefab(Random.Range(0, 2) == 0 ? (int)ItemDatabase.ItemID.HealStar : (int)ItemDatabase.ItemID.SandstarBlock);
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

    protected override void AttackBody() {
        float spRate = (isSuperman ? 4f / 3f : 1f) * 1.2f;
        if (groundedFlag && skillPercentage >= 100 && playerInput.GetButton(RewiredConsts.Action.Special) && JudgeStamina(GetCost(CostType.Skill))) {
            if (AttackBase(2, 1f, 1f, GetCost(CostType.Skill), 80f / 30f / spRate, 80f / 30f / spRate, 0, spRate, false)) {
                attackProcess = 0;
                skillPercentage = 0;
                isSkillAttacking = true;
                attackingMoveReservedTimer = 70f / 30f / spRate;
                attackingDodgeReservedTimer = 62f / 30f / spRate;
                AttackStartAir();
            }
        } else {
            if (AttackBase(attackProcess, 1f, 1f, GetCost(CostType.Attack), 10f / 30f / spRate, 10f / 30f / spRate, 1f, spRate)) {
                S_ParticlePlay(attackProcess);
                if (groundedFlag && !playerInput.GetButton(RewiredConsts.Action.Dodge))
                {
                    SpecialStep(0.4f, 0.25f / spRate, 4f, 0f, 0.4f, true, true, EasingType.SineInOut, true);
                }
                attackProcess = (attackProcess + 1) % 2;
                isSkillAttacking = false;
                attackingMoveReservedTimer = 7f / 30f / spRate;
                AttackStartAir();
                nowSpeed = Mathf.Min(nowSpeed, 9f);
            }
        }
    }

    protected override void AttackContinuous()
    {
        base.AttackContinuous();
        AttackContinuousAir();
    }

}
