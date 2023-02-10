using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Otter : FriendsBase {
    
    private const float baseSpeed = 1.9f;
    private bool trophyChecked;

    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            // moveCost.attack = 12;
            moveCost.attack = 60f / baseSpeed * staminaCostRate;
        }
    }
    
    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (actDistNum == 0 && !JudgeStamina(GetCost(CostType.Attack))) {
            actDistNum = 1;
        } else if (actDistNum == 1 && nowST >= GetMaxST() * staminaBorder) {
            actDistNum = 0;
        }
        if (!trophyChecked && state == State.Attack && throwing.throwSettings[3].instance) {
            trophyChecked = true;
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_Otter, true);
        }
    }

    public override void ChangeActionDistance(bool isBossBattle) {
        if (!isBossBattle) {
            agentActionDistance[0].attack.y = 12f;
            agentActionDistance[0].chase.y = 15f;
        } else {
            agentActionDistance[0].attack.y = 20f;
            agentActionDistance[0].chase.y = 30f;
        }
    }

    protected override void Attack() {
        base.Attack();
        float spRate = (isSuperman ? 4f / 3f : 1f) * baseSpeed;
        if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 60f / 30f / spRate, 60f / 30f / spRate, 0f, spRate)) {
            if (targetTrans && GetTargetDistance(true, true, true) < 4.5f * 4.5f) {
                fbStepMaxDist = 2.5f;
                fbStepTime = 20f / 30f / spRate;
                SeparateFromTarget(4.5f);
            }
        }
    }
    
}
