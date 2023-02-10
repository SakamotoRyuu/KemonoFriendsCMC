using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Beaver : FriendsBase {

    bool trophyChecked;
    
    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            // moveCost.attack = 14;
            moveCost.attack = 51f * staminaCostRate;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (actDistNum == 0 && !JudgeStamina(GetCost(CostType.Attack))) {
            actDistNum = 1;
        } else if (actDistNum == 1 && nowST >= GetMaxST() * staminaBorder) {
            actDistNum = 0;
        }
        if (!trophyChecked && state == State.Attack && throwing.throwSettings[4].instance) {
            trophyChecked = true;
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_Beaver, true);
        }
    }

    public override void ChangeActionDistance(bool isBossBattle) {
        if (!isBossBattle) {
            agentActionDistance[0].attack.y = 15f;
        } else {
            agentActionDistance[0].attack.y = 20f;
        }
    }

    protected override void Attack() {
        base.Attack();
        float spRate = isSuperman ? 4f / 3f : 1;
        if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 51f / 30f / spRate, 51f / 30f / spRate, 0f, spRate)) {
            if (targetTrans && GetTargetDistance(true, true, true) < 6f * 6f) {
                fbStepMaxDist = 4f;
                fbStepTime = 18f / 30f / spRate;
                SeparateFromTarget(6f);
            }
        }
    }    

}
