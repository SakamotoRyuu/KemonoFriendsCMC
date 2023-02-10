using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friends_Kaban : FriendsBase {
    
    protected override void Awake() {
        base.Awake();
        if (animatorForBattle) {
            attackedTimeRemainOnDamage = 82f / 30f / 2f;
            moveCost.attack = 90;
            belligerent = false;
            separateFearRate = 8f;
        }
    }    

    public override void ChangeActionDistance(bool isBossBattle) {
        if (!isBossBattle) {
            agentActionDistance[0].attack.y = 15f;
        } else {
            agentActionDistance[0].attack.y = 25f;
        }
    }

    protected override bool AttackConditionAdditive() {
        if (searchArea[0].GetThisIsFound()) {
            return true;
        } else {
            attackedTimeRemain = 0.25f;
        }
        return false;
    }

    protected override void Attack() {
        base.Attack();
        float spRate = (isSuperman ? 4f / 3f : 1f) * 2f;
        throwing.target = targetTrans;
        if (AttackBase(0, 1f, 1f, GetCost(CostType.Attack), 82f / 30f / spRate, (82f / 30f + 3f) / spRate, 0f, spRate)) {
            if (targetTrans && GetTargetDistance(true, true, true) < 4.5f * 4.5f) {
                fbStepMaxDist = 2.5f;
                fbStepTime = 30f / 30f / spRate;
                SeparateFromTarget(4.5f);
            }
        }
    }

}
