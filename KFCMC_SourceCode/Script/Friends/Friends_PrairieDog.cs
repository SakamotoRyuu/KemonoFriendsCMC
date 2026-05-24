using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_PrairieDog : FriendsBase {

    private bool bossBattleSave;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            moveCost.attack = 50;
            separateFearRate = 8f;
        }
    }
    
    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (bossBattleSave != CharacterManager.Instance.isBossBattle) {
            bossBattleSave = CharacterManager.Instance.isBossBattle;
            if (!bossBattleSave) {
                agentActionDistance[0].attack.y = 20f;
                agentActionDistance[0].chase.y = 20f;
            } else {
                agentActionDistance[0].attack.y = 30f;
                agentActionDistance[0].chase.y = 30f;
            }
        }
        if (actDistNum == 0 && !JudgeStamina(GetCost(CostType.Attack))) {
            actDistNum = 1;
        } else if (actDistNum == 1 && nowST >= GetMaxST() * staminaBorder) {
            actDistNum = 0;
        }
    }

    protected override void Attack() {
        base.Attack();
        float spRate = isSuperman ? 4f / 3f : 1;
        AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 66f / 30f / spRate, (66f / 30f + 1f) / spRate, 0, spRate, false);
    }
}
